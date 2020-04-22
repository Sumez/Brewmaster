﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrewMaster.Modules.Build;
using BrewMaster.ProjectModel;

namespace BrewMaster.Emulation
{
	public interface IEmulatorHandler : IDisposable
	{
		event Action<int> OnBreak;
		event Action<MemoryState> OnMemoryUpdate;
		event Action<NametableData> OnNametableUpdate;
		event Action<RegisterState> OnRegisterUpdate;
		event Action OnRun;
		event Action<EmulatorStatus> OnStatusChange;

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
		void StepBack();
		void StepInto();
		void StepOut();
		void StepOver();
		void Stop();
		void SetScale(double scale);
		void SetSpeed(int speed);
		void ForceNewState(RegisterState state);
		void SaveState(string file);
		void LoadState(string file);
		void UpdateSettings(MesenControl.EmulatorSettings emulatorSettings);
		void UpdateControllerMappings(Dictionary<int, int> mappings);
	}

	public class RegisterState
	{
		public RegisterState(ProjectType type)
		{
			Type = type;
		}
		public DebugState NesState;
		public Mesen.GUI.DebugState SnesState;
		public ProjectType Type { get; }
	}
	public class MemoryState
	{
		public MemoryState(byte[] cpuData, byte[] ppuData, byte[] oamData)
		{
			CpuData = cpuData;
			PpuData = ppuData;
			OamData = oamData;
		}

		public byte[] CpuData;
		public byte[] PpuData;
		public byte[] OamData;

		// TODO: Check size of symbols (24/16/8) and use knowledge of CPU bank registers (K/DP/DB) to look at correct address

		public int ReadAddress(int index, bool readWord = false)
		{
			if (index >= CpuData.Length || (readWord && index + 1 >= CpuData.Length)) return -1;
			return CpuData[index] + (readWord ? (CpuData[index + 1] << 8) : 0);

		}
	}
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