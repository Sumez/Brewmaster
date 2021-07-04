using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ColorPaletteView : Control
	{
		public event Action<int> SelectedPaletteChanged;
		public event Action PalettesModified;
		public event Action<int, int> SelectedColorIndexChanged;

		public ColorPaletteView(List<Palette> palettes, int colorCount, TargetPlatform platform)
		{
			_paletteControls = new List<ColorPalette>();

			_layoutPanel = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top };
			RefreshPaletteView(palettes, colorCount, platform);

			Controls.Add(_layoutPanel);

			_layoutPanel.Resize += (s, a) =>
			{
				if (_layoutPanel.Height != Height)
				{
					Height = _layoutPanel.Height;
					if (Parent != null && Parent.Parent != null) Parent.Parent.ResumeLayout(true);
				}
			};
		}

		public void RefreshPaletteView(List<Palette> palettes, int colorCount, TargetPlatform platform)
		{
			SuspendLayout();
			_paletteControls.Clear();

			for (var i = 0; i < palettes.Count; i++)
			{
				var paletteIndex = i;
				var paletteControl = new ColorPalette(colorCount, platform) { Palette = palettes[i], Margin = new Padding(0, 3, 3, 0) };
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
					if (SelectedColorIndexChanged != null) SelectedColorIndexChanged(paletteControl.SelectedColorIndex, paletteIndex);
				};
			}
			_layoutPanel.Controls.Clear();
			_layoutPanel.Controls.AddRange(_paletteControls.Select(p => p as Control).ToArray());
			ResumeLayout();
		}

		private int _selectedPaletteIndex;
		private readonly List<ColorPalette> _paletteControls;
		private readonly FlowLayoutPanel _layoutPanel;

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

		public void ShowSelectedColorIndex(int stateColorIndex)
		{
			for (var i = 0; i < 4; i++)
			{
				_paletteControls[i].SelectedColorIndex = stateColorIndex;
			}
		}
	}

	public class ColorPalette : Control
	{
		public event Action<Palette> PaletteChanged;
		public event Action SelectedColorIndexChanged;

		public int SelectedColorIndex
		{
			get { return _selectedColorIndex; }
			set
			{
				_selectedColorIndex = value;
				_paletteViewer.DefaultIndex = value;
				_paletteViewer.Invalidate();
			}
		}

		private int _selectedColorIndex;
		private bool _selected;
		public bool Selected
		{
			get { return _selected;}
			set { _paletteViewer.AllowHover = _selected = value; _paletteViewer.Invalidate(); }
		}
		private readonly PaletteViewer _paletteViewer;
		private readonly TargetPlatform _platform;

		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			base.OnInvalidated(e);
			_paletteViewer.Invalidate();
		}

		public ColorPalette(int colorCount, TargetPlatform platform)
		{
			_platform = platform;
			Controls.Add(_paletteViewer = new PaletteViewer
			{
				AllowHover = false,
				Columns = colorCount,
				MaxColors = colorCount,
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
			ColorPicker colorPicker = _platform == TargetPlatform.Nes ? new NesColorPicker() : new SnesColorPicker() as ColorPicker;
			colorPicker.Color = Palette.Colors[paletteIndex];
			colorPicker.StartPosition = FormStartPosition.Manual;
			colorPicker.Location = PointToScreen(new Point(0, -colorPicker.Height));
			colorPicker.Show();

			void updateColor()
			{
				if (colorPicker.Color == Palette.Colors[paletteIndex]) return;

				Palette.Colors[paletteIndex] = colorPicker.Color;
				_paletteViewer.Invalidate();
				if (PaletteChanged != null) PaletteChanged(Palette);
			}
			colorPicker.ColorChanged += updateColor;
			colorPicker.FormClosing += (s, a) => updateColor();
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
