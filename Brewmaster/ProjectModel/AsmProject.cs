using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Brewmaster.EditorWindows;
using Brewmaster.Pipeline;
using Brewmaster.ProjectExplorer;
using Brewmaster.ProjectModel.Compatibility;

namespace Brewmaster.ProjectModel
{
	public enum ProjectType
	{
		Nes = 0,
		Snes = 1
	}
	public enum FileTemplate
	{
		None, Famitracker, Chr, AssemblyCode, AssemblyInclude
	}
	public class AsmProject : IDisposable, ISaveable, ISymbolSource
	{
		// DATA ESSENTIAL TO OUR STORED FILE
		public string Name { get; set; }
		public List<NesCartridge> BuildConfigurations { get; set; }
		public readonly ObservableCollection<AsmProjectFile> Files;
		public int CurrentConfigurationIndex { get; private set; }
		// END


		private FileSystemWatcher _fileSystemWatcher;
		public event Action Changed;
		public event Action ContentsChanged;
		public event Action<IEnumerable<Breakpoint>> BreakpointsChanged;
		public event Action PristineChanged;
		public Action<string, int, int> GoTo;

		public FileInfo ProjectFile { get; set; }
		public DirectoryInfo Directory { get; set; }
		private bool _pristine;
		public bool Pristine
		{
			get { return _pristine; }
			set
			{
				var updated = _pristine != value;
				_pristine = value;
				if (updated && PristineChanged != null) PristineChanged();
				if (!_pristine) UpdatedSinceLastBuild = true;
			}
		}

		public NesCartridge CurrentConfiguration
		{
			get { return BuildConfigurations[CurrentConfigurationIndex]; }
			set
			{
				if (value != null && BuildConfigurations.Contains(value)) CurrentConfigurationIndex = BuildConfigurations.IndexOf(value);
			}
		}

		public List<KeyValuePair<string, Symbol>> Symbols { get; set; }
		public ProjectType Type { get; set; }
		public Dictionary<string, DebugSymbol> DebugSymbols { get; private set; }
		public List<DirectoryInfo> Directories { get; internal set; }
		public bool UpdatedSinceLastBuild { get; set; }

		private List<Breakpoint> Breakpoints;

		private object _lockSymbols = new object();
		private Task _symbolTask;
		private Task _dbgTask;


		private AsmProject()
		{
			Type = ProjectType.Nes;
			Files = new ObservableCollection<AsmProjectFile>();
			Files.CollectionChanged += (s, e) => { if (ContentsChanged != null) ContentsChanged(); };
			Pristine = true;
			DebugSymbols = new Dictionary<string, DebugSymbol>();
			Breakpoints = new List<Breakpoint>();
			Directories = new List<DirectoryInfo>();
			BuildConfigurations = new List<NesCartridge>();
			ContentsChanged += () => Pristine = false;
			CurrentConfigurationIndex = 0;
			UpdatedSinceLastBuild = true;
		}

		public void RefreshBreakpoints()
		{
			if (BreakpointsChanged != null) BreakpointsChanged(GetAllBreakpoints());
		}
		public async Task LoadAllSymbolsAsync()
		{
			if (_symbolTask != null && !_symbolTask.IsCompleted)
			{
				await _symbolTask; // Await existing task, since cancelling it might interfer with the new task
				// TODO: deadlocks when 3 or more is started
			}
			await (_symbolTask = Task.Run(() => { LoadAllSymbols(); }));
		}

		public async Task ParseDebugDataAsync(string debugFile = null)
		{
			if (debugFile == null) debugFile = Path.Combine(Directory.FullName, CurrentConfiguration.DebugFile);
			if (_dbgTask != null && !_dbgTask.IsCompleted)
			{
				await _dbgTask; // Await existing task, since cancelling it might interfer with the new task
				// TODO: deadlocks when 3 or more is started
			}
			await (_dbgTask = Task.Run(() => { ParseDebugData(debugFile); }));
		}

		public void AwaitDebugTask()
		{
			if (_dbgTask == null) throw new Exception("Debug parsing wasn't started");
			if (_dbgTask.IsCompleted) return;
			_dbgTask.Wait();
		}

