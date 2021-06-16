using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class MetaTilePen : TilePen
	{
		public event Action MetaTileChanged;
		private MetaTile _metaTile;
		public int MetaTileSize { get; private set; }

		public MetaTile MetaTile
		{
			get { return _metaTile; }
			set
			{
				_metaTile = value;
				if (_metaTile.Attributes.Length == 1 && (_metaTile.Attributes[0] & 0xff) < 0xff) SetSelectedPalette(_metaTile.Attributes[0] & 0xff);
				if (MetaTileChanged != null) MetaTileChanged();
			}
		}
		
		public MetaTilePen(int metaTileSize)
		{
			MetaTileSize = metaTileSize;
			Size = new Size(metaTileSize, metaTileSize);
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.PrintMetaTile(x, y, MetaTile, MetaTileSize);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			MetaTile = screen.GetMetaTile(x, y, MetaTileSize);
		}
		public override void SetImage(byte[] chrData, TileMap map)
		{
			if (MetaTile.Attributes.Length == 1) MetaTile.Attributes[0] = GetSelectedPalette();
			var image = new Bitmap(map.BaseTileSize.Width * MetaTileSize, map.BaseTileSize.Height * MetaTileSize, PixelFormat.Format32bppPArgb);
			using (var graphics = Graphics.FromImage(image))
			{
				for (var i = 0; i < MetaTile.Tiles.Length; i++)
				{
					var paletteIndex = MetaTile.Attributes[((i/MetaTileSize) / map.AttributeSize.Height) * map.AttributeSize.Width + ((i % MetaTileSize) / map.AttributeSize.Width)];
					using (var tile = TileImage.GetTileImage(chrData, MetaTile.Tiles[i], map.Palettes[paletteIndex].Colors))
					{
						if (tile == null) continue;
						graphics.DrawImageUnscaled(tile.Image, (i % MetaTileSize) * map.BaseTileSize.Width, (i / MetaTileSize) * map.BaseTileSize.Height);
					}
				}
			}
			if (Image != null) Image.Dispose();
			Image = image;
		}
	}

	public class MetaTile : IEquatable<MetaTile>
	{
		public int[] Tiles = new int[0];
		public int[] Attributes = new int[0];
		public int[] MetaValues = new int[0];

		public bool Equals(MetaTile other)
		{
			return other != null &&
			       other.Tiles.SequenceEqual(Tiles) &&
			       ((Attributes.Length == 1 && other.Attributes.Length == 1) || other.Attributes.SequenceEqual(Attributes)) &&
			       other.MetaValues.SequenceEqual(MetaValues);
		}

		public override bool Equals(object obj)
		{
			if (obj is MetaTile metaTile) return Equals(metaTile);
			return false;
		}

		public override int GetHashCode()
		{
			return (Tiles != null && Tiles.Length > 0 ? Tiles[0] : 0) << 8 +
			       (Attributes != null && Attributes.Length > 0 ? Attributes[0] : 0) << 16 +
			       (MetaValues != null && MetaValues.Length > 0 ? MetaValues[0] : 0) << 24;
		}

		internal MetaTile Clone()
		{
			return new MetaTile
			{
				Tiles = Tiles.ToArray(),
				Attributes = Attributes.ToArray(),
				MetaValues = MetaValues.ToArray()
			};
		}
	}
}