using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.Modules.Build;
using Brewmaster.ProjectModel;
using Mesen.GUI;
using Mesen.GUI.Config;
using Mesen.GUI.Config.Shortcuts;
using SnesBreakpointTypeFlags = Mesen.GUI.Debugger.BreakpointTypeFlags;
using SnesBreakSource = Brewmaster.Emulation.BreakSource;
using SnesApi = Mesen.GUI.EmuApi;
using SnesDebugApi = Mesen.GUI.DebugApi;
using SnesConfigApi = Mesen.GUI.ConfigApi;
using SnesInputApi = Mesen.GUI.InputApi;
using SnesDebuggerFlags = Mesen.GUI.DebuggerFlags;
using SnesDebugState = Mesen.GUI.DebugState;
using SnesEmulationFlags = Brewmaster.Emulation.EmulationFlags;
using SnesBreakpoint = Mesen.GUI.Debugger.InteropBreakpoint;

//using NotificationHandler = Mesen.GUI.NotificationListener;

//using NotificationType = Brewmaster.Emulation.ConsoleNotificationType;


using EmuApi = Brewmaster.Emulation.EmuApi;
using SnesAspectRatio = Mesen.GUI.Config.VideoAspectRatio;

namespace Brewmaster.Emulation
{
	public class EmuApi
	{
		private const string DllPath = "Mesen\\MesenSCore.dll";
		[DllImport(DllPath)] public static extern void InitDll();
		[DllImport(DllPath, EntryPoint = "GetMesenVersion")] private static extern UInt32 GetMesenVersionWrapper();
		public static Version GetMesenVersion()
		{
			UInt32 version = GetMesenVersionWrapper();
			UInt32 revision = version & 0xFF;
			UInt32 minor = (version >> 8) & 0xFF;
			UInt32 major = (version >> 16) & 0xFFFF;
			return new Version((int)major, (int)minor, (int)revision, 0);
		}
		[DllImport(DllPath)] public static extern void SetDisplayLanguage(Brewmaster.Emulation.Language lang);
		[DllImport(DllPath)] public static extern IntPtr RegisterNotificationCallback(NotificationListener.NotificationCallback callback);
		[DllImport(DllPath)] public static extern void UnregisterNotificationCallback(IntPtr notificationListener);
	}

	public class SnesEmulatorHandler: IEmulatorHandler
	{
		private readonly Form _mainWindow;
		private Control _renderControl;
		private NotificationListener _notifListener;
		private Action<LogData> _logHandler;

		public int UpdateRate { get; set; }
		private int _updateCounter = 0;

		public event Action OnRun;
		public event Action<int> OnBreak;
		public event Action<EmulatorStatus> OnStatusChange;
		public event Action<MemoryState> OnMemoryUpdate;
		public event Action<RegisterState> OnRegisterUpdate;
		public event Action<NametableData> OnNametableUpdate;

		private Object emulatorLock = new Object();

		public SnesEmulatorHandler(Form mainWindow)
		{
			_mainWindow = mainWindow;
		}

		public void LoadCartridge(string baseDir, string cartridgeFile)
		{
			SnesApi.LoadRom(cartridgeFile, string.Empty);
			InitDebugger();
			EnableDebugger();
			RunGame();
		}

		public void LoadCartridgeAtSameState(string baseDir, string cartridgeFile, Func<int, int> getNewPc)
		{
			throw new NotImplementedException();
		}

		public void InitializeEmulator(string baseDir, Action<LogData> logHandler, Control renderControl)
		{
			_renderControl = renderControl;
			_logHandler = logHandler;
			StartEmulatorProcess(baseDir);
		}

		public void InitDebugger()
		{
			//var debuggerAlreadyRunning = SnesDebugApi.DebugIsDebuggerRunning();
			SnesDebugApi.InitializeDebugger();
		}
		public void EnableDebugger()
		{
			SnesConfigApi.SetDebuggerFlag(SnesDebuggerFlags.CpuDebuggerEnabled, true);
			//SnesApi.DebugRun();
			RefreshBreakpoints();
		}

