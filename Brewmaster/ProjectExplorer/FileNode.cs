using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectExplorer
{

	public class EditableNode : TreeNode
	{
		public bool Editable { get; set; }

		public EditableNode()
		{
			Editable = true;
		}

		public event Action<NodeLabelEditEventArgs> Edited;
		internal virtual void AfterEdit(NodeLabelEditEventArgs args)
		{
			if (Editable && Edited != null) Edited(args);
		}

	}

	public class PipelineNode : EditableNode
	{
		private AsmProjectFile _projectFile;

		public PipelineNode(AsmProjectFile fileInfo, string fileName)
		{
			_projectFile = fileInfo;
			Text = fileName;
			ForeColor = Color.DarkGray;
			ImageIndex = 9;
			Editable = false;
		}
	}

	public class DirectoryNode : EditableNode, IIdentifiableNode
	{
		public DirectoryInfo DirectoryInfo { get; set; }

		public DirectoryNode(DirectoryInfo directory)
		{
			DirectoryInfo = directory;
			Text = directory.Name;
			ImageIndex = 3;
			SelectedImageIndex = 3;
		}

		public void AddSubdirectory()
		{
			throw new NotImplementedException();
		}

		public object UniqueIdentifier { get { return DirectoryInfo.FullName.TrimEnd('\\', '/'); } }
	}

	public class NonIncludedFileNode: TreeNode, IIdentifiableNode
	{
		public NonIncludedFileNode(FileInfo fileInfo)
		{
			FileInfo = fileInfo;
			Text = fileInfo.Name;
			ImageIndex = 8;
			SelectedImageIndex = 8;
			ForeColor = SystemColors.GrayText;
		}

		public object UniqueIdentifier
		{
			get { return FileInfo.FullName; }
		}
		public FileInfo FileInfo { get; }
	}

	public class FileNode : EditableNode, IIdentifiableNode
	{
		public AsmProjectFile FileInfo { get; set; }

		public FileNode(AsmProjectFile fileInfo)
		{
			UpdateFromFile(fileInfo);
		}
		public object UniqueIdentifier { get { return FileInfo.File; } }

		public void UpdateFromFile(AsmProjectFile fileInfo)
		{
			FileInfo = fileInfo;
			Text = fileInfo.File.Name;

			switch (fileInfo.Type)
			{
				case FileType.Source:
				case FileType.Include:
					ImageIndex = 6;
					break;
				case FileType.Image:
					ImageIndex = 5;
					break;
				default:
					ImageIndex = 4;
					break;
			}

			if (fileInfo.Mode == CompileMode.LinkerConfig) ImageIndex = 7;
			if (fileInfo.Missing)
			{
				ImageIndex = 8;
				ForeColor = SystemColors.GrayText;
				Text += " (missing)";
			}
			SelectedImageIndex = ImageIndex;

			/*if (fileInfo.Pipeline != null) // TODO: Display pipeline output in explorer?
			{
				foreach (var outputFile in fileInfo.Pipeline.OutputFiles.Where(f => f != null))
				{
					var file = new FileInfo(fileInfo.Project.GetFullPath(outputFile));
					if (file.Exists) Nodes.Add(new PipelineNode(fileInfo, file.Name));
				}
			}*/
		}
	}
	public interface IIdentifiableNode
	{
		object UniqueIdentifier { get; }
		bool IsExpanded { get; }
		TreeNodeCollection Nodes { get; }
		void Expand();
	}
}
