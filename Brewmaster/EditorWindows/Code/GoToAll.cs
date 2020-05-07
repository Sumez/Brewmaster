using System;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.EditorWindows.Text;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.Code
{
	public class GoToAll : Form
	{
		private TextBox _searchQuery;
		private CompletionWindow _completionWindow;
		private CompletionDataProvider _completionProvider;

		public GoToAll(AsmProject project, Events events)
		{
			_completionProvider = new AllCompletion(project, events);
			var dummyControl = new TextEditor(); // TODO: Create own, improved auto-completion window to remove reliance on texteditor
			InitializeComponent();

			_searchQuery.TextChanged += (s, a) =>
			{
				if (_searchQuery.Text.Length == 0)
				{
					if (_completionWindow != null) _completionWindow.Close();
					_completionWindow = null;
					return;
				}

				_completionProvider.PreSelection = _searchQuery.Text;
				if (_completionWindow != null && !_completionWindow.IsDisposed)
				{
					_completionWindow.RefreshCompletionData(null, ' ');
					return;
				}
				_completionWindow = CompletionWindow.ShowCompletionWindow(this, dummyControl, _completionProvider, PointToScreen(new Point(_searchQuery.Left, _searchQuery.Bounds.Bottom)));
				if (_completionWindow != null) _completionWindow.InsertCompleted += Close;
			};
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Close();
				return true;
			}

			if (_completionWindow != null && _completionWindow.ProcessKey(keyData)) return true;
			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_searchQuery.Focus();

			Deactivate += (s, a) =>
			{
				BeginInvoke(new Action(() =>
				{
					if (ContainsFocus || (_completionWindow != null && _completionWindow.ContainsFocus)) return;
					Close();
				}));
			};
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.Panel panel;
			System.Windows.Forms.Label label;
			this._searchQuery = new System.Windows.Forms.TextBox();
			panel = new System.Windows.Forms.Panel();
			label = new System.Windows.Forms.Label();
			panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			panel.Controls.Add(this._searchQuery);
			panel.Controls.Add(label);
			panel.Dock = System.Windows.Forms.DockStyle.Fill;
			panel.Location = new System.Drawing.Point(0, 0);
			panel.Name = "panel";
			panel.Size = new System.Drawing.Size(352, 42);
			panel.TabIndex = 2;
			// 
			// _searchQuery
			// 
			this._searchQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._searchQuery.Location = new System.Drawing.Point(2, 20);
			this._searchQuery.Name = "_searchQuery";
			this._searchQuery.Size = new System.Drawing.Size(348, 20);
			this._searchQuery.TabIndex = 0;
			// 
			// label
			// 
			label.AutoSize = true;
			label.Location = new System.Drawing.Point(0, 4);
			label.Name = "label";
			label.Size = new System.Drawing.Size(102, 13);
			label.TabIndex = 1;
			label.Text = "Search everywhere:";
			// 
			// GoToAll
			// 
			this.ClientSize = new System.Drawing.Size(352, 42);
			this.ControlBox = false;
			this.Controls.Add(panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "GoToAll";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			panel.ResumeLayout(false);
			panel.PerformLayout();
			this.ResumeLayout(false);

		}
	}
}
