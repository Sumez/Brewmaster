using System.Windows.Forms;
using Brewmaster.EditorWindows;
using Brewmaster.Modules.Ppu;

namespace Brewmaster
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
			UnloadEmulator();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Panel availableIdePanels;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
			this.editorTabs = new Brewmaster.EditorWindows.EditorTabs();
			this.mesen = new Brewmaster.Emulation.MesenControl();
			this.cpuStatus1 = new Brewmaster.StatusView.CpuStatus();
			this.TileMap = new TileMapViewer();
			this.MemoryTabs = new System.Windows.Forms.TabControl();
			this.CpuMemoryTab = new System.Windows.Forms.TabPage();
			this.CpuMemoryViewer = new Brewmaster.MemoryViewer.MemoryViewer();
			this.PpuMemoryTab = new System.Windows.Forms.TabPage();
			this.PpuMemoryViewer = new Brewmaster.MemoryViewer.MemoryViewer();
			this.OamMemoryTab = new System.Windows.Forms.TabPage();
			this.OamMemoryViewer = new Brewmaster.MemoryViewer.MemoryViewer();
			this.CartridgeExplorer = new Brewmaster.CartridgeExplorer.CartridgeExplorer();
			this.WatchPanel = new Brewmaster.Ide.IdeGroupedPanel();
			this.OutputPanel = new Brewmaster.Ide.IdeGroupedPanel();
			this.ProjectExplorer = new Brewmaster.ProjectExplorer.ProjectExplorer();
			this.ProjectExplorerImageList = new System.Windows.Forms.ImageList(this.components);
			this.MainWindowMenu = new System.Windows.Forms.MenuStrip();
			this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_NewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nesProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.snesProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sourceFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.includeFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tileMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.graphicsDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.File_OpenProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.File_SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_SaveAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.File_CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_CloseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_CloseProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.File_PrintMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.File_PrintPreviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
			this.recentProjectsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.File_ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.EditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.Edit_UndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Edit_RedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.findMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findInFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.Edit_GoToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator49 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertMacroMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolbarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewStatusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator35 = new System.Windows.Forms.ToolStripSeparator();
			this.fullScreenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator34 = new System.Windows.Forms.ToolStripSeparator();
			this.appearanceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.viewLineNumbersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lineAddressMappingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.BuildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runNewBuildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.continueWithNewBuildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.restartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flashCartridgeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			this.stepOverMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepIntoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stepBackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
			this.buildSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.EmulatorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateEveryFrameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.randomValuesAtPowerOnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.integerScalingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nesGraphicsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayNesBgMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayNesObjectsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nesAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playPulse1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playPulse2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playTriangleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playNoiseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playPcmMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.snesGraphicsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayBgLayer1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayBgLayer2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayBgLayer3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displayBgLayer4MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.displaySnesSpritesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.snesAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playSpcAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mapInputMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.emulatorSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.WindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeAllWindowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.WindowMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewHelpTopicsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolstrippanel = new System.Windows.Forms.FlowLayoutPanel();
			this.MainToolStrip = new System.Windows.Forms.ToolStrip();
			this.newProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.openProjectToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveAllToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.redoToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.configurationSelector = new System.Windows.Forms.ToolStripComboBox();
			this.buildSettings = new System.Windows.Forms.ToolStripButton();
			this.build = new System.Windows.Forms.ToolStripButton();
			this.runNewBuild = new System.Windows.Forms.ToolStripButton();
			this.continueWithNewBuild = new System.Windows.Forms.ToolStripButton();
			this.run = new System.Windows.Forms.ToolStripButton();
			this.pause = new System.Windows.Forms.ToolStripButton();
			this.stop = new System.Windows.Forms.ToolStripButton();
			this.restart = new System.Windows.Forms.ToolStripButton();
			this.flashCartridge = new System.Windows.Forms.ToolStripButton();
			this.stepOver = new System.Windows.Forms.ToolStripButton();
			this.stepInto = new System.Windows.Forms.ToolStripButton();
			this.stepOut = new System.Windows.Forms.ToolStripButton();
			this.stepBack = new System.Windows.Forms.ToolStripButton();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.filenameLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.lineLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.charLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.fpsLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.New_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_Class_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_Package_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_Interface_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_Enums_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_HTMLFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_CSSFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_TextFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_JavaScriptFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_SQLFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_XMLFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.New_NewFile_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator41 = new System.Windows.Forms.ToolStripSeparator();
			this.Open_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenwithSystemEditor_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator42 = new System.Windows.Forms.ToolStripSeparator();
			this.Delete_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator43 = new System.Windows.Forms.ToolStripSeparator();
			this.CloseProject_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator44 = new System.Windows.Forms.ToolStripSeparator();
			this.RemoveAddedFiles_ProjExplorerContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.OpenProjectFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.OpenFilesFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.CreateNewFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.CodeItemImages = new System.Windows.Forms.ImageList(this.components);
			this.MainEastContainer2 = new Brewmaster.Ide.MultiSplitContainer();
			this._menuHelper = new Brewmaster.Modules.MenuHelper(this.components);
			availableIdePanels = new System.Windows.Forms.Panel();
			toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
			availableIdePanels.SuspendLayout();
			this.MemoryTabs.SuspendLayout();
			this.CpuMemoryTab.SuspendLayout();
			this.PpuMemoryTab.SuspendLayout();
			this.OamMemoryTab.SuspendLayout();
			this.MainWindowMenu.SuspendLayout();
			this.toolstrippanel.SuspendLayout();
			this.MainToolStrip.SuspendLayout();
			this.statusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// availableIdePanels
			// 
			availableIdePanels.Controls.Add(this.editorTabs);
			availableIdePanels.Controls.Add(this.mesen);
			availableIdePanels.Controls.Add(this.cpuStatus1);
			availableIdePanels.Controls.Add(this.TileMap);
			availableIdePanels.Controls.Add(this.MemoryTabs);
			availableIdePanels.Controls.Add(this.CartridgeExplorer);
			availableIdePanels.Controls.Add(this.WatchPanel);
			availableIdePanels.Controls.Add(this.OutputPanel);
			availableIdePanels.Controls.Add(this.ProjectExplorer);
			availableIdePanels.Location = new System.Drawing.Point(10, 80);
			availableIdePanels.Name = "availableIdePanels";
			availableIdePanels.Size = new System.Drawing.Size(856, 450);
			availableIdePanels.TabIndex = 3;
			availableIdePanels.Visible = false;
			// 
			// editorTabs
			// 
			this.editorTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.editorTabs.HotTrack = true;
			this.editorTabs.Location = new System.Drawing.Point(0, 0);
			this.editorTabs.Name = "editorTabs";
			this.editorTabs.Padding = new System.Drawing.Point(30, 4);
			this.editorTabs.SelectedIndex = 0;
			this.editorTabs.ShowToolTips = true;
			this.editorTabs.Size = new System.Drawing.Size(856, 450);
			this.editorTabs.TabIndex = 0;
			this.editorTabs.TextColor = System.Drawing.Color.Navy;
			this.editorTabs.TextColorInactive = System.Drawing.Color.Navy;
			// 
			// mesen
			// 
			this.mesen.BackColor = System.Drawing.Color.Black;
			this.mesen.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mesen.IntegerScaling = false;
			this.mesen.Location = new System.Drawing.Point(0, 0);
			this.mesen.Margin = new System.Windows.Forms.Padding(0);
			this.mesen.Name = "mesen";
			this.mesen.Size = new System.Drawing.Size(856, 450);
			this.mesen.TabIndex = 1;
			// 
			// cpuStatus1
			// 
			this.cpuStatus1.AutoScroll = true;
			this.cpuStatus1.AutoSize = true;
			this.cpuStatus1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cpuStatus1.Location = new System.Drawing.Point(0, 0);
			this.cpuStatus1.MinimumSize = new System.Drawing.Size(275, 0);
			this.cpuStatus1.ModuleEvents = null;
			this.cpuStatus1.Name = "cpuStatus1";
			this.cpuStatus1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.cpuStatus1.Size = new System.Drawing.Size(856, 450);
			this.cpuStatus1.TabIndex = 2;
			// 
			// TileMap
			// 
			this.TileMap.AutoSize = true;
			this.TileMap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TileMap.FitImage = false;
			this.TileMap.Location = new System.Drawing.Point(0, 0);
			this.TileMap.MinimumSize = new System.Drawing.Size(275, 0);
			this.TileMap.Name = "TileMap";
			this.TileMap.ShowScrollOverlay = false;
			this.TileMap.Size = new System.Drawing.Size(856, 450);
			this.TileMap.TabIndex = 2;
			// 
			// MemoryTabs
			// 
			this.MemoryTabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.MemoryTabs.Controls.Add(this.CpuMemoryTab);
			this.MemoryTabs.Controls.Add(this.PpuMemoryTab);
			this.MemoryTabs.Controls.Add(this.OamMemoryTab);
			this.MemoryTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MemoryTabs.Location = new System.Drawing.Point(0, 0);
			this.MemoryTabs.Name = "MemoryTabs";
			this.MemoryTabs.SelectedIndex = 0;
			this.MemoryTabs.Size = new System.Drawing.Size(856, 450);
			this.MemoryTabs.TabIndex = 2;
			// 
			// CpuMemoryTab
			// 
			this.CpuMemoryTab.Controls.Add(this.CpuMemoryViewer);
			this.CpuMemoryTab.Location = new System.Drawing.Point(4, 4);
			this.CpuMemoryTab.Name = "CpuMemoryTab";
			this.CpuMemoryTab.Size = new System.Drawing.Size(848, 424);
			this.CpuMemoryTab.TabIndex = 0;
			this.CpuMemoryTab.Text = "CPU";
			this.CpuMemoryTab.UseVisualStyleBackColor = true;
			// 
			// CpuMemoryViewer
			// 
			this.CpuMemoryViewer.BackColor = System.Drawing.SystemColors.Control;
			this.CpuMemoryViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CpuMemoryViewer.Font = new System.Drawing.Font("Consolas", 10F);
			this.CpuMemoryViewer.Location = new System.Drawing.Point(3, 3);
			this.CpuMemoryViewer.Name = "CpuMemoryViewer";
			this.CpuMemoryViewer.Size = new System.Drawing.Size(842, 418);
			this.CpuMemoryViewer.TabIndex = 0;
			// 
			// PpuMemoryTab
			// 
			this.PpuMemoryTab.Controls.Add(this.PpuMemoryViewer);
			this.PpuMemoryTab.Location = new System.Drawing.Point(4, 4);
			this.PpuMemoryTab.Name = "PpuMemoryTab";
			this.PpuMemoryTab.Size = new System.Drawing.Size(848, 424);
			this.PpuMemoryTab.TabIndex = 1;
			this.PpuMemoryTab.Text = "PPU";
			this.PpuMemoryTab.UseVisualStyleBackColor = true;
			// 
			// PpuMemoryViewer
			// 
			this.PpuMemoryViewer.BackColor = System.Drawing.SystemColors.Control;
			this.PpuMemoryViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PpuMemoryViewer.Font = new System.Drawing.Font("Consolas", 10F);
			this.PpuMemoryViewer.Location = new System.Drawing.Point(3, 3);
			this.PpuMemoryViewer.Name = "PpuMemoryViewer";
			this.PpuMemoryViewer.Size = new System.Drawing.Size(842, 418);
			this.PpuMemoryViewer.TabIndex = 0;
			// 
			// OamMemoryTab
			// 
			this.OamMemoryTab.Controls.Add(this.OamMemoryViewer);
			this.OamMemoryTab.Location = new System.Drawing.Point(4, 4);
			this.OamMemoryTab.Name = "OamMemoryTab";
			this.OamMemoryTab.Size = new System.Drawing.Size(848, 424);
			this.OamMemoryTab.TabIndex = 2;
			this.OamMemoryTab.Text = "OAM";
			this.OamMemoryTab.UseVisualStyleBackColor = true;
			// 
			// OamMemoryViewer
			// 
			this.OamMemoryViewer.BackColor = System.Drawing.SystemColors.Control;
			this.OamMemoryViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OamMemoryViewer.Font = new System.Drawing.Font("Consolas", 10F);
			this.OamMemoryViewer.Location = new System.Drawing.Point(3, 3);
			this.OamMemoryViewer.Name = "OamMemoryViewer";
			this.OamMemoryViewer.Size = new System.Drawing.Size(842, 418);
			this.OamMemoryViewer.TabIndex = 0;
			// 
			// CartridgeExplorer
			// 
			this.CartridgeExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.CartridgeExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CartridgeExplorer.ImageIndex = 0;
			this.CartridgeExplorer.ItemHeight = 20;
			this.CartridgeExplorer.Location = new System.Drawing.Point(0, 0);
			this.CartridgeExplorer.Name = "CartridgeExplorer";
			this.CartridgeExplorer.SelectedImageIndex = 0;
			this.CartridgeExplorer.ShowLines = false;
			this.CartridgeExplorer.Size = new System.Drawing.Size(856, 450);
			this.CartridgeExplorer.TabIndex = 2;
			// 
			// WatchPanel
			// 
			this.WatchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.WatchPanel.GroupParent = null;
			this.WatchPanel.Label = "test";
			this.WatchPanel.Location = new System.Drawing.Point(0, 0);
			this.WatchPanel.Name = "WatchPanel";
			this.WatchPanel.ShowHeader = true;
			this.WatchPanel.Size = new System.Drawing.Size(856, 450);
			this.WatchPanel.TabIndex = 0;
			// 
			// OutputPanel
			// 
			this.OutputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OutputPanel.GroupParent = null;
			this.OutputPanel.Label = "test";
			this.OutputPanel.Location = new System.Drawing.Point(0, 0);
			this.OutputPanel.Name = "OutputPanel";
			this.OutputPanel.ShowHeader = true;
			this.OutputPanel.Size = new System.Drawing.Size(856, 450);
			this.OutputPanel.TabIndex = 2;
			// 
			// ProjectExplorer
			// 
			this.ProjectExplorer.AddExistingFile = null;
			this.ProjectExplorer.AllowDrop = true;
			this.ProjectExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ProjectExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ProjectExplorer.ImageIndex = 9;
			this.ProjectExplorer.ImageList = this.ProjectExplorerImageList;
			this.ProjectExplorer.ItemHeight = 20;
			this.ProjectExplorer.LabelEdit = true;
			this.ProjectExplorer.Location = new System.Drawing.Point(0, 0);
			this.ProjectExplorer.Name = "ProjectExplorer";
			this.ProjectExplorer.SelectedImageIndex = 0;
			this.ProjectExplorer.ShowLines = false;
			this.ProjectExplorer.ShowNodeToolTips = true;
			this.ProjectExplorer.Size = new System.Drawing.Size(856, 450);
			this.ProjectExplorer.TabIndex = 2;
			// 
			// ProjectExplorerImageList
			// 
			this.ProjectExplorerImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectExplorerImageList.ImageStream")));
			this.ProjectExplorerImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.ProjectExplorerImageList.Images.SetKeyName(0, "classfile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(1, "cssfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(2, "htmlfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(3, "imagefile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(4, "jarfile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(5, "javafile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(6, "javascriptfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(7, "newfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(8, "packagejavafile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(9, "nesproject.png");
			this.ProjectExplorerImageList.Images.SetKeyName(10, "sqlfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(11, "textfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(12, "xmlfile2.png");
			this.ProjectExplorerImageList.Images.SetKeyName(13, "cppfile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(14, "headerfile.png");
			this.ProjectExplorerImageList.Images.SetKeyName(15, "folder-dark.png");
			this.ProjectExplorerImageList.Images.SetKeyName(16, "file.png");
			this.ProjectExplorerImageList.Images.SetKeyName(17, "image.png");
			this.ProjectExplorerImageList.Images.SetKeyName(18, "text.png");
			this.ProjectExplorerImageList.Images.SetKeyName(19, "data.png");
			// 
			// toolStripSeparator14
			// 
			toolStripSeparator14.Name = "toolStripSeparator14";
			toolStripSeparator14.Size = new System.Drawing.Size(205, 6);
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(202, 6);
			// 
			// toolStripSeparator25
			// 
			toolStripSeparator25.Name = "toolStripSeparator25";
			toolStripSeparator25.Size = new System.Drawing.Size(137, 6);
			// 
			// toolStripSeparator26
			// 
			toolStripSeparator26.Name = "toolStripSeparator26";
			toolStripSeparator26.Size = new System.Drawing.Size(202, 6);
			// 
			// toolStripSeparator30
			// 
			toolStripSeparator30.Name = "toolStripSeparator30";
			toolStripSeparator30.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator22
			// 
			toolStripSeparator22.Name = "toolStripSeparator22";
			toolStripSeparator22.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator31
			// 
			toolStripSeparator31.Name = "toolStripSeparator31";
			toolStripSeparator31.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator16
			// 
			toolStripSeparator16.Name = "toolStripSeparator16";
			toolStripSeparator16.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator19
			// 
			toolStripSeparator19.Name = "toolStripSeparator19";
			toolStripSeparator19.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator23
			// 
			toolStripSeparator23.Name = "toolStripSeparator23";
			toolStripSeparator23.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator13
			// 
			toolStripSeparator13.Name = "toolStripSeparator13";
			toolStripSeparator13.Size = new System.Drawing.Size(214, 6);
			toolStripSeparator13.Visible = false;
			// 
			// toolStripSeparator27
			// 
			toolStripSeparator27.Name = "toolStripSeparator27";
			toolStripSeparator27.Size = new System.Drawing.Size(211, 6);
			toolStripSeparator27.Visible = false;
			// 
			// MainWindowMenu
			// 
			this.MainWindowMenu.BackColor = System.Drawing.SystemColors.MenuBar;
			this.MainWindowMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.EditMenuItem,
            this.ViewMenuItem,
            this.BuildMenuItem,
            this.EmulatorMenuItem,
            this.WindowMenuItem,
            this.HelpMenuItem});
			this.MainWindowMenu.Location = new System.Drawing.Point(0, 0);
			this.MainWindowMenu.Name = "MainWindowMenu";
			this.MainWindowMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.MainWindowMenu.Size = new System.Drawing.Size(1287, 24);
			this.MainWindowMenu.TabIndex = 1;
			// 
			// FileMenuItem
			// 
			this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.File_NewMenuItem,
            this.toolStripSeparator3,
            this.File_OpenProjectMenuItem,
            this.importProjectMenuItem,
            this.toolStripSeparator5,
            this.File_SaveMenuItem,
            this.File_SaveAsMenuItem,
            this.File_SaveAllMenuItem,
            this.toolStripSeparator6,
            this.File_CloseMenuItem,
            this.File_CloseAllMenuItem,
            this.File_CloseProjectMenuItem,
            this.toolStripSeparator7,
            this.File_PrintMenuItem,
            this.File_PrintPreviewMenuItem,
            this.toolStripSeparator8,
            this.settingsMenuItem,
            this.toolStripSeparator24,
            this.recentProjectsMenuItem,
            this.toolStripSeparator9,
            this.File_ExitMenuItem});
			this.FileMenuItem.Name = "FileMenuItem";
			this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
			this.FileMenuItem.Text = "File";
			// 
			// File_NewMenuItem
			// 
			this.File_NewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nesProjectMenuItem,
            this.snesProjectMenuItem,
            this.toolStripSeparator1,
            this.sourceFileMenuItem,
            this.includeFileMenuItem,
            this.toolStripSeparator2,
            this.tileMapMenuItem,
            this.graphicsDataMenuItem});
			this.File_NewMenuItem.Name = "File_NewMenuItem";
			this.File_NewMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_NewMenuItem.Text = "New                                                      ";
			// 
			// nesProjectMenuItem
			// 
			this.nesProjectMenuItem.Name = "nesProjectMenuItem";
			this.nesProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.nesProjectMenuItem.Size = new System.Drawing.Size(187, 22);
			this.nesProjectMenuItem.Text = "NES Project...";
			this.nesProjectMenuItem.Click += new System.EventHandler(this.newNesProjectMenuItem_Click);
			// 
			// snesProjectMenuItem
			// 
			this.snesProjectMenuItem.Name = "snesProjectMenuItem";
			this.snesProjectMenuItem.Size = new System.Drawing.Size(187, 22);
			this.snesProjectMenuItem.Text = "SNES Project...";
			this.snesProjectMenuItem.Click += new System.EventHandler(this.newSnesProjectMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(184, 6);
			// 
			// sourceFileMenuItem
			// 
			this.sourceFileMenuItem.Name = "sourceFileMenuItem";
			this.sourceFileMenuItem.Size = new System.Drawing.Size(187, 22);
			this.sourceFileMenuItem.Text = "Source File...";
			this.sourceFileMenuItem.Click += new System.EventHandler(this.sourceFileMenuItem_Click);
			// 
			// includeFileMenuItem
			// 
			this.includeFileMenuItem.Name = "includeFileMenuItem";
			this.includeFileMenuItem.Size = new System.Drawing.Size(187, 22);
			this.includeFileMenuItem.Text = "Include File...";
			this.includeFileMenuItem.Click += new System.EventHandler(this.includeFileMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(184, 6);
			this.toolStripSeparator2.Visible = false;
			// 
			// tileMapMenuItem
			// 
			this.tileMapMenuItem.Name = "tileMapMenuItem";
			this.tileMapMenuItem.Size = new System.Drawing.Size(187, 22);
			this.tileMapMenuItem.Text = "Tile Map";
			this.tileMapMenuItem.Visible = false;
			// 
			// graphicsDataMenuItem
			// 
			this.graphicsDataMenuItem.Name = "graphicsDataMenuItem";
			this.graphicsDataMenuItem.Size = new System.Drawing.Size(187, 22);
			this.graphicsDataMenuItem.Text = "Graphics Data";
			this.graphicsDataMenuItem.Visible = false;
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(257, 6);
			// 
			// File_OpenProjectMenuItem
			// 
			this.File_OpenProjectMenuItem.Image = global::Brewmaster.Properties.Resources.open;
			this.File_OpenProjectMenuItem.Name = "File_OpenProjectMenuItem";
			this.File_OpenProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.File_OpenProjectMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_OpenProjectMenuItem.Text = "Open Project...";
			this.File_OpenProjectMenuItem.Click += new System.EventHandler(this.File_OpenProjectMenuItem_Click);
			// 
			// importProjectMenuItem
			// 
			this.importProjectMenuItem.Name = "importProjectMenuItem";
			this.importProjectMenuItem.Size = new System.Drawing.Size(260, 22);
			this.importProjectMenuItem.Text = "Import Existing Project...";
			this.importProjectMenuItem.Click += new System.EventHandler(this.importProjectMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(257, 6);
			// 
			// File_SaveMenuItem
			// 
			this.File_SaveMenuItem.Image = global::Brewmaster.Properties.Resources.save1;
			this.File_SaveMenuItem.Name = "File_SaveMenuItem";
			this.File_SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.File_SaveMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_SaveMenuItem.Text = "Save";
			this.File_SaveMenuItem.Click += new System.EventHandler(this.File_SaveMenuItem_Click);
			// 
			// File_SaveAsMenuItem
			// 
			this.File_SaveAsMenuItem.Image = global::Brewmaster.Properties.Resources.save1;
			this.File_SaveAsMenuItem.Name = "File_SaveAsMenuItem";
			this.File_SaveAsMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_SaveAsMenuItem.Text = "Save As...";
			this.File_SaveAsMenuItem.Click += new System.EventHandler(this.File_SaveAsMenuItem_Click);
			// 
			// File_SaveAllMenuItem
			// 
			this.File_SaveAllMenuItem.Image = global::Brewmaster.Properties.Resources.saveAll;
			this.File_SaveAllMenuItem.Name = "File_SaveAllMenuItem";
			this.File_SaveAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.File_SaveAllMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_SaveAllMenuItem.Text = "Save All";
			this.File_SaveAllMenuItem.Click += new System.EventHandler(this.File_SaveAllMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(257, 6);
			// 
			// File_CloseMenuItem
			// 
			this.File_CloseMenuItem.Name = "File_CloseMenuItem";
			this.File_CloseMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_CloseMenuItem.Text = "Close";
			this.File_CloseMenuItem.Click += new System.EventHandler(this.File_CloseMenuItem_Click);
			// 
			// File_CloseAllMenuItem
			// 
			this.File_CloseAllMenuItem.Name = "File_CloseAllMenuItem";
			this.File_CloseAllMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_CloseAllMenuItem.Text = "Close All";
			this.File_CloseAllMenuItem.Click += new System.EventHandler(this.File_CloseAllMenuItem_Click);
			// 
			// File_CloseProjectMenuItem
			// 
			this.File_CloseProjectMenuItem.Name = "File_CloseProjectMenuItem";
			this.File_CloseProjectMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_CloseProjectMenuItem.Text = "Close Project";
			this.File_CloseProjectMenuItem.Click += new System.EventHandler(this.File_CloseProjectMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(257, 6);
			// 
			// File_PrintMenuItem
			// 
			this.File_PrintMenuItem.Name = "File_PrintMenuItem";
			this.File_PrintMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.File_PrintMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_PrintMenuItem.Text = "Print...";
			this.File_PrintMenuItem.Click += new System.EventHandler(this.File_PrintMenuItem_Click);
			// 
			// File_PrintPreviewMenuItem
			// 
			this.File_PrintPreviewMenuItem.Name = "File_PrintPreviewMenuItem";
			this.File_PrintPreviewMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_PrintPreviewMenuItem.Text = "Print Preview...";
			this.File_PrintPreviewMenuItem.Click += new System.EventHandler(this.File_PrintPreviewMenuItem_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(257, 6);
			// 
			// settingsMenuItem
			// 
			this.settingsMenuItem.Name = "settingsMenuItem";
			this.settingsMenuItem.Size = new System.Drawing.Size(260, 22);
			this.settingsMenuItem.Text = "Settings...";
			this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
			// 
			// toolStripSeparator24
			// 
			this.toolStripSeparator24.Name = "toolStripSeparator24";
			this.toolStripSeparator24.Size = new System.Drawing.Size(257, 6);
			// 
			// recentProjectsMenuItem
			// 
			this.recentProjectsMenuItem.Name = "recentProjectsMenuItem";
			this.recentProjectsMenuItem.Size = new System.Drawing.Size(260, 22);
			this.recentProjectsMenuItem.Text = "Recent Projects";
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(257, 6);
			// 
			// File_ExitMenuItem
			// 
			this.File_ExitMenuItem.Name = "File_ExitMenuItem";
			this.File_ExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.File_ExitMenuItem.Size = new System.Drawing.Size(260, 22);
			this.File_ExitMenuItem.Text = "Exit";
			this.File_ExitMenuItem.Click += new System.EventHandler(this.File_ExitMenuItem_Click);
			// 
			// EditMenuItem
			// 
			this.EditMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Edit_CutMenuItem,
            this.Edit_CopyMenuItem,
            this.Edit_PasteMenuItem,
            this.toolStripSeparator10,
            this.Edit_UndoMenuItem,
            this.Edit_RedoMenuItem,
            this.toolStripSeparator11,
            this.findMenuItem,
            this.findInFilesMenuItem,
            this.findNextMenuItem,
            this.replaceMenuItem,
            this.toolStripSeparator12,
            this.Edit_GoToMenuItem,
            this.toolStripSeparator49,
            this.selectAllMenuItem,
            toolStripSeparator13,
            this.insertMenuItem});
			this.EditMenuItem.Name = "EditMenuItem";
			this.EditMenuItem.Size = new System.Drawing.Size(39, 20);
			this.EditMenuItem.Text = "Edit";
			// 
			// Edit_CutMenuItem
			// 
			this.Edit_CutMenuItem.Image = global::Brewmaster.Properties.Resources.cut1;
			this.Edit_CutMenuItem.Name = "Edit_CutMenuItem";
			this.Edit_CutMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_CutMenuItem.Text = "Cut                           ";
			this.Edit_CutMenuItem.Click += new System.EventHandler(this.Edit_CutMenuItem_Click);
			// 
			// Edit_CopyMenuItem
			// 
			this.Edit_CopyMenuItem.Image = global::Brewmaster.Properties.Resources.copy1;
			this.Edit_CopyMenuItem.Name = "Edit_CopyMenuItem";
			this.Edit_CopyMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
			this.Edit_CopyMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_CopyMenuItem.Text = "Copy";
			this.Edit_CopyMenuItem.Click += new System.EventHandler(this.Edit_CopyMenuItem_Click);
			// 
			// Edit_PasteMenuItem
			// 
			this.Edit_PasteMenuItem.Image = global::Brewmaster.Properties.Resources.paste1;
			this.Edit_PasteMenuItem.Name = "Edit_PasteMenuItem";
			this.Edit_PasteMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
			this.Edit_PasteMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_PasteMenuItem.Text = "Paste";
			this.Edit_PasteMenuItem.Click += new System.EventHandler(this.Edit_PasteMenuItem_Click);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(214, 6);
			// 
			// Edit_UndoMenuItem
			// 
			this.Edit_UndoMenuItem.Image = global::Brewmaster.Properties.Resources.undo1;
			this.Edit_UndoMenuItem.Name = "Edit_UndoMenuItem";
			this.Edit_UndoMenuItem.ShortcutKeyDisplayString = "Ctrl+Z";
			this.Edit_UndoMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_UndoMenuItem.Text = "Undo";
			this.Edit_UndoMenuItem.Click += new System.EventHandler(this.Edit_UndoMenuItem_Click);
			// 
			// Edit_RedoMenuItem
			// 
			this.Edit_RedoMenuItem.Image = global::Brewmaster.Properties.Resources.redo1;
			this.Edit_RedoMenuItem.Name = "Edit_RedoMenuItem";
			this.Edit_RedoMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
			this.Edit_RedoMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_RedoMenuItem.Text = "Redo";
			this.Edit_RedoMenuItem.Click += new System.EventHandler(this.Edit_RedoMenuItem_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(214, 6);
			// 
			// findMenuItem
			// 
			this.findMenuItem.Name = "findMenuItem";
			this.findMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findMenuItem.Size = new System.Drawing.Size(217, 22);
			this.findMenuItem.Text = "Find...";
			this.findMenuItem.Click += new System.EventHandler(this.Edit_FindMenuItem_Click);
			// 
			// findInFilesMenuItem
			// 
			this.findInFilesMenuItem.Name = "findInFilesMenuItem";
			this.findInFilesMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
			this.findInFilesMenuItem.Size = new System.Drawing.Size(217, 22);
			this.findInFilesMenuItem.Text = "Find in Files...";
			this.findInFilesMenuItem.Click += new System.EventHandler(this.findInFilesMenuItem_Click);
			// 
			// findNextMenuItem
			// 
			this.findNextMenuItem.Name = "findNextMenuItem";
			this.findNextMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.findNextMenuItem.Size = new System.Drawing.Size(217, 22);
			this.findNextMenuItem.Text = "Find Next";
			this.findNextMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
			// 
			// replaceMenuItem
			// 
			this.replaceMenuItem.Name = "replaceMenuItem";
			this.replaceMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.replaceMenuItem.Size = new System.Drawing.Size(217, 22);
			this.replaceMenuItem.Text = "Replace";
			this.replaceMenuItem.Click += new System.EventHandler(this.Edit_ReplaceMenuItem_Click);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(214, 6);
			// 
			// Edit_GoToMenuItem
			// 
			this.Edit_GoToMenuItem.Name = "Edit_GoToMenuItem";
			this.Edit_GoToMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.Edit_GoToMenuItem.Size = new System.Drawing.Size(217, 22);
			this.Edit_GoToMenuItem.Text = "Go To";
			this.Edit_GoToMenuItem.Click += new System.EventHandler(this.Edit_GoToMenuItem_Click);
			// 
			// toolStripSeparator49
			// 
			this.toolStripSeparator49.Name = "toolStripSeparator49";
			this.toolStripSeparator49.Size = new System.Drawing.Size(214, 6);
			// 
			// selectAllMenuItem
			// 
			this.selectAllMenuItem.Name = "selectAllMenuItem";
			this.selectAllMenuItem.ShortcutKeyDisplayString = "Ctrl+A";
			this.selectAllMenuItem.Size = new System.Drawing.Size(217, 22);
			this.selectAllMenuItem.Text = "Select All";
			this.selectAllMenuItem.Click += new System.EventHandler(this.Edit_SelectAllMenuItem_Click);
			// 
			// insertMenuItem
			// 
			this.insertMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertMacroMenuItem});
			this.insertMenuItem.Name = "insertMenuItem";
			this.insertMenuItem.Size = new System.Drawing.Size(217, 22);
			this.insertMenuItem.Text = "Insert";
			this.insertMenuItem.Visible = false;
			// 
			// insertMacroMenuItem
			// 
			this.insertMacroMenuItem.Name = "insertMacroMenuItem";
			this.insertMacroMenuItem.Size = new System.Drawing.Size(108, 22);
			this.insertMacroMenuItem.Text = "Macro";
			// 
			// ViewMenuItem
			// 
			this.ViewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolbarMenuItem,
            this.viewStatusBarMenuItem,
            this.toolStripSeparator35,
            this.fullScreenMenuItem,
            this.toolStripSeparator34,
            this.appearanceMenuItem,
            this.toolStripSeparator17,
            this.viewLineNumbersMenuItem,
            this.lineAddressMappingsMenuItem,
            this.toolStripSeparator18});
			this.ViewMenuItem.Name = "ViewMenuItem";
			this.ViewMenuItem.Size = new System.Drawing.Size(44, 20);
			this.ViewMenuItem.Text = "View";
			// 
			// viewToolbarMenuItem
			// 
			this.viewToolbarMenuItem.Checked = true;
			this.viewToolbarMenuItem.CheckOnClick = true;
			this.viewToolbarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.viewToolbarMenuItem.Name = "viewToolbarMenuItem";
			this.viewToolbarMenuItem.Size = new System.Drawing.Size(197, 22);
			this.viewToolbarMenuItem.Text = "Toolbar";
			this.viewToolbarMenuItem.Click += new System.EventHandler(this.toolbarMenuItem_Click);
			// 
			// viewStatusBarMenuItem
			// 
			this.viewStatusBarMenuItem.Checked = true;
			this.viewStatusBarMenuItem.CheckOnClick = true;
			this.viewStatusBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.viewStatusBarMenuItem.Name = "viewStatusBarMenuItem";
			this.viewStatusBarMenuItem.Size = new System.Drawing.Size(197, 22);
			this.viewStatusBarMenuItem.Text = "Status Bar";
			this.viewStatusBarMenuItem.Click += new System.EventHandler(this.statusBarMenuItem_Click);
			// 
			// toolStripSeparator35
			// 
			this.toolStripSeparator35.Name = "toolStripSeparator35";
			this.toolStripSeparator35.Size = new System.Drawing.Size(194, 6);
			// 
			// fullScreenMenuItem
			// 
			this.fullScreenMenuItem.CheckOnClick = true;
			this.fullScreenMenuItem.Name = "fullScreenMenuItem";
			this.fullScreenMenuItem.Size = new System.Drawing.Size(197, 22);
			this.fullScreenMenuItem.Text = "Full Screen";
			this.fullScreenMenuItem.Click += new System.EventHandler(this.fullScreenMenuItem_Click);
			// 
			// toolStripSeparator34
			// 
			this.toolStripSeparator34.Name = "toolStripSeparator34";
			this.toolStripSeparator34.Size = new System.Drawing.Size(194, 6);
			// 
			// appearanceMenuItem
			// 
			this.appearanceMenuItem.Enabled = false;
			this.appearanceMenuItem.Name = "appearanceMenuItem";
			this.appearanceMenuItem.Size = new System.Drawing.Size(197, 22);
			this.appearanceMenuItem.Text = "Appearance";
			// 
			// toolStripSeparator17
			// 
			this.toolStripSeparator17.Name = "toolStripSeparator17";
			this.toolStripSeparator17.Size = new System.Drawing.Size(194, 6);
			// 
			// viewLineNumbersMenuItem
			// 
			this.viewLineNumbersMenuItem.Checked = true;
			this.viewLineNumbersMenuItem.CheckOnClick = true;
			this.viewLineNumbersMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.viewLineNumbersMenuItem.Name = "viewLineNumbersMenuItem";
			this.viewLineNumbersMenuItem.Size = new System.Drawing.Size(197, 22);
			this.viewLineNumbersMenuItem.Text = "Line Numbers";
			this.viewLineNumbersMenuItem.Click += new System.EventHandler(this.lineNumbersMenuItem_Click);
			// 
			// lineAddressMappingsMenuItem
			// 
			this.lineAddressMappingsMenuItem.CheckOnClick = true;
			this.lineAddressMappingsMenuItem.Name = "lineAddressMappingsMenuItem";
			this.lineAddressMappingsMenuItem.Size = new System.Drawing.Size(197, 22);
			this.lineAddressMappingsMenuItem.Text = "Line Address Mappings";
			this.lineAddressMappingsMenuItem.Click += new System.EventHandler(this.lineAddressMappingsMenuItem_Click);
			// 
			// toolStripSeparator18
			// 
			this.toolStripSeparator18.Name = "toolStripSeparator18";
			this.toolStripSeparator18.Size = new System.Drawing.Size(194, 6);
			// 
			// BuildMenuItem
			// 
			this.BuildMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildProjectMenuItem,
            this.runNewBuildMenuItem,
            this.continueWithNewBuildMenuItem,
            this.runMenuItem,
            this.pauseMenuItem,
            this.stopMenuItem,
            this.restartMenuItem,
            toolStripSeparator14,
            this.flashCartridgeMenuItem,
            this.toolStripSeparator20,
            this.stepOverMenuItem,
            this.stepIntoMenuItem,
            this.stepOutMenuItem,
            this.stepBackMenuItem,
            this.toolStripSeparator21,
            this.buildSettingsMenuItem});
			this.BuildMenuItem.Name = "BuildMenuItem";
			this.BuildMenuItem.Size = new System.Drawing.Size(46, 20);
			this.BuildMenuItem.Text = "Build";
			// 
			// buildProjectMenuItem
			// 
			this.buildProjectMenuItem.Image = global::Brewmaster.Properties.Resources.build;
			this.buildProjectMenuItem.Name = "buildProjectMenuItem";
			this.buildProjectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
			this.buildProjectMenuItem.Size = new System.Drawing.Size(208, 22);
			this.buildProjectMenuItem.Text = "Build";
			this.buildProjectMenuItem.Click += new System.EventHandler(this.buildToolStripMenuItem_Click);
			// 
			// runNewBuildMenuItem
			// 
			this.runNewBuildMenuItem.Name = "runNewBuildMenuItem";
			this.runNewBuildMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
			this.runNewBuildMenuItem.Size = new System.Drawing.Size(208, 22);
			this.runNewBuildMenuItem.Text = "Run New Build";
			this.runNewBuildMenuItem.Click += new System.EventHandler(this.runNewBuildToolStripMenuItem_Click);
			// 
			// continueWithNewBuildMenuItem
			// 
			this.continueWithNewBuildMenuItem.Name = "continueWithNewBuildMenuItem";
			this.continueWithNewBuildMenuItem.Size = new System.Drawing.Size(208, 22);
			this.continueWithNewBuildMenuItem.Text = "Continue With New Build";
			this.continueWithNewBuildMenuItem.Visible = false;
			this.continueWithNewBuildMenuItem.Click += new System.EventHandler(this.continueWithNewBuildToolStripMenuItem_Click);
			// 
			// runMenuItem
			// 
			this.runMenuItem.Image = global::Brewmaster.Properties.Resources.run;
			this.runMenuItem.Name = "runMenuItem";
			this.runMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.runMenuItem.Size = new System.Drawing.Size(208, 22);
			this.runMenuItem.Text = "Run";
			this.runMenuItem.Click += new System.EventHandler(this.Run_RunApplicationMenuItem_Click);
			// 
			// pauseMenuItem
			// 
			this.pauseMenuItem.Image = global::Brewmaster.Properties.Resources.pause;
			this.pauseMenuItem.Name = "pauseMenuItem";
			this.pauseMenuItem.Size = new System.Drawing.Size(208, 22);
			this.pauseMenuItem.Text = "Pause";
			this.pauseMenuItem.Click += new System.EventHandler(this.Run_RunAppletMenuItem_Click);
			// 
			// stopMenuItem
			// 
			this.stopMenuItem.Image = global::Brewmaster.Properties.Resources.stop1;
			this.stopMenuItem.Name = "stopMenuItem";
			this.stopMenuItem.Size = new System.Drawing.Size(208, 22);
			this.stopMenuItem.Text = "Stop";
			this.stopMenuItem.Click += new System.EventHandler(this.StopMenuItem_Click);
			// 
			// restartMenuItem
			// 
			this.restartMenuItem.Image = global::Brewmaster.Properties.Resources.restart;
			this.restartMenuItem.Name = "restartMenuItem";
			this.restartMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.restartMenuItem.Size = new System.Drawing.Size(208, 22);
			this.restartMenuItem.Text = "Restart";
			this.restartMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
			// 
			// flashCartridgeMenuItem
			// 
			this.flashCartridgeMenuItem.Name = "flashCartridgeMenuItem";
			this.flashCartridgeMenuItem.Size = new System.Drawing.Size(208, 22);
			this.flashCartridgeMenuItem.Text = "Flash Cartridge";
			this.flashCartridgeMenuItem.Click += new System.EventHandler(this.flashCartridgeMenuItem_Click);
			// 
			// toolStripSeparator20
			// 
			this.toolStripSeparator20.Name = "toolStripSeparator20";
			this.toolStripSeparator20.Size = new System.Drawing.Size(205, 6);
			// 
			// stepOverMenuItem
			// 
			this.stepOverMenuItem.Name = "stepOverMenuItem";
			this.stepOverMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
			this.stepOverMenuItem.Size = new System.Drawing.Size(208, 22);
			this.stepOverMenuItem.Text = "Step Over";
			this.stepOverMenuItem.Click += new System.EventHandler(this.stepOverMenuItem_Click);
			// 
			// stepIntoMenuItem
			// 
			this.stepIntoMenuItem.Name = "stepIntoMenuItem";
			this.stepIntoMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
			this.stepIntoMenuItem.Size = new System.Drawing.Size(208, 22);
			this.stepIntoMenuItem.Text = "Step Into";
			this.stepIntoMenuItem.Click += new System.EventHandler(this.stepIntoToolStripMenuItem_Click);
			// 
			// stepOutMenuItem
			// 
			this.stepOutMenuItem.Name = "stepOutMenuItem";
			this.stepOutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
			this.stepOutMenuItem.Size = new System.Drawing.Size(208, 22);
			this.stepOutMenuItem.Text = "Step Out";
			this.stepOutMenuItem.Click += new System.EventHandler(this.stepOutToolStripMenuItem_Click);
			// 
			// stepBackMenuItem
			// 
			this.stepBackMenuItem.Name = "stepBackMenuItem";
			this.stepBackMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
			this.stepBackMenuItem.Size = new System.Drawing.Size(208, 22);
			this.stepBackMenuItem.Text = "Step Back";
			this.stepBackMenuItem.Click += new System.EventHandler(this.stepBackToolStripMenuItem_Click);
			// 
			// toolStripSeparator21
			// 
			this.toolStripSeparator21.Name = "toolStripSeparator21";
			this.toolStripSeparator21.Size = new System.Drawing.Size(205, 6);
			// 
			// buildSettingsMenuItem
			// 
			this.buildSettingsMenuItem.Name = "buildSettingsMenuItem";
			this.buildSettingsMenuItem.Size = new System.Drawing.Size(208, 22);
			this.buildSettingsMenuItem.Text = "Build Settings...";
			this.buildSettingsMenuItem.Click += new System.EventHandler(this.buildSettingsMenuItem_Click);
			// 
			// EmulatorMenuItem
			// 
			this.EmulatorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateEveryFrameMenuItem,
            this.randomValuesAtPowerOnMenuItem,
            this.integerScalingMenuItem,
            this.nesGraphicsMenuItem,
            this.nesAudioMenuItem,
            this.snesGraphicsMenuItem,
            this.snesAudioMenuItem,
            toolStripSeparator26,
            this.saveStateMenuItem,
            this.loadStateMenuItem,
            toolStripSeparator4,
            this.mapInputMenuItem,
            this.emulatorSettingsMenuItem});
			this.EmulatorMenuItem.Name = "EmulatorMenuItem";
			this.EmulatorMenuItem.Size = new System.Drawing.Size(67, 20);
			this.EmulatorMenuItem.Text = "Emulator";
			// 
			// updateEveryFrameMenuItem
			// 
			this.updateEveryFrameMenuItem.Checked = true;
			this.updateEveryFrameMenuItem.CheckOnClick = true;
			this.updateEveryFrameMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.updateEveryFrameMenuItem.Name = "updateEveryFrameMenuItem";
			this.updateEveryFrameMenuItem.Size = new System.Drawing.Size(205, 22);
			this.updateEveryFrameMenuItem.Text = "Update Every Frame";
			this.updateEveryFrameMenuItem.Click += new System.EventHandler(this.updateEveryFrameToolStripMenuItem_Click);
			// 
			// randomValuesAtPowerOnMenuItem
			// 
			this.randomValuesAtPowerOnMenuItem.CheckOnClick = true;
			this.randomValuesAtPowerOnMenuItem.Name = "randomValuesAtPowerOnMenuItem";
			this.randomValuesAtPowerOnMenuItem.Size = new System.Drawing.Size(205, 22);
			this.randomValuesAtPowerOnMenuItem.Text = "Random Power-On State";
			this.randomValuesAtPowerOnMenuItem.Click += new System.EventHandler(this.randomValuesAtPowerOnToolStripMenuItem_Click);
			// 
			// integerScalingMenuItem
			// 
			this.integerScalingMenuItem.Checked = true;
			this.integerScalingMenuItem.CheckOnClick = true;
			this.integerScalingMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.integerScalingMenuItem.Name = "integerScalingMenuItem";
			this.integerScalingMenuItem.Size = new System.Drawing.Size(205, 22);
			this.integerScalingMenuItem.Text = "Integer Scaling";
			this.integerScalingMenuItem.Click += new System.EventHandler(this.integerScalingToolStripMenuItem_Click);
			// 
			// nesGraphicsMenuItem
			// 
			this.nesGraphicsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayNesBgMenuItem,
            this.displayNesObjectsMenuItem});
			this.nesGraphicsMenuItem.Name = "nesGraphicsMenuItem";
			this.nesGraphicsMenuItem.Size = new System.Drawing.Size(205, 22);
			this.nesGraphicsMenuItem.Text = "Graphics";
			// 
			// displayNesBgMenuItem
			// 
			this.displayNesBgMenuItem.Checked = true;
			this.displayNesBgMenuItem.CheckOnClick = true;
			this.displayNesBgMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayNesBgMenuItem.Name = "displayNesBgMenuItem";
			this.displayNesBgMenuItem.Size = new System.Drawing.Size(155, 22);
			this.displayNesBgMenuItem.Text = "Display BG";
			this.displayNesBgMenuItem.Click += new System.EventHandler(this.displayBGToolStripMenuItem_Click);
			// 
			// displayNesObjectsMenuItem
			// 
			this.displayNesObjectsMenuItem.Checked = true;
			this.displayNesObjectsMenuItem.CheckOnClick = true;
			this.displayNesObjectsMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayNesObjectsMenuItem.Name = "displayNesObjectsMenuItem";
			this.displayNesObjectsMenuItem.Size = new System.Drawing.Size(155, 22);
			this.displayNesObjectsMenuItem.Text = "Display Objects";
			this.displayNesObjectsMenuItem.Click += new System.EventHandler(this.displayObjectsToolStripMenuItem_Click);
			// 
			// nesAudioMenuItem
			// 
			this.nesAudioMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playAudioMenuItem,
            toolStripSeparator25,
            this.playPulse1MenuItem,
            this.playPulse2MenuItem,
            this.playTriangleMenuItem,
            this.playNoiseMenuItem,
            this.playPcmMenuItem});
			this.nesAudioMenuItem.Name = "nesAudioMenuItem";
			this.nesAudioMenuItem.Size = new System.Drawing.Size(205, 22);
			this.nesAudioMenuItem.Text = "Audio";
			// 
			// playAudioMenuItem
			// 
			this.playAudioMenuItem.Checked = true;
			this.playAudioMenuItem.CheckOnClick = true;
			this.playAudioMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playAudioMenuItem.Name = "playAudioMenuItem";
			this.playAudioMenuItem.Size = new System.Drawing.Size(140, 22);
			this.playAudioMenuItem.Text = "Play Audio";
			this.playAudioMenuItem.Click += new System.EventHandler(this.playAudioToolStripMenuItem_Click);
			// 
			// playPulse1MenuItem
			// 
			this.playPulse1MenuItem.Checked = true;
			this.playPulse1MenuItem.CheckOnClick = true;
			this.playPulse1MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playPulse1MenuItem.Name = "playPulse1MenuItem";
			this.playPulse1MenuItem.Size = new System.Drawing.Size(140, 22);
			this.playPulse1MenuItem.Text = "Play Pulse 1";
			this.playPulse1MenuItem.Click += new System.EventHandler(this.playPulse1ToolStripMenuItem_Click);
			// 
			// playPulse2MenuItem
			// 
			this.playPulse2MenuItem.Checked = true;
			this.playPulse2MenuItem.CheckOnClick = true;
			this.playPulse2MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playPulse2MenuItem.Name = "playPulse2MenuItem";
			this.playPulse2MenuItem.Size = new System.Drawing.Size(140, 22);
			this.playPulse2MenuItem.Text = "Play Pulse 2";
			this.playPulse2MenuItem.Click += new System.EventHandler(this.playPulse2ToolStripMenuItem_Click);
			// 
			// playTriangleMenuItem
			// 
			this.playTriangleMenuItem.Checked = true;
			this.playTriangleMenuItem.CheckOnClick = true;
			this.playTriangleMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playTriangleMenuItem.Name = "playTriangleMenuItem";
			this.playTriangleMenuItem.Size = new System.Drawing.Size(140, 22);
			this.playTriangleMenuItem.Text = "Play Triangle";
			this.playTriangleMenuItem.Click += new System.EventHandler(this.playTriangleToolStripMenuItem_Click);
			// 
			// playNoiseMenuItem
			// 
			this.playNoiseMenuItem.Checked = true;
			this.playNoiseMenuItem.CheckOnClick = true;
			this.playNoiseMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playNoiseMenuItem.Name = "playNoiseMenuItem";
			this.playNoiseMenuItem.Size = new System.Drawing.Size(140, 22);
			this.playNoiseMenuItem.Text = "Play Noise";
			this.playNoiseMenuItem.Click += new System.EventHandler(this.playNoiseToolStripMenuItem_Click);
			// 
			// playPcmMenuItem
			// 
			this.playPcmMenuItem.Checked = true;
			this.playPcmMenuItem.CheckOnClick = true;
			this.playPcmMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playPcmMenuItem.Name = "playPcmMenuItem";
			this.playPcmMenuItem.Size = new System.Drawing.Size(140, 22);
			this.playPcmMenuItem.Text = "Play PCM";
			this.playPcmMenuItem.Click += new System.EventHandler(this.playPCMToolStripMenuItem_Click);
			// 
			// snesGraphicsMenuItem
			// 
			this.snesGraphicsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayBgLayer1MenuItem,
            this.displayBgLayer2MenuItem,
            this.displayBgLayer3MenuItem,
            this.displayBgLayer4MenuItem,
            this.displaySnesSpritesMenuItem});
			this.snesGraphicsMenuItem.Name = "snesGraphicsMenuItem";
			this.snesGraphicsMenuItem.Size = new System.Drawing.Size(205, 22);
			this.snesGraphicsMenuItem.Text = "Graphics";
			// 
			// displayBgLayer1MenuItem
			// 
			this.displayBgLayer1MenuItem.Checked = true;
			this.displayBgLayer1MenuItem.CheckOnClick = true;
			this.displayBgLayer1MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayBgLayer1MenuItem.Name = "displayBgLayer1MenuItem";
			this.displayBgLayer1MenuItem.Size = new System.Drawing.Size(170, 22);
			this.displayBgLayer1MenuItem.Text = "Display BG Layer 1";
			this.displayBgLayer1MenuItem.Click += new System.EventHandler(this.displayBGLayer1ToolStripMenuItem_Click);
			// 
			// displayBgLayer2MenuItem
			// 
			this.displayBgLayer2MenuItem.Checked = true;
			this.displayBgLayer2MenuItem.CheckOnClick = true;
			this.displayBgLayer2MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayBgLayer2MenuItem.Name = "displayBgLayer2MenuItem";
			this.displayBgLayer2MenuItem.Size = new System.Drawing.Size(170, 22);
			this.displayBgLayer2MenuItem.Text = "Display BG Layer 2";
			this.displayBgLayer2MenuItem.Click += new System.EventHandler(this.displayBGLayer2ToolStripMenuItem_Click);
			// 
			// displayBgLayer3MenuItem
			// 
			this.displayBgLayer3MenuItem.Checked = true;
			this.displayBgLayer3MenuItem.CheckOnClick = true;
			this.displayBgLayer3MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayBgLayer3MenuItem.Name = "displayBgLayer3MenuItem";
			this.displayBgLayer3MenuItem.Size = new System.Drawing.Size(170, 22);
			this.displayBgLayer3MenuItem.Text = "Display BG Layer 3";
			this.displayBgLayer3MenuItem.Click += new System.EventHandler(this.displayBGLayer3ToolStripMenuItem_Click);
			// 
			// displayBgLayer4MenuItem
			// 
			this.displayBgLayer4MenuItem.Checked = true;
			this.displayBgLayer4MenuItem.CheckOnClick = true;
			this.displayBgLayer4MenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displayBgLayer4MenuItem.Name = "displayBgLayer4MenuItem";
			this.displayBgLayer4MenuItem.Size = new System.Drawing.Size(170, 22);
			this.displayBgLayer4MenuItem.Text = "Display BG Layer 4";
			this.displayBgLayer4MenuItem.Click += new System.EventHandler(this.displayBGLayer4ToolStripMenuItem_Click);
			// 
			// displaySnesSpritesMenuItem
			// 
			this.displaySnesSpritesMenuItem.Checked = true;
			this.displaySnesSpritesMenuItem.CheckOnClick = true;
			this.displaySnesSpritesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.displaySnesSpritesMenuItem.Name = "displaySnesSpritesMenuItem";
			this.displaySnesSpritesMenuItem.Size = new System.Drawing.Size(170, 22);
			this.displaySnesSpritesMenuItem.Text = "Display Sprites";
			this.displaySnesSpritesMenuItem.Click += new System.EventHandler(this.displaySpritesToolStripMenuItem_Click);
			// 
			// snesAudioMenuItem
			// 
			this.snesAudioMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playSpcAudioMenuItem});
			this.snesAudioMenuItem.Name = "snesAudioMenuItem";
			this.snesAudioMenuItem.Size = new System.Drawing.Size(205, 22);
			this.snesAudioMenuItem.Text = "Audio";
			// 
			// playSpcAudioMenuItem
			// 
			this.playSpcAudioMenuItem.Checked = true;
			this.playSpcAudioMenuItem.CheckOnClick = true;
			this.playSpcAudioMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.playSpcAudioMenuItem.Name = "playSpcAudioMenuItem";
			this.playSpcAudioMenuItem.Size = new System.Drawing.Size(155, 22);
			this.playSpcAudioMenuItem.Text = "Play SPC Audio";
			this.playSpcAudioMenuItem.Click += new System.EventHandler(this.playSPCAudioToolStripMenuItem_Click);
			// 
			// saveStateMenuItem
			// 
			this.saveStateMenuItem.Name = "saveStateMenuItem";
			this.saveStateMenuItem.Size = new System.Drawing.Size(205, 22);
			this.saveStateMenuItem.Text = "Save State";
			this.saveStateMenuItem.Click += new System.EventHandler(this.saveStateToolStripMenuItem_Click);
			// 
			// loadStateMenuItem
			// 
			this.loadStateMenuItem.Name = "loadStateMenuItem";
			this.loadStateMenuItem.Size = new System.Drawing.Size(205, 22);
			this.loadStateMenuItem.Text = "Load State";
			this.loadStateMenuItem.Click += new System.EventHandler(this.loadStateToolStripMenuItem_Click);
			// 
			// mapInputMenuItem
			// 
			this.mapInputMenuItem.Name = "mapInputMenuItem";
			this.mapInputMenuItem.Size = new System.Drawing.Size(205, 22);
			this.mapInputMenuItem.Text = "Map Input...";
			this.mapInputMenuItem.Click += new System.EventHandler(this.mapInputToolStripMenuItem_Click);
			// 
			// emulatorSettingsMenuItem
			// 
			this.emulatorSettingsMenuItem.Name = "emulatorSettingsMenuItem";
			this.emulatorSettingsMenuItem.Size = new System.Drawing.Size(205, 22);
			this.emulatorSettingsMenuItem.Text = "Emulator Settings...";
			this.emulatorSettingsMenuItem.Click += new System.EventHandler(this.emulatorSettingsMenuItem_Click);
			// 
			// WindowMenuItem
			// 
			this.WindowMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeAllWindowsMenuItem,
            this.WindowMenuSeparator});
			this.WindowMenuItem.Name = "WindowMenuItem";
			this.WindowMenuItem.Size = new System.Drawing.Size(63, 20);
			this.WindowMenuItem.Text = "Window";
			// 
			// closeAllWindowsMenuItem
			// 
			this.closeAllWindowsMenuItem.Name = "closeAllWindowsMenuItem";
			this.closeAllWindowsMenuItem.Size = new System.Drawing.Size(172, 22);
			this.closeAllWindowsMenuItem.Text = "Close All Windows";
			this.closeAllWindowsMenuItem.Click += new System.EventHandler(this.Window_CloseAllWindowsMenuItem_Click);
			// 
			// WindowMenuSeparator
			// 
			this.WindowMenuSeparator.Name = "WindowMenuSeparator";
			this.WindowMenuSeparator.Size = new System.Drawing.Size(169, 6);
			// 
			// HelpMenuItem
			// 
			this.HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpTopicsMenuItem,
            toolStripSeparator27,
            this.aboutMenuItem});
			this.HelpMenuItem.Name = "HelpMenuItem";
			this.HelpMenuItem.Size = new System.Drawing.Size(44, 20);
			this.HelpMenuItem.Text = "Help";
			// 
			// viewHelpTopicsMenuItem
			// 
			this.viewHelpTopicsMenuItem.Name = "viewHelpTopicsMenuItem";
			this.viewHelpTopicsMenuItem.Size = new System.Drawing.Size(214, 22);
			this.viewHelpTopicsMenuItem.Text = "View Help Topics                 ";
			this.viewHelpTopicsMenuItem.Visible = false;
			this.viewHelpTopicsMenuItem.Click += new System.EventHandler(this.Help_ViewHelpTopicsMenuItem_Click);
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Name = "aboutMenuItem";
			this.aboutMenuItem.Size = new System.Drawing.Size(214, 22);
			this.aboutMenuItem.Text = "About";
			this.aboutMenuItem.Click += new System.EventHandler(this.Help_AboutMenuItem_Click);
			// 
			// toolstrippanel
			// 
			this.toolstrippanel.BackColor = System.Drawing.SystemColors.MenuBar;
			this.toolstrippanel.Controls.Add(this.MainToolStrip);
			this.toolstrippanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolstrippanel.Location = new System.Drawing.Point(0, 24);
			this.toolstrippanel.Name = "toolstrippanel";
			this.toolstrippanel.Size = new System.Drawing.Size(1287, 26);
			this.toolstrippanel.TabIndex = 3;
			// 
			// MainToolStrip
			// 
			this.MainToolStrip.CanOverflow = false;
			this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripButton,
            this.openProjectToolStripButton,
            this.saveToolStripButton,
            this.saveAllToolStripButton,
            toolStripSeparator30,
            this.undoToolStripButton,
            this.redoToolStripButton,
            toolStripSeparator22,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            toolStripSeparator31,
            this.configurationSelector,
            this.buildSettings,
            toolStripSeparator16,
            this.build,
            this.runNewBuild,
            this.continueWithNewBuild,
            this.run,
            this.pause,
            this.stop,
            this.restart,
            toolStripSeparator19,
            this.flashCartridge,
            toolStripSeparator23,
            this.stepOver,
            this.stepInto,
            this.stepOut,
            this.stepBack});
			this.MainToolStrip.Location = new System.Drawing.Point(0, 0);
			this.MainToolStrip.Name = "MainToolStrip";
			this.MainToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.MainToolStrip.Size = new System.Drawing.Size(654, 25);
			this.MainToolStrip.TabIndex = 0;
			// 
			// newProjectToolStripButton
			// 
			this.newProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newProjectToolStripButton.Image = global::Brewmaster.Properties.Resources.newfile;
			this.newProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newProjectToolStripButton.Name = "newProjectToolStripButton";
			this.newProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.newProjectToolStripButton.Text = "New Project";
			this.newProjectToolStripButton.ToolTipText = "New Project";
			this.newProjectToolStripButton.Click += new System.EventHandler(this.newProjectToolStripButton_Click);
			// 
			// openProjectToolStripButton
			// 
			this.openProjectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openProjectToolStripButton.Image = global::Brewmaster.Properties.Resources.open;
			this.openProjectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openProjectToolStripButton.Name = "openProjectToolStripButton";
			this.openProjectToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openProjectToolStripButton.Text = "Open Project";
			this.openProjectToolStripButton.Click += new System.EventHandler(this.OpenProject_ToolStripButton_Click);
			// 
			// saveToolStripButton
			// 
			this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveToolStripButton.Image = global::Brewmaster.Properties.Resources.save1;
			this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveToolStripButton.Name = "saveToolStripButton";
			this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveToolStripButton.Text = "Save";
			this.saveToolStripButton.Click += new System.EventHandler(this.Save_ToolStripButton_Click);
			// 
			// saveAllToolStripButton
			// 
			this.saveAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveAllToolStripButton.Image = global::Brewmaster.Properties.Resources.saveAll;
			this.saveAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveAllToolStripButton.Name = "saveAllToolStripButton";
			this.saveAllToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveAllToolStripButton.Text = "Save All";
			this.saveAllToolStripButton.Click += new System.EventHandler(this.SaveAll_ToolStripButton_Click);
			// 
			// undoToolStripButton
			// 
			this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.undoToolStripButton.Image = global::Brewmaster.Properties.Resources.undo1;
			this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.undoToolStripButton.Name = "undoToolStripButton";
			this.undoToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.undoToolStripButton.Text = "Undo";
			this.undoToolStripButton.Click += new System.EventHandler(this.Undo_ToolStripButton_Click);
			// 
			// redoToolStripButton
			// 
			this.redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.redoToolStripButton.Image = global::Brewmaster.Properties.Resources.redo1;
			this.redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.redoToolStripButton.Name = "redoToolStripButton";
			this.redoToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.redoToolStripButton.Text = "Redo";
			this.redoToolStripButton.Click += new System.EventHandler(this.Redo_ToolStripButton_Click);
			// 
			// cutToolStripButton
			// 
			this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.cutToolStripButton.Image = global::Brewmaster.Properties.Resources.cut1;
			this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.cutToolStripButton.Name = "cutToolStripButton";
			this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.cutToolStripButton.Text = "Cut";
			this.cutToolStripButton.Click += new System.EventHandler(this.Cut_ToolStripButton_Click);
			// 
			// copyToolStripButton
			// 
			this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.copyToolStripButton.Image = global::Brewmaster.Properties.Resources.copy1;
			this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.copyToolStripButton.Name = "copyToolStripButton";
			this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.copyToolStripButton.Text = "Copy";
			this.copyToolStripButton.Click += new System.EventHandler(this.Copy_ToolStripButton_Click);
			// 
			// pasteToolStripButton
			// 
			this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pasteToolStripButton.Image = global::Brewmaster.Properties.Resources.paste1;
			this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.pasteToolStripButton.Name = "pasteToolStripButton";
			this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.pasteToolStripButton.Text = "Paste";
			this.pasteToolStripButton.Click += new System.EventHandler(this.Paste_ToolStripButton_Click);
			// 
			// configurationSelector
			// 
			this.configurationSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.configurationSelector.Name = "configurationSelector";
			this.configurationSelector.Size = new System.Drawing.Size(121, 25);
			// 
			// buildSettings
			// 
			this.buildSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buildSettings.Image = global::Brewmaster.Properties.Resources.config;
			this.buildSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buildSettings.Name = "buildSettings";
			this.buildSettings.Size = new System.Drawing.Size(23, 22);
			this.buildSettings.Text = "Build Settings";
			this.buildSettings.Click += new System.EventHandler(this.buildSettings_Click);
			// 
			// build
			// 
			this.build.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.build.Image = global::Brewmaster.Properties.Resources.build3;
			this.build.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.build.Name = "build";
			this.build.Size = new System.Drawing.Size(23, 22);
			this.build.Text = "Build";
			this.build.Click += new System.EventHandler(this.Compile_ToolStripButton_Click);
			// 
			// runNewBuild
			// 
			this.runNewBuild.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.runNewBuild.Image = global::Brewmaster.Properties.Resources.buildrun;
			this.runNewBuild.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.runNewBuild.Name = "runNewBuild";
			this.runNewBuild.Size = new System.Drawing.Size(23, 22);
			this.runNewBuild.Text = "Run New Build";
			this.runNewBuild.Click += new System.EventHandler(this.runNewBuild_Click);
			// 
			// continueWithNewBuild
			// 
			this.continueWithNewBuild.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.continueWithNewBuild.Image = ((System.Drawing.Image)(resources.GetObject("continueWithNewBuild.Image")));
			this.continueWithNewBuild.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.continueWithNewBuild.Name = "continueWithNewBuild";
			this.continueWithNewBuild.Size = new System.Drawing.Size(23, 22);
			this.continueWithNewBuild.Text = "Continue With New Build";
			this.continueWithNewBuild.Visible = false;
			this.continueWithNewBuild.Click += new System.EventHandler(this.continueWithNewBuild_Click);
			// 
			// run
			// 
			this.run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.run.Image = global::Brewmaster.Properties.Resources.run;
			this.run.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.run.Name = "run";
			this.run.Size = new System.Drawing.Size(23, 22);
			this.run.Text = "Run";
			this.run.Click += new System.EventHandler(this.Run_ToolStripButton_Click);
			// 
			// pause
			// 
			this.pause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pause.Image = global::Brewmaster.Properties.Resources.pause;
			this.pause.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.pause.Name = "pause";
			this.pause.Size = new System.Drawing.Size(23, 22);
			this.pause.Text = "Pause";
			this.pause.Click += new System.EventHandler(this.pause_Click);
			// 
			// stop
			// 
			this.stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stop.Image = global::Brewmaster.Properties.Resources.stop1;
			this.stop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stop.Name = "stop";
			this.stop.Size = new System.Drawing.Size(23, 22);
			this.stop.Text = "Stop";
			this.stop.Click += new System.EventHandler(this.stop_Click);
			// 
			// restart
			// 
			this.restart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.restart.Image = global::Brewmaster.Properties.Resources.restart;
			this.restart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.restart.Name = "restart";
			this.restart.Size = new System.Drawing.Size(23, 22);
			this.restart.Text = "Restart";
			this.restart.Click += new System.EventHandler(this.restart_Click);
			// 
			// flashCartridge
			// 
			this.flashCartridge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.flashCartridge.Image = global::Brewmaster.Properties.Resources.cartridge;
			this.flashCartridge.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.flashCartridge.Name = "flashCartridge";
			this.flashCartridge.Size = new System.Drawing.Size(23, 22);
			this.flashCartridge.Text = "Flash to Cartridge";
			this.flashCartridge.Click += new System.EventHandler(this.flashCartridge_Click);
			// 
			// stepOver
			// 
			this.stepOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stepOver.Image = global::Brewmaster.Properties.Resources.stepover;
			this.stepOver.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stepOver.Name = "stepOver";
			this.stepOver.Size = new System.Drawing.Size(23, 22);
			this.stepOver.Text = "Step Over (F10)";
			this.stepOver.Click += new System.EventHandler(this.stepOver_Click);
			// 
			// stepInto
			// 
			this.stepInto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stepInto.Image = ((System.Drawing.Image)(resources.GetObject("stepInto.Image")));
			this.stepInto.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stepInto.Name = "stepInto";
			this.stepInto.Size = new System.Drawing.Size(23, 22);
			this.stepInto.Text = "Step Into (F11)";
			this.stepInto.Click += new System.EventHandler(this.stepInto_Click);
			// 
			// stepOut
			// 
			this.stepOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stepOut.Image = global::Brewmaster.Properties.Resources.stepout;
			this.stepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stepOut.Name = "stepOut";
			this.stepOut.Size = new System.Drawing.Size(23, 22);
			this.stepOut.Text = "Step Out (Shift+F11)";
			this.stepOut.Click += new System.EventHandler(this.stepOut_Click);
			// 
			// stepBack
			// 
			this.stepBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.stepBack.Image = global::Brewmaster.Properties.Resources.stepback;
			this.stepBack.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stepBack.Name = "stepBack";
			this.stepBack.Size = new System.Drawing.Size(23, 22);
			this.stepBack.Text = "Step Back (Shift+F10)";
			this.stepBack.Click += new System.EventHandler(this.stepBack_Click);
			// 
			// statusBar
			// 
			this.statusBar.BackColor = System.Drawing.SystemColors.HotTrack;
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.filenameLabel,
            this.toolStripStatusLabel1,
            this.lineLabel,
            this.toolStripStatusLabel2,
            this.charLabel,
            this.toolStripStatusLabel3,
            this.fpsLabel});
			this.statusBar.Location = new System.Drawing.Point(0, 539);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(1287, 22);
			this.statusBar.TabIndex = 5;
			// 
			// statusLabel
			// 
			this.statusLabel.AutoSize = false;
			this.statusLabel.ForeColor = System.Drawing.Color.White;
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(500, 17);
			this.statusLabel.Text = "Ready";
			this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// filenameLabel
			// 
			this.filenameLabel.AutoSize = false;
			this.filenameLabel.ForeColor = System.Drawing.Color.White;
			this.filenameLabel.Name = "filenameLabel";
			this.filenameLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.filenameLabel.Size = new System.Drawing.Size(450, 17);
			this.filenameLabel.Text = "Brewmaster";
			this.filenameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.White;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(29, 17);
			this.toolStripStatusLabel1.Text = "Line";
			this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lineLabel
			// 
			this.lineLabel.AutoSize = false;
			this.lineLabel.ForeColor = System.Drawing.Color.White;
			this.lineLabel.Name = "lineLabel";
			this.lineLabel.Size = new System.Drawing.Size(50, 17);
			this.lineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.White;
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(32, 17);
			this.toolStripStatusLabel2.Text = "Char";
			// 
			// charLabel
			// 
			this.charLabel.AutoSize = false;
			this.charLabel.ForeColor = System.Drawing.Color.White;
			this.charLabel.Name = "charLabel";
			this.charLabel.Size = new System.Drawing.Size(50, 17);
			this.charLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripStatusLabel3
			// 
			this.toolStripStatusLabel3.AutoSize = false;
			this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.White;
			this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
			this.toolStripStatusLabel3.Size = new System.Drawing.Size(111, 17);
			this.toolStripStatusLabel3.Spring = true;
			this.toolStripStatusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// fpsLabel
			// 
			this.fpsLabel.AutoSize = false;
			this.fpsLabel.ForeColor = System.Drawing.Color.White;
			this.fpsLabel.Name = "fpsLabel";
			this.fpsLabel.Size = new System.Drawing.Size(50, 17);
			this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// New_ProjExplorerContextMenuItem
			// 
			this.New_ProjExplorerContextMenuItem.Name = "New_ProjExplorerContextMenuItem";
			this.New_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_Class_ProjExplorerContextMenuItem
			// 
			this.New_Class_ProjExplorerContextMenuItem.Name = "New_Class_ProjExplorerContextMenuItem";
			this.New_Class_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_Package_ProjExplorerContextMenuItem
			// 
			this.New_Package_ProjExplorerContextMenuItem.Name = "New_Package_ProjExplorerContextMenuItem";
			this.New_Package_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_Interface_ProjExplorerContextMenuItem
			// 
			this.New_Interface_ProjExplorerContextMenuItem.Name = "New_Interface_ProjExplorerContextMenuItem";
			this.New_Interface_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_Enums_ProjExplorerContextMenuItem
			// 
			this.New_Enums_ProjExplorerContextMenuItem.Name = "New_Enums_ProjExplorerContextMenuItem";
			this.New_Enums_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_HTMLFile_ProjExplorerContextMenuItem
			// 
			this.New_HTMLFile_ProjExplorerContextMenuItem.Name = "New_HTMLFile_ProjExplorerContextMenuItem";
			this.New_HTMLFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_CSSFile_ProjExplorerContextMenuItem
			// 
			this.New_CSSFile_ProjExplorerContextMenuItem.Name = "New_CSSFile_ProjExplorerContextMenuItem";
			this.New_CSSFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_TextFile_ProjExplorerContextMenuItem
			// 
			this.New_TextFile_ProjExplorerContextMenuItem.Name = "New_TextFile_ProjExplorerContextMenuItem";
			this.New_TextFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_JavaScriptFile_ProjExplorerContextMenuItem
			// 
			this.New_JavaScriptFile_ProjExplorerContextMenuItem.Name = "New_JavaScriptFile_ProjExplorerContextMenuItem";
			this.New_JavaScriptFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_SQLFile_ProjExplorerContextMenuItem
			// 
			this.New_SQLFile_ProjExplorerContextMenuItem.Name = "New_SQLFile_ProjExplorerContextMenuItem";
			this.New_SQLFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_XMLFile_ProjExplorerContextMenuItem
			// 
			this.New_XMLFile_ProjExplorerContextMenuItem.Name = "New_XMLFile_ProjExplorerContextMenuItem";
			this.New_XMLFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// New_NewFile_ProjExplorerContextMenuItem
			// 
			this.New_NewFile_ProjExplorerContextMenuItem.Name = "New_NewFile_ProjExplorerContextMenuItem";
			this.New_NewFile_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator41
			// 
			this.toolStripSeparator41.Name = "toolStripSeparator41";
			this.toolStripSeparator41.Size = new System.Drawing.Size(6, 6);
			// 
			// Open_ProjExplorerContextMenuItem
			// 
			this.Open_ProjExplorerContextMenuItem.Name = "Open_ProjExplorerContextMenuItem";
			this.Open_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// OpenwithSystemEditor_ProjExplorerContextMenuItem
			// 
			this.OpenwithSystemEditor_ProjExplorerContextMenuItem.Name = "OpenwithSystemEditor_ProjExplorerContextMenuItem";
			this.OpenwithSystemEditor_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator42
			// 
			this.toolStripSeparator42.Name = "toolStripSeparator42";
			this.toolStripSeparator42.Size = new System.Drawing.Size(6, 6);
			// 
			// Delete_ProjExplorerContextMenuItem
			// 
			this.Delete_ProjExplorerContextMenuItem.Name = "Delete_ProjExplorerContextMenuItem";
			this.Delete_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator43
			// 
			this.toolStripSeparator43.Name = "toolStripSeparator43";
			this.toolStripSeparator43.Size = new System.Drawing.Size(6, 6);
			// 
			// CloseProject_ProjExplorerContextMenuItem
			// 
			this.CloseProject_ProjExplorerContextMenuItem.Name = "CloseProject_ProjExplorerContextMenuItem";
			this.CloseProject_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator44
			// 
			this.toolStripSeparator44.Name = "toolStripSeparator44";
			this.toolStripSeparator44.Size = new System.Drawing.Size(6, 6);
			// 
			// RemoveAddedFiles_ProjExplorerContextMenuItem
			// 
			this.RemoveAddedFiles_ProjExplorerContextMenuItem.Name = "RemoveAddedFiles_ProjExplorerContextMenuItem";
			this.RemoveAddedFiles_ProjExplorerContextMenuItem.Size = new System.Drawing.Size(32, 19);
			// 
			// OpenProjectFileDialog
			// 
			this.OpenProjectFileDialog.Filter = "Brewmaster Project (*.bwm)|*.bwm|Nesicide Project (*.nesproject)|*.nesproject";
			// 
			// OpenFilesFileDialog
			// 
			this.OpenFilesFileDialog.Filter = resources.GetString("OpenFilesFileDialog.Filter");
			this.OpenFilesFileDialog.Multiselect = true;
			this.OpenFilesFileDialog.Title = "Open Files";
			// 
			// CreateNewFileDialog
			// 
			this.CreateNewFileDialog.Filter = "All files|*.*";
			this.CreateNewFileDialog.OverwritePrompt = false;
			this.CreateNewFileDialog.Title = "Save File";
			// 
			// CodeItemImages
			// 
			this.CodeItemImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CodeItemImages.ImageStream")));
			this.CodeItemImages.TransparentColor = System.Drawing.Color.Transparent;
			this.CodeItemImages.Images.SetKeyName(0, "cartridge.png");
			this.CodeItemImages.Images.SetKeyName(1, "data.png");
			this.CodeItemImages.Images.SetKeyName(2, "image.png");
			this.CodeItemImages.Images.SetKeyName(3, "nesproject.ico");
			this.CodeItemImages.Images.SetKeyName(4, "text.png");
			// 
			// MainEastContainer2
			// 
			this.MainEastContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainEastContainer2.Horizontal = true;
			this.MainEastContainer2.Location = new System.Drawing.Point(0, 50);
			this.MainEastContainer2.Name = "MainEastContainer2";
			this.MainEastContainer2.Size = new System.Drawing.Size(1287, 489);
			this.MainEastContainer2.TabIndex = 1;
			this.MainEastContainer2.Text = "multiSplitContainer2";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1287, 561);
			this.Controls.Add(availableIdePanels);
			this.Controls.Add(this.MainEastContainer2);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.toolstrippanel);
			this.Controls.Add(this.MainWindowMenu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.MainWindowMenu;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Brewmaster";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
			availableIdePanels.ResumeLayout(false);
			availableIdePanels.PerformLayout();
			this.MemoryTabs.ResumeLayout(false);
			this.CpuMemoryTab.ResumeLayout(false);
			this.PpuMemoryTab.ResumeLayout(false);
			this.OamMemoryTab.ResumeLayout(false);
			this.MainWindowMenu.ResumeLayout(false);
			this.MainWindowMenu.PerformLayout();
			this.toolstrippanel.ResumeLayout(false);
			this.toolstrippanel.PerformLayout();
			this.MainToolStrip.ResumeLayout(false);
			this.MainToolStrip.PerformLayout();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainWindowMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_NewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nesProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem sourceFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem includeFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tileMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphicsDataMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem File_OpenProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem File_SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_SaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_SaveAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem File_CloseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_CloseAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_CloseProjectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem File_PrintMenuItem;
        private System.Windows.Forms.ToolStripMenuItem File_PrintPreviewMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem File_ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Edit_CutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Edit_CopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Edit_PasteMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem Edit_UndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Edit_RedoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Edit_GoToMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem insertMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertMacroMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem BuildMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildProjectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem stepOverMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem buildSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllWindowsMenuItem;
        private System.Windows.Forms.ToolStripSeparator WindowMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpTopicsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripButton openProjectToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton saveAllToolStripButton;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripButton undoToolStripButton;
        private System.Windows.Forms.ToolStripButton redoToolStripButton;
        private System.Windows.Forms.ToolStripButton build;
        private System.Windows.Forms.ToolStripButton run;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel filenameLabel;
        public System.Windows.Forms.ToolStripStatusLabel lineLabel;
        public System.Windows.Forms.ToolStripStatusLabel fpsLabel;
        private EditorTabs editorTabs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator34;
        private System.Windows.Forms.ToolStripMenuItem appearanceMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenProjectFileDialog;
        private System.Windows.Forms.OpenFileDialog OpenFilesFileDialog;
        private System.Windows.Forms.SaveFileDialog CreateNewFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator35;
        private System.Windows.Forms.ToolStripMenuItem fullScreenMenuItem;
        public System.Windows.Forms.ToolStripMenuItem viewStatusBarMenuItem;
        public System.Windows.Forms.ToolStripMenuItem viewToolbarMenuItem;
        public System.Windows.Forms.ToolStripMenuItem viewLineNumbersMenuItem;
        public System.Windows.Forms.FlowLayoutPanel toolstrippanel;
        public System.Windows.Forms.ToolStrip MainToolStrip;
        public System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripMenuItem New_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_Class_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_Package_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_Interface_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_Enums_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_HTMLFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_CSSFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_TextFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_JavaScriptFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_SQLFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_XMLFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem New_NewFile_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator41;
        private System.Windows.Forms.ToolStripMenuItem Open_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenwithSystemEditor_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator42;
        private System.Windows.Forms.ToolStripMenuItem Delete_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator43;
        private System.Windows.Forms.ToolStripMenuItem CloseProject_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator44;
        private System.Windows.Forms.ToolStripMenuItem RemoveAddedFiles_ProjExplorerContextMenuItem;
        private System.Windows.Forms.ImageList ProjectExplorerImageList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator49;
		private Emulation.MesenControl mesen;
		private System.Windows.Forms.ToolStripButton pause;
		private System.Windows.Forms.ToolStripButton stop;
		private System.Windows.Forms.ToolStripButton restart;
		private ProjectExplorer.ProjectExplorer ProjectExplorer;
		private CartridgeExplorer.CartridgeExplorer CartridgeExplorer;
		private System.Windows.Forms.ToolStripButton newProjectToolStripButton;
		private StatusView.CpuStatus cpuStatus1;
		private System.Windows.Forms.ToolStripMenuItem EmulatorMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateEveryFrameMenuItem;
		private TileMapViewer TileMap;
		private System.Windows.Forms.ImageList CodeItemImages;
		private System.Windows.Forms.ToolStripMenuItem findNextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nesGraphicsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem displayNesBgMenuItem;
		private System.Windows.Forms.ToolStripMenuItem displayNesObjectsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nesAudioMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playPulse1MenuItem;
		private System.Windows.Forms.ToolStripMenuItem playPulse2MenuItem;
		private System.Windows.Forms.ToolStripMenuItem playTriangleMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playNoiseMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playPcmMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mapInputMenuItem;
		private System.Windows.Forms.ToolStripButton flashCartridge;
		private System.Windows.Forms.ToolStripMenuItem stepIntoMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepOutMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stepBackMenuItem;
		private System.Windows.Forms.ToolStripButton stepOver;
		private System.Windows.Forms.ToolStripButton stepInto;
		private System.Windows.Forms.ToolStripButton stepOut;
		private System.Windows.Forms.ToolStripButton stepBack;
		private System.Windows.Forms.TabControl MemoryTabs;
		private System.Windows.Forms.TabPage CpuMemoryTab;
		private MemoryViewer.MemoryViewer CpuMemoryViewer;
		private System.Windows.Forms.TabPage PpuMemoryTab;
		private MemoryViewer.MemoryViewer PpuMemoryViewer;
		private System.Windows.Forms.TabPage OamMemoryTab;
		private MemoryViewer.MemoryViewer OamMemoryViewer;
		private Brewmaster.Ide.IdeGroupedPanel WatchPanel;
		private System.Windows.Forms.ToolStripMenuItem restartMenuItem;
		private System.Windows.Forms.ToolStripMenuItem snesProjectMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
		private Brewmaster.Ide.IdeGroupedPanel OutputPanel;
		public Ide.MultiSplitContainer MainEastContainer2;
		private ToolStripMenuItem runNewBuildMenuItem;
		private ToolStripButton runNewBuild;
		private ToolStripMenuItem snesGraphicsMenuItem;
		private ToolStripMenuItem displayBgLayer1MenuItem;
		private ToolStripMenuItem displayBgLayer2MenuItem;
		private ToolStripMenuItem displayBgLayer3MenuItem;
		private ToolStripMenuItem displayBgLayer4MenuItem;
		private ToolStripMenuItem displaySnesSpritesMenuItem;
		private ToolStripMenuItem snesAudioMenuItem;
		private ToolStripMenuItem playSpcAudioMenuItem;
		private ToolStripMenuItem lineAddressMappingsMenuItem;
		private ToolStripMenuItem flashCartridgeMenuItem;
		private ToolStripMenuItem importProjectMenuItem;
		private ToolStripMenuItem continueWithNewBuildMenuItem;
		private ToolStripButton continueWithNewBuild;
		private ToolStripComboBox configurationSelector;
		private ToolStripButton buildSettings;
		private ToolStripMenuItem integerScalingMenuItem;
		private ToolStripMenuItem randomValuesAtPowerOnMenuItem;
		private ToolStripMenuItem playAudioMenuItem;
		private ToolStripMenuItem loadStateMenuItem;
		private ToolStripMenuItem saveStateMenuItem;
		private ToolStripMenuItem selectAllMenuItem;
		private Modules.MenuHelper _menuHelper;
		public ToolStripStatusLabel toolStripStatusLabel1;
		public ToolStripStatusLabel toolStripStatusLabel2;
		private ToolStripMenuItem emulatorSettingsMenuItem;
		public ToolStripStatusLabel charLabel;
		public ToolStripStatusLabel toolStripStatusLabel3;
		private ToolStripMenuItem findInFilesMenuItem;
	}
}

