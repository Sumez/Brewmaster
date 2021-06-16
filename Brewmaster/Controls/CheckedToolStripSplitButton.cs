using System;
using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Controls
{
	public class CheckedToolStripSplitButton : ToolStripSplitButton
	{
		private bool _checked;

		public CheckedToolStripSplitButton(string text, Bitmap image, EventHandler onClick, string name) : base(text, image, onClick, name)
		{
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				var oldValue = _checked;
				_checked = value;
				if (value != oldValue) Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_checked) ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, SystemColors.MenuHighlight, ButtonBorderStyle.Solid);
		}
    }
}
