using System;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class NesColorPicker : Form
	{
		public static Palette NesPalette;
		private readonly ConsolePaletteViewer _paletteView;

		public NesColorPicker()
		{
			FormBorderStyle = FormBorderStyle.FixedSingle;

			_paletteView = new ConsolePaletteViewer {Dock = DockStyle.Fill};
			Controls.Add(_paletteView);
			_paletteView.Palette = NesPalette;
			Width = _paletteView.Width + 6;
			Height = _paletteView.Height + 6;
			
			_paletteView.ColorClicked += index =>
			{
				Color = NesPalette.Colors[index];
				Close();
			};

			ControlBox = false;
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterScreen;

			LostFocus += (s, a) => Close();
		}

		private Color _color;
		public Color Color {
			get { return _color; }
			set
			{
				_color = value;
				_paletteView.DefaultIndex = NesPalette.Colors.IndexOf(_color);
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			Deactivate += (s, a) => Close();
		}
	}
}
