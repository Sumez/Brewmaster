using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brewmaster.EditorWindows.Code;
using Brewmaster.Pipeline;

namespace Brewmaster.ProjectModel
{
	public class AsmProjectFile
	{
		// DATA ESSENTIAL TO OUR STORED FILE
		public FileInfo File { get; set; }
		public CompileMode Mode { get; set; }
		public DataPipeline Pipeline { get; set; }

		public IEnumerable<Breakpoint> EditorBreakpoints
		{
			get { return _editorBreakpoints.ToArray(); }
		}

		private List<Breakpoint> _editorBreakpoints = new List<Breakpoint>();
		// END

		public AsmProject Project { get; set; }
		public Dictionary<int, DebugLine> DebugLines { get; set; }
		public bool Missing { get; set; }

		public static readonly Regex GlobalSymbolRegex = new Regex(@"^\s*\.global(?:zp){0,1}\s+(.+?)\s*$", RegexOptions.IgnoreCase);
		public static readonly Regex ExportSymbolRegex = new Regex(@"^\s*\.export(?:zp){0,1}\s+(.+?)\s*$", RegexOptions.IgnoreCase);
		public static readonly Regex ImportSymbolRegex = new Regex(@"^\s*\.import(?:zp){0,1}\s+(.+?)\s*$", RegexOptions.IgnoreCase);
		public static readonly Regex DefineSymbolRegex = new Regex(@"^\s*([@a-z_][0-9a-z_]*?)(?::.*|\s*:{0,1}=.*)$", RegexOptions.IgnoreCase);
		public static readonly Regex DefineProcSymbolRegex = new Regex(@"^\s*\.proc\s+([@a-z_][0-9a-z_]*?)\s*$", RegexOptions.IgnoreCase);
		public static readonly Regex IncludeFileRegex = new Regex(@"^\s*\.include\s+\""(.+?)\""\s*$", RegexOptions.IgnoreCase);
		public static readonly Regex MacroRegex = new Regex(@"^\s*\.mac(?:ro){0,1}\s+([@a-z_][0-9a-z_]*)(?:\s+(.+)){0,1}$", RegexOptions.IgnoreCase);

		private static readonly Dictionary<FileType, string[]> FileTypes = new Dictionary<FileType, string[]>
		{
			{FileType.Image, new[] {".png", ".jpg", ".bmp", ".gif", ".chr"}},
			{FileType.TileMap, new[] {".bwmap"}},
			{FileType.Source, new[] {".asm", ".s"}},
			{FileType.Include, new[] {".h", ".inc", ".include", ".i"}},
			{FileType.Text, new[] {".txt", ".md", ".ini", ".cfg", ".json", ".xml", ".csv"}},
			{FileType.Script, new[] {".py", ".js", ".bat", ".php", ".yaml", ".mk"}},
			{FileType.Audio, new[] {".wav", ".pcm", ".dmc", ".nsf", ".wma", ".mp3", ".mid", ".ogg", ".xm", ".ft", ".it", ".mod"}},
			{FileType.FamiTracker, new[] {".ftm"}},
		};

		private static readonly FileType[] TextFiles = {
			FileType.Script,
			FileType.Source,
			FileType.Text,
			FileType.Include
		};
		private static readonly FileType[] ContentFiles = {
			FileType.Image,
			FileType.Audio,
			FileType.FamiTracker,
			FileType.TileMap
		};
		private static readonly string[] ContentFileExtensions = {".bin", ".json", ".xml"};

		private FileType _type = FileType.Unknown;
		public FileType Type { get
			{
				if (_type != FileType.Unknown) return _type;
				if (Mode == CompileMode.IncludeInAssembly) return FileType.Source;
				if (Mode == CompileMode.LinkerConfig) return FileType.Text;
				var extension = File.Extension.ToLower();
				foreach (var type in FileTypes)
				{
					if (type.Value.Contains(extension)) return type.Key;
				}
				return FileType.Unknown;
			}
			set { _type = value; }
		}

		public bool IsTextFile
		{
			get { return TextFiles.Contains(Type); }
		}

		public bool IsDataFile
		{
			get { return ContentFiles.Contains(Type) || ContentFileExtensions.Contains(File.Extension.ToLower()); }
		}

		public override string ToString()
		{
			return GetRelativePath();
		}

		// TODO: Is a "same dir" include path in an included file relative to the original file, or the included file
		private static void GetSymbolsFromText(string source, List<Symbol> newSymbols, List<string> exportedSymbols,
			string[] includePaths, Dictionary<string, List<AsmProject.FileInclude>> includeChain, List<string> files = null)
		{
			if (files == null) files = new List<string>();
			if (!System.IO.File.Exists(source)) return;

			var lineNumber = 0;
			// TODO: If a file is open in an editor, get text from the editor, instead of the saved file (issue #74)
			foreach (var line in System.IO.File.ReadLines(source))
			{
				lineNumber++;

				var includeMatch = IncludeFileRegex.Match(line);
				if (includeMatch.Success && includePaths.Length > 0)
				{
					var matchedFilename = includeMatch.Groups[1].Value;
					var childFilename = matchedFilename;
					foreach (var path in includePaths)
					{
						childFilename = Path.Combine(path, matchedFilename);
						if (System.IO.File.Exists(childFilename)) break;
					}
					if (!System.IO.File.Exists(childFilename)) continue;

					childFilename = new FileInfo(childFilename).FullName;
					var include = new AsmProject.FileInclude { IncludingFile = source, Line = lineNumber };
					if (!includeChain.ContainsKey(childFilename)) includeChain.Add(childFilename, new List<AsmProject.FileInclude>());
					if (!includeChain[childFilename].Contains(include)) includeChain[childFilename].Add(include);
					if (files.Contains(childFilename)) continue;
					files.Add(childFilename);

					GetSymbolsFromText(childFilename, newSymbols, exportedSymbols, includePaths, includeChain, files);
					continue;
				}

				var exportMatch = ExportSymbolRegex.Match(line);
				if (!exportMatch.Success) exportMatch = GlobalSymbolRegex.Match(line);
				if (exportMatch.Success)
				{
					exportedSymbols.AddRange(exportMatch.Groups[1].Value
						.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
						.Select(symbol => symbol.Trim()));
					continue;
				}
				var defineMatch = DefineSymbolRegex.Match(line);
				if (!defineMatch.Success) defineMatch = DefineProcSymbolRegex.Match(line);
				if (defineMatch.Success)
				{
					newSymbols.Add(new Symbol
					{
						Source = source,
						Text = defineMatch.Groups[1].Value,
						Character = defineMatch.Groups[1].Index,
						Line = lineNumber
					});
				}

				var macroMatch = MacroRegex.Match(line);
				if (macroMatch.Success)
				{
					newSymbols.Add(new MacroSymbol
					{
						Source = source,
						Text = macroMatch.Groups[1].Value,
						Character = macroMatch.Groups[1].Index,
						Line = lineNumber,
						Parameters = macroMatch.Groups[2].Success
							? macroMatch.Groups[2].Value.Split(',').Select(p => p.Trim()).ToList()
							: new List<string>()
					});
				}
			}

		}

		public event Action ParsedLocalSymbols;
		public List<Symbol> LocalSymbols = new List<Symbol>();
		public void AddSymbolsFromFile(List<KeyValuePair<string, Symbol>> symbols, Dictionary<string, List<AsmProject.FileInclude>> includeChain)
		{
			var exportedSymbols = new List<string>();
			LocalSymbols = new List<Symbol>();
			GetSymbolsFromText(File.FullName, LocalSymbols, exportedSymbols, new [] { File.DirectoryName, Project.Directory.FullName }, includeChain);
			foreach (var symbol in LocalSymbols)
			{
				symbol.LocalToFile = File.FullName; // TODO: Save individual list on each file instead?
				if (exportedSymbols.Contains(symbol.Text)) symbol.Public = true;
				symbols.Add(new KeyValuePair<string, Symbol>(symbol.Text, symbol));
			}
			if (ParsedLocalSymbols != null) ParsedLocalSymbols();
		}

		public void SetEditorBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			_editorBreakpoints = breakpoints.ToList();
			Project.RefreshBreakpoints();
		}
		public void RemoveBreakpoint(Breakpoint breakpoint)
		{
			_editorBreakpoints.Remove(breakpoint);
			Project.RefreshBreakpoints();
		}

		public string GetRelativeDirectory(bool trailingSlash = false)
		{
			var directory = Project.GetRelativePath(File.DirectoryName);
			return directory == "" || !trailingSlash ? directory : (directory + @"/");
		}
		public string GetRelativePath()
		{
			return Project.GetRelativePath(File.FullName);
		}

	}
	public enum FileType
	{
		Unknown, Image, Source, Text, Include,
		Script, FamiTracker, Audio, TileMap
	}
	public enum CompileMode
	{
		Ignore = 0,
		IncludeInAssembly = 1,
		ContentPipeline = 2,
		LinkerConfig = 3
	}
}