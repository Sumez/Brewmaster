using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.BuildProcess;
using Brewmaster.EditorWindows;
using Brewmaster.EditorWindows.Code;
using Brewmaster.EditorWindows.Images;
using Brewmaster.EditorWindows.Text;
using Brewmaster.Emulation;
using Brewmaster.Layout;
using Brewmaster.MemoryViewer;
using Brewmaster.Modules;
using Brewmaster.Modules.Breakpoints;
using Brewmaster.Modules.Build;
using Brewmaster.Modules.Ca65Helper;
using Brewmaster.Modules.NumberHelper;
using Brewmaster.Modules.OpcodeHelper;
using Brewmaster.Modules.Ppu;
using Brewmaster.Modules.SpriteList;
using Brewmaster.Modules.Watch;
using Brewmaster.ProjectExplorer;
using Brewmaster.ProjectModel;
using Brewmaster.ProjectWizard;
using Brewmaster.Settings;
using Brewmaster.StatusView;

namespace Brewmaster
{
	public partial class MainForm : Form, IIdeLayoutParent
	{
		public const string SettingsFileName = "user.bwmsettings";
		public const string LayoutFileName = "user.bwmlayout";
	    public const string ProgramTitle = "Brewmaster";

	    public AsmProject CurrentProject { get; set; }
	    public Settings.Settings Settings { get; private set; }
		public SpriteViewer Sprites { get; private set; }
		public ChrViewer ChrViewer { get; private set; }
		public SpriteList SpriteList { get; private set; }
		public TileMapViewer TileMap { get; private set; }
		public MesenControl Mesen { get; private set; }
		public CpuStatus CpuStatus { get; private set; }
		public MemoryTabs MemoryTabs { get; private set; }
		public ConsolePaletteViewer ConsolePaletteViewer { get; private set; }
		public GamePaletteViewer GamePaletteViewer { get; private set; }
		public LayoutHandler LayoutHandler { get { return StoredLayoutHandler; } }
		public StoredLayoutHandler StoredLayoutHandler { get; private set; }
	    public BuildHandler BuildHandler { get; private set; }
	    public string RequestFile { get; set; }
		public OpcodeHelper OpcodeHelper { get; private set; }
		public Ca65CommandDocumentation Ca65Helper { get; private set; }
		public ProjectExplorer.ProjectExplorer ProjectExplorer { get; private set; }

		private readonly Action<LogData> _logHandler;
		private readonly Action<int> _breakHandler;
	    private readonly Action<IEnumerable<Breakpoint>> _breakpointHandler;
		private readonly Action<EmulatorStatus> _emulationStatusHandler;
		private readonly Action<EmulationState> _debugStateHandler;
		private Events _moduleEvents = new Events();
		private MemoryState _memoryState;


		private void RefreshView()
	    {
		    Text = CurrentProject == null ? ProgramTitle : CurrentProject.Name + " - " + ProgramTitle;
		    SetStatus();

			build.Enabled =
			buildProjectMenuItem.Enabled =
			runNewBuild.Enabled =
			runNewBuildMenuItem.Enabled =

			saveToolStripButton.Enabled =
			saveAllToolStripButton.Enabled =
			File_SaveMenuItem.Enabled =
			File_SaveAsMenuItem.Enabled =
			File_SaveAllMenuItem.Enabled =
			File_CloseProjectMenuItem.Enabled =

			sourceFileMenuItem.Enabled =
			includeFileMenuItem.Enabled =
			tileMapMenuItem.Enabled =
			graphicsDataMenuItem.Enabled =

			nesGraphicsMenuItem.Enabled =
			nesAudioMenuItem.Enabled =

			configurationSelector.Enabled =
			buildSettings.Enabled = 
			buildSettingsMenuItem.Enabled =
			mapInputMenuItem.Enabled = 
				CurrentProject != null;

			nesGraphicsMenuItem.Visible =
			nesAudioMenuItem.Visible =
				CurrentProject == null || CurrentProject.Type == ProjectType.Nes;
			snesGraphicsMenuItem.Visible =
			snesAudioMenuItem.Visible =
				CurrentProject != null && CurrentProject.Type == ProjectType.Snes;

			saveToolStripButton.Enabled =
				editorTabs.SelectedTab is EditorWindow;

			EmulatorStatusChanged(_emulatorStatus);

		    ActiveFileChanged();

	    }
		private void ActiveFileChanged()
		{
			var currentTab = editorTabs.SelectedTab;
			var saveableTab = currentTab as ISaveable;
			var editorTab = currentTab as EditorWindow;

			if (saveableTab != null)
			{
				var fileName = currentTab.Text.TrimEnd('*', ' ');
				File_SaveMenuItem.Text = "Save " + fileName;
				File_SaveAsMenuItem.Text = "Save " + fileName + " As...";

				saveToolStripButton.Enabled = File_SaveMenuItem.Enabled = !saveableTab.Pristine;
				File_SaveAsMenuItem.Enabled = true;
			}
			else
			{
				File_SaveMenuItem.Text = "Save";
				File_SaveAsMenuItem.Text = "Save As...";
				saveToolStripButton.Enabled = 
				File_SaveMenuItem.Enabled = File_SaveAsMenuItem.Enabled = false;
			}
			Edit_CutMenuItem.Enabled = Edit_CopyMenuItem.Enabled = Edit_PasteMenuItem.Enabled =
			selectAllMenuItem.Enabled = insertMenuItem.Enabled =
			Edit_UndoMenuItem.Enabled = Edit_RedoMenuItem.Enabled =
			findMenuItem.Enabled = findNextMenuItem.Enabled = 
			replaceMenuItem.Enabled = Edit_GoToMenuItem.Enabled =
			File_PrintMenuItem.Enabled = File_PrintPreviewMenuItem.Enabled = currentTab is TextEditorWindow;
			File_CloseMenuItem.Enabled = File_CloseAllMenuItem.Enabled = closeAllWindowsMenuItem.Enabled = editorTabs.TabCount > 0;

			if (editorTab == null) filenameLabel.Text = ProgramTitle;
			else filenameLabel.Text = editorTab.ProjectFile.File.FullName;

			if (editorTab is TextEditorWindow textEditorWindow) SetCaretInformation(textEditorWindow);
			else lineLabel.Text = charLabel.Text = "";

			UpdateTabListInWindowMenu();
		}

	    private void SetCaretInformation(TextEditorWindow textEditorWindow)
	    {
			var caret = textEditorWindow.TextEditor.ActiveTextAreaControl.Caret;
		    lineLabel.Text = (caret.Line + 1).ToString();
			charLabel.Text = (caret.Column + 1).ToString();
		}

	    private void BuildErrorUpdate(List<BuildHandler.BuildError> list)
	    {
		    ErrorList.RefreshList(list);
		    if (list.Any(e => e.Type == BuildHandler.BuildError.BuildErrorType.Error)) RevealPanel(ErrorList);
	    }

	    private void RevealPanel(Control targetControl)
	    {
			var panel = targetControl.Parent as IdePanel;
		    if (panel != null && panel.GroupParent != null) panel.GroupParent.ShowPanel(panel);
		}


		private void ThreadSafeLogOutput(LogData data)
	    {
		    if (IsDisposed || Disposing) return;
			BeginInvoke(_logHandler, data);
	    }
		private void ThreadSafeBuildErrorUpdate(List<BuildHandler.BuildError> list)
		{
			BeginInvoke(new Action<List<BuildHandler.BuildError>>(BuildErrorUpdate), list);
		}
		private void ThreadSafeBreakHandler(int address)
		{
			BeginInvoke(_breakHandler, address);
		}
	    private void ThreadSafeBreakpointHandler(IEnumerable<Breakpoint> breakpoints)
	    {
		    BeginInvoke(_breakpointHandler, breakpoints);
	    }
		private void ThreadSafeStatusHandler(EmulatorStatus status)
		{
			BeginInvoke(_emulationStatusHandler, status);
		}
		private void ThreadSafeDebugStateHandler(EmulationState state)
		{
			if (IsDisposed || Disposing) return;
			BeginInvoke(_debugStateHandler, state);
		}

