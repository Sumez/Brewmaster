using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules.Ppu
{
	public class GamePaletteViewer : PaletteViewer
	{
		private List<int> _paletteIndexes;
		private ProjectType _type;
		private int _bytesPerColor = 1;
		private ToolTip _toolTip;

		public Palette SourcePalette { get; set; }

		public GamePaletteViewer(Events events)
		{
			Columns = 4;
			CellWidth = 40;
			CellHeight = 40;

			Palette = new Palette();
			events.EmulationStateUpdate += state =>
			{
				if (_type != state.Type)
				{
					_type = state.Type;
					_bytesPerColor = _type == ProjectType.Snes ? 2 : 1;
					CellWidth = CellHeight = _type == ProjectType.Snes ? 20 : 40;
					Columns = _type == ProjectType.Nes ? 4 : 16;
				}
				var paletteSource = state.Memory.CgRam;
				Palette.Colors = new List<Color>(paletteSource.Length / _bytesPerColor);

				if (_bytesPerColor == 1) _paletteIndexes = paletteSource.Select(v => (int)v).ToList();
				else
				{
					_paletteIndexes = new List<int>(paletteSource.Length / _bytesPerColor);
					for (var i = 0; i < paletteSource.Length; i += _bytesPerColor)
					{
						var paletteIndex = 0;
						for (var j = 0; j < _bytesPerColor; j++)
						{
							paletteIndex |= paletteSource[i + j] << (8 * j);
						}
						_paletteIndexes.Add(paletteIndex);
					}
				}
				
				for (var i = 0; i < _paletteIndexes.Count; i ++)
				{
					Palette.Colors.Add(SourcePalette.Get(_paletteIndexes[i]));
				}
				Invalidate();
				FitSize();
			};
			_toolTip = new ToolTip();
			_toolTip.AutoPopDelay = 5000;
			_toolTip.InitialDelay = 0;
			_toolTip.ReshowDelay = 100;
			_toolTip.ShowAlways = true;
			_toolTip.SetToolTip(this, "");

		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (HoverIndex < 0)
			{
				_toolTip.Active = false;
				return;
			}
			var indexHex = Convert.ToString(HoverIndex, 16).PadLeft(2, '0').ToUpper();
			var colorHex = Convert.ToString(_paletteIndexes[HoverIndex], 16).PadLeft(_type == ProjectType.Snes ? 4 : 2, '0').ToUpper();
			_toolTip.Active = true;
			_toolTip.Show(string.Format("${0}: ${1}", indexHex, colorHex), this, e.Location.X + 20, e.Location.Y + 2);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_toolTip.Active = false;
		}

		protected override void DrawColor(Graphics graphics, int index, int x, int y)
		{
			base.DrawColor(graphics, index, x, y);

			var brightness = Palette.Colors[index].GetBrightness();
			var hex = Convert.ToString(_paletteIndexes[index], 16).PadLeft(2, '0').ToUpper();
			if (CellWidth >= 30) graphics.DrawString(hex, Font, brightness >= 0.7f ? Brushes.Black : Brushes.White, x + 2, y + 2);
		}
	}
}
