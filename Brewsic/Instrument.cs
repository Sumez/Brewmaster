using System;
using System.Collections.Generic;

namespace Brewsic
{
	public class Instrument
	{
		public string Name { get; set; }
		public byte Sample { get; set; }
		public byte FadeOut { get; set; }
		public Dictionary<int, byte> NoteMap { get; set; } = new Dictionary<int, byte>();
		public UInt16 VolumeEnvelopeAddress { get; set; }
		public byte[] VolumeEnvelope { get; set; }
		public int Volume { get; set; }
	}
}