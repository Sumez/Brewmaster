using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brewmaster.EditorWindows;
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
		public static readonly Regex ImportFileRegex = new Regex(@"^\s*\.include\s+\""(.+?)\""\s*$", RegexOptions.IgnoreCase);

		private static readonly string[] ImageFileTypes = new[] { ".png", ".jpg", ".bmp", ".gif" };
		private static readonly string[] SourceFileTypes = new[] { ".asm", ".s" };
		private static readonly string[] IncludeFileTypes = new[] { ".h", ".inc", ".include", ".i" };
		private static readonly string[] TextFileTypes = new[] { ".txt", ".md", ".ini", ".cfg", ".py", ".js" };

		private FileType _type = FileType.Unknown;
		public FileType Type { get
			{
				if (_type != FileType.Unknown) return FileType.Unknown;
				var extension = File.Extension.ToLower();
				if (Mode == CompileMode.IncludeInAssembly) return FileType.Source;
				if (Mode == CompileMode.LinkerConfig) return FileType.Text;
				if (ImageFileTypes.Contains(extension)) return FileType.Image;
				if (SourceFileTypes.Contains(extension)) return FileType.Source;
				if (IncludeFileTypes.Contains(extension)) return FileType.Include;
				if (TextFileTypes.Contains(extension)) return FileType.Text;
				return FileType.Unknown;
			}
			set { _type = value; }
		}

		public override string ToString()
		{
			return GetRelativePath();
		}

		// TODO: Is a "same dir" include path in an included file relative to the original file, or the included file
		private static void GetSymbolsFromText(string source, List<Symbol> newSymbols, List<string> exportedSymbols, string[] includePaths, List<string> files = null)
		{
			if (files == null) files = new List<string>();
			if (!System.IO.File.Exists(source)) return;

			var lineNumber = 0;
			foreach (var line in System.IO.File.ReadLines(source))
			{
				lineNumber++;

				var importMatch = ImportFileRegex.Match(line);
				if (importMatch.Success && includePaths.Length > 0)
				{
					var matchedFilename = importMatch.Groups[1].Value;
					var childFilename = matchedFilename;
					foreach (var path in includePaths)
					{
						childFilename = Path.Combine(path, matchedFilename);
						if (System.IO.File.Exists(childFilename)) break;
					}
					if (!System.IO.File.Exists(childFilename)) continue;

					childFilename = new FileInfo(childFilename).FullName;
					if (files.Contains(childFilename)) continue;
					files.Add(childFilename);

					GetSymbolsFromText(childFilename, newSymbols, exportedSymbols, includePaths, files);
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
			}

		}

		public event Action ParsedLocalSymbols;
		public List<Symbol> LocalSymbols = new List<Symbol>();
		public void AddSymbolsFromFile(List<KeyValuePair<string, Symbol>> symbols)
		{
			var exportedSymbols = new List<string>();
			LocalSymbols = new List<Symbol>();
			GetSymbolsFromText(File.FullName, LocalSymbols, exportedSymbols, new [] { File.DirectoryName, Project.Directory.FullName });
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

		public string GetRelativeDirectory()
		{
			return Project.GetRelativePath(File.DirectoryName);
		}
		public string GetRelativePath()
		{
			return Project.GetRelativePath(File.FullName);
		}

	}
	public enum FileType
	{
		Unknown, Image, Source, Text, Include
	}
	public enum CompileMode
	{
		Ignore = 0,
		IncludeInAssembly = 1,
		ContentPipeline = 2,
		LinkerConfig = 3
	}
}