		private bool _isRunning = false;
		public void RunGame()
		{
			_isRunning = true;
			var task = new Thread(() => {
				SnesApi.Run();
			}, 30000000);
			task.Start();
		}
		private void StartEmulatorProcess(string baseDir)
		{
			var test = SnesApi.TestDll();
			SnesApi.InitDll();
			//EmuApi.InitDll();
			var version = SnesApi.GetMesenVersion();
			//InteropEmu.SetDisplayLanguage(Brewmaster.Emulation.Language.English);
			//SnesApi.SetDisplayLanguage(Mesen.GUI.Forms.Language.English);


			ApplyVideoConfig();
			SnesApi.InitializeEmu(baseDir, _mainWindow.Handle, _renderControl.Handle, false, false, false);

			ApplyVideoConfig();
			ApplyAudioConfig();
			ApplyInputConfig();
			ApplyEmulationConfig();
			ApplyPreferenceConfig();
			ApplyDebuggerConfig();

			Mesen.GUI.ScreenSize size = SnesApi.GetScreenSize(false);
			_renderControl.Size = new Size(size.Width, size.Height);




			//SnesApi.AddKnownGameFolder(@"C:\Users\dkmrs\Documents\NesDev\sc");

			_notifListener = new NotificationListener();
			_notifListener.OnNotification += HandleNotification;
			//SnesApi.SetNesModel(NesModel.Auto);
			//SnesApi.DebugSetDebuggerConsole(SnesApi.ConsoleId.Master);

		}

		public void SetScale(double scale)
		{
			VideoConfig.VideoScale = scale;
			ApplyVideoConfig();
			ScreenSize size = SnesApi.GetScreenSize(false);
			_renderControl.Size = new Size(size.Width, size.Height);
		}

		public void SetSpeed(int speed)
		{
			EmulationConfig.EmulationSpeed = (uint)speed;
			ApplyEmulationConfig();
		}

		public void ForceNewState(RegisterState state)
		{
			// TODO when Mesen-S supports it
		}

		public void SaveState(string file)
		{
			SnesApi.SaveStateFile(file);
		}

		public void LoadState(string file)
		{
			SnesApi.LoadStateFile(file);
		}

		public void UpdateSettings(MesenControl.EmulatorSettings settings)
		{
			EmulationConfig.RamPowerOnState = settings.RandomPowerOnState ? RamState.Random : RamState.AllZeros;
			EmulationConfig.EnableMapperRandomPowerOnState = settings.RandomPowerOnState;
			VideoConfig.HideBgLayer0 = !settings.ShowBgLayer1;
			VideoConfig.HideBgLayer1 = !settings.ShowBgLayer2;
			VideoConfig.HideBgLayer2 = !settings.ShowBgLayer3;
			VideoConfig.HideBgLayer3 = !settings.ShowBgLayer4;
			VideoConfig.HideSprites = !settings.ShowSpriteLayer;

			SnesConfigApi.SetAudioConfig(new AudioConfig { EnableAudio = settings.PlayAudio });
			ApplyEmulationConfig();
			ApplyVideoConfig();
		}

		private void HandleNotification(NotificationEventArgs e)
		{
			switch (e.NotificationType)
			{
				case ConsoleNotificationType.GameLoaded:
					if (!SnesApi.IsPaused())
					{
						RefreshBreakpoints();
					}
					if (OnRun != null) OnRun();
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
					EmitDebugData();
					break;

				case ConsoleNotificationType.CodeBreak:
					var source = (SnesBreakSource)(byte)e.Parameter.ToInt64();
					//if (source == SnesBreakSource.Breakpoint && OnBreak != null)
					if (OnBreak != null)
					{
						var state = SnesDebugApi.GetState();
						var address = SnesDebugApi.GetAbsoluteAddress(new AddressInfo
						{
							Address = (state.Cpu.K << 16) | state.Cpu.PC,
							Type = SnesMemoryType.CpuMemory
						});
						OnBreak(address.Address);
					}
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Paused);
					EmitDebugData();
					break;
				case ConsoleNotificationType.PpuFrameDone:
					if (UpdateRate == 0) break;
					_updateCounter++;
					if (_updateCounter < UpdateRate) break;
					EmitDebugData();
					_updateCounter = 0;
					break;
			}

			if (e.NotificationType == ConsoleNotificationType.PpuFrameDone) return;
			if (e.NotificationType == ConsoleNotificationType.EventViewerRefresh) return;

			var status = string.Format("Emulator: {0}", e.NotificationType.ToString());
			_logHandler(new LogData(status, LogType.Normal));
		}

