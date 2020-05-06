using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.Ide
{
	public partial class FloatPanel : Form, IIdeLayoutParent
	{
		public LayoutHandler LayoutHandler { get; set; }

		public FloatPanel(LayoutHandler layoutHandler)
		{
			LayoutHandler = layoutHandler;
			InitializeComponent();
		}


		private HeaderPanel _header;
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);

			var idePanel = Controls.OfType<IdePanel>().FirstOrDefault();
			if (idePanel != null)
			{
				_header = idePanel.Header;
				_header.MouseDownHandler = AscendMouseEvent;
			}
		}

		private void AscendMouseEvent(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_header != null) _header.MouseDownHandler = null;
			base.OnClosing(e);
		}

		private void FloatPanel_Load(object sender, EventArgs e)
		{

		}


		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		private bool _dragging = false;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
			{
				_dragging = true;
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
			if (_dragging) LayoutHandler.DockPanel(this);
			TopMost = false;
			_dragging = false;
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			var cursorPosition = new Point(Cursor.Position.X - Owner.Left, Cursor.Position.Y - Owner.Top);
			//System.Diagnostics.Debug.Write(cursorPosition.X + "," + cursorPosition.Y + Environment.NewLine);

			TopMost = true;
			if (_dragging) LayoutHandler.SuggestDock(cursorPosition, this);
		}

		public void SetChildPanel(IdePanel panel)
		{
			if (Controls.Count >= 0) Controls.Clear();
			panel.Dock = DockStyle.Fill;
			Controls.Add(panel);
		}
		public IdePanel ChildPanel { get { return Controls[0] as IdePanel; } }
	}
	public interface IIdeLayoutParent
	{
		LayoutHandler LayoutHandler { get; }
	}
}
