using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Brewmaster.Modules.Build;
using Brewmaster.Pipeline;
using Brewmaster.ProjectModel;

namespace Brewmaster.BuildProcess
{
	public interface IFileLocation
	{
		string File { get; }
		int Line { get; }
	}

	public class BuildHandler
	{
		public Action<LogData> Log { get; set; }
		public Action<String> Status { get; set; }
		public Action<List<BuildError>> RefreshErrorList { get; set; }
		public event Action OnDebugDataUpdated;
		public Task<bool> Build(AsmProject project)
		{
			var buildProcessSource = new TaskCompletionSource<bool>();

			Task.Run(() =>
			{
				if (RefreshErrorList != null) RefreshErrorList(new List<BuildError>());
				try
				{
					var errors = BuildSync(project, buildProcessSource);
					if (RefreshErrorList != null) RefreshErrorList(errors);
				}
				catch (Exception ex)
				{
					Log(new LogData("Unexpected error: " + ex.Message, LogType.Error));
					buildProcessSource.SetResult(false);
				}
			});

			return buildProcessSource.Task;
		}

		public Task<List<BuildError>> ParseErrors(string sourceCode, AsmProjectFile projectFile)
		{
			return Task.Run(() =>
			{
				try
				{
					return ParseErrorsSync(sourceCode, projectFile);
				}
				catch (IOException ex)
				{
					if (ex.HResult == -2147024864)
					{
						// This task isn't essential, so just suppress IO errors when file is used by other process
						return new List<BuildError>();
					}
					Log(new LogData("Error testing assembly: " + ex.Message, LogType.Error));
					throw ex;
				}
				catch (Exception ex)
				{
					Log(new LogData("Error testing assembly: " + ex.Message, LogType.Error));
					throw ex;
				}
			});
		}

		private void BuildChr(string projectFolder, AsmProject project, NesCartridge cartridge)
		{
			var outputDirectory = Path.Combine(projectFolder, cartridge.ChrBuildPath);
			if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

			var chrFilePath = string.Format(@"{0}\{1}", outputDirectory, cartridge.ChrFile);
			using (var chrFile = File.Create(chrFilePath))
			{
				foreach (var bank in cartridge.ChrBanks)
				{
					foreach (var source in bank.Sources)
					{
						if (!(source.Pipeline is ChrPipeline)) continue;

						using (var read = File.OpenRead(((ChrPipeline)source.Pipeline).ChrOutput))
						{
							var bufferSize = (int)read.Length;
							var chrData = new byte[bufferSize];
							read.Read(chrData, 0, bufferSize);
							chrFile.Write(chrData, 0, bufferSize);
						}
					}
					// TODO: Confirm bank size (8KB)
				}
				chrFile.Close();
			}
		}

		public class BuildError : IFileLocation
		{
			public enum BuildErrorType { Unknown, Warning, Error };
			private const string ErrorMatch = @"^(.*)\(([0-9]+)\):\s+(Error|Warning):\s+(.*)$";
			private const string LinkerMatch = @"^ld65\.exe:\s+(Error|Warning):\s+(.*)\(([0-9]+)\):\s+(.*)$";
			private const string LinkerMatch2 = @"^ld65\.exe:\s+(Error|Warning):\s+(.*\s+`(.*)'),\s+line\s+([0-9]+)$";
			private const string ReferenceMatch = @"^(.+)\s+(referenced in:)\s+(.*)\(([0-9]+)\)$";

			public BuildError(string message, BuildErrorType type)
			{
				FullMessage = Message = message;
				Type = type;
			}

