namespace BrewMaster.ProjectWizard
{
	partial class ConfigurationManager
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.ConfigurationSettings = new BrewMaster.ProjectWizard.ConfigurationSettings();
			panel1 = new System.Windows.Forms.Panel();
			panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			panel1.Controls.Add(this.okButton);
			panel1.Controls.Add(this.cancelButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			panel1.Location = new System.Drawing.Point(0, 337);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(604, 38);
			panel1.TabIndex = 2;
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
			// ConfigurationSettings
			// 
			this.ConfigurationSettings.Location = new System.Drawing.Point(12, 12);
			this.ConfigurationSettings.Name = "ConfigurationSettings";
			this.ConfigurationSettings.Size = new System.Drawing.Size(592, 303);
			this.ConfigurationSettings.TabIndex = 0;
			// 
			// ConfigurationManager
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(604, 375);
			this.Controls.Add(panel1);
			this.Controls.Add(this.ConfigurationSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ConfigurationManager";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "New Configuration";
			panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private ConfigurationSettings ConfigurationSettings;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}