using Brewmaster.Settings;
using System;
using System.Collections.Generic;
using Brewmaster.Modules.Watch;

namespace Brewmaster.ProjectModel
{
	public class Breakpoint
	{
		private bool _disabled;

		[Flags]
		public enum Types
		{
			Execute = 1,
			Read = 2,
			Write = 4,
			Marked = 8
		}

		public event Action EnabledChanged;
		public enum AddressTypes
		{
			PrgRom = 1, ChrRom = 2, Cpu = 3, Ppu = 4, Apu = 5, Oam = 6, SpcRam = 7
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
			if (symbols != null && symbols.ContainsKey(Symbol))
			{
				StartAddress = symbols[Symbol].Value;
				Broken = false;
			}
			else Broken = true;
		}

		public string GetAddressDescription()
		{
			return string.Format("{0}{1} ({2})",
				WatchValue.FormatHexAddress(StartAddress),
				EndAddress == null ? "" : ("-" + WatchValue.FormatHexAddress(EndAddress.Value)),
				AddressType.ToString().ToUpper());
		}

		public override string ToString()
		{
			return File != null
				? string.Format("{0}:{1}", File.File.Name, CurrentLine)
				: Symbol != null
					? Symbol + (StartAddress >= 0 ? string.Format(" ({0})", WatchValue.FormatHexAddress(StartAddress)) : "")
					: GetAddressDescription();
		}
	}
}