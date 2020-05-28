using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Brewmaster.EditorWindows.Text;
using Brewmaster.ProjectExplorer;
using Brewmaster.Properties;
using Brewmaster.Settings;

namespace Brewmaster.EditorWindows
{
	public class EditorTabs : TabControl
    {
		public event Action<TabPageCollection> TabWindowsChanged;
		public event Action ActiveTabChanged;

		private Color forecolor = Color.White;
	    private Color textColorInactive = Color.Navy;

        public Color TextColor
        {
            get { return forecolor; }
            set { forecolor = value; Invalidate(); }
        }
		public Color TextColorInactive
		{
			get { return textColorInactive; }
			set { textColorInactive = value; Invalidate(); }
		}

		public EditorTabs()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
            Padding = new Point(30, 0);
			ContextMenuStrip = GetContextMenu();
	        //SizeMode = TabSizeMode.Fixed;
	        //DoubleBuffered = true;

	        SelectedIndexChanged += (sender, args) =>
	        {
		        if (SelectedTab is TextEditorWindow editorWindow) editorWindow.TextEditor.Focus();
	        };

	        TabWindowsChanged += (tabs) => CheckActiveTabChanged();
	        SelectedIndexChanged += (o, a) => CheckActiveTabChanged();
	        ControlAdded += (o, a) => CheckActiveTabChanged();
        }

	    private void CheckActiveTabChanged()
	    {
		    if (!(SelectedTab is EditorWindow) || _activeTab == SelectedTab) return;
			_activeTab = SelectedTab as EditorWindow;
			_activeTab.TabActivated();
			if (ActiveTabChanged != null) ActiveTabChanged();
	    }


