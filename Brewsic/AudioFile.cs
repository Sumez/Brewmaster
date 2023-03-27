using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace Brewsic
{
	public class AudioFile
	{
		public Sample Sample { get; private set; }

		public static AudioFile LoadFromFile(string fileName, Action<string> output = null)
		{
			return new AudioFile { Sample = GetSampleFromFile(fileName, output) };
		}

		private static Sample GetSampleFromFile(string fileName, Action<string> output)
		{
			using (var fileReader = File.OpenRead(fileName))
			{
				using (var reader = new Mp3FileReader(fileReader))
				{
					return GetSampleFromWaveProvider(reader, output);
				}
			}

		}
		private static Sample GetSampleFromWaveProvider(IWaveProvider waveProvider, Action<string> output)
		{
			var samples = new List<short>();
			var sampleProvider = waveProvider.ToSampleProvider().ToMono();
			var buffer = new float[1000];
			while (sampleProvider.Read(buffer, 0, 1000) > 0)
			{
				foreach (var frame in buffer)
				{
					var shortFrame = (short)Math.Round(frame * short.MaxValue);
					samples.Add(shortFrame);
				}
			}


			var sample = new Sample();

			/*sample.GlobalVolume = stream.ReadByte(); // 0-64
			sample.Volume = stream.ReadByte(); // 0-64
			sample.Name = ReadString(stream, 26).Trim('\0', ' ');
			sample.Panning = stream.ReadByte(); // 0-64 (32 = center)
			sample.Length = ReadUInt32(stream);
			sample.LoopStart = ReadUInt32(stream);
			sample.LoopEnd = ReadUInt32(stream);
			sample.C5Speed = (double)ReadUInt32(stream);

			stream.Position = samplePointer;
			sample.ConvertedFromStereo = stereo;

			sample.Data = new 
			ms.Read(sample.Data, 0, ms.Length);
			sample.Data = ReadSampleData(stream, sample, compression, bitDepth, stereo);
			*/

			sample.ConvertedFromStereo = false;
			sample.Data = samples.ToArray();

			return sample;
		}
	}
}