	    private WatchValues WatchValues;
	    private BreakpointList BreakpointList;
	    private ErrorList ErrorList;
	    private OutputWindow OutputWindow;
	    private NumberHelper NumberHelper;
		public MainForm()
        {
			Program.CurrentWindow = this;
			LoadSettings();
			try
			{
				// Create split-containers first, so their message filters will handle mouse events before the underlying panels
				var mainSouthContainer = new MultiSplitContainer { Dock = DockStyle.Fill, Horizontal = false };
				var tabsWestContainer = new MultiSplitContainer { Dock = DockStyle.Fill };

				var eastContainer = new MultiSplitContainer { Name = "east", Dock = DockStyle.Fill, Horizontal = false };
				var westContainer = new MultiSplitContainer { Name = "west", Dock = DockStyle.Fill, Horizontal = false };
				var southContainer = new MultiSplitContainer { Name = "south", Dock = DockStyle.Fill };

				InitializeComponent();
				SuspendLayout();

				// Makes sure a control's handle is set before setting visible state
				foreach (Control control in Controls) { var handle = control.Handle; }

				// Load layout
				LoadModule(ChrViewer = new ChrViewer(_moduleEvents), "Chr");
				LoadModule(TileMap = new TileMapViewer(_moduleEvents), "Nametables");
				LoadModule(Sprites = new SpriteViewer(_moduleEvents), "Sprites");
				LoadModule(new ScrollableView(ConsolePaletteViewer = new ConsolePaletteViewer(_moduleEvents)), "Global Palette");
				LoadModule(new ScrollableView(GamePaletteViewer = new GamePaletteViewer(_moduleEvents)), "Palette");

				LoadModule(MemoryTabs = new MemoryTabs(_moduleEvents), "Memory Viewer");
				LoadModule(SpriteList = new SpriteList(_moduleEvents), "Sprite List");

				LoadModule(WatchValues = new WatchValues(), "Watch");
				LoadModule(BreakpointList = new BreakpointList(_moduleEvents), "Breakpoints");

				LoadModule(OutputWindow = new OutputWindow(), "Output");
				LoadModule(ErrorList = new ErrorList(), "Build Errors");

				LoadModule(CpuStatus = new CpuStatus(_moduleEvents), "Console Status");
				LoadModule(Mesen = new MesenControl(), "Mesen");

				LoadModule(ProjectExplorer = new ProjectExplorer.ProjectExplorer(_moduleEvents), "Project Explorer");

				LoadModule(OpcodeHelper = new OpcodeHelper(_moduleEvents), "Opcodes");
				LoadModule(Ca65Helper = new Ca65CommandDocumentation(_moduleEvents), "Commands");
				LoadModule(NumberHelper = new NumberHelper(), "Number Formats");


				MainEastContainer2.AddPanel(mainSouthContainer);
				MainEastContainer2.AddPanel(eastContainer);

				mainSouthContainer.AddPanel(tabsWestContainer);
				mainSouthContainer.AddPanel(southContainer);

				tabsWestContainer.AddPanel(westContainer);
				tabsWestContainer.AddPanel(editorTabs);

				StoredLayoutHandler = new StoredLayoutHandler(this);
				StoredLayoutHandler.SetDockContainers(eastContainer, westContainer, southContainer);
				StoredLayoutHandler.LoadPanelLayout(_modules, Program.GetUserFilePath(LayoutFileName));
				AddWindowOptions();

				// Setup features
				BuildHandler = new BuildHandler();
				_logHandler = OutputWindow.LogOutput; // Create ouput log delegate in same thread as output window
				_breakHandler = FocusOnCodeLine;
				_breakpointHandler = SetBreakpoints;
				_emulationStatusHandler = EmulatorStatusChanged;

				_debugStateHandler = (state) =>
				{
					_memoryState = state.Memory;
					WatchValues.SetData(state.Memory);
					_moduleEvents.UpdateStates(state);
				};
				_moduleEvents.SelectedSpriteChanged += (index) =>
				{
					RevealPanel(Sprites);
					RevealPanel(SpriteList);
				};


				editorTabs.TabWindowsChanged += (tabs) => ActiveFileChanged();
				editorTabs.SelectedIndexChanged += (o, a) => ActiveFileChanged();
				editorTabs.ControlAdded += (o, a) => ActiveFileChanged();

				_menuHelper.Prepare(new [] { MainWindowMenu, MainToolStrip }, WriteStatus);

				// Apply settings
				viewToolbarMenuItem.Checked = toolstrippanel.Visible = MainToolStrip.Visible = Settings.ShowToolbar;
				viewStatusBarMenuItem.Checked = statusBar.Visible = Settings.ShowStatusBar;
				TextEditor.DefaultCodeProperties.ShowLineNumbers = viewLineNumbersMenuItem.Checked = Settings.ShowLineNumbers;
				lineAddressMappingsMenuItem.Checked = Settings.ShowLineAddresses;

				updateEveryFrameMenuItem.Checked = (Mesen.UpdateRate = Settings.UpdateRate) == 1;
				integerScalingMenuItem.Checked = Mesen.IntegerScaling = Settings.EmuIntegerScaling;
				randomValuesAtPowerOnMenuItem.Checked = Mesen.RandomPowerOnState = Settings.EmuRandomPowerOn;
				playSpcAudioMenuItem.Checked = Mesen.PlayAudio = Settings.EmuPlayAudio;
				playPulse1MenuItem.Checked = Mesen.PlaySquare1 = Settings.EmuPlayPulse1;
				playPulse2MenuItem.Checked = Mesen.PlaySquare2 = Settings.EmuPlayPulse2;
				playTriangleMenuItem.Checked = Mesen.PlayTriangle = Settings.EmuPlayTriangle;
				playNoiseMenuItem.Checked = Mesen.PlayNoise = Settings.EmuPlayNoise;
				playPcmMenuItem.Checked = Mesen.PlayPcm = Settings.EmuPlayPcm;
				displayNesBgMenuItem.Checked = Mesen.ShowBgLayer = Settings.EmuDisplayNesBg;
				displayNesObjectsMenuItem.Checked = Mesen.ShowSpriteLayer = Settings.EmuDisplaySprites;
				Mesen.EmulatorBackgroundColor = Settings.EmuBackgroundColor;

				TileMap.ShowScrollOverlay = Settings.ShowScrollOverlay;
				TileMap.FitImage = Settings.ResizeTileMap;

				if (Settings.AsmHighlighting != null && Settings.AsmHighlighting.SerializedData.Count > 0) Ca65Highlighting.DefaultColors = Settings.AsmHighlighting.Data;
				TextEditor.DefaultCodeProperties.Font = Settings.DefaultFont;

				if (Settings.WindowX.HasValue && Settings.WindowY.HasValue)
				{
					var location = new Point(Settings.WindowX.Value, Settings.WindowY.Value);
					// Check if last position is still a visible position on the desktop
					if (Screen.FromPoint(location).WorkingArea.IntersectsWith(new Rectangle(location, Size))) {
						StartPosition = FormStartPosition.Manual;
						Location = location;
						Left = location.X;
						Top = location.Y;
					}
					if (Settings.WindowState != FormWindowState.Minimized) WindowState = Settings.WindowState;
				}

				BindShortcutKeys();

				//Resize += (sender, args) => { Refresh(); }; // this shouldn't be necessary
			}
			catch (Exception ex)
			{
				Program.Error(string.Format("An unexpected error happened while loading settings. Check the error log for details:\n\n{0}",Program.GetErrorFilePath()), ex);
			}
			ResumeLayout();
        }

		private readonly Dictionary<string, Control> _modules = new Dictionary<string, Control>();
		private void LoadModule(Control control, string label)
		{
			new IdePanel(control) { Label = label };
			_modules.Add(label, control);
		}

