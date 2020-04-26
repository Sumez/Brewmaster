using System;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.ProjectExplorer;

namespace Brewmaster.Ide
{

	public class HeaderPanel : Panel
    {
	    public MouseEventHandler MouseDownHandler { get; set; }
		public const int DragTreshold = 20;
		private Label _label;
	    private FlowLayoutPanel _tabPanel;
        public HeaderPanel()
        {
	        InitializeComponent();

	        _tabPanel = new FlowLayoutPanel();
	        _tabPanel.Dock = DockStyle.Fill;
	        _tabPanel.Visible = false;
	        _tabPanel.MouseDown += (sender, args) =>
	        {
		        OnMouseDown(args);
	        };
	        _tabPanel.Padding = Padding.Empty;
			Padding = Padding.Empty;
	        _tabPanel.Margin = Padding.Empty;
			Controls.Add(_tabPanel);

	        _label = new Label();
	        _label.ForeColor = SystemColors.ControlText;
	        _label.Dock = DockStyle.Fill;
	        _label.Padding = new Padding(3, 3, 0, 0);
	        _label.MouseDown += (sender, args) => OnMouseDown(args);
	        Controls.Add(_label);

			var mouseHandler = new OsFeatures.GlobalMouseHandler();
	        mouseHandler.MouseMoved += MouseMoved;
	        mouseHandler.MouseUp += l => { _dragging = null; return false; };
	        mouseHandler.MouseDown += LeftMouseDown;
	        Application.AddMessageFilter(mouseHandler);
			ControlRemoved += (s, a) => Application.RemoveMessageFilter(mouseHandler);
		}


		private IdePanel _dragging;
	    private Point _dragOffset;
	    protected bool LeftMouseDown(Point location)
	    {
		    if (Parent == null) return false;
		    var control = OsFeatures.GlobalMouseHandler.GetCurrentControl();
		    if (control != null)
		    {
			    while (control != null && control != this) control = control.Parent;
			    if (control == null) return false;
		    }

			if (!Bounds.Contains(Parent.PointToClient(location))) return false;

		    if ((FindForm() is MainForm)) _dragging = Parent as IdePanel;
		    _dragOffset = location;
		    if (Parent is IdeGroupedPanel groupedPanel)
			    foreach (var tab in groupedPanel.Tabs)
			    {
				    if (tab.Button.Bounds.Contains(_tabPanel.PointToClient(location)))
					    _dragging = tab.Panel;
			    }
			
			return false;
	    }

	    protected bool MouseMoved(Point location)
	    {
		    if (_dragging == null) return false;

		    var delta = new Point(Cursor.Position.X - _dragOffset.X, Cursor.Position.Y - _dragOffset.Y);

		    if (Math.Abs(delta.X) > DragTreshold || Math.Abs(delta.Y) > DragTreshold)
		    {
			    var draggingPanel = _dragging;
			    _dragging = null;
			    var form = FindForm();
			    if (form is MainForm mainForm)
			    {
				    mainForm.ReleasePanel(draggingPanel, PointToScreen(delta));
			    }
				else if (form is FloatPanel floatPanel)
			    {
					floatPanel.ReleasePanel(draggingPanel, PointToScreen(delta));
				}
			    else return false;
				draggingPanel.Header.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)); // Forces form drag immediately
		    }

		    return false;
	    }

	    protected override void OnMouseDown(MouseEventArgs e)
	    {
		    base.OnMouseDown(e);
		    if (MouseDownHandler != null) MouseDownHandler(this, e);
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

	    /*protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
			e.Graphics.DrawLine(new Pen(SystemColors.ActiveBorder, 1), ClientRectangle.Left, ClientRectangle.Bottom-1, ClientRectangle.Right, ClientRectangle.Bottom-1);
        }*/

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.BackColor = SystemColors.Control;
			this.Size = new Size(200, 20);
			this.ResumeLayout(false);
		}

	    public IdeTab AddTab(IdePanel panel)
	    {
		    return CreateTabButton(panel);
		}
	    public event Action<IdeTab> TabChange;

		private IdeTab CreateTabButton(IdePanel panel)
		{
			var tab = new IdeTab(this, panel);
			SuspendLayout();
			_tabPanel.Controls.Add(tab.Button);
		    _label.Visible = false;
		    _tabPanel.Visible = true;
			ResumeLayout();
		    return tab;
	    }

	    protected virtual void OnTabChange(IdeTab tab)
	    {
		    if (TabChange != null) TabChange(tab);
	    }

	    public IdeTab InsertTab(int indexValue, IdePanel panel)
	    {
		    var tab = CreateTabButton(panel);
		    _tabPanel.Controls.SetChildIndex(tab.Button, indexValue);
		    return tab;
	    }

	    public void RemoveTab(IdeTab tab)
	    {
		    if (_tabPanel.Controls.Contains(tab.Button)) _tabPanel.Controls.Remove(tab.Button);
	    }
    }

	public class IdeTab
	{
		private RadioButton _tabButton;
		public IdePanel Panel { get; private set; }
		public Control Button { get { return _tabButton; } }
		public event Action WasSelected;

		public IdeTab(Control parent, IdePanel panel)
		{
			_tabButton = new RadioButton();
			_tabButton.Text = panel.Label;
			_tabButton.AutoSize = true;
			_tabButton.Appearance = Appearance.Button;
			_tabButton.FlatStyle = FlatStyle.Flat;
			_tabButton.FlatAppearance.BorderSize = 0;
			_tabButton.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
			_tabButton.ForeColor = parent.ForeColor;
			_tabButton.BackColor = parent.BackColor;
			_tabButton.Margin = Padding.Empty;
			_tabButton.Height = parent.Height;
			_tabButton.CheckedChanged += (s, a) => { if (Selected && WasSelected != null) WasSelected(); };
			_tabButton.MouseDown += (s, a) => { if (a.Button == MouseButtons.Left) Selected = true; };
			panel.LabelChanged += text => _tabButton.Text = text;

			Panel = panel;
		}

		public bool Selected
		{
			get { return _tabButton.Checked; }
			set { _tabButton.Checked = value; }
		}
	}
}
