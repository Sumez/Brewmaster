using System;
using System.Collections.Generic;
using System.Linq;

namespace Brewsic
{
	public class ModuleConverter
	{
		public const int BaseAddress = 0x1000;
		public static byte[] GetBrewsicMusicDump(List<ItModule> modules, Action<string> output, int maximumSampleGrowth = 500, bool compressPatterns = true, int maxSampleSize = 0)
		{
			var emptySampleSlots = 1; // For sound effects
			var sampleCount = modules[0].Samples.Count + emptySampleSlots;
			var sampleDirectoryLength = (byte)(sampleCount * 2);
			var trackDirectoryLength = modules.Count;

			var sampleDirectory = new List<int>(sampleDirectoryLength);
			var trackDirectory = new List<int>(trackDirectoryLength);
			var sampleAddress = BaseAddress + sampleDirectoryLength * 2 + trackDirectoryLength * 2;
			var sampleData = new List<byte>();
			var trackData = new List<byte>();

			foreach (var sample in modules[0].Samples)
			{
				var brr = sample.GetBrr(maximumSampleGrowth, maxSampleSize, output);
				if (sample.LoopStart % 16 != 0) throw new Exception(string.Format("Looping error on Sample \"{0}\"", sample.Name));

				sampleDirectory.Add(sampleAddress); // Start address
				sampleDirectory.Add((UInt16)(sampleAddress + (sample.LoopStart / 16 * 9))); // Loop address

				sampleAddress += brr.Length;
				sampleData.AddRange(brr);
			}
			sampleDirectory.AddRange(new int[2 * emptySampleSlots]);
			output(string.Format("Sample data size: {0} bytes", sampleData.Count));

			var trackAddress = sampleAddress;
			foreach (var module in modules)
			{
				if (compressPatterns) output("Compressing pattern data...");
				var compressedTrack = module.GetCompressedPatternData(compressPatterns);
				trackDirectory.Add(trackAddress);
				trackData.AddRange(compressedTrack);
			}
			output(string.Format("Track data size: {0} bytes", trackData.Count));

			var sampleDirectoryData = GetAsByteArray(sampleDirectory);
			var trackDirectoryData = GetAsByteArray(trackDirectory);

			var data = new List<byte>();
			data.Add(sampleDirectoryLength);
			data.AddRange(sampleDirectoryData);
			data.AddRange(trackDirectoryData);
			data.AddRange(sampleData.ToArray());
			data.AddRange(trackData.ToArray());
			return data.ToArray();
		}

		private static byte[] GetAsByteArray(IReadOnlyList<int> list)
		{
			var array = new byte[list.Count * 2];
			for (var i = 0; i < list.Count; i++)
			{
				var value = list[i];
				if (value > 0xffff) throw new Exception("Module is too large to store in 64kb");
				array[i * 2] = (byte)(value & 0xff);
				array[i * 2 + 1] = (byte)((value >> 8) & 0xff);
			}
			return array;
		}
	}
}