			public BuildError(string message) : this(message, BuildErrorType.Unknown)
			{
				var match = Regex.Match(message, ErrorMatch);
				if (match.Success)
				{
					Type = match.Groups[3].Value == "Error" ? BuildErrorType.Error : BuildErrorType.Warning;
					File = match.Groups[1].Value;
					Line = int.Parse(match.Groups[2].Value);
					Message = match.Groups[4].Value;
				}
				else if ((match = Regex.Match(message, LinkerMatch)).Success)
				{
					Type = match.Groups[1].Value == "Error" ? BuildErrorType.Error : BuildErrorType.Warning;
					File = match.Groups[2].Value;
					Line = int.Parse(match.Groups[3].Value);
					Message = match.Groups[4].Value;
				}
				else if ((match = Regex.Match(message, LinkerMatch2)).Success)
				{
					Type = match.Groups[1].Value == "Error" ? BuildErrorType.Error : BuildErrorType.Warning;
					File = match.Groups[3].Value;
					Line = int.Parse(match.Groups[4].Value);
					Message = match.Groups[2].Value;
				}
				else if ((match = Regex.Match(message, ReferenceMatch)).Success)
				{
					Type = BuildErrorType.Error;
					File = match.Groups[3].Value;
					Line = int.Parse(match.Groups[4].Value);
					Message = match.Groups[1].Value;
				}
			}

