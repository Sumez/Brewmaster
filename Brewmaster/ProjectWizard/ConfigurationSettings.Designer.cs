using System.Windows.Forms;

namespace Brewmaster.ProjectWizard
{
	partial class ConfigurationSettings
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
			System.Windows.Forms.Label configurationLabel;
			System.Windows.Forms.Label outputLabel;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label nameLabel;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationSettings));
			this.ChecksumLabel = new System.Windows.Forms.Label();
			this.OutputFile = new System.Windows.Forms.TextBox();
			this.ConfigurationFile = new System.Windows.Forms.TextBox();
			this.BrowseConfigFile = new System.Windows.Forms.Button();
			this.GenerateMapFile = new System.Windows.Forms.CheckBox();
			this.Symbols = new System.Windows.Forms.TextBox();
			this.ConfigurationName = new System.Windows.Forms.TextBox();
			this.CalculateSnesChecksum = new System.Windows.Forms.CheckBox();
			this._integratedPanel = new System.Windows.Forms.Panel();
			this.UseIntegrated = new System.Windows.Forms.RadioButton();
			this.UseCustom = new System.Windows.Forms.RadioButton();
			this._customPanel = new System.Windows.Forms.Panel();
			this.CustomDebugFile = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.CustomOutputFile = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.CustomScript = new System.Windows.Forms.TextBox();
			configurationLabel = new System.Windows.Forms.Label();
			outputLabel = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			nameLabel = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			this._integratedPanel.SuspendLayout();
			this._customPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// configurationLabel
			// 
			configurationLabel.AutoSize = true;
			configurationLabel.Location = new System.Drawing.Point(-3, 29);
			configurationLabel.Name = "configurationLabel";
			configurationLabel.Size = new System.Drawing.Size(88, 13);
			configurationLabel.TabIndex = 8;
			configurationLabel.Text = "Configuration file:";
			// 
			// outputLabel
			// 
			outputLabel.AutoSize = true;
			outputLabel.Location = new System.Drawing.Point(-3, 3);
			outputLabel.Name = "outputLabel";
			outputLabel.Size = new System.Drawing.Size(58, 13);
			outputLabel.TabIndex = 7;
			outputLabel.Text = "Output file:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(-3, 55);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(94, 13);
			label1.TabIndex = 11;
			label1.Text = "Generate Map file:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(-3, 80);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(81, 13);
			label2.TabIndex = 14;
			label2.Text = "Define symbols:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(111, 204);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(148, 13);
			label3.TabIndex = 15;
			label3.Text = "One symbol per line. Example:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.Location = new System.Drawing.Point(260, 204);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(59, 36);
			label4.TabIndex = 16;
			label4.Text = "MAPPER=3\r\nDEBUG\r\nCHRROM=0";
			// 
			// nameLabel
			// 
			nameLabel.AutoSize = true;
			nameLabel.Location = new System.Drawing.Point(-3, 3);
			nameLabel.Name = "nameLabel";
			nameLabel.Size = new System.Drawing.Size(101, 13);
			nameLabel.TabIndex = 18;
			nameLabel.Text = "Configuration name:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(-3, 3);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(115, 13);
			label5.TabIndex = 16;
			label5.Text = "Commands to execute:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label6.Location = new System.Drawing.Point(163, 78);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(207, 36);
			label6.TabIndex = 18;
			label6.Text = "py -3 convertchr.py chr/source.png\r\nmake -f makefile.mk\r\ncopy bin/output.prg chr/" +
    "source.chr bin/output.nes";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(111, 78);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(50, 13);
			label7.TabIndex = 17;
			label7.Text = "Example:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(0, 123);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(86, 13);
			label8.TabIndex = 21;
			label8.Text = "Output ROM file:";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(0, 149);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(116, 13);
			label9.TabIndex = 24;
			label9.Text = "Ld65 debug output file:";
			// 
			// label10
			// 
			label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			label10.Location = new System.Drawing.Point(114, 171);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(381, 52);
			label10.TabIndex = 25;
			label10.Text = resources.GetString("label10.Text");
			// 
			// ChecksumLabel
			// 
			this.ChecksumLabel.AutoSize = true;
			this.ChecksumLabel.Location = new System.Drawing.Point(-3, 246);
			this.ChecksumLabel.Name = "ChecksumLabel";
			this.ChecksumLabel.Size = new System.Drawing.Size(91, 13);
			this.ChecksumLabel.TabIndex = 20;
			this.ChecksumLabel.Text = "SNES checksum:";
			// 
			// OutputFile
			// 
			this.OutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputFile.Location = new System.Drawing.Point(114, 0);
			this.OutputFile.Name = "OutputFile";
			this.OutputFile.Size = new System.Drawing.Size(378, 20);
			this.OutputFile.TabIndex = 1;
			// 
			// ConfigurationFile
			// 
			this.ConfigurationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfigurationFile.Location = new System.Drawing.Point(114, 26);
			this.ConfigurationFile.Name = "ConfigurationFile";
			this.ConfigurationFile.Size = new System.Drawing.Size(378, 20);
			this.ConfigurationFile.TabIndex = 3;
			// 
			// BrowseConfigFile
			// 
			this.BrowseConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowseConfigFile.Location = new System.Drawing.Point(498, 25);
			this.BrowseConfigFile.Name = "BrowseConfigFile";
			this.BrowseConfigFile.Size = new System.Drawing.Size(75, 23);
			this.BrowseConfigFile.TabIndex = 4;
			this.BrowseConfigFile.Text = "Browse...";
			this.BrowseConfigFile.UseVisualStyleBackColor = true;
			this.BrowseConfigFile.Click += new System.EventHandler(this.BrowseConfigFile_Click);
			// 
			// GenerateMapFile
			// 
			this.GenerateMapFile.AutoSize = true;
			this.GenerateMapFile.Checked = true;
			this.GenerateMapFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.GenerateMapFile.Location = new System.Drawing.Point(114, 54);
			this.GenerateMapFile.Name = "GenerateMapFile";
			this.GenerateMapFile.Size = new System.Drawing.Size(110, 17);
			this.GenerateMapFile.TabIndex = 5;
			this.GenerateMapFile.Text = "Generate Map file";
			this.GenerateMapFile.UseVisualStyleBackColor = true;
			// 
			// Symbols
			// 
			this.Symbols.AcceptsReturn = true;
			this.Symbols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Symbols.Location = new System.Drawing.Point(114, 77);
			this.Symbols.Multiline = true;
			this.Symbols.Name = "Symbols";
			this.Symbols.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Symbols.Size = new System.Drawing.Size(378, 124);
			this.Symbols.TabIndex = 6;
			// 
			// ConfigurationName
			// 
			this.ConfigurationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ConfigurationName.Location = new System.Drawing.Point(114, 0);
			this.ConfigurationName.Name = "ConfigurationName";
			this.ConfigurationName.Size = new System.Drawing.Size(378, 20);
			this.ConfigurationName.TabIndex = 0;
			// 
			// CalculateSnesChecksum
			// 
			this.CalculateSnesChecksum.AutoSize = true;
			this.CalculateSnesChecksum.Checked = true;
			this.CalculateSnesChecksum.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CalculateSnesChecksum.Location = new System.Drawing.Point(114, 245);
			this.CalculateSnesChecksum.Name = "CalculateSnesChecksum";
			this.CalculateSnesChecksum.Size = new System.Drawing.Size(167, 17);
			this.CalculateSnesChecksum.TabIndex = 19;
			this.CalculateSnesChecksum.Text = "Calculate and include in ROM";
			this.CalculateSnesChecksum.UseVisualStyleBackColor = true;
			// 
			// _integratedPanel
			// 
			this._integratedPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._integratedPanel.Controls.Add(this.OutputFile);
			this._integratedPanel.Controls.Add(outputLabel);
			this._integratedPanel.Controls.Add(this.CalculateSnesChecksum);
			this._integratedPanel.Controls.Add(configurationLabel);
			this._integratedPanel.Controls.Add(this.ChecksumLabel);
			this._integratedPanel.Controls.Add(this.ConfigurationFile);
			this._integratedPanel.Controls.Add(this.BrowseConfigFile);
			this._integratedPanel.Controls.Add(label1);
			this._integratedPanel.Controls.Add(label4);
			this._integratedPanel.Controls.Add(this.GenerateMapFile);
			this._integratedPanel.Controls.Add(label3);
			this._integratedPanel.Controls.Add(this.Symbols);
			this._integratedPanel.Controls.Add(label2);
			this._integratedPanel.Location = new System.Drawing.Point(0, 54);
			this._integratedPanel.Margin = new System.Windows.Forms.Padding(0);
			this._integratedPanel.Name = "_integratedPanel";
			this._integratedPanel.Size = new System.Drawing.Size(576, 270);
			this._integratedPanel.TabIndex = 21;
			// 
			// UseIntegrated
			// 
			this.UseIntegrated.AutoSize = true;
			this.UseIntegrated.Location = new System.Drawing.Point(114, 26);
			this.UseIntegrated.Name = "UseIntegrated";
			this.UseIntegrated.Size = new System.Drawing.Size(159, 17);
			this.UseIntegrated.TabIndex = 23;
			this.UseIntegrated.TabStop = true;
			this.UseIntegrated.Text = "Use integrated build process";
			this.UseIntegrated.UseVisualStyleBackColor = true;
			// 
			// UseCustom
			// 
			this.UseCustom.AutoSize = true;
			this.UseCustom.Location = new System.Drawing.Point(282, 26);
			this.UseCustom.Name = "UseCustom";
			this.UseCustom.Size = new System.Drawing.Size(109, 17);
			this.UseCustom.TabIndex = 24;
			this.UseCustom.TabStop = true;
			this.UseCustom.Text = "Use custom script";
			this.UseCustom.UseVisualStyleBackColor = true;
			// 
			// _customPanel
			// 
			this._customPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._customPanel.Controls.Add(label10);
			this._customPanel.Controls.Add(label9);
			this._customPanel.Controls.Add(this.CustomDebugFile);
			this._customPanel.Controls.Add(this.button2);
			this._customPanel.Controls.Add(label8);
			this._customPanel.Controls.Add(this.CustomOutputFile);
			this._customPanel.Controls.Add(this.button1);
			this._customPanel.Controls.Add(label6);
			this._customPanel.Controls.Add(label7);
			this._customPanel.Controls.Add(this.CustomScript);
			this._customPanel.Controls.Add(label5);
			this._customPanel.Location = new System.Drawing.Point(0, 385);
			this._customPanel.Name = "_customPanel";
			this._customPanel.Size = new System.Drawing.Size(576, 245);
			this._customPanel.TabIndex = 25;
			this._customPanel.Visible = false;
			// 
			// CustomDebugFile
			// 
			this.CustomDebugFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CustomDebugFile.Location = new System.Drawing.Point(117, 146);
			this.CustomDebugFile.Name = "CustomDebugFile";
			this.CustomDebugFile.Size = new System.Drawing.Size(378, 20);
			this.CustomDebugFile.TabIndex = 22;
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(501, 145);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 23;
			this.button2.Text = "Browse...";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// CustomOutputFile
			// 
			this.CustomOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CustomOutputFile.Location = new System.Drawing.Point(117, 120);
			this.CustomOutputFile.Name = "CustomOutputFile";
			this.CustomOutputFile.Size = new System.Drawing.Size(378, 20);
			this.CustomOutputFile.TabIndex = 19;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(501, 119);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 20;
			this.button1.Text = "Browse...";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// CustomScript
			// 
			this.CustomScript.AcceptsReturn = true;
			this.CustomScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CustomScript.Location = new System.Drawing.Point(114, 0);
			this.CustomScript.Multiline = true;
			this.CustomScript.Name = "CustomScript";
			this.CustomScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.CustomScript.Size = new System.Drawing.Size(378, 75);
			this.CustomScript.TabIndex = 15;
			// 
			// ConfigurationSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._customPanel);
			this.Controls.Add(this.UseCustom);
			this.Controls.Add(this.UseIntegrated);
			this.Controls.Add(this._integratedPanel);
			this.Controls.Add(nameLabel);
			this.Controls.Add(this.ConfigurationName);
			this.MinimumSize = new System.Drawing.Size(579, 309);
			this.Name = "ConfigurationSettings";
			this.Size = new System.Drawing.Size(579, 633);
			this._integratedPanel.ResumeLayout(false);
			this._integratedPanel.PerformLayout();
			this._customPanel.ResumeLayout(false);
			this._customPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Button BrowseConfigFile;
		public TextBox ConfigurationName;
		public TextBox OutputFile;
		public TextBox ConfigurationFile;
		public CheckBox GenerateMapFile;
		public TextBox Symbols;
		public CheckBox CalculateSnesChecksum;
		private Label ChecksumLabel;
		private Panel _integratedPanel;
		private Panel _customPanel;
		public TextBox CustomScript;
		public TextBox CustomDebugFile;
		private Button button2;
		public TextBox CustomOutputFile;
		private Button button1;
		public RadioButton UseIntegrated;
		public RadioButton UseCustom;
	}
}
