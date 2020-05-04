using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Brewmaster.Emulation;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;
using Mesen.GUI;

namespace Brewmaster.StatusView
{
	public partial class CpuStatus : UserControl
	{
		public CpuStatus()
		{
			InitializeComponent();
			InitializeFlagEvents();
			SetMode(ProjectType.Nes);
		}

		public Events ModuleEvents
		{
			get { return _moduleEvents; }
			set
			{
				_moduleEvents = value;
				if (_moduleEvents == null) return;
				_moduleEvents.EmulationStateUpdate += UpdateStates;
				_moduleEvents.ProjectTypeChanged += SetMode;
			}
		}

		private void InitializeFlagEvents()
		{
			CheckC.CheckedChanged += FlagChanged;
			CheckZ.CheckedChanged += FlagChanged;
			CheckIrq.CheckedChanged += FlagChanged;
			CheckDec.CheckedChanged += FlagChanged;
			CheckV.CheckedChanged += FlagChanged;
			CheckN.CheckedChanged += FlagChanged;
			
			CheckM.CheckedChanged += FlagChanged;
			CheckX.CheckedChanged += FlagChanged;
			CheckEmu.CheckedChanged += FlagChanged;

			RegisterA.ValueChangedByUser += EditA_ValueChanged;
			RegisterX.ValueChangedByUser += EditX_ValueChanged;
			RegisterY.ValueChangedByUser += EditY_ValueChanged;
			RegisterSP.ValueChangedByUser += EditSP_ValueChanged;
			RegisterPC.ValueChangedByUser += EditPC_ValueChanged;
			RegisterDB.ValueChangedByUser += EditDB_ValueChanged;
			RegisterDP.ValueChangedByUser += EditDP_ValueChanged;
			RegisterP.ValueChangedByUser += EditP_ValueChanged;

			nesEnableNmi.CheckedChanged += NesPpuCtrlChanged;
			nesLargeSprites.CheckedChanged += NesPpuCtrlChanged;
			nesVramDown.CheckedChanged += NesPpuCtrlChanged;
			nesBg1000.CheckedChanged += NesPpuCtrlChanged;
			nesSprites1000.CheckedChanged += NesPpuCtrlChanged;

			nesEmphasizeB.CheckedChanged += NesPpuCtrlChanged;
			nesEmphasizeG.CheckedChanged += NesPpuCtrlChanged;
			nesEmphasizeR.CheckedChanged += NesPpuCtrlChanged;
			nesShowSprites.CheckedChanged += NesPpuCtrlChanged;
			nesShowBg.CheckedChanged += NesPpuCtrlChanged;
			nesShowLeftSprites.CheckedChanged += NesPpuCtrlChanged;
			nesShowLeftBg.CheckedChanged += NesPpuCtrlChanged;
			nesGreyscale.CheckedChanged += NesPpuCtrlChanged;

			nesVblankFlag.CheckedChanged += NesPpuCtrlChanged;
			nesSprite0.CheckedChanged += NesPpuCtrlChanged;
			nesSpriteOverflow.CheckedChanged += NesPpuCtrlChanged;

			stack8bit.CheckedChanged += StackSizeChanged;
			stack16bit.CheckedChanged += StackSizeChanged;
			stack24bit.CheckedChanged += StackSizeChanged;
		}

		bool _showDecimalNumbers = false;

		private string ParseNumber(int number, int bytes)
		{
			if (_showDecimalNumbers) return number.ToString();
			return Convert.ToString(number, 16).ToUpper().PadLeft(bytes*2, '0');
		}

		public void SetMode(ProjectType type)
		{
			flagPanel.SuspendLayout();
			registerPanel.SuspendLayout();
			switch (type)
			{
				case ProjectType.Nes:
					RegisterA.RegisterSize =
					RegisterX.RegisterSize =
					RegisterY.RegisterSize = RegisterSize.EightBit;

					RegisterPC.RegisterSize = RegisterSize.SixteenBit;

					snesFlags.Visible = panelDB.Visible = panelDP.Visible = false;
					break;
				case ProjectType.Snes:
					RegisterA.RegisterSize =
					RegisterX.RegisterSize =
					RegisterY.RegisterSize = RegisterSize.SixteenBit;

					RegisterPC.RegisterSize = RegisterSize.TwentyfourBit;

					snesFlags.Visible = panelDB.Visible = panelDP.Visible = true;
					break;
			}
			registerPanel.ResumeLayout();
			flagPanel.ResumeLayout();
		}

		private EmulationState _lastState = null;
		private bool _loading = false;
		public void UpdateStates(EmulationState state)
		{
			if (state == null) return;
			_loading = true;
			_lastState = state;
			if (state.Type == ProjectType.Nes) UpdateNesState(state.NesState);
			if (state.Type == ProjectType.Snes) UpdateSnesState(state.SnesState);
			_cpuMemory = state.Memory.CpuData;
			RefreshStack();
			_loading = false;
		}

