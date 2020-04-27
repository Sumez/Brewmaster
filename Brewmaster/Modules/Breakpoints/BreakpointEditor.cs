using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.Settings
{
	public class BreakpointEditor : Form
	{
		private readonly AsmProject _project;
		private Button _cancelButton;
		private RadioButton _breakBySourceLine;
		private RadioButton _breakBySymbol;
		private RadioButton _breakByAddress;
		private ComboBox _fileSelect;
		private TextBox _fileLineNumber;
		private TextBox _symbol;
		private TextBox _fromAddress;
		private TextBox _toAddress;
		private RadioButton _addressFromRom;
		private RadioButton _addressFromCpu;
		private RadioButton _addressFromVram;
		private CheckBox _breakOnRead;
		private CheckBox _breakOnWrite;
		private CheckBox _breakOnExecute;
		private TextBox _conditions;
		private Button _okButton;
		public Breakpoint Breakpoint { get; private set; }

		public BreakpointEditor(AsmProject project)
		{
			_project = project;
			InitializeComponent();

			foreach (var file in project.Files.Where(f => f.Type == FileType.Source || f.Type == FileType.Include).OrderBy(f => f.GetRelativePath()))
			{
				_fileSelect.Items.Add(file);
			}

			_breakBySourceLine.CheckedChanged += (s, a) => DisableInputs();
			_breakBySymbol.CheckedChanged += (s, a) => DisableInputs();
			_breakByAddress.CheckedChanged += (s, a) => DisableInputs();
			DisableInputs();

			Breakpoint = new Breakpoint();
		}

		private void DisableInputs()
		{
			_fileSelect.Enabled = _fileLineNumber.Enabled = _breakBySourceLine.Checked;
			_symbol.Enabled = _breakBySymbol.Checked;
			_fromAddress.Enabled = _toAddress.Enabled =
				_addressFromCpu.Enabled = _addressFromRom.Enabled = _addressFromVram.Enabled =
					_breakByAddress.Checked;
		}

		public BreakpointEditor(Breakpoint breakpoint, AsmProject project) : this(project)
		{
			if (breakpoint.File != null) return; // Editing editor breakpoints outside of the text editor windows causes too many potential issues

			Breakpoint = breakpoint;
			if (breakpoint.File != null) _breakBySourceLine.Checked = true;
			else if (breakpoint.Symbol != null) _breakBySymbol.Checked = true;
			else _breakByAddress.Checked = true;
			DisableInputs();

			if (breakpoint.File != null)
			{
				_fileSelect.SelectedItem = breakpoint.File;
				_fileLineNumber.Text = breakpoint.CurrentLine.ToString();
			}
			if (!string.IsNullOrWhiteSpace(breakpoint.Symbol)) _symbol.Text = breakpoint.Symbol;
			if (breakpoint.StartAddress >= 0) _fromAddress.Text = Convert.ToString(breakpoint.StartAddress, 16).ToUpper();
			if (breakpoint.EndAddress.HasValue && breakpoint.EndAddress >= 0) _toAddress.Text = Convert.ToString(breakpoint.EndAddress.Value, 16).ToUpper();

			_breakOnExecute.Checked = breakpoint.Type.HasFlag(Breakpoint.Types.Execute);
			_breakOnRead.Checked = breakpoint.Type.HasFlag(Breakpoint.Types.Read);
			_breakOnWrite.Checked = breakpoint.Type.HasFlag(Breakpoint.Types.Write);

			if (breakpoint.AddressType == Breakpoint.AddressTypes.Cpu) _addressFromCpu.Checked = true;
			if (breakpoint.AddressType == Breakpoint.AddressTypes.PrgRom) _addressFromRom.Checked = true;
			if (breakpoint.AddressType == Breakpoint.AddressTypes.Ppu) _addressFromVram.Checked = true;
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
			System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			this._addressFromCpu = new System.Windows.Forms.RadioButton();
			this._addressFromVram = new System.Windows.Forms.RadioButton();
			this._addressFromRom = new System.Windows.Forms.RadioButton();
			this._breakOnRead = new System.Windows.Forms.CheckBox();
			this._breakOnWrite = new System.Windows.Forms.CheckBox();
			this._breakOnExecute = new System.Windows.Forms.CheckBox();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._breakBySourceLine = new System.Windows.Forms.RadioButton();
			this._breakBySymbol = new System.Windows.Forms.RadioButton();
			this._breakByAddress = new System.Windows.Forms.RadioButton();
			this._fileSelect = new System.Windows.Forms.ComboBox();
			this._fileLineNumber = new System.Windows.Forms.TextBox();
			this._symbol = new System.Windows.Forms.TextBox();
			this._fromAddress = new System.Windows.Forms.TextBox();
			this._toAddress = new System.Windows.Forms.TextBox();
			this._conditions = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			flowLayoutPanel1.SuspendLayout();
			flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(183, 433);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(30, 13);
			label1.TabIndex = 7;
			label1.Text = "Line:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(28, 84);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(33, 13);
			label2.TabIndex = 10;
			label2.Text = "From:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(124, 84);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(23, 13);
			label3.TabIndex = 12;
			label3.Text = "To:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(205, 84);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(50, 13);
			label4.TabIndex = 14;
			label4.Text = "(optional)";
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.Controls.Add(this._addressFromCpu);
			flowLayoutPanel1.Controls.Add(this._addressFromVram);
			flowLayoutPanel1.Controls.Add(this._addressFromRom);
			flowLayoutPanel1.Location = new System.Drawing.Point(31, 103);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(241, 23);
			flowLayoutPanel1.TabIndex = 15;
			// 
			// _addressFromCpu
			// 
			this._addressFromCpu.AutoSize = true;
			this._addressFromCpu.Checked = true;
			this._addressFromCpu.Location = new System.Drawing.Point(3, 3);
			this._addressFromCpu.Name = "_addressFromCpu";
			this._addressFromCpu.Size = new System.Drawing.Size(82, 17);
			this._addressFromCpu.TabIndex = 1;
			this._addressFromCpu.TabStop = true;
			this._addressFromCpu.Text = "CPU / RAM";
			this._addressFromCpu.UseVisualStyleBackColor = true;
			// 
			// _addressFromVram
			// 
			this._addressFromVram.AutoSize = true;
			this._addressFromVram.Location = new System.Drawing.Point(91, 3);
			this._addressFromVram.Name = "_addressFromVram";
			this._addressFromVram.Size = new System.Drawing.Size(89, 17);
			this._addressFromVram.TabIndex = 2;
			this._addressFromVram.Text = "PPU / VRAM";
			this._addressFromVram.UseVisualStyleBackColor = true;
			// 
			// _addressFromRom
			// 
			this._addressFromRom.AutoSize = true;
			this._addressFromRom.Location = new System.Drawing.Point(186, 3);
			this._addressFromRom.Name = "_addressFromRom";
			this._addressFromRom.Size = new System.Drawing.Size(50, 17);
			this._addressFromRom.TabIndex = 0;
			this._addressFromRom.Text = "ROM";
			this._addressFromRom.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.AutoSize = true;
			flowLayoutPanel2.Controls.Add(this._breakOnRead);
			flowLayoutPanel2.Controls.Add(this._breakOnWrite);
			flowLayoutPanel2.Controls.Add(this._breakOnExecute);
			flowLayoutPanel2.Location = new System.Drawing.Point(9, 152);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Size = new System.Drawing.Size(260, 23);
			flowLayoutPanel2.TabIndex = 16;
			// 
			// _breakOnRead
			// 
			this._breakOnRead.AutoSize = true;
			this._breakOnRead.Checked = true;
			this._breakOnRead.CheckState = System.Windows.Forms.CheckState.Checked;
			this._breakOnRead.Location = new System.Drawing.Point(3, 3);
			this._breakOnRead.Name = "_breakOnRead";
			this._breakOnRead.Size = new System.Drawing.Size(52, 17);
			this._breakOnRead.TabIndex = 0;
			this._breakOnRead.Text = "Read";
			this._breakOnRead.UseVisualStyleBackColor = true;
			// 
			// _breakOnWrite
			// 
			this._breakOnWrite.AutoSize = true;
			this._breakOnWrite.Checked = true;
			this._breakOnWrite.CheckState = System.Windows.Forms.CheckState.Checked;
			this._breakOnWrite.Location = new System.Drawing.Point(61, 3);
			this._breakOnWrite.Name = "_breakOnWrite";
			this._breakOnWrite.Size = new System.Drawing.Size(51, 17);
			this._breakOnWrite.TabIndex = 1;
			this._breakOnWrite.Text = "Write";
			this._breakOnWrite.UseVisualStyleBackColor = true;
			// 
			// _breakOnExecute
			// 
			this._breakOnExecute.AutoSize = true;
			this._breakOnExecute.Location = new System.Drawing.Point(118, 3);
			this._breakOnExecute.Name = "_breakOnExecute";
			this._breakOnExecute.Size = new System.Drawing.Size(65, 17);
			this._breakOnExecute.TabIndex = 2;
			this._breakOnExecute.Text = "Execute";
			this._breakOnExecute.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(9, 136);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(53, 13);
			label5.TabIndex = 17;
			label5.Text = "Break on:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(9, 178);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(59, 13);
			label6.TabIndex = 18;
			label6.Text = "Conditions:";
			label6.Visible = false;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(198, 182);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(117, 182);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _breakBySourceLine
			// 
			this._breakBySourceLine.Enabled = false;
			this._breakBySourceLine.Location = new System.Drawing.Point(12, 407);
			this._breakBySourceLine.Name = "_breakBySourceLine";
			this._breakBySourceLine.Size = new System.Drawing.Size(260, 17);
			this._breakBySourceLine.TabIndex = 3;
			this._breakBySourceLine.Text = "Source code:";
			this._breakBySourceLine.UseVisualStyleBackColor = true;
			// 
			// _breakBySymbol
			// 
			this._breakBySymbol.Location = new System.Drawing.Point(12, 9);
			this._breakBySymbol.Name = "_breakBySymbol";
			this._breakBySymbol.Size = new System.Drawing.Size(260, 17);
			this._breakBySymbol.TabIndex = 4;
			this._breakBySymbol.Text = "Symbol:";
			this._breakBySymbol.UseVisualStyleBackColor = true;
			// 
			// _breakByAddress
			// 
			this._breakByAddress.Checked = true;
			this._breakByAddress.Location = new System.Drawing.Point(12, 58);
			this._breakByAddress.Name = "_breakByAddress";
			this._breakByAddress.Size = new System.Drawing.Size(261, 17);
			this._breakByAddress.TabIndex = 5;
			this._breakByAddress.TabStop = true;
			this._breakByAddress.Text = "Specific address:";
			this._breakByAddress.UseVisualStyleBackColor = true;
			// 
			// _fileSelect
			// 
			this._fileSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._fileSelect.FormattingEnabled = true;
			this._fileSelect.Location = new System.Drawing.Point(31, 430);
			this._fileSelect.Name = "_fileSelect";
			this._fileSelect.Size = new System.Drawing.Size(147, 21);
			this._fileSelect.TabIndex = 6;
			// 
			// _fileLineNumber
			// 
			this._fileLineNumber.Location = new System.Drawing.Point(215, 430);
			this._fileLineNumber.Name = "_fileLineNumber";
			this._fileLineNumber.Size = new System.Drawing.Size(43, 20);
			this._fileLineNumber.TabIndex = 8;
			// 
			// _symbol
			// 
			this._symbol.Location = new System.Drawing.Point(31, 32);
			this._symbol.Name = "_symbol";
			this._symbol.Size = new System.Drawing.Size(147, 20);
			this._symbol.TabIndex = 9;
			// 
			// _fromAddress
			// 
			this._fromAddress.Location = new System.Drawing.Point(61, 81);
			this._fromAddress.Name = "_fromAddress";
			this._fromAddress.Size = new System.Drawing.Size(57, 20);
			this._fromAddress.TabIndex = 11;
			// 
			// _toAddress
			// 
			this._toAddress.Location = new System.Drawing.Point(146, 81);
			this._toAddress.Name = "_toAddress";
			this._toAddress.Size = new System.Drawing.Size(57, 20);
			this._toAddress.TabIndex = 13;
			// 
			// _conditions
			// 
			this._conditions.AcceptsReturn = true;
			this._conditions.Location = new System.Drawing.Point(12, 194);
			this._conditions.Multiline = true;
			this._conditions.Name = "_conditions";
			this._conditions.Size = new System.Drawing.Size(260, 86);
			this._conditions.TabIndex = 19;
			this._conditions.Visible = false;
			// 
			// BreakpointEditor
			// 
			this.AcceptButton = this._okButton;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(285, 217);
			this.Controls.Add(this._conditions);
			this.Controls.Add(label6);
			this.Controls.Add(label5);
			this.Controls.Add(flowLayoutPanel2);
			this.Controls.Add(flowLayoutPanel1);
			this.Controls.Add(label4);
			this.Controls.Add(this._toAddress);
			this.Controls.Add(label3);
			this.Controls.Add(this._fromAddress);
			this.Controls.Add(label2);
			this.Controls.Add(this._symbol);
			this.Controls.Add(this._fileLineNumber);
			this.Controls.Add(label1);
			this.Controls.Add(this._fileSelect);
			this.Controls.Add(this._breakByAddress);
			this.Controls.Add(this._breakBySymbol);
			this.Controls.Add(this._breakBySourceLine);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BreakpointEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Breakpoint";
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			flowLayoutPanel2.ResumeLayout(false);
			flowLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void _okButton_Click(object sender, System.EventArgs e)
		{
			var breakpoint = Breakpoint;
			if (_breakBySourceLine.Checked)
			{
				int lineNumber;
				if (!int.TryParse(_fileLineNumber.Text, out lineNumber) || lineNumber < 1)
				{
					Program.Error("Invalid line number. Must be a valid number.");
					return;
				}

				if (breakpoint.File != _fileSelect.SelectedItem || breakpoint.CurrentLine != lineNumber)
				{
					breakpoint.File = _fileSelect.SelectedItem as AsmProjectFile;
					breakpoint.BuildLine = breakpoint.CurrentLine = lineNumber;
				}
				breakpoint.Symbol = null;
				breakpoint.StartAddress = -1;
				breakpoint.EndAddress = null;
				breakpoint.AddressType = Breakpoint.AddressTypes.PrgRom;
				_breakOnExecute.Checked = true;
			}
			else if (_breakBySymbol.Checked)
			{
				breakpoint.File = null;
				breakpoint.Symbol = _symbol.Text.Trim();
				breakpoint.StartAddress = -1;
				breakpoint.EndAddress = null;
				breakpoint.AddressType = Breakpoint.AddressTypes.Cpu;
				breakpoint.UpdateFromSymbols(_project.DebugSymbols);
			}
			else
			{
				int startAddress;
				int endAddress;
				if (!int.TryParse(_fromAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out startAddress))
				{
					Program.Error("Invalid start address. Must be a hex value.");
					return;
				}
				if (string.IsNullOrWhiteSpace(_toAddress.Text)) endAddress = -1;
				else if (!int.TryParse(_toAddress.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out endAddress))
				{
					Program.Error("Invalid end address. Must be a hex value.");
					return;
				}
				breakpoint.File = null;
				breakpoint.Symbol = null;
				breakpoint.StartAddress = startAddress;
				breakpoint.EndAddress = endAddress >= 0 ? endAddress : null as int?;
				if (_addressFromCpu.Checked) breakpoint.AddressType = Breakpoint.AddressTypes.Cpu;
				if (_addressFromRom.Checked) breakpoint.AddressType = Breakpoint.AddressTypes.PrgRom;
				if (_addressFromVram.Checked) breakpoint.AddressType = Breakpoint.AddressTypes.Ppu;
			}

			breakpoint.Type = 0;
			if (_breakOnExecute.Checked) breakpoint.Type |= Breakpoint.Types.Execute;
			if (_breakOnRead.Checked) breakpoint.Type |= Breakpoint.Types.Read;
			if (_breakOnWrite.Checked) breakpoint.Type |= Breakpoint.Types.Write;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}