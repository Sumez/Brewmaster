using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.ProjectExplorer
{
	class ProjectExplorerMenu : ContextMenuStrip
	{
		private readonly ToolStripMenuItem NewMenu;
		private readonly ToolStripMenuItem PipelineNewMenu;
		private readonly ToolStripMenuItem OpenOption;
		private readonly ToolStripMenuItem DeleteOption;
		private readonly ToolStripMenuItem RenameOption;
		private readonly ToolStripMenuItem OpenFolderOption;
		private readonly ToolStripItem AddExistingOption;
		private readonly ToolStripItem PipelineAddExistingOption;
		private readonly ToolStripItem AddToPipelineOption;
		private readonly ToolStripItem RemoveFromProjectOption;
		private readonly ToolStripSeparator MoveSeparator;
		private readonly ToolStripSeparator OpenFolderSeperator;
		private readonly ToolStripSeparator EditSeparator;
		private TreeNode CurrentNode;

		public bool AllowFileEditing { get; set; }
		public Action<string, FileTemplate, string> CreateNewFile;
		public Action<string, string, CompileMode> AddExistingFile;
		public Action<TreeNode> OpenNode;
		public Action<TreeNode> EditNode;
		public Action<DirectoryInfo> AddSubdirectory;
		public Action<AsmProjectFile> RemoveFromProject;
		public Action<AsmProjectFile> MoveToPipeline;
		public Action<AsmProjectFile> DeleteFile;
		public Action<DirectoryInfo> DeleteDirectory;

		public ProjectExplorerMenu()
		{
			NewMenu = new ToolStripMenuItem { Text = "Add" };
			NewMenu.DropDownItems.Add("New Source File...", null, (s, e) => { NewFile(".s", FileTemplate.AssemblyCode); });
			NewMenu.DropDownItems.Add("New Include File...", null, (s, e) => { NewFile(".inc", FileTemplate.AssemblyInclude); });
			NewMenu.DropDownItems.Add("New Directory", null, (s, e) => { NewDirectory(); });

			PipelineNewMenu = new ToolStripMenuItem { Text = "Add" };
			PipelineNewMenu.DropDownItems.Add("New Famitracker Project...", null, (s, e) => { NewFile(".ftm", FileTemplate.Famitracker); });
			PipelineNewMenu.DropDownItems.Add("New Level Data...", null, (s, e) => { NewFile(".level", FileTemplate.None); });

			AddExistingOption = new ToolStripMenuItem("Add existing File...", null, (s, e) => ExistingFile(CompileMode.IncludeInAssembly));
			PipelineAddExistingOption = new ToolStripMenuItem("Add existing File...", null, (s, e) => ExistingFile(CompileMode.ContentPipeline));
			OpenOption = new ToolStripMenuItem("Open", null, OpenFile_Click);
			DeleteOption = new ToolStripMenuItem("Delete", null, DeleteFile_Click);
			RenameOption = new ToolStripMenuItem("Rename", null, RenameFile_Click);
			OpenFolderOption = new ToolStripMenuItem("Open in File Explorer", null, OpenFolder_Click);

			Items.Add(NewMenu);
			Items.Add(AddExistingOption);
			Items.Add(PipelineNewMenu);
			Items.Add(PipelineAddExistingOption);
			Items.Add(OpenOption);
			Items.Add(EditSeparator = new ToolStripSeparator());
			Items.Add(DeleteOption);
			Items.Add(RenameOption);
			Items.Add(OpenFolderSeperator = new ToolStripSeparator());
			Items.Add(OpenFolderOption);

			Program.BindKey(Feature.ActivateItem, (keys) => OpenOption.ShortcutKeys = keys);
			Program.BindKey(Feature.RemoveFromList, (keys) => DeleteOption.ShortcutKeys = keys);
			Program.BindKey(Feature.Rename, (keys) => RenameOption.ShortcutKeys = keys);

			Items.Add(MoveSeparator = new ToolStripSeparator());
			RemoveFromProjectOption = Items.Add("Remove From Project", null, RemoveFromProject_Click);
			AddToPipelineOption = Items.Add("Move to Data Pipeline", null, MoveToPipeline_Click);

			Enabled = false;
		}

		private void OpenFolder_Click(object sender, EventArgs e)
		{
			var directoryNode = CurrentNode as DirectoryNode;
			if (directoryNode != null) OsFeatures.OpenFolder(directoryNode.DirectoryInfo.FullName);
		}

		private void RemoveFromProject_Click(object sender, EventArgs e)
		{
			var fileNode = CurrentNode as FileNode;
			if (fileNode != null && RemoveFromProject != null) RemoveFromProject(fileNode.FileInfo);
		}

		private void RenameFile_Click(object sender, EventArgs e)
		{
			if (EditNode != null) EditNode(CurrentNode);
		}

		private void MoveToPipeline_Click(object sender, EventArgs e)
		{
			var fileNode = CurrentNode as FileNode;
			if (fileNode != null && MoveToPipeline != null) MoveToPipeline(fileNode.FileInfo);
		}

		private void NewDirectory()
		{
			var directoryNode = CurrentNode as DirectoryNode;
			if (directoryNode == null) return;
			if (AddSubdirectory != null) AddSubdirectory(directoryNode.DirectoryInfo);
		}
		private void NewFile(string extension, FileTemplate template)
		{
			var directoryNode = CurrentNode as DirectoryNode;
			if (directoryNode == null) return;
			if (CreateNewFile != null) CreateNewFile(directoryNode.DirectoryInfo.FullName, template, extension);
		}

		private void ExistingFile(CompileMode mode)
		{
			var directoryNode = CurrentNode as DirectoryNode;
			if (directoryNode == null) return;
			if (AddExistingFile != null) AddExistingFile(null, directoryNode.DirectoryInfo.FullName, mode);
		}

		protected override void OnOpening(CancelEventArgs e)
		{
			if (!Enabled) e.Cancel = true;
			base.OnOpening(e);
		}

		private void DeleteFile_Click(object sender, EventArgs e)
		{
			var fileNode = CurrentNode as FileNode;
			var dirNode = CurrentNode as DirectoryNode;
			if (fileNode != null && DeleteFile != null) DeleteFile(fileNode.FileInfo);
			if (dirNode != null && DeleteDirectory != null) DeleteDirectory(dirNode.DirectoryInfo);
		}

		private void OpenFile_Click(object sender, EventArgs e)
		{
			if (CurrentNode != null && OpenNode != null) OpenNode(CurrentNode);
		}

		public void SetNode(TreeNode node)
		{
			CurrentNode = node;

			if (node == null)
			{
				Enabled = false;
				return;
			}

			var fileNode = node is FileNode;
			var dirNode = node is DirectoryNode;
			var editableNode = node as EditableNode;

			var root = ProjectExplorer.GetRoot(node);
			if (root.Name == "data")
			{
				// Data pipeline
				NewMenu.Visible = AddExistingOption.Visible = false;
				PipelineNewMenu.Visible = PipelineAddExistingOption.Visible = dirNode && AllowFileEditing;
			}
			else
			{
				NewMenu.Visible = AddExistingOption.Visible = dirNode && AllowFileEditing;
				PipelineNewMenu.Visible = PipelineAddExistingOption.Visible = false;
			}

			EditSeparator.Visible = AllowFileEditing;
			OpenOption.Visible = fileNode && AllowFileEditing;
			DeleteOption.Enabled = node != root;
			DeleteOption.Visible = (fileNode || dirNode) && AllowFileEditing;
			RenameOption.Enabled = editableNode != null && editableNode.Editable;
			RenameOption.Visible = editableNode != null && AllowFileEditing;

			OpenFolderSeperator.Visible = OpenFolderOption.Visible = dirNode;

			RemoveFromProjectOption.Visible = AddToPipelineOption.Visible = fileNode;
			MoveSeparator.Visible = fileNode && AllowFileEditing;
			AddToPipelineOption.Enabled = (root.Name != "data");
			Enabled = AllowFileEditing || fileNode;
		}
	}
}
