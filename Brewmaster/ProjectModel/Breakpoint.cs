using BrewMaster.Settings;
using System;
using System.Collections.Generic;

namespace BrewMaster.ProjectModel
{
	public class Breakpoint
	{
		private bool _disabled;

		[Flags]
		public enum Types
		{
			Execute = 1,
			Read = 2,
			Write = 4
		}

		public event Action EnabledChanged;
		public enum AddressTypes
		{
			PrgRom = 1, ChrRom = 2, Cpu = 3, Ppu = 4, Apu = 5, Oam = 6
		}
		public int StartAddress { get; set; }
		public int? EndAddress { get; set; }
		public Types Type { get; set; }
		public AddressTypes AddressType { get; set; }
		public bool Automatic { get; set; }
		public string Symbol { get; set; }
		public AsmProjectFile File { get; set; }
		public int CurrentLine { get; set; }
		public int BuildLine { get; set; }
		public bool Broken { get; set; }

		public bool Disabled
		{
			get { return _disabled; }
			set
			{
				_disabled = value;
				if (EnabledChanged != null) EnabledChanged();
			}
		}

		public BreakpointData GetSerializable()
		{
			return new BreakpointData
			{
				StartAddress = StartAddress,
				EndAddress = EndAddress,
				Type = (int)Type,
				AddressType = (int)AddressType,
				Automatic = Automatic,
				Symbol = Symbol,
				File = File == null ? null : File.GetRelativePath(),
				Line = CurrentLine,
				Disabled = Disabled
			};
		}

		public void UpdateFromSymbols(Dictionary<string, DebugSymbol> symbols)
		{
			if (symbols.ContainsKey(Symbol))
			{
				StartAddress = symbols[Symbol].Value;
				Broken = false;
			}
			else Broken = true;
		}
	}
}