using System.Collections.Generic;

namespace Brewmaster.ProjectModel
{
	public class DebugInfo
	{
		public Dictionary<int, DebugFile> Files { get; private set; }
		public Dictionary<int, DebugLine> Lines { get; private set; }
		public Dictionary<int, DebugLine> LineIds { get; private set; }
		public Dictionary<int, DebugSegment> Segments { get; private set; }
		public Dictionary<string, DebugSymbol> Symbols { get; private set; }

		public DebugInfo()
		{
			Files = new Dictionary<int, DebugFile>();
			Lines = new Dictionary<int, DebugLine>();
			LineIds = new Dictionary<int, DebugLine>();
			Segments = new Dictionary<int, DebugSegment>();
			Symbols = new Dictionary<string, DebugSymbol>();
		}
	}


	public struct DebugSegment
	{
		public int? RomOffset;
		public int? CpuAddress;
	}

	public class DebugFile
	{
		public string Name;
		public Dictionary<int, DebugLine> Lines;

		public DebugFile()
		{
			Lines = new Dictionary<int, DebugLine>();
		}
	}

	public class DebugLine
	{
		public DebugFile File;
		public int Line;
		public int? RomAddress;
		public int Size;
		public int? CpuAddress;
	}
	public class DebugSymbol
	{
		public int Value;
		public int Size;
		public int DebugLine;
		public string Source;
	}
}
