using System;
using System.Globalization;
using System.Windows.Forms;
using Brewmaster.Emulation;
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

		private void InitializeFlagEvents()
		{
			CheckC.CheckedChanged += FlagChanged;
			CheckZ.CheckedChanged += FlagChanged;
			CheckIrq.CheckedChanged += FlagChanged;
			CheckDec.CheckedChanged += FlagChanged;
			CheckV.CheckedChanged += FlagChanged;
			CheckN.CheckedChanged += FlagChanged;
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

		private RegisterState _lastState = null;
		private bool _loading = false;
		public void UpdateStates(RegisterState registerState)
		{
			_loading = true;
			_lastState = registerState;
			if (registerState.Type == ProjectType.Nes) UpdateNesState(registerState.NesState);
			if (registerState.Type == ProjectType.Snes) UpdateSnesState(registerState.SnesState);
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

			CheckC.Checked = (state.CPU.PS & (1 << 0)) != 0;
			CheckZ.Checked = (state.CPU.PS & (1 << 1)) != 0;
			CheckIrq.Checked = (state.CPU.PS & (1 << 2)) != 0;
			CheckDec.Checked = (state.CPU.PS & (1 << 3)) != 0;
			CheckV.Checked = (state.CPU.PS & (1 << 6)) != 0;
			CheckN.Checked = (state.CPU.PS & (1 << 7)) != 0;

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
		public Action<RegisterState> StateEdited;

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

	}
}
