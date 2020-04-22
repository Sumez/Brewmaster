namespace BrewMaster.StatusView
{
	partial class CpuStatus
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
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox RegisterGroup;
			System.Windows.Forms.Label labelP;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox FlagGroup;
			System.Windows.Forms.TableLayoutPanel FlagTable;
			System.Windows.Forms.GroupBox TimingGroup;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Panel panel3;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.ToolTip RegisterToolTip;
			this.EditP = new System.Windows.Forms.TextBox();
			this.EditSP = new System.Windows.Forms.TextBox();
			this.EditPC = new System.Windows.Forms.TextBox();
			this.EditY = new System.Windows.Forms.TextBox();
			this.EditX = new System.Windows.Forms.TextBox();
			this.EditA = new System.Windows.Forms.TextBox();
			this.CheckN = new System.Windows.Forms.CheckBox();
			this.CheckV = new System.Windows.Forms.CheckBox();
			this.CheckDec = new System.Windows.Forms.CheckBox();
			this.CheckIrq = new System.Windows.Forms.CheckBox();
			this.CheckZ = new System.Windows.Forms.CheckBox();
			this.CheckC = new System.Windows.Forms.CheckBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.ResetCycle = new System.Windows.Forms.Button();
			this.ResetFrame = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.EditCycle = new System.Windows.Forms.TextBox();
			this.EditFrame = new System.Windows.Forms.TextBox();
			this.EditScanline = new System.Windows.Forms.TextBox();
			this.EditPixel = new System.Windows.Forms.TextBox();
			RegisterGroup = new System.Windows.Forms.GroupBox();
			labelP = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			FlagGroup = new System.Windows.Forms.GroupBox();
			FlagTable = new System.Windows.Forms.TableLayoutPanel();
			TimingGroup = new System.Windows.Forms.GroupBox();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			panel3 = new System.Windows.Forms.Panel();
			label6 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			RegisterToolTip = new System.Windows.Forms.ToolTip(this.components);
			RegisterGroup.SuspendLayout();
			FlagGroup.SuspendLayout();
			FlagTable.SuspendLayout();
			TimingGroup.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// RegisterGroup
			// 
			RegisterGroup.Controls.Add(this.EditP);
			RegisterGroup.Controls.Add(labelP);
			RegisterGroup.Controls.Add(this.EditSP);
			RegisterGroup.Controls.Add(label5);
			RegisterGroup.Controls.Add(this.EditPC);
			RegisterGroup.Controls.Add(label4);
			RegisterGroup.Controls.Add(this.EditY);
			RegisterGroup.Controls.Add(label3);
			RegisterGroup.Controls.Add(this.EditX);
			RegisterGroup.Controls.Add(label2);
			RegisterGroup.Controls.Add(this.EditA);
			RegisterGroup.Controls.Add(label1);
			RegisterGroup.Dock = System.Windows.Forms.DockStyle.Top;
			RegisterGroup.Location = new System.Drawing.Point(2, 0);
			RegisterGroup.Name = "RegisterGroup";
			RegisterGroup.Size = new System.Drawing.Size(349, 45);
			RegisterGroup.TabIndex = 0;
			RegisterGroup.TabStop = false;
			RegisterGroup.Text = "Registers";
			// 
			// EditP
			// 
			this.EditP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditP.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditP.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditP.Location = new System.Drawing.Point(241, 16);
			this.EditP.MaxLength = 2;
			this.EditP.Name = "EditP";
			this.EditP.Size = new System.Drawing.Size(22, 20);
			this.EditP.TabIndex = 6;
			this.EditP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditP.TextChanged += new System.EventHandler(this.EditP_TextChanged);
			// 
			// labelP
			// 
			labelP.AutoSize = true;
			labelP.Dock = System.Windows.Forms.DockStyle.Left;
			labelP.Location = new System.Drawing.Point(224, 16);
			labelP.Name = "labelP";
			labelP.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			labelP.Size = new System.Drawing.Size(17, 16);
			labelP.TabIndex = 5;
			labelP.Text = "P";
			RegisterToolTip.SetToolTip(labelP, "Processor status (holds flags shown below)");
			// 
			// EditSP
			// 
			this.EditSP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditSP.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditSP.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditSP.Location = new System.Drawing.Point(202, 16);
			this.EditSP.MaxLength = 2;
			this.EditSP.Name = "EditSP";
			this.EditSP.Size = new System.Drawing.Size(22, 20);
			this.EditSP.TabIndex = 7;
			this.EditSP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditSP.TextChanged += new System.EventHandler(this.EditSP_TextChanged);
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Dock = System.Windows.Forms.DockStyle.Left;
			label5.Location = new System.Drawing.Point(178, 16);
			label5.Name = "label5";
			label5.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label5.Size = new System.Drawing.Size(24, 16);
			label5.TabIndex = 4;
			label5.Text = "SP";
			RegisterToolTip.SetToolTip(label5, "Stack pointer ($01xx)");
			// 
			// EditPC
			// 
			this.EditPC.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditPC.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditPC.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditPC.Location = new System.Drawing.Point(144, 16);
			this.EditPC.MaxLength = 4;
			this.EditPC.Name = "EditPC";
			this.EditPC.Size = new System.Drawing.Size(34, 20);
			this.EditPC.TabIndex = 8;
			this.EditPC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditPC.TextChanged += new System.EventHandler(this.EditPC_TextChanged);
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Dock = System.Windows.Forms.DockStyle.Left;
			label4.Location = new System.Drawing.Point(120, 16);
			label4.Name = "label4";
			label4.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label4.Size = new System.Drawing.Size(24, 16);
			label4.TabIndex = 3;
			label4.Text = "PC";
			RegisterToolTip.SetToolTip(label4, "Program Counter (address of currently executing code)");
			// 
			// EditY
			// 
			this.EditY.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditY.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditY.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditY.Location = new System.Drawing.Point(98, 16);
			this.EditY.MaxLength = 2;
			this.EditY.Name = "EditY";
			this.EditY.Size = new System.Drawing.Size(22, 20);
			this.EditY.TabIndex = 11;
			this.EditY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditY.TextChanged += new System.EventHandler(this.EditY_TextChanged);
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Dock = System.Windows.Forms.DockStyle.Left;
			label3.Location = new System.Drawing.Point(81, 16);
			label3.Name = "label3";
			label3.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label3.Size = new System.Drawing.Size(17, 16);
			label3.TabIndex = 2;
			label3.Text = "Y";
			RegisterToolTip.SetToolTip(label3, "Y index");
			// 
			// EditX
			// 
			this.EditX.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditX.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditX.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditX.Location = new System.Drawing.Point(59, 16);
			this.EditX.MaxLength = 2;
			this.EditX.Name = "EditX";
			this.EditX.Size = new System.Drawing.Size(22, 20);
			this.EditX.TabIndex = 10;
			this.EditX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditX.TextChanged += new System.EventHandler(this.EditX_TextChanged);
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Dock = System.Windows.Forms.DockStyle.Left;
			label2.Location = new System.Drawing.Point(42, 16);
			label2.Name = "label2";
			label2.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label2.Size = new System.Drawing.Size(17, 16);
			label2.TabIndex = 1;
			label2.Text = "X";
			RegisterToolTip.SetToolTip(label2, "X index");
			// 
			// EditA
			// 
			this.EditA.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditA.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditA.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditA.Location = new System.Drawing.Point(20, 16);
			this.EditA.MaxLength = 2;
			this.EditA.Name = "EditA";
			this.EditA.Size = new System.Drawing.Size(22, 20);
			this.EditA.TabIndex = 9;
			this.EditA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditA.TextChanged += new System.EventHandler(this.EditA_TextChanged);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Dock = System.Windows.Forms.DockStyle.Left;
			label1.Location = new System.Drawing.Point(3, 16);
			label1.Name = "label1";
			label1.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label1.Size = new System.Drawing.Size(17, 16);
			label1.TabIndex = 0;
			label1.Text = "A";
			RegisterToolTip.SetToolTip(label1, "Accumulator");
			// 
			// FlagGroup
			// 
			FlagGroup.Controls.Add(FlagTable);
			FlagGroup.Dock = System.Windows.Forms.DockStyle.Top;
			FlagGroup.Location = new System.Drawing.Point(2, 45);
			FlagGroup.Name = "FlagGroup";
			FlagGroup.Size = new System.Drawing.Size(349, 70);
			FlagGroup.TabIndex = 0;
			FlagGroup.TabStop = false;
			FlagGroup.Text = "Flags";
			// 
			// FlagTable
			// 
			FlagTable.ColumnCount = 3;
			FlagTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			FlagTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			FlagTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			FlagTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			FlagTable.Controls.Add(this.CheckN, 0, 0);
			FlagTable.Controls.Add(this.CheckV, 1, 0);
			FlagTable.Controls.Add(this.CheckDec, 2, 0);
			FlagTable.Controls.Add(this.CheckIrq, 0, 1);
			FlagTable.Controls.Add(this.CheckZ, 1, 1);
			FlagTable.Controls.Add(this.CheckC, 2, 1);
			FlagTable.Dock = System.Windows.Forms.DockStyle.Left;
			FlagTable.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			FlagTable.Location = new System.Drawing.Point(3, 16);
			FlagTable.Name = "FlagTable";
			FlagTable.RowCount = 2;
			FlagTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			FlagTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			FlagTable.Size = new System.Drawing.Size(260, 51);
			FlagTable.TabIndex = 0;
			// 
			// CheckN
			// 
			this.CheckN.AutoSize = true;
			this.CheckN.Location = new System.Drawing.Point(3, 3);
			this.CheckN.Name = "CheckN";
			this.CheckN.Size = new System.Drawing.Size(69, 17);
			this.CheckN.TabIndex = 3;
			this.CheckN.Text = "Negative";
			RegisterToolTip.SetToolTip(this.CheckN, "Indicates a negative value (given two\'s complement). Typically gets set when the " +
        "result of the last operation sets bit 7 of a register");
			this.CheckN.UseVisualStyleBackColor = true;
			// 
			// CheckV
			// 
			this.CheckV.AutoSize = true;
			this.CheckV.Location = new System.Drawing.Point(78, 3);
			this.CheckV.Name = "CheckV";
			this.CheckV.Size = new System.Drawing.Size(68, 17);
			this.CheckV.TabIndex = 4;
			this.CheckV.Text = "Overflow";
			RegisterToolTip.SetToolTip(this.CheckV, "A very complex flag that I can\'t explain in a single tool tip. Look it up, or jus" +
        "t ignore that it exists");
			this.CheckV.UseVisualStyleBackColor = true;
			// 
			// CheckDec
			// 
			this.CheckDec.AutoSize = true;
			this.CheckDec.Location = new System.Drawing.Point(152, 3);
			this.CheckDec.Name = "CheckDec";
			this.CheckDec.Size = new System.Drawing.Size(64, 17);
			this.CheckDec.TabIndex = 5;
			this.CheckDec.Text = "Decimal";
			RegisterToolTip.SetToolTip(this.CheckDec, "If decimal mode is set, additions and subtractions will treat the value as Binary" +
        " Coded Decimal (doesn\'t work on NES)");
			this.CheckDec.UseVisualStyleBackColor = true;
			// 
			// CheckIrq
			// 
			this.CheckIrq.AutoSize = true;
			this.CheckIrq.Location = new System.Drawing.Point(3, 28);
			this.CheckIrq.Name = "CheckIrq";
			this.CheckIrq.Size = new System.Drawing.Size(65, 17);
			this.CheckIrq.TabIndex = 6;
			this.CheckIrq.Text = "Interrupt";
			RegisterToolTip.SetToolTip(this.CheckIrq, "Get set when an IRQ occurs, and cleared when returning from it. Non-NMI IRQs are " +
        "prevented while flag is set");
			this.CheckIrq.UseVisualStyleBackColor = true;
			// 
			// CheckZ
			// 
			this.CheckZ.AutoSize = true;
			this.CheckZ.Location = new System.Drawing.Point(78, 28);
			this.CheckZ.Name = "CheckZ";
			this.CheckZ.Size = new System.Drawing.Size(48, 17);
			this.CheckZ.TabIndex = 7;
			this.CheckZ.Text = "Zero";
			RegisterToolTip.SetToolTip(this.CheckZ, "Zero flag gets set when a comparison results in equal values, or a load, math or " +
        "logical operation results in 0. If the result is the opposite, the flag gets cle" +
        "ared");
			this.CheckZ.UseVisualStyleBackColor = true;
			// 
			// CheckC
			// 
			this.CheckC.AutoSize = true;
			this.CheckC.Location = new System.Drawing.Point(152, 28);
			this.CheckC.Name = "CheckC";
			this.CheckC.Size = new System.Drawing.Size(50, 17);
			this.CheckC.TabIndex = 8;
			this.CheckC.Text = "Carry";
			RegisterToolTip.SetToolTip(this.CheckC, "Carry flag indicates a carry from math or shift operations, allowing you to store" +
        " bigger values across multiple bytes. Also used to detect smaller/greater number" +
        "s after a comparison");
			this.CheckC.UseVisualStyleBackColor = true;
			// 
			// TimingGroup
			// 
			TimingGroup.AutoSize = true;
			TimingGroup.Controls.Add(this.panel2);
			TimingGroup.Controls.Add(this.panel1);
			TimingGroup.Controls.Add(panel3);
			TimingGroup.Dock = System.Windows.Forms.DockStyle.Top;
			TimingGroup.Location = new System.Drawing.Point(2, 115);
			TimingGroup.Name = "TimingGroup";
			TimingGroup.Size = new System.Drawing.Size(349, 92);
			TimingGroup.TabIndex = 1;
			TimingGroup.TabStop = false;
			TimingGroup.Text = "Timing";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.ResetCycle);
			this.panel2.Controls.Add(this.ResetFrame);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 68);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(343, 21);
			this.panel2.TabIndex = 2;
			// 
			// ResetCycle
			// 
			this.ResetCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ResetCycle.Location = new System.Drawing.Point(138, 0);
			this.ResetCycle.Name = "ResetCycle";
			this.ResetCycle.Size = new System.Drawing.Size(69, 21);
			this.ResetCycle.TabIndex = 1;
			this.ResetCycle.Text = "Reset";
			this.ResetCycle.UseVisualStyleBackColor = true;
			this.ResetCycle.Click += new System.EventHandler(this.ResetCycle_Click);
			// 
			// ResetFrame
			// 
			this.ResetFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ResetFrame.Location = new System.Drawing.Point(39, 0);
			this.ResetFrame.Name = "ResetFrame";
			this.ResetFrame.Size = new System.Drawing.Size(39, 21);
			this.ResetFrame.TabIndex = 0;
			this.ResetFrame.Text = "Reset";
			this.ResetFrame.UseVisualStyleBackColor = true;
			this.ResetFrame.Click += new System.EventHandler(this.ResetFrame_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.EditCycle);
			this.panel1.Controls.Add(label7);
			this.panel1.Controls.Add(this.EditFrame);
			this.panel1.Controls.Add(label8);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 42);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(343, 26);
			this.panel1.TabIndex = 0;
			// 
			// EditCycle
			// 
			this.EditCycle.BackColor = System.Drawing.SystemColors.Window;
			this.EditCycle.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditCycle.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditCycle.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditCycle.Location = new System.Drawing.Point(138, 0);
			this.EditCycle.MaxLength = 2;
			this.EditCycle.Name = "EditCycle";
			this.EditCycle.ReadOnly = true;
			this.EditCycle.Size = new System.Drawing.Size(69, 20);
			this.EditCycle.TabIndex = 16;
			this.EditCycle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Dock = System.Windows.Forms.DockStyle.Left;
			label7.Location = new System.Drawing.Point(78, 0);
			label7.Name = "label7";
			label7.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label7.Size = new System.Drawing.Size(60, 16);
			label7.TabIndex = 13;
			label7.Text = "CPU cycle";
			// 
			// EditFrame
			// 
			this.EditFrame.BackColor = System.Drawing.SystemColors.Window;
			this.EditFrame.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditFrame.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditFrame.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditFrame.Location = new System.Drawing.Point(39, 0);
			this.EditFrame.MaxLength = 2;
			this.EditFrame.Name = "EditFrame";
			this.EditFrame.ReadOnly = true;
			this.EditFrame.Size = new System.Drawing.Size(39, 20);
			this.EditFrame.TabIndex = 15;
			this.EditFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Dock = System.Windows.Forms.DockStyle.Left;
			label8.Location = new System.Drawing.Point(0, 0);
			label8.Name = "label8";
			label8.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label8.Size = new System.Drawing.Size(39, 16);
			label8.TabIndex = 12;
			label8.Text = "Frame";
			// 
			// panel3
			// 
			panel3.Controls.Add(this.EditScanline);
			panel3.Controls.Add(label6);
			panel3.Controls.Add(this.EditPixel);
			panel3.Controls.Add(label9);
			panel3.Dock = System.Windows.Forms.DockStyle.Top;
			panel3.Location = new System.Drawing.Point(3, 16);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(343, 26);
			panel3.TabIndex = 3;
			// 
			// EditScanline
			// 
			this.EditScanline.BackColor = System.Drawing.SystemColors.Window;
			this.EditScanline.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditScanline.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditScanline.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditScanline.Location = new System.Drawing.Point(138, 0);
			this.EditScanline.MaxLength = 2;
			this.EditScanline.Name = "EditScanline";
			this.EditScanline.ReadOnly = true;
			this.EditScanline.Size = new System.Drawing.Size(39, 20);
			this.EditScanline.TabIndex = 17;
			this.EditScanline.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label6
			// 
			label6.Dock = System.Windows.Forms.DockStyle.Left;
			label6.Location = new System.Drawing.Point(78, 0);
			label6.Name = "label6";
			label6.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label6.Size = new System.Drawing.Size(60, 26);
			label6.TabIndex = 14;
			label6.Text = "Scanline";
			// 
			// EditPixel
			// 
			this.EditPixel.BackColor = System.Drawing.SystemColors.Window;
			this.EditPixel.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditPixel.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditPixel.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditPixel.Location = new System.Drawing.Point(39, 0);
			this.EditPixel.MaxLength = 2;
			this.EditPixel.Name = "EditPixel";
			this.EditPixel.ReadOnly = true;
			this.EditPixel.Size = new System.Drawing.Size(39, 20);
			this.EditPixel.TabIndex = 19;
			this.EditPixel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label9
			// 
			label9.Dock = System.Windows.Forms.DockStyle.Left;
			label9.Location = new System.Drawing.Point(0, 0);
			label9.Name = "label9";
			label9.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label9.Size = new System.Drawing.Size(39, 26);
			label9.TabIndex = 18;
			label9.Text = "Pixel";
			// 
			// CpuStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(TimingGroup);
			this.Controls.Add(FlagGroup);
			this.Controls.Add(RegisterGroup);
			this.MinimumSize = new System.Drawing.Size(275, 0);
			this.Name = "CpuStatus";
			this.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.Size = new System.Drawing.Size(351, 281);
			RegisterGroup.ResumeLayout(false);
			RegisterGroup.PerformLayout();
			FlagGroup.ResumeLayout(false);
			FlagTable.ResumeLayout(false);
			FlagTable.PerformLayout();
			TimingGroup.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox EditP;
		private System.Windows.Forms.TextBox EditSP;
		private System.Windows.Forms.TextBox EditPC;
		private System.Windows.Forms.TextBox EditY;
		private System.Windows.Forms.TextBox EditX;
		private System.Windows.Forms.TextBox EditA;
		private System.Windows.Forms.CheckBox CheckN;
		private System.Windows.Forms.CheckBox CheckV;
		private System.Windows.Forms.CheckBox CheckDec;
		private System.Windows.Forms.CheckBox CheckIrq;
		private System.Windows.Forms.CheckBox CheckZ;
		private System.Windows.Forms.CheckBox CheckC;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox EditScanline;
		private System.Windows.Forms.TextBox EditCycle;
		private System.Windows.Forms.TextBox EditFrame;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button ResetCycle;
		private System.Windows.Forms.Button ResetFrame;
		private System.Windows.Forms.TextBox EditPixel;
	}
}
