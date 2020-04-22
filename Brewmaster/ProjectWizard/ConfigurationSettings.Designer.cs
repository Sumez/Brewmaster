using System.Windows.Forms;

namespace BrewMaster.ProjectWizard
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
			this.OutputFile = new System.Windows.Forms.TextBox();
			this.ConfigurationFile = new System.Windows.Forms.TextBox();
			this.BrowseConfigFile = new System.Windows.Forms.Button();
			this.GenerateMapFile = new System.Windows.Forms.CheckBox();
			this.Symbols = new System.Windows.Forms.TextBox();
			this.ConfigurationName = new System.Windows.Forms.TextBox();
			configurationLabel = new System.Windows.Forms.Label();
			outputLabel = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			nameLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// configurationLabel
			// 
			configurationLabel.AutoSize = true;
			configurationLabel.Location = new System.Drawing.Point(-3, 69);
			configurationLabel.Name = "configurationLabel";
			configurationLabel.Size = new System.Drawing.Size(88, 13);
			configurationLabel.TabIndex = 8;
			configurationLabel.Text = "Configuration file:";
			// 
			// outputLabel
			// 
			outputLabel.AutoSize = true;
			outputLabel.Location = new System.Drawing.Point(-3, 43);
			outputLabel.Name = "outputLabel";
			outputLabel.Size = new System.Drawing.Size(58, 13);
			outputLabel.TabIndex = 7;
			outputLabel.Text = "Output file:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(-3, 95);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(94, 13);
			label1.TabIndex = 11;
			label1.Text = "Generate Map file:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(-3, 121);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(81, 13);
			label2.TabIndex = 14;
			label2.Text = "Define symbols:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(111, 244);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(148, 13);
			label3.TabIndex = 15;
			label3.Text = "One symbol per line. Example:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(256, 244);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(67, 39);
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
			// OutputFile
			// 
			this.OutputFile.Location = new System.Drawing.Point(114, 40);
			this.OutputFile.Name = "OutputFile";
			this.OutputFile.Size = new System.Drawing.Size(381, 20);
			this.OutputFile.TabIndex = 1;
			// 
			// ConfigurationFile
			// 
			this.ConfigurationFile.Location = new System.Drawing.Point(114, 66);
			this.ConfigurationFile.Name = "ConfigurationFile";
			this.ConfigurationFile.Size = new System.Drawing.Size(381, 20);
			this.ConfigurationFile.TabIndex = 3;
			// 
			// BrowseConfigFile
			// 
			this.BrowseConfigFile.Location = new System.Drawing.Point(501, 65);
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
			this.GenerateMapFile.Location = new System.Drawing.Point(114, 94);
			this.GenerateMapFile.Name = "GenerateMapFile";
			this.GenerateMapFile.Size = new System.Drawing.Size(110, 17);
			this.GenerateMapFile.TabIndex = 5;
			this.GenerateMapFile.Text = "Generate Map file";
			this.GenerateMapFile.UseVisualStyleBackColor = true;
			// 
			// Symbols
			// 
			this.Symbols.AcceptsReturn = true;
			this.Symbols.Location = new System.Drawing.Point(114, 117);
			this.Symbols.Multiline = true;
			this.Symbols.Name = "Symbols";
			this.Symbols.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Symbols.Size = new System.Drawing.Size(381, 124);
			this.Symbols.TabIndex = 6;
			// 
			// ConfigurationName
			// 
			this.ConfigurationName.Location = new System.Drawing.Point(114, 0);
			this.ConfigurationName.Name = "ConfigurationName";
			this.ConfigurationName.Size = new System.Drawing.Size(381, 20);
			this.ConfigurationName.TabIndex = 0;
			// 
			// ConfigurationSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(nameLabel);
			this.Controls.Add(this.ConfigurationName);
			this.Controls.Add(label4);
			this.Controls.Add(label3);
			this.Controls.Add(label2);
			this.Controls.Add(this.Symbols);
			this.Controls.Add(this.GenerateMapFile);
			this.Controls.Add(label1);
			this.Controls.Add(this.BrowseConfigFile);
			this.Controls.Add(this.ConfigurationFile);
			this.Controls.Add(configurationLabel);
			this.Controls.Add(outputLabel);
			this.Controls.Add(this.OutputFile);
			this.Name = "ConfigurationSettings";
			this.Size = new System.Drawing.Size(579, 309);
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
	}
}