		private MemoryState _memoryState = new MemoryState(null, null, null);
		private void EmitDebugData()
		{
			//lock (emulatorLock)
			{
				if (OnMemoryUpdate != null)
				{
					_memoryState.CpuData = SnesDebugApi.GetMemoryState(SnesMemoryType.CpuMemory);
					_memoryState.PpuData = SnesDebugApi.GetMemoryState(SnesMemoryType.VideoRam);
					_memoryState.OamData = SnesDebugApi.GetMemoryState(SnesMemoryType.SpriteRam);
					OnMemoryUpdate(_memoryState);
				}
				if (OnRegisterUpdate != null)
				{
					var state = new RegisterState(ProjectType.Snes);
					state.SnesState = SnesDebugApi.GetState();
					OnRegisterUpdate(state);

				}
				if (OnNametableUpdate != null)
				{
					//var nametableData = new NametableData();
					//SnesDebugApi.GetScroll(out nametableData.ScrollX, out nametableData.ScrollY);
					//for (int i = 0; i < 4; i++)
					//{
					//	SnesApi.DebugGetNametable(i, false, out nametableData.PixelData[i], out nametableData.TileData[i], out nametableData.AttributeData[i]);
					//}
					//OnNametableUpdate(nametableData);
				}
			}
		}

		public void SetCpuMemory(int offset, byte value)
		{
			SnesDebugApi.SetMemoryValue(SnesMemoryType.CpuMemory, (uint)offset, value);
		}
		public void SetPpuMemory(int offset, byte value)
		{
			SnesDebugApi.SetMemoryValue(SnesMemoryType.VideoRam, (uint)offset, value);
		}
		public void SetOamMemory(int offset, byte value)
		{
			SnesDebugApi.SetMemoryValue(SnesMemoryType.SpriteRam, (uint)offset, value);
		}

		public void Dispose()
		{
			if (IsRunning())
			{
				SnesApi.Pause();
				Stop();
			}
		}

		static private double ConvertVolume(UInt32 volume)
		{
			if (true)
			{
				return ((double)volume / 100d);
			}
			else
			{
				return 0;
			}
		}
		static private double ConvertPanning(Int32 panning)
		{
			return (double)((panning + 100) / 100d);
		}

		public static void ApplyEmulationConfig()
		{
			ConfigApi.SetEmulationConfig(EmulationConfig);
		}

