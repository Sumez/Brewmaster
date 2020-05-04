using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Brewmaster.Pipeline;

namespace Brewmaster.ProjectModel
{
	[Serializable]
	[XmlRoot("BrewmasterProject")]
	public class ProjectFileHeader
	{
		public ProjectType Type;
		public string Name;
		public BuildConfigurationHeader Cartridge;
		[XmlElement(ElementName = "BuildConfiguration")]
		public BuildConfigurationHeader[] BuildConfigurations;
		[XmlElement(ElementName = "File")]
		public ProjectFileFileHeader[] Files;
		[XmlElement(ElementName = "Directory")]
		public string[] ExtraDirectories;

		// EXPORT
		public static ProjectFileHeader GetFileHeader(AsmProject project)
		{
			var header = new ProjectFileHeader();
			var fileReferences = new Dictionary<AsmProjectFile, int>();

			header.Type = project.Type;
			header.Name = project.Name;
			header.ExtraDirectories = project.Directories.Select(d => project.GetRelativePath(d.FullName)).ToArray();

			var filesHeaders = new List<ProjectFileFileHeader>();
			for (var i = 0; i < project.Files.Count; i++)
			{
				var file = project.Files[i];

				var fileHeader = new ProjectFileFileHeader();
				fileHeader.RelativePath = project.GetRelativePath(file.File.FullName);
				fileHeader.Id = i;
				fileHeader.Mode = file.Mode;
				if (file.Pipeline != null)
				{
					var pipelineHeader = new PipelineHeader();
					if (file.Pipeline is ChrPipeline)
					{
						pipelineHeader.Type = "chr";
					}
					file.Pipeline.GetSettings(pipelineHeader.Settings);
					pipelineHeader.LastProcessed = file.Pipeline.LastProcessed;
					pipelineHeader.Output = file.Pipeline.OutputFiles.Select(project.GetRelativePath).ToArray();

					fileHeader.Pipeline = pipelineHeader;
				}

				fileReferences.Add(file, i);
				filesHeaders.Add(fileHeader);
			}

			header.Files = filesHeaders.ToArray();
			var configurations = new List<BuildConfigurationHeader>();
			foreach (var c in project.BuildConfigurations)
			{
				var buildConfigurationHeader = new BuildConfigurationHeader
				{
					Data = new[]
					{
						c.ChrBuildPath, c.PrgBuildPath, c.BuildPath, c.ChrFile,
						c.DebugFile, c.Filename, c.LinkerConfigFile, c.MapFile, c.PrgFile,
						c.Name, string.Join(",", c.Symbols), c.CalculateChecksum ? "1" : "0"
					},
					ChrBankFileIds = c.ChrBanks.Select(b =>
						b.Sources.Select(f => fileReferences[f]).ToArray()).ToArray()
				};
				if (buildConfigurationHeader.ChrBankFileIds.Length == 0) buildConfigurationHeader.ChrBankFileIds = null;
				configurations.Add(buildConfigurationHeader);
			}
			header.BuildConfigurations = configurations.ToArray();

			return header;
		}


		// IMPORT
		public void GetProjectModel(AsmProject project)
		{
			var errors = new List<string>();
			var fileReferences = new Dictionary<int, AsmProjectFile>();

			project.Type = Type;
			project.Name = Name;
			if (ExtraDirectories != null) project.Directories = ExtraDirectories
					.Select(d => new DirectoryInfo(Path.Combine(project.Directory.FullName, d)))
					.Where(d => d.Exists).ToList();
			foreach (var fileHeader in Files)
			{
				var fileInfo = new FileInfo(Path.Combine(project.Directory.FullName, fileHeader.RelativePath));
				var file = new AsmProjectFile { Project = project };

				file.File = fileInfo;
				if (!fileInfo.Exists) file.Missing = true;
				
				file.Mode = fileHeader.Mode;

				if (fileHeader.Pipeline != null)
				{
					switch (fileHeader.Pipeline.Type)
					{
						case "chr":
							// TODO: Deserialization method on the pipeline itself
							var pipeline = new ChrPipeline(file, Path.Combine(project.Directory.FullName, fileHeader.Pipeline.Output[0]), fileHeader.Pipeline.LastProcessed);
							pipeline.PaletteOutput = fileHeader.Pipeline.Output.Length < 2 || fileHeader.Pipeline.Output[1] == null ? null : Path.Combine(project.Directory.FullName, fileHeader.Pipeline.Output[1]);
							pipeline.SetSettings(fileHeader.Pipeline.Settings);
							file.Pipeline = pipeline;
							break;
					}
				}

				fileReferences.Add(fileHeader.Id, file);
				project.Files.Add(file);
			}

			var buildConfigurations = new List<BuildConfigurationHeader>();
			if (Cartridge != null) buildConfigurations.Add(Cartridge);
			if (BuildConfigurations != null) buildConfigurations.AddRange(BuildConfigurations);

			foreach (var configurationHeader in buildConfigurations)
			{
				var cData = configurationHeader.Data;
				var configuration = new NesCartridge
				{
					ChrBuildPath = cData[0],
					PrgBuildPath = cData[1],
					BuildPath = cData[2],
					ChrFile = cData[3],
					DebugFile = cData[4],
					Filename = cData[5],
					LinkerConfigFile = cData[6],
					MapFile = cData[7],
					PrgFile = cData[8],
					Name = cData.Length > 9 ? cData[9] : null,
					Symbols = cData.Length > 10 ? cData[10].Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>(),
					CalculateChecksum = cData.Length > 11 ? cData[11] != "0" : true,
				};
				if (configurationHeader.ChrBankFileIds != null)
				foreach (var bank in configurationHeader.ChrBankFileIds)
				{
					var chrBank = new ChrBank();
					foreach (var fileId in bank)
					{
						chrBank.Sources.Add(fileReferences[fileId]);
					}
					configuration.ChrBanks.Add(chrBank);
				}
				project.BuildConfigurations.Add(configuration);
			}

			if (errors.Any()) throw new Exception(string.Join(Environment.NewLine, errors));
			project.Pristine = true;
		}
	}
	
	[Serializable]
	public class ProjectFileFileHeader
	{
		public int Id;
		public string RelativePath;
		public CompileMode Mode;
		public PipelineHeader Pipeline;
	}

	[Serializable]
	public class PipelineHeader
	{
		public string Type;
		[XmlElement(ElementName = "Output")]
		public string[] Output;
		public DateTime? LastProcessed;
		[XmlElement(ElementName = "Property")]
		public Properties Settings = new Properties();
	}

	[Serializable]
	public class BuildConfigurationHeader
	{
		public string[] Data;
		public int[][] ChrBankFileIds { get; set; }
	}


	[Serializable]
	public class Properties : List<Property>
	{
		public string this[string key]    // Indexer declaration  
		{
			set
			{
				this.RemoveAll(p => p.Key == key);
				this.Add(new Property { Key = key, Value = value });
			}
			get { return this.FirstOrDefault(p => p.Key == key).Value; }
		}
	}

	[Serializable]
	public struct Property
	{
		public string Key
		{ get; set; }

		public string Value
		{ get; set; }
	}
}
