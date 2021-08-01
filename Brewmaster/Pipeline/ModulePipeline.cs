using Brewmaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Brewsic;

namespace Brewmaster.Pipeline
{
	public class ModulePipeline : PipelineOption
	{
		public override string TypeName => "tracker.module";

		public override IEnumerable<FileType> SupportedFileTypes => new[] { FileType.Audio };

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".bin");
		}

		public override void Process(PipelineSettings settings, Action<string> output)
		{
			var modules = new List<ItModule> { ItModule.LoadFromFile(settings.File.File.FullName) };
			output(string.Format("Converting song \"{0}\" to Brewsic music format", modules[0].Title));
			var musicBank = ModuleConverter.GetBrewsicMusicDump(modules, output, 500, true, 0);

			/*foreach (var sample in modules[0].Samples)
			{
				var filename = sample.Name;
				foreach (var ch in Path.GetInvalidFileNameChars()) filename = filename.Replace(ch.ToString(), "");
				File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".brewsic.wav", GetWavFile(sample.Data));
				File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".brewsic.brr", sample.GetBrr());
			}*/
			using (var bankStream = File.Create(settings.GetFilePath(0)))
			{
				bankStream.Write(musicBank, 0, musicBank.Length);
				bankStream.Close();
			}

			//return; 
			
			var spcFile = SpcFile.Create(musicBank, modules[0].Title);
			using (var spcStream = File.Create(settings.GetFilePath(0) + ".spc"))
			{
				spcStream.Write(spcFile.Data, 0, spcFile.Size);
				spcStream.Close();
			}
		}

		private byte[] GetWavFile(short[] sampleData)
		{
			using (var stream = new MemoryStream())
			{
				var data = new byte[sampleData.Length * 2];
				Buffer.BlockCopy(sampleData, 0, data, 0, data.Length);
				WriteWavHeader(stream, false, 1, 16, 32000, data.Length);
				stream.Write(data, 0, data.Length);
				var waveBytes = new byte[stream.Length];
				stream.Position = 0;
				stream.Read(waveBytes, 0, waveBytes.Length);
				return waveBytes;
			}
		}

		private void WriteWavHeader(MemoryStream stream, bool isFloatingPoint, int channelCount, int bitDepth, int sampleRate, int size)
		{
			stream.Position = 0;
			stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
			// Chunk size.
			stream.Write(BitConverter.GetBytes(size + 36), 0, 4);
			stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

			// Sub-chunk 1.
			stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
			// Sub-chunk 1 size.
			stream.Write(BitConverter.GetBytes(16), 0, 4);
			// Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
			stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);
			// Channels.
			stream.Write(BitConverter.GetBytes(channelCount), 0, 2);
			// Sample rate.
			stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
			// Bytes rate.
			stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);
			// Block align.
			stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);
			// Bits per sample.
			stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);

			// Sub-chunk 2.
			stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
			// Sub-chunk 2 size.
			stream.Write(BitConverter.GetBytes(size), 0, 4);
		}
	}
}
