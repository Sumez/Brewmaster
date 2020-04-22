namespace BrewMaster.EditorWindows.Text
{
    partial class FindWindow
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
			System.Windows.Forms.Label label1;
			this.panel1 = new System.Windows.Forms.Panel();
			this.StatusLabel = new System.Windows.Forms.Label();
			this.CancelButton = new System.Windows.Forms.Button();
			this.FindNextButton = new System.Windows.Forms.Button();
			this.SearchQuery = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(7, 14);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(59, 13);
			label1.TabIndex = 6;
			label1.Text = "Find what: ";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.Location = new System.Drawing.Point(10, 41);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(372, 1);
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
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(307, 51);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 9;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// FindNextButton
			// 
			this.FindNextButton.Location = new System.Drawing.Point(226, 51);
			this.FindNextButton.Name = "FindNextButton";
			this.FindNextButton.Size = new System.Drawing.Size(75, 23);
			this.FindNextButton.TabIndex = 8;
			this.FindNextButton.Text = "Find Next";
			this.FindNextButton.UseVisualStyleBackColor = true;
			this.FindNextButton.Click += new System.EventHandler(this.FindNextButton_Click);
			// 
			// SearchQuery
			// 
			this.SearchQuery.Location = new System.Drawing.Point(87, 10);
			this.SearchQuery.Name = "SearchQuery";
			this.SearchQuery.Size = new System.Drawing.Size(295, 20);
			this.SearchQuery.TabIndex = 7;
			this.SearchQuery.TextChanged += new System.EventHandler(this.SearchQuery_TextChanged);
			// 
			// FindWindow
			// 
			this.AcceptButton = this.FindNextButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelButton;
			this.ClientSize = new System.Drawing.Size(398, 84);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.StatusLabel);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.FindNextButton);
			this.Controls.Add(this.SearchQuery);
			this.Controls.Add(label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindWindow";
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
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button FindNextButton;
        private System.Windows.Forms.TextBox SearchQuery;
    }
}