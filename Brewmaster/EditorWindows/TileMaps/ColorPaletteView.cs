using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ColorPaletteView : Control
	{
		public event Action<int> SelectedPaletteChanged;
		public event Action PalettesModified;
		public event Action<int, int> SelectedColorIndexChanged;

		public ColorPaletteView(List<Palette> palettes)
		{
			_paletteControls = new List<ColorPalette>();

			for (var i = 0; i < palettes.Count; i++)
			{
				var paletteIndex = i;
				var paletteControl = new ColorPalette(4) {Palette = palettes[i], Margin = new Padding(0, 3, 3, 0)};
				_paletteControls.Add(paletteControl);

				if (i == 0) paletteControl.Selected = true;
				paletteControl.Click += (s, a) => SelectedPaletteIndex = paletteIndex;
				paletteControl.PaletteChanged += (changedPalette) =>
				{
					foreach (var p in _paletteControls.Where(p => p.Palette != changedPalette))
					{
						p.Palette.Colors[0] = changedPalette.Colors[0];
						p.Invalidate();
					}

					if (PalettesModified != null) PalettesModified();
				};
				paletteControl.SelectedColorIndexChanged += () =>
				{
					if (SelectedColorIndexChanged != null)
						SelectedColorIndexChanged(paletteControl.SelectedColorIndex, paletteIndex);
				};
			}

			var layoutPanel = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top };
			foreach (var paletteViewer in _paletteControls) layoutPanel.Controls.Add(paletteViewer);

			Controls.Add(layoutPanel);

			layoutPanel.Resize += (s, a) =>
			{
				if (layoutPanel.Height != Height)
				{
					Height = layoutPanel.Height;
					if (Parent != null && Parent.Parent != null) Parent.Parent.ResumeLayout(true);
				}
			};
		}

		private int _selectedPaletteIndex;
		private readonly List<ColorPalette> _paletteControls;

		public int SelectedPaletteIndex
		{
			get { return _selectedPaletteIndex; }
			set
			{
				if (_selectedPaletteIndex == value) return;
				_selectedPaletteIndex = value;
				for (var i = 0; i < _paletteControls.Count; i++)
				{
					_paletteControls[i].Selected = i == _selectedPaletteIndex;
					_paletteControls[i].Invalidate();
				}
				if (SelectedPaletteChanged != null) SelectedPaletteChanged(_selectedPaletteIndex);

			}
		}

		public void ImportPaletteData(FileStream stream)
		{
			foreach (var paletteControl in _paletteControls)
			{
				if (stream.Position >= stream.Length) break;
				paletteControl.Palette.Colors = new List<Color>();
				for (var i = 0; i < 4; i++)
				{
					paletteControl.Palette.Colors.Add(NesColorPicker.NesPalette.Colors[stream.ReadByte()]);
				}
				paletteControl.Invalidate();
			}
			if (PalettesModified != null) PalettesModified();
		}
	}

	public class ColorPalette : Control
	{
		public event Action<Palette> PaletteChanged;
		public event Action SelectedColorIndexChanged;
		public int SelectedColorIndex { get; set; }
		private bool _selected;
		public bool Selected
		{
			get { return _selected;}
			set { _paletteViewer.AllowHover = _selected = value; _paletteViewer.Invalidate(); }
		}
		private readonly PaletteViewer _paletteViewer;

		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			base.OnInvalidated(e);
			_paletteViewer.Invalidate();
		}

		public ColorPalette(int colorCount)
		{
			Controls.Add(_paletteViewer = new PaletteViewer
			{
				AllowHover = false,
				Columns = colorCount,
				Width = colorCount * 16,
				Height = 16,
				Top = 1,
				Left = 1,
				CellHeight = 16,
				CellWidth = 16
			});
			Width = colorCount * 16 + 2;
			Height = 18;

			_paletteViewer.Click += (s, a) => OnClick(a);
			_paletteViewer.DoubleClick += (s, a) => OnDoubleClick(a);
		}

		protected override void OnClick(EventArgs e)
		{
			if (_paletteViewer.HoverIndex < 0)
			{
				base.OnClick(e);
				return;
			}
			
			if (Selected && _paletteViewer.HoverIndex == SelectedColorIndex)
			{
				PickNewColor();
			}
			SelectedColorIndex = _paletteViewer.HoverIndex;
			if (SelectedColorIndexChanged != null) SelectedColorIndexChanged();
			base.OnClick(e);
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			PickNewColor();
			base.OnDoubleClick(e);
		}

		private void PickNewColor()
		{
			var paletteIndex = _paletteViewer.HoverIndex;
			var colorPicker = new NesColorPicker();
			colorPicker.Color = Palette.Colors[paletteIndex];
			colorPicker.StartPosition = FormStartPosition.Manual;
			colorPicker.Location = PointToScreen(new Point(0, -colorPicker.Height));
			colorPicker.Show();
			colorPicker.FormClosing += (s, a) =>
			{
				Palette.Colors[paletteIndex] = colorPicker.Color;
				_paletteViewer.Invalidate();
				if (PaletteChanged != null) PaletteChanged(Palette);
			};
		}

		public Palette Palette
		{
			get { return _paletteViewer.Palette; }
			set { _paletteViewer.Palette = value; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawRectangle(Selected ? Pens.Black : Pens.White, 0, 0, Width-1, Height-1);
		}
	}
}
