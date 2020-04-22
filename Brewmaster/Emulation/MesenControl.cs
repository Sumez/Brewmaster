﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrewMaster.ProjectModel;
using BrewMaster.Settings;

namespace BrewMaster.Emulation
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
			this._renderer = new BrewMaster.Emulation.MesenRenderer();
			this._speedSlider = new BrewMaster.Emulation.SmallTrackBar();
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
			this._saveButton.Image = global::BrewMaster.Properties.Resources.smallsave;
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
			this._loadButton.Image = global::BrewMaster.Properties.Resources.smallload;
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

		private EmulatorSettings _emulatorSettings = new EmulatorSettings();

		public class EmulatorSettings
		{
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
		}
		public bool RandomPowerOnState
		{
			set { _emulatorSettings.RandomPowerOnState = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlayAudio
		{
			set { _emulatorSettings.PlayAudio = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowBgLayer
		{
			set { _emulatorSettings.ShowBgLayer = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowBgLayer1
		{
			set { _emulatorSettings.ShowBgLayer1 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowBgLayer2
		{
			set { _emulatorSettings.ShowBgLayer2 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowBgLayer3
		{
			set { _emulatorSettings.ShowBgLayer3 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowBgLayer4
		{
			set { _emulatorSettings.ShowBgLayer4 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool ShowSpriteLayer
		{
			set { _emulatorSettings.ShowSpriteLayer = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlaySquare1
		{
			set { _emulatorSettings.PlaySquare1 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlaySquare2
		{
			set { _emulatorSettings.PlaySquare2 = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlayTriangle
		{
			set { _emulatorSettings.PlayTriangle = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlayNoise
		{
			set { _emulatorSettings.PlayNoise = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}
		public bool PlayPcm
		{
			set { _emulatorSettings.PlayPcm = value; if (Emulator != null) Emulator.UpdateSettings(_emulatorSettings); }
		}

		public Color EmulatorBackgroundColor { set { _renderer.BackColor = value; Invalidate(); } }


		private ProjectType _currentProjectType = ProjectType.Nes;
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

		public void SwitchSystem(ProjectType projectType, Action<IEmulatorHandler> initMethod)
		{
			switch (projectType)
			{
				case ProjectType.Snes when _snesEmulator == null:
					_snesEmulator = new SnesEmulatorHandler(FindForm());
					_snesEmulator.UpdateRate = _updateRate;
					_snesEmulator.OnStatusChange += ThreadSafeEmulationStatusChanged;
					initMethod(_snesEmulator);
					break;
				
				case ProjectType.Nes when _nesEmulator == null:
					_nesEmulator = new NesEmulatorHandler(FindForm());
					_nesEmulator.UpdateRate = _updateRate;
					_nesEmulator.OnStatusChange += ThreadSafeEmulationStatusChanged;
					initMethod(_nesEmulator);
					break;
			}

			_renderer.SwitchRenderSurface(projectType);

			Emulator = projectType == ProjectType.Snes
				? _snesEmulator
				: _nesEmulator;

			Emulator.UpdateSettings(_emulatorSettings);
			currentMapping = mappings[projectType];
			Emulator.UpdateControllerMappings(currentMapping.Where(m => m.Key >= 0x10000).ToDictionary(m => m.Value, m => m.Key));
			_currentProjectType = projectType;
		}

		private void ThreadSafeEmulationStatusChanged(EmulatorStatus status)
		{
			BeginInvoke(new Action<EmulatorStatus>(EmulationStatusChanged), status);
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

			var wParam = (int)((Int64)m.WParam & 0xFF);
			if (!currentMapping.ContainsKey(wParam)) return false;
			var scanCode = currentMapping[wParam];

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

		private Dictionary<ProjectType, Dictionary<int, int>> mappings = new Dictionary<ProjectType, Dictionary<int, int>>();
		private Dictionary<int, int> currentMapping = new Dictionary<int, int>();

		public void SetButtonMappings(List<KeyboardMapping> nes, List<KeyboardMapping> snes)
		{
			var nesMappings = new Dictionary<int, int>();
			var snesMappings = new Dictionary<int, int>();
			foreach (var mapping in nes) nesMappings[mapping.MappedTo] = mapping.TargetKey;
			foreach (var mapping in snes) snesMappings[mapping.MappedTo] = mapping.TargetKey;

			mappings[ProjectType.Nes] = nesMappings;
			mappings[ProjectType.Snes] = snesMappings;

			currentMapping = mappings[_currentProjectType];
			if (Emulator != null) Emulator.UpdateControllerMappings(currentMapping.Where(m => m.Key >= 0x10000).ToDictionary(m => m.Value, m => m.Key));
		}

		public void UnloadEmulator()
		{
			Emulator = null;
		}

		public Control GetRenderSurface(ProjectType projectType)
		{
			return _renderer.GetRenderSurface(projectType);
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
}
