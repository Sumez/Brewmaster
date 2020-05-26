using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class MapEditorTool
	{
		public event Action Unselected;
		public virtual Size Size { get; protected set; }
		public Image Image { get; protected set; }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);

		public void Unselect()
		{
			if (Unselected != null) Unselected();
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
	public class MetaTilePen : TilePen
	{
		private readonly int _metaTileSize;
		public event Action MetaTileChanged;
		private int[] _metaTile;
		public int[] MetaTile
		{
			get { return _metaTile; }
			set
			{
				_metaTile = value;
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
			for (var i = 0; i < MetaTile.Length; i++)
			{
				var iX = x * _metaTileSize + (i % _metaTileSize);
				var iY = y * _metaTileSize + (i / _metaTileSize);
				screen.PrintTile(iX, iY, MetaTile[i]);
				screen.SetColorTile(iX, iY, GetSelectedPalette());
			}
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			var metaTileLength = _metaTileSize * _metaTileSize;
			var metaTile = new int[metaTileLength];
			for (var i = 0; i < MetaTile.Length; i++)
			{
				var iX = x * _metaTileSize + (i % _metaTileSize);
				var iY = y * _metaTileSize + (i / _metaTileSize);
				if (i == 0) SetSelectedPalette(screen.GetColorTile(iX, iY));
				metaTile[i] = screen.GetTile(iX, iY);
			}
			MetaTile = metaTile;
		}
		public override void SetImage(byte[] chrData, TileMap map)
		{
			var image = new Bitmap(map.BaseTileSize.Width * _metaTileSize, map.BaseTileSize.Height * _metaTileSize);
			using (var graphics = Graphics.FromImage(image))
			{
				for (var i = 0; i < MetaTile.Length; i++)
				{
					using (var tile = TilePalette.GetTileImage(chrData, MetaTile[i], map.Palettes[GetSelectedPalette()].Colors))
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