			public string Message { get; private set; }
			public string File { get; private set; }
			public int Line { get; set; }
			public BuildErrorType Type { get; private set; }
			public string FullMessage { get; private set; }
		}
		private List<BuildError> ParseErrorsSync(string sourceCode, AsmProjectFile projectFile)
		{
			var errors = new List<BuildError>();
			var projectFolder = projectFile.Project.Directory.FullName;
			var sourceFile = Path.GetTempPath() + @"/ca65temp.s";
			var objectFile = Path.GetTempPath() + @"/ca65temp.o";
			var configuration = projectFile.Project.CurrentConfiguration;

			File.WriteAllText(sourceFile, sourceCode);
			using (var process = new Process())
			{
				process.StartInfo = GetAsmParams(projectFolder);
				process.EnableRaisingEvents = true;
				process.OutputDataReceived += OutputReceived;
				process.ErrorDataReceived += (s, e) =>
				{
					if (e.Data != null) errors.Add(new BuildError(e.Data));
				};
				
				var fileDirectory = projectFile.GetRelativeDirectory();
				process.StartInfo.Arguments = string.Format("\"{0}\" -o \"{1}\" -t nes --cpu {2}{4} -I .{3}", 
					sourceFile, 
					objectFile, 
					projectFile.Project.GetTargetCpu(), 
					string.IsNullOrWhiteSpace(fileDirectory) ? "" : " -I \"" + fileDirectory + "\"",
					string.Join("", configuration.Symbols.Select(s => " -D " + s)));

				process.Start();
				process.BeginErrorReadLine();
				process.WaitForExit();
			}
			return errors;
		}
		private List<BuildError> BuildSync(AsmProject project, TaskCompletionSource<bool> buildProcessSource)
		{
			var errors = new List<BuildError>();
			var asmFailed = false;
			var cartridge = project.CurrentConfiguration;
			var ideFolder = Program.WorkingDirectory;
			var projectFolder = project.Directory.FullName;

			var linkerConfig = project.Files.FirstOrDefault(f => f.GetRelativePath() == cartridge.LinkerConfigFile);
			if (linkerConfig == null)
			{
				var configFiles = project.Files.Where(l => l.Mode == CompileMode.LinkerConfig);
				if (!configFiles.Any())
				{
					buildProcessSource.SetResult(false);
					return errors;
				}
				linkerConfig = configFiles.First();
			}

			Status("Building NES cartridge...");

			Log(new LogData("Building NES cartridge...", LogType.Headline));
			Log(new LogData("Processing content pipeline"));

			var skippedCount = 0;
			foreach (var file in project.Files.Where(f => f.Mode == CompileMode.ContentPipeline && f.Pipeline != null))
			{
				// Get a fresh File Info from the file system in case something was changed since we loaded the project
				var fileInfo = new FileInfo(file.File.FullName); // TODO: Refresh ProjectFile.File using file system watch to get LastWriteTime
                                                     // ?
				if (!fileInfo.Exists)
				{
					var error = new BuildError(
						string.Format(@"File '{0}' is missing. Skipping data processing...", file.GetRelativePath()),
						BuildError.BuildErrorType.Warning);
					errors.Add(error);
					ErrorReceived(error);
					continue;
				}

				if (file.Pipeline.LastProcessed != null && file.Pipeline.LastProcessed > fileInfo.LastWriteTime)
				{
					//Log(new LogData(string.Format("Skipping '{0}'", file.GetRelativePath())));
					skippedCount++;
					continue;
				}

				Log(new LogData(string.Format("Building {0}", string.Join(",", file.Pipeline.OutputFiles.Where(f => f != null).Select(project.GetRelativePath)))));
				file.Pipeline.Process();
			}
			if (skippedCount > 0) Log(new LogData(string.Format("Skipped {0} unchanged files", skippedCount)));

			if (projectFolder == null) throw new Exception("Project directory not found");

			var targetFile = Path.Combine(projectFolder, cartridge.Filename);
			var buildFolder = Path.Combine(projectFolder, cartridge.BuildPath);
			var prgFolder = cartridge.PrgBuildPath != null ? Path.Combine(projectFolder, cartridge.PrgBuildPath) : null;
			var chrFolder = cartridge.ChrBuildPath != null ? Path.Combine(projectFolder, cartridge.ChrBuildPath) : null;
			var debugFile = Path.Combine(projectFolder, cartridge.DebugFile);

			if (!Directory.Exists(Path.GetDirectoryName(targetFile))) Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
			if (!Directory.Exists(buildFolder)) Directory.CreateDirectory(buildFolder);
			if (prgFolder != null && !Directory.Exists(prgFolder)) Directory.CreateDirectory(prgFolder);
			if (chrFolder != null && !Directory.Exists(chrFolder)) Directory.CreateDirectory(chrFolder);
			if (!Directory.Exists(Path.GetDirectoryName(debugFile))) Directory.CreateDirectory(Path.GetDirectoryName(debugFile));

			if (cartridge.ChrBanks.Any() && chrFolder != null) {
				Log(new LogData("Building CHR from graphic banks", LogType.Headline));
				BuildChr(projectFolder, project, cartridge);
			}

			Log(new LogData("Building PRG from source files", LogType.Headline));

			var asmParams = GetAsmParams(projectFolder);

			var sourceFiles = project.Files.Where(f => f.Mode == CompileMode.IncludeInAssembly);
			var objectFiles = new List<string>();
			foreach (var projectFile in sourceFiles)
			{
				using (var asmProcess = new Process())
				{
					asmProcess.StartInfo = asmParams;
					asmProcess.EnableRaisingEvents = true;
					asmProcess.OutputDataReceived += OutputReceived;
					string multilineError = null;
					asmProcess.ErrorDataReceived += (s, e) => { ProcessErrorData(e.Data, errors, ref multilineError); };

					var directory = Path.Combine(projectFolder, cartridge.BuildPath, projectFile.GetRelativeDirectory());
					if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
					var fileBase = projectFile.GetRelativeDirectory() + @"/" + Path.GetFileNameWithoutExtension(projectFile.File.Name);
					var sourceFile = projectFile.GetRelativePath();
					var objectFile = cartridge.BuildPath + @"/" + fileBase + ".o";
					var dependencyFile = cartridge.BuildPath + @"/" + fileBase + ".d";
					objectFiles.Add(objectFile);

					asmProcess.StartInfo.Arguments = string.Format("\"{0}\" -o \"{1}\" --create-dep \"{2}\" -t nes --cpu {3}{4} -g -I .", 
						sourceFile, 
						objectFile, 
						dependencyFile, 
						project.GetTargetCpu(),
						string.Join("", cartridge.Symbols.Select(s => " -D " + s)));

					Log(new LogData("ca65 " + asmProcess.StartInfo.Arguments));
					asmProcess.Start();
					asmProcess.BeginOutputReadLine();
					asmProcess.BeginErrorReadLine();
					asmProcess.WaitForExit();
					if (asmProcess.ExitCode != 0) asmFailed = true;
				}
			}
			if (asmFailed)
			{
				buildProcessSource.SetResult(false);
				return errors;
			}

			if (File.Exists(targetFile)) File.Delete(targetFile);
			var outputFile = targetFile;
			if (cartridge.PrgBuildPath != null && cartridge.PrgFile != null)
			{
				outputFile = cartridge.PrgBuildPath + @"/" + cartridge.PrgFile;
			}

			using (var linkerProcess = new Process())
			{
				linkerProcess.StartInfo = asmParams;
				linkerProcess.EnableRaisingEvents = true;
				linkerProcess.OutputDataReceived += OutputReceived;
				string multilineError = null;
				linkerProcess.ErrorDataReceived += (s, e) => { ProcessErrorData(e.Data, errors, ref multilineError); };

				//linkerProcess.StartInfo.FileName = string.Format(@"{0}\cc65\bin\cl65.exe", ideFolder);
				//linkerProcess.StartInfo.Arguments = string.Format("-t nes -C {0} --mapfile {1} -Wl --dbgfile,{2} -o {3} {4}", cartridge.LinkerConfigFile, cartridge.MapFile, cartridge.DebugFile, prgFile, string.Join(" ", objectFiles));

				linkerProcess.StartInfo.FileName = string.Format(@"{0}\cc65\bin\ld65.exe", ideFolder);
				linkerProcess.StartInfo.Arguments = string.Format("-o \"{3}\" -C \"{0}\" -m \"{1}\" --dbgfile \"{2}\" {4}", 
					linkerConfig.GetRelativePath(), 
					cartridge.MapFile, 
					cartridge.DebugFile, 
					outputFile, 
					string.Join(" ", objectFiles.Select(f => string.Format("\"{0}\"", f)))
				);

				Log(new LogData("ld65 " + linkerProcess.StartInfo.Arguments));
				linkerProcess.Start();
				linkerProcess.BeginOutputReadLine();
				linkerProcess.BeginErrorReadLine();
				linkerProcess.WaitForExit();
				if (linkerProcess.ExitCode != 0)
				{
					buildProcessSource.SetResult(false);
					return errors;
				}
			}

			if (project.Type == ProjectType.Snes && cartridge.CalculateChecksum)
			using (var stream = File.Open(outputFile, FileMode.Open, FileAccess.ReadWrite))
			{
				Log(new LogData("Calculating SNES checksum", LogType.Headline));

				var prgSize = (Int32)stream.Length; // We don't expect the PRG size to ever be longer than a 32 bit int
				var prgData = new byte[prgSize];
				var checksumData = new byte[4];

				stream.Read(prgData, 0, prgSize);

				ushort checksum = 0;
				unchecked // Allow 16bit integer to overflow
				{
					for (var i = 0; i < prgSize; i++)
					{
						if (i >= 0x7FDC && i <= 0x7FDF) continue;
						checksum += prgData[i];
					}
					checksum += 0xff;
					checksum += 0xff;
				}
				checksumData[2] = (byte)(checksum & 0xff);
				checksumData[3] = (byte)((checksum >> 8) & 0xff);
				checksumData[0] = (byte)(~checksum & 0xff);
				checksumData[1] = (byte)((~checksum >> 8) & 0xff);

				stream.Position = 0x7FDC;
				stream.Write(checksumData, 0, 4);
			}

			if (prgFolder != null) {
				if (project.Type == ProjectType.Nes) Log(new LogData("Merging into iNES file", LogType.Headline));
				using (var write = File.OpenWrite(targetFile))
				{
					using (var read = File.OpenRead(string.Format(@"{0}\{1}", prgFolder, cartridge.PrgFile)))
					{
						var prgSize = (Int32)read.Length; // We don't expect the PRG size to ever be longer than a 32 bit int
						var prgData = new byte[prgSize];

						read.Read(prgData, 0, prgSize);
						write.Write(prgData, 0, prgSize);
					}
					if (chrFolder != null)
					using (var read = File.OpenRead(string.Format(@"{0}\{1}", chrFolder, cartridge.ChrFile)))
					{
						var chrSize = (Int32)read.Length; // We don't expect the CHR size to ever be longer than a 32 bit int
						var chrData = new byte[chrSize];
						read.Read(chrData, 0, chrSize);
						write.Write(chrData, 0, chrSize);
					}
					write.Close();
					Log(new LogData(targetFile.Replace('/', '\\')));
				}
			}
			Log(new LogData("---"));
			Log(new LogData("Build complete\r\n"));
			Status("Build complete");

			Log(new LogData("Parsing debug info"));
			var debugDataTask = project.ParseDebugDataAsync(debugFile);
			debugDataTask.ContinueWith((t) =>
			{
				if (t.Status != TaskStatus.RanToCompletion)
				{
					Log(new LogData("Error parsing: " + t.Exception.InnerExceptions[0].Message, LogType.Error));
					buildProcessSource.SetResult(false);
					return;
				}
				if (OnDebugDataUpdated != null) OnDebugDataUpdated();
				Log(new LogData("Done parsing"));

				project.UpdatedSinceLastBuild = false;
				buildProcessSource.SetResult(true);
			});

			return errors;
		}