		private void BindShortcutKeys()
		{
			Program.BindKey(Feature.CreateNew, nesProjectMenuItem);
			Program.BindKey(Feature.Open, File_OpenProjectMenuItem);
			Program.BindKey(Feature.Save, File_SaveMenuItem);
			Program.BindKey(Feature.SaveAll, File_SaveAllMenuItem);
			Program.BindKey(Feature.Print, File_PrintMenuItem);
			Program.BindKey(Feature.Exit, File_ExitMenuItem);

			Program.BindKey(Feature.Undo, (keys) => Edit_UndoMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			Program.BindKey(Feature.Redo, (keys) => Edit_RedoMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			Program.BindKey(Feature.Cut, (keys) => Edit_CutMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			Program.BindKey(Feature.Copy, (keys) => Edit_CopyMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			Program.BindKey(Feature.Paste, (keys) => Edit_PasteMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			Program.BindKey(Feature.SelectAll, (keys) => selectAllMenuItem.ShortcutKeyDisplayString = keys == Keys.None ? "" : ButtonAssignment.GetString(keys));
			
			Program.BindKey(Feature.Find, findMenuItem);
			Program.BindKey(Feature.FindAll, findInFilesMenuItem);
			Program.BindKey(Feature.FindNext, findNextMenuItem);
			Program.BindKey(Feature.Replace, replaceMenuItem);
			Program.BindKey(Feature.GoToLine, Edit_GoToMenuItem);
			Program.BindKey(Feature.GoToAll, goToAllMenuItem);
			Program.BindKey(Feature.Build, buildProjectMenuItem);
			Program.BindKey(Feature.Run, runMenuItem);
			Program.BindKey(Feature.RunNewBuild, runNewBuildMenuItem);
			Program.BindKey(Feature.Pause, pauseMenuItem);
			Program.BindKey(Feature.Stop, stopMenuItem);
			Program.BindKey(Feature.Restart, restartMenuItem);

			Program.BindKey(Feature.StepOver, stepOverMenuItem);
			Program.BindKey(Feature.StepInto, stepIntoMenuItem);
			Program.BindKey(Feature.StepOut, stepOutMenuItem);
			Program.BindKey(Feature.StepBack, stepBackMenuItem);

			Program.BindKey(Feature.SaveState, saveStateMenuItem);
			Program.BindKey(Feature.LoadState, loadStateMenuItem);

		}

		private void AddWindowOptions()
		{
			foreach (var module in _modules) AddWindowOption(module.Value);
		}
		private void AddWindowOption(Control windowControl)
		{
			var idePanel = LayoutHandler.GetPanel(windowControl);
			if (idePanel == null) return;

			var menuItem = new ToolStripMenuItem(idePanel.Label);
			idePanel.LabelChanged += label => { menuItem.Text = label; };
			menuItem.Checked = idePanel.FindForm() != null;
			menuItem.Click += (s, e) => {
				if (menuItem.Checked) LayoutHandler.HidePanel(idePanel);
				else LayoutHandler.ShowPanel(idePanel);
			};
			ViewMenuItem.DropDownItems.Add(menuItem);

			LayoutHandler.PanelStatusChanged += (changedPanel, visible) =>
			{
				if (changedPanel == idePanel) menuItem.Checked = visible;
			};

			// TODO: Save in settings
		}

		protected override void OnLoad(EventArgs events)
	    {
		    base.OnLoad(events);

			KeyPreview = true;
			PreviewKeyDown += (s, e) =>
			{
				e.IsInputKey = true;
			};

			Mesen.SetButtonMappings(Settings.NesMappings, Settings.SnesMappings);
			
		    MemoryTabs.Cpu.AddWatch += AddWatch;
		    MemoryTabs.Cpu.AddBreakpoint += (a, t) => AddBreakpoint(a, t, Breakpoint.AddressTypes.Cpu);
		    MemoryTabs.Ppu.AddBreakpoint += (a, t) => AddBreakpoint(a, t, Breakpoint.AddressTypes.Ppu);
		    MemoryTabs.Oam.AddBreakpoint += (a, t) => AddBreakpoint(a, t, Breakpoint.AddressTypes.Oam);
		    MemoryTabs.Cpu.RemoveBreakpoints += RemoveBreakpoints;
		    MemoryTabs.Ppu.RemoveBreakpoints += RemoveBreakpoints;
		    MemoryTabs.Oam.RemoveBreakpoints += RemoveBreakpoints;

		    _moduleEvents.GetCurrentProject = () => CurrentProject;
		    _moduleEvents.RemoveBreakpoints = RemoveBreakpoints;
		    _moduleEvents.AddBreakpoint = AddBreakpoint;
		    _moduleEvents.UpdatedBreakpoints = ActivateBreakPointsForCurrentProject;
		    _moduleEvents.GetCurrentTextEditor = GetCurrentTextEditor;
			_moduleEvents.OpenFileAction = (file, line, column, length) =>
			{
				if (line.HasValue) GoTo(file.File.FullName, line.Value, column.HasValue ? column.Value : 0, length);
				else OpenFileInTab(file);
			};


			_moduleEvents.Cut = Cut;
		    _moduleEvents.Copy = Copy;
		    _moduleEvents.Paste = Paste;
		    _moduleEvents.Delete = Delete;
		    _moduleEvents.SelectAll = SelectAll;

		    BreakpointList.GoTo = (file, line) => FocusOnCodeLine(file, line);
		    OutputWindow.GoTo = (file, line) => FocusOnCodeLine(file, line);
		    ErrorList.GoTo = (file, line) => FocusOnCodeLine(file, line);

			FormClosed += (sender, args) => UnloadEmulator();

			ProjectExplorer.OpenFile = (file) => OpenFileInTab(file);
		    ProjectExplorer.CreateNewFile = CreateNewFile;
		    ProjectExplorer.AddExistingFile = AddExistingFile;

			BuildHandler.Log = ThreadSafeLogOutput;
		    BuildHandler.RefreshErrorList = ThreadSafeBuildErrorUpdate;
			BuildHandler.Status = SetStatus;
		    BuildHandler.OnDebugDataUpdated += () => BeginInvoke(new Action(RefreshBreakpointsToLatestBuild));

		    configurationSelector.SelectedIndexChanged += (sender, args) => ChangeConfiguration();

			WatchValues.GetSymbol = (exp) => CurrentProject == null || !CurrentProject.DebugSymbols.ContainsKey(exp) ? null : CurrentProject.DebugSymbols[exp];
		    WatchValues.AddBreakpoint = AddBreakpoint;

		    OpcodeParser.GetOpcodes(ProjectType.Nes);
		    OpcodeParser.GetOpcodes(ProjectType.Snes);
		    Ca65Parser.GetCommands();

			AddRecentProjects();
			RefreshView();

		    LoadEmulator(ProjectType.Nes); // Load NES emulator so it can be used in the button configuration (TODO: use only emulator for current project to detect buttons?)

			if (RequestFile != null) LoadProject(RequestFile);
			else if (Settings.ReOpenLastProject && Settings.CurrentProject != null)
				LoadProject(Settings.CurrentProject);
			else
				_moduleEvents.SetProjectType(ProjectType.Nes);
		}

	    private void RemoveBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			if (CurrentProject != null) CurrentProject.RemoveBreakpoints(breakpoints);

		}

		public void AddBreakpoint(int address, Breakpoint.Types type, Breakpoint.AddressTypes addressType, string symbol = null)
		{
			AddBreakpoint(new Breakpoint
			{
				AddressType = addressType,
				Type = type,
				StartAddress = address,
				Symbol = symbol
			});
		}

	    private void AddBreakpoint(Breakpoint breakpoint)
	    {
		    if (CurrentProject == null) return;
			
		    if (breakpoint.Symbol != null) breakpoint.UpdateFromSymbols(CurrentProject.DebugSymbols);
		    CurrentProject.AddBreakpoint(breakpoint);
	    }

		private void LoadEmulator(ProjectType projectType)
	    {
		    Mesen.SwitchSystem(projectType, (e) => InitializeEmulator(e, projectType), this);
		    if (projectType == ProjectType.Nes) GamePaletteViewer.SourcePalette = ConsolePaletteViewer.Palette = Mesen.EmulatorSettings.NesPalette;
		    if (projectType == ProjectType.Snes) GamePaletteViewer.SourcePalette = ConsolePaletteViewer.Palette = Mesen.EmulatorSettings.SnesPalette;
			// TODO: Use events object and/or go through mesen control
		    MemoryTabs.Cpu.DataChanged = Mesen.Emulator.SetCpuMemory;
		    MemoryTabs.Ppu.DataChanged = Mesen.Emulator.SetPpuMemory;
		    MemoryTabs.Oam.DataChanged = Mesen.Emulator.SetOamMemory;
	    }

	    private void InitializeEmulator(IEmulatorHandler emulator, ProjectType projectType)
	    {
		    emulator.InitializeEmulator(Program.EmulatorDirectory, ThreadSafeLogOutput, Mesen.GetRenderSurface(projectType));
		    emulator.OnBreak += ThreadSafeBreakHandler;
		    emulator.OnRun += ActivateBreakPointsForCurrentProject;
		    emulator.OnStatusChange += ThreadSafeStatusHandler;
		    emulator.OnRegisterUpdate += ThreadSafeDebugStateHandler;
			emulator.OnFpsUpdate += fps => BeginInvoke(new Action<int>(UpdateFps), fps);

		    CpuStatus.StateEdited = emulator.ForceNewState;
	    }

	    private void UpdateFps(int fps)
	    {
		    fpsLabel.Text = string.Format(@"{0} FPS", fps);
	    }

	    private void UnloadEmulator()
		{
			if (Mesen.Emulator == null) return;

			Mesen.Emulator.Stop();
			Mesen.UnloadEmulator();
		}

		protected override void OnResizeBegin(EventArgs e)
		{
			SuspendLayout();
			base.OnResizeBegin(e);
		}
		protected override void OnResizeEnd(EventArgs e)
		{
			ResumeLayout();
			base.OnResizeEnd(e);
		}

	    private string _currentStatus;
	    private void SetStatus(string status = null)
	    {
		    _currentStatus = status;
		    WriteStatus();
	    }
	    private void WriteStatus(string message = null)
	    {
		    BeginInvoke(new Action(() =>
		    {
			    statusLabel.Text = !string.IsNullOrWhiteSpace(message) ? message : _currentStatus ?? "Ready";
			}));
	    }

		private void AddExistingFile(string targetFile, string targetDirectory, CompileMode mode)
	    {
		    if (!Directory.Exists(targetDirectory))
		    {
			    Error(string.Format("Could not find the directory {0}", targetDirectory));
			    return;
		    }
		    if (targetFile == null)
		    {
				OpenFilesFileDialog.InitialDirectory = targetDirectory;
			    OpenFilesFileDialog.FileName = "";
			    if (OpenFilesFileDialog.ShowDialog() != DialogResult.OK) return;
			    targetFile = OpenFilesFileDialog.FileName;
		    }

		    if (!File.Exists(targetFile))
		    {
			    Error(string.Format("Could not find the file {0}", targetFile));
			    return;
		    }

		    if (!targetFile.StartsWith(CurrentProject.Directory.FullName))
		    {
			    var newFilename = Path.Combine(targetDirectory, new FileInfo(targetFile).Name);
				if (File.Exists(newFilename))
			    {
				    Error("A file with the same name already exists in the project directory");
				    return;
				}
				File.Copy(targetFile, targetFile = newFilename);
		    }

			var file = new FileInfo(targetFile);
			if (mode == CompileMode.IncludeInAssembly && new string[] { ".ini", ".conf", ".ld", ".cfg", ".x" }.Contains(file.Extension))
			{
				mode = CompileMode.LinkerConfig;
			}
		    var projectFile = new AsmProjectFile
		    {
			    Mode = mode,
			    Project = CurrentProject,
			    File = file
			};
		    CurrentProject.AddNewProjectFile(projectFile);
			ProjectExplorer.FocusNode(projectFile);
	    }
		private void CreateNewFile(string target, FileTemplate template, string extension)
	    {
		    var filename = target;
		    if (Directory.Exists(target))
		    {
			    CreateNewFileDialog.InitialDirectory = target;
			    CreateNewFileDialog.FileName = "";
				CreateNewFileDialog.DefaultExt = extension;
				//CreateNewFileDialog.Filter = "FamiTracker files|*.ftm";
			    CreateNewFileDialog.Filter = "*" + extension + "|*" + extension;
				if (CreateNewFileDialog.ShowDialog() != DialogResult.OK) return;
			    filename = CreateNewFileDialog.FileName;
			}

		    if (File.Exists(filename))
		    {
			    Error(string.Format("The file {0} already exists", filename));
			    return;
		    }

		    string sourceFilename = null;
		    var projectFile = new AsmProjectFile { Mode = CompileMode.Ignore, Project = CurrentProject };
		    switch (template)
		    {
			    case FileTemplate.Famitracker:
				    sourceFilename = Path.Combine(Application.StartupPath, @"Templates\ftm");
				    projectFile.Mode = CompileMode.ContentPipeline;
				    break;
				case FileTemplate.AssemblyCode:
				    projectFile.Mode = CompileMode.IncludeInAssembly;
					break;
		    }

			if (sourceFilename == null)
		    {
				using (var file = File.Create(filename)) { }
		    }
		    else
		    {
			    File.Copy(sourceFilename, filename);
		    }

		    projectFile.File = new FileInfo(filename);
		    CurrentProject.AddNewProjectFile(projectFile);
			ProjectExplorer.FocusNode(projectFile);

			OpenFileInTab(projectFile);
		}

		private void Error(string errorMessage)
	    {
			ThreadSafeLogOutput(new LogData(errorMessage, LogType.Error));
		    MessageBox.Show(errorMessage, "Warning");
	    }

		private CodeEditor _currentFocus = null;
		private void FocusOnCodeLine(int address)
		{
			//TODO: Index all known program lines in debug info for quick lookup?
			foreach (var file in CurrentProject.Files.Where(f => f.DebugLines != null))
			{
				var matchingLine = file.DebugLines.Values.FirstOrDefault(l => l.RomAddress == address);
				if (matchingLine != null)
				{
					if (!FocusOnCodeLine(file, matchingLine.Line, true, true)) break;
				}
			}
		}

	    private void FocusOnCodeLine(string fileName, int line)
	    {
		    var file = CurrentProject.Files.FirstOrDefault(f => f.GetRelativePath() == fileName); // TODO: consider replacing back/forward slashes for platform compatibility
			if (file == null) return;
		    FocusOnCodeLine(file, line);
	    }
		private bool FocusOnCodeLine(AsmProjectFile file, int line, bool buildLine = false, bool setMarker = false)
		{
			var window = OpenFileInTab(file) as TextEditorWindow;
			if (window == null) return true;
			var codeEditor = window.TextEditor as CodeEditor;
			if (setMarker && codeEditor != null)
			{
				HideFocusArrow();
				codeEditor.FocusArrow(line);
				_currentFocus = codeEditor;
			}
			else
			{
				window.TextEditor.FocusLine(line, buildLine);
				window.TextEditor.Focus();
			}
			return true;
		}

	    private EmulatorStatus _emulatorStatus = EmulatorStatus.Stopped;
		private void EmulatorStatusChanged(EmulatorStatus status)
		{
			_emulatorStatus = status;
			if (status == EmulatorStatus.Playing || status == EmulatorStatus.Stopped) HideFocusArrow();

			runMenuItem.Enabled = run.Enabled = status != EmulatorStatus.Playing && build.Enabled;
			pauseMenuItem.Enabled = pause.Enabled = status == EmulatorStatus.Playing;
			stopMenuItem.Enabled = stop.Enabled = status != EmulatorStatus.Stopped;
			restartMenuItem.Enabled = restart.Enabled = status != EmulatorStatus.Stopped;

			loadStateMenuItem.Enabled = saveStateMenuItem.Enabled = status != EmulatorStatus.Stopped;

			stepIntoMenuItem.Enabled = stepInto.Enabled =
			stepOverMenuItem.Enabled = stepOver.Enabled =
			stepOutMenuItem.Enabled = stepOut.Enabled =
			stepBackMenuItem.Enabled = stepBack.Enabled =
				status != EmulatorStatus.Stopped;
		}

		public MemoryState GetCpuMemory()
		{
			return _memoryState;
		}
		private void HideFocusArrow()
		{
			if (_currentFocus == null) return;
			_currentFocus.RemoveFocusArrow();
			_currentFocus = null;
		}

	    private FindWindow _findForm;
	    private void FindInFiles(FindMode mode)
	    {
		    if (_findForm != null && _findForm.Visible)
		    {
			    _findForm.Mode = mode;
			    _findForm.Focus();
			    return;
		    }
		    _findForm = new FindWindow(_moduleEvents, mode);
		    _findForm.Show(this);
	    }
	    private void GoToLine()
	    {
			using (var goToForm = new GoToWindow(GetCurrentTextEditor))
			{
				goToForm.ShowDialog(this);
			}
	    }

		private void FindNext()
	    {
		    var editorWindow = GetCurrentTextEditor();
		    if (editorWindow == null) return;

			FindWindow.FindNext(editorWindow.TextEditor);
	    }

		private TextEditorWindow GetCurrentTextEditor()
	    {
		    if (editorTabs.TabCount == 0) return null;
		    return editorTabs.SelectedTab as TextEditorWindow;
		}


		private void ActivateBreakPointsForCurrentProject()
		{
			CurrentProject.RefreshBreakpoints();
		}

        public void UpdateTabListInWindowMenu()
        {
	        WindowMenuSeparator.Visible = editorTabs.TabCount > 0;
	        var windowIndex = WindowMenuItem.DropDownItems.IndexOf(WindowMenuSeparator) + 1;

	        SuspendLayout();
			while (WindowMenuItem.DropDownItems.Count > windowIndex)
				WindowMenuItem.DropDownItems.RemoveAt(windowIndex);

	        foreach (TabPage tabPage in editorTabs.TabPages)
	        {
		        var menuItem = new ToolStripMenuItem(tabPage.Text)
		        {
			        Checked = editorTabs.SelectedTab == tabPage
		        };
                menuItem.Click += (s, a) => { editorTabs.SelectedTab = tabPage; };
		        WindowMenuItem.DropDownItems.Add(menuItem);
            }
			ResumeLayout();
        }

	    private void LoadSettings()
	    {
			var userSettingsPath = Program.GetUserFilePath(SettingsFileName);
			Settings = Brewmaster.Settings.Settings.Load(userSettingsPath);
	    }

		private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
	        if (!CloseCurrentProject(true)) e.Cancel = true;
			Settings.WindowX = Location.X;
			Settings.WindowY = Location.Y;
			Settings.WindowState = WindowState;
	        Settings.ShowScrollOverlay = TileMap.ShowScrollOverlay;
	        Settings.ResizeTileMap = TileMap.FitImage;
			Settings.Save();

			StoredLayoutHandler.StorePanelLayout(_modules, Program.GetUserFilePath(LayoutFileName));
        }

        private void newNesProjectMenuItem_Click(object sender, EventArgs e)
        {
	        CreateNewProject(ProjectType.Nes);
        }
	    private void newSnesProjectMenuItem_Click(object sender, EventArgs e)
	    {
		    CreateNewProject(ProjectType.Snes);
	    }


		private void CreateNewProject(ProjectType projectType)
		{
		    using (var newProjectDialog = new NewProject(Settings, projectType))
		    {
			    newProjectDialog.StartPosition = FormStartPosition.CenterParent;
			    newProjectDialog.ShowDialog();
			    //if (!CloseCurrentProject()) return;
		    }
		}
		private void ImportExistingProject()
	    {
		    using (var importProjectDialog = new ImportProject(Settings))
		    {
			    importProjectDialog.StartPosition = FormStartPosition.CenterParent;
			    importProjectDialog.ShowDialog();
			    if (importProjectDialog.DialogResult == DialogResult.Cancel || importProjectDialog.Project == null) return;
			    if (!CloseCurrentProject()) return;

				LoadProject(importProjectDialog.Project.ProjectFile.FullName);
		    }
		}

private void File_OpenProjectMenuItem_Click(object sender, EventArgs e)
	    {
		    if (OpenProjectFileDialog.ShowDialog() != DialogResult.OK) return;
		    var projectFilename = OpenProjectFileDialog.FileName;
		    if (!File.Exists(projectFilename)) return;
		    LoadProject(projectFilename);
	    }

		public void File_SaveMenuItem_Click(object sender, EventArgs e)
        {
	        if (!(editorTabs.SelectedTab is ISaveable tab)) return;
	        tab.Save();
        }

		private void File_SaveAsMenuItem_Click(object sender, EventArgs e)
        {
	        if (!(editorTabs.SelectedTab is ISaveable tab)) return;
	        tab.Save(GetNewFileName);
        }

		private string GetNewFileName(FileInfo file)
		{
			CreateNewFileDialog.InitialDirectory = file.DirectoryName;
			CreateNewFileDialog.FileName = file.Name;
			CreateNewFileDialog.DefaultExt = file.Extension;
			CreateNewFileDialog.Filter = "*" + file.Extension + "|*" + file.Extension;
			if (CreateNewFileDialog.ShowDialog() != DialogResult.OK) return null;
			var fileInfo = new FileInfo(CreateNewFileDialog.FileName);
			if (CurrentProject != null && CurrentProject.GetRelativePath(fileInfo.FullName).StartsWith(".."))
			{
				Error("New file path must be in the project directory");
				return null;
			}
			return fileInfo.FullName;
		}

		private void File_SaveAllMenuItem_Click(object sender, EventArgs e)
        {
			SaveAll();
        }

	    private void File_CloseMenuItem_Click(object sender, EventArgs e)
	    {
		    if (editorTabs.SelectedTab != null) editorTabs.CloseTab(editorTabs.SelectedTab);
	    }
		
		private void File_CloseAllMenuItem_Click(object sender, EventArgs e)
        {
			editorTabs.CloseAll();
        }
		
	    public void SaveCurrentProject(bool saveState = true)
	    {
		    if (CurrentProject.ProjectFile == null)
		    {
			    var prompt = true;
			    while (prompt)
			    {
				    CreateNewFileDialog.FileName = "";
					CreateNewFileDialog.InitialDirectory = CurrentProject.Directory.FullName;
				    CreateNewFileDialog.DefaultExt = ".bwm";
				    CreateNewFileDialog.Filter = "Brewmaster projects|*.bwm";
				    if (CreateNewFileDialog.ShowDialog() != DialogResult.OK) return;
				    var fileName = CreateNewFileDialog.FileName;

				    if (File.Exists(fileName))
				    {
					    Error(string.Format("The file {0} already exists", fileName));
				    }
				    else
				    {
					    prompt = false;
					    CurrentProject.ProjectFile = new FileInfo(fileName);
					    CurrentProject.WatchFile();
					    AddFileToRecentProjects(fileName);
				    }
				}
		    }
		    CurrentProject.Save();
			if (saveState) SaveProjectState();
		}

		private void File_CloseProjectMenuItem_Click(object sender, EventArgs e)
        {
            CloseCurrentProject();
        }

        private void File_PrintMenuItem_Click(object sender, EventArgs e)
        {
			if (!(editorTabs.SelectedTab is TextEditorWindow)) return;

	        var printDialog = new PrintDialog { UseEXDialog = true };
			using (printDialog.Document = GetPrintDocument())
				printDialog.ShowDialog();
        }

        private void File_PrintPreviewMenuItem_Click(object sender, EventArgs e)
        {
	        if (!(editorTabs.SelectedTab is TextEditorWindow)) return;

			var printDialog = new PrintPreviewDialog();
			using (printDialog.Document = GetPrintDocument())
				printDialog.ShowDialog();
        }

	    private PrintDocument GetPrintDocument()
	    {
		    var document = new PrintDocument();
		    document.PrintPage += printDocument_PrintPage;
		    document.BeginPrint += printDocument_BeginPrint;
		    document.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
		    return document;
	    }

		private float _printPageCounter;
	    private List<string> _printText;
	    private Font _printFont;
		private void printDocument_BeginPrint(object sender, PrintEventArgs e)
	    {
		    var textEditorWindow = editorTabs.SelectedTab as TextEditorWindow;
		    if (textEditorWindow == null)
		    {
			    e.Cancel = true;
			    return;
		    }
		    var texteditor = textEditorWindow.TextEditor;
		    _printText = texteditor.Text.Split('\n').ToList();
		    _printFont = texteditor.Font;

			_printPageCounter = 0;
	    }

		private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
			// Crappy method for guessing line height, but who prints code anyway?
			var lineHeight = e.Graphics.MeasureString("t\nt\nt\nt\nt\nt\nt\nt", _printFont).Height / 8;
	        var lines = Math.Min(_printText.Count, (int)(e.MarginBounds.Height / lineHeight));

            e.Graphics.DrawString(string.Join("\n", _printText.Take(lines)), _printFont, Brushes.Black, e.MarginBounds, new StringFormat());
            e.Graphics.PageUnit = GraphicsUnit.Inch;

	        _printText.RemoveRange(0, lines);
			_printPageCounter++;
	        if (_printText.Count > 0) e.HasMorePages = true;
        }



        public void AddRecentProjects()
        {
	        recentProjectsMenuItem.DropDownItems.Clear();
			foreach (var projectPath in Settings.RecentProjects)
	        {
		        var fileInfo = new FileInfo(projectPath);
		        recentProjectsMenuItem.DropDownItems.Add(
			        string.Format(@"{0} ({1})", fileInfo.Name, fileInfo.FullName), null, 
			        (sender, args) => LoadProject(fileInfo.FullName));
	        }

	        recentProjectsMenuItem.Enabled = (recentProjectsMenuItem.DropDownItems.Count > 0);
        }
		
	    private void GoTo(string file, int line, int character, int? length = null)
	    {
		    if (CurrentProject == null) return;
		    var goToFile = CurrentProject.Files.SingleOrDefault(f => f.File.FullName == file);
		    if (goToFile == null) throw new Exception("Could not find file in project: " + file);
		    var tab = OpenFileInTab(goToFile) as TextEditorWindow;
			if (tab != null) tab.TextEditor.GoToWordAt(line, character, length);
		}

		private void LoadProject(string projectFilename)
		{
			// Check if there are unsaved changes, and give a chance to abort close
			if (CurrentProject != null && !CloseCurrentProject()) return;

			/*var loadingForm = new LoadWindow();
			loadingForm.StartPosition = FormStartPosition.CenterScreen;
			loadingForm.ShowImmediately(this);*/
			SetStatus("Loading project...");
			Refresh();

			AsmProject project;
			try
			{
				project = AsmProject.LoadFromFile(projectFilename);
			}
			catch (Exception ex)
			{
				Error("Failed opening file:\n" + ex.Message);
				//loadingForm.Close();
				SetStatus();
				return;
			}
			SuspendLayout();

			_moduleEvents.SetProjectType(project.Type);
			LoadEmulator(project.Type);
			project.GoTo = (file, length, ch) => GoTo(file, length, ch);
			project.BreakpointsChanged += ThreadSafeBreakpointHandler;

			AddFileToRecentProjects(projectFilename);

			ProjectExplorer.SetProject(project);
			//CartridgeExplorer.RefreshTree();
			CurrentProject = project;
			LoadProjectState();

			//loadingForm.Close();
			RefreshView();
			RefreshConfigurationList();
			SetStatus("Parsing debug symbols...");
			ResumeLayout();

			Task.WhenAll(
				BuildHandler.ParseDebugDataAsync(CurrentProject),
				CurrentProject.LoadAllSymbolsAsync()).ContinueWith((t) =>
			{
				ThreadSafeBreakpointHandler(CurrentProject.GetAllBreakpoints());
			}).ContinueWith(task => SetStatus());
		}

	    private void RefreshConfigurationList()
	    {
			configurationSelector.Items.Clear();
		    if (CurrentProject == null) return;
		    foreach (var configuration in CurrentProject.BuildConfigurations)
		    {
			    configurationSelector.Items.Add(configuration);
			    if (CurrentProject.CurrentConfiguration == configuration) configurationSelector.SelectedItem = configuration;
		    }
		    configurationSelector.Items.Add("Create new...");
		}
	    private void ChangeConfiguration()
	    {
			var configuration = configurationSelector.SelectedItem as NesCartridge;
		    if (configuration == null)
		    {
			    using (var newConfigurationWindow = new ConfigurationManager(CurrentProject))
			    {
				    newConfigurationWindow.StartPosition = FormStartPosition.CenterParent;
				    var result = newConfigurationWindow.ShowDialog();
				    if (result == DialogResult.OK)
				    {
					    configuration = newConfigurationWindow.Configuration;

						CurrentProject.BuildConfigurations.Add(configuration);
					    CurrentProject.Pristine = false;
					    configurationSelector.Items.Insert(configurationSelector.Items.Count - 1, configuration);
					    configurationSelector.SelectedItem = configuration;

				    }
				    else
				    {
					    configurationSelector.SelectedItem = CurrentProject.CurrentConfiguration;
					    return;
				    }
			    }
			}

		    CurrentProject.CurrentConfiguration = configuration;
	    }

		private List<Breakpoint> allBreakpoints = new List<Breakpoint>();
	    private void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
	    {
			allBreakpoints = breakpoints.ToList();

		    foreach (var breakpoint in allBreakpoints.Where(bp => bp.Symbol != null)) breakpoint.UpdateFromSymbols(CurrentProject.DebugSymbols);
			if (Mesen.Emulator != null) Mesen.Emulator.SetBreakpoints(allBreakpoints.Where(bp => !bp.Broken && !bp.Disabled));
		    BreakpointList.SetBreakpoints(allBreakpoints);
		    MemoryTabs.Cpu.SetBreakpoints(allBreakpoints.Where(bp => bp.AddressType == Breakpoint.AddressTypes.Cpu));
		    MemoryTabs.Ppu.SetBreakpoints(allBreakpoints.Where(bp => bp.AddressType == Breakpoint.AddressTypes.Ppu));
		    MemoryTabs.Oam.SetBreakpoints(allBreakpoints.Where(bp => bp.AddressType == Breakpoint.AddressTypes.Oam));

		    foreach (var editor in editorTabs.TabPages.OfType<TextEditorWindow>()) editor.RefreshEditorBreakpoints();
		}

		private bool CloseCurrentProject(bool closingApplication = false)
	    {
		    if (CurrentProject == null) return true;
			SuspendLayout();
			SaveProjectState();
			if (!editorTabs.CloseAll())
			{
				ResumeLayout();
				return false;
			}
		    if (!CurrentProject.Pristine)
		    {
			    var choice = MessageBox.Show(this, "Do you want to save changes to '" + CurrentProject.Name + "' before closing?", "Project changed", MessageBoxButtons.YesNoCancel);
				if (choice != DialogResult.Yes && choice != DialogResult.No)
				{
					ResumeLayout();
					return false;
				}
			    if (choice == DialogResult.Yes) SaveCurrentProject(false);
		    }

			UnloadEmulator();
			ProjectExplorer.Nodes.Clear();
			//CartridgeExplorer.Nodes.Clear();
		    editorTabs.TabPages.Clear();
		    WatchValues.Clear();
		    configurationSelector.Items.Clear();
			SetBreakpoints(new List<Breakpoint>());
			CurrentProject.Dispose();
		    if (!closingApplication)
		    {
			    CurrentProject = null;
			    Settings.CurrentProject = null;
		    }
		    Settings.Save();
			RefreshView();
			ResumeLayout();
		    return true;
	    }

		private void AddFileToRecentProjects(string projectFilename)
	    {
			Settings.CurrentProject = projectFilename;
			Settings.RecentProjects.Remove(projectFilename);
		    Settings.RecentProjects.Insert(0, projectFilename);
		    Settings.RecentProjects = Settings.RecentProjects.Take(10).ToList();
		    Settings.Save();
		    AddRecentProjects();

			try
			{
				OsFeatures.AddFileToRecent(projectFilename);
			}
			catch (Exception) { }
		}

		private void SaveProjectState()
		{
			if (CurrentProject == null) return;
			Settings.SetProjectState(CurrentProject, 
				editorTabs.TabPages.OfType<EditorWindow>(), 
				WatchValues.GetSerializableData(),
				allBreakpoints);
			Settings.Save();
		}
		private void LoadProjectState()
		{
			// TODO: Load-logic in settings class
			if (CurrentProject == null || CurrentProject.ProjectFile == null) return;
			var savedState = Settings.ProjectStates.FirstOrDefault(s => s.Filename == CurrentProject.ProjectFile.FullName);
			if (savedState == null) return;

			if (savedState.WatchData != null) WatchValues.SetWatchValues(savedState.WatchData);
			
			if (savedState.Breakpoints != null)
			{
				CurrentProject.SetBreakpoints(savedState.Breakpoints.Where(bp => bp.File == null).Select(bp => bp.GetBreakpoint()));
				foreach (var breakpointFile in savedState.Breakpoints.Where(bp => bp.File != null).Select(bp => bp.File).Distinct())
				{
					var file = CurrentProject.Files.FirstOrDefault(f => f.GetRelativePath() == breakpointFile);
					if (file == null) continue;

					file.SetEditorBreakpoints(savedState.Breakpoints.Where(bp => bp.File == breakpointFile).Select(bp => bp.GetBreakpoint(file)));
				}
			}

			if (savedState.CurrentConfiguration != null)
				CurrentProject.CurrentConfiguration = CurrentProject.BuildConfigurations.FirstOrDefault(c => c.Name == savedState.CurrentConfiguration);
			if (savedState.OpenFiles != null)
			foreach (var filename in savedState.OpenFiles)
			{
				var projectFile = CurrentProject.Files.FirstOrDefault(f => f.File.FullName == filename);
				if (projectFile == null) continue;
				OpenFileInTab(projectFile, true);
			}
		}

		private void File_ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

	    public void Cut()
	    {
		    if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
		    textEditorWindow.TextEditor.Cut(this, null);
	    }
		public void Copy()
	    {
		    if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
		    textEditorWindow.TextEditor.Copy(this, null);
		}
		public void Paste()
	    {
		    if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
		    textEditorWindow.TextEditor.Paste(this, null);
		}
		public void Delete()
	    {
		    if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
		    textEditorWindow.TextEditor.ActiveTextAreaControl.TextArea.InsertString("");
	    }
	    public void SelectAll()
	    {
		    if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
		    textEditorWindow.TextEditor.SelectAll(this, null);
		}


		private void Edit_CutMenuItem_Click(object s, EventArgs e)
        {
	        _moduleEvents.Cut();
        }

        private void Edit_CopyMenuItem_Click(object s, EventArgs e)
        {
	        _moduleEvents.Copy();
        }

        private void Edit_PasteMenuItem_Click(object s, EventArgs e)
        {
	        _moduleEvents.Paste();
        }

	    private void Edit_UndoMenuItem_Click(object sender, EventArgs e)
        {
			if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
	        textEditorWindow.TextEditor.Undo();
        }

	    private void Edit_RedoMenuItem_Click(object sender, EventArgs e)
        {
	        if (!(editorTabs.SelectedTab is TextEditorWindow textEditorWindow)) return;
	        textEditorWindow.TextEditor.Redo();
        }

		private void Edit_FindMenuItem_Click(object sender, EventArgs e)
        {
			FindInFiles(FindMode.FindInCurrentFile);
        }
	    private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
	    {
		    FindNext();
	    }

	    private void Edit_GoToMenuItem_Click(object sender, EventArgs e)
	    {
		    GoToLine();
	    }

		private void Edit_SelectAllMenuItem_Click(object s, EventArgs e)
        {
	        _moduleEvents.SelectAll();
        }

		private void Edit_ReplaceMenuItem_Click(object sender, EventArgs e)
	    {
			FindInFiles(FindMode.Replace);
		}
	    private void findInFilesMenuItem_Click(object sender, EventArgs e)
	    {
		    FindInFiles(FindMode.FindInAllFiles);
	    }


		private void statusBarMenuItem_Click(object sender, EventArgs e)
        {
			Settings.ShowStatusBar = statusBar.Visible = viewStatusBarMenuItem.Checked;
	        Settings.Save();
        }
		private void toolbarMenuItem_Click(object sender, EventArgs e)
        {
			Settings.ShowToolbar = toolstrippanel.Visible = MainToolStrip.Visible = viewToolbarMenuItem.Checked;
	        Settings.Save();
        }

        private void fullScreenMenuItem_Click(object sender, EventArgs e)
        {
			WindowState = FormWindowState.Normal;
			FormBorderStyle = fullScreenMenuItem.Checked ? FormBorderStyle.None : FormBorderStyle.Sizable;
			if (fullScreenMenuItem.Checked) WindowState = FormWindowState.Maximized;
		}

		private void lineNumbersMenuItem_Click(object sender, EventArgs e)
        {
	        TextEditor.DefaultCodeProperties.ShowLineNumbers = Settings.ShowLineNumbers = viewLineNumbersMenuItem.Checked;
			if (editorTabs.SelectedTab is TextEditorWindow textEditorWindow)
			    textEditorWindow.TextEditor.ActiveTextAreaControl.TextArea.Invalidate();

			Settings.Save();
        }
		private void lineAddressMappingsMenuItem_Click(object sender, EventArgs e)
		{
			Settings.ShowLineAddresses = lineAddressMappingsMenuItem.Checked;
			foreach (var textEditorWindow in editorTabs.TabPages.OfType<TextEditorWindow>())
				if (textEditorWindow.TextEditor is Ca65Editor asmEditor)
					asmEditor.ShowCpuAddresses = Settings.ShowLineAddresses;

			Settings.Save();
		}



		private Task<bool> BuildProject(bool force = true)
	    {
		    if (!force && !CurrentProject.UpdatedSinceLastBuild)
		    {
			    return Task.FromResult(true);
		    }

		    SaveAll();
			if (!CurrentProject.Files.Any(l => l.Mode == CompileMode.LinkerConfig))
			{
				Error("Error: No linker configuration was found.\nEither create a new one from a template, or add an existing file.");
			}
			return BuildHandler.Build(CurrentProject);
		}

	    private void RefreshBreakpointsToLatestBuild()
	    {
		    foreach (var window in GetTextEditorWindows())
		    {
			    var codeEditor = window.TextEditor as CodeEditor;
			    if (codeEditor == null) continue;
				
			    codeEditor.UpdateBreakpointsWithBuildInfo();
			}
		}

		private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
	        var buildTask = BuildProject();
	        if (!Mesen.Emulator.IsRunning() || _currentFocus == null) return;

			// If emulator is running and an editor is currently focused on a code line, upload the new ROM to the running editor, and continue where we are
			buildTask.ContinueWith((t, o) =>
	        {
		        if (t.Status != TaskStatus.RanToCompletion || !t.Result) return;
		        CurrentProject.AwaitDebugTask();
		        var currentFocusLine = _currentFocus.GetFocusArrow();
		        if (currentFocusLine == null) return;
		        Mesen.LoadCartridgeAtSameState(CurrentProject.Directory.FullName, CurrentProject.Directory.FullName + @"\" + CurrentProject.CurrentConfiguration.Filename,
			        i => currentFocusLine.CpuAddress ?? i);
	        }, null);
		}

		private void Run_RunApplicationMenuItem_Click(object sender, EventArgs e)
        {
			Run();
        }
		private void SaveAll()
		{
			foreach (var tab in editorTabs.TabPages.OfType<ISaveable>())
			{
				tab.Save();
			}
			SaveCurrentProject();
		}
		private void Run()
		{
			Mesen.Focus();
			if (Mesen.Emulator.IsRunning())
			{
				// While running the game, treat the "run" button as a resume
				Mesen.Emulator.Resume();
				return;
			}

			var buildTask = BuildProject(false);
			buildTask.ContinueWith((t, o) =>
			{
				if (t.Status != TaskStatus.RanToCompletion || !t.Result) return;
				var romFile = CurrentProject.Directory.FullName + @"\" + CurrentProject.CurrentConfiguration.Filename;
				if (!File.Exists(romFile))
				{
					Error(string.Format("Could not find the ROM file: {0}. Did someone delete it?", romFile));
					return;
				}
				Mesen.LoadCartridge(CurrentProject.Directory.FullName, romFile);
			}, null);
		}


		private void runNewBuildToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BuildProject(true).ContinueWith((t, o) =>
			{
				if (t.Status != TaskStatus.RanToCompletion || !t.Result) return;
				Mesen.LoadCartridge(CurrentProject.Directory.FullName, CurrentProject.Directory.FullName + @"\" + CurrentProject.CurrentConfiguration.Filename);
			}, null);
		}
	    private void continueWithNewBuildToolStripMenuItem_Click(object sender, EventArgs e)
	    {
			BuildProject(true).ContinueWith((t, o) =>
		    {
			    if (t.Status != TaskStatus.RanToCompletion || !t.Result) return;
				CurrentProject.AwaitDebugTask();
			    var currentFocusLine = _currentFocus.GetFocusArrow();
			    if (currentFocusLine == null) return;
				Mesen.LoadCartridgeAtSameState(CurrentProject.Directory.FullName, CurrentProject.Directory.FullName + @"\" + CurrentProject.CurrentConfiguration.Filename, 
				    i => currentFocusLine.CpuAddress ?? i);
		    }, null);
	    }

		private void Run_RunAppletMenuItem_Click(object sender, EventArgs e)
        {
			Mesen.Emulator.Pause();
		}
		
        private void StopMenuItem_Click(object sender, EventArgs e)
        {
			Mesen.Emulator.Stop();
		}

        private void Window_CloseAllWindowsMenuItem_Click(object sender, EventArgs e)
        {
            File_CloseAllMenuItem.PerformClick();
        }


        private void Help_ViewHelpTopicsMenuItem_Click(object sender, EventArgs e)
        {
			throw new NotImplementedException();
        }
        private void Help_AboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }

