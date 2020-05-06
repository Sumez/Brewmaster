using System;
using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Modules.Ppu
{
	public class PaletteViewer : Control
	{
		private Palette _palette;
		protected int HoverIndex { get; private set; }
		public int Columns { get; set; }
		public int CellHeight { get; set; }
		public int CellWidth { get; set; }

		protected event Action<int> ColorClicked;

		public PaletteViewer()
		{
			HoverIndex = -1;
			CellWidth = 20;
			CellHeight = 20;
			DoubleBuffered = true;
			Font = new Font("Consolas", 12, FontStyle.Bold, GraphicsUnit.Pixel);
		}

		public Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			var hoverIndex = (e.Location.Y / CellHeight) * Columns + (e.Location.X / CellWidth);
			if (hoverIndex >= Palette.Colors.Count || e.Location.X > Columns * CellWidth) hoverIndex = -1;
			if (hoverIndex == HoverIndex) return;
			HoverIndex = hoverIndex;
			if (ColorClicked != null) Cursor = HoverIndex >= 0 ? Cursors.Hand : Cursors.Default;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			HoverIndex = -1;
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
			for (var i = 0; i < Palette.Colors.Count; i++)
			{
				DrawColor(e.Graphics, i, CellWidth * (i % Columns), CellHeight * (i / Columns));
			}

			if (HoverIndex >= 0)
			{
				var i = HoverIndex;
				e.Graphics.DrawRectangle(Pens.White, CellWidth * (i % Columns), CellHeight * (i / Columns), CellWidth - 1, CellHeight - 1);
				e.Graphics.DrawRectangle(Pens.Black, CellWidth * (i % Columns) + 1, CellHeight * (i / Columns) + 1, CellWidth - 3, CellHeight - 3);
			}
		}

		protected virtual void DrawColor(Graphics graphics, int index, int x, int y)
		{
			using (var brush = new SolidBrush(Palette.Colors[index]))
				graphics.FillRectangle(brush, x, y, CellWidth, CellHeight);
		}
	}
}
