using System.Collections.Generic;
using System.Drawing;

namespace Brewmaster.Modules.SpriteList
{
	public class Sprite
	{
		public int Index;
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public int TileIndex;
		public int Palette;
		public int Priority;
		public bool Large;
		public bool FlipH;
		public bool FlipV;
		public bool UseSecondTable;

		public bool Visible
		{
			get {
				if (X + Width <= 0 || X > 255) return false;
				if (Y > 239 && ((Y + Height) & 0xFF) > Y) return false;

				return true;
			}
		}

		public Rectangle Bounds
		{
			get { return new Rectangle(X, Y, Width, Height); }
		}

		public static List<Sprite> GetSnesSprites(byte[] oamRam, int oamMode)
		{
			var sprites = new List<Sprite>();
			for (var i = 0; i < 512; i += 4)
			{

				var sprite = new Sprite();
				sprite.Y = oamRam[i + 1];

				var highTableOffset = i >> 4;
				var shift = ((i >> 2) & 0x03) << 1;
				var highTableValue = (byte)(oamRam[0x200 | highTableOffset] >> shift);
				sprite.Large = ((highTableValue & 0x02) >> 1) != 0;
				sprite.Height = OamSizes[oamMode, sprite.Large ? 1 : 0, 1] << 3;
				sprite.Width = OamSizes[oamMode, sprite.Large ? 1 : 0, 0] << 3;
				
				var sign = (highTableValue & 0x01) << 8;
				sprite.X = ((sign | oamRam[i]) << 23) >> 23;
				sprite.TileIndex = oamRam[i + 2];

				var flags = oamRam[i + 3];
				sprite.UseSecondTable = (flags & 0x01) != 0;
				sprite.Palette = (flags >> 1) & 0x07;
				sprite.Priority = (flags >> 4) & 0x03;
				sprite.FlipH = (flags & 0x40) != 0;
				sprite.FlipV = (flags & 0x80) != 0;

				sprites.Add(sprite);
			}
			return sprites;
		}

		public static List<Sprite> GetNesSprites(byte[] oamRam, bool largeSprites)
		{
			var sprites = new List<Sprite>();
			for (var i = 0; i < 256; i += 4)
			{
				var sprite = new Sprite();
				sprite.Y = oamRam[i];
				sprite.X = oamRam[i + 3];
				sprite.TileIndex = oamRam[i + 1];
				if (largeSprites)
				{
					sprite.UseSecondTable = (sprite.TileIndex & 1) == 1;
					sprite.TileIndex &= 0xFE;
				}
				sprite.Width = 8;
				sprite.Height = largeSprites ? 16 : 8;

				var attributes = oamRam[i + 2];
				sprite.Palette = attributes & 0x03;
				sprite.Priority = (attributes >> 5) & 1;
				sprite.FlipH = (attributes & 0x40) != 0;
				sprite.FlipV = (attributes & 0x80) != 0;

				sprites.Add(sprite);
			}
			return sprites;
		}

		private static readonly byte[,,] OamSizes = new byte[,,] {
			{ { 1, 1 }, { 2, 2 } }, //8x8 + 16x16
			{ { 1, 1 }, { 4, 4 } }, //8x8 + 32x32
			{ { 1, 1 }, { 8, 8 } }, //8x8 + 64x64
			{ { 2, 2 }, { 4, 4 } }, //16x16 + 32x32
			{ { 2, 2 }, { 8, 8 } }, //16x16 + 64x64
			{ { 4, 4 }, { 8, 8 } }, //32x32 + 64x64
			{ { 2, 4 }, { 4, 8 } }, //16x32 + 32x64
			{ { 2, 4 }, { 4, 4 } }  //16x32 + 32x32
		};
	}
}
