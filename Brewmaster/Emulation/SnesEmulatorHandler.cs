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
using Brewmaster.Modules.SpriteList;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;
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
using SnesBreakpoint = Mesen.GUI.Debugger.InteropBreakpoint;

//using NotificationHandler = Mesen.GUI.NotificationListener;

//using NotificationType = Brewmaster.Emulation.ConsoleNotificationType;
using SnesAspectRatio = Mesen.GUI.Config.VideoAspectRatio;

namespace Brewmaster.Emulation
{
	public class EmuApi
	{
		private const string DllPath = "lib\\MesenSCore.dll";
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

	public class SnesEmulatorHandler: EmulatorHandler, IEmulatorHandler
	{
		private readonly Form _mainWindow;
		private Control _renderControl;
		private NotificationListener _notifListener;
		private Action<LogData> _logHandler;

		public event Action OnRun;
		public event Action<BreakInfo> OnBreak;
		public event Action<EmulatorStatus> OnStatusChange;
		public event Action<EmulationState> OnRegisterUpdate;

		private Object emulatorLock = new Object();

		public SnesEmulatorHandler(Form mainWindow)
		{
			_mainWindow = mainWindow;
			_state.TileMaps.PixelData[0] = new byte[1024 * 1024 * 4];
			_state.TileMaps.OnRefreshRequest += () => PushTileMapData();
			_state.CharacterData.OnRefreshRequest += () => PushCharacterData();
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
			return; // TODO: Feature unusable until Mesen-S gets "Debugger.SetNextStatement()"

			if (!IsRunning()) return;
			SnesDebugApi.Step(CpuType.Cpu, 0);
			var state = SnesDebugApi.GetState();

			var prgSize = SnesDebugApi.GetMemorySize(SnesMemoryType.PrgRom);
			using (var file = File.OpenRead(cartridgeFile))
			{
				var buffer = new byte[prgSize];
				file.Read(buffer, 0, prgSize);
				SnesDebugApi.SetMemoryValues(SnesMemoryType.PrgRom, 0, buffer, prgSize);
			}
			//SnesDebugApi.SetNextStatement((ushort)getNewPc(state.Cpu.PC));
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
			//SnesConfigApi.SetDebuggerFlag(SnesDebuggerFlags.CpuDebuggerEnabled, true);
			//SnesConfigApi.SetDebuggerFlag(SnesDebuggerFlags.SpcDebuggerEnabled, true);
			//SnesApi.DebugRun();
			RefreshBreakpoints();
		}

		private bool _isRunning = false;
		public void RunGame()
		{
			_isRunning = true;
			var task = new Thread(() => {
				//SnesApi.;
			}, 30000000);
			task.Start();
			ParseBreakpoints();
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

		public void ForceNewState(EmulationState state)
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

		public void UpdateSettings(EmulatorSettings settings)
		{
			EmulationConfig.RamPowerOnState = settings.RandomPowerOnState ? RamState.Random : RamState.AllZeros;
			EmulationConfig.EnableRandomPowerOnState = settings.RandomPowerOnState;
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
					GameLoaded();
					if (OnRun != null) OnRun();
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
					EmitDebugData();
					break;

				case ConsoleNotificationType.CodeBreak:
					var source = (BreakEvent)Marshal.PtrToStructure(e.Parameter, typeof(BreakEvent));
					//if (source == SnesBreakSource.Breakpoint && OnBreak != null)
					if (OnBreak != null)
					{
						var state = SnesDebugApi.GetState();
						var cpuAddress = SnesDebugApi.GetAbsoluteAddress(new AddressInfo
						{
							Address = (state.Cpu.K << 16) | state.Cpu.PC,
							Type = SnesMemoryType.CpuMemory
						});
						var spcAddress = SnesDebugApi.GetAbsoluteAddress(new AddressInfo
						{
							Address = state.Spc.PC,
							Type = SnesMemoryType.SpcMemory
						});

						OnBreak(new BreakInfo { CpuAddress = cpuAddress.Address, SpcAddress = spcAddress.Address });
					}
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Paused);
					EmitDebugData();
					break;
				case ConsoleNotificationType.EventViewerRefresh:
					var options = new Mesen.GUI.EventViewerDisplayOptions
					{
						ShowMarkedBreakpoints = true,
						BreakpointColor = 0,
						ShowPpuRegisterVramWrites = true,
						PpuRegisterWriteVramColor = 9
					};
					var test = SnesDebugApi.GetDebugEvents(options);

					break;
				case ConsoleNotificationType.PpuFrameDone:
					CountFrame();
					break;
			}

