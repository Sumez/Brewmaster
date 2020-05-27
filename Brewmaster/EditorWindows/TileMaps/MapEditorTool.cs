using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class MapEditorTool
	{
		public event Action Unselected;
		public virtual Size Size { get; protected set; }
		public Image Image { get; protected set; }
		public virtual bool Pixel { get { return false; } }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);

		public virtual void Unselect()
		{
			if (Unselected != null) Unselected();
		}

		public virtual void RefreshImage(Palette attribute) { }
	}

	public class PixelPen : MapEditorTool
	{
		public PixelPen()
		{
			Size = new Size(1, 1);
			Image = _pixelImage = new Bitmap(1, 1);
		}
		public override bool Pixel { get { return true; } }
		public override void Paint(int x, int y, TileMapScreen screen)
		{
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
		}

		public override void RefreshImage(Palette palette)
		{
			_pixelImage.SetPixel(0, 0, palette.Colors[SelectedColor]);
		}

		public int SelectedColor { get; set; }

		private static Dictionary<Color, Image> _colorImages = new Dictionary<Color, Image>();
		private Bitmap _pixelImage;

		public void PrepareImages(TileMap map)
		{
			foreach (var color in map.Palettes.SelectMany(p => p.Colors))
			{
				if (!_colorImages.ContainsKey(color))
				{
					var image = new Bitmap(1, 1);
					image.SetPixel(0, 0, color);
					_colorImages[color] = image;
				}
			}
		}
	}

	public class TilePen : MapEditorTool, IPaletteTool
	{
		public event Action SelectedTileChanged;
		private int _selectedTile;
		public int SelectedTile
		{
			get { return _selectedTile; }
			set
			{
				_selectedTile = value;
				if (SelectedTileChanged != null) SelectedTileChanged();
			}
		}

		public TilePen()
		{
			Size = new Size(1, 1);
			SelectedTile = 0;
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.PrintTile(x, y, SelectedTile);
			screen.SetColorTile(x, y, GetSelectedPalette());
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SetSelectedPalette(screen.GetColorTile(x, y));
			SelectedTile = screen.GetTile(x, y);
		}
		public Func<int> GetSelectedPalette { get; set; }
		public Action<int> SetSelectedPalette { get; set; }

		public virtual void SetImage(byte[] chrData, TileMap map)
		{
			if (Image != null) Image.Dispose();
			Image = TilePalette.GetTileImage(chrData, SelectedTile, map.Palettes[GetSelectedPalette()].Colors);
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
			       other.Attributes.SequenceEqual(Attributes) &&
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
	}
	public class MetaTilePen : TilePen
	{
		private readonly int _metaTileSize;
		public event Action MetaTileChanged;
		private MetaTile _metaTile;
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
			_metaTileSize = metaTileSize;
			Size = new Size(metaTileSize, metaTileSize);
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.PrintMetaTile(x, y, MetaTile, _metaTileSize);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			MetaTile = screen.GetMetaTile(x, y, _metaTileSize);
		}
		public override void SetImage(byte[] chrData, TileMap map)
		{
			if (MetaTile.Attributes.Length == 1) MetaTile.Attributes[0] = GetSelectedPalette();
			var image = new Bitmap(map.BaseTileSize.Width * _metaTileSize, map.BaseTileSize.Height * _metaTileSize);
			using (var graphics = Graphics.FromImage(image))
			{
				for (var i = 0; i < MetaTile.Tiles.Length; i++)
				{
					var paletteIndex = MetaTile.Attributes[((i/_metaTileSize) / map.AttributeSize.Height) * map.AttributeSize.Width + ((i % _metaTileSize) / map.AttributeSize.Width)];
					using (var tile = TilePalette.GetTileImage(chrData, MetaTile.Tiles[i], map.Palettes[paletteIndex].Colors))
					{
						graphics.DrawImageUnscaled(tile, (i % _metaTileSize) * map.BaseTileSize.Width, (i / _metaTileSize) * map.BaseTileSize.Height);
					}
				}
			}
			if (Image != null) Image.Dispose();
			Image = image;
		}
	}

	public class PalettePen : MapEditorTool, IPaletteTool
	{
		public PalettePen(Size attributeSize)
		{
			Size = attributeSize;
		}
		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.SetColorAttribute(x, y, GetSelectedPalette());
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SetSelectedPalette(screen.GetColorAttribute(x, y));
		}

		public Func<int> GetSelectedPalette { get; set; }
		public Action<int> SetSelectedPalette { get; set; }
	}

	public interface IPaletteTool
	{
		Func<int> GetSelectedPalette { get; set; }
		Action<int> SetSelectedPalette { get; set; }
	}
}