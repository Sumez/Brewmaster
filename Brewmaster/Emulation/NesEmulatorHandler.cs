using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.Modules.Build;
using Brewmaster.Modules.SpriteList;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.Emulation
{
	public class NesEmulatorHandler: EmulatorHandler, IEmulatorHandler
	{
		private readonly Form _mainWindow;
		private Control _renderControl;
		private InteropEmu.NotificationListener _notifListener;
		private Action<LogData> _logHandler;

		public event Action OnRun;
		public event Action<int> OnBreak;
		public event Action<EmulatorStatus> OnStatusChange;
		public event Action<EmulationState> OnRegisterUpdate;

		public NesEmulatorHandler(Form mainWindow)
		{
			_mainWindow = mainWindow;
			_debugState.TileMaps = new TileMapData { NumberOfMaps = 4, MapWidth = 256, MapHeight = 240, DataWidth = 256 * 4 };
			_debugState.TileMaps.OnRefreshRequest += PushNametableData;
		}

		public void LoadCartridgeAtSameState(string baseDir, string cartridgeFile, Func<int, int> getNewPc)
		{
			if (!IsRunning()) return;
			var wasPaused = InteropEmu.DebugIsExecutionStopped();
			InteropEmu.DebugStep(0);
			var state = new DebugState();
			InteropEmu.DebugGetState(ref state);

			var prgSize = InteropEmu.DebugGetMemorySize(DebugMemoryType.PrgRom);
			using (var file = File.OpenRead(cartridgeFile))
			{
				var buffer = new byte[prgSize];
				file.Position = 16; // Header offset
				file.Read(buffer, 0, prgSize);
				InteropEmu.DebugSetMemoryValues(DebugMemoryType.PrgRom, 0, buffer);
			}
			//state.CPU.PC = (ushort)getNewPc(state.CPU.PC);
			//InteropEmu.DebugSetState(state);
			InteropEmu.DebugSetNextStatement((ushort)getNewPc(state.CPU.PC));
		}

		public void LoadCartridge(string baseDir, string cartridgeFile)
		{
			if (IsRunning())
			{
				InteropEmu.Pause();
				InteropEmu.Stop();
			}

			var testRom = InteropEmu.GetRomInfo(cartridgeFile);
			if (testRom.Crc32 == 0)
			{
				throw new Exception("Invalid NES ROM");
			}
			InteropEmu.LoadROM(cartridgeFile, string.Empty);
			InitDebugger();
			EnableDebugger();
			RunGame();
		}

		public void InitializeEmulator(string baseDir, Action<LogData> logHandler, Control renderControl)
		{
			_renderControl = renderControl;
			_logHandler = logHandler;
			StartEmulatorProcess(baseDir);
		}

		public void InitDebugger()
		{
            var debuggerAlreadyRunning = InteropEmu.DebugIsDebuggerRunning();
			InteropEmu.DebugInitialize();

			//_previousCycle = state.CPU.CycleCount;
			/*
			//Pause a few frames later to give the debugger a chance to disassemble some code
		 // _firstBreak = true;
			if (!debuggerAlreadyRunning)
			{
			    InteropEmu.SetFlag(EmulationFlags.ForceMaxSpeed, true);
			    InteropEmu.DebugStep((uint)1);
			}
			else
			{
			    //Break once to show code and then resume execution
			    InteropEmu.DebugStep(1);
			}
			InteropEmu.Resume();*/

		}
		public void EnableDebugger()
		{
			InteropEmu.SetFlag(EmulationFlags.DebuggerWindowEnabled, true);
			InteropEmu.DebugRun();
			RefreshBreakpoints();
		}

		public void RunGame()
        {
			/*emulatorThread = Task.Run(() =>
	        {
		        try
		        {
			        InteropEmu.Run();
		        }
		        catch (Exception ex)
		        {
					Program.Error(ex.Message, ex);
		        }

			})*/
	        var task = new Thread(() => {
		        InteropEmu.Run();
	        }, 30000000);
	        task.Start(); ;
			/*emulatorThread = new Thread(() => {
				InteropEmu.Run();
            }, 30000000);
	        emulatorThread.Start();*/
		}
		private void StartEmulatorProcess(string baseDir)
		{
			/*
			var process = new Process();
			process.StartInfo.WorkingDirectory = Environment.CurrentDirectory + @"\fceux";
			process.StartInfo.FileName = "fceux.exe";
			process.StartInfo.Arguments = cartridgeFile;
			process.EnableRaisingEvents = true;
			process.Exited += (sender, args) => { _emulatorProcess = null; };

			process.Start();
			_emulatorProcess = process;
			*/
			var test = InteropEmu.TestDll();
			var version = InteropEmu.GetMesenVersion();
			InteropEmu.SetDisplayLanguage(Language.English);
			InteropEmu.InitDll();

			_notifListener = new InteropEmu.NotificationListener(InteropEmu.ConsoleId.Master);
			_notifListener.OnNotification += HandleNotification;

			ApplyConfig();
			InteropEmu.ScreenSize size = InteropEmu.GetScreenSize(false);
			_renderControl.Size = new Size(size.Width, size.Height);
			InteropEmu.InitializeEmu(baseDir, _mainWindow.Handle, _renderControl.Handle, false, false, false);



			InteropEmu.AddKnownGameFolder(@"C:\Users\dkmrs\Documents\NesDev\sc");
			ApplyInputConfig();
			ApplyAudioConfig();
			ApplyPreferenceConfig();
			ApplyEmulationConfig();

			InteropEmu.SetNesModel(NesModel.Auto);
			InteropEmu.DebugSetDebuggerConsole(InteropEmu.ConsoleId.Master);

		}

		public void SetScale(double scale)
		{
			if (scale % 1 == 0) InteropEmu.SetVideoResizeFilter(VideoResizeFilter.NearestNeighbor);
			else InteropEmu.SetVideoResizeFilter(VideoResizeFilter.Bilinear);
			InteropEmu.SetVideoScale(scale);
			InteropEmu.ScreenSize size = InteropEmu.GetScreenSize(false);
			_renderControl.Size = new Size(size.Width, size.Height);
		}

		public void ForceNewState(EmulationState state)
		{
			InteropEmu.DebugSetState(state.NesState);
		}

		public void SaveState(string file)
		{
			InteropEmu.SaveStateFile(file);
		}

		public void LoadState(string file)
		{
			InteropEmu.LoadStateFile(file);
		}

		private void HandleNotification(InteropEmu.NotificationEventArgs e)
		{
			switch (e.NotificationType)
			{
				case InteropEmu.ConsoleNotificationType.GameLoaded:
					if (InteropEmu.IsRunning())
					{
						RefreshBreakpoints();
					}
					GameLoaded();
					if (OnRun != null) OnRun();
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
					EmitDebugData();
					return;

				case InteropEmu.ConsoleNotificationType.CodeBreak:
					var source = (BreakSource)(byte)e.Parameter.ToInt64();
					//if (source == BreakSource.Breakpoint && OnBreak != null)
					if (OnBreak != null)
					{
						DebugState state = default(DebugState);
						InteropEmu.DebugGetState(ref state);
						var address = InteropEmu.DebugGetAbsoluteAddress(state.CPU.DebugPC);
						OnBreak(address);
					}
					if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Paused);
					EmitDebugData();
					return;
				case InteropEmu.ConsoleNotificationType.PpuFrameDone:
					CountFrame();
					return;
			}

			if (e.NotificationType == InteropEmu.ConsoleNotificationType.PpuFrameDone) return;
            if (e.NotificationType == InteropEmu.ConsoleNotificationType.EventViewerDisplayFrame) return;

            var status = string.Format("Emulator: {0}", e.NotificationType.ToString());
			_logHandler(new LogData(status, LogType.Normal));
		}

		private readonly EmulationState _debugState = new EmulationState(ProjectType.Nes);
		private uint[] _dummyPaletteData;
		protected override void EmitDebugData()
		{
			//lock (emulatorLock)
			{
				if (OnRegisterUpdate != null)
				{
					_debugState.Memory.CpuData = InteropEmu.DebugGetMemoryState(DebugMemoryType.CpuMemory);
					_debugState.Memory.PpuData = InteropEmu.DebugGetMemoryState(DebugMemoryType.PpuMemory);
					_debugState.Memory.OamData = InteropEmu.DebugGetMemoryState(DebugMemoryType.SpriteMemory);
					_debugState.Memory.CgRam = InteropEmu.DebugGetMemoryState(DebugMemoryType.PaletteMemory);

					InteropEmu.DebugGetState(ref _debugState.NesState);
					_debugState.Memory.X = _debugState.NesState.CPU.X;
					_debugState.Memory.Y = _debugState.NesState.CPU.Y;

					_debugState.Sprites.PixelData = InteropEmu.DebugGetSprites();
					_debugState.Sprites.Details = Sprite.GetNesSprites(_debugState.Memory.OamData, _debugState.NesState.PPU.ControlFlags.LargeSprites == 1);

					_debugState.CharacterData.PixelData[0] = InteropEmu.DebugGetChrBank(0, 0, false, CdlHighlightType.None, true, false, out _dummyPaletteData);
					_debugState.CharacterData.PixelData[1] = InteropEmu.DebugGetChrBank(1, 0, false, CdlHighlightType.None, true, false, out _dummyPaletteData);

					GetNametableData();

					OnRegisterUpdate(_debugState);
				}
			}
		}

		private void GetNametableData()
		{
			InteropEmu.DebugGetPpuScroll(out _debugState.TileMaps.ScrollX, out _debugState.TileMaps.ScrollY);
			for (int i = 0; i < _debugState.TileMaps.NumberOfMaps; i++)
			{
				InteropEmu.DebugGetNametable(i, NametableDisplayMode.Normal, out _debugState.TileMaps.PixelData[i], out _debugState.TileMaps.TileData[i], out _debugState.TileMaps.AttributeData[i]);
			}
		}
		private void PushNametableData()
		{
			GetNametableData();
			if (OnRegisterUpdate != null) OnRegisterUpdate(_debugState);
		}
		public void SetCpuMemory(int offset, byte value)
		{
			InteropEmu.DebugSetMemoryValue(DebugMemoryType.CpuMemory, (uint)offset, value);
		}
		public void SetPpuMemory(int offset, byte value)
		{
			InteropEmu.DebugSetMemoryValue(DebugMemoryType.PpuMemory, (uint)offset, value);
		}
		public void SetOamMemory(int offset, byte value)
		{
			InteropEmu.DebugSetMemoryValue(DebugMemoryType.SpriteMemory, (uint)offset, value);
		}

		public void Dispose()
		{
			if (IsRunning())
			{
				InteropEmu.Pause();
				Stop();
			}
		}

		private static double ConvertVolume(int volume)
		{
			return ((double)volume / 100d);
		}
		private static double ConvertPanning(Int32 panning)
		{
			return (double)((panning + 100) / 100d);
		}

		public static void ApplyEmulationConfig()
		{
			InteropEmu.SetEmulationSpeed(100);
			InteropEmu.SetTurboRewindSpeed(300, 100);

			InteropEmu.SetFlag(EmulationFlags.DisableGameSelectionScreen, true);
			InteropEmu.SetFlag(EmulationFlags.Mmc3IrqAltBehavior, false);
			InteropEmu.SetFlag(EmulationFlags.AllowInvalidInput, false);
			InteropEmu.SetFlag(EmulationFlags.RemoveSpriteLimit, false);
			InteropEmu.SetFlag(EmulationFlags.AdaptiveSpriteLimit, true);
			InteropEmu.SetFlag(EmulationFlags.ShowLagCounter, false);
			InteropEmu.SetFlag(EmulationFlags.DisablePpu2004Reads, false);
			InteropEmu.SetFlag(EmulationFlags.DisablePaletteRead, false);
			InteropEmu.SetFlag(EmulationFlags.DisableOamAddrBug, false);
			InteropEmu.SetFlag(EmulationFlags.DisablePpuReset, false);
			InteropEmu.SetFlag(EmulationFlags.EnableOamDecay, false);
			InteropEmu.SetFlag(EmulationFlags.UseNes101Hvc101Behavior, false);
			InteropEmu.SetFlag(EmulationFlags.RandomizeMapperPowerOnState, false);

			InteropEmu.SetPpuNmiConfig(0, 0);

			InteropEmu.SetRamPowerOnState(RamPowerOnState.AllZeros);
		}
		public void UpdateSettings(EmulatorSettings settings)
		{
			InteropEmu.SetRamPowerOnState(settings.RandomPowerOnState ? RamPowerOnState.Random : RamPowerOnState.AllZeros);
			InteropEmu.SetFlag(EmulationFlags.RandomizeMapperPowerOnState, settings.RandomPowerOnState);

			InteropEmu.SetMasterVolume(settings.PlayAudio ? 25 / 10d : 0, 75 / 100d);
			InteropEmu.SetChannelVolume(AudioChannel.Square1, ConvertVolume(settings.PlaySquare1 ? 100 : 0));
			InteropEmu.SetChannelVolume(AudioChannel.Square2, ConvertVolume(settings.PlaySquare2 ? 100 : 0));
			InteropEmu.SetChannelVolume(AudioChannel.Triangle, ConvertVolume(settings.PlayTriangle ? 100 : 0));
			InteropEmu.SetChannelVolume(AudioChannel.Noise, ConvertVolume(settings.PlayNoise ? 100 : 0));
			InteropEmu.SetChannelVolume(AudioChannel.DMC, ConvertVolume(settings.PlayPcm ? 100 : 0));

			InteropEmu.SetFlag(EmulationFlags.DisableBackground, !settings.ShowBgLayer);
			InteropEmu.SetFlag(EmulationFlags.DisableSprites, !settings.ShowSpriteLayer);

			InteropEmu.SetRgbPalette(settings.NesPalette.GetBinary(), 4*16);
		}


		public static void ApplyAudioConfig()
		{
			InteropEmu.SetAudioDevice("");
			InteropEmu.SetAudioLatency(60);
			InteropEmu.SetMasterVolume(25/ 10d, 75/ 100d);
			InteropEmu.SetChannelVolume(AudioChannel.Square1, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.Square2, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.Triangle, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.Noise, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.DMC, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.FDS, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.MMC5, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.VRC6, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.VRC7, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.Namco163, ConvertVolume(100));
			InteropEmu.SetChannelVolume(AudioChannel.Sunsoft5B, ConvertVolume(100));

			InteropEmu.SetChannelPanning(AudioChannel.Square1, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.Square2, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.Triangle, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.Noise, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.DMC, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.FDS, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.MMC5, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.VRC6, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.VRC7, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.Namco163, ConvertPanning(0));
			InteropEmu.SetChannelPanning(AudioChannel.Sunsoft5B, ConvertPanning(0));

			InteropEmu.SetEqualizerFilterType(EqualizerFilterType.None);

			if(true) {
				double[] defaultBands = new double[20] { 40, 56, 80, 113, 160, 225, 320, 450, 600, 750, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 10000, 12500, 15000 };
				InteropEmu.SetEqualizerBands(defaultBands, (UInt32)defaultBands.Length);

				InteropEmu.SetBandGain(0, (double)0 / 10);
				InteropEmu.SetBandGain(1, (double)0 / 10);
				InteropEmu.SetBandGain(2, (double)0 / 10);
				InteropEmu.SetBandGain(3, (double)0 / 10);
				InteropEmu.SetBandGain(4, (double)0 / 10);
				InteropEmu.SetBandGain(5, (double)0 / 10);
				InteropEmu.SetBandGain(6, (double)0 / 10);
				InteropEmu.SetBandGain(7, (double)0 / 10);
				InteropEmu.SetBandGain(8, (double)0/ 10);
				InteropEmu.SetBandGain(9, (double)0/ 10);
				InteropEmu.SetBandGain(10, (double)0/ 10);
				InteropEmu.SetBandGain(11, (double)0/ 10);
				InteropEmu.SetBandGain(12, (double)0/ 10);
				InteropEmu.SetBandGain(13, (double)0/ 10);
				InteropEmu.SetBandGain(14, (double)0/ 10);
				InteropEmu.SetBandGain(15, (double)0/ 10);
				InteropEmu.SetBandGain(16, (double)0/ 10);
				InteropEmu.SetBandGain(17, (double)0/ 10);
				InteropEmu.SetBandGain(18, (double)0/ 10);
				InteropEmu.SetBandGain(19, (double)0/ 10);
			}

			InteropEmu.SetSampleRate(48000);

			InteropEmu.SetFlag(EmulationFlags.MuteSoundInBackground, false);
			InteropEmu.SetFlag(EmulationFlags.ReduceSoundInBackground, true);
			InteropEmu.SetFlag(EmulationFlags.ReduceSoundInFastForward, false);

			InteropEmu.SetFlag(EmulationFlags.DisableDynamicSampleRate, false);
			InteropEmu.SetFlag(EmulationFlags.SwapDutyCycles, false);
			InteropEmu.SetFlag(EmulationFlags.SilenceTriangleHighFreq, false);
			InteropEmu.SetFlag(EmulationFlags.ReduceDmcPopping, false);
			InteropEmu.SetFlag(EmulationFlags.DisableNoiseModeFlag, false);

			InteropEmu.SetAudioFilterSettings(new InteropEmu.AudioFilterSettings() {
				Filter = InteropEmu.StereoFilter.None,
				Angle = (double)15 / 180 * Math.PI,
				Delay = 5,
				Strength = 100,
				ReverbDelay = 0,
				ReverbStrength = 0,
				CrossFeedRatio = 0
			});
		}

		public static void ApplyPreferenceConfig()
		{
			InteropEmu.SetOsdState(false);
			InteropEmu.SetGameDatabaseState(true);

			InteropEmu.SetFlag(EmulationFlags.AllowInvalidInput, false);
			InteropEmu.SetFlag(EmulationFlags.RemoveSpriteLimit, false);
			InteropEmu.SetFlag(EmulationFlags.FdsAutoLoadDisk, true);
			InteropEmu.SetFlag(EmulationFlags.FdsFastForwardOnLoad, false);
			InteropEmu.SetFlag(EmulationFlags.FdsAutoInsertDisk, false);
			InteropEmu.SetFlag(EmulationFlags.PauseOnMovieEnd, true);
			InteropEmu.SetFlag(EmulationFlags.AllowBackgroundInput, false);
			InteropEmu.SetFlag(EmulationFlags.UseHighResolutionTimer, true);
			InteropEmu.SetFlag(EmulationFlags.AllowMismatchingSaveStates, false);			

			InteropEmu.SetFlag(EmulationFlags.ShowFrameCounter, false);
			InteropEmu.SetFlag(EmulationFlags.ShowGameTimer, false);

			InteropEmu.SetFlag(EmulationFlags.HidePauseOverlay, false);
			InteropEmu.SetFlag(EmulationFlags.DisplayMovieIcons, false);
			InteropEmu.SetFlag(EmulationFlags.DisableGameSelectionScreen, false);
			InteropEmu.SetFlag(EmulationFlags.ConfirmExitResetPower, false);

			InteropEmu.NsfSetNsfConfig(3000, 120, true);
			InteropEmu.SetFlag(EmulationFlags.NsfRepeat, false);
			InteropEmu.SetFlag(EmulationFlags.NsfShuffle, false);

			InteropEmu.SetFlag(EmulationFlags.DisplayDebugInfo, false);

			InteropEmu.SetFlag(EmulationFlags.VsDualMuteMaster, false);
			InteropEmu.SetFlag(EmulationFlags.VsDualMuteSlave, false);

			InteropEmu.SetAutoSaveOptions(5, false);

			InteropEmu.ClearShortcutKeys();


			InteropEmu.SetRewindBufferSize(300);

			InteropEmu.SetFolderOverrides(Path.Combine(Program.EmulatorDirectory, "Saves"), Path.Combine(Program.EmulatorDirectory, "SaveStates"), Path.Combine(Program.EmulatorDirectory, "Screenshots"));
		}

		public static void ApplyConfig()
		{
			InteropEmu.SetFlag(EmulationFlags.ShowFPS, false);
			InteropEmu.SetFlag(EmulationFlags.VerticalSync, false);
			InteropEmu.SetFlag(EmulationFlags.UseHdPacks, false);
			InteropEmu.SetFlag(EmulationFlags.IntegerFpsMode, false);

			InteropEmu.SetFlag(EmulationFlags.DisableBackground, false);
			InteropEmu.SetFlag(EmulationFlags.DisableSprites, false);
			InteropEmu.SetFlag(EmulationFlags.ForceBackgroundFirstColumn, false);
			InteropEmu.SetFlag(EmulationFlags.ForceSpritesFirstColumn, false);

			InteropEmu.SetFlag(EmulationFlags.UseCustomVsPalette, false);

			InteropEmu.SetOverscanDimensions(0, 0, 0, 0);
			InteropEmu.SetScreenRotation((UInt32)0);

			InteropEmu.SetExclusiveRefreshRate((UInt32)60);

			InteropEmu.SetVideoFilter(VideoFilterType.Prescale2x);
			InteropEmu.SetVideoResizeFilter(VideoResizeFilter.NearestNeighbor);
			InteropEmu.SetVideoScale(1);
			InteropEmu.SetVideoAspectRatio(VideoAspectRatio.NoStretching, 1);

			InteropEmu.SetPictureSettings(0 / 100.0, 0 / 100.0, 0 / 100.0, 0 / 100.0, 0 / 100.0);
			InteropEmu.SetNtscFilterSettings(0 / 100.0, 0 / 100.0, 0 / 100.0, 0 / 100.0, 0 / 100.0, 0 / 100.0, false, 0 / 100.0, 0 / 100.0, 0 / 100.0, true);

			/*if(!string.IsNullOrWhiteSpace(videoInfo.PaletteData)) {
				try {
					byte[] palette = System.Convert.FromBase64String(videoInfo.PaletteData);
					if(palette.Length == 64*4) {
						InteropEmu.SetRgbPalette(palette);
					}
				} catch { }
			}*/
		}

		public void UpdateControllerMappings(Dictionary<ControllerButtons, int> mappings)
		{
			var newMapping = new InteropEmu.KeyMappingSet();
			newMapping.Mapping1.A = GetMapping(mappings, ControllerButtons.A);
			newMapping.Mapping1.B = GetMapping(mappings, ControllerButtons.B);
			newMapping.Mapping1.Down = GetMapping(mappings, ControllerButtons.Down);
			newMapping.Mapping1.Up = GetMapping(mappings, ControllerButtons.Up);
			newMapping.Mapping1.Left = GetMapping(mappings, ControllerButtons.Left);
			newMapping.Mapping1.Right = GetMapping(mappings, ControllerButtons.Right);
			newMapping.Mapping1.Select = GetMapping(mappings, ControllerButtons.Select);
			newMapping.Mapping1.Start = GetMapping(mappings, ControllerButtons.Start);
			InteropEmu.SetControllerKeys(0, newMapping);
		}

		private static uint GetMapping(Dictionary<ControllerButtons, int> mappings, ControllerButtons defaultKey)
		{
			return mappings.ContainsKey(defaultKey) ? (uint)mappings[defaultKey] : (uint)defaultKey;
		}

		private void ApplyInputConfig()
		{
			InteropEmu.SetFlag(EmulationFlags.AutoConfigureInput, true);
			InteropEmu.SetConsoleType(ConsoleType.Nes);
			InteropEmu.SetExpansionDevice(InteropEmu.ExpansionPortDevice.None);
			InteropEmu.SetFlag(EmulationFlags.HasFourScore, false);

			InteropEmu.SetControllerType(0, InteropEmu.ControllerType.StandardController);
			UpdateControllerMappings(new Dictionary<ControllerButtons, int>());

			for (int i = 1; i < 4; i++) {
				InteropEmu.SetControllerType(i, InteropEmu.ControllerType.None);
//				InteropEmu.SetControllerKeys(i, inputInfo.Controllers[i].GetKeyMappingSet());
			}

			byte displayPorts = 0;
			InteropEmu.SetInputDisplaySettings(displayPorts, InteropEmu.InputDisplayPosition.BottomRight, true);

			InteropEmu.SetZapperDetectionRadius(0);

			InteropEmu.SetMouseSensitivity(InteropEmu.MouseDevice.ArkanoidController, (1 + 1) / 2.0);
			InteropEmu.SetMouseSensitivity(InteropEmu.MouseDevice.HoriTrack, (1 + 1) / 2.0);
			InteropEmu.SetMouseSensitivity(InteropEmu.MouseDevice.SnesMouse, (1 + 1) / 2.0);
			InteropEmu.SetMouseSensitivity(InteropEmu.MouseDevice.SuborMouse, (1 + 1) / 2.0);

		}

		private List<InteropBreakpoint> _breakpoints = new List<InteropBreakpoint>();
		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			lock (_breakpoints)
			{
				_breakpoints = new List<InteropBreakpoint>();
				var id = 0;
				byte[] condition = Encoding.UTF8.GetBytes("");

				foreach (var breakpoint in breakpoints)
				{
					var emuBreakpoint = new InteropBreakpoint
					{
						Id = id,
						Enabled = true,
						StartAddress = breakpoint.StartAddress,
						EndAddress = breakpoint.EndAddress == null ? -1 : breakpoint.EndAddress.Value,
						MarkEvent = false,
						ProcessDummyReadWrites = false,
						Condition = new byte[1000]
					};
					Array.Copy(condition, emuBreakpoint.Condition, condition.Length);
					if ((breakpoint.Type & Breakpoint.Types.Execute) != 0) emuBreakpoint.Type |= BreakpointTypeFlags.Execute;
					if ((breakpoint.Type & Breakpoint.Types.Read) != 0) emuBreakpoint.Type |= BreakpointTypeFlags.Read;
					if ((breakpoint.Type & Breakpoint.Types.Write) != 0) emuBreakpoint.Type |= BreakpointTypeFlags.Write;

					if (breakpoint.AddressType == Breakpoint.AddressTypes.PrgRom) emuBreakpoint.MemoryType = DebugMemoryType.PrgRom;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.ChrRom) emuBreakpoint.MemoryType = DebugMemoryType.ChrRom;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Cpu) emuBreakpoint.MemoryType = DebugMemoryType.CpuMemory;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Ppu) emuBreakpoint.MemoryType = DebugMemoryType.PpuMemory;
					if (breakpoint.AddressType == Breakpoint.AddressTypes.Oam) emuBreakpoint.MemoryType = DebugMemoryType.SpriteMemory;

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
				InteropEmu.DebugSetBreakpoints(_breakpoints.ToArray(), (UInt32)_breakpoints.Count);
			}
		}

		public override bool IsRunning()
		{
			return InteropEmu.IsRunning();
		}

		public void Resume()
		{
			if (!IsRunning()) return;
			InteropEmu.DebugRun();
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
		}

		public void Pause()
		{
			if (!IsRunning()) return;
			InteropEmu.DebugStep(1);
		}

		public void Stop()
		{
			if (!IsRunning()) return;

			Task.Run(() =>
			{
				InteropEmu.Pause();
				InteropEmu.Stop();
				//InteropEmu.Release();
			});
			//emulatorThread.Wait();

			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Stopped);
		}

		public void Restart()
		{
			if (!IsRunning()) return;
			InteropEmu.PowerCycle();
			if (OnStatusChange != null) OnStatusChange(EmulatorStatus.Playing);
		}

		public void SetKeyState(int scanCode, bool state)
		{
			InteropEmu.SetKeyState(scanCode, state);
		}

		public void StepOver()
		{
			InteropEmu.DebugStepOver();
		}

		public void StepInto()
		{
			InteropEmu.DebugStep(1);
		}

		public void StepOut()
		{
			InteropEmu.DebugStepOut();
		}

		public void StepBack()
		{
			InteropEmu.DebugStepBack();
		}

		public void SetSpeed(int speed)
		{
			InteropEmu.SetEmulationSpeed((UInt32)speed);
		}

		public static void InitiateInputCheck()
		{
			InteropEmu.UpdateInputDevices();
			InteropEmu.ResetKeyState();
			InteropEmu.DisableAllKeys(true);
		}
		public static void EndInputCheck()
		{
			InteropEmu.DisableAllKeys(false);
		}

		public static Tuple<uint, string> GetCurrentInput()
		{
			var scanCodes = InteropEmu.GetPressedKeys().ToList();
			if (scanCodes.Count == 0) return null;
			var key = scanCodes.OrderBy(c => c).Last();
			var name = InteropEmu.GetKeyName(key);

			return new Tuple<uint, string>(key, name);
		}

		public static string GetInputName(int code)
		{
			return InteropEmu.GetKeyName((uint)code);
		}
	}
	public enum EmulatorStatus
	{
		Stopped, Playing, Paused
	}
}
