using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TileImage : FastBitmap
	{
		private TileImage() : base(8, 8, PixelFormat.Format32bppPArgb)
		{
		}
		public static TileImage GetTileImage(byte[] data, int index, List<Color> palette, int bitDepth, TargetPlatform platform = TargetPlatform.Nes)
		{
			if (bitDepth > 2) platform = TargetPlatform.Snes;

			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize || index < 0) return null;

			var tile = new TileImage();
			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 8; x++)
				{
					var colorIndex = 0;
					for (var j = 0; j <= (bitDepth / 2); j += 2)
					{
						var byte0Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 : y);
						var byte1Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 + 1 : y + 8);

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

		public static int GetTilePixel(byte[] data, int index, int x, int y, int bitDepth, TargetPlatform platform = TargetPlatform.Nes)
		{
			if (bitDepth > 2) platform = TargetPlatform.Snes;

			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize || index < 0) return 0;

			var colorIndex = 0;
			for (var j = 0; j <= (bitDepth / 2); j += 2)
			{
				var byte0Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 : y);
				var byte1Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 + 1 : y + 8);

				var byte0 = data[offset + byte0Index];
				var byte1 = data[offset + byte1Index];

				var bit0 = (byte0 >> (7 - x)) & 1;
				var bit1 = (byte1 >> (7 - x)) & 1;

				colorIndex |= bit0 << j;
				colorIndex |= bit1 << (j + 1);
			}

			return colorIndex;
		}

		public static void SetTilePixel(byte[] data, int index, int x, int y, int colorIndex, int bitDepth, TargetPlatform platform = TargetPlatform.Nes)
		{
			if (bitDepth > 2) platform = TargetPlatform.Snes;

			// TODO: Mostly redundant compared to chrpipeline
			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize || index < 0) return;

			for (var j = 0; j <= (bitDepth / 2); j += 2)
			{
				var byte0 = (colorIndex & (1 << j)) == 0 ? 0 : 1;
				var byte1 = (colorIndex & (1 << (j + 1))) == 0 ? 0 : 1;

				var val0 = (byte)(byte0 << (7 - x));
				var val1 = (byte)(byte1 << (7 - x));

				var mask = (byte)((1 << (7 - x)) ^ 0xff);

				var byte0Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 : y);
				var byte1Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 + 1 : y + 8);

				data[offset + byte0Index] = (byte)((data[offset + byte0Index] & mask) | val0);
				data[offset + byte1Index] = (byte)((data[offset + byte1Index] & mask) | val1);
			}
		}

		public void CopyTile(FastBitmap target, int tileX, int tileY)
		{
			var x = tileX * Width;
			var y = tileY * Height;
			for (var yOffset = 0; yOffset < Height; yOffset++)
			{
				CopySlice(target, yOffset * Width, Width, x, y + yOffset);
			}
		}

		public static int GetTileDataLength(int bitDepth = 2)
		{
			return 8 * bitDepth;
		}

		public static void FlipTile(byte[] data, int index, bool vertical, int bitDepth, TargetPlatform platform = TargetPlatform.Nes)
		{
			if (bitDepth > 2) platform = TargetPlatform.Snes;

			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize || index < 0) return;

			if (vertical)
			{
				var flipped = new byte[tileSize];
				for (var plane = 0; plane < bitDepth; plane++)
				for (var i = 0; i < 8; i++)
				{
					// TODO: Snes format with higher bit depth
					flipped[plane * 8 + 7 - i] = data[offset + plane * 8 + i];
				}
				Buffer.BlockCopy(flipped, 0, data, offset, tileSize);
			}
			else
			{
				for (var i = 0; i < tileSize; i++)
				{
					int row = data[offset + i];
					row = (row & 0xF0) >> 4 | (row & 0x0F) << 4;
					row = (row & 0xCC) >> 2 | (row & 0x33) << 2;
					data[offset + i] = (byte)((row & 0xAA) >> 1 | (row & 0x55) << 1);
				}
			}
		}
	}

	public class FastBitmap : IDisposable
	{
		private readonly int[] _data;
		public Bitmap Image { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		public FastBitmap(int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppPArgb)
		{
			Width = width;
			Height = height;
			_data = new int[width * height];
			BitsHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
			Image = new Bitmap(width, height, width * 4, pixelFormat, BitsHandle.AddrOfPinnedObject());
		}

		public void SetPixel(int x, int y, Color color)
		{
			_data[y * Width + x] = color.ToArgb();
		}

		public HashSet<int> GetFillRegion(int startX, int startY, bool connectDiagonally = false)
		{
			var index = startY * Width + startX;
			var matchColor = _data[index];
			var foundValues = new HashSet<int>();
			if (connectDiagonally) TraverseFillDiagonal(index, matchColor, foundValues);
			else TraverseFill(index, matchColor, foundValues);
			return foundValues;
		}

		public void FillRegion(IEnumerable<int> indexes, Color color)
		{
			var value = color.ToArgb();
			foreach (var index in indexes) _data[index] = value;
		}

		private void TraverseFill(int firstIndex, int matchColor, ISet<int> foundValues)
		{
			var stack = new Stack<int>();
			stack.Push(firstIndex);
			while (stack.Count > 0)
			{
				var index = stack.Pop();
				if (index < 0 || index >= _data.Length || _data[index] != matchColor || foundValues.Contains(index)) continue;
				foundValues.Add(index);

				stack.Push(index - Width);
				stack.Push(index + Width);
				stack.Push(index - 1);
				stack.Push(index + 1);
			}
		}
		private void TraverseFillDiagonal(int firstIndex, int matchColor, ISet<int> foundValues)
		{
			var stack = new Stack<int>();
			stack.Push(firstIndex);
			while (stack.Count > 0)
			{
				var index = stack.Pop();
				if (index < 0 || index >= _data.Length || _data[index] != matchColor || foundValues.Contains(index)) continue;
				foundValues.Add(index);

				stack.Push(index - Width);
				stack.Push(index + Width);
				stack.Push(index - 1);
				stack.Push(index + 1);
				stack.Push(index - Width - 1);
				stack.Push(index - Width + 1);
				stack.Push(index + Width - 1);
				stack.Push(index + Width + 1);
			}
		}


		public Color GetPixel(int x, int y)
		{
			return Color.FromArgb(_data[y * Width + x]);
		}

		public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Image.Dispose();
			BitsHandle.Free();
		}

		public void CopySlice(FastBitmap target, int offset, int length, int targetX, int targetY)
		{
			Buffer.BlockCopy(_data, offset * 4, target._data, 4 * (targetY * target.Width + targetX), length * 4);
		}

		public void Clear(Color color)
		{
			var value = color.ToArgb();
			for (var i = 0; i < _data.Length; ++i) _data[i] = value;
		}
	}
}
