using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Modules
{
	public class ScrollableView : Panel
	{
		public ScrollableView(Control child)
		{
			Controls.Add(child);
			child.Size = new Size(100, 100);
			child.Location = Point.Empty;
			AutoScroll = true;
		}
	}
}
