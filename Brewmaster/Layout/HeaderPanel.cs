using System;
using System.Drawing;
using System.Windows.Forms;
using BrewMaster.ProjectExplorer;

namespace BrewMaster.Ide
{

	public class HeaderPanel : Panel
    {
	    public const int DragTreshold = 20;
		private Label _label;
        public HeaderPanel()
        {
	        InitializeComponent();

	        _label = new Label();
	        Label = "test";
	        _label.ForeColor = SystemColors.ControlText;
	        _label.Dock = DockStyle.Fill;
	        _label.Padding = new Padding(3, 3, 0, 0);
	        _label.MouseDown += (sender, args) => OnMouseDown(args);
	        Controls.Add(_label);

	        var mouseHandler = new OsFeatures.GlobalMouseHandler();
	        mouseHandler.MouseMoved += MouseMoved;
	        mouseHandler.MouseUp += l => _dragging = false;
	        mouseHandler.MouseDown += LeftMouseDown;
	        Application.AddMessageFilter(mouseHandler);
			ControlRemoved += (s, a) => Application.RemoveMessageFilter(mouseHandler);
		}


	    private bool _dragging = false;
	    private Point _dragOffset;
	    protected bool LeftMouseDown(Point location)
	    {
		    if (!(FindForm() is MainForm)) return false;

		    var control = OsFeatures.GlobalMouseHandler.GetCurrentControl();
		    if (control != null)
		    {
			    while (control != null && control != this) control = control.Parent;
			    if (control == null) return false;
		    }

			if (!Bounds.Contains(Parent.PointToClient(location))) return false;

			_dragging = true;
		    _dragOffset = location;
		    return false;
	    }

	    protected bool MouseMoved(Point location)
	    {
		    if (!_dragging) return false;

		    var delta = new Point(Cursor.Position.X - _dragOffset.X, Cursor.Position.Y - _dragOffset.Y);

		    if (Math.Abs(delta.X) > DragTreshold || Math.Abs(delta.Y) > DragTreshold)
		    {
			    _dragging = false;
			    var mainForm = FindForm() as MainForm;
			    if (mainForm == null) return false;

			    mainForm.ReleasePanel(this, delta);
			    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)); // Forces form drag immediately
		    }

		    return false;
	    }

		protected override void OnCreateControl()
	    {
		    base.OnCreateControl();
		    Height = 20;
	    }

	    public string Label
	    {
		    get { return _label.Text; }
		    set { _label.Text = value; }
	    }
        protected override void OnPaint(PaintEventArgs e)
        {
	        Height = 20;
            base.OnPaint(e);
	        e.Graphics.DrawLine(new Pen(SystemColors.ActiveBorder, 1), ClientRectangle.Left, ClientRectangle.Bottom-1, ClientRectangle.Right, ClientRectangle.Bottom-1);
        }

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.BackColor = SystemColors.Control;
			this.Size = new Size(200, 20);
			this.ResumeLayout(false);
		}
	}
}
