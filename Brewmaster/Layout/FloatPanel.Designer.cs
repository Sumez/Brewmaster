using System.Windows.Forms;

namespace Brewmaster.Ide
{
	partial class FloatPanel
	{
		private System.ComponentModel.IContainer components = null;

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
			this.SuspendLayout();
			// 
			// FloatPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(308, 246);
			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FloatPanel";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.FloatPanel_Load);
			this.ResumeLayout(false);

		}

		#endregion




	}
}