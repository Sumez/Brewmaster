using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace Brewsic
{
	public class BrrSample
	{
		public byte[] SampleData { get; set; }
		public int LoopStart { get; set; }
		public double Rate { get; private set; }

		public BrrSample(byte[] sampleData, int loopStart = 0)
		{
			SampleData = sampleData;
			LoopStart = loopStart;
			Rate = 32000;
		}
	}
}
