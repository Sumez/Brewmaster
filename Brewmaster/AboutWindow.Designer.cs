namespace Brewmaster
{
    partial class AboutWindow
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
			System.Windows.Forms.Label text;
			System.Windows.Forms.Label header;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
			this.panel2 = new System.Windows.Forms.Panel();
			this.logoPicture1 = new Brewmaster.LogoPicture();
			this.link = new System.Windows.Forms.LinkLabel();
			this.closeButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			text = new System.Windows.Forms.Label();
			header = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPicture1)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// text
			// 
			text.AutoSize = true;
			text.Location = new System.Drawing.Point(113, 74);
			text.Name = "text";
			text.Size = new System.Drawing.Size(217, 65);
			text.TabIndex = 4;
			text.Text = "Version 0.1.0\r\n\r\nAn open source homebrew IDE for Windows\r\n\r\n2019-2020 Created by " +
    "Sumez";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.logoPicture1);
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Controls.Add(this.link);
			this.panel2.Controls.Add(this.closeButton);
			this.panel2.Controls.Add(text);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(387, 210);
			this.panel2.TabIndex = 0;
			// 
			// logoPicture1
			// 
			this.logoPicture1.Image = global::Brewmaster.Properties.Resources.logo;
			this.logoPicture1.Location = new System.Drawing.Point(24, 71);
			this.logoPicture1.Name = "logoPicture1";
			this.logoPicture1.Size = new System.Drawing.Size(64, 64);
			this.logoPicture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.logoPicture1.TabIndex = 8;
			this.logoPicture1.TabStop = false;
			// 
			// link
			// 
			this.link.AutoSize = true;
			this.link.Location = new System.Drawing.Point(113, 153);
			this.link.Name = "link";
			this.link.Size = new System.Drawing.Size(118, 13);
			this.link.TabIndex = 6;
			this.link.TabStop = true;
			this.link.Text = "https://brewmaster.dev";
			this.link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// header
			// 
			header.AutoSize = true;
			header.Font = new System.Drawing.Font("Microsoft Tai Le", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			header.Location = new System.Drawing.Point(18, 11);
			header.Name = "header";
			header.Size = new System.Drawing.Size(153, 34);
			header.TabIndex = 5;
			header.Text = "Brewmaster";
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(300, 175);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(65, 65);
			this.panel1.TabIndex = 0;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.White;
			this.panel3.Controls.Add(header);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(387, 55);
			this.panel3.TabIndex = 9;
			// 
			// AboutWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(387, 210);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutWindow";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About Brewmaster";
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPicture1)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.LinkLabel link;
		private LogoPicture logoPicture1;
		private System.Windows.Forms.Panel panel3;
	}
}