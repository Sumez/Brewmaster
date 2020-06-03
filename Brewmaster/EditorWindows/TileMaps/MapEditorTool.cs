using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class MapEditorTool
	{
		public event Action Unselected;
		public virtual Size Size { get; protected set; }
		public virtual Image Image { get; protected set; }
		public virtual bool Pixel { get { return false; } }
		public virtual Brush Brush { get; protected set; }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);

		public virtual void Unselect()
		{
			if (Unselected != null) Unselected();
		}

		public virtual void RefreshImage(Palette attribute) { }

		public virtual void AfterPaint() { }
	}

	public class PixelPen : MapEditorTool
	{
		private readonly TileMap _map;
		private readonly MapEditorState _state;

		public PixelPen(MapEditorState state, TileMap map)
		{
			_state = state;
			_map = map;
			Size = new Size(1, 1);
			Image = _pixelImage = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}
		public override bool Pixel { get { return true; } }
		public override void Paint(int x, int y, TileMapScreen screen)
		{
			var tile = screen.GetTile(x / _map.BaseTileSize.Width, y / _map.BaseTileSize.Height);
			var palette = screen.GetColorTile(x / _map.BaseTileSize.Width, y / _map.BaseTileSize.Height);
			
			screen.Image.SetPixel(x, y, _map.Palettes[palette].Colors[SelectedColor]);
			_state.SetPixel(tile, x % _map.BaseTileSize.Width, y % _map.BaseTileSize.Height, SelectedColor);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			var tile = screen.GetTile(x / _map.BaseTileSize.Width, y / _map.BaseTileSize.Height);
			SelectedColor = _state.GetPixel(tile, x % _map.BaseTileSize.Width, y % _map.BaseTileSize.Height);
		}

		public override void RefreshImage(Palette palette)
		{
			_pixelImage.SetPixel(0, 0, palette.Colors[SelectedColor]);
		}

		public override void AfterPaint()
		{
			_state.OnChrDataChanged();
		}

		public int SelectedColor { get; set; }

		private Bitmap _pixelImage;

	}

	public class MetaValuePen : MapEditorTool
	{
		public MetaValuePen(Size metaValueSize, Func<int, Color> getColor)
		{
			_getColor = getColor;
			Size = metaValueSize;
			SelectedValue = 0;
		}

		public event Action SelectedValueChanged;
		private readonly Func<int, Color> _getColor;
		private int _selectedValue;

		public int SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				if (_selectedValue == value) return;
				_selectedValue = value;
				if (Brush != null) Brush.Dispose();
				Brush = new SolidBrush(_getColor(_selectedValue));
				if (SelectedValueChanged != null) SelectedValueChanged();
			}
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.SetMetaValue(x, y, SelectedValue);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SelectedValue = screen.GetMetaValue(x, y);
		}
	}
	public class TilePen : MapEditorTool, IPaletteTool
	{
		public event Action SelectedTileChanged;
		private int _selectedTile;
		private TileImage _imageTile;

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
			var imageTile = TileImage.GetTileImage(chrData, SelectedTile, map.Palettes[GetSelectedPalette()].Colors);
			Image = imageTile.Image;
			if (_imageTile != null) _imageTile.Dispose();
			_imageTile = imageTile;
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
			var image = new Bitmap(map.BaseTileSize.Width * _metaTileSize, map.BaseTileSize.Height * _metaTileSize, PixelFormat.Format32bppPArgb);
			using (var graphics = Graphics.FromImage(image))
			{
				for (var i = 0; i < MetaTile.Tiles.Length; i++)
				{
					var paletteIndex = MetaTile.Attributes[((i/_metaTileSize) / map.AttributeSize.Height) * map.AttributeSize.Width + ((i % _metaTileSize) / map.AttributeSize.Width)];
					using (var tile = TileImage.GetTileImage(chrData, MetaTile.Tiles[i], map.Palettes[paletteIndex].Colors))
					{
						graphics.DrawImageUnscaled(tile.Image, (i % _metaTileSize) * map.BaseTileSize.Width, (i / _metaTileSize) * map.BaseTileSize.Height);
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