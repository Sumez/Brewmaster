using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public abstract class PipelineOption
	{
		public abstract string TypeName { get; }
		public abstract IEnumerable<FileType> SupportedFileTypes { get; }
		public virtual IEnumerable<PipelineProperty> Properties { get { return new PipelineProperty[] {}; } }
		public override string ToString() { return Program.Language.Get(TypeName); }
		public abstract void Process(PipelineSettings settings, Action<string> output);
		public abstract PipelineSettings Create(AsmProjectFile file);
		public virtual PipelineSettings Load(AsmProject project, PipelineHeader pipelineHeader)
		{
			var pipeline = new PipelineSettings(this, null, pipelineHeader.LastProcessed);
			SetSettings(pipeline, pipelineHeader.Settings);
			pipeline.OutputFiles.AddRange(pipelineHeader.Output);
			return pipeline;
		}
		public virtual PipelineHeader Save(PipelineSettings settings)
		{
			var header = new PipelineHeader
			{
				Type = TypeName,
				LastProcessed = settings.LastProcessed,
				Output = settings.OutputFiles.ToArray()
			};
			GetSettings(settings, header.Settings);
			return header;
		}
		public virtual PipelineSettings Clone(PipelineSettings pipelineSettings)
		{
			var header = Save(pipelineSettings);
			var newPipeline = Load(pipelineSettings.File.Project, header);
			newPipeline.File = pipelineSettings.File;
			newPipeline.SettingChanged(false);
			return newPipeline;
		}

		public virtual void GetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{
			foreach (var property in Properties)
			{
				if (!settings.GenericSettings.ContainsKey(property.SystemName)) continue;
				headerSettings[property.SystemName] = settings.GenericSettings[property.SystemName];
			}
		}

		public virtual void SetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{
			foreach (var property in Properties)
			{
				settings.GenericSettings[property.SystemName] = headerSettings.ContainsKey(property.SystemName)
					? headerSettings[property.SystemName]
					: property.DefaultValue ?? "";
			}
		}
		protected PipelineSettings CreateGeneric(AsmProjectFile file, string extension)
		{
			var baseFile = file.GetRelativeDirectory(true) + Path.GetFileNameWithoutExtension(file.File.Name);
			var newPipeline = new PipelineSettings(this, file);
			newPipeline.OutputFiles.Add(baseFile + extension);
			foreach (var property in Properties)
			{
				newPipeline.GenericSettings[property.SystemName] = property.DefaultValue ?? "";
			}
			return newPipeline;
		}

		/// <summary>
		/// Allows individual processors to indicate whether it's safe to skip a file (in cases of dependency on other external resources). Return false to always force processing.
		/// </summary>
		/// <param name="filePipeline"></param>
		/// <returns></returns>
		public virtual bool CanSkip(PipelineSettings filePipeline)
		{
			return true;
		}
	}
	public enum PipelinePropertyType
	{
		Boolean, Select, Text, ProjectFile
	}
	public class PipelineProperty
	{
		public PipelineProperty(string systemName, PipelinePropertyType type, string defaultValue = null, IEnumerable<string> options = null)
		{
			SystemName = systemName;
			Type = type;
			if (options != null) Options = options.ToList();
			if (defaultValue != null) DefaultValue = defaultValue;
		}

		public PipelineProperty(string systemName, PipelinePropertyType type, bool? defaultValue = null, IEnumerable<string> options = null)
		: this(systemName, type, defaultValue.HasValue ? (defaultValue.Value ? "1" : "0") : null, options)
		{
		}

		public PipelineProperty(string systemName, PipelinePropertyType type, string defaultValue, Func<PipelineSettings, string> getFileNameFilter)
		: this(systemName, type, defaultValue)
		{
			GetFileNameFilter = getFileNameFilter;
		}

		public Func<PipelineSettings, string> GetFileNameFilter { get; set; }
		public string SystemName { get; set; }
		public PipelinePropertyType Type { get; set; }
		public List<string> Options { get; set; }
		public string DefaultValue { get; set; }
	}
}