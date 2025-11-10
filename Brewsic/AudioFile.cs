using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Brewsic
{
	public class AudioFile
	{
		public Sample Sample { get; private set; }

		public static AudioFile LoadFromFile(string fileName, int? sampleRate, Action<string> output = null, CancellationToken? token = null)
		{
			return new AudioFile { Sample = GetSampleFromFile(fileName, sampleRate, output, token) };
		}

		public static WaveStream GetWaveStreamFromFile(string fileName)
		{
			var extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
			switch (extension)
			{
				case "wav":
					return new WaveFileReader(fileName);
				case "mp3":
					return new Mp3FileReader(fileName);
				default:
					throw new Exception("Unrecognized file format");
			}
		}


		private static Sample GetSampleFromFile(string fileName, int? sampleRate, Action<string> output, CancellationToken? token)
		{
			using (var reader = GetWaveStreamFromFile(fileName))
				return sampleRate.HasValue ? GetResampled(sampleRate.Value, reader, output, token) : GetSampleFromWaveProvider(reader, output, token);
		}
		private static Sample GetResampled(int sampleRate, IWaveProvider waveProvider, Action<string> output, CancellationToken? token)
		{
			var originalRate = waveProvider.WaveFormat.SampleRate;
			if (sampleRate == originalRate) return GetSampleFromWaveProvider(waveProvider, output, token);

			if (output != null) output("Resampling audio from " + originalRate + "hz to " + sampleRate + "hz");
			using (var resampler = new MediaFoundationResampler(waveProvider, new WaveFormat(sampleRate, 1)))
			{
				return GetSampleFromWaveProvider(resampler, output, token);
			}

		}

		private static Sample GetSampleFromWaveProvider(IWaveProvider waveProvider, Action<string> output, CancellationToken? token)
		{
			var samples = new List<short>();
			var sampleProvider = waveProvider.ToSampleProvider().ToMono();
			var buffer = new float[1000];
			int samplesRead;
			while ((samplesRead = sampleProvider.Read(buffer, 0, 1000)) > 0)
			{
				if (token?.IsCancellationRequested ?? true) return null;
				for (var i = 0; i < samplesRead; i++)
				{
					var sample16bit = (short)Math.Round(buffer[i] * short.MaxValue);
					samples.Add(sample16bit);
				}
			}


			var sample = new Sample();

			sample.ConvertedFromStereo = false;
			sample.Data = samples.ToArray();

			return sample;
		}
	}
}
