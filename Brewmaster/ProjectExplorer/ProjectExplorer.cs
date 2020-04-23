using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectExplorer
{
	public class ProjectExplorer : SortableTreeView
	{
		private ProjectExplorerMenu _menu;
		private AsmProject _project;
		private EditableNode _dataRootNode;
		private EditableNode _projectRootNode;

		public Action<AsmProjectFile> OpenFile;
		public Action<string, FileTemplate, string> CreateNewFile { set { _menu.CreateNewFile = value; } }
		public Action<string, string, CompileMode> AddExistingFile { set { _menu.AddExistingFile = value; } get { return _menu.AddExistingFile; } }


		private void InitializeComponent()
		{
			this._menu = new ProjectExplorerMenu();
			this.SuspendLayout();
			// 
			// Menu
			// 
			this._menu.Enabled = false;
			this._menu.Name = "Menu";
			// 
			// ProjectExplorer
			// 
			this.ContextMenuStrip = this._menu;
			this.LabelEdit = true;
			this.ShowLines = false;
			this.AllowDrop = true;
			this.ResumeLayout(false);

		}

		public ProjectExplorer()
		{
			InitializeComponent();

			AfterSelect += (sender, args) => _menu.SetNode(args.Node);
			NodeMouseDoubleClick += (sender, args) => OpenNode(args.Node);
			_menu.OpenNode = OpenNode;
			_menu.AddSubdirectory = AddSubdirectory;
			_menu.DeleteFile = DeleteFile;
			_menu.EditNode = EditNode;
			_menu.DeleteDirectory = DeleteDirectory;
			_menu.MoveToPipeline = MoveFileToPipeline;
			_menu.RemoveFromProject = (file) => _project.RemoveProjectFile(file);
		}

		protected override bool OnBeforeDrag(object draggedItem)
		{
			return draggedItem is FileNode;
		}

		protected override void OnAfterDrag(IDataObject data, TreeNode dragTarget)
		{
			var targetRoot = GetRoot(dragTarget);
			var targetDirectoryNode = dragTarget as DirectoryNode ?? dragTarget.Parent as DirectoryNode;
			if (targetDirectoryNode == null) return;

			if (data.GetDataPresent(typeof(FileNode)))
			{
				var fileNode = data.GetData(typeof(FileNode)) as FileNode;
				if (fileNode == null) return;
				var currentRoot = GetRoot(fileNode);
				if (currentRoot != targetRoot) fileNode.FileInfo.Mode = targetRoot == _dataRootNode ? CompileMode.ContentPipeline : CompileMode.IncludeInAssembly;
				if (targetDirectoryNode != fileNode.Parent)
				{
					var file = fileNode.FileInfo.File;
					file.MoveTo(Path.Combine(targetDirectoryNode.DirectoryInfo.FullName, file.Name));
					_project.Pristine = false;
					RefreshTree();
				}
				return;
			}
			if (data.GetDataPresent("FileDrop"))
			{
				if (AddExistingFile == null) return;
				foreach (var file in ((string[])data.GetData("FileDrop")).Select(f => new FileInfo(f)))
				{
					if (!file.Exists) continue;
					AddExistingFile(file.FullName, targetDirectoryNode.DirectoryInfo.FullName, targetRoot == _dataRootNode ? CompileMode.ContentPipeline : CompileMode.IncludeInAssembly);
				}
				return;
			}
		}

		private void MoveFileToPipeline(AsmProjectFile file)
		{
			file.Mode = CompileMode.ContentPipeline;
			_project.Pristine = false;
			RefreshTree();
		}

		private void DeleteDirectory(DirectoryInfo directory)
		{
			var choice = MessageBox.Show(this, string.Format("Do you want to delete '{0}' and all of its contents?\nThis action cannot be undone", directory.Name), "Delete directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
			if (choice != DialogResult.OK) return;

			try
			{
				directory.Delete(true);
			}
			catch (Exception ex)
			{
				Program.Error("Failed deleting directory:\n" + ex.Message, ex);
			}

			_project.Directories.RemoveAll(d => d.FullName == directory.FullName);
			_project.RemoveDeletedFiles();
			_project.Pristine = false;
			RefreshTree();
			// TODO: What if files are open in editor windows

		}

		private void DeleteFile(AsmProjectFile file)
		{
			var choice = MessageBox.Show(this, string.Format("Do you want to delete '{0}' permanently?\nThis action cannot be undone", file.File.Name), "Delete file", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
			if (choice != DialogResult.OK) return;

			try
			{
				file.File.Delete();
			}
			catch (Exception ex)
			{
				Program.Error("Failed deleting file:\n" + ex.Message, ex);
			}
			_project.RemoveProjectFile(file);
			// TODO: What if file is open in an editor window
		}

		private void AddSubdirectory(DirectoryInfo parentDirectory)
		{
			var i = 0;
			string suggestedPath;
			DirectoryInfo newDirectory;
			do
			{
				i++;
				suggestedPath = Path.Combine(parentDirectory.FullName, string.Format("New Folder {0}", i));
			} while (Directory.Exists(suggestedPath));

			try
			{
				newDirectory = Directory.CreateDirectory(suggestedPath);
			}
			catch (Exception ex)
			{
				Program.Error("Failed creating directory", ex);
				return;
			}
			_project.Directories.Add(newDirectory);
			_project.Pristine = false;
			RefreshTree();
			var newNode = GetNode<DirectoryNode>(Nodes, d => d.DirectoryInfo.FullName == newDirectory.FullName);
			var parent = newNode.Parent;
			while (parent != null)
			{
				parent.Expand();
				parent = parent.Parent;
			}
			EditNode(newNode);
		}
		public T GetNode<T>(TreeNodeCollection nodes, Func<T, bool> predicate) where T : TreeNode
		{
			foreach (TreeNode node in nodes)
			{
				var typeNode = node as T;
				if (typeNode != null && predicate(typeNode)) return typeNode;
				var childNode = GetNode<T>(node.Nodes, predicate);
				if (childNode != null) return childNode;
			}
			return null;
		}

		protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnBeforeLabelEdit(e);
			var editable = e.Node as EditableNode;
			if (editable == null || !editable.Editable) e.CancelEdit = true;
		}
		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.Label))
			{
				e.CancelEdit = true;
				return;
			}
			SelectedNode = e.Node;
			var editableNode = e.Node as EditableNode;
			if (editableNode != null) editableNode.AfterEdit(e);

			base.OnAfterLabelEdit(e);
		}

		private void OpenNode(TreeNode node)
		{
			var fileNode = node as FileNode;
			if (fileNode != null) OpenFile(fileNode.FileInfo);
		}

		public void SetProject(AsmProject project)
		{
			_project = project;
			_project.ContentsChanged += RefreshTree; // TODO: Retain open nodes
			RefreshTree();
		}
		private void RefreshTree()
		{
			var projectNode = CreateNodeFromDirectory(_project.Directory, false, _project.Directories);
			projectNode.Edited += (e) => _project.Name = e.Label; // TODO: Update window title
			projectNode.Text = _project.Name;
			projectNode.ImageIndex = 9;
			projectNode.SelectedImageIndex = 9;

			var dataNode = CreateNodeFromDirectory(_project.Directory, true, new List<DirectoryInfo>());
			dataNode.Editable = false;
			dataNode.Text = "Data pipeline";
			dataNode.ImageIndex = 19;
			dataNode.SelectedImageIndex = 19;

			if (Nodes.Count == 0)
			{
				projectNode.Expand();
				dataNode.Expand();
			}
			GetExpandedState();
			Nodes.Clear();
			Nodes.Add(projectNode);
			Nodes.Add(dataNode);
			_menu.SetNode(null);

			_projectRootNode = projectNode;
			_dataRootNode = dataNode;
			SetExpandedState();
		}
		private DirectoryNode CreateNodeFromDirectory(DirectoryInfo directory, bool pipeline, List<DirectoryInfo> extraDirectories)
		{
			var node = new DirectoryNode(directory);
			foreach (var subDirectory in directory.GetDirectories())
			{
				if (subDirectory.Name[0] == '.') continue;
				var subDirNode = CreateNodeFromDirectory(subDirectory, pipeline, extraDirectories);
				subDirNode.Edited += (e) => RenameDirectory(subDirectory, e);
				if (subDirNode.Nodes.Count > 0) {
					extraDirectories.RemoveAll(d => d.FullName == subDirectory.FullName);
				}
				else if (!extraDirectories.Any(d => d.FullName == subDirectory.FullName)) continue;
				node.Nodes.Add(subDirNode);
			}
			foreach (var projectFile in _project.Files.Where(f => f.File.DirectoryName == directory.FullName).OrderBy(f => f.File.Name))
			{
				if ((projectFile.Mode == CompileMode.ContentPipeline) != pipeline) continue;

				var fileNode = new FileNode(projectFile);
				fileNode.Edited += (e) => RenameFile(projectFile.File, e);
				node.Nodes.Add(fileNode);
			}
			return node;
		}
		private void RenameFile(FileInfo file, NodeLabelEditEventArgs e)
		{
			foreach (var character in Path.GetInvalidFileNameChars())
				if (e.Label.Contains(character))
				{
					e.CancelEdit = true;
					Program.Error(string.Format("Invalid character '{0}'", character));
					return;
				}
			file.MoveTo(Path.Combine(file.DirectoryName, e.Label));
			_project.Pristine = false;
			// TODO: Update open tabs, and breakpoint list
		}
		private void RenameDirectory(DirectoryInfo directory, NodeLabelEditEventArgs e)
		{
			if (directory.Parent == null)
			{
				e.CancelEdit = true;
				return;
			}
			foreach (var character in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
			if (e.Label.Contains(character))
			{
				e.CancelEdit = true;
				Program.Error(string.Format("Invalid character '{0}'", character));
				return;
			}

			var oldPath = directory.FullName.TrimEnd('\\', '/');
			var newPath = Path.Combine(directory.Parent.FullName, e.Label);

			if (Directory.Exists(newPath))
			{
				e.CancelEdit = true;
				Program.Error("Path already exists");
				return;
			}
			try
			{
				directory.MoveTo(newPath);
			}
			catch (Exception ex)
			{
				e.CancelEdit = true;
				Program.Error("Unable to rename directory:\n" + ex.Message);
				return;
			}
			_project.Pristine = false;
			for (var i = 0; i < _project.Directories.Count; i++)
			{
				var testDirectory = _project.Directories[i];
				if (testDirectory.FullName == oldPath)
				{
					_project.Directories[i] = new DirectoryInfo(newPath);
				}
				else if (OsFeatures.IsInDirectory(testDirectory, oldPath))
				{
					_project.Directories[i] = new DirectoryInfo(Path.Combine(newPath, _project.Directories[i].FullName.Substring(oldPath.Length + 1)));
					var explorerDir = GetNode<DirectoryNode>(Nodes, d => d.DirectoryInfo == testDirectory);
					if (explorerDir != null) explorerDir.DirectoryInfo = _project.Directories[i];
				}
			}
			foreach (var file in _project.Files.Where(f => _project.IsInDirectory(f, oldPath)))
			{
				file.File = new FileInfo(Path.Combine(newPath, file.File.FullName.Substring(oldPath.Length + 1)));
			}

			RefreshTree();
		}

		public void FocusNode(AsmProjectFile projectFile)
		{
			var node = GetNode<FileNode>(Nodes, n => n.FileInfo == projectFile);
			if (node == null) return;
			var parent = node.Parent;
			while (parent != null)
			{
				parent.Expand();
				parent = parent.Parent;
			}
			SelectedNode = node;
		}

		private List<object> _expandedStateSource;
		private List<object> _expandedStateData;
		private object _selectedState;

		private void GetExpandedState()
		{
			_expandedStateSource = new List<object>();
			_expandedStateData = new List<object>();
			var selectedNode = SelectedNode as IIdentifiableNode;
			_selectedState = selectedNode == null ? null : selectedNode.UniqueIdentifier;

			if (_projectRootNode != null)
			{
				if (_projectRootNode.IsExpanded) _expandedStateSource.Add("!root");
				GetExpandedState(_projectRootNode.Nodes, _expandedStateSource);
			}
			if (_dataRootNode != null)
			{
				if (_dataRootNode.IsExpanded) _expandedStateData.Add("!root");
				GetExpandedState(_dataRootNode.Nodes, _expandedStateData);
			}
		}
		private static void GetExpandedState(TreeNodeCollection nodes, List<object> states)
		{
			foreach (var node in nodes.OfType<IIdentifiableNode>())
			{
				if (node.IsExpanded) states.Add(node.UniqueIdentifier);
				GetExpandedState(node.Nodes, states);
			}
		}
		private void SetExpandedState()
		{
			if (_projectRootNode != null)
			{
				if (_expandedStateSource.Contains("!root")) _projectRootNode.Expand();
				SetExpandedState(_projectRootNode.Nodes, _expandedStateSource);
			}
			if (_dataRootNode != null)
			{
				if (_expandedStateData.Contains("!root")) _dataRootNode.Expand();
				SetExpandedState(_dataRootNode.Nodes, _expandedStateData);
			}
			if (_selectedState != null) SelectedNode = GetNode<TreeNode>(Nodes, n =>
			{
				var identifiable = n as IIdentifiableNode;
				return identifiable != null && identifiable.UniqueIdentifier.Equals(_selectedState);
			});
		}
		private static void SetExpandedState(TreeNodeCollection nodes, List<object> states)
		{
			foreach (var node in nodes.OfType<IIdentifiableNode>())
			{
				if (states.Contains(node.UniqueIdentifier)) node.Expand();
				SetExpandedState(node.Nodes, states);
			}
		}
		public static TreeNode GetRoot(TreeNode node)
		{
			if (node.Parent == null) return node;
			return GetRoot(node.Parent);
		}
	}
}
