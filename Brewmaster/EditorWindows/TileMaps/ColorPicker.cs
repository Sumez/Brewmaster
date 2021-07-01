using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public abstract class ColorPicker : Form
	{
		public event Action ColorChanged;
		private Color _color;
		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				UpdateSelectedColor();
				if (ColorChanged != null) ColorChanged();
			}
		}

		protected abstract void UpdateSelectedColor();

		protected ColorPicker()
		{
			FormBorderStyle = FormBorderStyle.FixedSingle;

			ControlBox = false;
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterScreen;

			LostFocus += (s, a) => Close();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			Deactivate += (s, a) => Close();
		}
	}

	public class SnesColorPicker : ColorPicker
	{
		public static Palette SnesPalette;

		private readonly TrackBar _red;
		private readonly TrackBar _green;
		private readonly TrackBar _blue;
		private readonly Panel _colorPreview;

		public SnesColorPicker()
		{
			var labelR = new Label { Text = "Red:" };
			var labelG = new Label { Text = "Green:" };
			var labelB = new Label { Text = "Blue:" };
			_red = new TrackBar
			{
				AutoSize = false,
				TickStyle = TickStyle.None,
				Width = 100,
				Height = 20,
				Location = new Point(35, 5),
				Minimum = 0,
				Maximum = 31
			};
			_green = new TrackBar
			{
				AutoSize = false,
				TickStyle = TickStyle.None,
				Width = 100,
				Height = 20,
				Location = new Point(35, 25),
				Minimum = 0,
				Maximum = 31
			};
			_blue = new TrackBar
			{
				AutoSize = false,
				TickStyle = TickStyle.None,
				Width = 100,
				Height = 20,
				Location = new Point(35, 45),
				Minimum = 0,
				Maximum = 31
			};
			_colorPreview = new Panel
			{
				Top = _red.Top,
				Left = _red.Left + _red.Width + 10,
				Width = _blue.Top - _red.Top + _blue.Height,
				Height = _blue.Top - _red.Top + _blue.Height,
				BorderStyle = BorderStyle.FixedSingle,
				Cursor = Cursors.Hand
			};
			labelR.Left = labelG.Left = labelB.Left = 0;
			labelR.Width = labelG.Width = labelB.Width = 30;
			labelR.Height = labelG.Height = labelB.Height = 18;
			labelR.Top = _red.Top;
			labelG.Top = _green.Top;
			labelB.Top = _blue.Top;

			Width = _colorPreview.Left + _colorPreview.Width + 15;
			Height = 75;
			Controls.AddRange(new Control[] { labelR, labelG, labelB, _red, _green, _blue, _colorPreview });

			_colorPreview.Click += (o, a) => Close();
			_red.ValueChanged += (o, a) => { Color = Color.FromArgb(Palette.To8BitChannel(_red.Value), Color.G, Color.B); };
			_green.ValueChanged += (o, a) => { Color = Color.FromArgb(Color.R, Palette.To8BitChannel(_green.Value), Color.B); };
			_blue.ValueChanged += (o, a) => { Color = Color.FromArgb(Color.R, Color.G, Palette.To8BitChannel(_blue.Value)); };
		}
		protected override void UpdateSelectedColor()
		{
			_red.Value = Palette.From8BitChannel(Color.R);
			_green.Value = Palette.From8BitChannel(Color.G);
			_blue.Value = Palette.From8BitChannel(Color.B);

			_colorPreview.BackColor = Color;
		}
	}

	public class NesColorPicker : ColorPicker
	{
		public static Palette NesPalette;
		private readonly ConsolePaletteViewer _paletteView;

		public NesColorPicker()
		{
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
		}

		protected override void UpdateSelectedColor()
		{
			_paletteView.DefaultIndex = NesPalette.Colors.IndexOf(Color);
		}
	}
}
