using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class MapEditorTool {
		public virtual Size Size { get; protected set; }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);
	}

	public class TilePen : MapEditorTool
	{
		public event Action SelectedTileChanged;
		private int _selectedTile;
		public int SelectedTile
		{
			get { return _selectedTile; }
			set
			{
				_selectedTile = value;
				OnSelectedTileChanged();
			}
		}

		protected virtual void OnSelectedTileChanged()
		{
			if (SelectedTileChanged != null) SelectedTileChanged();
		}

		public TilePen()
		{
			Size = new Size(1, 1);
			SelectedTile = 0;
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.PrintTile(x, y, SelectedTile);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SelectedTile = screen.GetTile(x, y);
		}
	}
}