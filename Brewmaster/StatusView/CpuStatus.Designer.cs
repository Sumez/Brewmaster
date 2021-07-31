namespace Brewmaster.StatusView
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
			System.Windows.Forms.Panel RegisterGroup;
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CpuStatus));
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label18;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label19;
			System.Windows.Forms.Label labelP;
			System.Windows.Forms.Panel FlagGroup;
			System.Windows.Forms.Panel TimingGroup;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.ToolTip RegisterToolTip;
			System.Windows.Forms.Panel PpuGroup;
			System.Windows.Forms.Panel panel4;
			System.Windows.Forms.Label label15;
			System.Windows.Forms.Label label13;
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Panel stackGroup;
			Brewmaster.StatusView.HorizontalLine horizontalLine5;
			Brewmaster.StatusView.HorizontalLine horizontalLine4;
			Brewmaster.StatusView.HorizontalLine horizontalLine3;
			Brewmaster.StatusView.HorizontalLine horizontalLine2;
			Brewmaster.StatusView.HorizontalLine horizontalLine1;
			this.registerPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.panelA = new System.Windows.Forms.Panel();
			this.RegisterA = new Brewmaster.StatusView.RegisterValue();
			this.panelX = new System.Windows.Forms.Panel();
			this.RegisterX = new Brewmaster.StatusView.RegisterValue();
			this.panelY = new System.Windows.Forms.Panel();
			this.RegisterY = new Brewmaster.StatusView.RegisterValue();
			this.panelPC = new System.Windows.Forms.Panel();
			this.RegisterPC = new Brewmaster.StatusView.RegisterValue();
			this.panelDB = new System.Windows.Forms.Panel();
			this.RegisterDB = new Brewmaster.StatusView.RegisterValue();
			this.panelSP = new System.Windows.Forms.Panel();
			this.RegisterSP = new Brewmaster.StatusView.RegisterValue();
			this.panelDP = new System.Windows.Forms.Panel();
			this.RegisterDP = new Brewmaster.StatusView.RegisterValue();
			this.panelP = new System.Windows.Forms.Panel();
			this.RegisterP = new Brewmaster.StatusView.RegisterValue();
			this.flagPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.panel7 = new System.Windows.Forms.Panel();
			this.CheckDec = new System.Windows.Forms.CheckBox();
			this.CheckV = new System.Windows.Forms.CheckBox();
			this.CheckN = new System.Windows.Forms.CheckBox();
			this.panel8 = new System.Windows.Forms.Panel();
			this.CheckC = new System.Windows.Forms.CheckBox();
			this.CheckZ = new System.Windows.Forms.CheckBox();
			this.CheckIrq = new System.Windows.Forms.CheckBox();
			this.snesFlags = new System.Windows.Forms.Panel();
			this.CheckEmu = new System.Windows.Forms.CheckBox();
			this.CheckX = new System.Windows.Forms.CheckBox();
			this.CheckM = new System.Windows.Forms.CheckBox();
			this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
			this.panel9 = new System.Windows.Forms.Panel();
			this.EditPixel = new System.Windows.Forms.TextBox();
			this.panel10 = new System.Windows.Forms.Panel();
			this.EditScanline = new System.Windows.Forms.TextBox();
			this.panel11 = new System.Windows.Forms.Panel();
			this.ResetFrame = new System.Windows.Forms.Button();
			this.ResetCycle = new System.Windows.Forms.Button();
			this.EditCycle = new System.Windows.Forms.TextBox();
			this.EditFrame = new System.Windows.Forms.TextBox();
			this.nesEnableNmi = new System.Windows.Forms.CheckBox();
			this.nesLargeSprites = new System.Windows.Forms.CheckBox();
			this.nesVramDown = new System.Windows.Forms.CheckBox();
			this.nesBg1000 = new System.Windows.Forms.CheckBox();
			this.nesSprites1000 = new System.Windows.Forms.CheckBox();
			this.nesShowSprites = new System.Windows.Forms.CheckBox();
			this.nesShowBg = new System.Windows.Forms.CheckBox();
			this.nesGreyscale = new System.Windows.Forms.CheckBox();
			this.nesShowLeftSprites = new System.Windows.Forms.CheckBox();
			this.nesShowLeftBg = new System.Windows.Forms.CheckBox();
			this.nesEmphasizeB = new System.Windows.Forms.CheckBox();
			this.nesEmphasizeG = new System.Windows.Forms.CheckBox();
			this.nesEmphasizeR = new System.Windows.Forms.CheckBox();
			this.nesVblankFlag = new System.Windows.Forms.CheckBox();
			this.nesSprite0 = new System.Windows.Forms.CheckBox();
			this.nesSpriteOverflow = new System.Windows.Forms.CheckBox();
			this.nesPpuPanel = new System.Windows.Forms.Panel();
			this.nesPpuTRegister = new System.Windows.Forms.TextBox();
			this.nesNtAddr = new System.Windows.Forms.TextBox();
			this.nesVramAddr = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.panel6 = new System.Windows.Forms.Panel();
			this.completeStack = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.stack8bit = new System.Windows.Forms.RadioButton();
			this.stack16bit = new System.Windows.Forms.RadioButton();
			this.stack24bit = new System.Windows.Forms.RadioButton();
			this.topOfStack = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.expandPpu = new Brewmaster.StatusView.ExpandButton();
			this.expandStack = new Brewmaster.StatusView.ExpandButton();
			this.expandTiming = new Brewmaster.StatusView.ExpandButton();
			this.expandFlags = new Brewmaster.StatusView.ExpandButton();
			this.expandCpu = new Brewmaster.StatusView.ExpandButton();
			RegisterGroup = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label18 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label19 = new System.Windows.Forms.Label();
			labelP = new System.Windows.Forms.Label();
			FlagGroup = new System.Windows.Forms.Panel();
			TimingGroup = new System.Windows.Forms.Panel();
			label9 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			RegisterToolTip = new System.Windows.Forms.ToolTip(this.components);
			PpuGroup = new System.Windows.Forms.Panel();
			panel4 = new System.Windows.Forms.Panel();
			label15 = new System.Windows.Forms.Label();
			label13 = new System.Windows.Forms.Label();
			label14 = new System.Windows.Forms.Label();
			stackGroup = new System.Windows.Forms.Panel();
			horizontalLine5 = new Brewmaster.StatusView.HorizontalLine();
			horizontalLine4 = new Brewmaster.StatusView.HorizontalLine();
			horizontalLine3 = new Brewmaster.StatusView.HorizontalLine();
			horizontalLine2 = new Brewmaster.StatusView.HorizontalLine();
			horizontalLine1 = new Brewmaster.StatusView.HorizontalLine();
			RegisterGroup.SuspendLayout();
			this.registerPanel.SuspendLayout();
			this.panelA.SuspendLayout();
			this.panelX.SuspendLayout();
			this.panelY.SuspendLayout();
			this.panelPC.SuspendLayout();
			this.panelDB.SuspendLayout();
			this.panelSP.SuspendLayout();
			this.panelDP.SuspendLayout();
			this.panelP.SuspendLayout();
			FlagGroup.SuspendLayout();
			this.flagPanel.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel8.SuspendLayout();
			this.snesFlags.SuspendLayout();
			TimingGroup.SuspendLayout();
			this.flowLayoutPanel4.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel11.SuspendLayout();
			PpuGroup.SuspendLayout();
			this.nesPpuPanel.SuspendLayout();
			panel4.SuspendLayout();
			stackGroup.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel5.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// RegisterGroup
			// 
			RegisterGroup.AutoSize = true;
			RegisterGroup.Controls.Add(this.registerPanel);
			RegisterGroup.Dock = System.Windows.Forms.DockStyle.Top;
			RegisterGroup.Location = new System.Drawing.Point(2, 20);
			RegisterGroup.Name = "RegisterGroup";
			RegisterGroup.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			RegisterGroup.Size = new System.Drawing.Size(620, 26);
			RegisterGroup.TabIndex = 0;
			RegisterGroup.Text = "Registers";
			// 
			// registerPanel
			// 
			this.registerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.registerPanel.AutoSize = true;
			this.registerPanel.Controls.Add(this.panelA);
			this.registerPanel.Controls.Add(this.panelX);
			this.registerPanel.Controls.Add(this.panelY);
			this.registerPanel.Controls.Add(this.panelPC);
			this.registerPanel.Controls.Add(this.panelDB);
			this.registerPanel.Controls.Add(this.panelSP);
			this.registerPanel.Controls.Add(this.panelDP);
			this.registerPanel.Controls.Add(this.panelP);
			this.registerPanel.Location = new System.Drawing.Point(0, 3);
			this.registerPanel.Margin = new System.Windows.Forms.Padding(0);
			this.registerPanel.Name = "registerPanel";
			this.registerPanel.Size = new System.Drawing.Size(620, 23);
			this.registerPanel.TabIndex = 12;
			// 
			// panelA
			// 
			this.panelA.AutoSize = true;
			this.panelA.Controls.Add(this.RegisterA);
			this.panelA.Controls.Add(label1);
			this.panelA.Location = new System.Drawing.Point(0, 0);
			this.panelA.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelA.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelA.Name = "panelA";
			this.panelA.Size = new System.Drawing.Size(37, 20);
			this.panelA.TabIndex = 0;
			// 
			// RegisterA
			// 
			this.RegisterA.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterA.DimColor = System.Drawing.Color.Gray;
			this.RegisterA.DimUpperByte = false;
			this.RegisterA.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterA.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterA.Location = new System.Drawing.Point(17, 0);
			this.RegisterA.Name = "RegisterA";
			this.RegisterA.ReadOnly = false;
			this.RegisterA.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterA.Size = new System.Drawing.Size(20, 20);
			this.RegisterA.TabIndex = 1;
			this.RegisterA.Text = "00";
			this.RegisterA.Value = 0;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Dock = System.Windows.Forms.DockStyle.Left;
			label1.Location = new System.Drawing.Point(0, 0);
			label1.Name = "label1";
			label1.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label1.Size = new System.Drawing.Size(17, 16);
			label1.TabIndex = 0;
			label1.Text = "A";
			RegisterToolTip.SetToolTip(label1, resources.GetString("label1.ToolTip"));
			// 
			// panelX
			// 
			this.panelX.AutoSize = true;
			this.panelX.Controls.Add(this.RegisterX);
			this.panelX.Controls.Add(label2);
			this.panelX.Location = new System.Drawing.Point(37, 0);
			this.panelX.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelX.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelX.Name = "panelX";
			this.panelX.Size = new System.Drawing.Size(37, 20);
			this.panelX.TabIndex = 1;
			// 
			// RegisterX
			// 
			this.RegisterX.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterX.DimColor = System.Drawing.Color.Gray;
			this.RegisterX.DimUpperByte = false;
			this.RegisterX.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterX.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterX.Location = new System.Drawing.Point(17, 0);
			this.RegisterX.Name = "RegisterX";
			this.RegisterX.ReadOnly = false;
			this.RegisterX.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterX.Size = new System.Drawing.Size(20, 20);
			this.RegisterX.TabIndex = 2;
			this.RegisterX.Text = "00";
			this.RegisterX.Value = 0;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Dock = System.Windows.Forms.DockStyle.Left;
			label2.Location = new System.Drawing.Point(0, 0);
			label2.Name = "label2";
			label2.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label2.Size = new System.Drawing.Size(17, 16);
			label2.TabIndex = 1;
			label2.Text = "X";
			RegisterToolTip.SetToolTip(label2, "X index\r\nNES:\r\nAn 8-bit value typically used for indexing off an absolute address" +
        ", counting loops, or holding extra values\r\nSNES:\r\n8-bit or 16-bit modes are avai" +
        "lable depending on the X flag.");
			// 
			// panelY
			// 
			this.panelY.AutoSize = true;
			this.panelY.Controls.Add(this.RegisterY);
			this.panelY.Controls.Add(label3);
			this.panelY.Location = new System.Drawing.Point(74, 0);
			this.panelY.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelY.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelY.Name = "panelY";
			this.panelY.Size = new System.Drawing.Size(37, 20);
			this.panelY.TabIndex = 2;
			// 
			// RegisterY
			// 
			this.RegisterY.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterY.DimColor = System.Drawing.Color.Gray;
			this.RegisterY.DimUpperByte = false;
			this.RegisterY.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterY.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterY.Location = new System.Drawing.Point(17, 0);
			this.RegisterY.Name = "RegisterY";
			this.RegisterY.ReadOnly = false;
			this.RegisterY.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterY.Size = new System.Drawing.Size(20, 20);
			this.RegisterY.TabIndex = 3;
			this.RegisterY.Text = "00";
			this.RegisterY.Value = 0;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Dock = System.Windows.Forms.DockStyle.Left;
			label3.Location = new System.Drawing.Point(0, 0);
			label3.Name = "label3";
			label3.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label3.Size = new System.Drawing.Size(17, 16);
			label3.TabIndex = 2;
			label3.Text = "Y";
			RegisterToolTip.SetToolTip(label3, "Y index\r\nNES:\r\nAn 8-bit value typically used for indexing off a static or indirec" +
        "t address, counting loops, or holding extra values\r\nSNES:\r\n8-bit or 16-bit modes" +
        " are available depending on the X flag.");
			// 
			// panelPC
			// 
			this.panelPC.AutoSize = true;
			this.panelPC.Controls.Add(this.RegisterPC);
			this.panelPC.Controls.Add(label4);
			this.panelPC.Location = new System.Drawing.Point(111, 0);
			this.panelPC.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelPC.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelPC.Name = "panelPC";
			this.panelPC.Size = new System.Drawing.Size(44, 20);
			this.panelPC.TabIndex = 3;
			// 
			// RegisterPC
			// 
			this.RegisterPC.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterPC.DimColor = System.Drawing.Color.Gray;
			this.RegisterPC.DimUpperByte = false;
			this.RegisterPC.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterPC.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterPC.Location = new System.Drawing.Point(24, 0);
			this.RegisterPC.Name = "RegisterPC";
			this.RegisterPC.ReadOnly = true;
			this.RegisterPC.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterPC.Size = new System.Drawing.Size(20, 20);
			this.RegisterPC.TabIndex = 4;
			this.RegisterPC.TabStop = false;
			this.RegisterPC.Text = "00";
			this.RegisterPC.Value = 0;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Dock = System.Windows.Forms.DockStyle.Left;
			label4.Location = new System.Drawing.Point(0, 0);
			label4.Name = "label4";
			label4.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label4.Size = new System.Drawing.Size(24, 16);
			label4.TabIndex = 3;
			label4.Text = "PC";
			RegisterToolTip.SetToolTip(label4, "Program Counter\r\nAddress of currently executing code\r\nSNES:\r\nTop 8 bits is the Pr" +
        "ogram Bank register. If jumping to a 16-bit address, the program bank will remai" +
        "n the same.");
			// 
			// panelDB
			// 
			this.panelDB.AutoSize = true;
			this.panelDB.Controls.Add(this.RegisterDB);
			this.panelDB.Controls.Add(label18);
			this.panelDB.Location = new System.Drawing.Point(155, 0);
			this.panelDB.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelDB.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelDB.Name = "panelDB";
			this.panelDB.Size = new System.Drawing.Size(45, 20);
			this.panelDB.TabIndex = 4;
			this.panelDB.Visible = false;
			// 
			// RegisterDB
			// 
			this.RegisterDB.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterDB.DimColor = System.Drawing.Color.Gray;
			this.RegisterDB.DimUpperByte = false;
			this.RegisterDB.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterDB.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterDB.Location = new System.Drawing.Point(25, 0);
			this.RegisterDB.Name = "RegisterDB";
			this.RegisterDB.ReadOnly = false;
			this.RegisterDB.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterDB.Size = new System.Drawing.Size(20, 20);
			this.RegisterDB.TabIndex = 5;
			this.RegisterDB.Text = "00";
			this.RegisterDB.Value = 0;
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.Dock = System.Windows.Forms.DockStyle.Left;
			label18.Location = new System.Drawing.Point(0, 0);
			label18.Name = "label18";
			label18.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label18.Size = new System.Drawing.Size(25, 16);
			label18.TabIndex = 8;
			label18.Text = "DB";
			RegisterToolTip.SetToolTip(label18, "Data Bank register\r\nAll 16-bit reads and writes will target this bank");
			// 
			// panelSP
			// 
			this.panelSP.AutoSize = true;
			this.panelSP.Controls.Add(this.RegisterSP);
			this.panelSP.Controls.Add(label5);
			this.panelSP.Location = new System.Drawing.Point(200, 0);
			this.panelSP.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelSP.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelSP.Name = "panelSP";
			this.panelSP.Size = new System.Drawing.Size(44, 20);
			this.panelSP.TabIndex = 5;
			// 
			// RegisterSP
			// 
			this.RegisterSP.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterSP.DimColor = System.Drawing.Color.Gray;
			this.RegisterSP.DimUpperByte = false;
			this.RegisterSP.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterSP.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterSP.Location = new System.Drawing.Point(24, 0);
			this.RegisterSP.Name = "RegisterSP";
			this.RegisterSP.ReadOnly = false;
			this.RegisterSP.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterSP.Size = new System.Drawing.Size(20, 20);
			this.RegisterSP.TabIndex = 6;
			this.RegisterSP.Text = "00";
			this.RegisterSP.Value = 0;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Dock = System.Windows.Forms.DockStyle.Left;
			label5.Location = new System.Drawing.Point(0, 0);
			label5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			label5.Name = "label5";
			label5.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label5.Size = new System.Drawing.Size(24, 16);
			label5.TabIndex = 4;
			label5.Text = "SP";
			RegisterToolTip.SetToolTip(label5, "Stack pointer\r\nPoints to the address currently at the top of the stack\r\nNES:\r\nThe" +
        " register points to the lower 8 bits of the stack address ($01xx)");
			// 
			// panelDP
			// 
			this.panelDP.AutoSize = true;
			this.panelDP.Controls.Add(this.RegisterDP);
			this.panelDP.Controls.Add(label19);
			this.panelDP.Location = new System.Drawing.Point(244, 0);
			this.panelDP.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelDP.MinimumSize = new System.Drawing.Size(0, 20);
			this.panelDP.Name = "panelDP";
			this.panelDP.Size = new System.Drawing.Size(45, 20);
			this.panelDP.TabIndex = 6;
			this.panelDP.Visible = false;
			// 
			// RegisterDP
			// 
			this.RegisterDP.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterDP.DimColor = System.Drawing.Color.Gray;
			this.RegisterDP.DimUpperByte = false;
			this.RegisterDP.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterDP.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterDP.Location = new System.Drawing.Point(25, 0);
			this.RegisterDP.Name = "RegisterDP";
			this.RegisterDP.ReadOnly = false;
			this.RegisterDP.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterDP.Size = new System.Drawing.Size(20, 20);
			this.RegisterDP.TabIndex = 7;
			this.RegisterDP.Text = "00";
			this.RegisterDP.Value = 0;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.Dock = System.Windows.Forms.DockStyle.Left;
			label19.Location = new System.Drawing.Point(0, 0);
			label19.Name = "label19";
			label19.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label19.Size = new System.Drawing.Size(25, 16);
			label19.TabIndex = 8;
			label19.Text = "DP";
			RegisterToolTip.SetToolTip(label19, "Direct Page\r\nSelects which page of CPU addresses acts as a 6502 style \"zeropage\"");
			// 
			// panelP
			// 
			this.panelP.Controls.Add(this.RegisterP);
			this.panelP.Controls.Add(labelP);
			this.panelP.Location = new System.Drawing.Point(289, 0);
			this.panelP.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panelP.Name = "panelP";
			this.panelP.Size = new System.Drawing.Size(39, 20);
			this.panelP.TabIndex = 7;
			// 
			// RegisterP
			// 
			this.RegisterP.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.RegisterP.DimColor = System.Drawing.Color.Gray;
			this.RegisterP.DimUpperByte = false;
			this.RegisterP.Dock = System.Windows.Forms.DockStyle.Left;
			this.RegisterP.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.RegisterP.Location = new System.Drawing.Point(17, 0);
			this.RegisterP.Name = "RegisterP";
			this.RegisterP.ReadOnly = false;
			this.RegisterP.RegisterSize = Brewmaster.StatusView.RegisterSize.EightBit;
			this.RegisterP.Size = new System.Drawing.Size(20, 20);
			this.RegisterP.TabIndex = 8;
			this.RegisterP.Text = "00";
			this.RegisterP.Value = 0;
			// 
			// labelP
			// 
			labelP.AutoSize = true;
			labelP.Dock = System.Windows.Forms.DockStyle.Left;
			labelP.Location = new System.Drawing.Point(0, 0);
			labelP.Name = "labelP";
			labelP.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			labelP.Size = new System.Drawing.Size(17, 16);
			labelP.TabIndex = 5;
			labelP.Text = "P";
			RegisterToolTip.SetToolTip(labelP, "Processor status\r\nIndicates the state of the flags shown below");
			// 
			// FlagGroup
			// 
			FlagGroup.AutoSize = true;
			FlagGroup.Controls.Add(this.flagPanel);
			FlagGroup.Dock = System.Windows.Forms.DockStyle.Top;
			FlagGroup.Location = new System.Drawing.Point(2, 67);
			FlagGroup.Name = "FlagGroup";
			FlagGroup.Size = new System.Drawing.Size(620, 29);
			FlagGroup.TabIndex = 0;
			FlagGroup.Text = "Flags";
			// 
			// flagPanel
			// 
			this.flagPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flagPanel.AutoSize = true;
			this.flagPanel.Controls.Add(this.panel7);
			this.flagPanel.Controls.Add(this.panel8);
			this.flagPanel.Controls.Add(this.snesFlags);
			this.flagPanel.Location = new System.Drawing.Point(0, 6);
			this.flagPanel.Name = "flagPanel";
			this.flagPanel.Size = new System.Drawing.Size(620, 20);
			this.flagPanel.TabIndex = 1;
			// 
			// panel7
			// 
			this.panel7.AutoSize = true;
			this.panel7.Controls.Add(this.CheckDec);
			this.panel7.Controls.Add(this.CheckV);
			this.panel7.Controls.Add(this.CheckN);
			this.panel7.Location = new System.Drawing.Point(0, 0);
			this.panel7.Margin = new System.Windows.Forms.Padding(0);
			this.panel7.MinimumSize = new System.Drawing.Size(0, 20);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(204, 20);
			this.panel7.TabIndex = 0;
			// 
			// CheckDec
			// 
			this.CheckDec.AutoSize = true;
			this.CheckDec.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckDec.Location = new System.Drawing.Point(140, 0);
			this.CheckDec.Name = "CheckDec";
			this.CheckDec.Size = new System.Drawing.Size(64, 20);
			this.CheckDec.TabIndex = 2;
			this.CheckDec.Text = "Decimal";
			RegisterToolTip.SetToolTip(this.CheckDec, "If decimal mode is set, additions and subtractions will treat the value as Binary" +
        " Coded Decimal (doesn\'t work on NES)");
			this.CheckDec.UseVisualStyleBackColor = true;
			// 
			// CheckV
			// 
			this.CheckV.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckV.Location = new System.Drawing.Point(70, 0);
			this.CheckV.Name = "CheckV";
			this.CheckV.Size = new System.Drawing.Size(70, 20);
			this.CheckV.TabIndex = 1;
			this.CheckV.Text = "Overflow";
			RegisterToolTip.SetToolTip(this.CheckV, "A very complex flag that I can\'t explain in a single tool tip. Look it up, or jus" +
        "t ignore that it exists");
			this.CheckV.UseVisualStyleBackColor = true;
			// 
			// CheckN
			// 
			this.CheckN.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckN.Location = new System.Drawing.Point(0, 0);
			this.CheckN.Name = "CheckN";
			this.CheckN.Size = new System.Drawing.Size(70, 20);
			this.CheckN.TabIndex = 0;
			this.CheckN.Text = "Negative";
			RegisterToolTip.SetToolTip(this.CheckN, "Indicates a negative value (given two\'s complement). Typically gets set when the " +
        "result of the last operation sets bit 7 of a register");
			this.CheckN.UseVisualStyleBackColor = true;
			// 
			// panel8
			// 
			this.panel8.AutoSize = true;
			this.panel8.Controls.Add(this.CheckC);
			this.panel8.Controls.Add(this.CheckZ);
			this.panel8.Controls.Add(this.CheckIrq);
			this.panel8.Location = new System.Drawing.Point(204, 0);
			this.panel8.Margin = new System.Windows.Forms.Padding(0);
			this.panel8.MinimumSize = new System.Drawing.Size(0, 20);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(190, 20);
			this.panel8.TabIndex = 1;
			// 
			// CheckC
			// 
			this.CheckC.AutoSize = true;
			this.CheckC.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckC.Location = new System.Drawing.Point(140, 0);
			this.CheckC.Name = "CheckC";
			this.CheckC.Size = new System.Drawing.Size(50, 20);
			this.CheckC.TabIndex = 2;
			this.CheckC.Text = "Carry";
			RegisterToolTip.SetToolTip(this.CheckC, "Carry flag indicates a carry from math or shift operations, allowing you to store" +
        " bigger values across multiple bytes. Also used to detect smaller/greater number" +
        "s after a comparison");
			this.CheckC.UseVisualStyleBackColor = true;
			// 
			// CheckZ
			// 
			this.CheckZ.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckZ.Location = new System.Drawing.Point(70, 0);
			this.CheckZ.Name = "CheckZ";
			this.CheckZ.Size = new System.Drawing.Size(70, 20);
			this.CheckZ.TabIndex = 1;
			this.CheckZ.Text = "Zero";
			RegisterToolTip.SetToolTip(this.CheckZ, "Zero flag gets set when a comparison results in equal values, or a load, math or " +
        "logical operation results in 0. If the result is the opposite, the flag gets cle" +
        "ared");
			this.CheckZ.UseVisualStyleBackColor = true;
			// 
			// CheckIrq
			// 
			this.CheckIrq.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckIrq.Location = new System.Drawing.Point(0, 0);
			this.CheckIrq.Name = "CheckIrq";
			this.CheckIrq.Size = new System.Drawing.Size(70, 20);
			this.CheckIrq.TabIndex = 0;
			this.CheckIrq.Text = "Interrupt";
			RegisterToolTip.SetToolTip(this.CheckIrq, "Get set when an IRQ occurs, and cleared when returning from it. Non-NMI IRQs are " +
        "prevented while flag is set");
			this.CheckIrq.UseVisualStyleBackColor = true;
			// 
			// snesFlags
			// 
			this.snesFlags.AutoSize = true;
			this.snesFlags.Controls.Add(this.CheckEmu);
			this.snesFlags.Controls.Add(this.CheckX);
			this.snesFlags.Controls.Add(this.CheckM);
			this.snesFlags.Location = new System.Drawing.Point(394, 0);
			this.snesFlags.Margin = new System.Windows.Forms.Padding(0);
			this.snesFlags.MinimumSize = new System.Drawing.Size(0, 20);
			this.snesFlags.Name = "snesFlags";
			this.snesFlags.Size = new System.Drawing.Size(212, 20);
			this.snesFlags.TabIndex = 2;
			// 
			// CheckEmu
			// 
			this.CheckEmu.AutoSize = true;
			this.CheckEmu.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckEmu.Location = new System.Drawing.Point(140, 0);
			this.CheckEmu.Name = "CheckEmu";
			this.CheckEmu.Size = new System.Drawing.Size(72, 20);
			this.CheckEmu.TabIndex = 2;
			this.CheckEmu.Text = "Emulation";
			RegisterToolTip.SetToolTip(this.CheckEmu, "The Emulation setting makes most instructions behave like their 6502 equivalents");
			this.CheckEmu.UseVisualStyleBackColor = true;
			// 
			// CheckX
			// 
			this.CheckX.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckX.Location = new System.Drawing.Point(70, 0);
			this.CheckX.Name = "CheckX";
			this.CheckX.Size = new System.Drawing.Size(70, 20);
			this.CheckX.TabIndex = 1;
			this.CheckX.Text = "8-bit X/Y";
			RegisterToolTip.SetToolTip(this.CheckX, "When set, the X and Y registers work as 8-bit registers, otherwise all 16 bits ar" +
        "e used");
			this.CheckX.UseVisualStyleBackColor = true;
			// 
			// CheckM
			// 
			this.CheckM.Dock = System.Windows.Forms.DockStyle.Left;
			this.CheckM.Location = new System.Drawing.Point(0, 0);
			this.CheckM.Name = "CheckM";
			this.CheckM.Size = new System.Drawing.Size(70, 20);
			this.CheckM.TabIndex = 0;
			this.CheckM.Text = "8-bit A";
			RegisterToolTip.SetToolTip(this.CheckM, "When set, the A registers work as an 8-bit register, otherwise all 16 bits are us" +
        "ed");
			this.CheckM.UseVisualStyleBackColor = true;
			// 
			// TimingGroup
			// 
			TimingGroup.AutoSize = true;
			TimingGroup.Controls.Add(this.flowLayoutPanel4);
			TimingGroup.Dock = System.Windows.Forms.DockStyle.Top;
			TimingGroup.Location = new System.Drawing.Point(2, 117);
			TimingGroup.Name = "TimingGroup";
			TimingGroup.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			TimingGroup.Size = new System.Drawing.Size(620, 52);
			TimingGroup.TabIndex = 1;
			TimingGroup.Text = "Timing";
			// 
			// flowLayoutPanel4
			// 
			this.flowLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel4.AutoSize = true;
			this.flowLayoutPanel4.Controls.Add(this.panel9);
			this.flowLayoutPanel4.Controls.Add(this.panel10);
			this.flowLayoutPanel4.Controls.Add(this.panel11);
			this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 3);
			this.flowLayoutPanel4.Name = "flowLayoutPanel4";
			this.flowLayoutPanel4.Size = new System.Drawing.Size(620, 46);
			this.flowLayoutPanel4.TabIndex = 4;
			// 
			// panel9
			// 
			this.panel9.AutoSize = true;
			this.panel9.Controls.Add(this.EditPixel);
			this.panel9.Controls.Add(label9);
			this.panel9.Location = new System.Drawing.Point(0, 0);
			this.panel9.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panel9.MinimumSize = new System.Drawing.Size(0, 20);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(78, 20);
			this.panel9.TabIndex = 0;
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
			this.EditPixel.TabIndex = 1;
			this.EditPixel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label9
			// 
			label9.Dock = System.Windows.Forms.DockStyle.Left;
			label9.Location = new System.Drawing.Point(0, 0);
			label9.Name = "label9";
			label9.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label9.Size = new System.Drawing.Size(39, 20);
			label9.TabIndex = 0;
			label9.Text = "Pixel";
			// 
			// panel10
			// 
			this.panel10.AutoSize = true;
			this.panel10.Controls.Add(this.EditScanline);
			this.panel10.Controls.Add(label6);
			this.panel10.Location = new System.Drawing.Point(78, 0);
			this.panel10.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panel10.MinimumSize = new System.Drawing.Size(0, 20);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(99, 20);
			this.panel10.TabIndex = 1;
			// 
			// EditScanline
			// 
			this.EditScanline.BackColor = System.Drawing.SystemColors.Window;
			this.EditScanline.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.EditScanline.Dock = System.Windows.Forms.DockStyle.Left;
			this.EditScanline.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.EditScanline.Location = new System.Drawing.Point(60, 0);
			this.EditScanline.MaxLength = 2;
			this.EditScanline.Name = "EditScanline";
			this.EditScanline.ReadOnly = true;
			this.EditScanline.Size = new System.Drawing.Size(39, 20);
			this.EditScanline.TabIndex = 1;
			this.EditScanline.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label6
			// 
			label6.Dock = System.Windows.Forms.DockStyle.Left;
			label6.Location = new System.Drawing.Point(0, 0);
			label6.Name = "label6";
			label6.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label6.Size = new System.Drawing.Size(60, 20);
			label6.TabIndex = 0;
			label6.Text = "Scanline";
			// 
			// panel11
			// 
			this.panel11.AutoSize = true;
			this.panel11.Controls.Add(this.ResetFrame);
			this.panel11.Controls.Add(this.ResetCycle);
			this.panel11.Controls.Add(this.EditCycle);
			this.panel11.Controls.Add(label7);
			this.panel11.Controls.Add(this.EditFrame);
			this.panel11.Controls.Add(label8);
			this.panel11.Location = new System.Drawing.Point(177, 0);
			this.panel11.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.panel11.Name = "panel11";
			this.panel11.Size = new System.Drawing.Size(207, 43);
			this.panel11.TabIndex = 2;
			// 
			// ResetFrame
			// 
			this.ResetFrame.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ResetFrame.Location = new System.Drawing.Point(39, 22);
			this.ResetFrame.Margin = new System.Windows.Forms.Padding(0);
			this.ResetFrame.Name = "ResetFrame";
			this.ResetFrame.Size = new System.Drawing.Size(39, 21);
			this.ResetFrame.TabIndex = 4;
			this.ResetFrame.Text = "Reset";
			this.ResetFrame.UseVisualStyleBackColor = true;
			this.ResetFrame.Click += new System.EventHandler(this.ResetFrame_Click);
			// 
			// ResetCycle
			// 
			this.ResetCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ResetCycle.Location = new System.Drawing.Point(138, 22);
			this.ResetCycle.Margin = new System.Windows.Forms.Padding(0);
			this.ResetCycle.Name = "ResetCycle";
			this.ResetCycle.Size = new System.Drawing.Size(69, 21);
			this.ResetCycle.TabIndex = 5;
			this.ResetCycle.Text = "Reset";
			this.ResetCycle.UseVisualStyleBackColor = true;
			this.ResetCycle.Click += new System.EventHandler(this.ResetCycle_Click);
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
			this.EditCycle.TabIndex = 3;
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
			label7.TabIndex = 2;
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
			this.EditFrame.TabIndex = 1;
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
			label8.TabIndex = 0;
			label8.Text = "Frame";
			// 
			// RegisterToolTip
			// 
			RegisterToolTip.AutoPopDelay = 0;
			RegisterToolTip.InitialDelay = 500;
			RegisterToolTip.ReshowDelay = 100;
			// 
			// nesEnableNmi
			// 
			this.nesEnableNmi.AutoSize = true;
			this.nesEnableNmi.Location = new System.Drawing.Point(6, 39);
			this.nesEnableNmi.Name = "nesEnableNmi";
			this.nesEnableNmi.Size = new System.Drawing.Size(82, 17);
			this.nesEnableNmi.TabIndex = 3;
			this.nesEnableNmi.Text = "Enable NMI";
			RegisterToolTip.SetToolTip(this.nesEnableNmi, "When enabled, NMI will occur on vblank");
			this.nesEnableNmi.UseVisualStyleBackColor = true;
			// 
			// nesLargeSprites
			// 
			this.nesLargeSprites.AutoSize = true;
			this.nesLargeSprites.Location = new System.Drawing.Point(106, 39);
			this.nesLargeSprites.Name = "nesLargeSprites";
			this.nesLargeSprites.Size = new System.Drawing.Size(86, 17);
			this.nesLargeSprites.TabIndex = 4;
			this.nesLargeSprites.Text = "Large sprites";
			RegisterToolTip.SetToolTip(this.nesLargeSprites, "Uses 8x16 sprites instead of 8x8");
			this.nesLargeSprites.UseVisualStyleBackColor = true;
			// 
			// nesVramDown
			// 
			this.nesVramDown.AutoSize = true;
			this.nesVramDown.Location = new System.Drawing.Point(206, 39);
			this.nesVramDown.Name = "nesVramDown";
			this.nesVramDown.Size = new System.Drawing.Size(91, 17);
			this.nesVramDown.TabIndex = 5;
			this.nesVramDown.Text = "Vertical writes";
			RegisterToolTip.SetToolTip(this.nesVramDown, "After read or write to VRAM the address will increment by 32 instead of 1, as if " +
        "moving down a nametable column");
			this.nesVramDown.UseVisualStyleBackColor = true;
			// 
			// nesBg1000
			// 
			this.nesBg1000.AutoSize = true;
			this.nesBg1000.Location = new System.Drawing.Point(6, 54);
			this.nesBg1000.Name = "nesBg1000";
			this.nesBg1000.Size = new System.Drawing.Size(86, 17);
			this.nesBg1000.TabIndex = 6;
			this.nesBg1000.Text = "BG at $1000";
			RegisterToolTip.SetToolTip(this.nesBg1000, "Indicates which of the two 4kb blocks of CHR to read BG tiles from");
			this.nesBg1000.UseVisualStyleBackColor = true;
			// 
			// nesSprites1000
			// 
			this.nesSprites1000.AutoSize = true;
			this.nesSprites1000.Location = new System.Drawing.Point(106, 54);
			this.nesSprites1000.Name = "nesSprites1000";
			this.nesSprites1000.Size = new System.Drawing.Size(103, 17);
			this.nesSprites1000.TabIndex = 7;
			this.nesSprites1000.Text = "Sprites at $1000";
			RegisterToolTip.SetToolTip(this.nesSprites1000, "Indicates which of the two 4kb blocks of CHR to read sprite tiles from");
			this.nesSprites1000.UseVisualStyleBackColor = true;
			// 
			// nesShowSprites
			// 
			this.nesShowSprites.AutoSize = true;
			this.nesShowSprites.Location = new System.Drawing.Point(6, 104);
			this.nesShowSprites.Name = "nesShowSprites";
			this.nesShowSprites.Size = new System.Drawing.Size(86, 17);
			this.nesShowSprites.TabIndex = 3;
			this.nesShowSprites.Text = "Show sprites";
			RegisterToolTip.SetToolTip(this.nesShowSprites, "Enables drawing sprites (disables both BG and sprites to disable rendering entire" +
        "ly)");
			this.nesShowSprites.UseVisualStyleBackColor = true;
			// 
			// nesShowBg
			// 
			this.nesShowBg.AutoSize = true;
			this.nesShowBg.Location = new System.Drawing.Point(106, 104);
			this.nesShowBg.Name = "nesShowBg";
			this.nesShowBg.Size = new System.Drawing.Size(71, 17);
			this.nesShowBg.TabIndex = 4;
			this.nesShowBg.Text = "Show BG";
			RegisterToolTip.SetToolTip(this.nesShowBg, "Enables drawing background (disables both BG and sprites to disable rendering ent" +
        "irely)");
			this.nesShowBg.UseVisualStyleBackColor = true;
			// 
			// nesGreyscale
			// 
			this.nesGreyscale.AutoSize = true;
			this.nesGreyscale.Location = new System.Drawing.Point(206, 119);
			this.nesGreyscale.Name = "nesGreyscale";
			this.nesGreyscale.Size = new System.Drawing.Size(73, 17);
			this.nesGreyscale.TabIndex = 5;
			this.nesGreyscale.Text = "Greyscale";
			RegisterToolTip.SetToolTip(this.nesGreyscale, "Shows all colors as greyscale");
			this.nesGreyscale.UseVisualStyleBackColor = true;
			// 
			// nesShowLeftSprites
			// 
			this.nesShowLeftSprites.AutoSize = true;
			this.nesShowLeftSprites.Location = new System.Drawing.Point(6, 119);
			this.nesShowLeftSprites.Name = "nesShowLeftSprites";
			this.nesShowLeftSprites.Size = new System.Drawing.Size(103, 17);
			this.nesShowLeftSprites.TabIndex = 6;
			this.nesShowLeftSprites.Text = "Show left sprites";
			RegisterToolTip.SetToolTip(this.nesShowLeftSprites, "If unset, sprites are masked on the leftmost 8px of the screen");
			this.nesShowLeftSprites.UseVisualStyleBackColor = true;
			// 
			// nesShowLeftBg
			// 
			this.nesShowLeftBg.AutoSize = true;
			this.nesShowLeftBg.Location = new System.Drawing.Point(106, 119);
			this.nesShowLeftBg.Name = "nesShowLeftBg";
			this.nesShowLeftBg.Size = new System.Drawing.Size(88, 17);
			this.nesShowLeftBg.TabIndex = 7;
			this.nesShowLeftBg.Text = "Show left BG";
			RegisterToolTip.SetToolTip(this.nesShowLeftBg, "If unset, the background is masked on the leftmost 8px of the screen");
			this.nesShowLeftBg.UseVisualStyleBackColor = true;
			// 
			// nesEmphasizeB
			// 
			this.nesEmphasizeB.AutoSize = true;
			this.nesEmphasizeB.Location = new System.Drawing.Point(6, 89);
			this.nesEmphasizeB.Name = "nesEmphasizeB";
			this.nesEmphasizeB.Size = new System.Drawing.Size(87, 17);
			this.nesEmphasizeB.TabIndex = 8;
			this.nesEmphasizeB.Text = "Emphasize B";
			RegisterToolTip.SetToolTip(this.nesEmphasizeB, "Emphasizes blue colors");
			this.nesEmphasizeB.UseVisualStyleBackColor = true;
			// 
			// nesEmphasizeG
			// 
			this.nesEmphasizeG.AutoSize = true;
			this.nesEmphasizeG.Location = new System.Drawing.Point(106, 89);
			this.nesEmphasizeG.Name = "nesEmphasizeG";
			this.nesEmphasizeG.Size = new System.Drawing.Size(88, 17);
			this.nesEmphasizeG.TabIndex = 9;
			this.nesEmphasizeG.Text = "Emphasize G";
			RegisterToolTip.SetToolTip(this.nesEmphasizeG, "Emphasizes green colors");
			this.nesEmphasizeG.UseVisualStyleBackColor = true;
			// 
			// nesEmphasizeR
			// 
			this.nesEmphasizeR.AutoSize = true;
			this.nesEmphasizeR.Location = new System.Drawing.Point(206, 89);
			this.nesEmphasizeR.Name = "nesEmphasizeR";
			this.nesEmphasizeR.Size = new System.Drawing.Size(88, 17);
			this.nesEmphasizeR.TabIndex = 10;
			this.nesEmphasizeR.Text = "Emphasize R";
			RegisterToolTip.SetToolTip(this.nesEmphasizeR, "Emphasizes red colors");
			this.nesEmphasizeR.UseVisualStyleBackColor = true;
			// 
			// nesVblankFlag
			// 
			this.nesVblankFlag.AutoSize = true;
			this.nesVblankFlag.Location = new System.Drawing.Point(6, 154);
			this.nesVblankFlag.Name = "nesVblankFlag";
			this.nesVblankFlag.Size = new System.Drawing.Size(59, 17);
			this.nesVblankFlag.TabIndex = 3;
			this.nesVblankFlag.Text = "Vblank";
			RegisterToolTip.SetToolTip(this.nesVblankFlag, "Gets set at the start of vertical blank");
			this.nesVblankFlag.UseVisualStyleBackColor = true;
			// 
			// nesSprite0
			// 
			this.nesSprite0.AutoSize = true;
			this.nesSprite0.Location = new System.Drawing.Point(106, 154);
			this.nesSprite0.Name = "nesSprite0";
			this.nesSprite0.Size = new System.Drawing.Size(76, 17);
			this.nesSprite0.TabIndex = 4;
			this.nesSprite0.Text = "Sprite 0 hit";
			RegisterToolTip.SetToolTip(this.nesSprite0, "Gets set when a sprite 0 hit is detected. Unset at the start of each frame");
			this.nesSprite0.UseVisualStyleBackColor = true;
			// 
			// nesSpriteOverflow
			// 
			this.nesSpriteOverflow.AutoSize = true;
			this.nesSpriteOverflow.Location = new System.Drawing.Point(206, 154);
			this.nesSpriteOverflow.Name = "nesSpriteOverflow";
			this.nesSpriteOverflow.Size = new System.Drawing.Size(96, 17);
			this.nesSpriteOverflow.TabIndex = 5;
			this.nesSpriteOverflow.Text = "Sprite overflow";
			RegisterToolTip.SetToolTip(this.nesSpriteOverflow, "Gets set when a sprite overflow is detected on a scanline (buggy)");
			this.nesSpriteOverflow.UseVisualStyleBackColor = true;
			// 
			// PpuGroup
			// 
			PpuGroup.AutoSize = true;
			PpuGroup.Controls.Add(this.nesPpuPanel);
			PpuGroup.Dock = System.Windows.Forms.DockStyle.Top;
			PpuGroup.Location = new System.Drawing.Point(2, 238);
			PpuGroup.Name = "PpuGroup";
			PpuGroup.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			PpuGroup.Size = new System.Drawing.Size(376, 181);
			PpuGroup.TabIndex = 11;
			PpuGroup.Text = "Flags";
			// 
			// nesPpuPanel
			// 
			this.nesPpuPanel.Controls.Add(panel4);
			this.nesPpuPanel.Controls.Add(this.nesSpriteOverflow);
			this.nesPpuPanel.Controls.Add(this.nesSprite0);
			this.nesPpuPanel.Controls.Add(this.nesVblankFlag);
			this.nesPpuPanel.Controls.Add(this.nesGreyscale);
			this.nesPpuPanel.Controls.Add(this.nesShowLeftBg);
			this.nesPpuPanel.Controls.Add(this.nesShowLeftSprites);
			this.nesPpuPanel.Controls.Add(this.nesEmphasizeR);
			this.nesPpuPanel.Controls.Add(this.nesSprites1000);
			this.nesPpuPanel.Controls.Add(this.nesShowBg);
			this.nesPpuPanel.Controls.Add(this.nesEmphasizeG);
			this.nesPpuPanel.Controls.Add(this.nesShowSprites);
			this.nesPpuPanel.Controls.Add(this.nesBg1000);
			this.nesPpuPanel.Controls.Add(this.nesEmphasizeB);
			this.nesPpuPanel.Controls.Add(this.nesVramDown);
			this.nesPpuPanel.Controls.Add(this.nesLargeSprites);
			this.nesPpuPanel.Controls.Add(this.nesEnableNmi);
			this.nesPpuPanel.Controls.Add(this.label12);
			this.nesPpuPanel.Controls.Add(this.label11);
			this.nesPpuPanel.Controls.Add(this.label10);
			this.nesPpuPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.nesPpuPanel.Location = new System.Drawing.Point(0, 5);
			this.nesPpuPanel.Name = "nesPpuPanel";
			this.nesPpuPanel.Size = new System.Drawing.Size(376, 176);
			this.nesPpuPanel.TabIndex = 2;
			// 
			// panel4
			// 
			panel4.Controls.Add(this.nesPpuTRegister);
			panel4.Controls.Add(label15);
			panel4.Controls.Add(this.nesNtAddr);
			panel4.Controls.Add(label13);
			panel4.Controls.Add(this.nesVramAddr);
			panel4.Controls.Add(label14);
			panel4.Dock = System.Windows.Forms.DockStyle.Top;
			panel4.Location = new System.Drawing.Point(0, 0);
			panel4.Name = "panel4";
			panel4.Size = new System.Drawing.Size(376, 20);
			panel4.TabIndex = 11;
			// 
			// nesPpuTRegister
			// 
			this.nesPpuTRegister.BackColor = System.Drawing.SystemColors.Window;
			this.nesPpuTRegister.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.nesPpuTRegister.Dock = System.Windows.Forms.DockStyle.Left;
			this.nesPpuTRegister.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.nesPpuTRegister.Location = new System.Drawing.Point(216, 0);
			this.nesPpuTRegister.MaxLength = 2;
			this.nesPpuTRegister.Name = "nesPpuTRegister";
			this.nesPpuTRegister.ReadOnly = true;
			this.nesPpuTRegister.Size = new System.Drawing.Size(39, 20);
			this.nesPpuTRegister.TabIndex = 21;
			this.nesPpuTRegister.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label15
			// 
			label15.Dock = System.Windows.Forms.DockStyle.Left;
			label15.Location = new System.Drawing.Point(196, 0);
			label15.Name = "label15";
			label15.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label15.Size = new System.Drawing.Size(20, 20);
			label15.TabIndex = 20;
			label15.Text = "T";
			// 
			// nesNtAddr
			// 
			this.nesNtAddr.BackColor = System.Drawing.SystemColors.Window;
			this.nesNtAddr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.nesNtAddr.Dock = System.Windows.Forms.DockStyle.Left;
			this.nesNtAddr.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.nesNtAddr.Location = new System.Drawing.Point(157, 0);
			this.nesNtAddr.MaxLength = 2;
			this.nesNtAddr.Name = "nesNtAddr";
			this.nesNtAddr.ReadOnly = true;
			this.nesNtAddr.Size = new System.Drawing.Size(39, 20);
			this.nesNtAddr.TabIndex = 17;
			this.nesNtAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label13
			// 
			label13.Dock = System.Windows.Forms.DockStyle.Left;
			label13.Location = new System.Drawing.Point(107, 0);
			label13.Name = "label13";
			label13.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label13.Size = new System.Drawing.Size(50, 20);
			label13.TabIndex = 14;
			label13.Text = "NT addr";
			// 
			// nesVramAddr
			// 
			this.nesVramAddr.BackColor = System.Drawing.SystemColors.Window;
			this.nesVramAddr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.nesVramAddr.Dock = System.Windows.Forms.DockStyle.Left;
			this.nesVramAddr.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.nesVramAddr.Location = new System.Drawing.Point(68, 0);
			this.nesVramAddr.MaxLength = 2;
			this.nesVramAddr.Name = "nesVramAddr";
			this.nesVramAddr.ReadOnly = true;
			this.nesVramAddr.Size = new System.Drawing.Size(39, 20);
			this.nesVramAddr.TabIndex = 19;
			this.nesVramAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label14
			// 
			label14.Dock = System.Windows.Forms.DockStyle.Left;
			label14.Location = new System.Drawing.Point(0, 0);
			label14.Name = "label14";
			label14.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			label14.Size = new System.Drawing.Size(68, 20);
			label14.TabIndex = 18;
			label14.Text = "VRAM addr";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(3, 138);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(76, 13);
			this.label12.TabIndex = 5;
			this.label12.Text = "Status ($2002)";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(3, 73);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(72, 13);
			this.label11.TabIndex = 3;
			this.label11.Text = "Mask ($2001)";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(3, 23);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(90, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Controller ($2000)";
			// 
			// stackGroup
			// 
			stackGroup.AutoSize = true;
			stackGroup.Controls.Add(this.panel6);
			stackGroup.Controls.Add(this.panel5);
			stackGroup.Dock = System.Windows.Forms.DockStyle.Top;
			stackGroup.Location = new System.Drawing.Point(2, 237);
			stackGroup.MinimumSize = new System.Drawing.Size(0, 20);
			stackGroup.Name = "stackGroup";
			stackGroup.Size = new System.Drawing.Size(376, 71);
			stackGroup.TabIndex = 12;
			// 
			// panel6
			// 
			this.panel6.AutoSize = true;
			this.panel6.Controls.Add(this.completeStack);
			this.panel6.Controls.Add(this.label17);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel6.Location = new System.Drawing.Point(0, 22);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(376, 49);
			this.panel6.TabIndex = 23;
			// 
			// completeStack
			// 
			this.completeStack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.completeStack.BackColor = System.Drawing.SystemColors.Window;
			this.completeStack.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.completeStack.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.completeStack.Location = new System.Drawing.Point(95, 0);
			this.completeStack.MaxLength = 2;
			this.completeStack.Multiline = true;
			this.completeStack.Name = "completeStack";
			this.completeStack.ReadOnly = true;
			this.completeStack.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.completeStack.Size = new System.Drawing.Size(278, 46);
			this.completeStack.TabIndex = 20;
			// 
			// label17
			// 
			this.label17.Dock = System.Windows.Forms.DockStyle.Left;
			this.label17.Location = new System.Drawing.Point(0, 0);
			this.label17.Name = "label17";
			this.label17.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this.label17.Size = new System.Drawing.Size(95, 49);
			this.label17.TabIndex = 0;
			this.label17.Text = "Complete stack";
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.flowLayoutPanel1);
			this.panel5.Controls.Add(this.topOfStack);
			this.panel5.Controls.Add(this.label16);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(0, 0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(376, 22);
			this.panel5.TabIndex = 22;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.stack8bit);
			this.flowLayoutPanel1.Controls.Add(this.stack16bit);
			this.flowLayoutPanel1.Controls.Add(this.stack24bit);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(117, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 22);
			this.flowLayoutPanel1.TabIndex = 24;
			// 
			// stack8bit
			// 
			this.stack8bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.stack8bit.Checked = true;
			this.stack8bit.Location = new System.Drawing.Point(2, 0);
			this.stack8bit.Margin = new System.Windows.Forms.Padding(2, 0, 0, 5);
			this.stack8bit.Name = "stack8bit";
			this.stack8bit.Size = new System.Drawing.Size(30, 20);
			this.stack8bit.TabIndex = 21;
			this.stack8bit.TabStop = true;
			this.stack8bit.Text = "8";
			this.stack8bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.stack8bit.UseVisualStyleBackColor = true;
			// 
			// stack16bit
			// 
			this.stack16bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.stack16bit.Location = new System.Drawing.Point(34, 0);
			this.stack16bit.Margin = new System.Windows.Forms.Padding(2, 0, 0, 3);
			this.stack16bit.Name = "stack16bit";
			this.stack16bit.Size = new System.Drawing.Size(30, 20);
			this.stack16bit.TabIndex = 22;
			this.stack16bit.Text = "16";
			this.stack16bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.stack16bit.UseVisualStyleBackColor = true;
			// 
			// stack24bit
			// 
			this.stack24bit.Appearance = System.Windows.Forms.Appearance.Button;
			this.stack24bit.Location = new System.Drawing.Point(66, 0);
			this.stack24bit.Margin = new System.Windows.Forms.Padding(2, 0, 0, 3);
			this.stack24bit.Name = "stack24bit";
			this.stack24bit.Size = new System.Drawing.Size(30, 20);
			this.stack24bit.TabIndex = 23;
			this.stack24bit.Text = "24";
			this.stack24bit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.stack24bit.UseVisualStyleBackColor = true;
			// 
			// topOfStack
			// 
			this.topOfStack.BackColor = System.Drawing.SystemColors.Window;
			this.topOfStack.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.topOfStack.Dock = System.Windows.Forms.DockStyle.Left;
			this.topOfStack.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.topOfStack.Location = new System.Drawing.Point(95, 0);
			this.topOfStack.MaxLength = 2;
			this.topOfStack.Name = "topOfStack";
			this.topOfStack.ReadOnly = true;
			this.topOfStack.Size = new System.Drawing.Size(22, 20);
			this.topOfStack.TabIndex = 20;
			this.topOfStack.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label16
			// 
			this.label16.Dock = System.Windows.Forms.DockStyle.Left;
			this.label16.Location = new System.Drawing.Point(0, 0);
			this.label16.Name = "label16";
			this.label16.Padding = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this.label16.Size = new System.Drawing.Size(95, 22);
			this.label16.TabIndex = 0;
			this.label16.Text = "Top of stack";
			// 
			// horizontalLine5
			// 
			horizontalLine5.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine5.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine5.Location = new System.Drawing.Point(2, 211);
			horizontalLine5.Name = "horizontalLine5";
			horizontalLine5.Size = new System.Drawing.Size(620, 1);
			horizontalLine5.TabIndex = 14;
			horizontalLine5.Text = "horizontalLine5";
			// 
			// horizontalLine4
			// 
			horizontalLine4.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine4.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine4.Location = new System.Drawing.Point(2, 190);
			horizontalLine4.Name = "horizontalLine4";
			horizontalLine4.Size = new System.Drawing.Size(620, 1);
			horizontalLine4.TabIndex = 10;
			horizontalLine4.Text = "horizontalLine4";
			// 
			// horizontalLine3
			// 
			horizontalLine3.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine3.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine3.Location = new System.Drawing.Point(2, 169);
			horizontalLine3.Name = "horizontalLine3";
			horizontalLine3.Size = new System.Drawing.Size(620, 1);
			horizontalLine3.TabIndex = 9;
			horizontalLine3.Text = "horizontalLine3";
			// 
			// horizontalLine2
			// 
			horizontalLine2.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine2.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine2.Location = new System.Drawing.Point(2, 96);
			horizontalLine2.Name = "horizontalLine2";
			horizontalLine2.Size = new System.Drawing.Size(620, 1);
			horizontalLine2.TabIndex = 8;
			horizontalLine2.Text = "horizontalLine2";
			// 
			// horizontalLine1
			// 
			horizontalLine1.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine1.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine1.Location = new System.Drawing.Point(2, 46);
			horizontalLine1.Name = "horizontalLine1";
			horizontalLine1.Size = new System.Drawing.Size(620, 1);
			horizontalLine1.TabIndex = 7;
			horizontalLine1.Text = "horizontalLine1";
			// 
			// expandPpu
			// 
			this.expandPpu.ButtonText = "PPU";
			this.expandPpu.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandPpu.ExpandedState = false;
			this.expandPpu.ExpandPanel = PpuGroup;
			this.expandPpu.FlatAppearance.BorderSize = 0;
			this.expandPpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.expandPpu.Location = new System.Drawing.Point(2, 191);
			this.expandPpu.Name = "expandPpu";
			this.expandPpu.Size = new System.Drawing.Size(620, 20);
			this.expandPpu.TabIndex = 6;
			this.expandPpu.UseVisualStyleBackColor = true;
			// 
			// expandStack
			// 
			this.expandStack.ButtonText = "Stack";
			this.expandStack.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandStack.ExpandedState = false;
			this.expandStack.ExpandPanel = stackGroup;
			this.expandStack.FlatAppearance.BorderSize = 0;
			this.expandStack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.expandStack.Location = new System.Drawing.Point(2, 170);
			this.expandStack.Name = "expandStack";
			this.expandStack.Size = new System.Drawing.Size(620, 20);
			this.expandStack.TabIndex = 3;
			this.expandStack.UseVisualStyleBackColor = true;
			// 
			// expandTiming
			// 
			this.expandTiming.ButtonText = "Timing";
			this.expandTiming.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandTiming.ExpandedState = true;
			this.expandTiming.ExpandPanel = TimingGroup;
			this.expandTiming.FlatAppearance.BorderSize = 0;
			this.expandTiming.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.expandTiming.Location = new System.Drawing.Point(2, 97);
			this.expandTiming.Name = "expandTiming";
			this.expandTiming.Size = new System.Drawing.Size(620, 20);
			this.expandTiming.TabIndex = 2;
			this.expandTiming.UseVisualStyleBackColor = true;
			// 
			// expandFlags
			// 
			this.expandFlags.ButtonText = "Flags";
			this.expandFlags.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandFlags.ExpandedState = true;
			this.expandFlags.ExpandPanel = FlagGroup;
			this.expandFlags.FlatAppearance.BorderSize = 0;
			this.expandFlags.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.expandFlags.Location = new System.Drawing.Point(2, 47);
			this.expandFlags.Name = "expandFlags";
			this.expandFlags.Size = new System.Drawing.Size(620, 20);
			this.expandFlags.TabIndex = 1;
			this.expandFlags.UseVisualStyleBackColor = true;
			// 
			// expandCpu
			// 
			this.expandCpu.ButtonText = "Cpu registers";
			this.expandCpu.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandCpu.ExpandedState = true;
			this.expandCpu.ExpandPanel = RegisterGroup;
			this.expandCpu.FlatAppearance.BorderSize = 0;
			this.expandCpu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.expandCpu.Location = new System.Drawing.Point(2, 0);
			this.expandCpu.Name = "expandCpu";
			this.expandCpu.Size = new System.Drawing.Size(620, 20);
			this.expandCpu.TabIndex = 0;
			this.expandCpu.UseVisualStyleBackColor = true;
			// 
			// CpuStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.Controls.Add(horizontalLine5);
			this.Controls.Add(PpuGroup);
			this.Controls.Add(this.expandPpu);
			this.Controls.Add(horizontalLine4);
			this.Controls.Add(stackGroup);
			this.Controls.Add(this.expandStack);
			this.Controls.Add(horizontalLine3);
			this.Controls.Add(TimingGroup);
			this.Controls.Add(this.expandTiming);
			this.Controls.Add(horizontalLine2);
			this.Controls.Add(FlagGroup);
			this.Controls.Add(this.expandFlags);
			this.Controls.Add(horizontalLine1);
			this.Controls.Add(RegisterGroup);
			this.Controls.Add(this.expandCpu);
			this.MinimumSize = new System.Drawing.Size(100, 0);
			this.Name = "CpuStatus";
			this.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.Size = new System.Drawing.Size(622, 622);
			RegisterGroup.ResumeLayout(false);
			RegisterGroup.PerformLayout();
			this.registerPanel.ResumeLayout(false);
			this.registerPanel.PerformLayout();
			this.panelA.ResumeLayout(false);
			this.panelA.PerformLayout();
			this.panelX.ResumeLayout(false);
			this.panelX.PerformLayout();
			this.panelY.ResumeLayout(false);
			this.panelY.PerformLayout();
			this.panelPC.ResumeLayout(false);
			this.panelPC.PerformLayout();
			this.panelDB.ResumeLayout(false);
			this.panelDB.PerformLayout();
			this.panelSP.ResumeLayout(false);
			this.panelSP.PerformLayout();
			this.panelDP.ResumeLayout(false);
			this.panelDP.PerformLayout();
			this.panelP.ResumeLayout(false);
			this.panelP.PerformLayout();
			FlagGroup.ResumeLayout(false);
			FlagGroup.PerformLayout();
			this.flagPanel.ResumeLayout(false);
			this.flagPanel.PerformLayout();
			this.panel7.ResumeLayout(false);
			this.panel7.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.snesFlags.ResumeLayout(false);
			this.snesFlags.PerformLayout();
			TimingGroup.ResumeLayout(false);
			TimingGroup.PerformLayout();
			this.flowLayoutPanel4.ResumeLayout(false);
			this.flowLayoutPanel4.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.panel9.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel10.PerformLayout();
			this.panel11.ResumeLayout(false);
			this.panel11.PerformLayout();
			PpuGroup.ResumeLayout(false);
			this.nesPpuPanel.ResumeLayout(false);
			this.nesPpuPanel.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			stackGroup.ResumeLayout(false);
			stackGroup.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel6.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox CheckN;
		private System.Windows.Forms.CheckBox CheckV;
		private System.Windows.Forms.CheckBox CheckDec;
		private System.Windows.Forms.CheckBox CheckIrq;
		private System.Windows.Forms.CheckBox CheckZ;
		private System.Windows.Forms.CheckBox CheckC;
		private System.Windows.Forms.TextBox EditScanline;
		private System.Windows.Forms.TextBox EditCycle;
		private System.Windows.Forms.TextBox EditFrame;
		private System.Windows.Forms.Button ResetCycle;
		private System.Windows.Forms.Button ResetFrame;
		private System.Windows.Forms.TextBox EditPixel;
		private ExpandButton expandTiming;
		private ExpandButton expandCpu;
		private ExpandButton expandFlags;
		private ExpandButton expandPpu;
		private System.Windows.Forms.CheckBox nesEnableNmi;
		private System.Windows.Forms.CheckBox nesLargeSprites;
		private System.Windows.Forms.CheckBox nesVramDown;
		private System.Windows.Forms.CheckBox nesBg1000;
		private System.Windows.Forms.CheckBox nesSprites1000;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Panel nesPpuPanel;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox nesShowSprites;
		private System.Windows.Forms.CheckBox nesShowBg;
		private System.Windows.Forms.CheckBox nesGreyscale;
		private System.Windows.Forms.CheckBox nesShowLeftSprites;
		private System.Windows.Forms.CheckBox nesShowLeftBg;
		private System.Windows.Forms.CheckBox nesEmphasizeR;
		private System.Windows.Forms.CheckBox nesEmphasizeG;
		private System.Windows.Forms.CheckBox nesEmphasizeB;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox nesVblankFlag;
		private System.Windows.Forms.CheckBox nesSprite0;
		private System.Windows.Forms.CheckBox nesSpriteOverflow;
		private System.Windows.Forms.TextBox nesNtAddr;
		private System.Windows.Forms.TextBox nesVramAddr;
		private System.Windows.Forms.TextBox nesPpuTRegister;
		private ExpandButton expandStack;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox topOfStack;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.TextBox completeStack;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.RadioButton stack8bit;
		private System.Windows.Forms.RadioButton stack24bit;
		private System.Windows.Forms.RadioButton stack16bit;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel registerPanel;
		private System.Windows.Forms.Panel panelA;
		private System.Windows.Forms.Panel panelX;
		private System.Windows.Forms.Panel panelY;
		private System.Windows.Forms.Panel panelPC;
		private System.Windows.Forms.Panel panelSP;
		private System.Windows.Forms.Panel panelP;
		private System.Windows.Forms.Panel panelDB;
		private System.Windows.Forms.Panel panelDP;
		private System.Windows.Forms.FlowLayoutPanel flagPanel;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
		private System.Windows.Forms.Panel panel9;
		private System.Windows.Forms.Panel panel10;
		private System.Windows.Forms.Panel panel11;
		private System.Windows.Forms.Panel snesFlags;
		private System.Windows.Forms.CheckBox CheckEmu;
		private System.Windows.Forms.CheckBox CheckX;
		private System.Windows.Forms.CheckBox CheckM;
		private RegisterValue RegisterA;
		private RegisterValue RegisterX;
		private RegisterValue RegisterY;
		private RegisterValue RegisterPC;
		private RegisterValue RegisterDB;
		private RegisterValue RegisterSP;
		private RegisterValue RegisterDP;
		private RegisterValue RegisterP;
	}
}
