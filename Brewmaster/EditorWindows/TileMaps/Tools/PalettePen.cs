using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
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
}