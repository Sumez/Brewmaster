namespace BrewMaster.Modules.Watch
{
	partial class WatchValues
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
			this.watchList = new BrewMaster.Modules.Watch.WatchList();
			this.SuspendLayout();
			// 
			// watchList
			// 
			this.watchList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.watchList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.watchList.Location = new System.Drawing.Point(2, 0);
			this.watchList.Name = "watchList";
			this.watchList.Size = new System.Drawing.Size(273, 181);
			this.watchList.TabIndex = 0;
			this.watchList.UseCompatibleStateImageBehavior = false;
			this.watchList.View = System.Windows.Forms.View.Details;
			// 
			// WatchValues
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.watchList);
			this.MinimumSize = new System.Drawing.Size(275, 0);
			this.Name = "WatchValues";
			this.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.Size = new System.Drawing.Size(275, 181);
			this.ResumeLayout(false);

		}

		#endregion

		private WatchList watchList;
	}
}
