using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public class CustomTileMapPipeline : PipelineOption
	{
		public override string TypeName
		{
			get { return "tilemap.custom";  }
		}
		public override IEnumerable<FileType> SupportedFileTypes { get { return new [] { FileType.TileMap }; } }
		public override void Process(PipelineSettings settings, Action<string> output)
		{
			var scriptFile = settings.GenericSettings["script-file"];
			if (string.IsNullOrWhiteSpace(scriptFile)) throw new Exception("No script file defined for " + settings.File.File.Name);

			scriptFile = settings.File.Project.GetFullPath(scriptFile);
			var fileInfo = new FileInfo(scriptFile);
			if (!fileInfo.Exists) throw new Exception(string.Format("Unable to find script file: '{0}'", scriptFile));

			switch (settings.GenericSettings["script-type"])
			{
				case "Python":
					ExecutePythonScript(fileInfo, settings.File.File.FullName, settings.GetFilePath(0), output);
					break;
				case "Lua":
				case "JavaScript":
				default:
					throw new NotImplementedException();
			}
		}

		private void ExecutePythonScript(FileInfo file, string inputFile, string outputFile, Action<string> output)
		{
			using (var commandProcess = new Process())
			{
				var command = new ProcessStartInfo();
				command.WorkingDirectory = file.DirectoryName;
				command.FileName = Program.PythonPath; // TODO: Catch missing python installation/PATH
				command.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\"", file.Name, inputFile, outputFile);
				command.UseShellExecute = false;
				command.RedirectStandardError = true;
				command.RedirectStandardOutput = true;
				command.CreateNoWindow = true;

				commandProcess.StartInfo = command;
				commandProcess.EnableRaisingEvents = true;
				commandProcess.OutputDataReceived += (s, e) => output(e.Data);
				var errorMessage = new StringBuilder();

				commandProcess.ErrorDataReceived += (s, e) => errorMessage.AppendLine(e.Data);

				try
				{
					commandProcess.Start();
				}
				catch (Win32Exception ex)
				{
					if (ex.ErrorCode == -0x7FFFBFFB) {
						throw new Exception("Python installation was not found. Either install missing binaries, or add the location of Python to your PATHS variable.");
					}
				}
				commandProcess.BeginOutputReadLine();
				commandProcess.BeginErrorReadLine();
				commandProcess.WaitForExit();
				if (commandProcess.ExitCode != 0) throw new Exception("Python error:" + Environment.NewLine + errorMessage);
			}
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".inc");
		}

		public override bool CanSkip(PipelineSettings settings)
		{
			var scriptFile = new FileInfo(settings.File.Project.GetFullPath(settings.GenericSettings["script-file"]));
			if (scriptFile.Exists && settings.File.Pipeline.LastProcessed <= scriptFile.LastWriteTime) return false;

			return true;
		}

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("script-type", PipelinePropertyType.Select, "Python", new List<string> { /*"JavaScript",*/ "Python", "Lua" }),
					new PipelineProperty("script-file", PipelinePropertyType.ProjectFile, "", GetFileNameFilter)
				};
			}
		}

		private string GetFileNameFilter(PipelineSettings settings)
		{
			switch (settings.GenericSettings["script-type"]) {
				case "Python":
					return "*.py (Python script)|*.py|*.*|*.*";
				case "Lua":
					return "*.lua (Lua script)|*.lua|*.*|*.*";
				case "JavaScript":
					return "*.js (JavaScript/EcmaScript)|*.js|*.*|*.*";
				default:
					return "*.*|*.*";
			}
		}
	}
}
