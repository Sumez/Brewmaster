using System;
using System.Collections.Generic;
using System.IO;
using Brewmaster.ProjectModel;
using Brewsic;

namespace Brewmaster.Pipeline
{
	public class BrewsicModulePipeline : PipelineOption
	{
		public override string TypeName => "brewsic.module";

		public override IEnumerable<FileType> SupportedFileTypes => new[] { FileType.Audio };

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".bin");
		}
		public override void Process(PipelineSettings settings, Action<string> output)
		{
			var compressPatterns = settings.GenericSettings.GetBoolean("compress-patterns");
			var maxSampleGrowth = settings.GenericSettings.GetInt("max-sample-growth");
			var maxSampleSize = string.IsNullOrWhiteSpace(settings.GenericSettings["max-sample-size"])
				? 0
				: settings.GenericSettings.GetInt("max-sample-size");

			var modules = new List<ItModule> { ItModule.LoadFromFile(settings.File.File.FullName) };
			output(string.Format("Converting song \"{0}\" to Brewsic music format", modules[0].Title));
			var musicBank = ModuleConverter.GetBrewsicMusicDump(modules, output, maxSampleGrowth, compressPatterns, maxSampleSize);

			using (var bankStream = File.Create(settings.GetFilePath(0)))
			{
				bankStream.Write(musicBank, 0, musicBank.Length);
				bankStream.Close();
			}
		}

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("compress-patterns", PipelinePropertyType.Select, "1", new[] {"1", "0"}),
					new PipelineProperty("max-sample-size", PipelinePropertyType.Text, ""),
					new PipelineProperty("max-sample-growth", PipelinePropertyType.Text, "500")
				};
			}
		}
	}
}