	    protected override void OnPaintBackground(PaintEventArgs e)
	    {
		    base.OnPaintBackground(e);
		    var yIndex = ItemSize.Height + Padding.Y - 1;
			e.Graphics.DrawLine(Pens.Gray, ClientRectangle.Left, yIndex, ClientRectangle.Right, yIndex);
		    for (var i = 0; i < TabCount; i++)
		    {
			    OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, GetTabRect(i), i, SelectedIndex == i ? DrawItemState.Selected : DrawItemState.None));
		    }
		    var dragIndex = _dragTab != null ? TabPages.IndexOf(_dragTab) : -1;
		    if (_dragging && dragIndex != TabCount - 1)
			    OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, GetTabRect(dragIndex), dragIndex, DrawItemState.Selected));
		}

		private int _hoverButton = -1;
	    private int _hoverTab = -1;
		protected override void OnDrawItem(DrawItemEventArgs e)
        {
			base.OnDrawItem(e);

	        var dragIndex = _dragTab != null ? TabPages.IndexOf(_dragTab) : -1;
			var bounds = e.Bounds;
	        if (_dragging && e.Index == dragIndex)
	        {
		        var cursorPosition = PointToClient(Cursor.Position);
		        bounds.X = Math.Min(Math.Max(0, cursorPosition.X + _dragOffset), Width - bounds.Width);
	        }

			if (TabRenderer.IsSupported && Application.RenderWithVisualStyles)
			{
				TabRenderer.DrawTabItem(e.Graphics, bounds, e.State == DrawItemState.Selected ? TabItemState.Selected : TabItemState.Normal);
			}
			
			TabPages[e.Index].BorderStyle = BorderStyle.None;
			TabPages[e.Index].ForeColor = SystemColors.ControlText;

	        var textColor = SystemColors.ControlText;
	        if (_hoverTab == e.Index || e.State == DrawItemState.Selected)
	        {
		        //textColor = Color.White;
	        }
			//if (e.State == DrawItemState.Selected) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, 180, 180)), rc);
			//else if (_hoverTab == e.Index) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 200, 200)), rc);

			e.Graphics.DrawString(TabPages[e.Index].Text, this.Font, new SolidBrush(textColor), bounds.Left + 5, bounds.Top + 5);

			// Draw X:
			if (_hoverButton == e.Index) e.Graphics.FillRectangle(Brushes.LightGray, GetButtonBounds(bounds));
	        if (_hoverTab == e.Index || e.State == DrawItemState.Selected)
	        {
		        var xFont = new Font(FontFamily.GenericSansSerif, 7);
		        e.Graphics.DrawString("X", xFont, new SolidBrush(textColor), bounds.Right - 17, bounds.Top + 6);
	        }
		}


	    private Rectangle GetButtonBounds(Rectangle tabBounds)
		{
			return new Rectangle(tabBounds.Right - 20, tabBounds.Top+4, 16, 16);
		}
		private int GetTabButtonFromCoordinates(Point location)
		{
			var tab = GetTabFromCoordinates(location);
			if (tab == -1) return -1;

			var currentTabRectangle = GetTabRect(tab);
			var closeButton = GetButtonBounds(currentTabRectangle);
			if (closeButton.Contains(location)) return tab;
			return -1;
		}

		private int GetTabFromCoordinates(Point location)
		{
			// iterate through all the tab pages
			for (var i = 0; i < TabCount; i++)
			{
				// get their rectangle area and check if it contains the mouse cursor
				var currentTabRectangle = GetTabRect(i);
				if (currentTabRectangle.Contains(location)) return i;
			}
			return -1;
		}

	    private int GetTabFromXCoordinate(Point location)
	    {
		    // iterate through all the tab pages
		    for (var i = 0; i < TabCount; i++)
		    {
			    var currentTabRectangle = GetTabRect(i);
			    if (currentTabRectangle.Right >= location.X) return i;
		    }

		    return TabCount - 1;
	    }


		private const int DragTreshold = 20;
	    private bool _dragging = false;
	    private TabPage _dragTab = null;
	    private int _previousDragIndex;
	    private int _dragOffset;
	    private EditorWindow _activeTab;

	    protected override void OnMouseDown(MouseEventArgs e)
	    {
		    var hoverTab = GetTabFromCoordinates(e.Location);
		    if (hoverTab < 0) return;
			
		    _dragging = false;
		    _dragTab = TabPages[hoverTab];

		    var tabBounds = GetTabRect(hoverTab);
			_dragOffset = tabBounds.X - e.Location.X;
		    _previousDragIndex = hoverTab;


			base.OnMouseDown(e);
		}

	    protected override void OnMouseUp(MouseEventArgs e)
	    {
		    if (_dragTab != null)
		    {
			    _dragging = false;
			    _dragTab = null;
				Invalidate();
		    }

		    base.OnMouseUp(e);
	    }


		// TODO: OS specific?
	    protected void BeginTabUpdates()
	    {
		    var msgSuspendUpdate = Message.Create(Parent.Handle, 11, IntPtr.Zero, IntPtr.Zero);
			NativeWindow.FromHandle(Parent.Handle).DefWndProc(ref msgSuspendUpdate);
		    SuspendLayout();
		}

		protected void EndTabUpdates()
	    {
		    ResumeLayout();
		    IntPtr wparam = new IntPtr(1);
		    var msgResumeUpdate = Message.Create(Parent.Handle, 11, wparam, IntPtr.Zero);
			NativeWindow.FromHandle(Parent.Handle).DefWndProc(ref msgResumeUpdate);
		    Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_dragTab != null && !_dragging)
			{
				var tabBounds = GetTabRect(TabPages.IndexOf(_dragTab));
				var delta = tabBounds.X - e.Location.X - _dragOffset;

				if (Math.Abs(delta) > DragTreshold)
				{
					_dragging = true;
					_hoverTab = -1;
					_hoverButton = -1;
					Invalidate();
				}

			}
			else if (_dragging)
			{
				var hoverTab = GetTabFromXCoordinate(e.Location);
				if (hoverTab >= 0 && hoverTab != _previousDragIndex)
				{
					if (TabPages.IndexOf(_dragTab) != hoverTab) {
						BeginTabUpdates();

						// TODO: Just reorder tab drawing instead of the actual tab pages (until mouseup) for better performance
						TabPages.Remove(_dragTab);
						TabPages.Insert(hoverTab, _dragTab);
						SelectedIndex = hoverTab;

						EndTabUpdates();
					}
					_previousDragIndex = GetTabFromCoordinates(e.Location); // Prevents "flashing" due to uneven tab widths
				}
				Invalidate();
			}
			else {
				var hoverTab = GetTabFromCoordinates(e.Location);
				if (hoverTab != _hoverTab)
				{
					if (_hoverTab != -1 && _hoverTab < TabCount) Invalidate(GetTabRect(_hoverTab), false);
					_hoverTab = hoverTab;
					if (_hoverTab != -1) Invalidate(GetTabRect(_hoverTab), false);
				}

				var hover = GetTabButtonFromCoordinates(e.Location);
				if (hover != _hoverButton)
				{
					if (_hoverButton != -1 && _hoverButton < TabCount) Invalidate(GetTabRect(_hoverButton), false);
					_hoverButton = hover;
					if (_hoverButton != -1) Invalidate(GetTabRect(_hoverButton), false);
				}
			}
			base.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			if (_hoverButton != -1 && TabCount > _hoverButton) Invalidate(GetTabRect(_hoverButton), false);
			if (_hoverTab != -1 && TabCount > _hoverTab) Invalidate(GetTabRect(_hoverTab), false);
			_hoverButton = -1;
			_hoverTab = -1;
		}
		protected override void OnMouseClick(MouseEventArgs e)
		{
			if (_dragging) return; // Fortunately "click" fires before "mouseup"
			
			if (e.Button != MouseButtons.Left) return;
			var tab = GetTabButtonFromCoordinates(e.Location);
			if (tab >= 0) {
				CloseTab(TabPages[tab]);
				return;
			}
			base.OnMouseClick(e);
		}

		public bool CloseAll(TabPage exceptTab = null)
		{
			BeginTabUpdates();
			var result = CloseAllRaw(exceptTab);
			EndTabUpdates();

			if (result && TabWindowsChanged != null) TabWindowsChanged(TabPages);
			return result;
		}

	    private bool CloseAllRaw(TabPage exceptTab)
	    {
		    var pagesBuffer = new List<TabPage>();
		    foreach (TabPage tab in TabPages)
			    if (tab != exceptTab) pagesBuffer.Add(tab);

		    foreach (var tab in pagesBuffer)
			    if (!ConfirmCloseTab(tab)) return false;

		    TabPages.Clear();
		    if (exceptTab != null) TabPages.Add(exceptTab);
		    foreach (var tab in pagesBuffer) tab.Dispose();

		    return true;
	    }

		private bool ConfirmCloseTab(TabPage tab)
		{
			var saveTab = tab as ISaveable;
			if (saveTab == null || saveTab.Pristine) return true;

			SelectedTab = tab;
			var tabName = tab.Text.TrimEnd('*', ' ');
			var choice = MessageBox.Show(this.Parent, "Do you want to save changes to '" + tabName + "' before closing?", "File changed", MessageBoxButtons.YesNoCancel);
			if (choice != DialogResult.Yes && choice != DialogResult.No) return false;
			if (choice == DialogResult.Yes) saveTab.Save();
			return true;
		}

		public bool CloseTab(TabPage tab)
		{
			if (!ConfirmCloseTab(tab)) return false;

			TabPages.Remove(tab);
			tab.Dispose();
			if (TabWindowsChanged != null) TabWindowsChanged(TabPages);
			
			return true;
		}


		private ContextMenuStrip GetContextMenu()
		{
			var menu = new ContextMenuStrip();

			var saveMenuItem = new ToolStripMenuItem("Save", Resources.save1);

			var closeMenuItem = new ToolStripMenuItem("Close");
			var closeAllMenuItem = new ToolStripMenuItem("Close All");
			var closeAllOthersMenuItem = new ToolStripMenuItem("Close All But This");

			var copyPathMenuItem = new ToolStripMenuItem("Copy Full Path");
			var openDirectoryMenuItem = new ToolStripMenuItem("Open Containing Directory");

			menu.Items.AddRange(new ToolStripItem[]
			{
				saveMenuItem, new ToolStripSeparator(),
				closeMenuItem, closeAllMenuItem, closeAllOthersMenuItem, new ToolStripSeparator(),
				copyPathMenuItem, openDirectoryMenuItem //, new ToolStripSeparator()
			});

			TabPage menuTab = null;
			menu.Opening += (s, a) =>
			{
				var tabIndex = GetTabFromCoordinates(PointToClient(Cursor.Position));
				menuTab = tabIndex >= 0 ? TabPages[tabIndex] : SelectedTab; // SelectedTab;
				var editorWindow = menuTab as EditorWindow;
				if (editorWindow == null)
				{
					saveMenuItem.Text = "Save";
					copyPathMenuItem.Enabled = openDirectoryMenuItem.Enabled = saveMenuItem.Enabled = false;
					return;
				}
				var saveable = editorWindow as ISaveable;

				saveMenuItem.Text = string.Format("Save {0}", editorWindow.ProjectFile.GetRelativePath());
				saveMenuItem.Enabled = saveable != null && !saveable.Pristine;

				copyPathMenuItem.Enabled = openDirectoryMenuItem.Enabled = true;

			};
			menu.Closing += (s, a) => { menuTab = null; }; // Makes menu shortcuts work when menu is closed

			saveMenuItem.Click += (s, a) => {
				var saveable = (menuTab ?? SelectedTab) as ISaveable;
				if (saveable != null) saveable.Save();
			};
			closeMenuItem.Click += (s, a) => CloseTab(menuTab ?? SelectedTab);
			closeAllMenuItem.Click += (s, a) => CloseAll();
			closeAllOthersMenuItem.Click += (s, a) => CloseAll(menuTab ?? SelectedTab);

			copyPathMenuItem.Click += (s, a) =>
			{
				var editorWindow = (menuTab ?? SelectedTab) as EditorWindow;
				if (editorWindow != null) Clipboard.SetText(editorWindow.ProjectFile.File.FullName);
			};
			openDirectoryMenuItem.Click += (s, a) =>
			{
				var editorWindow = (menuTab ?? SelectedTab) as EditorWindow;
				if (openDirectoryMenuItem != null) OsFeatures.OpenFolder(editorWindow.ProjectFile.File.Directory.FullName);
			};

			Program.BindKey(Feature.CloseWindow, (keys) => closeMenuItem.ShortcutKeys = keys);

			return menu;
		}

	}
}
