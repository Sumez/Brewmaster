using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class MapEditorTool {
		public virtual Size Size { get; protected set; }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);
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
			screen.SetColorTile(x, y, GetSelectedPalette());
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SelectedTile = screen.GetTile(x, y);
			SetSelectedPalette(screen.GetColorTile(x, y));
		}
		public Func<int> GetSelectedPalette { get; set; }
		public Action<int> SetSelectedPalette { get; set; }
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