using System.Windows.Forms;

namespace Brewmaster.ProjectWizard
{
	partial class NewProjectPath
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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			this._projectName = new System.Windows.Forms.TextBox();
			this._projectDirectory = new System.Windows.Forms.TextBox();
			this._projectFolderName = new System.Windows.Forms.TextBox();
			this._projectPathPreview = new System.Windows.Forms.Label();
			this._browseProjectPath = new System.Windows.Forms.Button();
			this._nesImage = new System.Windows.Forms.PictureBox();
			this._nesProjectOption = new System.Windows.Forms.RadioButton();
			this._snesProjectOption = new System.Windows.Forms.RadioButton();
			this._snesImage = new System.Windows.Forms.PictureBox();
			this._platformSelection = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._nesImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._snesImage)).BeginInit();
			this._platformSelection.SuspendLayout();
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
			label2.Size = new System.Drawing.Size(84, 13);
			label2.TabIndex = 6;
			label2.Text = "Parent directory:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(-3, 220);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(68, 13);
			label3.TabIndex = 7;
			label3.Text = "Folder name:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.Location = new System.Drawing.Point(-3, 15);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(155, 20);
			label4.TabIndex = 13;
			label4.Text = "Create a new project";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(-3, 245);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(59, 13);
			label5.TabIndex = 14;
			label5.Text = "Project file:";
			// 
			// _projectName
			// 
			this._projectName.Location = new System.Drawing.Point(87, 165);
			this._projectName.Name = "_projectName";
			this._projectName.Size = new System.Drawing.Size(408, 20);
			this._projectName.TabIndex = 0;
			this._projectName.Text = "New Project";
			this._projectName.TextChanged += new System.EventHandler(this._projectName_TextChanged);
			// 
			// _projectDirectory
			// 
			this._projectDirectory.Location = new System.Drawing.Point(87, 191);
			this._projectDirectory.Name = "_projectDirectory";
			this._projectDirectory.Size = new System.Drawing.Size(408, 20);
			this._projectDirectory.TabIndex = 1;
			this._projectDirectory.TextChanged += new System.EventHandler(this._projectDirectory_TextChanged);
			// 
			// _projectFolderName
			// 
			this._projectFolderName.Location = new System.Drawing.Point(87, 217);
			this._projectFolderName.Name = "_projectFolderName";
			this._projectFolderName.Size = new System.Drawing.Size(408, 20);
			this._projectFolderName.TabIndex = 2;
			this._projectFolderName.Text = "New Project";
			this._projectFolderName.TextChanged += new System.EventHandler(this._projectFolderName_TextChanged);
			// 
			// _projectPathPreview
			// 
			this._projectPathPreview.AutoSize = true;
			this._projectPathPreview.Location = new System.Drawing.Point(84, 245);
			this._projectPathPreview.Name = "_projectPathPreview";
			this._projectPathPreview.Size = new System.Drawing.Size(0, 13);
			this._projectPathPreview.TabIndex = 3;
			// 
			// _browseProjectPath
			// 
			this._browseProjectPath.Location = new System.Drawing.Point(502, 191);
			this._browseProjectPath.Name = "_browseProjectPath";
			this._browseProjectPath.Size = new System.Drawing.Size(75, 23);
			this._browseProjectPath.TabIndex = 4;
			this._browseProjectPath.Text = "Browse...";
			this._browseProjectPath.UseVisualStyleBackColor = true;
			this._browseProjectPath.Click += new System.EventHandler(this._browseProjectPath_Click);
			// 
			// _nesImage
			// 
			this._nesImage.Location = new System.Drawing.Point(167, 15);
			this._nesImage.Name = "_nesImage";
			this._nesImage.Size = new System.Drawing.Size(64, 64);
			this._nesImage.TabIndex = 8;
			this._nesImage.TabStop = false;
			this._nesImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this._nesImage_Click);
			// 
			// _nesProjectOption
			// 
			this._nesProjectOption.AutoSize = true;
			this._nesProjectOption.Location = new System.Drawing.Point(148, 87);
			this._nesProjectOption.Name = "_nesProjectOption";
			this._nesProjectOption.Size = new System.Drawing.Size(83, 17);
			this._nesProjectOption.TabIndex = 10;
			this._nesProjectOption.TabStop = true;
			this._nesProjectOption.Text = "NES Project";
			this._nesProjectOption.UseVisualStyleBackColor = true;
			// 
			// _snesProjectOption
			// 
			this._snesProjectOption.AutoSize = true;
			this._snesProjectOption.Location = new System.Drawing.Point(319, 87);
			this._snesProjectOption.Name = "_snesProjectOption";
			this._snesProjectOption.Size = new System.Drawing.Size(90, 17);
			this._snesProjectOption.TabIndex = 11;
			this._snesProjectOption.TabStop = true;
			this._snesProjectOption.Text = "SNES Project";
			this._snesProjectOption.UseVisualStyleBackColor = true;
			// 
			// _snesImage
			// 
			this._snesImage.Location = new System.Drawing.Point(337, 17);
			this._snesImage.Name = "_snesImage";
			this._snesImage.Size = new System.Drawing.Size(64, 64);
			this._snesImage.TabIndex = 12;
			this._snesImage.TabStop = false;
			this._snesImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this._snesImage_Click);
			// 
			// _platformSelection
			// 
			this._platformSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._platformSelection.Controls.Add(this._snesImage);
			this._platformSelection.Controls.Add(this._snesProjectOption);
			this._platformSelection.Controls.Add(this._nesProjectOption);
			this._platformSelection.Controls.Add(this._nesImage);
			this._platformSelection.Location = new System.Drawing.Point(0, 40);
			this._platformSelection.Name = "_platformSelection";
			this._platformSelection.Size = new System.Drawing.Size(592, 119);
			this._platformSelection.TabIndex = 15;
			// 
			// NewProjectPath
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._platformSelection);
			this.Controls.Add(label5);
			this.Controls.Add(label4);
			this.Controls.Add(label3);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Controls.Add(this._browseProjectPath);
			this.Controls.Add(this._projectPathPreview);
			this.Controls.Add(this._projectFolderName);
			this.Controls.Add(this._projectDirectory);
			this.Controls.Add(this._projectName);
			this.Name = "NewProjectPath";
			this.Size = new System.Drawing.Size(592, 309);
			((System.ComponentModel.ISupportInitialize)(this._nesImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._snesImage)).EndInit();
			this._platformSelection.ResumeLayout(false);
			this._platformSelection.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _projectName;
		private System.Windows.Forms.TextBox _projectDirectory;
		private System.Windows.Forms.TextBox _projectFolderName;
		private System.Windows.Forms.Label _projectPathPreview;
		private System.Windows.Forms.Button _browseProjectPath;
		private System.Windows.Forms.PictureBox _nesImage;
		private System.Windows.Forms.RadioButton _nesProjectOption;
		private System.Windows.Forms.RadioButton _snesProjectOption;
		private System.Windows.Forms.PictureBox _snesImage;
		private Panel _platformSelection;
	}
}
