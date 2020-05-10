using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Brewmaster.Modules.OpcodeHelper
{
	public class AddressingModeDescription : UserControl
	{
		private ToolTip _toolTip;
		public SolidBrush ByteBg { get; }
		public SolidBrush CycleBg { get; }

		private List<AddressingModeLine> _lines = new List<AddressingModeLine>();

		protected override void OnPaint(PaintEventArgs e)
		{
			for (var i = 0; i < _lines.Count; i++)
			{
				_lines[i].DrawMode(e.Graphics, i * 17, Font, ByteBg, CycleBg);
			}
		}

		public IEnumerable<AddressingMode> Modes
		{
			set
			{
				_lines.Clear();
				foreach (var mode in value)
				{
					_lines.Add(new AddressingModeLine(mode));
				}
				var height = _lines.Count * 17;
				if (Height != height) Height = height;
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			foreach (var line in _lines)
			{
				if (line.CycleExplanation != null && line.ToolTipBounds.Contains(e.Location))
				{
					_toolTip.SetToolTip(this, line.CycleExplanation);
					return;
				}
			}
			_toolTip.SetToolTip(this, null);
		}

		public AddressingModeDescription(ToolTip toolTip)
		{
			DoubleBuffered = true;
			_toolTip = toolTip;
			Font = new Font("Microsoft Tai Le", 10, FontStyle.Regular, GraphicsUnit.Pixel);
			Height = 17;
			Width = 200;
			ByteBg = new SolidBrush(Color.FromArgb(255, 224, 192));
			CycleBg = new SolidBrush(Color.FromArgb(210, 250, 220));
		}

		private class AddressingModeLine
		{
			private AddressingMode _mode;
			private string _byteText;
			private string _cycleText;
			public string CycleExplanation { get; private set; }
			public Rectangle ToolTipBounds;

			public AddressingModeLine(AddressingMode mode)
			{
				_mode = mode;
				_byteText = string.Format("{0} bytes", _mode.Bytes);

				var match = Regex.Match(_mode.Cycles, "^([0-9]+)(.*)$");
				if (!match.Success)	return;

				var cycleCount = int.Parse(match.Groups[1].Value);
				_cycleText = string.Format("{0} cycles", cycleCount);
				if (match.Groups.Count > 2)
				{
					CycleExplanation = match.Groups[2].Value.Trim();
					var match2 = Regex.Match(CycleExplanation, @"\+[0-9]+(?!.*\+.*)");
					if (match2.Success) _cycleText = string.Format("{0} (+) cycles", cycleCount);
				}

			}
			public void DrawMode(Graphics graphics, int y, Font font, Brush byteBg, Brush cycleBg)
			{
				if (_mode == null) return;
				graphics.DrawString(_mode.Name, font, Brushes.Black, 4, y);

				var bWidth = graphics.MeasureString(_byteText, font).Width + 4;
				graphics.FillRectangle(byteBg, 80, y, bWidth, 13);
				graphics.DrawRectangle(Pens.DarkGray, 80, y, bWidth, 13);
				graphics.DrawString(_byteText, font, Brushes.Black, 82, y);

				if (_cycleText == null) return;

				var cX = (int)(80 + bWidth + 10);
				var cWidth = graphics.MeasureString(_cycleText, font).Width + 4;
				graphics.FillRectangle(cycleBg, cX, y, cWidth, 13);
				graphics.DrawRectangle(Pens.DarkGray, cX, y, cWidth, 13);
				graphics.DrawString(_cycleText, font, Brushes.Black, cX + 2, y);

				ToolTipBounds = new Rectangle(cX, y, (int)cWidth, 13);
			}

		}
	}
}
