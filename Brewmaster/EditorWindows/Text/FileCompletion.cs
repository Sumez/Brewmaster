using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.Text
{
	public class FileCompletion : CompletionDataProvider
	{
		public override ImageList ImageList { get; set; }
		public override string PreSelection { get; set; }
		public override int DefaultIndex { get; set; }
		private DirectoryInfo[] _directories;

		public FileCompletion(DirectoryInfo[] directories)
		{
			_directories = directories;
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			throw new NotImplementedException();
		}

		public override bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			throw new NotImplementedException();
		}

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			throw new NotImplementedException();
		}
	}
}
