namespace BrewMaster.Settings
{
	partial class ProjectSettingsWindow
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Panel panel1;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Panel panel2;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label1;
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.ConfigurationBox = new System.Windows.Forms.GroupBox();
			this.configurationSettings1 = new BrewMaster.ProjectWizard.ConfigurationSettings();
			this.ConfigurationSelector = new System.Windows.Forms.ComboBox();
			this.ProjectName = new System.Windows.Forms.TextBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.projectTab = new System.Windows.Forms.TabPage();
			this.DeleteConfigurationButton = new System.Windows.Forms.Button();
			panel1 = new System.Windows.Forms.Panel();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			panel2 = new System.Windows.Forms.Panel();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label1 = new System.Windows.Forms.Label();
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			this.ConfigurationBox.SuspendLayout();
			groupBox1.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.projectTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			panel1.Controls.Add(this.okButton);
			panel1.Controls.Add(this.cancelButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			panel1.Location = new System.Drawing.Point(0, 433);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(607, 38);
			panel1.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(436, 8);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(517, 8);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(6, 23);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(60, 13);
			label7.TabIndex = 0;
			label7.Text = "CA65 path:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(6, 66);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(60, 13);
			label8.TabIndex = 1;
			label8.Text = "LD65 path:";
			// 
			// panel2
			// 
			panel2.Controls.Add(this.ConfigurationBox);
			panel2.Controls.Add(groupBox1);
			panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			panel2.Location = new System.Drawing.Point(0, 0);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(607, 433);
			panel2.TabIndex = 2;
			// 
			// ConfigurationBox
			// 
			this.ConfigurationBox.Controls.Add(this.configurationSettings1);
			this.ConfigurationBox.Controls.Add(this.DeleteConfigurationButton);
			this.ConfigurationBox.Controls.Add(this.ConfigurationSelector);
			this.ConfigurationBox.Location = new System.Drawing.Point(7, 68);
			this.ConfigurationBox.Name = "ConfigurationBox";
			this.ConfigurationBox.Size = new System.Drawing.Size(594, 357);
			this.ConfigurationBox.TabIndex = 1;
			this.ConfigurationBox.TabStop = false;
			this.ConfigurationBox.Text = "Build configuration";
			// 
			// configurationSettings1
			// 
			this.configurationSettings1.Location = new System.Drawing.Point(9, 45);
			this.configurationSettings1.Name = "configurationSettings1";
			this.configurationSettings1.Size = new System.Drawing.Size(579, 311);
			this.configurationSettings1.TabIndex = 1;
			// 
			// ConfigurationSelector
			// 
			this.ConfigurationSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ConfigurationSelector.FormattingEnabled = true;
			this.ConfigurationSelector.Location = new System.Drawing.Point(9, 16);
			this.ConfigurationSelector.Name = "ConfigurationSelector";
			this.ConfigurationSelector.Size = new System.Drawing.Size(175, 21);
			this.ConfigurationSelector.TabIndex = 0;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(this.ProjectName);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(7, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(585, 50);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "General";
			// 
			// ProjectName
			// 
			this.ProjectName.Location = new System.Drawing.Point(84, 21);
			this.ProjectName.Name = "ProjectName";
			this.ProjectName.Size = new System.Drawing.Size(495, 20);
			this.ProjectName.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 24);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(72, 13);
			label1.TabIndex = 0;
			label1.Text = "Project name:";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.projectTab);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = new System.Drawing.Point(10, 5);
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(607, 471);
			this.tabControl.TabIndex = 0;
			this.tabControl.Visible = false;
			// 
			// projectTab
			// 
			this.projectTab.BackColor = System.Drawing.SystemColors.Control;
			this.projectTab.Controls.Add(label8);
			this.projectTab.Controls.Add(label7);
			this.projectTab.Location = new System.Drawing.Point(4, 26);
			this.projectTab.Name = "projectTab";
			this.projectTab.Padding = new System.Windows.Forms.Padding(3);
			this.projectTab.Size = new System.Drawing.Size(599, 441);
			this.projectTab.TabIndex = 2;
			this.projectTab.Text = "Assembler";
			// 
			// DeleteConfigurationButton
			// 
			this.DeleteConfigurationButton.Location = new System.Drawing.Point(190, 15);
			this.DeleteConfigurationButton.Name = "DeleteConfigurationButton";
			this.DeleteConfigurationButton.Size = new System.Drawing.Size(129, 23);
			this.DeleteConfigurationButton.TabIndex = 2;
			this.DeleteConfigurationButton.Text = "Delete configuration";
			this.DeleteConfigurationButton.UseVisualStyleBackColor = true;
			this.DeleteConfigurationButton.Click += new System.EventHandler(this.DeleteConfigurationButton_Click);
			// 
			// ProjectSettingsWindow
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(607, 471);
			this.Controls.Add(panel2);
			this.Controls.Add(panel1);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(623, 510);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(623, 510);
			this.Name = "ProjectSettingsWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Project Settings";
			panel1.ResumeLayout(false);
			panel2.ResumeLayout(false);
			this.ConfigurationBox.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.projectTab.ResumeLayout(false);
			this.projectTab.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TabPage projectTab;
		private System.Windows.Forms.TextBox ProjectName;
		private ProjectWizard.ConfigurationSettings configurationSettings1;
		private System.Windows.Forms.ComboBox ConfigurationSelector;
		private System.Windows.Forms.GroupBox ConfigurationBox;
		private System.Windows.Forms.Button DeleteConfigurationButton;
	}
}