		private void ProcessErrorData(string message, List<BuildError> errors, ref string multilineError)
		{
			if (message == null) return;
			var fullMessage = message;
			if (multilineError != null && message.StartsWith(" ")) fullMessage = multilineError + message;
			else if (message.EndsWith("referenced in:"))
			{
				multilineError = message;
				Log(new LogData(message, LogType.Error));
				return;
			}
			else multilineError = null;
			
			var error = new BuildError(fullMessage);
			errors.Add(error);
			ErrorReceived(error, message);
		}
		private void ErrorReceived(BuildError error, string message = null)
		{
			Log(new LogData(
				message ?? error.FullMessage, 
				error.Type == BuildError.BuildErrorType.Warning ? LogType.Warning : LogType.Error, 
				error));
		}

		private void OutputReceived(object sender, DataReceivedEventArgs args)
		{
			if (args.Data != null) Log(new LogData(args.Data));
		}

		private ProcessStartInfo GetAsmParams(string projectFolder)
		{
			var asmParams = new ProcessStartInfo(string.Format(@"{0}\cc65\bin\ca65.exe", Program.WorkingDirectory));
			SetEnvironmentVariables(asmParams.EnvironmentVariables);

			asmParams.WorkingDirectory = projectFolder;
			asmParams.CreateNoWindow = true;
			asmParams.UseShellExecute = false;
			asmParams.RedirectStandardError = true;
			asmParams.RedirectStandardOutput = true;
			asmParams.StandardOutputEncoding = Encoding.UTF8;

			return asmParams;
		}
		private void SetEnvironmentVariables(StringDictionary environmentVariables)
		{
			var ideFolder = Program.WorkingDirectory;
			
			var oldPath = environmentVariables["PATH"];
			environmentVariables["PATH"] = string.Format(@"{0}\cc65\bin;{1}", ideFolder, oldPath);
			environmentVariables["CC65_HOME"] = string.Format(@"{0}\cc65", ideFolder);
			environmentVariables["LD65_LIB"] = string.Format(@"{0}\cc65\lib", ideFolder);
			environmentVariables["CA65_INC"] = string.Format(@"{0}\cc65\asminc", ideFolder);
			environmentVariables["CC65_INC"] = string.Format(@"{0}\cc65\include", ideFolder);

		}
	}
}
