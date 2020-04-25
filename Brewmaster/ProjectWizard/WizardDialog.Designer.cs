namespace Brewmaster.ProjectWizard
{
	partial class WizardDialog
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
			this.StepPanel = new System.Windows.Forms.Panel();
			this._finishButton = new System.Windows.Forms.Button();
			this._nextButton = new System.Windows.Forms.Button();
			this._previousButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// StepPanel
			// 
			this.StepPanel.Location = new System.Drawing.Point(12, 12);
			this.StepPanel.Name = "StepPanel";
			this.StepPanel.Size = new System.Drawing.Size(592, 299);
			this.StepPanel.TabIndex = 1;
			// 
			// _finishButton
			// 
			this._finishButton.Enabled = false;
			this._finishButton.Location = new System.Drawing.Point(448, 340);
			this._finishButton.Name = "_finishButton";
			this._finishButton.Size = new System.Drawing.Size(75, 23);
			this._finishButton.TabIndex = 2;
			this._finishButton.Text = "Finish >>";
			this._finishButton.UseVisualStyleBackColor = true;
			this._finishButton.Click += new System.EventHandler(this._finishButton_Click);
			// 
			// _nextButton
			// 
			this._nextButton.Enabled = false;
			this._nextButton.Location = new System.Drawing.Point(367, 340);
			this._nextButton.Name = "_nextButton";
			this._nextButton.Size = new System.Drawing.Size(75, 23);
			this._nextButton.TabIndex = 3;
			this._nextButton.Text = "Next >";
			this._nextButton.UseVisualStyleBackColor = true;
			this._nextButton.Click += new System.EventHandler(this._nextButton_Click);
			// 
			// _previousButton
			// 
			this._previousButton.Enabled = false;
			this._previousButton.Location = new System.Drawing.Point(286, 340);
			this._previousButton.Name = "_previousButton";
			this._previousButton.Size = new System.Drawing.Size(75, 23);
			this._previousButton.TabIndex = 4;
			this._previousButton.Text = "< Back";
			this._previousButton.UseVisualStyleBackColor = true;
			this._previousButton.Click += new System.EventHandler(this._previousButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(529, 340);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 6;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// WizardDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(616, 375);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._previousButton);
			this.Controls.Add(this._nextButton);
			this.Controls.Add(this._finishButton);
			this.Controls.Add(this.StepPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "WizardDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "New Project";
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel StepPanel;
		private System.Windows.Forms.Button _finishButton;
		private System.Windows.Forms.Button _nextButton;
		private System.Windows.Forms.Button _previousButton;
		private System.Windows.Forms.Button _cancelButton;
	}
}