		public void ParseDebugData(string debugFile)
		{
			var headerLength = Type == ProjectType.Nes ? 16 : 0; // TODO: Base extra offset on info in the debug file
			var debugInfo = new DebugInfo();
			var symbols = new Dictionary<string, DebugSymbol>();
			var debugFileInfo = new FileInfo(debugFile);
			if (!debugFileInfo.Exists) return;
			using (var reader = debugFileInfo.OpenText())
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var matches = Regex.Match(line, @"^(\S+)\s+(.+)$");
					var data = new Dictionary<string, string>();
					if (!matches.Success) continue;

					var type = matches.Groups[1].Value;
					foreach (Match match in Regex.Matches(matches.Groups[2].Value, @"([^=]+)\=([^,=]+)(,|$)"))
					{
						data.Add(match.Groups[1].Value, match.Groups[2].Value);
					}

					switch (type)
					{
						case "file":
							debugInfo.Files.Add(int.Parse(data["id"]),
								new DebugFile { Name = new FileInfo(Path.Combine(Directory.FullName, data["name"].Trim('"'))).FullName });
							break;
						case "line":
							if (!data.ContainsKey("span")) break;
							foreach (var span in data["span"].Split('+'))
							{
								if (debugInfo.Lines.ContainsKey(int.Parse(span)))
								{
									// TODO: If span already exists but file is different? (macro)
									if (data.ContainsKey("type") && data["type"] == "2")
									{
										debugInfo.Lines.Remove(int.Parse(span));
									}
									else continue;
								}
								debugInfo.Lines.Add(int.Parse(span), debugInfo.LineIds[int.Parse(data["id"])] = new DebugLine
								{
									File = debugInfo.Files[int.Parse(data["file"])],
									Line = int.Parse(data["line"]),
								});
							}
							break;
						case "seg":
							if (!data.ContainsKey("ooffs") && !data.ContainsKey("start")) break;
							debugInfo.Segments.Add(int.Parse(data["id"]), 
								new DebugSegment
								{
									RomOffset = data.ContainsKey("ooffs") ? (int.Parse(data["ooffs"]) - headerLength) : null as int?,
									CpuAddress = data.ContainsKey("start") ? Convert.ToInt32(data["start"], 16) : null as int?
								});
							break;
						case "span":
							var spanId = int.Parse(data["id"]);
							var segId = int.Parse(data["seg"]);
							if (!debugInfo.Lines.ContainsKey(spanId)) break; // Not all spans refer to single lines
							if (!debugInfo.Segments.ContainsKey(segId)) break; // Only debug lines in PRG segments

							debugInfo.Lines[spanId].RomAddress =
								debugInfo.Segments[segId].RomOffset.HasValue
								? (debugInfo.Segments[segId].RomOffset.Value + int.Parse(data["start"]))
								: null as int?;
							debugInfo.Lines[spanId].CpuAddress =
								debugInfo.Segments[segId].CpuAddress.HasValue
								? debugInfo.Segments[segId].CpuAddress.Value + int.Parse(data["start"])
								: null as int?;
							debugInfo.Lines[spanId].Size = int.Parse(data["size"]);
							break;
						case "sym":
							if (!data.ContainsKey("val")) break;
							var name = data["name"].Trim('"');
							foreach (var definedAt in data["def"].Split('+').Select(int.Parse)) // Multiple defs when reused with a macro
							{
								var definedLine = debugInfo.LineIds.ContainsKey(definedAt) ? debugInfo.LineIds[definedAt] : null;
								if (debugInfo.Symbols.ContainsKey(name)) break; // TODO: Handle multiple/local symbols with the same name!
								debugInfo.Symbols.Add(name, new DebugSymbol
								{
									DebugLine = definedLine != null ? definedLine.Line : 0,
//									Source = definedLine != null ? definedLine.File.Name : null,
									Size = data["addrsize"] == "zeropage" ? 8 : 16,
									Value = int.Parse(data["val"].Substring(2), NumberStyles.HexNumber)
								});
							}
							break;
					}
				}

