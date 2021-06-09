using System;
using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public abstract class PipelineOption
	{
		public abstract string TypeName { get; }
		public override string ToString() { return Program.Language.Get(TypeName); }
		public abstract void Process(PipelineSettings dataPipelineSettings);
		public abstract PipelineSettings Create(AsmProjectFile file);
		public abstract PipelineSettings Load(AsmProject project, PipelineHeader pipelineHeader);
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
			newPipeline.SettingChanged();
			return newPipeline;
		}

		public virtual void GetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{

		}

		public virtual void SetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{
			
		}
	}

	public class PipelineSettings
	{
		public static readonly List<PipelineOption> PipelineOptions = new List<PipelineOption>
		{
			new TilemapAsmPipeline(),
			new TilemapBinaryPipeline(),
			new ChrPipeline()
		};


		// DATA ESSENTIAL TO OUR STORED FILE
		public AsmProjectFile File { get; set; }
		public virtual List<string> OutputFiles { get; } = new List<string>();
		public DateTime? LastProcessed { get; protected set; }
		public PipelineOption Type { get; }
		// END

		public PipelineSettings(PipelineOption type, AsmProjectFile file, DateTime? lastProcessed = null)
		{
			File = file;
			LastProcessed = lastProcessed;
			Type = type;
		}

		public void Process()
		{
			Type.Process(this);
			LastProcessed = DateTime.Now;
		}

		public PipelineSettings Clone()
		{
			return Type.Clone(this);
		}

		public virtual void SettingChanged()
		{
			LastProcessed = null;
		}
	}
}


