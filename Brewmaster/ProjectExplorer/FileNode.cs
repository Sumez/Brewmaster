﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BrewMaster.ProjectModel;

namespace BrewMaster.ProjectExplorer
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
		}
	}

	public class DirectoryNode : EditableNode, IIdentifiableNode
	{
		public DirectoryInfo DirectoryInfo { get; set; }

		public DirectoryNode(DirectoryInfo directory)
		{
			DirectoryInfo = directory;
			Text = directory.Name;
			ImageIndex = 15;
			SelectedImageIndex = 15;
		}

		public void AddSubdirectory()
		{
			throw new NotImplementedException();
		}

		public object UniqueIdentifier { get { return DirectoryInfo.FullName.TrimEnd('\\', '/'); } }
	}

	public class FileNode : EditableNode, IIdentifiableNode
	{
		public AsmProjectFile FileInfo { get; set; }

		public FileNode(AsmProjectFile fileInfo)
		{
			FileInfo = fileInfo;
			Text = fileInfo.File.Name;

			switch (fileInfo.Type)
			{
				case FileType.Source:
				case FileType.Include:
					ImageIndex = 18;
					break;
				case FileType.Image:
					ImageIndex = 17;
					break;
				default:
					ImageIndex = 16;
					break;
			}
			if (fileInfo.Missing)
			{
				ImageIndex = 100;
				ForeColor = SystemColors.GrayText;
				Text += " (missing)";
			}
			SelectedImageIndex = ImageIndex;

			if (false && fileInfo.Pipeline != null) // TODO: Display pipeline output in explorer?
			{
				foreach (var outputFile in fileInfo.Pipeline.OutputFiles)
				{
					if (outputFile == null) continue;
					
					var file = new FileInfo(outputFile);
					Nodes.Add(new PipelineNode(fileInfo, file.Name));
				}
			}
		}
		public object UniqueIdentifier { get { return FileInfo.File; } }
	}
	public interface IIdentifiableNode
	{
		object UniqueIdentifier { get; }
		bool IsExpanded { get; }
		TreeNodeCollection Nodes { get; }
		void Expand();
	}
}