		private void UpdateSnesState(Mesen.GUI.DebugState state)
		{
			RegisterA.DimUpperByte = CheckM.Checked = (state.Cpu.PS & ProcFlags.MemoryMode8) != 0;
			RegisterX.DimUpperByte = RegisterY.DimUpperByte = CheckX.Checked = (state.Cpu.PS & ProcFlags.IndexMode8) != 0;
			CheckEmu.Checked = state.Cpu.EmulationMode;

			RegisterPC.Value = state.Cpu.K << 16 | state.Cpu.PC;
			RegisterP.Value = (int)state.Cpu.PS;

			RegisterA.Value = state.Cpu.A;
			RegisterX.Value = state.Cpu.X;
			RegisterY.Value = state.Cpu.Y;
			RegisterDB.Value = state.Cpu.DBR;
			RegisterDP.Value = state.Cpu.D;
			RegisterSP.Value = state.Cpu.SP;

			_stackPointer = state.Cpu.SP;

			CheckC.Checked = (state.Cpu.PS & ProcFlags.Carry) != 0;
			CheckZ.Checked = (state.Cpu.PS & ProcFlags.Zero) != 0;
			CheckIrq.Checked = (state.Cpu.PS & ProcFlags.IrqDisable) != 0;
			CheckDec.Checked = (state.Cpu.PS & ProcFlags.Decimal) != 0;
			CheckV.Checked = (state.Cpu.PS & ProcFlags.Overflow) != 0;
			CheckN.Checked = (state.Cpu.PS & ProcFlags.Negative) != 0;

			_lastCycle = state.Cpu.CycleCount;
			_lastFrame = state.Ppu.FrameCount;
			if (_lastCycle < _cycleBase) _cycleBase = 0;
			if (_lastFrame < _frameBase) _frameBase = 0;

			EditCycle.Text = (_lastCycle - _cycleBase).ToString();
			EditScanline.Text = state.Ppu.Scanline.ToString();
			EditPixel.Text = state.Ppu.Cycle.ToString();
			EditFrame.Text = (_lastFrame - _frameBase).ToString();
		}

		private void UpdateNesState(Emulation.DebugState state)
		{
			RegisterPC.Value = state.CPU.PC;
			RegisterP.Value = state.CPU.PS;
			RegisterA.Value = state.CPU.A;
			RegisterX.Value = state.CPU.X;
			RegisterY.Value = state.CPU.Y;
			RegisterSP.Value = state.CPU.SP;

			_stackPointer = state.CPU.SP | 0x100;

			CheckC.Checked = (state.CPU.PS & (1 << 0)) != 0;
			CheckZ.Checked = (state.CPU.PS & (1 << 1)) != 0;
			CheckIrq.Checked = (state.CPU.PS & (1 << 2)) != 0;
			CheckDec.Checked = (state.CPU.PS & (1 << 3)) != 0;
			CheckV.Checked = (state.CPU.PS & (1 << 6)) != 0;
			CheckN.Checked = (state.CPU.PS & (1 << 7)) != 0;

			UpdateTextBox(nesVramAddr, state.PPU.State.VideoRamAddr, 2);
			UpdateTextBox(nesNtAddr, state.PPU.State.VideoRamAddr & 0x0fff | 0x2000, 2);
			UpdateTextBox(nesPpuTRegister, state.PPU.State.TmpVideoRamAddr, 2);

			nesEnableNmi.Checked = state.PPU.ControlFlags.VBlank > 0;
			nesLargeSprites.Checked = state.PPU.ControlFlags.LargeSprites > 0;
			nesVramDown.Checked = state.PPU.ControlFlags.VerticalWrite > 0;
			nesBg1000.Checked = state.PPU.ControlFlags.BackgroundPatternAddr == 0x1000;
			nesSprites1000.Checked = state.PPU.ControlFlags.SpritePatternAddr == 0x1000;

			nesEmphasizeB.Checked = state.PPU.ControlFlags.IntensifyBlue > 0;
			nesEmphasizeG.Checked = state.PPU.ControlFlags.IntensifyGreen > 0;
			nesEmphasizeR.Checked = state.PPU.ControlFlags.IntensifyRed > 0;
			nesShowSprites.Checked = state.PPU.ControlFlags.SpritesEnabled > 0;
			nesShowBg.Checked = state.PPU.ControlFlags.BackgroundEnabled > 0;
			nesShowLeftSprites.Checked = state.PPU.ControlFlags.SpriteMask > 0;
			nesShowLeftBg.Checked = state.PPU.ControlFlags.BackgroundMask > 0;
			nesGreyscale.Checked = state.PPU.ControlFlags.Grayscale > 0;

			nesVblankFlag.Checked = state.PPU.StatusFlags.VerticalBlank > 0;
			nesSprite0.Checked = state.PPU.StatusFlags.Sprite0Hit > 0;
			nesSpriteOverflow.Checked = state.PPU.StatusFlags.SpriteOverflow > 0;

			_lastCycle = (ulong)state.CPU.CycleCount;
			_lastFrame = state.PPU.FrameCount;
			if (_lastCycle < _cycleBase) _cycleBase = 0;
			if (_lastFrame < _frameBase) _frameBase = 0;

			EditCycle.Text = (_lastCycle - _cycleBase).ToString();
			EditScanline.Text = state.PPU.Scanline.ToString();
			EditPixel.Text = state.PPU.Cycle.ToString();
			EditFrame.Text = (_lastFrame - _frameBase).ToString();
		}