			if (e.NotificationType == ConsoleNotificationType.PpuFrameDone) return;
			if (e.NotificationType == ConsoleNotificationType.EventViewerRefresh) return;

			var status = string.Format("Emulator: {0}", e.NotificationType.ToString());
			_logHandler(new LogData(status, LogType.Normal));
		}

		private readonly EmulationState _state = new EmulationState(TargetPlatform.Snes)
		{
			TileMaps = new TileMapData { NumberOfMaps = 1, DataWidth = 1024 * 4, NumberOfPages = 4 },
			CharacterData = new CharacterData { PixelData = new [] { new byte[0x400000] } }
		};
		private GetTilemapOptions _tilemapOptions = new GetTilemapOptions();
		private readonly GetSpritePreviewOptions _spriteOptions = new GetSpritePreviewOptions();
		private GetTileViewOptions _tileViewOptions = new GetTileViewOptions();
		private readonly byte[] _tileSource = new byte[0x40000];
		
		protected override void EmitDebugData()
		{
			if (OnRegisterUpdate == null) return;

			_state.SnesState = SnesDebugApi.GetState();
			_state.Memory.PpuData = SnesDebugApi.GetMemoryState(SnesMemoryType.VideoRam);
			_state.Memory.OamData = SnesDebugApi.GetMemoryState(SnesMemoryType.SpriteRam);
			_state.Memory.CgRam = SnesDebugApi.GetMemoryState(SnesMemoryType.CGRam);
			_state.Memory.CpuData = SnesDebugApi.GetMemoryState(SnesMemoryType.CpuMemory);
			_state.Memory.SpcData = SnesDebugApi.GetMemoryState(SnesMemoryType.SpcMemory);
			_state.Memory.X = _state.SnesState.Cpu.X & (_state.SnesState.Cpu.PS.HasFlag(ProcFlags.IndexMode8) ? 0xFF : 0xFFFF);
			_state.Memory.Y = _state.SnesState.Cpu.Y & (_state.SnesState.Cpu.PS.HasFlag(ProcFlags.IndexMode8) ? 0xFF : 0xFFFF);
			SnesDebugApi.GetSpritePreview(_spriteOptions, _state.SnesState.Ppu, _state.Memory.PpuData, _state.Memory.OamData, _state.Memory.CgRam, _state.Sprites.PixelData);
			_state.Sprites.Details = Sprite.GetSnesSprites(_state.Memory.OamData, _state.SnesState.Ppu.OamMode);

			GetCharacterData();
			GetTileMapData();

			OnRegisterUpdate(_state);
		}

		private void PushCharacterData()
		{
			GetCharacterData();
			if (OnRegisterUpdate != null) OnRegisterUpdate(_state);
		}
		private void GetCharacterData()
		{
			_tileViewOptions.Format = (TileFormat)_state.CharacterData.ColorMode;
			_tileViewOptions.Width = 16;
			_tileViewOptions.PageSize = 0x10000;
			var source = SnesDebugApi.GetMemoryState(SnesMemoryType.VideoRam);
			var address = 0;
			var size = Math.Min(source.Length - address, _tileViewOptions.PageSize);
			Array.Copy(source, address, _tileSource, 0, size);
			_state.CharacterData.Width = 128;
			_state.CharacterData.Height = GetChrHeight(_tileViewOptions, _state.CharacterData.ColorMode);
			SnesDebugApi.GetTileView(_tileViewOptions, _tileSource, _tileSource.Length, _state.Memory.CgRam, _state.CharacterData.PixelData[0]);
		}

		private static int GetChrHeight(GetTileViewOptions options, int colorMode)
		{
			var bitDepth = colorMode == 0 ? 2 : colorMode == 1 ? 4 : 8;
			if (colorMode >= 4) bitDepth *= 2;
			return 8 * options.PageSize / (8 * options.Width * bitDepth);
		}

