using System;
using System.Drawing;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public abstract class MapEditorTool
	{
		public event Action Unselected;
		public virtual Size Size { get; protected set; }
		public virtual Image Image { get; protected set; }
		public virtual bool Pixel { get { return false; } }
		public virtual Brush Brush { get; protected set; }
		public virtual bool EditsChr { get {return false;} }

		public abstract void Paint(int x, int y, TileMapScreen screen);
		public abstract void EyeDrop(int x, int y, TileMapScreen screen);

		public virtual void Unselect()
		{
			if (Unselected != null) Unselected();
		}

		public virtual void RefreshImage(Palette attribute) { }

		public virtual void AfterPaint() { }
	}

	public interface IPaletteTool
	{
		Func<int> GetSelectedPalette { get; set; }
		Action<int> SetSelectedPalette { get; set; }
	}
}