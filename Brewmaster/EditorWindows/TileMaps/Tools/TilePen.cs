using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
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
}