using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.Text
{
	public class FindResults : Form
	{
		private StatusStrip statusStrip1;
		private ToolStripProgressBar _statusBar;
		private ToolStripStatusLabel _statusLabel;
		private Action<int, string> _setStatus;
		private Action<AsmProjectFile, List<SearchResult>, string> _addResult;
		public TreeView ResultsTree;

		public FindResults()
		{
			InitializeComponent();
			_setStatus = new Action<int, string>((progress, message) =>
			{
				_statusLabel.Text = message;
				_statusBar.Value = progress;
				if (progress == 100)
				{
					if (ResultsTree.Nodes.Count == 1) ResultsTree.Nodes[0].Expand();
					if (ResultsTree.Nodes.Count == 0) ResultsTree.Nodes.Add(new TreeNode("No results found"));
				}
			});
			_addResult = new Action<AsmProjectFile, List<SearchResult>, string>((file, results, query) =>
			{
				ResultsTree.Nodes.Add(new SearchResultNode(file, results, query));
			});
			ResultsTree.DrawMode = TreeViewDrawMode.OwnerDrawText;
			ResultsTree.DrawNode += (s, a) =>
			{
				if (!a.Node.IsVisible) return;

				var flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
				var color = a.State.HasFlag(TreeNodeStates.Focused) ? SystemColors.HighlightText : ForeColor;


				var currentX = a.Bounds.Left;
				var currentY = a.Bounds.Top + 1;
				TextRenderer.DrawText(a.Graphics, a.Node.Text, Font, new Point(currentX, currentY), color, flags);
				currentX += TextRenderer.MeasureText(a.Graphics, a.Node.Text, Font, a.Bounds.Size, flags).Width;

				if (a.Node is SearchResultLineNode lineNode)
				{
					var BoldFont = new Font(Font, FontStyle.Bold);
					var text = lineNode.Result.FullLine.Substring(0, lineNode.Result.Column);

					TextRenderer.DrawText(a.Graphics, text, Font, new Point(currentX, currentY), color, flags);
					currentX += TextRenderer.MeasureText(a.Graphics, text, Font, a.Bounds.Size, flags).Width;

					text = lineNode.Result.FullLine.Substring(lineNode.Result.Column, lineNode.Query.Length);
					TextRenderer.DrawText(a.Graphics, text, BoldFont, new Point(currentX, currentY), color, flags);
					currentX += TextRenderer.MeasureText(a.Graphics, text, BoldFont, a.Bounds.Size, flags).Width;

					text = lineNode.Result.FullLine.Substring(lineNode.Result.Column + lineNode.Query.Length);
					TextRenderer.DrawText(a.Graphics, text, Font, new Point(currentX, currentY), color, flags);
				}
			};

			ResultsTree.DoubleClick += (s, a) =>
			{
				if (ResultsTree.SelectedNode is SearchResultLineNode lineNode) lineNode.Result.ExecuteAction();
			};
		}

		private void InitializeComponent()
		{
			this.ResultsTree = new System.Windows.Forms.TreeView();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this._statusBar = new System.Windows.Forms.ToolStripProgressBar();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ResultsTree
			// 
			this.ResultsTree.BackColor = System.Drawing.SystemColors.Control;
			this.ResultsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ResultsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ResultsTree.ForeColor = System.Drawing.SystemColors.ControlText;
			this.ResultsTree.FullRowSelect = true;
			this.ResultsTree.Location = new System.Drawing.Point(0, 0);
			this.ResultsTree.Name = "ResultsTree";
			this.ResultsTree.ShowLines = false;
			this.ResultsTree.Size = new System.Drawing.Size(546, 164);
			this.ResultsTree.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusBar,
            this._statusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 164);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(546, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// _statusBar
			// 
			this._statusBar.Name = "_statusBar";
			this._statusBar.Size = new System.Drawing.Size(100, 16);
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(429, 17);
			this._statusLabel.Spring = true;
			this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FindResults
			// 
			this.ClientSize = new System.Drawing.Size(546, 186);
			this.Controls.Add(this.ResultsTree);
			this.Controls.Add(this.statusStrip1);
			this.MinimizeBox = false;
			this.Name = "FindResults";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Find Results";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public void Clear()
		{
			ResultsTree.Nodes.Clear();
		}

		public void SetStatus(int progress, string message)
		{
			if (IsDisposed) return;
			BeginInvoke(_setStatus, progress, message);
		}

		public void AddResult(AsmProjectFile file, List<SearchResult> results, string query)
		{
			if (IsDisposed) return;
			BeginInvoke(_addResult, file, results, query);
		}

	}

	public class SearchResult
	{
		public int Line;
		public int Column;
		public string FullLine;
		public Action<int, int> Action;

		public SearchResult(int column, int line, string fullLine, Action<int, int> action)
		{
			Column = column;
			Line = line;
			FullLine = fullLine;
			Action = action;
		}
		public void ExecuteAction()
		{
			if (Action != null) Action(Line, Column);
		}
	}

	public class SearchResultNode : TreeNode
	{
		public SearchResultNode(AsmProjectFile file, List<SearchResult> results, string query)
		{
			Text = string.Format("{0} ({1})", file.File.FullName, results.Count);
			Nodes.AddRange(results.Select(r => new SearchResultLineNode(file, r, query)).ToArray());
		}
	}
	public class SearchResultLineNode : TreeNode
	{
		public string Query { get; set; }
		public SearchResult Result { get; set; }
		public SearchResultLineNode(AsmProjectFile file, SearchResult result, string query)
		{
			Query = query;
			Result = result;
			Text = string.Format("Line {0}, char {1}: ", result.Line, result.Column + 1); // Columns are 1-indexed on the user end
		}
	}
}
