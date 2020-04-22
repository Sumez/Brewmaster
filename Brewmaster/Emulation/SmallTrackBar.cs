using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BrewMaster.Emulation
{
	public class SmallTrackBar : TrackBar
	{
		public SmallTrackBar()
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
			//SetStyle(ControlStyles.UserPaint, true);
		}

		private int _oldValue;
		public new event Action ValueChanged;
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_oldValue != Value)
			{
				_oldValue = Value;
				if (ValueChanged != null) ValueChanged();
			}

			base.OnPaint(e);

			var trackBarWidth = Width - 16;
			var thumbWidth = 7;
			//TrackBarRenderer.DrawBottomPointingThumb(e.Graphics, e.ClipRectangle, TrackBarThumbState.Normal);
			//base.OnPaint(e);
			var position = Maximum == 0 ? 0 : (int)Math.Floor(((Value - Minimum) / (double)Maximum) * (double) (trackBarWidth - thumbWidth));
			TrackBarRenderer.DrawHorizontalTrack(e.Graphics, new Rectangle(8, 6, trackBarWidth, 3));
			TrackBarRenderer.DrawHorizontalThumb(e.Graphics, new Rectangle(8 + position, 2, thumbWidth, 12), TrackBarThumbState.Normal);
		}

		protected override void OnScroll(EventArgs e)
		{
			base.OnScroll(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}

		protected override void OnValueChanged(EventArgs e)
		{
			base.OnValueChanged(e);
		}

		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			base.OnInvalidated(e);
		}
	
	}
}
