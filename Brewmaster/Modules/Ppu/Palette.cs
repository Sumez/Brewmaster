using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Brewmaster.Modules.Ppu
{
	public class Palette
	{
		public List<Color> Colors { get; set; }
		private bool _snes = false;

		public Palette()
		{
			Colors = new List<Color>();
		}

		public void Load(string filename)
		{
			Colors = new List<Color>();
			using (var stream = File.OpenRead(filename))
			{
				var channels = new byte[3];
				while (stream.Read(channels, 0, 3) == 3)
				{
					Colors.Add(Color.FromArgb(channels[0], channels[1], channels[2]));
				}
			}
		}

		public Color Get(int index)
		{
			if (!_snes) return Colors[index];
			return From15BitColor(index);
		}

		public static Color From15BitColor(int value)
		{
			var r = To8BitChannel(value & 0x1F);
			var g = To8BitChannel((value >> 5) & 0x1F);
			var b = To8BitChannel((value >> 10) & 0x1F);

			return Color.FromArgb(r, g, b);
		}
		public static int To15BitColor(Color value)
		{

			var r = From8BitChannel(value.R) & 0x1F;
			var g = From8BitChannel(value.G) & 0x1F;
			var b = From8BitChannel(value.B) & 0x1F;

			return r | (g << 5) | (b << 10);
		}

		public static int From8BitChannel(byte color)
		{
			return color >> 3;
		}

		public static int To8BitChannel(int color)
		{
			return ((color << 3) + (color >> 2));
		}

		public byte[] GetBinary(bool argb = true)
		{
			var returnValue = new List<byte>();
			foreach (var color in Colors)
			{
				if (argb)
				{
					returnValue.AddRange(new[] { color.B, color.G, color.R });
					returnValue.Add(255);
				}
				else returnValue.AddRange(new[] { color.R, color.G, color.B });
			}
			return returnValue.ToArray();
		}

		public void LoadSnesPalette()
		{
			_snes = true;
			//return;

			Colors = new List<Color>();
			for (var i = 0; i < 0x8000; i++) Colors.Add(Get(i));
		}
	}

}
