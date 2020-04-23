using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brewmaster.StatusView
{
	public class ExpandButton : Button
	{
		private Bitmap _collapsedIcon;
		private Bitmap _expandedIcon;
		private Panel _expandPanel;

		public Panel ExpandPanel
		{
			get
			{
				return _expandPanel;
			}
			set
			{
				_expandPanel = value;
				if (_expandPanel != null) _expandPanel.Visible = ExpandedState;
			}
		}

		public string ButtonText { get; set; }
		public bool ExpandedState { get; set; }

		public override string Text { get { return ""; } set {} }

		public ExpandButton()
		{
			_collapsedIcon = new Bitmap(Properties.Resources.collapsed);
			_expandedIcon = new Bitmap(Properties.Resources.expanded);
		}
		protected override bool ShowFocusCues { get { return false; } }
		protected override void OnClick(EventArgs e)
		{
			ExpandedState = !ExpandedState;
			if (_expandPanel != null) _expandPanel.Visible = ExpandedState;
			base.OnClick(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawImageUnscaled(ExpandedState ? _expandedIcon : _collapsedIcon, 2, 3);
			e.Graphics.DrawString(ButtonText, Font, new SolidBrush(ForeColor), 14, 3);
		}

		protected override void Dispose(bool disposing)
		{
			_collapsedIcon.Dispose();
			_expandedIcon.Dispose();
			base.Dispose(disposing);
		}
	}
}
