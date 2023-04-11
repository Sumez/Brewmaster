using Brewmaster.ProjectModel;
using Brewsic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Brewmaster.Pipeline
{
	public class BrrPipeline : PipelineOption
	{
		public override string TypeName => "audio.brr";

		public override IEnumerable<FileType> SupportedFileTypes => new [] { FileType.Audio };

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("sample-rate", PipelinePropertyType.Text, "32000"),
					new PipelineProperty("pitch", PipelinePropertyType.Text, ""),
					new PipelineProperty("max-size", PipelinePropertyType.Text, ""),
				};
			}
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".brr");
		}
		public override void Process(PipelineSettings settings, Action<string> output)
		{
			var sampleRate = 32000;
			if (int.TryParse(settings.GenericSettings["sample-rate"], out var parsedSampleRate)) sampleRate = parsedSampleRate;
			var audioFile = AudioFile.LoadFromFile(settings.File.File.FullName, sampleRate, output);
			using (var outputStream = System.IO.File.Create(settings.GetFilePath(0)))
			{
				var maxSize = 0;
				if (int.TryParse(settings.GenericSettings["max-size"], out var intValue)) maxSize = intValue;
				var brrFile = audioFile.Sample.GetBrr(500, maxSize, output);
				outputStream.Write(brrFile, 0, brrFile.Length);
				outputStream.Close();
			}
		}

		public void OldProcess(PipelineSettings settings, Action<string> output)
		{
			var pitchSetting = settings.GenericSettings["pitch"];
			var loopSetting = settings.GenericSettings["loop-start"];
			int? pitch = null;
			int? loopStart = null;

			if (int.TryParse(pitchSetting, out int intPitch)) pitch = intPitch;
			if (int.TryParse(loopSetting, out int intLoop)) loopStart = intLoop;

			var startInfo = new ProcessStartInfo(string.Format(@"{0}\tools\snesbrr.exe", Program.WorkingDirectory));
			startInfo.WorkingDirectory = settings.File.File.Directory.FullName;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.StandardOutputEncoding = Encoding.UTF8;
			startInfo.Arguments = string.Format("-e{2}{3} \"{0}\" \"{1}\"",
								settings.File.File.FullName,
								settings.GetFilePath(0),
								pitch.HasValue ? " -p " + pitch.Value : "",
								loopStart.HasValue ? " -l " + loopStart.Value : "");

			using (var process = new Process())
			{
				process.StartInfo = startInfo;
				process.EnableRaisingEvents = true;
				process.OutputDataReceived += (s, e) => output(e.Data);
				var errorMessage = new StringBuilder();
				process.ErrorDataReceived += (s, e) => errorMessage.AppendLine(e.Data);

				output("snesbrr " + startInfo.Arguments);

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (process.ExitCode != 0) throw new Exception("SnesBRR error:" + Environment.NewLine + errorMessage);
			}
		}
	}
}