        private void JavaApplicationProject_ToolStripButton_Click(object sender, EventArgs e)
        {
            newNesProjectMenuItem_Click(sender, e);
        }

	    private void OpenProject_ToolStripButton_Click(object sender, EventArgs e)
	    {
		    File_OpenProjectMenuItem.PerformClick();
	    }

		private void Save_ToolStripButton_Click(object sender, EventArgs e)
        {
			File_SaveMenuItem.PerformClick();
		}

        private void SaveAs_ToolStripButton_Click(object sender, EventArgs e)
        {
            File_SaveAsMenuItem_Click(sender, e);
        }

        private void SaveAll_ToolStripButton_Click(object sender, EventArgs e)
        {
            File_SaveAllMenuItem_Click(sender, e);
        }

        private void Cut_ToolStripButton_Click(object sender, EventArgs e)
        {
            Edit_CutMenuItem_Click(sender, e);
        }

        private void Copy_ToolStripButton_Click(object sender, EventArgs e)
        {
            Edit_CopyMenuItem_Click(sender, e);
        }

        private void Paste_ToolStripButton_Click(object sender, EventArgs e)
        {
            Edit_PasteMenuItem_Click(sender, e);
        }

        private void Undo_ToolStripButton_Click(object sender, EventArgs e)
        {
            Edit_UndoMenuItem_Click(sender, e);
        }

