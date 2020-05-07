using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.BuildProcess;
using Brewmaster.EditorWindows.Text;
using Brewmaster.Emulation;
using Brewmaster.Modules;
using Brewmaster.Modules.Ca65Helper;
using Brewmaster.Modules.OpcodeHelper;
using Brewmaster.Modules.Watch;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.EditorWindows.Code
{

	public class Ca65Formatting : DefaultFormattingStrategy, IFormattingStrategy
	{
		public Ca65Formatting Formatting { get; set; }

		public Ca65Formatting()
		{
		}
	}


	public class CodeEditor : TextEditor
	{
		public AsmProjectFile File { get; private set; }
		protected Events ModuleEvents { get; private set; }
		protected CompletionDataProvider _completionDataProvider;
		protected CompletionDataProvider _fileCompletionDataProvider;

		private CompletionWindow _codeCompletionWindow;
		private int _caretLine = 0;
		private static readonly Keys[] NavigationKeys = { Keys.ShiftKey, Keys.ControlKey, Keys.Alt, Keys.Down, Keys.Up, Keys.Left, Keys.Right, Keys.Home, Keys.End, Keys.Enter, Keys.Escape, Keys.PageDown, Keys.PageUp, Keys.Tab, Keys.Return };

		private CodeMenu Menu { get; set; }
		public Func<string, Task<List<BuildHandler.BuildError>>> ParseErrors { get; set; }
		public Action<string, bool> AddToWatch { get; set; }
		public Action<int, Breakpoint.Types> AddAddressBreakpoint { get; set; }
		public Action<string, Breakpoint.Types> AddSymbolBreakpoint { get; set; }
		public Func<MemoryState> GetCpuMemory { get; set; }

		public CodeEditor(AsmProjectFile file, Events events)
		{
			File = file;
			ModuleEvents = events;
			Document.FormattingStrategy = new Ca65Formatting();
			_fileCompletionDataProvider = new FileCompletion(new[] { file.Project.Directory, file.File.Directory },
				() => {
					_forcedAutoCompleteWindow = true;
					ShowIntellisense('/', 0);
				});

			ActiveTextAreaControl.TextArea.InsertLeftMargin(1, 
				new CpuAddressMargin(ActiveTextAreaControl.TextArea,
					GetDebugLine,
					file.Project.Type == ProjectType.Snes ? 6 : 4));

			Menu = new CodeMenu(events);
			Menu.Enabled = true;
			Menu.Name = "Menu";
			Menu.Size = new Size(108, 70);
			ActiveTextAreaControl.TextArea.ContextMenuStrip = Menu;
			Menu.Opening += (sender, e) =>
			{
				var word = GetAsmWord(ActiveTextAreaControl.Caret.Position);
				Menu.CurrentWord = word;
			};
			Menu.GoToDefinition = GoToSymbol;
			Menu.AddToWatch = () =>
			{
				var word = GetAsmWord(ActiveTextAreaControl.Caret.Position);
				if (word == null) return;
				switch (word.WordType)
				{
					case AsmWord.AsmWordType.NumberWord:
					case AsmWord.AsmWordType.NumberByte:
						AddToWatch(word.Word, word.WordType == AsmWord.AsmWordType.NumberWord);
						break;
					case AsmWord.AsmWordType.LabelReference:
					case AsmWord.AsmWordType.LabelAbsolute:
					case AsmWord.AsmWordType.LabelDefinition:
						AddToWatch(word.Word, false);
						break;
				}
			};
			Menu.BreakOnAccess = (type) =>
			{
				var word = GetAsmWord(ActiveTextAreaControl.Caret.Position);
				if (word == null) return;
				switch (word.WordType)
				{
					case AsmWord.AsmWordType.NumberWord:
					case AsmWord.AsmWordType.NumberByte:
						AddAddressBreakpoint(WatchValue.ParseNumber(word.Word), type);
						break;
					case AsmWord.AsmWordType.LabelReference:
					case AsmWord.AsmWordType.LabelAbsolute:
					case AsmWord.AsmWordType.LabelDefinition:
						AddSymbolBreakpoint(word.Word, type);
						break;
				}
			};
			Menu.ToggleBreakpoint = ToggleBreakpointAtCaret;


			Document.HighlightingStrategy = new Ca65Highlighting(File.Project.Type);
			var testMarker = new TextMarker(0, 0, TextMarkerType.SolidBlock, Document.HighlightingStrategy.GetColorFor("Highlighted word").BackgroundColor);
			Document.MarkerStrategy.AddMarker(testMarker);
			/*ActiveTextAreaControl.TextArea.MouseMove += (sender, e) =>
				{
					//Document.MarkerStrategy.RemoveMarker(testMarker);
					//ActiveTextAreaControl.TextArea.Invalidate();

					var textPosition = new Point(e.Location.X - ActiveTextAreaControl.TextArea.LeftMargins.Where(m => m.IsVisible).Sum(m => m.Size.Width), e.Location.Y);
					if (textPosition.X < 0 || textPosition.Y < 0) return;

					var position = ActiveTextAreaControl.TextArea.TextView.GetLogicalPosition(textPosition);
					if (position.Line >= Document.TotalNumberOfLines) return;

					var line = Document.GetLineSegment(position.Line);
					if (line == null) return;
					var word = line.GetWord(position.Column);
					if (word == null || word.IsWhiteSpace) return;

					return;
					//word.SyntaxColor = new HighlightColor(word.Color, Color.DarkGray, true, false);
					Document.MarkerStrategy.AddMarker(testMarker);
					testMarker.Offset = word.Offset + line.Offset;
					testMarker.Length = word.Length;
					ActiveTextAreaControl.TextArea.Invalidate();
				};*/

			var lineAddressToolTip = new ToolTip();
			ActiveTextAreaControl.TextArea.ToolTipRequest += (s, e) =>
			{
				Document.MarkerStrategy.RemoveMarker(testMarker);
				ActiveTextAreaControl.TextArea.Invalidate();

				if (e.ToolTipShown || e.LogicalPosition.Line >= Document.TotalNumberOfLines) return;
				var line = Document.GetLineSegment(e.LogicalPosition.Line);
				if (line == null) return;
				lineAddressToolTip.Hide(FindForm());
				var word = e.InDocument ? GetAsmWord(e.LogicalPosition) : null;
				if (word == null || word.IsWhiteSpace || word.WordType == AsmWord.AsmWordType.Comment)
				{
					var debugLine = GetDebugLine(line.LineNumber);
					if (debugLine == null || debugLine.CpuAddress == null) return;

					testMarker.Offset = line.Offset;
					testMarker.Length = line.Length;
					Document.MarkerStrategy.AddMarker(testMarker);
					//e.ShowToolTip(WatchValue.FormatHexAddress(debugLine.Address));
					lineAddressToolTip.Show(
						WatchValue.FormatHexAddress(debugLine.CpuAddress.Value), 
						FindForm(),
						PointToScreen(new Point(-60, e.MousePosition.Y))
						, 3000 // TODO: Use a custom form object, not tooltips
					);
					return;
				}

				testMarker.Offset = word.Offset + line.Offset;
				testMarker.Length = word.Length;
				Document.MarkerStrategy.AddMarker(testMarker);
				ActiveTextAreaControl.TextArea.Invalidate();

				switch (word.WordType)
				{
					case AsmWord.AsmWordType.LabelAbsolute:
					case AsmWord.AsmWordType.LabelReference:
					case AsmWord.AsmWordType.LabelDefinition:
					case AsmWord.AsmWordType.Macro:
						e.ShowToolTip(GetSymbolDescription(word.Word));
						break;
					case AsmWord.AsmWordType.Command:
						var command = Ca65Parser.GetCommandFromWord(word.Word);
						if (command != null) e.ShowToolTip(command.ToString());
						break;
					case AsmWord.AsmWordType.Opcode:
						var opcode = OpcodeParser.GetOpcodeFromWord(word.Word, File.Project.Type);
						if (opcode != null) e.ShowToolTip(opcode.ToString());
						break;
					default:
						e.ShowToolTip(word.Word);
						break;
				}
			};

			Document.DocumentAboutToBeChanged += (s, arts) =>
			{
				_changedSinceLastCheck = true;
			};
			//Document.LineCountChanged += (sender, args) => RefreshErrorInfo();
			Document.LineLengthChanged += (s, args) =>
			{
				if (Document.MarkerStrategy.GetMarkers(args.LineSegment.Offset).Any(m => m is ErrorMarker))
				{
					RefreshErrorInfo();
				}
			};

			ActiveTextAreaControl.Caret.PositionChanged += (s, a) =>
			{
				HighlightCommandAtCaret();

				if (ActiveTextAreaControl.Caret.Line == _caretLine) return;

				_caretLine = ActiveTextAreaControl.Caret.Line;
				RefreshErrorInfo();
				HighlightOpcodeOnLine();
			};
			
			ActiveTextAreaControl.TextArea.KeyUp += delegate(object sender, KeyEventArgs e)
													{
														/*if (e.KeyCode == Program.Keys[Brewmaster.Settings.Feature.GoToDefinition])
														{
															GoToSymbol();
															return;
														}*/
														//if (e.KeyCode == Keys.Escape) return;
														if (NavigationKeys.Contains(e.KeyCode)) return;

														ShowIntellisense((char) e.KeyValue, 1);
														_forcedAutoCompleteWindow = false;

														HighlightOpcodeOnLine();
													};

			ActiveTextAreaControl.TextArea.IconBarMargin.MouseDown += (sender, mousepos, buttons) =>
			{
				if (buttons != MouseButtons.Left) return;
				var line = ActiveTextAreaControl.TextArea.TextView.GetLogicalLine(mousepos.Y);
				AddBreakpointMarker(line);
				RefreshBreakpointsInProject();
			};
			Document.DocumentChanged += (sender, args) =>
			{
				var changed = false;
				foreach (var bp in Document.BookmarkManager.Marks.OfType<BreakpointMarker>())
				{
					if (bp.GlobalBreakpoint.CurrentLine != bp.LineNumber + 1) changed = true;
					bp.GlobalBreakpoint.CurrentLine = bp.LineNumber + 1;
				}

				if (changed) RefreshBreakpointsInProject();
			};
		}

		private void HighlightCommandAtCaret()
		{
			var word = GetAsmWord(ActiveTextAreaControl.Caret.Position);
			if (word == null || word.WordType != AsmWord.AsmWordType.Command) return;

			var command = Ca65Parser.GetCommandFromWord(word.Word);
			if (command != null) ModuleEvents.HighlightCommand(command);
		}
		private void HighlightOpcodeOnLine()
		{
			var lineSegment = Document.GetLineSegment(_caretLine);
			if (lineSegment == null) return;
			var word = lineSegment.Words.OfType<AsmWord>().FirstOrDefault(w => w.WordType == AsmWord.AsmWordType.Opcode);

			if (word == null) return;
			var opcode = OpcodeParser.GetOpcodeFromWord(word.Word, File.Project.Type);
			if (opcode != null) ModuleEvents.HighlightOpcode(opcode);
		}

		private DebugLine GetDebugLine(int lineNumber)
		{
			var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>()
				.FirstOrDefault(bm => bm.LineNumber == lineNumber);
			if (buildLine == null) return null;

			if (!File.DebugLines.ContainsKey(buildLine.BuildLine + 1)) return null;
			return File.DebugLines[buildLine.BuildLine + 1];
		}

		private bool _forcedAutoCompleteWindow = false;

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData != Program.Keys[Feature.AutoComplete]) return base.ProcessCmdKey(ref msg, keyData);
			ShowIntellisense(' ', 0);
			_forcedAutoCompleteWindow = true;
			return true;
		}

		protected string GetSymbolDescription(string word)
		{
			//TODO: Watch manually written addresses, too!
			//TODO: X/Y offsets, maybe even DP?
			if (!File.Project.DebugSymbols.ContainsKey(word))
			{
				var macro = File.LocalSymbols.FirstOrDefault(s => s.Text == word) as MacroSymbol;
				if (macro != null) return macro.ToString();
				return word;
			}
			var symbol = File.Project.DebugSymbols[word];
			var memoryState = GetCpuMemory();
			if (memoryState == null) return string.Format("{0} ({1})", word, WatchValue.FormatHexAddress(symbol.Value));

			var val8 = memoryState.ReadAddress(symbol.Value);
			var val16 = memoryState.ReadAddress(symbol.Value, true);
			// TODO: Show where symbol is defined?
			return string.Format("{0} ({1})\n\nValue: {2} ({3})\nWord value: {4} ({5})",
				word,
				WatchValue.FormatHexAddress(symbol.Value),
				WatchValue.FormatHex(val8, 2), val8,
				WatchValue.FormatHex(val16, 4), val16
			);

		}

		private bool _changedSinceLastCheck = true;
		private bool _queueCheck = false;
		private Task _checkTask;
		private void RefreshErrorInfo()
		{
			if (!_changedSinceLastCheck) return;
			if (_checkTask != null && !_checkTask.IsCompleted)
			{
				_queueCheck = true;
				return;
			}

			_checkTask = Task.Run(async () =>
			{
				var errors = await ParseErrors(Document.TextContent);
				_changedSinceLastCheck = false;

				Document.MarkerStrategy.RemoveAll(m => m is ErrorMarker);
				foreach (var buildError in errors.Where(e => e.Type == BuildHandler.BuildError.BuildErrorType.Error))
				{
					var line = Document.GetLineSegment(buildError.Line - 1);
					Document.MarkerStrategy.AddMarker(new ErrorMarker(line.Offset, line.Length, buildError.Message, Document.HighlightingStrategy.GetColorFor("Assembler error").Color));
				}
				ActiveTextAreaControl.TextArea.Invalidate();

				await Task.Delay(3000); // Create a delay between check tasks to avoid spamming the file system
				if (_queueCheck)
				{
					_queueCheck = false;
					_checkTask = null;
					RefreshErrorInfo();
				}
			});
		}

		private AsmWord GetAsmWord(TextLocation position)
		{
			var line = Document.GetLineSegment(position.Line);
			var word = line.GetWord(position.Column);

			if (position.Column > 0 && (word == null || word.Type == TextWordType.Space || word.Type == TextWordType.Tab))
				word = line.GetWord(position.Column - 1);

			if (word == null) return null;
			var asmWord = word as AsmWord;

			if(asmWord == null && File.Project.Symbols != null && File.Project.DebugSymbols != null
				&& (File.Project.DebugSymbols.ContainsKey(word.Word) || File.Project.Symbols.Any(s => s.Key == word.Word)))
			{
				var type = AsmWord.AsmWordType.LabelReference;
				var macro = File.Project.Symbols.FirstOrDefault(s => s.Key == word.Word).Value as MacroSymbol;
				if (macro != null) type = AsmWord.AsmWordType.Macro;
				return new AsmWord(Document, line, word.Offset, word.Length, new HighlightColor(Color.Black, false, false), true, type);
			}
			if (IsIncludeLine(line) && asmWord != null && asmWord.WordType == AsmWord.AsmWordType.String)
			{
				asmWord.WordType = AsmWord.AsmWordType.FileReference;
			}
			return asmWord;
		}
		private void GoToSymbol()
		{
			// TODO: When we can identify labels correctly
			//var word = GetAsmWord(ActiveTextAreaControl.Caret.Position);
			//if (word == null || word.WordType != AsmWord.AsmWordType.Label) return;

			var position = ActiveTextAreaControl.Caret.Position;
			var line = Document.GetLineSegment(position.Line);
			var word = line.GetWord(position.Column);

			if (position.Column > 0 && (word == null || word.Type == TextWordType.Space || word.Type == TextWordType.Tab))
				word = line.GetWord(position.Column-1);

			if (word == null) return;

			if (word is AsmWord asmWord && (asmWord.WordType == AsmWord.AsmWordType.String || asmWord.WordType == AsmWord.AsmWordType.FileReference))
			{
				if (IsIncludeLine(line))
				{
					GoToFile(word.Word.Trim('\'', '\"'));
				}
				return;
			}

			var symbols = File.Project.Symbols; // TODO: use local symbol index
			var matchingSymbol = symbols.Where(s => s.Value.Text == word.Word).Select(s => s.Value)
				.OrderBy(s => s.Source != File.File.FullName).FirstOrDefault();
			if (matchingSymbol != null && File.Project.GoTo != null)
				File.Project.GoTo(matchingSymbol.Source, matchingSymbol.Line, matchingSymbol.Character);
		}

		private bool IsIncludeLine(LineSegment line)
		{
			return line.Words.OfType<AsmWord>()
				.Any(w => w.WordType == AsmWord.AsmWordType.Command &&
				          (w.Word.Equals(".INCLUDE", StringComparison.InvariantCultureIgnoreCase) ||
				           w.Word.Equals(".INCBIN", StringComparison.InvariantCultureIgnoreCase)));
		}

		private void GoToFile(string fileReference)
		{
			AsmProjectFile foundFile = null;
			try
			{
				var matchFiles = new[] {
					Path.Combine(File.GetRelativeDirectory(), fileReference).Replace('\\', '/'),
					fileReference.Replace('\\', '/')
				};
				var completeMatch = matchFiles.Select(mf => new DirectoryInfo(Path.Combine(File.Project.Directory.FullName, mf)).FullName);
				foundFile = File.Project.Files.FirstOrDefault(f =>
					completeMatch.Any(mf => mf.Equals(f.File.FullName, StringComparison.InvariantCultureIgnoreCase)) ||
					matchFiles.Any(mf => mf.Equals(f.GetRelativePath(), StringComparison.InvariantCultureIgnoreCase)));
			}
			catch
			{
				return;
			}
			if (foundFile != null) ModuleEvents.OpenFile(foundFile);
		}

		public void UpdateBreakpointsInEditor()
		{
			if (File.EditorBreakpoints == null) return;
			// Copy array, since each "AddBreakpoint()" will attempt to update the Project File
			var allBreakpoints = File.EditorBreakpoints;
			Document.BookmarkManager.RemoveMarks(m => m is BreakpointMarker);
			foreach (var breakpoint in allBreakpoints)
			{
				// TODO: TRACK BOTH BUILD AND CURRENT LINES IN EDITOR???
				//var line = breakpoint.CurrentLine - 1;
				//var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>().FirstOrDefault(m => m.BuildLine == line);
				//if (buildLine != null) line = buildLine.LineNumber;
				//var marker = AddBreakpointMarker(line, !breakpoint.Disabled);
				var marker = AddBreakpointMarker(breakpoint.CurrentLine - 1, !breakpoint.Disabled);
				marker.GlobalBreakpoint = breakpoint;
				breakpoint.EnabledChanged += () => marker.IsEnabled = !breakpoint.Disabled;
			}
		}

		private void RefreshBreakpointsInProject()
		{
			foreach (var breakpointMarker in Document.BookmarkManager.Marks.OfType<BreakpointMarker>())
				if (breakpointMarker.GlobalBreakpoint != null)
					breakpointMarker.GlobalBreakpoint.BuildLine = breakpointMarker.BuildLine + 1;

			File.SetEditorBreakpoints(Document.BookmarkManager.Marks.OfType<BreakpointMarker>()
				.Select(b => b.GlobalBreakpoint ?? new Breakpoint
				{
					Type = Breakpoint.Types.Execute,
					AddressType = Breakpoint.AddressTypes.PrgRom,
					Automatic = true,
					File = File,
					CurrentLine = b.LineNumber + 1,
					BuildLine = b.BuildLine + 1
				}));
			UpdateBreakpointsInEditor();
		}

		public void UpdateBreakpointsWithBuildInfo()
		{
			Document.BookmarkManager.RemoveMarks((mark) => mark is BuildLineMarker);
			if (File.DebugLines == null) return;
			
			foreach (var buildLine in File.DebugLines.Values)
			{
				Document.BookmarkManager.AddMark(new BuildLineMarker(Document, buildLine.Line - 1));
			}

			foreach (var breakpoint in Document.BookmarkManager.Marks.OfType<BreakpointMarker>())
			{
				breakpoint.BuildLine = breakpoint.LineNumber;
				breakpoint.Healthy = File.DebugLines.ContainsKey(breakpoint.BuildLine + 1);
			}

			RefreshBreakpointsInProject();
			ActiveTextAreaControl.TextArea.Invalidate();
		}
		public void RemoveFocusArrow()
		{
			Document.BookmarkManager.RemoveMarks((mark) => { return mark is PcArrow; });
			ActiveTextAreaControl.TextArea.Invalidate();
		}
		public void FocusArrow(int line)
		{
			RemoveFocusArrow();
			var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>().FirstOrDefault(m => m.BuildLine == line - 1);
			if (buildLine == null) return;

			Document.BookmarkManager.AddMark(new PcArrow(Document, buildLine.LineNumber));
			ActiveTextAreaControl.TextArea.Invalidate();
			ActiveTextAreaControl.TextArea.ScrollTo(buildLine.LineNumber);
		}

		public DebugLine GetFocusArrow()
		{
			var arrow = Document.BookmarkManager.Marks.OfType<PcArrow>().FirstOrDefault();
			var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>().FirstOrDefault(m => m.LineNumber == arrow.LineNumber);
			return File.DebugLines.Values.FirstOrDefault(dbl => dbl.Line == buildLine.BuildLine + 1);
		}

		private void ShowIntellisense(char keyValue, int wordLengthLimit)
		{
			if (_completionDataProvider == null) return;
			if (ActiveTextAreaControl.SelectionManager.HasSomethingSelected) return;

			var caretPosition = ActiveTextAreaControl.Caret;
			var line = Document.GetLineSegment(caretPosition.Line);
			var word = line.GetWord(caretPosition.Column - 1);

			var wordType = (word as AsmWord)?.WordType;
			if (wordType == AsmWord.AsmWordType.Comment)
			{
				if (_codeCompletionWindow != null) _codeCompletionWindow.Close();
				return;
			}
			if (wordType == AsmWord.AsmWordType.Opcode && wordLengthLimit > 0 && !_forcedAutoCompleteWindow)
			{
				if (_codeCompletionWindow != null) _codeCompletionWindow.Close();
				return;
			}
			if ((wordType == AsmWord.AsmWordType.NumberByte || wordType == AsmWord.AsmWordType.NumberWord) && wordLengthLimit > 0 && !_forcedAutoCompleteWindow)
			{
				if (_codeCompletionWindow != null) _codeCompletionWindow.Close();
				return;
			}

			if (word.Word.Trim().Length < wordLengthLimit)
			{
				if ((word.IsWhiteSpace || string.IsNullOrWhiteSpace(word.Word)) && !_forcedAutoCompleteWindow && _codeCompletionWindow != null) _codeCompletionWindow.Close();
				return;
			}
			var provider = _completionDataProvider;
			if (IsIncludeLine(line) && caretPosition.Column > 0)
			{
				// Auto-complete file names for include statements
				var fullLine = Document.GetText(line);
				var stringStart = fullLine.LastIndexOf("\"", caretPosition.Column - 1);
				if (stringStart > 0 && stringStart == fullLine.IndexOf("\""))
				{
					// Open file completion if caret is after the first " of the line
					provider = _fileCompletionDataProvider;
					provider.PreSelection = fullLine.Substring(stringStart, caretPosition.Column - stringStart);
				}
			}
			if (provider == _completionDataProvider)
			{
				// Regular auto-complete for symbols
				if (word.Offset + word.Length != caretPosition.Column)
				{
					if (_codeCompletionWindow != null) _codeCompletionWindow.Close();
					return;
				}
				provider.PreSelection = word.Word;
			}

			if (_codeCompletionWindow != null)
			{
				_codeCompletionWindow.RefreshCompletionData(File.File.FullName, keyValue);
				return;
			}

			try
			{
				_codeCompletionWindow = CompletionWindow.ShowCompletionWindow(
					ParentForm,					// The parent window for the completion window
					this,						// The text editor to show the window for
					File.File.FullName,			// Filename - will be passed back to the provider
					provider,					// Provider to get the list of possible completions
					keyValue					// Key pressed - will be passed to the provider
				);
			}
			catch (ArgumentOutOfRangeException)
			{
				_codeCompletionWindow = null;
			}
			if (_codeCompletionWindow != null) // ShowCompletionWindow can return null when the provider returns an empty list
			{
				_codeCompletionWindow.CloseWhenCaretAtBeginning = false;
				_codeCompletionWindow.Closed += CloseCodeCompletionWindow;
			}
		}

		private void CloseCodeCompletionWindow(object sender, EventArgs e)
		{
			_codeCompletionWindow = null;
		}

		public void AddMarker(int offset, int length, TextMarkerType type, Color color)
		{
			Document.MarkerStrategy.AddMarker(new TextMarker(offset, length, type, color));
		}

		public void ToggleBreakpointAtCaret()
		{
			var line = ActiveTextAreaControl.Caret.Line;
			var existingBreakpoint = Document.BookmarkManager.Marks.OfType<BreakpointMarker>().FirstOrDefault(m => m.LineNumber == line);
			if (existingBreakpoint != null)
			{
				Document.BookmarkManager.RemoveMark(existingBreakpoint);
				ActiveTextAreaControl.TextArea.Invalidate();
				RefreshBreakpointsInProject();
				return;
			}
			AddBreakpointMarker(line);
			RefreshBreakpointsInProject();
		}
		private BreakpointMarker AddBreakpointMarker(int line, bool enabled = true)
		{
			var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>().FirstOrDefault(m => m.LineNumber == line);
			return AddBreakpointMarker(line, buildLine == null ? line : buildLine.BuildLine, buildLine != null, enabled);
		}
		private BreakpointMarker AddBreakpointMarker(int line, int buildLine, bool healthy, bool enabled)
		{
			var marker = new BreakpointMarker(Document, line, buildLine, enabled, healthy, RefreshBreakpointsInProject);
			Document.BookmarkManager.AddMark(marker);
			var arrow = Document.BookmarkManager.Marks.OfType<PcArrow>().FirstOrDefault();
			if (arrow != null)
			{
				// Ensure the PC Arrow is always above the breakpoint
				Document.BookmarkManager.RemoveMark(arrow);
				Document.BookmarkManager.AddMark(arrow);
			}
			ActiveTextAreaControl.TextArea.Invalidate();
			return marker;
		}
	}

	public class Ca65Editor : CodeEditor
	{
		public Ca65Editor(AsmProjectFile file, Events events) : base(file, events)
		{
			_completionDataProvider = new Ca65Completion(File.Project, GetSymbolDescription, events);
			Document.TextContentChanged += (sender, args) =>
												{

												};

			ActiveTextAreaControl.Caret.PositionChanged += (s, a) => IdentifyLocalLabels();

			_labelStartMarker = new TextMarker(0, 1, TextMarkerType.SolidBlock, Document.HighlightingStrategy.GetColorFor("Highlighted word").BackgroundColor);
			_labelEndMarker = new TextMarker(0, 0, TextMarkerType.SolidBlock, Document.HighlightingStrategy.GetColorFor("Highlighted word").BackgroundColor);
		}

		private TextMarker _labelStartMarker;
		private TextMarker _labelEndMarker;
		private bool _labelsMarked = false;
		private bool _showCpuAddresses;

		public bool ShowCpuAddresses
		{
			get { return _showCpuAddresses; }
			set
			{
				_showCpuAddresses = value;
				ActiveTextAreaControl.TextArea.Invalidate();
			}
		}

		private void IdentifyLocalLabels()
		{
			var marked = MarkLocalLabels();
			// Only invalidate when the marking potentially changed
			if (marked || _labelsMarked) ActiveTextAreaControl.TextArea.Invalidate();
			_labelsMarked = marked;
		}
		private bool MarkLocalLabels()
		{
			Document.MarkerStrategy.RemoveMarker(_labelStartMarker);
			Document.MarkerStrategy.RemoveMarker(_labelEndMarker);

			// TODO: These lines are quite redundant right now
			var position = ActiveTextAreaControl.TextArea.Caret.Position;
			var line = Document.GetLineSegment(position.Line);
			var word = line.GetWord(position.Column);
			if (position.Column > 0 && (word == null || word.Type == TextWordType.Space || word.Type == TextWordType.Tab))
				word = line.GetWord(position.Column - 1);

			if (word == null) return false;
			var searchBack = Regex.Match(word.Word, @"^:(\-+)$");
			var searchAhead = Regex.Match(word.Word, @"^:(\++)$");

			var startPosition = new TextLocation(word.Offset, position.Line);
			var searchLine = startPosition.Line;
			int count, step;
			if (searchBack.Success)
			{
				count = searchBack.Groups[1].Value.Length;
				step = -1;
			}
			else if (searchAhead.Success)
			{
				count = searchAhead.Groups[1].Value.Length;
				step = 1;
			}
			else return false;

			TextWord firstWord;
			LineSegment matchingLine;
			do
			{
				matchingLine = Document.GetLineSegment(searchLine);
				firstWord = Document.GetLineSegment(searchLine).Words.FirstOrDefault(w => !string.IsNullOrWhiteSpace(w.Word));
				if (firstWord != null && firstWord.Word == ":") count--;
				searchLine += step;
				if (searchLine < 0 || searchLine >= Document.TotalNumberOfLines) return false;
			} while (count > 0);

			_labelStartMarker.Offset = word.Offset + line.Offset;
			_labelStartMarker.Length = word.Length;
			_labelEndMarker.Offset = firstWord.Offset + matchingLine.Offset;
			Document.MarkerStrategy.AddMarker(_labelStartMarker);
			Document.MarkerStrategy.AddMarker(_labelEndMarker);
			return true;
		}

	}

}