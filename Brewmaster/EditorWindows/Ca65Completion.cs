using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrewMaster.ProjectModel;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace BrewMaster.EditorWindows
{
	public abstract class CompletionDataProvider : ICompletionDataProvider
	{
		public abstract CompletionDataProviderKeyResult ProcessKey(char key);
		public abstract bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key);
		public abstract ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
		public abstract ImageList ImageList { get; set;  }
		public abstract string PreSelection { get; set; }
		public abstract int DefaultIndex { get; set; }
	}
	public class Ca65Completion : CompletionDataProvider
	{
		private AsmProject _project;
		private readonly Func<string, string> _getSymbolDescription;
		public List<KeyValuePair<string, Symbol>> GlobalSymbols { get { return _project.Symbols; } }

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			// Never used!?
			return CompletionDataProviderKeyResult.NormalKey;
		}

		public override bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			return data.InsertAction(textArea, key);
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			// TODO: Don't parse file in this method, do it async while editing file!!
			var symbols = new List<string>();
			var importedSymbols = new List<string>();
			var returnValues = new List<ICompletionData>();
			using (var reader = new StringReader(textArea.Document.TextContent))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var importMatch = AsmProjectFile.ImportSymbolRegex.Match(line);
					if (importMatch.Success)
					{
						importedSymbols.AddRange(importMatch.Groups[1].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(symbol => symbol.Trim()));
						continue;
					}

					var defineMatch = AsmProjectFile.DefineSymbolRegex.Match(line);
					if (!defineMatch.Success) defineMatch = AsmProjectFile.DefineProcSymbolRegex.Match(line);
					if (defineMatch.Success)
					{
						symbols.Add(defineMatch.Groups[1].Value);
					}
				}
			}
			foreach (var globalSymbol in GlobalSymbols)
			{
				if (!globalSymbol.Value.Text.ToLower().Contains(PreSelection.ToLower())) continue;

				if (symbols.Contains(globalSymbol.Key) && globalSymbol.Value.Source == fileName)
				{
					returnValues.Add(new Ca65SymbolData(globalSymbol.Value, 1, _getSymbolDescription));
				}
				else if (globalSymbol.Value.Public || (importedSymbols.Contains(globalSymbol.Key) && globalSymbol.Value.Source != fileName))
				{
					returnValues.Add(new Ca65SymbolData(globalSymbol.Value, 2, _getSymbolDescription));
				}
				else if (!globalSymbol.Value.Public && globalSymbol.Value.LocalToFile == fileName)
				{
					returnValues.Add(new Ca65SymbolData(globalSymbol.Value, 2, _getSymbolDescription));
				}

			}
			//CompletionData = returnValues.OrderBy(d => !d.Text.StartsWith(PreSelection)).ToArray();
			CompletionData = returnValues.ToArray();
			return CompletionData;
		}

		public ICompletionData[] CompletionData { get; set; }

		public override ImageList ImageList { get; set; }
		public override string PreSelection { get; set; }
		public override int DefaultIndex { get; set; }

		public Ca65Completion(AsmProject project, Func<string, string> getSymbolDescription)
		{
			_project = project;
			_getSymbolDescription = getSymbolDescription;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			ImageList = new ImageList
						{
							ImageStream = (ImageListStreamer)resources.GetObject("CodeItemImages.ImageStream")
						};
			DefaultIndex = -1;
		}
	}

	public class Ca65SymbolData : ICompletionData
	{
		public Symbol Symbol { get; private set; }
		public double Priority { get; private set; }
		public Func<string, string> GetDescription { get; private set; }

		public Ca65SymbolData(Symbol symbol, double priority, Func<string, string> getDescription)
		{
			Symbol = symbol;
			Priority = priority;
			GetDescription = getDescription;
		}

		public bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Text);
			return true;
		}

		public int ImageIndex { get { return 1; } }
		public string Text
		{
			get { return Symbol.Text; }
			set { }
		}

		public string Description { get { return GetDescription(Symbol.Text); }}
	}
}