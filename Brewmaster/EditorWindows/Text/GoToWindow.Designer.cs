namespace Brewmaster.EditorWindows.Text
{
    partial class GoToWindow
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
			this.GoToLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.StatusLabel = new System.Windows.Forms.Label();
			this.ButtonCancel = new System.Windows.Forms.Button();
			this.GoToButton = new System.Windows.Forms.Button();
			this.GoToLineTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// GoToLabel
			// 
			this.GoToLabel.AutoSize = true;
			this.GoToLabel.Location = new System.Drawing.Point(7, 14);
			this.GoToLabel.Name = "GoToLabel";
			this.GoToLabel.Size = new System.Drawing.Size(100, 13);
			this.GoToLabel.TabIndex = 6;
			this.GoToLabel.Text = "Go to line (1 - 190): ";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.Location = new System.Drawing.Point(10, 41);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(223, 1);
			this.panel1.TabIndex = 11;
			// 
			// StatusLabel
			// 
			this.StatusLabel.AutoSize = true;
			this.StatusLabel.Location = new System.Drawing.Point(7, 48);
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(0, 13);
			this.StatusLabel.TabIndex = 10;
			// 
			// ButtonCancel
			// 
			this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ButtonCancel.Location = new System.Drawing.Point(158, 49);
			this.ButtonCancel.Name = "ButtonCancel";
			this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
			this.ButtonCancel.TabIndex = 9;
			this.ButtonCancel.Text = "Cancel";
			this.ButtonCancel.UseVisualStyleBackColor = true;
			// 
			// GoToButton
			// 
			this.GoToButton.Location = new System.Drawing.Point(77, 49);
			this.GoToButton.Name = "GoToButton";
			this.GoToButton.Size = new System.Drawing.Size(75, 23);
			this.GoToButton.TabIndex = 8;
			this.GoToButton.Text = "Go To";
			this.GoToButton.UseVisualStyleBackColor = true;
			this.GoToButton.Click += new System.EventHandler(this.GoToButton_Click);
			// 
			// GoToLineTextBox
			// 
			this.GoToLineTextBox.Location = new System.Drawing.Point(115, 11);
			this.GoToLineTextBox.Name = "GoToLineTextBox";
			this.GoToLineTextBox.Size = new System.Drawing.Size(118, 20);
			this.GoToLineTextBox.TabIndex = 7;
			this.GoToLineTextBox.TextChanged += new System.EventHandler(this.GoToLine_TextChanged);
			// 
			// GoToWindow
			// 
			this.AcceptButton = this.GoToButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.ButtonCancel;
			this.ClientSize = new System.Drawing.Size(243, 84);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.StatusLabel);
			this.Controls.Add(this.ButtonCancel);
			this.Controls.Add(this.GoToButton);
			this.Controls.Add(this.GoToLineTextBox);
			this.Controls.Add(this.GoToLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GoToWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Find";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button GoToButton;
        private System.Windows.Forms.TextBox GoToLineTextBox;
		private System.Windows.Forms.Label GoToLabel;
	}
}