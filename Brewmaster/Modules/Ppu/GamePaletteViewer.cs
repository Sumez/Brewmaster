using System;
using System.Collections.Generic;
using System.Drawing;

namespace Brewmaster.Modules.Ppu
{
	public class GamePaletteViewer : PaletteViewer
	{
		private int[] _paletteIndexes;
		public Palette SourcePalette { get; set; }

		public GamePaletteViewer(Events events)
		{
			Columns = 4;
			CellWidth = 40;
			CellHeight = 40;

			Palette = new Palette();
			events.EmulationStateUpdate += state =>
			{
				Palette.Colors = new List<Color>(state.Palette.Length);
				_paletteIndexes = state.Palette;
				for (var i = 0; i < state.Palette.Length; i++)
				{
					Palette.Colors.Add(SourcePalette.Colors[_paletteIndexes[i]]);
				}
				Invalidate();
			};
		}

		protected override void DrawColor(Graphics graphics, int index, int x, int y)
		{
			base.DrawColor(graphics, index, x, y);

			var brightness = Palette.Colors[index].GetBrightness();
			var hex = Convert.ToString(_paletteIndexes[index], 16).PadLeft(2, '0').ToUpper();
			graphics.DrawString(hex, Font, brightness >= 0.7f ? Brushes.Black : Brushes.White, x + 2, y + 2);
		}
	}
}