		public static void ApplyDebuggerConfig()
		{
			ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnBrk, true);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnCop, BreakOnCop);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnWdm, BreakOnWdm);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnStp, BreakOnStp);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnUninitRead, BreakOnUninitRead);

			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.ShowUnidentifiedData, UnidentifiedBlockDisplay == CodeDisplayMode.Show);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.DisassembleUnidentifiedData, UnidentifiedBlockDisplay == CodeDisplayMode.Disassemble);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.ShowVerifiedData, VerifiedDataDisplay == CodeDisplayMode.Show);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.DisassembleVerifiedData, VerifiedDataDisplay == CodeDisplayMode.Disassemble);
		}
		public static void ApplyAudioConfig()
		{
			ConfigApi.SetAudioConfig(new AudioConfig());
		}

		public static void ApplyPreferenceConfig()
		{
			ShortcutKeyInfo[] shortcutKeys = new ShortcutKeyInfo[0];
			ConfigApi.SetShortcutKeys(shortcutKeys, (UInt32)shortcutKeys.Length);
			ConfigApi.SetPreferences(new InteropPreferencesConfig()
			{
				ShowFps = false,
				ShowFrameCounter = false,
				ShowGameTimer = false,
				ShowDebugInfo = false,
				DisableOsd = true,
				AllowBackgroundInput = false,
				SaveFolderOverride = Path.Combine(Program.EmulatorDirectory, "Saves"),
				SaveStateFolderOverride = Path.Combine(Program.EmulatorDirectory, "SaveStates"),
				ScreenshotFolderOverride = Path.Combine(Program.EmulatorDirectory, "Screenshots"),
				//RewindBufferSize = RewindBufferSize
			});

		}

		private static void ApplyVideoConfig()
		{
			ConfigApi.SetVideoConfig(VideoConfig);
		}

		private void ApplyInputConfig()
		{
			UpdateControllerMappings(new Dictionary<int, int>());
		}
		public void UpdateControllerMappings(Dictionary<int, int> mappings)
		{
			var p1Mapping = new KeyMapping
			{
				Up = GetMapping(mappings, 328),
				Down = GetMapping(mappings, 336),
				Left = GetMapping(mappings, 331),
				Right = GetMapping(mappings, 333),
				X = GetMapping(mappings, 45),
				Y = GetMapping(mappings, 44),
				A = GetMapping(mappings, 31),
				B = GetMapping(mappings, 30),
				Select = GetMapping(mappings, 18),
				Start = GetMapping(mappings, 32),
				L = GetMapping(mappings, 16),
				R = GetMapping(mappings, 17)
			};
			var config = new InputConfig();
			config.Controllers[0].Type = ControllerType.SnesController;
			config.Controllers[0].Keys.Mapping1 = p1Mapping;
			ConfigApi.SetInputConfig(config);
		}
		private static uint GetMapping(Dictionary<int, int> mappings, int defaultKey)
		{
			return mappings.ContainsKey(defaultKey) ? (uint)mappings[defaultKey] : (uint)defaultKey;
		}

		private List<SnesBreakpoint> _breakpoints = new List<SnesBreakpoint>();

		private static readonly EmulationConfig EmulationConfig = new EmulationConfig
		{
			RamPowerOnState = RamState.Random,
			AllowInvalidInput = false
		};
		private static readonly VideoConfig VideoConfig = new VideoConfig
		{
			AspectRatio = SnesAspectRatio.NoStretching,
			VideoScale = 1
		};

		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			lock (_breakpoints)
			{
				_breakpoints = new List<SnesBreakpoint>();
				var id = 0;
				byte[] condition = Encoding.UTF8.GetBytes("");

				foreach (var breakpoint in breakpoints)
				{
					var emuBreakpoint = new SnesBreakpoint
					{
						Id = id,
						Enabled = true,
						StartAddress = breakpoint.StartAddress,
						EndAddress = breakpoint.EndAddress == null ? -1 : breakpoint.EndAddress.Value,
						MarkEvent = false,
						Condition = new byte[1000],
						CpuType = CpuType.Cpu
					};
					Array.Copy(condition, emuBreakpoint.Condition, condition.Length);
					if ((breakpoint.Type & Breakpoint.Types.Execute) != 0) emuBreakpoint.Type |= SnesBreakpointTypeFlags.Execute;
					if ((breakpoint.Type & Breakpoint.Types.Read) != 0) emuBreakpoint.Type |= SnesBreakpointTypeFlags.Read;
					if ((breakpoint.Type & Breakpoint.Types.Write) != 0) emuBreakpoint.Type |= SnesBreakpointTypeFlags.Write;

					if (breakpoint.AddressType == Breakpoint.AddressTypes.PrgRom) emuBreakpoint.MemoryType = SnesMemoryType.PrgRom;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Cpu) emuBreakpoint.MemoryType = SnesMemoryType.CpuMemory;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Ppu) emuBreakpoint.MemoryType = SnesMemoryType.VideoRam;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Apu) emuBreakpoint.MemoryType = SnesMemoryType.SpcMemory;

					_breakpoints.Add(emuBreakpoint);

					id++;
				}
			}
			if (IsRunning()) RefreshBreakpoints();
		}
		private void RefreshBreakpoints()
		{
			//lock (emulatorLock)
			{
				SnesDebugApi.SetBreakpoints(_breakpoints.ToArray(), (UInt32)_breakpoints.Count);
			}
		}

		public bool IsRunning()
		{
			return _isRunning;
			//return false && !SnesApi.IsPaused();
		}

		public void Resume()
		{
			if (!IsRunning()) return;
			SnesApi.Resume();
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
		}

		public void Pause()
		{
			if (!IsRunning()) return;
			SnesDebugApi.Step(CpuType.Cpu, 1);
		}

		public void Stop()
		{
			if (!IsRunning()) return;
			Task.Run(() => {
				try
				{
					SnesApi.Stop();
				}
				catch (Exception ex)
				{
					Program.Error("An error occurred while trying to stop the SNES emulator!", ex);
				}
			});
			_isRunning = false;
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Stopped);
		}

		public void Restart()
		{
			if (!IsRunning()) return;
			SnesApi.Reset();
			//SnesApi.PowerCycle(); TODO: Would love to make powercycle possible, but causes a weird crash if you stop emulation after a powercycle before running the emulator for a while
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
		}

		public void SetKeyState(int scanCode, bool state)
		{
			SnesInputApi.SetKeyState(scanCode, state);
		}

		public void StepOver()
		{
			SnesDebugApi.Step(CpuType.Cpu, 1, StepType.StepOver);
		}

		public void StepInto()
		{
			SnesDebugApi.Step(CpuType.Cpu, 1, StepType.Step);
		}

		public void StepOut()
		{
			SnesDebugApi.Step(CpuType.Cpu, 1, StepType.StepOut);
		}

		public void StepBack()
		{
			//SnesDebugApi.Step(CpuType.Cpu, 1, StepType.StepBack);
		}
	}
}
