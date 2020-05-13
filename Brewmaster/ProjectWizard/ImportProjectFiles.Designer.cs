using System.Windows.Forms;

namespace Brewmaster.ProjectWizard
{
	partial class ImportProjectFiles
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
			this.FilePanel = new System.Windows.Forms.Panel();
			this.FileList = new System.Windows.Forms.Panel();
			this.ScrollBar = new System.Windows.Forms.VScrollBar();
			this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.FilePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
			this.SuspendLayout();
			// 
			// FilePanel
			// 
			this.FilePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FilePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FilePanel.Controls.Add(this.FileList);
			this.FilePanel.Controls.Add(this.ScrollBar);
			this.FilePanel.Location = new System.Drawing.Point(3, 31);
			this.FilePanel.Name = "FilePanel";
			this.FilePanel.Size = new System.Drawing.Size(586, 260);
			this.FilePanel.TabIndex = 0;
			// 
			// FileList
			// 
			this.FileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FileList.Location = new System.Drawing.Point(3, 0);
			this.FileList.Name = "FileList";
			this.FileList.Size = new System.Drawing.Size(564, 307);
			this.FileList.TabIndex = 1;
			this.FileList.Visible = false;
			// 
			// ScrollBar
			// 
			this.ScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.ScrollBar.Location = new System.Drawing.Point(567, 0);
			this.ScrollBar.Name = "ScrollBar";
			this.ScrollBar.Size = new System.Drawing.Size(17, 258);
			this.ScrollBar.TabIndex = 0;
			this.ScrollBar.Visible = false;
			// 
			// ImportProjectFiles
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.FilePanel);
			this.Name = "ImportProjectFiles";
			this.Size = new System.Drawing.Size(592, 309);
			this.FilePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Panel FilePanel;
		private BindingSource bindingSource1;
		private VScrollBar ScrollBar;
		private Panel FileList;
	}
}