		private void UpdateTextBox(TextBox textBox, int value, int bytes)
		{
			UpdateTextBox(textBox, ParseNumber(value, bytes));
		}
		private void UpdateTextBox(TextBox textBox, string value)
		{
			var selected = textBox.Focused;
			var caretStart = textBox.SelectionStart;
			var selectionLength = textBox.SelectionLength;
			textBox.Text = value;
			if (selected) textBox.Select(caretStart, selectionLength);
		}

		private uint _frameBase = 0;
		private ulong _cycleBase = 0;
		private uint _lastFrame = 0;
		private ulong _lastCycle = 0;
		public Action<EmulationState> StateEdited;

		private void ResetFrame_Click(object sender, EventArgs e)
		{
			_frameBase = _lastFrame;
			UpdateStates(_lastState);
		}

		private void ResetCycle_Click(object sender, EventArgs e)
		{
			_cycleBase = _lastCycle;
			UpdateStates(_lastState);
		}

		private static bool Parse(string value, out byte outputValue)
		{
			return byte.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out outputValue);
		}
		private static bool Parse(string value, out ushort outputValue)
		{
			return ushort.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out outputValue);
		}

		private void PushStateChanges()
		{
			if (StateEdited != null) StateEdited(_lastState);
		}

		private void UpdateStateValue(RegisterValue textBox, ref byte nesRegister, ref byte snesRegister)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) nesRegister = (byte)textBox.Value;
			if (_lastState.Type == ProjectType.Snes) snesRegister = (byte)textBox.Value;
			PushStateChanges();
		}
		private void UpdateStateValue(RegisterValue textBox, ref byte nesRegister, ref ushort snesRegister)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) nesRegister = (byte)textBox.Value;
			if (_lastState.Type == ProjectType.Snes) snesRegister = (ushort)textBox.Value;
			PushStateChanges();
		}
		private void UpdateStateValue(RegisterValue textBox, ref ushort nesRegister, ref ushort snesRegister)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) nesRegister = (ushort)textBox.Value;
			if (_lastState.Type == ProjectType.Snes) snesRegister = (ushort)textBox.Value;
			PushStateChanges();
		}
		private void EditA_ValueChanged(object sender, EventArgs e)
		{
			UpdateStateValue(RegisterA, ref _lastState.NesState.CPU.A, ref _lastState.SnesState.Cpu.A);
		}

		private void EditX_ValueChanged(object sender, EventArgs e)
		{
			UpdateStateValue(RegisterX, ref _lastState.NesState.CPU.X, ref _lastState.SnesState.Cpu.X);
		}

		private void EditY_ValueChanged(object sender, EventArgs e)
		{
			UpdateStateValue(RegisterY, ref _lastState.NesState.CPU.Y, ref _lastState.SnesState.Cpu.Y);
		}

		private void EditPC_ValueChanged(object sender, EventArgs e)
		{
			ushort dummy = 0;
			UpdateStateValue(RegisterPC, ref _lastState.NesState.CPU.PC, ref dummy);
		}

		private void EditSP_ValueChanged(object sender, EventArgs e)
		{
			UpdateStateValue(RegisterSP, ref _lastState.NesState.CPU.SP, ref _lastState.SnesState.Cpu.SP);
		}
		private void EditDP_ValueChanged(object sender, EventArgs e)
		{
			ushort dummy = 0;
			UpdateStateValue(RegisterDP, ref dummy, ref _lastState.SnesState.Cpu.D);
		}

		private void EditDB_ValueChanged(object sender, EventArgs e)
		{
			byte dummy = 0;
			UpdateStateValue(RegisterDB, ref dummy, ref _lastState.SnesState.Cpu.DBR);
		}

		private void EditP_ValueChanged(object sender, EventArgs e)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) _lastState.NesState.CPU.PS = (byte)RegisterP.Value;
			if (_lastState.Type == ProjectType.Snes) _lastState.SnesState.Cpu.PS = (ProcFlags)RegisterP.Value;

			UpdateStates(_lastState);
			PushStateChanges();
		}
		private void FlagChanged(object sender, EventArgs e)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes)
			{
				_lastState.NesState.CPU.PS = (byte) (
					(_lastState.NesState.CPU.PS & 0b00110000) // ignored flags
					| (CheckC.Checked ? 0b00000001 : 0)
					| (CheckZ.Checked ? 0b00000010 : 0)
					| (CheckIrq.Checked ? 0b00000100 : 0)
					| (CheckDec.Checked ? 0b00001000 : 0)
					| (CheckV.Checked ? 0b01000000 : 0)
					| (CheckN.Checked ? 0b10000000 : 0)
				);
			}
			if (_lastState.Type == ProjectType.Snes)
			{
				_lastState.SnesState.Cpu.PS = (
					(CheckM.Checked ? ProcFlags.MemoryMode8 : 0)
					| (CheckX.Checked ? ProcFlags.IndexMode8 : 0)
					| (CheckC.Checked ? ProcFlags.Carry : 0)
					| (CheckZ.Checked ? ProcFlags.Zero : 0)
					| (CheckIrq.Checked ? ProcFlags.IrqDisable : 0)
					| (CheckDec.Checked ? ProcFlags.Decimal : 0)
					| (CheckV.Checked ? ProcFlags.Overflow : 0)
					| (CheckN.Checked ? ProcFlags.Negative : 0)
				);
				_lastState.SnesState.Cpu.EmulationMode = CheckEmu.Checked;
			}
			UpdateStates(_lastState);
			PushStateChanges();
		}

		private void NesPpuCtrlChanged(object sender, EventArgs e)
		{
			if (_loading || _lastState == null) return;
			_lastState.NesState.PPU.ControlFlags.VBlank = (byte)(nesEnableNmi.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.LargeSprites = (byte)(nesLargeSprites.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.VerticalWrite = (byte)(nesVramDown.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.BackgroundPatternAddr = (byte)(nesBg1000.Checked ? 0x1000 : 0);
			_lastState.NesState.PPU.ControlFlags.SpritePatternAddr = (byte)(nesSprites1000.Checked ? 0x1000 : 0);

			_lastState.NesState.PPU.ControlFlags.IntensifyBlue = (byte)(nesEmphasizeB.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.IntensifyGreen = (byte)(nesEmphasizeG.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.IntensifyRed = (byte)(nesEmphasizeR.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.SpritesEnabled = (byte)(nesShowSprites.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.BackgroundEnabled = (byte)(nesShowBg.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.SpriteMask = (byte)(nesShowLeftSprites.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.BackgroundMask = (byte)(nesShowLeftBg.Checked ? 1 : 0);
			_lastState.NesState.PPU.ControlFlags.Grayscale = (byte)(nesGreyscale.Checked ? 1 : 0);

			_lastState.NesState.PPU.StatusFlags.VerticalBlank = (byte)(nesVblankFlag.Checked ? 1 : 0);
			_lastState.NesState.PPU.StatusFlags.Sprite0Hit = (byte)(nesSprite0.Checked ? 1 : 0);
			_lastState.NesState.PPU.StatusFlags.SpriteOverflow = (byte)(nesSpriteOverflow.Checked ? 1 : 0);

			PushStateChanges();

		}

		private byte[] _cpuMemory = new byte[0];
		private int _stackPointer = 0;
		private Events _moduleEvents;

		private void StackSizeChanged(object sender, EventArgs e)
		{
			topOfStack.Width = stack24bit.Checked ? 50 : stack16bit.Checked ? 34 : 22;
			RefreshStack();
		}

		private void RefreshStack()
		{
			var stackPage = _stackPointer & 0xff00;
			var stackBottom = stackPage + 0x100;
			var stackOffset = _stackPointer & 0xff;
			var first = true;
			if (_cpuMemory.Length < stackBottom) return;

				var stringBuilder = new StringBuilder((stackBottom - _stackPointer) * 4);
			var topSize = stack24bit.Checked ? 3 : stack16bit.Checked ? 2 : 1;
			for (var i = topSize; i > 0; i--)
			{
				stringBuilder.Append(Convert.ToString(_cpuMemory[stackPage | ((stackOffset + i) & 0xff)], 16).PadLeft(2, '0'));
				if (i == 3) stringBuilder.Append(':');
			}
			topOfStack.Text = stringBuilder.ToString();

			stringBuilder = new StringBuilder((stackBottom - _stackPointer) * 4);
			for (var i = _stackPointer + 1; i < stackBottom; i++)
			{
				if (!first) stringBuilder.Append(", ");
				stringBuilder.Append(Convert.ToString(_cpuMemory[i], 16).PadLeft(2, '0'));
				first = false;
			}
			completeStack.Text = stringBuilder.ToString();
		}
	}
}
