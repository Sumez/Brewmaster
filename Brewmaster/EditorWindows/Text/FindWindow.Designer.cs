namespace Brewmaster.EditorWindows.Text
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
			this._cancelButton = new System.Windows.Forms.Button();
			this.FindNextButton = new System.Windows.Forms.Button();
			this.SearchQuery = new System.Windows.Forms.TextBox();
			this.ReplaceWith = new System.Windows.Forms.TextBox();
			this.ReplaceLabel = new System.Windows.Forms.Label();
			this.AllFiles = new System.Windows.Forms.CheckBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.ReplaceButton = new System.Windows.Forms.Button();
			this.ReplaceAllButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
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
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.Location = new System.Drawing.Point(10, 62);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(372, 1);
			this.panel1.TabIndex = 11;
			// 
			// StatusLabel
			// 
			this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.StatusLabel.AutoSize = true;
			this.StatusLabel.Location = new System.Drawing.Point(7, 76);
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(0, 13);
			this.StatusLabel.TabIndex = 10;
			// 
			// CancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(246, 3);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 6;
			this._cancelButton.Text = "Close";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// FindNextButton
			// 
			this.FindNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.FindNextButton.Location = new System.Drawing.Point(3, 3);
			this.FindNextButton.Name = "FindNextButton";
			this.FindNextButton.Size = new System.Drawing.Size(75, 23);
			this.FindNextButton.TabIndex = 4;
			this.FindNextButton.Text = "Find Next";
			this.FindNextButton.UseVisualStyleBackColor = true;
			this.FindNextButton.Click += new System.EventHandler(this.FindNextButton_Click);
			// 
			// SearchQuery
			// 
			this.SearchQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SearchQuery.Location = new System.Drawing.Point(87, 10);
			this.SearchQuery.Name = "SearchQuery";
			this.SearchQuery.Size = new System.Drawing.Size(295, 20);
			this.SearchQuery.TabIndex = 1;
			this.SearchQuery.TextChanged += new System.EventHandler(this.SearchQuery_TextChanged);
			// 
			// ReplaceWith
			// 
			this.ReplaceWith.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ReplaceWith.Location = new System.Drawing.Point(87, 36);
			this.ReplaceWith.Name = "ReplaceWith";
			this.ReplaceWith.Size = new System.Drawing.Size(295, 20);
			this.ReplaceWith.TabIndex = 2;
			// 
			// ReplaceLabel
			// 
			this.ReplaceLabel.AutoSize = true;
			this.ReplaceLabel.Location = new System.Drawing.Point(7, 40);
			this.ReplaceLabel.Name = "ReplaceLabel";
			this.ReplaceLabel.Size = new System.Drawing.Size(72, 13);
			this.ReplaceLabel.TabIndex = 12;
			this.ReplaceLabel.Text = "Replace with:";
			// 
			// AllFiles
			// 
			this.AllFiles.AutoSize = true;
			this.AllFiles.Location = new System.Drawing.Point(288, 36);
			this.AllFiles.Name = "AllFiles";
			this.AllFiles.Size = new System.Drawing.Size(94, 17);
			this.AllFiles.TabIndex = 3;
			this.AllFiles.Text = "Search all files";
			this.AllFiles.UseVisualStyleBackColor = true;
			this.AllFiles.CheckedChanged += new System.EventHandler(this.AllFiles_CheckedChanged);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this.ReplaceAllButton);
			this.flowLayoutPanel1.Controls.Add(this.ReplaceButton);
			this.flowLayoutPanel1.Controls.Add(this.FindNextButton);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(61, 68);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(321, 29);
			this.flowLayoutPanel1.TabIndex = 15;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// ReplaceButton
			// 
			this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ReplaceButton.Location = new System.Drawing.Point(84, 3);
			this.ReplaceButton.Name = "ReplaceButton";
			this.ReplaceButton.Size = new System.Drawing.Size(75, 23);
			this.ReplaceButton.TabIndex = 5;
			this.ReplaceButton.Text = "Replace";
			this.ReplaceButton.UseVisualStyleBackColor = true;
			this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
			// 
			// ReplaceAllButton
			// 
			this.ReplaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ReplaceAllButton.Location = new System.Drawing.Point(165, 3);
			this.ReplaceAllButton.Name = "ReplaceAllButton";
			this.ReplaceAllButton.Size = new System.Drawing.Size(75, 23);
			this.ReplaceAllButton.TabIndex = 7;
			this.ReplaceAllButton.Text = "Replace All";
			this.ReplaceAllButton.UseVisualStyleBackColor = true;
			this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
			// 
			// FindWindow
			// 
			this.AcceptButton = this.FindNextButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(398, 106);
			this.Controls.Add(this.StatusLabel);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.AllFiles);
			this.Controls.Add(this.ReplaceWith);
			this.Controls.Add(this.ReplaceLabel);
			this.Controls.Add(this.panel1);
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
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button FindNextButton;
        private System.Windows.Forms.TextBox SearchQuery;
		private System.Windows.Forms.TextBox ReplaceWith;
		private System.Windows.Forms.Label ReplaceLabel;
		private System.Windows.Forms.CheckBox AllFiles;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button ReplaceButton;
		private System.Windows.Forms.Button ReplaceAllButton;
	}
}