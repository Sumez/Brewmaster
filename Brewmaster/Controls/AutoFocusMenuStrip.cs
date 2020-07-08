using System.Windows.Forms;

namespace Brewmaster.Controls
{
	public class AutoFocusMenuStrip : MenuStrip
	{
		private const int WM_MOUSEACTIVATE = 0x21;
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE && this.CanFocus && !this.Focused)
			{
				this.FindForm()?.Focus();
			}
			base.WndProc(ref m);
		}
	}
	public class AutoFocusToolStrip : ToolStrip
	{
		private const int WM_MOUSEACTIVATE = 0x21;
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEACTIVATE && this.CanFocus && !this.Focused)
			{
				this.FindForm()?.Focus();
			}
			base.WndProc(ref m);
		}
	}
}
