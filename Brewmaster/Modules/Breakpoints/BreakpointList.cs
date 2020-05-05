using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.Modules.Breakpoints
{
	public class BreakpointList : ListView
	{
		private ToolStripMenuItem clearAllMenuItem;
		private ToolStripMenuItem goToMenuItem;
		private ToolStripMenuItem addMenuItem;
		private readonly Events _events;

		private List<BreakpointItem> SelectedBreakpoints
		{
			get { return SelectedItems.OfType<BreakpointItem>().ToList(); }
		}

		public BreakpointList(Events events)
		{
			_events = events;
			InitializeComponent();
			OwnerDraw = true;
			ShowGroups = false;
			Activation = ItemActivation.Standard;
			FullRowSelect = true;
			GridLines = true;
			View = View.Details;
			BorderStyle = BorderStyle.None;
			Columns.AddRange(new[]
						{
				new ColumnHeader { Text = "", Width = 25 },
				new ColumnHeader { Text = "Address" },
				new ColumnHeader { Text = "Type", Width = 40 }
			});



			contextMenu.Opening += (s, a) =>
			{
				var project = _events.GetCurrentProject();

				addMenuItem.Enabled = project != null;
				clearAllMenuItem.Enabled = Items.Count > 0;
				goToMenuItem.Enabled = editMenuItem.Enabled = 
					deleteMenuItem.Enabled = enableMenuItem.Enabled = SelectedBreakpoints.Any();

				if (SelectedBreakpoints.Any(i => i.Breakpoint.File != null)) editMenuItem.Enabled = false;
				if (SelectedBreakpoints.Any(i => i.Breakpoint.File == null)) goToMenuItem.Enabled = false;
				enableMenuItem.CheckState = SelectedBreakpoints.Any() && SelectedBreakpoints.All(i => !i.Breakpoint.Disabled)
					? CheckState.Checked
					: SelectedBreakpoints.All(i => i.Breakpoint.Disabled)
						? CheckState.Unchecked
						: CheckState.Indeterminate;
			};
			contextMenu.Closing += (s, a) =>
				{
					goToMenuItem.Enabled = editMenuItem.Enabled = deleteMenuItem.Enabled =
						enableMenuItem.Enabled = clearAllMenuItem.Enabled = true;
				};

			clearAllMenuItem.Click += (s, a) =>
			{
				_events.RemoveBreakpoints(Items.OfType<BreakpointItem>().Select(b => b.Breakpoint));
			};
			deleteMenuItem.Click += (s, a) =>
			{
				_events.RemoveBreakpoints(SelectedBreakpoints.Select(b => b.Breakpoint));
			};
			enableMenuItem.Click += (s, a) =>
			{
				var targetStatus = SelectedBreakpoints.All(i => !i.Breakpoint.Disabled);
				foreach (var item in SelectedBreakpoints) item.Breakpoint.Disabled = targetStatus;
				_events.RemoveBreakpoints(new Breakpoint[] {});
				Invalidate();
				_events.UpdatedBreakpoints();
			};
			goToMenuItem.Click += (s, a) => JumpToBreakpoint(SelectedBreakpoints.FirstOrDefault());
			editMenuItem.Click += (s, a) => ActivateBreakpoint(SelectedBreakpoints.FirstOrDefault());
			addMenuItem.Click += (s, a) => AddNewBreakpoint();

			Program.BindKey(Feature.RemoveFromList, (keys) => deleteMenuItem.ShortcutKeys = keys);
			Program.BindKey(Feature.ActivateItem, (keys) => editMenuItem.ShortcutKeys = keys);
			ContextMenuStrip = contextMenu;
		}

		public Action<AsmProjectFile, int> GoTo { get; set; }

		private void ActivateBreakpoint(BreakpointItem item)
		{
			if (item == null) return;
			if (item.Breakpoint.File != null) JumpToBreakpoint(item);
			else EditBreakpoint(item);
		}

		private void JumpToBreakpoint(BreakpointItem item)
		{
			if (item == null) return;
			if (item.Breakpoint.File != null && GoTo != null) GoTo(item.Breakpoint.File, item.Breakpoint.CurrentLine);
		}
		private void EditBreakpoint(BreakpointItem item)
		{
			if (item == null) return;
			using (var dialog = new BreakpointEditor(item.Breakpoint, _events.GetCurrentProject()))
			{
				dialog.StartPosition = FormStartPosition.CenterParent;
				if (dialog.ShowDialog(this) == DialogResult.OK) _events.UpdatedBreakpoints();
			}
		}

		private void AddNewBreakpoint()
		{
			var project = _events.GetCurrentProject();
			if (project == null) return;
			using (var dialog = new BreakpointEditor(project))
			{
				dialog.StartPosition = FormStartPosition.CenterParent;
				if (dialog.ShowDialog(this) == DialogResult.OK) _events.AddBreakpoint(dialog.Breakpoint);
			}
		}

		protected override void OnItemActivate(EventArgs e)
		{
			base.OnItemActivate(e);
			ActivateBreakpoint(SelectedItems[0] as BreakpointItem);
		}

		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			base.OnDrawColumnHeader(e);
			e.DrawDefault = true;
		}
		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			base.OnDrawSubItem(e);
			if (e.ColumnIndex > 0)
			{
				e.DrawDefault = true;
				return;
			}

			var breakpointItem = e.Item as BreakpointItem;
			if (breakpointItem == null) return;
			CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.X + 3, e.Bounds.Y), breakpointItem.Breakpoint.Disabled ? CheckBoxState.UncheckedNormal : CheckBoxState.CheckedNormal);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			Columns[0].Width = 25;
			Columns[2].Width = 40;
			Columns[1].Width = Width - 70;
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			var location = HitTest(e.Location);
			var breakpointItem = location.Item as BreakpointItem;
			if (breakpointItem == null) return;
			if (e.Button != MouseButtons.Left) return;

			if (location.SubItem == location.Item.SubItems[0])
			{
				breakpointItem.Breakpoint.Disabled = !breakpointItem.Breakpoint.Disabled;
				_events.UpdatedBreakpoints();
			}
			Invalidate();
		}


		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem enableMenuItem;
		private ToolStripMenuItem editMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem deleteMenuItem;
		private System.ComponentModel.IContainer components;
		
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.enableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.goToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMenuItem,
            toolStripSeparator2,
            this.enableMenuItem,
            this.editMenuItem,
            this.goToMenuItem,
            this.toolStripSeparator1,
            this.deleteMenuItem,
            this.clearAllMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(170, 126);
			// 
			// enableMenuItem
			// 
			this.enableMenuItem.Name = "enableMenuItem";
			this.enableMenuItem.Size = new System.Drawing.Size(169, 22);
			this.enableMenuItem.Text = "Enable Breakpoint";
			// 
			// editMenuItem
			// 
			this.editMenuItem.Name = "editMenuItem";
			this.editMenuItem.Size = new System.Drawing.Size(169, 22);
			this.editMenuItem.Text = "Edit Breakpoint...";
			// 
			// goToMenuItem
			// 
			this.goToMenuItem.Name = "goToMenuItem";
			this.goToMenuItem.Size = new System.Drawing.Size(169, 22);
			this.goToMenuItem.Text = "Go To Breakpoint";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
			// 
			// deleteMenuItem
			// 
			this.deleteMenuItem.Name = "deleteMenuItem";
			this.deleteMenuItem.Size = new System.Drawing.Size(169, 22);
			this.deleteMenuItem.Text = "Delete Breakpoint";
			// 
			// clearAllMenuItem
			// 
			this.clearAllMenuItem.Name = "clearAllMenuItem";
			this.clearAllMenuItem.Size = new System.Drawing.Size(169, 22);
			this.clearAllMenuItem.Text = "Clear All";
			// 
			// addMenuItem
			// 
			this.addMenuItem.Name = "addMenuItem";
			this.addMenuItem.Size = new System.Drawing.Size(169, 22);
			this.addMenuItem.Text = "Add New...";
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			BeginUpdate();
			var breakpointItems = Items.OfType<BreakpointItem>();
			foreach (var breakpointItem in breakpointItems.Where(i => !breakpoints.Contains(i.Breakpoint)))
				Items.Remove(breakpointItem);

			foreach (var breakpoint in breakpoints.Where(b => !breakpointItems.Any(i => i.Breakpoint == b)))
			{
				Items.Add(new BreakpointItem(breakpoint));
			}
			foreach (var breakpointItem in breakpointItems) breakpointItem.Update();
			EndUpdate();
			Invalidate();
		}
	}

	public class BreakpointItem : ListViewItem
	{
		public Breakpoint Breakpoint { get; }

		public BreakpointItem(Breakpoint breakpoint)
		{
			Breakpoint = breakpoint;
			SubItems.Add("");
			SubItems.Add("");
			SubItems.Add("");

			Update();
		}
		public void Update()
		{
			SubItems[1].Text = Breakpoint.ToString();
			SubItems[2].Text = string.Format(@"{0}{1}{2}",
				Breakpoint.Type.HasFlag(Breakpoint.Types.Read) ? "R" : "-",
				Breakpoint.Type.HasFlag(Breakpoint.Types.Write) ? "W" : "-",
				Breakpoint.Type.HasFlag(Breakpoint.Types.Execute) ? "X" : "-");

			ForeColor = Color.Brown;
		}
	}
}
