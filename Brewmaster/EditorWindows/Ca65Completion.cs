using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.Modules.Ca65Helper;
using Brewmaster.Modules.OpcodeHelper;
using Brewmaster.ProjectModel;
using Brewmaster.Properties;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Brewmaster.EditorWindows
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
		private readonly Events _events;
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
			var preSelection = PreSelection.ToLower();

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
				if (!globalSymbol.Value.Text.ToLower().Contains(preSelection)) continue;

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

			foreach (var opcode in OpcodeParser.GetOpcodes(_project.Type).Values)
			{
				if (!opcode.Command.StartsWith(preSelection, true, CultureInfo.InvariantCulture)) continue;
				returnValues.Add(new OpcodeData(opcode) { Focus = () => _events.HighlightOpcode(opcode) });
			}

			if (PreSelection.StartsWith("."))
			{
				var commands = Ca65Parser.GetCommands();
				foreach (var command in commands.Keys.OrderBy(k => k))
				{
					if (!command.StartsWith(preSelection, true, CultureInfo.InvariantCulture)) continue;
					returnValues.Add(new CommandData(commands[command], command.ToLower()) { Focus = () => _events.HighlightCommand(commands[command]) });
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

		public Ca65Completion(AsmProject project, Func<string, string> getSymbolDescription, Events events)
		{
			_project = project;
			_getSymbolDescription = getSymbolDescription;
			_events = events;
			ImageList = new ImageList();
			ImageList.Images.Add(Resources.label);
			ImageList.Images.Add(Resources.opcode);
			ImageList.Images.Add(Resources.ca65icon);
			ImageList.Images.Add(Resources.macro);
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
			ImageIndex = symbol is MacroSymbol ? 3 : 0;
		}

		public bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Text);
			return true;
		}

		public int ImageIndex { get; private set; }
		public string Text
		{
			get { return Symbol.Text; }
			set { }
		}

		public string Description { get { return GetDescription(Symbol.Text); }}
	}

	public interface IHasFocusAction
	{
		Action Focus { get; set; }
	}
	public class OpcodeData : ICompletionData, IHasFocusAction
	{
		public OpcodeData(Opcode opcode)
		{
			Opcode = opcode;
			Text = Opcode.Command.ToLower();
			Description = opcode.ToString();
		}
		public bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Opcode.Command.ToLower());
			return true;
		}

		public int ImageIndex { get { return 1; } }
		public string Text { get; set; }
		public string Description { get; }
		public double Priority { get { return 10; } }
		public Opcode Opcode { get; }
		public Action Focus { get; set; }
	}
	public class CommandData : ICompletionData, IHasFocusAction
	{
		public CommandData(Ca65Command command, string alias)
		{
			Command = command;
			Text = alias;
			Description = Command.ToString();
		}
		public bool InsertAction(TextArea textArea, char ch)
		{
			textArea.InsertString(Text);
			return true;
		}

		public int ImageIndex { get { return 2; } }
		public string Text { get; set; }
		public string Description { get; }
		public double Priority { get { return 0; } }
		public Ca65Command Command { get; }
		public Action Focus { get; set; }
	}
}