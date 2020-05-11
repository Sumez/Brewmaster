using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Modules
{
	public class ScrollableView : Panel
	{
		public ScrollableView(Control child)
		{
			Controls.Add(child);
			child.Location = Point.Empty;
			AutoScroll = true;
		}
	}
}