		private void PushTileMapData()
		{
			_state.Memory.CgRam = SnesDebugApi.GetMemoryState(SnesMemoryType.CGRam);
			_state.Memory.PpuData = SnesDebugApi.GetMemoryState(SnesMemoryType.VideoRam);
			_state.SnesState = _state.SnesState = SnesDebugApi.GetState();

			GetTileMapData();
			if (OnRegisterUpdate != null) OnRegisterUpdate(_state);
		}

		private void GetTileMapData()
		{
			_tilemapOptions.Layer = (byte)_state.TileMaps.GetPage;
			_state.TileMaps.MapWidth = GetMapWidth();
			_state.TileMaps.MapHeight = GetMapHeight();
			SnesDebugApi.GetTilemap(_tilemapOptions, _state.SnesState.Ppu, _state.Memory.PpuData, _state.Memory.CgRam, _state.TileMaps.PixelData[0]);

			_state.TileMaps.ViewportHeight = _state.SnesState.Ppu.OverscanMode ? 239 : 224;
			_state.TileMaps.ScrollX = (_state.SnesState.Ppu.BgMode == 7 ? (int)_state.SnesState.Ppu.Mode7.HScroll : _state.SnesState.Ppu.Layers[_tilemapOptions.Layer].HScroll) % _state.TileMaps.MapWidth;
			_state.TileMaps.ScrollY = (_state.SnesState.Ppu.BgMode == 7 ? (int)_state.SnesState.Ppu.Mode7.VScroll : _state.SnesState.Ppu.Layers[_tilemapOptions.Layer].VScroll) % _state.TileMaps.MapHeight;
		}

		private int GetMapWidth()
		{
			if (_state.SnesState.Ppu.BgMode == 7) return 1024;

			var layer = _state.SnesState.Ppu.Layers[_tilemapOptions.Layer];
			var largeTileWidth = layer.LargeTiles || _state.SnesState.Ppu.BgMode == 5 || _state.SnesState.Ppu.BgMode == 6;

			var width = 256;
			if (layer.DoubleWidth) width *= 2;
			if (largeTileWidth) width *= 2;
			
			return width;
		}

		private int GetMapHeight()
		{
			if (_state.SnesState.Ppu.BgMode == 7) return 1024;

			var layer = _state.SnesState.Ppu.Layers[_tilemapOptions.Layer];

			var height = 256;
			if (layer.DoubleHeight) height *= 2;
			if (layer.LargeTiles) height *= 2;
			return height;
		}

		public void SetCpuMemory(int offset, byte value)
		{
			// TODO: Is emulator running?
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
				Stop(true);
			}
		}

		public static void ApplyEmulationConfig()
		{
			SnesConfigApi.SetEmulationConfig(EmulationConfig);
		}

