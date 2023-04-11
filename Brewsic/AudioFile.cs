using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace Brewsic
{
	public class AudioFile
	{
		public Sample Sample { get; private set; }

		public static AudioFile LoadFromFile(string fileName, int? sampleRate, Action<string> output = null)
		{
			return new AudioFile { Sample = GetSampleFromFile(fileName, sampleRate, output) };
		}

		private static Sample GetSampleFromFile(string fileName, int? sampleRate, Action<string> output)
		{
			using (var fileReader = File.OpenRead(fileName))
			{
				var extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
				switch (extension)
				{
					case "wav":
						using (var reader = new WaveFileReader(fileReader))
							return sampleRate.HasValue ? GetResampled(sampleRate.Value, reader, output) : GetSampleFromWaveProvider(reader, output);
					case "mp3":
						using (var reader = new Mp3FileReader(fileReader))
							return sampleRate.HasValue ? GetResampled(sampleRate.Value, reader, output) : GetSampleFromWaveProvider(reader, output);
					default:
						throw new Exception("Unrecognized file format");
				}
			}

		}
		private static Sample GetResampled(int sampleRate, IWaveProvider waveProvider, Action<string> output)
		{
			var originalRate = waveProvider.WaveFormat.SampleRate;
			if (sampleRate == originalRate) return GetSampleFromWaveProvider(waveProvider, output);

			output("Resampling audio from " + originalRate + "hz to " + sampleRate + "hz");
			using (var resampler = new MediaFoundationResampler(waveProvider, new WaveFormat(sampleRate, 1)))
			{
				return GetSampleFromWaveProvider(resampler, output);
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

			sample.ConvertedFromStereo = false;
			sample.Data = samples.ToArray();

			return sample;
		}
	}
}
