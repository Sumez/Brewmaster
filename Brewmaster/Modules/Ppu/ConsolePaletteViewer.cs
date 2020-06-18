using System;
using System.Drawing;

namespace Brewmaster.Modules.Ppu
{

	public class ConsolePaletteViewer : PaletteViewer
	{
		public ConsolePaletteViewer()
		{
			Columns = 16;
		}
		public ConsolePaletteViewer(Events events) : this()
		{

			// TODO configurable palettes:
			/*ColorClicked += index =>
			{
				using (var colorDialog = new ColorDialog { Color = Palette.Colors[index] })
				{
					colorDialog.ShowDialog(this);
					Palette.Colors[index] = colorDialog.Color;
				}
				Invalidate();
			};*/
			events.PlatformChanged += type =>
			{
				Columns = type == ProjectModel.TargetPlatform.Snes ? (0x20 * 8) : 16;
				CellWidth = CellHeight = type == ProjectModel.TargetPlatform.Snes ? 3 : 20;

				Invalidate();
				FitSize();
			};
		}
		protected override void DrawColor(Graphics graphics, int index, int x, int y)
		{
			base.DrawColor(graphics, index, x, y);
			if (index != HoverIndex) return;

			var brightness = Palette.Colors[index].GetBrightness();
			var hex = Convert.ToString(index, 16).PadLeft(2, '0').ToUpper();
			graphics.DrawString(hex, Font, brightness >= 0.7f ? Brushes.Black : Brushes.White, x + 1, y + 3);
		}
	}
}
