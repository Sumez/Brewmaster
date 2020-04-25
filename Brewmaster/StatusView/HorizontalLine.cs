using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.StatusView
{
	public class HorizontalLine : Control
	{
		private Color _lineColor = SystemColors.ButtonShadow;
		public Color LineColor
		{
			get { return _lineColor;  }
			set { _lineColor = value; }
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, 1, specified);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (var pen = new Pen(_lineColor)) e.Graphics.DrawLine(pen, 0, 0, Width, 0);
		}
	}
}
