using System.Windows.Forms;

namespace Brewmaster.ProjectWizard
{
	partial class ImportProjectPath
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			this._projectName = new System.Windows.Forms.TextBox();
			this._projectDirectory = new System.Windows.Forms.TextBox();
			this._projectPathPreview = new System.Windows.Forms.Label();
			this._browseProjectPath = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(-3, 168);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(72, 13);
			label1.TabIndex = 5;
			label1.Text = "Project name:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(-3, 194);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(77, 13);
			label2.TabIndex = 6;
			label2.Text = "Base directory:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.Location = new System.Drawing.Point(-3, 15);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(186, 20);
			label4.TabIndex = 13;
			label4.Text = "Import an existing project";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(-3, 219);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(59, 13);
			label5.TabIndex = 14;
			label5.Text = "Project file:";
			// 
			// _projectName
			// 
			this._projectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._projectName.Location = new System.Drawing.Point(87, 165);
			this._projectName.Name = "_projectName";
			this._projectName.Size = new System.Drawing.Size(408, 20);
			this._projectName.TabIndex = 0;
			this._projectName.TextChanged += new System.EventHandler(this._projectName_TextChanged);
			// 
			// _projectDirectory
			// 
			this._projectDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._projectDirectory.Location = new System.Drawing.Point(87, 191);
			this._projectDirectory.Name = "_projectDirectory";
			this._projectDirectory.Size = new System.Drawing.Size(408, 20);
			this._projectDirectory.TabIndex = 1;
			this._projectDirectory.TextChanged += new System.EventHandler(this._projectDirectory_TextChanged);
			// 
			// _projectPathPreview
			// 
			this._projectPathPreview.AutoSize = true;
			this._projectPathPreview.Location = new System.Drawing.Point(84, 219);
			this._projectPathPreview.Name = "_projectPathPreview";
			this._projectPathPreview.Size = new System.Drawing.Size(0, 13);
			this._projectPathPreview.TabIndex = 3;
			// 
			// _browseProjectPath
			// 
			this._browseProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._browseProjectPath.Location = new System.Drawing.Point(502, 191);
			this._browseProjectPath.Name = "_browseProjectPath";
			this._browseProjectPath.Size = new System.Drawing.Size(75, 23);
			this._browseProjectPath.TabIndex = 4;
			this._browseProjectPath.Text = "Browse...";
			this._browseProjectPath.UseVisualStyleBackColor = true;
			this._browseProjectPath.Click += new System.EventHandler(this._browseProjectPath_Click);
			// 
			// ImportProjectPath
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(label5);
			this.Controls.Add(label4);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Controls.Add(this._browseProjectPath);
			this.Controls.Add(this._projectPathPreview);
			this.Controls.Add(this._projectDirectory);
			this.Controls.Add(this._projectName);
			this.Name = "ImportProjectPath";
			this.Size = new System.Drawing.Size(592, 309);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _projectName;
		private System.Windows.Forms.TextBox _projectDirectory;
		private System.Windows.Forms.Label _projectPathPreview;
		private System.Windows.Forms.Button _browseProjectPath;
	}
}
