using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TileImage : IDisposable
	{
		public Bitmap Image { get; private set; }
		public int[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		private TileImage(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new int[width * height];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Image = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
		}

		public void SetPixel(int x, int y, Color colour)
		{
			int index = x + (y * Width);
			int col = colour.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Image.Dispose();
			BitsHandle.Free();
		}

		public static TileImage GetTileImage(byte[] data, int index, List<Color> palette, int bitDepth = 2, ProjectType projectType = ProjectType.Nes)
		{
			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize || index < 0) return null;

			var tile = new TileImage(8, 8);
			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 8; x++)
				{
					var colorIndex = 0;
					for (var j = 0; j <= (bitDepth / 2); j += 2)
					{
						var byte0Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 : y);
						var byte1Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 + 1 : y + 8);

						var byte0 = data[offset + byte0Index];
						var byte1 = data[offset + byte1Index];

						var bit0 = (byte0 >> (7 - x)) & 1;
						var bit1 = (byte1 >> (7 - x)) & 1;

						colorIndex |= bit0 << j;
						colorIndex |= bit1 << (j + 1);
					}
					tile.SetPixel(x, y, palette[colorIndex]);
				}
			}

			return tile;
		}
	}
}
