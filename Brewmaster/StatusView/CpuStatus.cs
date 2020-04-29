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
			switch (type)
			{
				case ProjectType.Nes:
					EditA.Width = EditX.Width = EditY.Width = EditSP.Width = 22;
					EditPC.Width = 34;
					break;
				case ProjectType.Snes:
					EditA.Width = EditX.Width = EditY.Width = EditSP.Width = 34;
					EditPC.Width = 50;
					break;
			}
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
			EditPC.Text = string.Format("{0}:{1}", ParseNumber(state.Cpu.K, 1), ParseNumber(state.Cpu.PC, 2));
			UpdateTextBox(EditP, (int)state.Cpu.PS, 1);
			UpdateTextBox(EditA, state.Cpu.A, (state.Cpu.PS & ProcFlags.MemoryMode8) != 0 ? 1 : 2);
			UpdateTextBox(EditX, state.Cpu.X, (state.Cpu.PS & ProcFlags.IndexMode8) != 0 ? 1 : 2);
			UpdateTextBox(EditY, state.Cpu.Y, (state.Cpu.PS & ProcFlags.IndexMode8) != 0 ? 1 : 2);
			UpdateTextBox(EditSP, state.Cpu.SP, 2);
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
			UpdateTextBox(EditPC, state.CPU.PC, 2);
			UpdateTextBox(EditP, state.CPU.PS, 1);
			UpdateTextBox(EditA, state.CPU.A, 1);
			UpdateTextBox(EditX, state.CPU.X, 1);
			UpdateTextBox(EditY, state.CPU.Y, 1);
			UpdateTextBox(EditSP, state.CPU.SP, 1);
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

		private void UpdateStateValue(TextBox textBox, ref byte nesRegister, ref ushort snesRegister)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) Parse(textBox.Text, out nesRegister);
			if (_lastState.Type == ProjectType.Snes) Parse(textBox.Text, out snesRegister);
			PushStateChanges();
		}
		private void UpdateStateValue(TextBox textBox, ref ushort nesRegister, ref ushort snesRegister)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) Parse(textBox.Text, out nesRegister);
			if (_lastState.Type == ProjectType.Snes) Parse(textBox.Text, out snesRegister);
			PushStateChanges();
		}
		private void EditA_TextChanged(object sender, EventArgs e)
		{
			UpdateStateValue(EditA, ref _lastState.NesState.CPU.A, ref _lastState.SnesState.Cpu.A);
		}

		private void EditX_TextChanged(object sender, EventArgs e)
		{
			UpdateStateValue(EditX, ref _lastState.NesState.CPU.X, ref _lastState.SnesState.Cpu.X);
		}

		private void EditY_TextChanged(object sender, EventArgs e)
		{
			UpdateStateValue(EditY, ref _lastState.NesState.CPU.Y, ref _lastState.SnesState.Cpu.Y);
		}

		private void EditPC_TextChanged(object sender, EventArgs e)
		{
			ushort dummy = 0;
			UpdateStateValue(EditPC, ref _lastState.NesState.CPU.PC, ref dummy);
		}

		private void EditSP_TextChanged(object sender, EventArgs e)
		{
			UpdateStateValue(EditSP, ref _lastState.NesState.CPU.SP, ref _lastState.SnesState.Cpu.SP);
		}

		private void EditP_TextChanged(object sender, EventArgs e)
		{
			if (_loading || _lastState == null) return;
			if (_lastState.Type == ProjectType.Nes) Parse(EditP.Text, out _lastState.NesState.CPU.PS);
			if (_lastState.Type == ProjectType.Snes)
			{
				byte snesP;
				if (Parse(EditP.Text, out snesP)) _lastState.SnesState.Cpu.PS = (ProcFlags)snesP;
			}
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
					(_lastState.SnesState.Cpu.PS & (ProcFlags.IndexMode8 | ProcFlags.MemoryMode8)) // ignored flags
					| (CheckC.Checked ? ProcFlags.Carry : 0)
					| (CheckZ.Checked ? ProcFlags.Zero : 0)
					| (CheckIrq.Checked ? ProcFlags.IrqDisable : 0)
					| (CheckDec.Checked ? ProcFlags.Decimal : 0)
					| (CheckV.Checked ? ProcFlags.Overflow : 0)
					| (CheckN.Checked ? ProcFlags.Negative : 0)
				);
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
