using System.Windows.Forms;

namespace BrewMaster.ProjectWizard
{
	partial class ImportFile
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
			this.EnableFile = new System.Windows.Forms.CheckBox();
			this.EnableProcessing = new System.Windows.Forms.CheckBox();
			this.ContentType = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// EnableFile
			// 
			this.EnableFile.Location = new System.Drawing.Point(0, 2);
			this.EnableFile.Name = "EnableFile";
			this.EnableFile.Size = new System.Drawing.Size(308, 17);
			this.EnableFile.TabIndex = 0;
			this.EnableFile.Text = "C:\\Users\\File\\Name.s";
			this.EnableFile.UseVisualStyleBackColor = true;
			this.EnableFile.CheckedChanged += new System.EventHandler(this.EnableFile_CheckedChanged);
			// 
			// EnableProcessing
			// 
			this.EnableProcessing.AutoSize = true;
			this.EnableProcessing.Location = new System.Drawing.Point(441, 2);
			this.EnableProcessing.Name = "EnableProcessing";
			this.EnableProcessing.Size = new System.Drawing.Size(118, 17);
			this.EnableProcessing.TabIndex = 2;
			this.EnableProcessing.Text = "Include in assembly";
			this.EnableProcessing.UseVisualStyleBackColor = true;
			// 
			// ContentType
			// 
			this.ContentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ContentType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ContentType.FormattingEnabled = true;
			this.ContentType.Location = new System.Drawing.Point(314, 0);
			this.ContentType.Name = "ContentType";
			this.ContentType.Size = new System.Drawing.Size(121, 21);
			this.ContentType.TabIndex = 1;
			this.ContentType.SelectedIndexChanged += new System.EventHandler(this.ContentType_SelectedIndexChanged);
			// 
			// ImportFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.EnableProcessing);
			this.Controls.Add(this.ContentType);
			this.Controls.Add(this.EnableFile);
			this.MaximumSize = new System.Drawing.Size(0, 24);
			this.Name = "ImportFile";
			this.Size = new System.Drawing.Size(562, 24);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CheckBox EnableFile;
		private CheckBox EnableProcessing;
		private ComboBox ContentType;
	}
}
