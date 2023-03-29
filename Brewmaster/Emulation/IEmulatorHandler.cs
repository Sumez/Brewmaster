using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.Modules.Build;
using Brewmaster.Modules.SpriteList;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.Emulation
{
	public interface IEmulatorHandler : IDisposable
	{
		event Action<BreakInfo> OnBreak;
		event Action<EmulationState> OnRegisterUpdate;
		event Action OnRun;
		event Action<EmulatorStatus> OnStatusChange;
		event Action<int> OnFpsUpdate;

		int UpdateRate { set; }

		void EnableDebugger();
		void InitDebugger();
		void InitializeEmulator(string baseDir, Action<LogData> logHandler, Control renderControl);
		bool IsRunning();
		void LoadCartridge(string baseDir, string cartridgeFile);
		void LoadCartridgeAtSameState(string baseDir, string cartridgeFile, Func<int, int> getNewPc);
		void Pause();
		void SetKeyState(int scanCode, bool state);
		void Restart();
		void Resume();
		void RunGame();
		void SetBreakpoints(IEnumerable<Breakpoint> breakpoints);
		void SetCpuMemory(int offset, byte value);
		void SetOamMemory(int offset, byte value);
		void SetPpuMemory(int offset, byte value);
		void StepBack(MemoryState.CpuType cpuType);
		void StepInto(MemoryState.CpuType cpuType);
		void StepOut(MemoryState.CpuType cpuType);
		void StepOver(MemoryState.CpuType cpuType);
		Task Stop(bool releaseResources = false);
		void SetScale(double scale);
		void SetSpeed(int speed);
		void ForceNewState(EmulationState state);
		void SaveState(string file);
		void LoadState(string file);
		void UpdateSettings(EmulatorSettings emulatorSettings);
		void UpdateControllerMappings(Dictionary<ControllerButtons, int> mappings);
	}

	public class BreakInfo
	{
		public int CpuAddress;
		public int SpcAddress;
	}
	public class EmulationState
	{
		public EmulationState(TargetPlatform type)
		{
			Type = type;
		}
		public DebugState NesState;
		public Mesen.GUI.DebugState SnesState;
		public TargetPlatform Type { get; }

		public SpriteData Sprites = new SpriteData();
		public CharacterData CharacterData = new CharacterData();
		public TileMapData TileMaps;
		public MemoryState Memory = new MemoryState();
	}

	public class CharacterData
	{
		public byte[][] PixelData = new byte[2][];
		public int Width = 128;
		public int Height = 128;

		public event Action OnRefreshRequest;
		public int ColorMode { get; private set; } = 1;
		public void RequestRefresh(int colorMode)
		{
			ColorMode = colorMode;
			if (OnRefreshRequest != null) OnRefreshRequest();
		}
	}
	public class SpriteData
	{
		public byte[] PixelData = new byte[256 * 240 * 4];
		public List<Sprite> Details { get; set; }
	}

	public class TileMapData
	{
		public int ScrollX;
		public int ScrollY;
		public int NumberOfMaps;
		public byte[][] PixelData = new byte[4][];
		public byte[][] TileData = new byte[4][];
		public byte[][] AttributeData = new byte[4][];
		public int MapWidth;
		public int MapHeight;
		public int DataWidth;
		public int NumberOfPages = 1;
		public int GetPage { get; private set; }
		public int ViewportWidth = 256;
		public int ViewportHeight = 240;

		public event Action OnRefreshRequest;
		public void RequestRefresh(int page)
		{
			GetPage = page;
			if (OnRefreshRequest != null) OnRefreshRequest();
		}
	}

	public class MemoryState
	{
		public byte[] CpuData;
		public byte[] SpcData;
		public byte[] PpuData;
		public byte[] OamData;
		public byte[] CgRam;

		public int Y;
		public int X;

		public enum CpuType { Cpu, Spc }

		// TODO: Check size of symbols (24/16/8) and use knowledge of CPU bank registers (K/DP/DB) to look at correct address
		public int ReadAddress(CpuType cpuType, int index, bool readWord, OffsetRegister offset, out int address)
		{
			address = index;
			byte[] data;
			switch (cpuType)
			{
				case CpuType.Cpu:
					data = CpuData;
					break;
				case CpuType.Spc:
					data = SpcData;
					break;
				default:
					throw new Exception("MemoryState.ReadAddress(): Invalid CPU type");
			}

			if (offset == OffsetRegister.X) address += X;
			if (offset == OffsetRegister.Y) address += Y;
			if (offset == OffsetRegister.IndirectY) address = ReadAddress(cpuType, address, true) + Y; // TODO: Page boundary crossing issue
			if (offset == OffsetRegister.IndirectX) address = ReadAddress(cpuType, address + X, true);

			if (address >= data.Length || (readWord && address + 1 >= data.Length)) return -1;
			return data[address] + (readWord ? (data[address + 1] << 8) : 0);
		}

		public int ReadAddress(CpuType cpuType, int index, bool readWord = false, OffsetRegister offset = OffsetRegister.None)
		{
			return ReadAddress(cpuType, index, readWord, offset, out var dummy);
		}

	}
	public enum OffsetRegister { None, X, Y, IndirectX, IndirectY };

	public enum Language
	{
		SystemDefault = 0,
		English = 1,
		French = 2,
		Japanese = 3,
		Russian = 4,
		Spanish = 5,
		Ukrainian = 6,
		Portuguese = 7,
		Catalan = 8,
		Chinese = 9,
	}
}