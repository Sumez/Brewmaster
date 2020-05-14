using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Brewmaster.ProjectModel
{
	[Serializable]
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
		[XmlElement(ElementName = "Symbol")]
		public List<string> Symbols { get; set; }
		public bool CalculateChecksum { get; set; }
		public bool Custom { get; set; }
		[XmlElement(ElementName = "Script")]
		public List<string> ScriptCommands { get; set; }

		public NesCartridge()
		{
			ChrBanks = new List<ChrBank>();
			Symbols = new List<string>();
		}
		[XmlIgnore]
		public List<ChrBank> ChrBanks { get; set; }
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