        private void Redo_ToolStripButton_Click(object sender, EventArgs e)
        {
            Edit_RedoMenuItem_Click(sender, e);
        }

        private void Compile_ToolStripButton_Click(object sender, EventArgs e)
        {
            buildProjectMenuItem.PerformClick();
        }

        private void Run_ToolStripButton_Click(object sender, EventArgs e)
        {
            runMenuItem.PerformClick();
        }
	    private void continueWithNewBuild_Click(object sender, EventArgs e)
	    {
		    continueWithNewBuildMenuItem.PerformClick();
	    }


		private void RunApplet_ToolStripButton_Click(object sender, EventArgs e)
        {
            Run_RunAppletMenuItem_Click(sender, e);
        }
		
	    private IEnumerable<TextEditorWindow> GetTextEditorWindows()
	    {
		    return editorTabs.TabPages.Cast<TabPage>().OfType<TextEditorWindow>();
	    }

	    private EditorWindow OpenFileInTab(AsmProjectFile file, bool openInBackground = false)
	    {
			var tab = editorTabs.TabPages.Cast<TabPage>().OfType<EditorWindow>().SingleOrDefault(w => w.ProjectFile == file);
			if (tab == null)
			{
				if (!File.Exists(file.File.FullName))
				{
					Error("Could not find the file: " + file.File.FullName);
					return null;
				}

				if (file.IsTextFile)
				{
					var textEditor = new TextEditorWindow(this, file, _moduleEvents);
					textEditor.RefreshEditorContents();
					tab = textEditor;
				}
				else
					switch (file.Type)
					{
						case FileType.Image:
							tab = new ImageWindow(this, file, _moduleEvents);
							break;
						default:
							// TODO: "Open with..." dialog
							var recommendedPrograms = OsFeatures.RecommendedPrograms(file.File.Extension);
							if (!recommendedPrograms.Any())
							{
								Error("No program identified for this file type");
								return null;
							}

							var command = recommendedPrograms[0].Open;
							var arguments = file.File.FullName;
							var matches = Regex.Match(command,
								@"^(.*?)(?:\s+(?=(?:[^\""]*\""[^\""]*\"")*[^\""]*$))(.*)$");
							if (matches.Success)
							{
								command = matches.Groups[1].Value.Replace("%1", file.File.FullName);
								arguments = matches.Groups[2].Value.Replace("%1", file.File.FullName);
							}

