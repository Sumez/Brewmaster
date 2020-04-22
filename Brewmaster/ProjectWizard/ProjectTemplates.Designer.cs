using System.Windows.Forms;

namespace BrewMaster.ProjectWizard
{
	partial class ProjectTemplates
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
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Items.AddRange(new object[] {
            "LoROM Base",
            "SNES Include File",
            "VRAM copy routines",
            "\"Hello World\" example",
            "Basic movement example"});
			this.checkedListBox1.Location = new System.Drawing.Point(432, 3);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(157, 304);
			this.checkedListBox1.TabIndex = 0;
			// 
			// ProjectTemplates
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkedListBox1);
			this.Name = "ProjectTemplates";
			this.Size = new System.Drawing.Size(592, 309);
			this.ResumeLayout(false);

		}

		#endregion

		private CheckedListBox checkedListBox1;
	}
}
