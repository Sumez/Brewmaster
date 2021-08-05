using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Brewsic
{
	public class SpcFile
	{
		public int Size { get => Data.Length; }
		private SpcFile(byte[] data)
		{
			Data = data;
		}

		public byte[] Data { get; private set; }
		public static SpcFile Create(byte[] musicBank, string songTitle)
		{
			byte[] driverData = new byte[0x10000];
			Array.Copy(Resources.spcdriver, 0, driverData, 0x200, Resources.spcdriver.Length);

			const int entryPoint = 0x0202;
			var titleData = Encoding.ASCII.GetBytes(songTitle);
			var titleBlock = new byte[32];
			Array.Copy(titleData, 0, titleBlock, 0, Math.Min(32, titleData.Length));
			Buffer.BlockCopy(musicBank, 0, driverData, ModuleConverter.BaseAddress - 1, musicBank.Length);

			var spcData = new List<byte>();
			spcData.AddRange(Encoding.ASCII.GetBytes("SNES-SPC700 Sound File Data v0.30"));
			spcData.AddRange(new byte[] { 26, 26, 27, 30 });
			spcData.AddRange(new byte[] { entryPoint & 0xff, (entryPoint >> 8) & 0xff }); // PC
			spcData.AddRange(new byte[] { 0, 0, 0, 0x0B, 0xED }); // A,X,Y,Flags,Stack
			spcData.AddRange(new byte[2]); // Unused
			spcData.AddRange(titleBlock);
			spcData.AddRange(new byte[32]); // Game title
			spcData.AddRange(new byte[16]); // Dumper
			spcData.AddRange(Encoding.ASCII.GetBytes("Converted using Brewsic         "));

			var header = new byte[0x100];
			var footer = new byte[0x100];

			Array.Copy(spcData.ToArray(), 0, header, 0, spcData.Count);
			footer[0x0C] = 0x60;
			footer[0x1C] = 0x60;
			footer[0x5D] = (byte)(ModuleConverter.BaseAddress >> 8);
			footer[0x07] = 0x1F;
			footer[0x17] = 0x1F;
			footer[0x27] = 0x1F;
			footer[0x37] = 0x1F;
			footer[0x47] = 0x1F;
			footer[0x57] = 0x1F;
			footer[0x67] = 0x1F;
			footer[0x77] = 0x1F;

			var data = new List<byte>();
			data.AddRange(header);
			data.AddRange(driverData);
			data.AddRange(footer);
			return new SpcFile(data.ToArray());
		}
	}
}