							Process.Start(command, arguments);
							return null;
					}

				if (tab is TextEditorWindow textEditorTab)
				{
					if (textEditorTab.TextEditor is Ca65Editor asmEditor) asmEditor.ShowCpuAddresses = Settings.ShowLineAddresses;
					textEditorTab.TextEditor.ActiveTextAreaControl.Caret.PositionChanged += (s, a) => SetCaretInformation(textEditorTab);
				}


				var saveable = tab as ISaveable;
				if (saveable != null) saveable.PristineChanged += ActiveFileChanged;
				editorTabs.TabPages.Add(tab);
			}
			if (openInBackground) return tab;

			editorTabs.SelectedTab = tab;
		    return tab;
	    }

	    private void runNewBuild_Click(object sender, EventArgs e)
		{
			runNewBuildMenuItem.PerformClick();
		}
		private void pause_Click(object sender, EventArgs e)
		{
			pauseMenuItem.PerformClick();
		}

		private void stop_Click(object sender, EventArgs e)
		{
			stopMenuItem.PerformClick();
		}

		private void restart_Click(object sender, EventArgs e)
		{
			restartMenuItem.PerformClick();
		}

		private void stepOver_Click(object sender, EventArgs e)
		{
			stepOverMenuItem.PerformClick();
		}

		private void stepInto_Click(object sender, EventArgs e)
		{
			stepIntoMenuItem.PerformClick();
		}

		private void stepOut_Click(object sender, EventArgs e)
		{
			stepOutMenuItem.PerformClick();
		}

		private void stepBack_Click(object sender, EventArgs e)
		{
			stepBackMenuItem.PerformClick();
		}

		private void stepOverMenuItem_Click(object sender, EventArgs e)
		{
			if (!Mesen.Emulator.IsRunning()) return;
			Mesen.Emulator.StepOver();
		}

		private void stepIntoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!Mesen.Emulator.IsRunning()) return;
			Mesen.Emulator.StepInto();
		}

		private void stepOutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!Mesen.Emulator.IsRunning()) return;
			Mesen.Emulator.StepOut();
		}

		private void stepBackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!Mesen.Emulator.IsRunning()) return;
			Mesen.Emulator.StepBack();
		}

	    public void AddWatch(string expression, bool word)
	    {
		    WatchValues.AddWatch(expression, word);
	    }

		private void restartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Mesen.Emulator.Restart();
		}

		private void flashCartridge_Click(object sender, EventArgs e)
		{
			flashCartridgeMenuItem.PerformClick();
		}

		private void settingsMenuItem_Click(object sender, EventArgs e)
		{
			ShowSettingsWindow();
		}
	    private void mapInputToolStripMenuItem_Click(object sender, EventArgs e)
	    {
		    if (CurrentProject == null) return;
		    using (var keyBindings = new KeyBindingWindow())
		    {
			    var mappings = CurrentProject.Type == ProjectType.Snes ? Settings.SnesMappings : Settings.NesMappings;
			    keyBindings.StartPosition = FormStartPosition.CenterParent;
			    keyBindings.KeyBindingSettings.SetMappings(mappings.Select(m => m.Clone()).ToList());
			    if (keyBindings.ShowDialog(this) != DialogResult.OK) return;

			    var newMappings = keyBindings.KeyBindingSettings.Mappings.Select(m => m.Clone()).ToList();
			    switch (CurrentProject.Type)
			    {
					case ProjectType.Nes:
						Settings.NesMappings = newMappings;
						break;
					case ProjectType.Snes:
					    Settings.SnesMappings = newMappings;
					    break;
			    }
			    Mesen.SetButtonMappings(Settings.NesMappings, Settings.SnesMappings);
				Settings.Save();
			}

		}
		private void emulatorSettingsMenuItem_Click(object sender, EventArgs e)
	    {
		    ShowSettingsWindow(3);
	    }

		private void ShowSettingsWindow(int defaultTab = 0)
	    {
		    using (var settingsWindow = new SettingsWindow(Settings))
		    {
			    settingsWindow.StartPosition = FormStartPosition.CenterParent;
			    settingsWindow.SelectedTab = defaultTab;

				if (settingsWindow.ShowDialog() != DialogResult.OK) return;
		    }

		    foreach (var textEditorWindow in editorTabs.TabPages.Cast<TabPage>().OfType<TextEditorWindow>())
		    {
			    var ca65highlighting = textEditorWindow.TextEditor.Document.HighlightingStrategy as Ca65Highlighting;
			    if (ca65highlighting == null) continue;
			    ca65highlighting.UpdateColorsFromDefault();
			    ca65highlighting.MarkTokens(textEditorWindow.TextEditor.Document, textEditorWindow.TextEditor.Document.LineSegmentCollection.ToList());
			}
		    Mesen.SetButtonMappings(Settings.NesMappings, Settings.SnesMappings);
		    Mesen.EmulatorBackgroundColor = Settings.EmuBackgroundColor;
			updateEveryFrameMenuItem.Checked = (Mesen.UpdateRate = Settings.UpdateRate) == 1;
		}
		private void buildSettingsMenuItem_Click(object sender, EventArgs e)
	    {
		    if (CurrentProject == null) return;
		    using (var settingsWindow = new ProjectSettingsWindow(CurrentProject))
		    {
			    settingsWindow.StartPosition = FormStartPosition.CenterParent;
			    settingsWindow.ShowDialog();
		    }

		    RefreshView();
			RefreshConfigurationList();
	    }

		private void updateEveryFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Mesen.UpdateRate = Settings.UpdateRate = updateEveryFrameMenuItem.Checked ? 1 : 30;
			Settings.Save();
		}
	    private void integerScalingToolStripMenuItem_Click(object sender, EventArgs e)
	    {
		    Mesen.IntegerScaling = Settings.EmuIntegerScaling = integerScalingMenuItem.Checked;
		    Settings.Save();
	    }

		private void flashCartridgeMenuItem_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void sourceFileMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentProject == null) return;
			CreateNewFile(CurrentProject.ProjectFile.DirectoryName, FileTemplate.AssemblyCode, ".s");
		}

		private void includeFileMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentProject == null) return;
			CreateNewFile(CurrentProject.ProjectFile.DirectoryName, FileTemplate.AssemblyInclude, ".inc");
		}

		private void importProjectMenuItem_Click(object sender, EventArgs e)
		{
			ImportExistingProject();
		}

		private void buildSettings_Click(object sender, EventArgs e)
		{
			buildSettingsMenuItem.PerformClick();
		}

		private void randomValuesAtPowerOnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuRandomPowerOn = Mesen.RandomPowerOnState = randomValuesAtPowerOnMenuItem.Checked;
			Settings.Save();
		}

		private void displayBGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplayNesBg = Mesen.ShowBgLayer = displayNesBgMenuItem.Checked;
			Settings.Save();
		}

		private void displayObjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplaySprites = Mesen.ShowSpriteLayer = displaySnesSpritesMenuItem.Checked = displayNesObjectsMenuItem.Checked;
			Settings.Save();
		}
	    private void displaySpritesToolStripMenuItem_Click(object sender, EventArgs e)
	    {
		    Settings.EmuDisplaySprites = Mesen.ShowSpriteLayer = displayNesObjectsMenuItem.Checked = displaySnesSpritesMenuItem.Checked;
		    Settings.Save();
		}


		private void playPulse1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayPulse1 = Mesen.PlaySquare1 = playPulse1MenuItem.Checked;
			Settings.Save();
		}

		private void playPulse2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayPulse2 = Mesen.PlaySquare2 = playPulse2MenuItem.Checked;
			Settings.Save();
		}

		private void playTriangleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayTriangle = Mesen.PlayTriangle = playTriangleMenuItem.Checked;
			Settings.Save();
		}

		private void playNoiseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayNoise = Mesen.PlayNoise = playNoiseMenuItem.Checked;
			Settings.Save();
		}

		private void playPCMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayPcm = Mesen.PlayPcm = playPcmMenuItem.Checked;
			Settings.Save();
		}

		private void playAudioToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayAudio = Mesen.PlayAudio = playSpcAudioMenuItem.Checked = playAudioMenuItem.Checked;
			Settings.Save();
		}

		private void playSPCAudioToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuPlayAudio = Mesen.PlayAudio = playAudioMenuItem.Checked = playSpcAudioMenuItem.Checked;
			Settings.Save();
		}

		private void displayBGLayer1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplaySnesBg1 = Mesen.ShowBgLayer1 = displayBgLayer1MenuItem.Checked;
			Settings.Save();
		}

		private void displayBGLayer2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplaySnesBg2 = Mesen.ShowBgLayer2 = displayBgLayer2MenuItem.Checked;
			Settings.Save();
		}

		private void displayBGLayer3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplaySnesBg3 = Mesen.ShowBgLayer3 = displayBgLayer3MenuItem.Checked;
			Settings.Save();
		}

		private void displayBGLayer4ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmuDisplaySnesBg4 = Mesen.ShowBgLayer4 = displayBgLayer4MenuItem.Checked;
			Settings.Save();
		}

		private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Mesen.SaveState();
		}

		private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Mesen.LoadState();
		}

		private void newProjectToolStripButton_Click(object sender, EventArgs e)
		{
			nesProjectMenuItem.PerformClick();
		}

		private void goToAllMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentProject == null) return;
			var goToAll = new GoToAll(CurrentProject, _moduleEvents);
			goToAll.Show(this);
		}

		private void resetLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SuspendLayout();
			StoredLayoutHandler.LoadPanelLayout(_modules);
			ResumeLayout(true);
		}
	}

}
