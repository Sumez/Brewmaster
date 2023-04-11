using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.EditorWindows.TileMaps;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.Emulation
{
	public class MesenControl : UserControl, IMessageFilter
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
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		public MesenControl()
		{
			InitializeComponent();
			_renderer.ScaleChanged = scale =>
			{
				if (Emulator == null) return false;
				Emulator.SetScale(scale);
				return true;
			};
			_speedSlider.GotFocus += (s, a) => { _renderer.Focus(); };
			_speedSlider.ValueChanged += () =>
			{
				if (Emulator != null) Emulator.SetSpeed(EmulationSpeed[_speedSlider.Value]);
				if (_isSpeedTolTipVisible) _toolTip.Show(EmulationSpeed[_speedSlider.Value] + "%", _speedSlider, Point.Add(_speedSlider.Location, new Size(45, -25)));
			};
			_speedSlider.MouseUp += (sender, args) =>
			{
				if (args.Button != MouseButtons.Left) return;
				_renderer.Focus();
				_isSpeedTolTipVisible = false;
				_toolTip.Hide(_speedSlider);
			};
			_speedSlider.MouseDown += (sender, args) =>
			{
				if (args.Button != MouseButtons.Left) return;
				_isSpeedTolTipVisible = true;
				_toolTip.Show(EmulationSpeed[_speedSlider.Value] + "%", _speedSlider, Point.Add(_speedSlider.Location, new Size(45, -25)));
			};

			// Loading files synchronously in constructor is bad practice, but the file is small, and this should only happen when program starts anyway
			// In the future, maybe get settings-object from a thread that loads all settings async on startup
			EmulatorSettings = new EmulatorSettings();
			if (Program.WorkingDirectory != null) {
				EmulatorSettings.NesPalette.Load(Path.Combine(Program.WorkingDirectory, "nes.pal")); // TODO: custom palettes
				NesColorPicker.NesPalette = EmulatorSettings.NesPalette;
			}
			EmulatorSettings.SnesPalette.LoadSnesPalette();
		}

		private bool _isSpeedTolTipVisible = false;
		private ToolTip _toolTip;
		private Button _loadButton;
		private Button _saveButton;
		private static readonly Dictionary<int, int> EmulationSpeed = new Dictionary<int, int> { { 0, 3 }, { 1, 6 }, { 2, 12 }, { 3, 25 }, { 4, 50 }, { 5, 100 }, { 6, 150 }, { 7, 200 }, { 8, 500 }, { 9, 1500 }, { 10, 4000 } };

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._bottomPanel = new System.Windows.Forms.Panel();
			this._renderPanel = new System.Windows.Forms.Panel();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._saveButton = new System.Windows.Forms.Button();
			this._loadButton = new System.Windows.Forms.Button();
			this._renderer = new Brewmaster.Emulation.MesenRenderer();
			this._speedSlider = new Brewmaster.Emulation.SmallTrackBar();
			this._bottomPanel.SuspendLayout();
			this._renderPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._speedSlider)).BeginInit();
			this.SuspendLayout();
			// 
			// _bottomPanel
			// 
			this._bottomPanel.BackColor = System.Drawing.SystemColors.Control;
			this._bottomPanel.Controls.Add(this._saveButton);
			this._bottomPanel.Controls.Add(this._speedSlider);
			this._bottomPanel.Controls.Add(this._loadButton);
			this._bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._bottomPanel.Location = new System.Drawing.Point(0, 202);
			this._bottomPanel.Margin = new System.Windows.Forms.Padding(0);
			this._bottomPanel.Name = "_bottomPanel";
			this._bottomPanel.Size = new System.Drawing.Size(349, 21);
			this._bottomPanel.TabIndex = 0;
			// 
			// _renderPanel
			// 
			this._renderPanel.Controls.Add(this._renderer);
			this._renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._renderPanel.Location = new System.Drawing.Point(0, 0);
			this._renderPanel.Name = "_renderPanel";
			this._renderPanel.Padding = new System.Windows.Forms.Padding(3);
			this._renderPanel.Size = new System.Drawing.Size(349, 202);
			this._renderPanel.TabIndex = 2;
			// 
			// _saveButton
			// 
			this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._saveButton.Enabled = false;
			this._saveButton.Image = global::Brewmaster.Properties.Resources.smallsave;
			this._saveButton.Location = new System.Drawing.Point(304, 0);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(22, 20);
			this._saveButton.TabIndex = 2;
			this._toolTip.SetToolTip(this._saveButton, "Save state");
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _loadButton
			// 
			this._loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._loadButton.Enabled = false;
			this._loadButton.Image = global::Brewmaster.Properties.Resources.smallload;
			this._loadButton.Location = new System.Drawing.Point(326, 0);
			this._loadButton.Name = "_loadButton";
			this._loadButton.Size = new System.Drawing.Size(22, 20);
			this._loadButton.TabIndex = 1;
			this._toolTip.SetToolTip(this._loadButton, "Load state");
			this._loadButton.UseVisualStyleBackColor = true;
			this._loadButton.Click += new System.EventHandler(this._loadButton_Click);
			// 
			// _renderer
			// 
			this._renderer.BackColor = System.Drawing.Color.Black;
			this._renderer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._renderer.IntegerScaling = false;
			this._renderer.Location = new System.Drawing.Point(3, 3);
			this._renderer.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this._renderer.Name = "_renderer";
			this._renderer.ScaleChanged = null;
			this._renderer.Size = new System.Drawing.Size(343, 196);
			this._renderer.TabIndex = 1;
			// 
			// _speedSlider
			// 
			this._speedSlider.LargeChange = 1;
			this._speedSlider.Location = new System.Drawing.Point(3, 3);
			this._speedSlider.Name = "_speedSlider";
			this._speedSlider.Size = new System.Drawing.Size(126, 45);
			this._speedSlider.TabIndex = 0;
			this._speedSlider.TickStyle = System.Windows.Forms.TickStyle.None;
			this._speedSlider.Value = 5;
			// 
			// MesenControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._renderPanel);
			this.Controls.Add(this._bottomPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "MesenControl";
			this.Size = new System.Drawing.Size(349, 223);
			this._bottomPanel.ResumeLayout(false);
			this._bottomPanel.PerformLayout();
			this._renderPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._speedSlider)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		public bool IntegerScaling
		{
			get { return _renderer.IntegerScaling; }
			set
			{
				_renderer.IntegerScaling = value;
				_renderer.ResizeSurface();
			}
		}

		public EmulatorSettings EmulatorSettings { get; private set; }

		public RegionTiming Timing
		{
			set { EmulatorSettings.Timing = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool RandomPowerOnState
		{
			set { EmulatorSettings.RandomPowerOnState = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlayAudio
		{
			set { EmulatorSettings.PlayAudio = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowBgLayer
		{
			set { EmulatorSettings.ShowBgLayer = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowBgLayer1
		{
			set { EmulatorSettings.ShowBgLayer1 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowBgLayer2
		{
			set { EmulatorSettings.ShowBgLayer2 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowBgLayer3
		{
			set { EmulatorSettings.ShowBgLayer3 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowBgLayer4
		{
			set { EmulatorSettings.ShowBgLayer4 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool ShowSpriteLayer
		{
			set { EmulatorSettings.ShowSpriteLayer = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlaySquare1
		{
			set { EmulatorSettings.PlaySquare1 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlaySquare2
		{
			set { EmulatorSettings.PlaySquare2 = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlayTriangle
		{
			set { EmulatorSettings.PlayTriangle = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlayNoise
		{
			set { EmulatorSettings.PlayNoise = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}
		public bool PlayPcm
		{
			set { EmulatorSettings.PlayPcm = value; if (Emulator != null) Emulator.UpdateSettings(EmulatorSettings); }
		}

		public Color EmulatorBackgroundColor { set { _renderer.BackColor = value; Invalidate(); } }


		private TargetPlatform _currentPlatform = TargetPlatform.Nes;
		public IEmulatorHandler Emulator { get; private set; }
		private int _updateRate = 1;
		public int UpdateRate
		{
			set
			{
				_updateRate = value;
				if (_nesEmulator != null) _nesEmulator.UpdateRate = _updateRate;
				if (_snesEmulator != null) _snesEmulator.UpdateRate = _updateRate;
			}
		}

		private IEmulatorHandler _snesEmulator;
		private Panel _bottomPanel;
		private MesenRenderer _renderer;
		private Panel _renderPanel;
		private SmallTrackBar _speedSlider;
		private IEmulatorHandler _nesEmulator;

		public void SwitchSystem(TargetPlatform platform, Action<IEmulatorHandler> initMethod, Form mainForm)
		{
			switch (platform)
			{
				case TargetPlatform.Snes when _snesEmulator == null:
					_snesEmulator = new SnesEmulatorHandler(mainForm);
					_snesEmulator.UpdateRate = _updateRate;
					_snesEmulator.OnStatusChange += ThreadSafeEmulationStatusChanged;
					initMethod(_snesEmulator);
					break;
				
				case TargetPlatform.Nes when _nesEmulator == null:
					_nesEmulator = new NesEmulatorHandler(mainForm);
					_nesEmulator.UpdateRate = _updateRate;
					_nesEmulator.OnStatusChange += ThreadSafeEmulationStatusChanged;
					initMethod(_nesEmulator);
					break;
			}

			_renderer.SwitchRenderSurface(platform);

			Emulator = platform == TargetPlatform.Snes
				? _snesEmulator
				: _nesEmulator;

			Emulator.UpdateSettings(EmulatorSettings);
			currentMapping = mappings[platform];
			Emulator.UpdateControllerMappings(currentMapping);
			_currentPlatform = platform;
		}

		private void ThreadSafeEmulationStatusChanged(EmulatorStatus status)
		{
			if (ParentForm != null && Visible) BeginInvoke(new Action<EmulatorStatus>(EmulationStatusChanged), status);
		}
		private void EmulationStatusChanged(EmulatorStatus status)
		{
			_loadButton.Enabled = _saveButton.Enabled = status != EmulatorStatus.Stopped;
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			_renderPanel.BackColor = Color.Gray;
		}
		protected override void OnLeave(EventArgs e)
		{
			base.OnEnter(e);
			_renderPanel.BackColor = Color.Black;
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var boundingRectangle = new Rectangle(Point.Empty, Size);
			if (ContainsFocus) e.Graphics.DrawRectangle(new Pen(Color.Gray, 3), boundingRectangle);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Application.AddMessageFilter(this);
		}
		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 0x104;
		private const int WM_SYSKEYUP = 0x105;

		public bool PreFilterMessage(ref Message m)
		{
			if (Emulator == null) return false;

			var scanCode = (Int32)(((Int64)m.LParam & 0x1FF0000) >> 16);
			if (m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP)
			{
				Emulator.SetKeyState(scanCode, false);
			}
			else if (ContainsFocus && (m.Msg == WM_SYSKEYDOWN || m.Msg == WM_KEYDOWN))
			{
				Emulator.SetKeyState(scanCode, true);
			}

			return false;
		}

		private Dictionary<TargetPlatform, Dictionary<ControllerButtons, int>> mappings = new Dictionary<TargetPlatform, Dictionary<ControllerButtons, int>>();
		private Dictionary<ControllerButtons, int> currentMapping = new Dictionary<ControllerButtons, int>();

		public void SetButtonMappings(List<KeyboardMapping> nes, List<KeyboardMapping> snes)
		{
			var nesMappings = new Dictionary<ControllerButtons, int>();
			var snesMappings = new Dictionary<ControllerButtons, int>();
			foreach (var mapping in nes) nesMappings[(ControllerButtons)mapping.TargetKey] = mapping.MappedTo;
			foreach (var mapping in snes) snesMappings[(ControllerButtons)mapping.TargetKey] = mapping.MappedTo;

			mappings[TargetPlatform.Nes] = nesMappings;
			mappings[TargetPlatform.Snes] = snesMappings;

			currentMapping = mappings[_currentPlatform];
			if (Emulator != null) Emulator.UpdateControllerMappings(currentMapping);
		}

		public async Task UnloadEmulator(bool freeResources = false)
		{
			if (freeResources)
			{
				if (_snesEmulator != null) await _snesEmulator.Stop(true);
				if (_nesEmulator != null) await _nesEmulator.Stop(true);
				_snesEmulator = null;
				_nesEmulator = null;
			}

			Emulator = null;
		}

		public Control GetRenderSurface(TargetPlatform platform)
		{
			return _renderer.GetRenderSurface(platform);
		}

		private void _saveButton_Click(object sender, EventArgs e)
		{
			SaveState();
		}

		private void _loadButton_Click(object sender, EventArgs e)
		{
			LoadState();
		}

		public void SaveState()
		{
			var file = SaveStateFile;
			if (file == null || Emulator == null) return;
			if (!Directory.Exists(Path.GetDirectoryName(file))) Directory.CreateDirectory(Path.GetDirectoryName(file));
			Emulator.SaveState(file);
		}
		public void LoadState()
		{
			var file = SaveStateFile;
			if (file == null || Emulator == null) return;
			Emulator.LoadState(file);
		}

		private string SaveStateFile { get { return _currentFile == null ? null : Program.GetUserFilePath(string.Format(@"states\{0}.mst", _currentFile)); } }

		private string _currentFile;
		public void LoadCartridge(string directory, string file)
		{
			_currentFile = Path.GetFileNameWithoutExtension(file);
			if (Emulator != null) Emulator.LoadCartridge(directory, file);
		}

		public void LoadCartridgeAtSameState(string directory, string file, Func<int, int> getPc)
		{
			_currentFile = Path.GetFileNameWithoutExtension(file);
			if (Emulator != null) Emulator.LoadCartridgeAtSameState(directory, file, getPc);
		}
	}

	public class EmulatorSettings
	{
		public RegionTiming Timing = RegionTiming.NTSC;
		public bool RandomPowerOnState = true;
		public bool PlayAudio = true;
		public bool ShowBgLayer = true;
		public bool ShowBgLayer1 = true;
		public bool ShowBgLayer2 = true;
		public bool ShowBgLayer3 = true;
		public bool ShowBgLayer4 = true;
		public bool ShowSpriteLayer = true;
		public bool PlaySquare1 = true;
		public bool PlaySquare2 = true;
		public bool PlayTriangle = true;
		public bool PlayNoise = true;
		public bool PlayPcm = true;
		public Palette NesPalette = new Palette();
		public Palette SnesPalette = new Palette();
	}
	public enum RegionTiming { NTSC, PAL }
}
