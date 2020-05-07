using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;
using Brewmaster.Properties;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Brewmaster.EditorWindows.Code
{
	public class AllCompletion : CompletionDataProvider
	{
		public AsmProject Project { get; private set; }
		public Events Events { get; private set; }
		public override ImageList ImageList { get; set; }
		public override string PreSelection { get; set; }
		public override int DefaultIndex { get; set; }

		public AllCompletion(AsmProject project, Events events)
		{
			Project = project;
			Events = events;
			ImageList = new ImageList();
			ImageList.Images.Add(Resources.label);
			ImageList.Images.Add(Resources.file);
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			var preSelection = PreSelection.ToLower();
			var returnValues = new List<ICompletionData>();
			var sources = new List<Source>();
			foreach (var symbol in Project.Symbols)
			{
				if (symbol.Key.ToLower().Contains(preSelection))
				{
					var source = new Source { Text = symbol.Value.Text, File = symbol.Value.Source, Line = symbol.Value.Line, Character = symbol.Value.Character };
					if (sources.Contains(source)) continue;
					returnValues.Add(new NavigationData(symbol.Value, Project, Events));
					sources.Add(source);
				}
			}

			foreach (var file in Project.Files)
			{
				if (file.File.Name.ToLower().Contains(preSelection))
				{
					returnValues.Add(new NavigationData(file, Events));
				}
			}

			return returnValues.ToArray();
		}

		public override bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			return data.InsertAction(textArea, key);
		}

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			return CompletionDataProviderKeyResult.NormalKey;
		}

		public struct Source
		{
			public string Text;
			public string File;
			public int Line;
			public int Character;
		}
	}

	public class NavigationData : ICompletionData
	{
		private readonly Action _navigate;
		public NavigationData(Symbol symbol, AsmProject project, Events events)
		{
			Text = symbol.Text;
			Description = string.Format(@"{0}: {1},{2}", new FileInfo(symbol.Source).Name, symbol.Line + 1, symbol.Character);
			_navigate = () =>
			{
				var file = project.Files.FirstOrDefault(f => f.File.FullName == symbol.Source);
				if (file == null) return;
				events.OpenFile(file, symbol.Line, symbol.Character, symbol.Text.Length);
			};
			ImageIndex = 0;
		}

		public NavigationData(AsmProjectFile file, Events events)
		{
			Text = file.File.Name;
			Description = file.File.FullName;
			_navigate = () => events.OpenFile(file);
			ImageIndex = 1;
		}

		public bool InsertAction(TextArea textArea, char ch)
		{
			_navigate();
			return true;
		}

		public int ImageIndex { get; }
		public string Text { get; set; }
		public string Description { get; }
		public double Priority { get; }
	}
}