				foreach (var debugLine in debugInfo.Lines.Values)
				{
					if (!debugLine.RomAddress.HasValue && !debugLine.CpuAddress.HasValue) continue;
					if (debugLine.File.Lines.ContainsKey(debugLine.Line)) continue; // TODO: Lines covering multiple spans due to MACROs!

					debugLine.File.Lines.Add(debugLine.Line, debugLine);
				}
				foreach (var projectFile in Files)
				{
					projectFile.DebugLines = new Dictionary<int, DebugLine>();
					foreach (var file in debugInfo.Files.Values.Where(f => f.Name == projectFile.File.FullName))
					{
						if (file.Lines.Any()) projectFile.DebugLines = file.Lines;
					}
				}
				DebugSymbols = debugInfo.Symbols;
			}
		}


		private void LoadAllSymbols()
		{
			var symbols = new List<KeyValuePair<string, Symbol>>();
			foreach (var sourceFile in Files.Where(f => f.IsTextFile && f.Mode != CompileMode.ContentPipeline && f.Mode != CompileMode.LinkerConfig))
			{
				sourceFile.AddSymbolsFromFile(symbols);
			}
			lock (_lockSymbols)
			{
				Symbols = symbols;
			}
		}


		public static AsmProject LoadFromFile(string filepath)
		{
			var file = new FileInfo(filepath);
			AsmProject project;
			switch (file.Extension)
			{
				case ".bwm":
					project = DeserializeBwmFile(file);
					break;
				case ".nesproject":
					project = ImportFromFile(file);
					break;
				default:
					throw new Exception("Unrecognized file type");
			}

			project.WatchFile();
			return project;
		}

		public void RemoveBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			foreach (var breakpoint in breakpoints.ToList())
			{
				Breakpoints.Remove(breakpoint);

				if (breakpoint.File == null) continue;
				breakpoint.File.RemoveBreakpoint(breakpoint);
			}
			RefreshBreakpoints();
		}

		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			Breakpoints.Clear();
			Breakpoints.AddRange(breakpoints);
			RefreshBreakpoints();
		}
		public void AddBreakpoint(Breakpoint breakpoint)
		{
			var existingBreakpoint = Breakpoints.FirstOrDefault(bp =>
				((bp.StartAddress == breakpoint.StartAddress && bp.StartAddress >= 0) || (bp.Symbol != null && bp.Symbol == breakpoint.Symbol)) &&
				bp.EndAddress == breakpoint.EndAddress &&
				bp.CurrentLine == breakpoint.CurrentLine &&
				bp.File == breakpoint.File &&
				bp.AddressType == breakpoint.AddressType);

			if (existingBreakpoint != null)
			{
				existingBreakpoint.Disabled = false;
				existingBreakpoint.Type |= breakpoint.Type;
			}
			else Breakpoints.Add(breakpoint);
			RefreshBreakpoints();
		}

		public static AsmProject DeserializeBwmFile(FileInfo file)
		{
			var header = new ProjectFileHeader();

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(file.FullName);
			string xmlString = xmlDocument.OuterXml;

			using (var read = new StringReader(xmlString))
			{
				var serializer = new XmlSerializer(typeof(ProjectFileHeader));
				using (var reader = new XmlTextReader(read))
				{
					if (serializer.CanDeserialize(reader))
					{
						header = (ProjectFileHeader) serializer.Deserialize(reader);
					}
					else
					{
						serializer = new XmlSerializer(typeof(LegacyProjectFileHeaderV1));
						var legacyHeader = (LegacyProjectFileHeaderV1)serializer.Deserialize(reader);
						legacyHeader.FixCompatibility();
						header = legacyHeader;
					}
				}
			}

			var project = new AsmProject {Directory = file.Directory, ProjectFile = file};
			header.GetProjectModel(project);
			return project;
		}

		public static AsmProject ImportFromDirectory(DirectoryInfo parentDirectory)
		{
			var project = new AsmProject { Directory = parentDirectory };
			var fileReferences = new Dictionary<string, AsmProjectFile>();
			AddFilesFromDirectory(project, parentDirectory);

			return project;
		}

		private static void AddFilesFromDirectory(AsmProject project, DirectoryInfo directory)
		{
			foreach (var file in directory.GetFiles())
			{
				var newFile = new AsmProjectFile { Project = project, Mode = CompileMode.Ignore, File = file };
				
				if (newFile.Type == FileType.Image) newFile.Mode = CompileMode.ContentPipeline;
				if (newFile.Type == FileType.Source) newFile.Mode = CompileMode.IncludeInAssembly;

				project.Files.Add(newFile);
			}
			foreach (var subDirectory in directory.GetDirectories())
			{
				if (subDirectory.Name[0] == '.') continue;
				AddFilesFromDirectory(project, subDirectory);
			}
		}

		public static AsmProject ImportFromFile(FileInfo file)
		{
			var project = new AsmProject { Directory = file.Directory };
			var doc = new XmlDocument();
			doc.Load(file.FullName);
			var projectNode = doc.SelectSingleNode("nesicideproject");
			var properties = projectNode.SelectSingleNode("properties");
			project.Name = projectNode.Attributes["title"].Value;
			var buildConfiguration = new NesCartridge
								{
									Name = "NES",
									Filename = "obj/" + properties.Attributes["cartridgeoutputname"].Value,
									PrgFile = properties.Attributes["linkeroutputname"].Value,
									ChrFile = properties.Attributes["chrromoutputname"].Value,
									PrgBuildPath = "obj/nes",
									DebugFile = "obj/" + properties.Attributes["debuginfoname"].Value,
									ChrBuildPath = "obj/nes",
									BuildPath = "obj/nes",
									MapFile = "obj/map.txt"
			};
			if (buildConfiguration.Filename.EndsWith(".sfc"))
			{
				project.Type = ProjectType.Snes;
				buildConfiguration.Name = "SNES";
			}
			project.BuildConfigurations.Add(buildConfiguration);
			var fileReferences = new Dictionary<string, AsmProjectFile>();

			foreach (XmlNode sourceNode in projectNode.SelectNodes("project/sources/source"))
			{
				var newFile = new AsmProjectFile { Project = project, Mode = CompileMode.Ignore };
				newFile.File = new FileInfo(project.Directory.FullName + @"\" + sourceNode.Attributes["path"].Value);
				if (newFile.File.Extension.ToLower() == ".s") newFile.Mode = CompileMode.IncludeInAssembly;
				project.Files.Add(newFile);

				fileReferences.Add(sourceNode.Attributes["uuid"].Value, newFile);
			}
			foreach (XmlNode sourceNode in projectNode.SelectNodes("project/binaryfiles/binaryfile"))
			{
				var relativeFilePath = sourceNode.Attributes["path"].Value;
				var newFile = new AsmProjectFile { Project = project, Mode = CompileMode.Ignore };
				newFile.File = new FileInfo(project.Directory.FullName + @"\" + relativeFilePath);
				if (newFile.Type == FileType.Image)
				{
					newFile.Mode = CompileMode.ContentPipeline;
					newFile.Pipeline = new ChrPipeline(newFile, newFile.File.DirectoryName + @"\" + Path.GetFileNameWithoutExtension(newFile.File.Name) + ".chr");
					//var directory = Path.GetDirectoryName(relativeFilePath);
					//if (!string.IsNullOrWhiteSpace(directory)) directory += @"\";
					//newFile.Pipeline = new ChrPipeline(newFile, directory + Path.GetFileNameWithoutExtension(relativeFilePath) + ".chr");
				}
				project.Files.Add(newFile);

				fileReferences.Add(sourceNode.Attributes["uuid"].Value, newFile);
			}
			foreach (XmlNode sourceNode in projectNode.SelectNodes("project/sounds/musics/music"))
			{
				var newFile = new AsmProjectFile { Project = project, Mode = CompileMode.ContentPipeline };
				newFile.File = new FileInfo(project.Directory.FullName + @"\" + sourceNode.Attributes["name"].Value);
				project.Files.Add(newFile);

				fileReferences.Add(sourceNode.Attributes["uuid"].Value, newFile);
			}
			var linkerConfig = new AsmProjectFile { Project = project, Mode = CompileMode.LinkerConfig };
			linkerConfig.File = new FileInfo(project.Directory.FullName + @"\" + properties.Attributes["linkerconfigfile"].Value);
			project.Files.Add(linkerConfig);

			foreach (XmlNode bankNode in projectNode.SelectNodes("project/graphicsbanks/graphicsbank"))
			{
				var bank = new ChrBank();

				foreach (XmlNode dataNode in bankNode.SelectNodes("graphicitem"))
				{
					bank.Sources.Add(fileReferences[dataNode.Attributes["uuid"].Value]);
				}

				buildConfiguration.ChrBanks.Add(bank);
			}

			return project;
		}

		public void WatchFile()
		{
			if (_fileSystemWatcher != null)
			{
				_fileSystemWatcher.Dispose();
				_fileSystemWatcher = null;
			}
			if (ProjectFile == null || ProjectFile.DirectoryName == null) return;

			_fileSystemWatcher = new FileSystemWatcher(ProjectFile.DirectoryName, ProjectFile.Name);
			_fileSystemWatcher.Changed += (sender, args) =>
										{
											if (Changed != null) Changed();
										};

		}

		public void Dispose()
		{
			if (_fileSystemWatcher != null) _fileSystemWatcher.Dispose();
		}

		public void Save()
		{
			try
			{
				var fileHeader = ProjectFileHeader.GetFileHeader(this);
				var xmlDocument = new XmlDocument();
				var serializer = new XmlSerializer(fileHeader.GetType());
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, fileHeader);
					stream.Position = 0;
					xmlDocument.Load(stream);
					xmlDocument.Save(ProjectFile.FullName);
				}

				Pristine = true;
			}
			catch (Exception ex)
			{
				throw new Exception("Error when trying to save project file: " + ex.Message, ex);
			}
		}


		public void AddNewProjectFile(AsmProjectFile projectFile)
		{
			Files.Add(projectFile);
			Pristine = false;
		}
		public void RemoveProjectFile(AsmProjectFile projectFile)
		{
			var parentDirectory = projectFile.File.Directory;
			// If no other files left in directory, add empty directory
			if (parentDirectory != null && !Files.Any(f => f != projectFile && IsInDirectory(f, parentDirectory.FullName))) Directories.Add(parentDirectory);
			Files.Remove(projectFile); // Remove after adding empty directory, because changing files list causes project explorer refresh
			Pristine = false;
		}


		public string GetTargetCpu()
		{
			return Type == ProjectType.Snes ? "65816" : "6502";
		}
		public static string GetRelativePath(AsmProject project, string filePath)
		{
			if (filePath == null) return null;
			if (filePath == project.Directory.FullName) return "";
			return Uri.UnescapeDataString(new Uri(project.Directory.FullName + @"\")
				.MakeRelativeUri(new Uri(filePath)).ToString());
		}
		public string GetRelativePath(string filePath)
		{
			return GetRelativePath(this, filePath);
		}

		public IEnumerable<Breakpoint> GetAllBreakpoints()
		{
			var editorBreakpoints = new List<Breakpoint>();

			// TODO: Update breakpoints whenever debug info is updated, instead of here
			foreach (var projectFile in Files)
			{
				foreach (var breakpoint in projectFile.EditorBreakpoints)
				{
					if (projectFile.DebugLines != null
					    && projectFile.DebugLines.ContainsKey(breakpoint.BuildLine)
					    && projectFile.DebugLines[breakpoint.BuildLine].RomAddress.HasValue)
					{
						breakpoint.Broken = false;
						breakpoint.StartAddress = projectFile.DebugLines[breakpoint.BuildLine].RomAddress.Value;
					}
					else breakpoint.Broken = true;

					editorBreakpoints.Add(breakpoint);
				}
			}
			return editorBreakpoints.Concat(Breakpoints);
		}

		public void RemoveDeletedFiles()
		{
			var removeFiles = Files.Where(f => !f.File.Exists).ToArray();
			foreach (var file in removeFiles) Files.Remove(file);
		}

		public bool IsInDirectory(AsmProjectFile file, string directoryPath)
		{
			return OsFeatures.IsInDirectory(file.File.Directory, directoryPath);
		}
	}

	public interface ISymbolSource
	{
		List<KeyValuePair<string, Symbol>> Symbols { get; }
	}
}
