using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.MemoryViewer
{
	public partial class HexEditor
	{
		private Font _headerFont;
		private Brush _textBrush;
		private TextFormatFlags _textFlags;
		private float _charWidth;
		private float _charHeight;
		private int _xOffset;
		private int _leftMargin;

		private const int HDistance = 22;
		private const int TopMargin = 20;
		private const int VDistance = 17;

		private void InitDrawing()
		{
			_headerFont = new Font(Font, FontStyle.Bold);
			_textBrush = Brushes.Black;
			using (var graphics = Graphics.FromHwnd(Handle))
			{
				//_textFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
				//var charSize = TextRenderer.MeasureText("0", _headerFont, Size.Empty, _textFlags);
				//_charWidth = charSize.Width;
				//_charHeight = charSize.Height;

				var letterSize = graphics.MeasureString("0", _headerFont, PointF.Empty, StringFormat.GenericTypographic);
				_charWidth = letterSize.Width;
				_charHeight = letterSize.Height;
			}
		}
		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			_leftMargin = _banked ? 70 : 50;

			if (HScrollBar == null) return;
			var fullWidth = _leftMargin + 16 * HDistance;
			var outside = fullWidth - Width;
			HScrollBar.Maximum = Math.Max(0, outside + (HScrollBar.LargeChange - 1)); // largechange-1 accounts for WinForms Scrollbar oddity
			if (HScrollBar.Value > HScrollBar.Maximum - (HScrollBar.LargeChange - 1))
				HScrollBar.Value = HScrollBar.Maximum - (HScrollBar.LargeChange - 1);
		}

		private void DrawHeader(Graphics graphics)
		{
			for (var i = 0; i < 16; i++)
			{
				var hex = Convert.ToString(i, 16).ToUpper();
				graphics.DrawString(hex, _headerFont, _textBrush, i * HDistance + _xOffset + _charWidth, 1);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			_headerFont = new Font(Font, FontStyle.Bold);
			_leftMargin = _banked ? 70 : 50;
			_xOffset = _leftMargin - (HScrollBar != null ? HScrollBar.Value : 0);
			var graphics = e.Graphics;

			graphics.FillRectangle(Brushes.White, _leftMargin, TopMargin, Width - _leftMargin, Height - TopMargin);
			var defaultClip = graphics.Clip;
			var columnClip = graphics.Clip = new Region(new Rectangle(_leftMargin, 0, Width - _leftMargin, Height));

			DrawHeader(graphics);

			var firstVisibleRow = VScrollBar != null ? VScrollBar.Value : 0;
			var visibleRows = (ClientRectangle.Height / VDistance);
			var bankHex = (Convert.ToString(_currentBank, 16).ToUpper()).PadLeft(2, '0') + ":";

			for (var i = 0; i < visibleRows; i++)
			{
				var row = firstVisibleRow + i;
				if (row >= Rows) break;
				var offsetY = i * VDistance + TopMargin;

				graphics.Clip = defaultClip;
				DrawRowMargin(graphics, bankHex, row, offsetY);

				graphics.Clip = columnClip;
				for (var j = 0; j < 16; j++)
				{
					DrawHexValue(graphics, row * 16 + j + (_currentBank * 0x10000), j * HDistance + _xOffset, offsetY);
				}
			}

			_headerFont.Dispose();
		}

		private void DrawHexValue(Graphics graphics, int index, int offsetX, int offsetY)
		{
			var hex = (index >= _data.Length) ? "00" : Convert.ToString(_data[index], 16).ToUpper().PadLeft(2, '0');

			if (index == _editAddress)
			{
				var letterBox = new RectangleF(offsetX + _charWidth * _editIndex + 2, offsetY, _charWidth, _charHeight);
				graphics.FillRectangle(Brushes.LightSteelBlue, letterBox);
				graphics.DrawLine(new Pen(Color.Black, 1), letterBox.Left, letterBox.Bottom, letterBox.Right, letterBox.Bottom);
			}
			else
			{
				Brush bgBrush = null;
				if (_breakpoints.Any(bp => bp.StartAddress == index || bp.StartAddress < index && bp.EndAddress > index))
				{
					bgBrush = Brushes.PaleVioletRed;
					if (index == _focusAddress) bgBrush = Brushes.MediumVioletRed;
				}
				else if (index == _focusAddress) bgBrush = Brushes.LightGray;

				if (bgBrush != null) graphics.FillRectangle(bgBrush, offsetX, offsetY, _charWidth * 2.5f, _charHeight);
			}

			graphics.DrawString(hex, Font, _textBrush, offsetX, offsetY);
		}

		private void DrawRowMargin(Graphics graphics, string bankHex, int row, int offsetY)
		{
			var rowHex = (Convert.ToString(row, 16).ToUpper() + "0").PadLeft(4, '0');
			graphics.DrawString(rowHex, _headerFont, _textBrush, _charWidth + (_banked ? _charWidth * 3 : 0), offsetY);
			if (_banked) graphics.DrawString(bankHex, Font, _textBrush, _charWidth, offsetY);
		}
	}
}
