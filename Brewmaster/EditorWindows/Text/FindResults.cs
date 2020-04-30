using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.Text
{
	public class FindResults : Form
	{
		public TreeView ResultsTree;

		public FindResults()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.ResultsTree = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// ResultsTree
			// 
			this.ResultsTree.BackColor = System.Drawing.SystemColors.Control;
			this.ResultsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ResultsTree.ForeColor = System.Drawing.SystemColors.ControlText;
			this.ResultsTree.Location = new System.Drawing.Point(0, 0);
			this.ResultsTree.Name = "ResultsTree";
			this.ResultsTree.Size = new System.Drawing.Size(405, 125);
			this.ResultsTree.TabIndex = 0;
			// 
			// FindResults
			// 
			this.ClientSize = new System.Drawing.Size(405, 125);
			this.Controls.Add(this.ResultsTree);
			this.MinimizeBox = false;
			this.Name = "FindResults";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Find Results";
			this.ResumeLayout(false);

		}
	}
}
