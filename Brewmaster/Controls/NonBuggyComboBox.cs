using System.Windows.Forms;

namespace Brewmaster.Controls
{
	public class NonBuggyComboBox : ComboBox
	{
		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			Refresh();
		}
	}
}