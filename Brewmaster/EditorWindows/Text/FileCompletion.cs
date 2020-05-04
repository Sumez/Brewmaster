using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.Text
{
	public class FileCompletion : CompletionDataProvider
	{
		public override ImageList ImageList { get; set; }
		public override string PreSelection { get; set; }
		public override int DefaultIndex { get; set; }
		private DirectoryInfo[] _directories;
		private readonly Action _reOpen;

		public FileCompletion(DirectoryInfo[] directories, Action reOpen)
		{
			_directories = directories;
			_reOpen = reOpen;
			ImageList = new ImageList();
			ImageList.Images.Add(Resources.folder_dark);
			ImageList.Images.Add(Resources.file);
			DefaultIndex = -1;
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			var returnValues = new List<ICompletionData>();
			if (!PreSelection.StartsWith("\"")) return returnValues.ToArray();

			var baseDir = "";
			var preSelection = PreSelection.Substring(1);
			var lastSlash = preSelection.LastIndexOfAny(new[] { '/', '\\' });
			if (lastSlash >= 0)
			{
				baseDir = preSelection.Substring(0, lastSlash + 1);
				preSelection = lastSlash < preSelection.Length ? preSelection.Substring(lastSlash + 1) : "";
			}

			var searchedDirectories = new List<string>();
			foreach (var originalDirectory in _directories)
			{
				try
				{
					var directory = new DirectoryInfo(Path.Combine(originalDirectory.FullName, baseDir));

					if (!directory.Exists) continue;
					if (searchedDirectories.Contains(directory.FullName)) continue;
					searchedDirectories.Add(directory.FullName);
					foreach (var subDirectory in directory.GetDirectories().Where(dir => dir.Name.StartsWith(preSelection, StringComparison.InvariantCultureIgnoreCase)))
					{
						returnValues.Add(new DirectoryData(subDirectory.Name, _reOpen));
					}
					foreach (var file in directory.GetFiles().Where(file => file.Name.StartsWith(preSelection, StringComparison.InvariantCultureIgnoreCase)))
					{
						returnValues.Add(new FileData(file.Name));
					}
				}
				catch
				{
					// Directory is an invalid name, user doesn't have access, etc.
					continue;
				}

			}

			return returnValues.ToArray();
		}

		public override bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			var preOffset = PreSelection.LastIndexOfAny(new[] {'\\', '/', '"'});
			if (preOffset < 0) return false;
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			textArea.InsertString(PreSelection.Substring(0, preOffset + 1));
			return data.InsertAction(textArea, key);
		}

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			return CompletionDataProviderKeyResult.NormalKey;
		}
	}

	public class FileData : ICompletionData
	{
		public FileData(string name)
		{
			Text = name;
			//Description = opcode.ToString();
		}
		public virtual bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Text + "\"");
			return true;
		}

		public virtual int ImageIndex { get { return 1; } }
		public string Text { get; set; }
		public virtual string Description { get; }
		public virtual double Priority { get { return 10; } }
	}
	public class DirectoryData : FileData
	{
		private readonly Action _reOpen;
		public DirectoryData(string name, Action reOpen) : base(name)
		{
			_reOpen = reOpen;
		}
		public override bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Text + "/");
			if (_reOpen != null) _reOpen();
			return false;
		}

		public override int ImageIndex { get { return 0; } }
		public override double Priority { get { return 1; } }
	}
}
