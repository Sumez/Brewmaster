using System;
using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Modules.Ppu
{
	public class PaletteViewer : Control
	{
		private Palette _palette;
		private int _defaultIndex = -1;
		public int DefaultIndex {
			get { return _defaultIndex; }
			set
			{
				if (HoverIndex == _defaultIndex) HoverIndex = -1;
				_defaultIndex = value;
				if (HoverIndex < 0) HoverIndex = _defaultIndex;
				Invalidate();
			}
		}
		public int HoverIndex { get; protected set; }
		public int Columns { get; set; }
		public int MaxColors { get; set; } 
		public int CellHeight { get; set; }
		public int CellWidth { get; set; }
		public bool AllowHover { get; set; }
		public int ColorCount { get { return MaxColors > 0 ? MaxColors : Palette.Colors.Count; } }


		public event Action<int> ColorClicked;

		public PaletteViewer()
		{
			AllowHover = true;
			HoverIndex = DefaultIndex;
			CellWidth = 20;
			CellHeight = 20;
			DoubleBuffered = true;
			Font = new Font("Consolas", 12, FontStyle.Bold, GraphicsUnit.Pixel);
			Layout += (s, a) => FitSize();
			SetStyle(ControlStyles.Selectable, true);
		}

		public Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;
				FitSize();
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			var hoverIndex = (e.Location.Y / CellHeight) * Columns + (e.Location.X / CellWidth);
			if (hoverIndex >= ColorCount || e.Location.X > Columns * CellWidth) hoverIndex = DefaultIndex;
			if (hoverIndex == HoverIndex) return;
			HoverIndex = hoverIndex;
			if (ColorClicked != null) Cursor = HoverIndex >= 0 ? Cursors.Hand : Cursors.Default;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			HoverIndex = DefaultIndex;
			Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (HoverIndex >= 0 && e.Button == MouseButtons.Left && ColorClicked != null) ColorClicked(HoverIndex);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			for (var i = 0; i < ColorCount; i++)
			{
				DrawColor(e.Graphics, i, CellWidth * (i % Columns), CellHeight * (i / Columns));
			}

			if (HoverIndex >= 0 && AllowHover)
			{
				var i = HoverIndex;
				e.Graphics.DrawRectangle(Pens.White, CellWidth * (i % Columns), CellHeight * (i / Columns), CellWidth - 1, CellHeight - 1);
				e.Graphics.DrawRectangle(Pens.Black, CellWidth * (i % Columns) + 1, CellHeight * (i / Columns) + 1, CellWidth - 3, CellHeight - 3);
			}
		}

		public int BestWidth
		{
			get { return Columns * CellWidth; }
		}
		public int BestHeight
		{
			get { return (ColorCount / Columns) * CellHeight; }
		}

		protected void FitSize()
		{
			if (Palette == null) return;
			if (Width == BestWidth && Height == BestHeight) return;
			Size = new Size(BestWidth, BestHeight);
		}

		protected virtual void DrawColor(Graphics graphics, int index, int x, int y)
		{
			using (var brush = new SolidBrush(Palette.Colors[index]))
				graphics.FillRectangle(brush, x, y, CellWidth, CellHeight);
		}
	}
}
