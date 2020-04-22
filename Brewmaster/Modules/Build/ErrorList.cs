using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrewMaster.BuildProcess;
using BrewMaster.Ide;

namespace BrewMaster.Modules.Build
{
	public class ErrorList : ListView
	{
		// TODO: Tool tips?

		private ImageList errorIcons;
		private System.ComponentModel.IContainer components;

		public Action<string, int> GoTo { get; set; }

		public ErrorList()
		{
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
				new ColumnHeader {Text = "", Width = 25},
				new ColumnHeader {Text = "Details"},
				new ColumnHeader {Text = "File", Width = 40},
				new ColumnHeader {Text = "Line", Width = 25}
			});
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			Columns[0].Width = 25;
			Columns[2].Width = 60;
			Columns[3].Width = 40;
			Columns[1].Width = Width - 120;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (SelectedItems.Count > 0 && keyData == (Keys.Control | Keys.C))
			{
				Clipboard.SetText(SelectedItems[0].SubItems[1].Text);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
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

			var errorItem = e.Item as ErrorListItem;
			if (errorItem == null) return;
			e.Graphics.DrawImageUnscaled(errorIcons.Images[errorItem.Error.Type == BuildHandler.BuildError.BuildErrorType.Error ? 0 : 1], new Point(e.Bounds.X + 3, e.Bounds.Y));
		}

		public void RefreshList(List<BuildHandler.BuildError> list)
		{
			BeginUpdate();
			Items.Clear();
			foreach (var error in list.OrderBy(l => l.Type != BuildHandler.BuildError.BuildErrorType.Error))
			{
				Items.Add(new ErrorListItem(error));
			}
			this.UpdateLabel(list.Count == 0 ? "Build Errors" : string.Format("Build Errors ({0})", list.Count));
			EndUpdate();
		}

		protected override void OnItemActivate(EventArgs e)
		{
			base.OnItemActivate(e);
			var errorItem = SelectedItems[0] as ErrorListItem;
			if (GoTo == null || errorItem == null || errorItem.Error.File == null) return;

			GoTo(errorItem.Error.File, errorItem.Error.Line);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorList));
			this.errorIcons = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// errorIcons
			// 
			this.errorIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorIcons.ImageStream")));
			this.errorIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.errorIcons.Images.SetKeyName(0, "error.png");
			this.errorIcons.Images.SetKeyName(1, "warning.png");
			this.ResumeLayout(false);

		}
	}

	public class ErrorListItem : ListViewItem
	{
		public BuildHandler.BuildError Error { get; }

		public ErrorListItem(BuildHandler.BuildError error)
		:base(error.Type == BuildHandler.BuildError.BuildErrorType.Error ? "!" : "")
		{
			Error = error;
			SubItems.Add(error.Message);
			SubItems.Add(error.File);
			SubItems.Add(string.IsNullOrWhiteSpace(error.File) ? "" : error.Line.ToString());
		}
	}
}
