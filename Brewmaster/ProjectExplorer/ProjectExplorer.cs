using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectExplorer
{
	public class ProjectExplorer : UserControl
	{
		public ProjectExplorer(Events events)
		{
			InitializeComponent();
			_multiSplitContainer.AddPanel(Tree = new ProjectExplorerTree(events));
			_multiSplitContainer.AddPanel(Settings = new ProjectFileSettings(events) { Dock = DockStyle.Fill }).StaticWidth = 150;

			Tree.AfterSelect += (s, a) =>
			{
				Settings.File = a.Node is FileNode fileNode ? fileNode.FileInfo : null;
			};
		}

		public void SetProject(AsmProject project)
		{
			Settings.Clear();

			if (project == null) { Tree.Nodes.Clear(); }
			else Tree.SetProject(project);
		}


		public ProjectExplorerTree Tree { get; private set; }
		public ProjectFileSettings Settings { get; private set; }

		private Layout.MultiSplitContainer _multiSplitContainer;
		
		private void InitializeComponent()
		{
			this._multiSplitContainer = new Brewmaster.Layout.MultiSplitContainer();
			this.SuspendLayout();
			// 
			// multiSplitContainer1
			// 
			this._multiSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._multiSplitContainer.Horizontal = false;
			this._multiSplitContainer.Location = new System.Drawing.Point(0, 0);
			this._multiSplitContainer.Name = "_multiSplitContainer";
			this._multiSplitContainer.Size = new System.Drawing.Size(150, 150);
			this._multiSplitContainer.TabIndex = 0;
			this._multiSplitContainer.BorderWidth = 0;
			this._multiSplitContainer.Text = "multiSplitContainer";
			// 
			// ProjectExplorer
			// 
			this.Controls.Add(this._multiSplitContainer);
			this.Name = "ProjectExplorer";
			this.ResumeLayout(false);

		}
	}
}
