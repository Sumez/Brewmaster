using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brewmaster.ProjectModel;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.EditorWindows.Code
{
	public class AsmWord : TextWord
	{
		public AsmWordType WordType { get; set; }
		public AsmWord(IDocument document, LineSegment line, int offset, int length, HighlightColor color, bool hasDefaultColor, AsmWordType wordType)
			: base(document, line, offset, length, color, hasDefaultColor)
		{
			WordType = wordType;
		}

		public enum AsmWordType {
			None, Opcode, LabelDefinition, LabelReference, LabelAbsolute, Macro, NumberWord, NumberByte, String,
			Command, Comment, FileReference
		}

		public bool IsLabel
		{
			get
			{
				return WordType == AsmWordType.LabelAbsolute || WordType == AsmWordType.LabelReference ||
				       WordType == AsmWordType.LabelDefinition;
			}
		}
	}

	public class Ca65Span
	{
		public bool IsBeginSingleWord { get; set; }
		public bool? IsBeginStartOfLine { get; set; }
		public Char[] Begin { get; set; }
		public HighlightColor BeginColor { get; set; }
		public bool IgnoreCase { get; set; }
		public HighlightColor Color { get; set; }
		public char EscapeCharacter { get; set; }
		public Char[] End { get; set; }
		public HighlightColor EndColor { get; set; }
		public AsmWord.AsmWordType Type { get; set; }
	}

	public class Ca65WordType
	{
		public AsmWord.AsmWordType Type { get; set; }
		public string Match { get; set; }
		public HighlightColor Color { get; set; }
	}

	public class Ca65Highlighting : IHighlightingStrategy
	{
		public Dictionary<string, string> Properties { get; set; }
		public string Name { get { return "ca65"; } }
		public HighlightRuleSet Rule { get; set; }
		public List<Ca65Span> Spans { get; set; }
		public List<Ca65WordType> Type { get; set; }
		public string[] Extensions { get; set; }
		public HighlightColor DefaultTextColor { get; set; }
		private HighlightColor DigitColor { get; set; }
		public HighlightColor OpcodeColor { get; set; }
		public Ca65WordType StringWord { get; set; }
		public Ca65WordType NumberWord { get; set; }
		public Ca65WordType CommandWord { get; set; }
		public Ca65Span CommentSpan { get; set; }
		public Dictionary<string, HighlightColor> Colors { get; set; }
		public static Dictionary<string, HighlightColor> DefaultColors = new Dictionary<string, HighlightColor>
		{
			{"Default", new HighlightColor(Color.Black, Color.White, false, false) },
			{"Opcodes", new HighlightColor(Color.CadetBlue, true, false) },
			{"Digits", new HighlightColor(Color.Red, false, false) },
			{"Strings", new HighlightColor(Color.DarkOrange, false, false) },
			{"Comments", new HighlightColor(Color.DarkGray, false, true) },
			{"Control commands", new HighlightColor(Color.DarkSlateGray, true, false) },
			{"Label definition",  new HighlightColor(Color.DarkGreen, false, false) },
			{"Selection", new HighlightBackground(Color.Black, Color.LightBlue,  false, false)},
			{"LineNumbers", new HighlightColor(Color.Gray, Color.FromArgb(248, 248, 248), false, false)},
			{"CPU Addresses", new HighlightColor(Color.DarkGray, Color.FromArgb(248, 248, 248), true, false)},

			{"Highlighted word", new HighlightBackground(Color.Black, Color.FromArgb(230, 230, 230), false, false)},
			{"Assembler error", new HighlightColor(Color.Red, false, false)}


		/*{ "FoldLine", new HighlightBackground(Color.Red, Color.Green, false, false)},
		{"SelectedFoldLine", new HighlightBackground(Color.Red, Color.Green, false, false)},
		{"TabMarkers", new HighlightBackground(Color.Red, Color.Green, false, false)},
		{"SpaceMarkers", new HighlightBackground(Color.Red, Color.Green, false, false)},
		{"VRuler", new HighlightBackground(Color.Red, Color.Green, false, false)}*/
	};




		private static HashSet<string> nesOpcodes = new HashSet<string>
												{
													"ADC", "AND", "ASL", "BCC", "BCS", "BEQ", "BIT", "BMI", "BNE", "BPL", "BRK", "BVC", "BVS", "CLC",
													"CLD", "CLI", "CLV", "CMP", "CPX", "CPY", "DEC", "DEX", "DEY", "EOR", "INC", "INX", "INY", "JMP", "JSR", "LDA", "LDX", "LDY", "LSR", "NOP", "ORA", "PHA", "PHP", "PLA", "PLP", "ROL", "ROR", "RTI", "RTS", "SBC", "SEC", "SED", "SEI", "STA", "STX", "STY", "TAX", "TAY", "TSX", "TXA", "TXS", "TYA"
												};
		private static HashSet<string> snesOpcodes = new HashSet<string>
		{
			"BRA","BRL","COP","JML","JSL","MVN","MVP","PEA","PEI","PER","PHB","PHD","PHK","PHX","PHY","PLB",
			"PLD","PLX","PLY","REP","RTL","SEP","STP","STZ","TCD","TCS","TDC","TSC","TXY","TYX","TRB","TSB",
			"WAI","WDM","XBA","XCE",
			"TAD","TAS","TDA","TSA", // Irregular way of writing the TCD, etc. opcodes, accepted by CA65
		};
		private HashSet<string> opcodes;

		const string OperatorMatch = @"^[+\-,=~<>*/&\^|:!()\[\]]+";
		const string CommandMatch = @"^\.[a-zA-Z0-9]+";
		const string StringMatch = @"^('|"").*?\1";
		const string NumberMatch = @"^(\$[0-9a-fA-F]+|[0-9]+|%[01]+|[0-9][0-9a-fA-F]*h)(?=\b)";
		public Ca65Highlighting(ProjectType projectType)
		{
			Extensions = new[] {"s", "h"};
			opcodes = new HashSet<string>(nesOpcodes);
			if (projectType == ProjectType.Snes)opcodes.UnionWith(snesOpcodes);
			Properties = new Dictionary<string, string>();
			Properties.Add("LineComment", ";");
			Rule = new HighlightRuleSet();

			NumberWord = new Ca65WordType
			{
				Type = AsmWord.AsmWordType.NumberWord,
				Match = NumberMatch
			};
			StringWord = new Ca65WordType
			{
				Type = AsmWord.AsmWordType.String,
				Match = StringMatch
			};
			CommandWord = new Ca65WordType
			{
				Type = AsmWord.AsmWordType.Command,
				Match = CommandMatch
			};
			CommentSpan = new Ca65Span
			{
				Begin = new char[] { ';' },
				End = new char[] { '\n' },
				Type = AsmWord.AsmWordType.Comment
			};

			UpdateColorsFromDefault();

			Type = new List<Ca65WordType> { NumberWord, StringWord, CommandWord };
			Spans = new List<Ca65Span> { CommentSpan };

		}

		public void UpdateColorsFromDefault()
		{
			Colors = DefaultColors.ToDictionary(i => i.Key, i => i.Value);
			foreach (var highlightColor in Colors.ToArray())
			{
				SetColor(highlightColor.Key, highlightColor.Value);
			}
		}

		public HighlightColor GetColorFor(string name)
		{
			//return _baseStrategy.GetColorFor(name);
			//if (name.Length == 3 && opcodes.Contains(name)) return new HighlightColor(Color.CadetBlue, Color.White, true, false);
			//return new HighlightColor(default(Color), false, false);

			if (Colors.ContainsKey(name)) return Colors[name];
			return DefaultTextColor;
		}

		// Line state variable
		protected LineSegment currentLine;
		protected int currentLineNumber;

		// Span stack state variable
		public void MarkTokens(IDocument document)
		{
		}
		protected virtual HighlightColor GetColor(HighlightRuleSet ruleSet, IDocument document, LineSegment currentSegment, int currentOffset, int currentLength)
		{
			if (ruleSet != null)
			{
				if (ruleSet.Reference != null)
				{
					return DefaultTextColor;
				}
				return (HighlightColor)ruleSet.KeyWords[document, currentSegment, currentOffset, currentLength];
			}
			return null;
		}
		/// <summary>
		/// pushes the curWord string on the word list, with the
		/// correct color.
		/// </summary>
		void PushCurWord(IDocument document, ref HighlightColor markNext, List<TextWord> words, bool firstWord = false)
		{
			var color = DefaultTextColor;
			if (!inSpan && currentLine.Length > currentOffset + currentLength)
			{
				var word = document.GetText(currentLine.Offset + currentOffset, currentLength + 1);
				if (firstWord && Regex.IsMatch(word, @"[a-zA-Z][^\s]*\:")) color = Colors["Label definition"];
			}

			if (inSpan) words.Add(new AsmWord(document, currentLine, currentOffset, currentLength, activeSpan.Color, true, activeSpan.Type));
			else words.Add(new TextWord(document, currentLine, currentOffset, currentLength, color, true));
			currentOffset += currentLength;
			currentLength = 0;
		}

		protected List<Ca65Span> currentSpanStack;
		protected bool inSpan;
		protected Ca65Span activeSpan;
		protected HighlightRuleSet activeRuleSet;

		// Line scanning state variables
		protected int currentOffset;
		protected int currentLength;


		private void PopSpanStack()
		{
			currentSpanStack.RemoveAt(currentSpanStack.Count - 1);
			inSpan = (currentSpanStack != null && currentSpanStack.Count > 0);
			activeSpan = inSpan ? currentSpanStack[currentSpanStack.Count - 1] : null;
			activeRuleSet = new HighlightRuleSet();
		}

		List<TextWord> ParseLine(IDocument document)
		{
			List<TextWord> words = new List<TextWord>();
			HighlightColor markNext = null;

			currentOffset = 0;
			currentLength = 0;
			inSpan = (currentSpanStack != null && currentSpanStack.Count > 0);
			activeSpan = inSpan ? currentSpanStack[currentSpanStack.Count - 1] : null;
			activeRuleSet = Rule;

			int currentLineLength = currentLine.Length;
			int currentLineOffset = currentLine.Offset;
			var firstWord = true;
			var isWord = false;
			var opcodeParams = false;
			var absoluteValue = false;

			for (int i = 0; i < currentLineLength; ++i)
			{
				char ch = document.GetCharAt(currentLineOffset + i);
				switch (ch)
				{
					case '\n':
					case '\r':
						PushCurWord(document, ref markNext, words, firstWord);
						if (isWord) firstWord = false;
						++currentOffset;
						break;
					case ' ':
						PushCurWord(document, ref markNext, words, firstWord);
						if (isWord) firstWord = false;
						if (activeSpan != null && activeSpan.Color.HasBackground)
						{
							words.Add(new TextWord.SpaceTextWord(activeSpan.Color));
						}
						else
						{
							words.Add(TextWord.Space);
						}
						++currentOffset;
						break;
					case '\t':
						PushCurWord(document, ref markNext, words, firstWord);
						if (isWord) firstWord = false;
						if (activeSpan != null && activeSpan.Color.HasBackground)
						{
							words.Add(new TextWord.TabTextWord(activeSpan.Color));
						}
						else
						{
							words.Add(TextWord.Tab);
						}
						++currentOffset;
						break;
					default:
						var lookAhead = document.GetText(currentLineOffset + i, (currentLineLength - i));

						var match = Regex.Match(lookAhead, OperatorMatch);
						if (match.Success)
						{
							PushCurWord(document, ref markNext, words, firstWord);
							if (isWord) firstWord = false;
							currentLength = match.Length;
							PushCurWord(document, ref markNext, words, firstWord);
							isWord = true;
							i += match.Length - 1;

							continue;
						}

						isWord = true;

						// handle escape characters
						var escapeCharacter = '\0';
						if (activeSpan != null && activeSpan.EscapeCharacter != '\0')
						{
							escapeCharacter = activeSpan.EscapeCharacter;
						}
						else if (activeRuleSet != null)
						{
							escapeCharacter = activeRuleSet.EscapeCharacter;
						}
						if (escapeCharacter != '\0' && escapeCharacter == ch)
						{
							// we found the escape character
							if (activeSpan != null && activeSpan.End != null && activeSpan.End.Length == 1
								&& escapeCharacter == activeSpan.End[0])
							{
								// the escape character is a end-doubling escape character
								// it may count as escape only when the next character is the escape, too
								if (i + 1 < currentLineLength)
								{
									if (document.GetCharAt(currentLineOffset + i + 1) == escapeCharacter)
									{
										currentLength += 2;
										PushCurWord(document, ref markNext, words);
										++i;
										continue;
									}
								}
							}
							else
							{
								// this is a normal \-style escape
								++currentLength;
								if (i + 1 < currentLineLength)
								{
									++currentLength;
								}
								PushCurWord(document, ref markNext, words);
								++i;
								continue;
							}
						}
						if (!inSpan && currentLength == 0)
						{
							// Handle new words from the beginning
							if (ch == '#')
							{
								currentLength += 1;
								words.Add(new AsmWord(document, currentLine, currentOffset, currentLength, DigitColor, false, AsmWord.AsmWordType.None));
								if (opcodeParams) absoluteValue = true;
								i += currentLength - 1;
								currentOffset += currentLength;
								currentLength = 0;
								continue;
							}

							foreach (var type in Type)
							{
								var typeMatch = Regex.Match(lookAhead, type.Match);
								if (typeMatch.Success)
								{
									currentLength = typeMatch.Length;
									words.Add(new AsmWord(document, currentLine, currentOffset, currentLength, type.Color, false, type.Type));
									i += currentLength - 1;
									currentOffset += currentLength;
									currentLength = 0;
									goto @skip;
								}
							}


							// highlight opcodes
							if (i + 2 < currentLineLength &&
								(i + 3 == currentLineLength || Char.IsWhiteSpace(document.GetCharAt(currentLineOffset + i + 3))))
							{
								var potentialOpcode = new string(new []
																{
																	document.GetCharAt(currentLineOffset + i + 0),
																	document.GetCharAt(currentLineOffset + i + 1),
																	document.GetCharAt(currentLineOffset + i + 2)
																});
								if (opcodes.Contains(potentialOpcode.ToUpperInvariant()))
								{
									currentLength += 3; // add 3 letter word
									i += 2; // skip the next two letters
									words.Add(new AsmWord(document, currentLine, currentOffset, currentLength, OpcodeColor, false, AsmWord.AsmWordType.Opcode));
									opcodeParams = true; // Anything after this will be treated as parameters to the opcode
									currentOffset += currentLength;
									currentLength = 0;
									continue; // If opcode, we have already completed the word, so move on to next word
								}
							}

						}

						// Check for SPAN ENDs
							if (inSpan)
						{
							if (activeSpan.End != null && activeSpan.End.Length > 0)
							{
								if (MatchExpr(currentLine, activeSpan.End, i, document, activeSpan.IgnoreCase))
								{
									PushCurWord(document, ref markNext, words);
									string regex = GetRegString(currentLine, activeSpan.End, i, document);
									currentLength += regex.Length;
									words.Add(new TextWord(document, currentLine, currentOffset, currentLength, activeSpan.EndColor ?? activeSpan.Color, false));
									currentOffset += currentLength;
									currentLength = 0;
									i += regex.Length - 1;
									PopSpanStack();

									continue;
								}
							}
						}

						// check for SPAN BEGIN
						foreach (var span in Spans)
						{
							if ((!span.IsBeginSingleWord || currentLength == 0)
								&&
								(!span.IsBeginStartOfLine.HasValue ||
								span.IsBeginStartOfLine.Value ==
								(currentLength == 0 &&
								words.TrueForAll(textWord => textWord.Type != TextWordType.Word)))
								&& MatchExpr(currentLine, span.Begin, i, document, activeRuleSet.IgnoreCase))
							{
								PushCurWord(document, ref markNext, words, firstWord);
								string regex = GetRegString(currentLine, span.Begin, i, document);

								//if (!OverrideSpan(regex, document, words, span, ref i)) {

								currentLength += regex.Length;
								words.Add(new TextWord(document, currentLine, currentOffset, currentLength, span.BeginColor ?? span.Color, false));
								currentOffset += currentLength;
								currentLength = 0;

								i += regex.Length - 1;
								if (currentSpanStack == null)
								{
									currentSpanStack = new List<Ca65Span>();
								}
								currentSpanStack.Add(span);
								span.IgnoreCase = activeRuleSet.IgnoreCase;

								inSpan = currentSpanStack != null && currentSpanStack.Count > 0;
								activeSpan = inSpan ? currentSpanStack[currentSpanStack.Count - 1] : null;
								activeRuleSet = new HighlightRuleSet();

								goto @skip;
							}

						}

						// check if the char is a delimiter
						if (activeRuleSet != null && (int) ch < 256 && activeRuleSet.Delimiters[(int) ch])
						{
							PushCurWord(document, ref markNext, words);
							if (currentOffset + currentLength + 1 < currentLine.Length)
							{
								++currentLength;
								PushCurWord(document, ref markNext, words);
								continue;
							}
						}

						++currentLength;
						@skip:  continue;
				}
			}

			PushCurWord(document, ref markNext, words, firstWord);
			while (activeSpan != null && activeSpan.End != null && activeSpan.End[0] == '\n') PopSpanStack();
			
			//OnParsedLine(document, currentLine, words);

			return words;
		}

		private bool MatchExpr(LineSegment lineSegment, char[] expr, int index, IDocument document, bool ignoreCase)
		{
			for (int i = 0, j = 0; i < expr.Length; ++i, ++j)
			{
				if (index + j >= lineSegment.Length) return false;

				char docChar = ignoreCase
					? Char.ToUpperInvariant(document.GetCharAt(lineSegment.Offset + index + j))
					: document.GetCharAt(lineSegment.Offset + index + j);
				char spanChar = ignoreCase ? Char.ToUpperInvariant(expr[i]) : expr[i];
				if (docChar != spanChar) return false;
			}
			return true;
		}

		static string GetRegString(LineSegment lineSegment, char[] expr, int index, IDocument document)
		{
			int j = 0;
			StringBuilder regexpr = new StringBuilder();

			for (int i = 0; i < expr.Length; ++i, ++j)
			{
				if (index + j >= lineSegment.Length)
					break;

				switch (expr[i])
				{
					case '@': // "special" meaning
						++i;
						switch (expr[i])
						{
							case '!': // don't match the following expression
								StringBuilder whatmatch = new StringBuilder();
								++i;
								while (i < expr.Length && expr[i] != '@')
								{
									whatmatch.Append(expr[i++]);
								}
								break;
							case '@': // matches @
								regexpr.Append(document.GetCharAt(lineSegment.Offset + index + j));
								break;
						}
						break;
					default:
						if (expr[i] != document.GetCharAt(lineSegment.Offset + index + j))
						{
							return regexpr.ToString();
						}
						regexpr.Append(document.GetCharAt(lineSegment.Offset + index + j));
						break;
				}
			}
			return regexpr.ToString();
		}

		private void MarkTokensInLine(IDocument document, int lineNumber)
		{
			currentLine = document.LineSegmentCollection[lineNumber];
			if (currentLine.Length == -1)
			{ // happens when buffer is empty !
				return;
			}

			var words = ParseLine(document);

			//// Alex: remove old words
			if (currentLine.Words != null) currentLine.Words.Clear();
			currentLine.Words = words;
			//currentLine.HighlightSpanStack = (currentSpanStack != null && currentSpanStack.Count > 0) ? currentSpanStack : null;
		}

		public void MarkTokens(IDocument document, List<LineSegment> inputLines)
		{
			var processedLines = new Dictionary<LineSegment, bool>();

			foreach (LineSegment lineToProcess in inputLines)
			{
				if (processedLines.ContainsKey(lineToProcess)) continue;
				var lineNumber = lineToProcess.LineNumber;
				if (lineNumber == -1) continue;

				MarkTokensInLine(document, lineNumber);
				processedLines[currentLine] = true;
			}

			if (inputLines.Count > 20)
			{
				// if the span was changed (more than inputLines lines had to be reevaluated)
				// or if there are many lines in inputLines, it's faster to update the whole
				// text area instead of many small segments
				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			}
			else
			{
				foreach (LineSegment lineToProcess in inputLines)
				{
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineToProcess.LineNumber));
				}
			}
			document.CommitUpdate();
			currentLine = null;
		}

		public void SetColor(string name, HighlightColor color)
		{
			Colors[name] = color;
			switch (name)
			{
				case "Default":
					DefaultTextColor = color;
					Colors["Selection"] = new HighlightBackground(DefaultTextColor.Color, Color.LightBlue, false, false);
					Colors["Highlighted word"] = new HighlightBackground(DefaultTextColor.Color, Colors["Highlighted word"].BackgroundColor, DefaultTextColor.Bold, DefaultTextColor.Italic);
					Colors["Assembler error"] = new HighlightColor(Colors["Assembler error"].Color, DefaultTextColor.Bold, DefaultTextColor.Italic);
					break;
				case "Opcodes":
					OpcodeColor = color;
					break;
				case "Strings":
					StringWord.Color = color;
					break;
				case "Comments":
					CommentSpan.Color = color;
					break;
				case "Control commands":
					CommandWord.Color = color;
					break;
				case "Digits":
					DigitColor = color;
					NumberWord.Color = color;
					break;
			}
		}
	}
}