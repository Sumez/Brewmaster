using System.Collections.Generic;

namespace Brewmaster.ProjectModel
{
	public class NesCartridge
	{
		public string Name { get; set; }
		public string Filename { get; set; }
		public string PrgFile { get; set; }
		public string ChrFile { get; set; }
		public string BuildPath { get; set; }
		public string PrgBuildPath { get; set; }
		public string DebugFile { get; set; }
		public string LinkerConfigFile { get; set; }
		public string ChrBuildPath { get; set; }
		public string MapFile { get; set; }
		public List<string> Symbols { get; set; }
		public List<ChrBank> ChrBanks { get; set; }
		
		public NesCartridge()
		{
			ChrBanks = new List<ChrBank>();
			Symbols = new List<string>();
		}

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(Name) ? Filename : Name;
		}
	}
	public class ChrBank
	{
		public List<AsmProjectFile> Sources { get; set; }

		public ChrBank()
		{
			Sources = new List<AsmProjectFile>();
		}
	}

}