		public static void ApplyDebuggerConfig()
		{
			SnesConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnBrk, true);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnCop, BreakOnCop);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnWdm, BreakOnWdm);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnStp, BreakOnStp);
			//ConfigApi.SetDebuggerFlag(SnesDebuggerFlags.BreakOnUninitRead, BreakOnUninitRead);
		}
		public static void ApplyAudioConfig()
		{
			SnesConfigApi.SetAudioConfig(new AudioConfig());
		}

		public static void ApplyPreferenceConfig()
		{
			var shortcutKeys = new ShortcutKeyInfo[0];
			SnesConfigApi.SetShortcutKeys(shortcutKeys, (UInt32)shortcutKeys.Length);
			SnesConfigApi.SetPreferences(new InteropPreferencesConfig()
			{
				DisableGameSelectionScreen = true,
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
			SnesConfigApi.SetVideoConfig(VideoConfig);
		}

		private void ApplyInputConfig()
		{
			UpdateControllerMappings(new Dictionary<ControllerButtons, int>());
		}
		public void UpdateControllerMappings(Dictionary<ControllerButtons, int> mappings)
		{
			var p1Mapping = new KeyMapping
			{
				Up = GetMapping(mappings, ControllerButtons.Up),
				Down = GetMapping(mappings, ControllerButtons.Down),
				Left = GetMapping(mappings, ControllerButtons.Left),
				Right = GetMapping(mappings, ControllerButtons.Right),
				X = GetMapping(mappings, ControllerButtons.X),
				Y = GetMapping(mappings, ControllerButtons.Y),
				A = GetMapping(mappings, ControllerButtons.A),
				B = GetMapping(mappings, ControllerButtons.B),
				Select = GetMapping(mappings, ControllerButtons.Select),
				Start = GetMapping(mappings, ControllerButtons.Start),
				L = GetMapping(mappings, ControllerButtons.L),
				R = GetMapping(mappings, ControllerButtons.R)
			};
			var config = new InputConfig();
			config.Controllers[0].Keys.TurboSpeed = 2;
			config.Controllers[0].Type = ControllerType.SnesController;
			config.Controllers[0].Keys.Mapping1 = p1Mapping;
			SnesConfigApi.SetInputConfig(config);
		}
		private static uint GetMapping(Dictionary<ControllerButtons, int> mappings, ControllerButtons defaultKey)
		{
			return mappings.ContainsKey(defaultKey) ? (uint)mappings[defaultKey] : (uint)defaultKey;
		}

		private List<SnesBreakpoint> _breakpoints = new List<SnesBreakpoint>();
		private IEnumerable<Breakpoint> _ideBreakpoints;
		private static readonly EmulationConfig EmulationConfig = new EmulationConfig { RamPowerOnState = RamState.Random };
		private static readonly VideoConfig VideoConfig = new VideoConfig
		{
			AspectRatio = SnesAspectRatio.NoStretching,
			VideoScale = 1
		};
		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			_ideBreakpoints = breakpoints;
			ParseBreakpoints();
		}

		private void ParseBreakpoints()
		{
			if (_ideBreakpoints == null) return;
			lock (_breakpoints)
			{
				_breakpoints = new List<SnesBreakpoint>();
				var id = 0;
				byte[] condition = Encoding.UTF8.GetBytes("");

				foreach (var breakpoint in _ideBreakpoints)
				{
					var emuBreakpoint = new SnesBreakpoint
					{
						Id = id,
						Enabled = (breakpoint.Type & Breakpoint.Types.Marked) == 0,
						StartAddress = breakpoint.StartAddress,
						EndAddress = breakpoint.EndAddress == null ? -1 : breakpoint.EndAddress.Value,
						MarkEvent = (breakpoint.Type & Breakpoint.Types.Marked) != 0,
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
					if (breakpoint.AddressType == Breakpoint.AddressTypes.SpcRam)
					{
						emuBreakpoint.MemoryType = SnesMemoryType.SpcRam;
						emuBreakpoint.CpuType = CpuType.Spc;
					}

					// Map mirrored CPU addresses to their actual address
					if (IsRunning() && breakpoint.AddressType == Breakpoint.AddressTypes.Cpu && breakpoint.StartAddress < 0x1000000)
					{
						var realAddress = SnesDebugApi.GetAbsoluteAddress(new AddressInfo { Address = breakpoint.StartAddress, Type = SnesMemoryType.CpuMemory });
						emuBreakpoint.MemoryType = realAddress.Type;
						emuBreakpoint.StartAddress = realAddress.Address;

						if (breakpoint.EndAddress != null && breakpoint.EndAddress < 0x1000000)
						{
							realAddress = SnesDebugApi.GetAbsoluteAddress(new AddressInfo { Address = breakpoint.EndAddress.Value, Type = SnesMemoryType.CpuMemory });
							emuBreakpoint.EndAddress = realAddress.Address;
						}
					}

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

		public override bool IsRunning()
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

		public async Task Stop(bool releaseResources = false)
		{
			if (!IsRunning() && !releaseResources) return;
			var task = Task.Run(() => {
				try
				{
					SnesApi.Stop();
					if (releaseResources) SnesApi.Release();
				}
				catch (Exception ex)
				{
					Program.Error("An error occurred while trying to stop the SNES emulator!", ex);
				}
			});
			_isRunning = false;
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Stopped);

			await task;
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
