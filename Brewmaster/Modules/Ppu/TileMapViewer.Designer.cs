namespace Brewmaster.Modules.Ppu
{
	partial class TileMapViewer
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
			System.Windows.Forms.ToolTip RegisterToolTip;
			this._controlPanel = new System.Windows.Forms.Panel();
			this._displayButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._viewportButton = new System.Windows.Forms.CheckBox();
			this._scaleButton = new System.Windows.Forms.CheckBox();
			this._layerButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._tileMapDisplay = new TileMapRender();
			this.horizontalLine1 = new Brewmaster.StatusView.HorizontalLine();
			RegisterToolTip = new System.Windows.Forms.ToolTip(this.components);
			this._controlPanel.SuspendLayout();
			this._displayButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _controlPanel
			// 
			this._controlPanel.Controls.Add(this._displayButtonPanel);
			this._controlPanel.Controls.Add(this._layerButtonPanel);
			this._controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._controlPanel.Location = new System.Drawing.Point(0, 598);
			this._controlPanel.Name = "_controlPanel";
			this._controlPanel.Size = new System.Drawing.Size(378, 24);
			this._controlPanel.TabIndex = 1;
			// 
			// _displayButtonPanel
			// 
			this._displayButtonPanel.AutoSize = true;
			this._displayButtonPanel.Controls.Add(this._viewportButton);
			this._displayButtonPanel.Controls.Add(this._scaleButton);
			this._displayButtonPanel.Dock = System.Windows.Forms.DockStyle.Right;
			this._displayButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._displayButtonPanel.Location = new System.Drawing.Point(267, 0);
			this._displayButtonPanel.Name = "_displayButtonPanel";
			this._displayButtonPanel.Size = new System.Drawing.Size(111, 24);
			this._displayButtonPanel.TabIndex = 1;
			this._displayButtonPanel.WrapContents = false;
			// 
			// _viewportButton
			// 
			this._viewportButton.Appearance = System.Windows.Forms.Appearance.Button;
			this._viewportButton.Location = new System.Drawing.Point(51, 2);
			this._viewportButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this._viewportButton.Name = "_viewportButton";
			this._viewportButton.Size = new System.Drawing.Size(58, 20);
			this._viewportButton.TabIndex = 0;
			this._viewportButton.Text = "Viewport";
			this._viewportButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._viewportButton.UseVisualStyleBackColor = true;
			this._viewportButton.CheckedChanged += new System.EventHandler(this._viewportButton_CheckedChanged);
			// 
			// _scaleButton
			// 
			this._scaleButton.Appearance = System.Windows.Forms.Appearance.Button;
			this._scaleButton.Location = new System.Drawing.Point(0, 2);
			this._scaleButton.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this._scaleButton.Name = "_scaleButton";
			this._scaleButton.Size = new System.Drawing.Size(49, 20);
			this._scaleButton.TabIndex = 1;
			this._scaleButton.Text = "Resize";
			this._scaleButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._scaleButton.UseVisualStyleBackColor = true;
			this._scaleButton.CheckedChanged += new System.EventHandler(this._scaleButton_CheckedChanged);
			// 
			// _layerButtonPanel
			// 
			this._layerButtonPanel.AutoSize = true;
			this._layerButtonPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this._layerButtonPanel.Location = new System.Drawing.Point(0, 0);
			this._layerButtonPanel.MinimumSize = new System.Drawing.Size(30, 25);
			this._layerButtonPanel.Name = "_layerButtonPanel";
			this._layerButtonPanel.Size = new System.Drawing.Size(30, 25);
			this._layerButtonPanel.TabIndex = 0;
			this._layerButtonPanel.WrapContents = false;
			// 
			// _tileMapDisplay
			// 
			this._tileMapDisplay.AutoScroll = true;
			this._tileMapDisplay.FitImage = false;
			this._tileMapDisplay.Location = new System.Drawing.Point(0, 0);
			this._tileMapDisplay.Name = "_tileMapDisplay";
			this._tileMapDisplay.ShowScrollOverlay = false;
			this._tileMapDisplay.Size = new System.Drawing.Size(317, 320);
			this._tileMapDisplay.TabIndex = 0;
			// 
			// horizontalLine1
			// 
			this.horizontalLine1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.horizontalLine1.LineColor = System.Drawing.SystemColors.ButtonShadow;
			this.horizontalLine1.Location = new System.Drawing.Point(0, 597);
			this.horizontalLine1.Name = "horizontalLine1";
			this.horizontalLine1.Size = new System.Drawing.Size(378, 1);
			this.horizontalLine1.TabIndex = 2;
			this.horizontalLine1.Text = "horizontalLine1";
			// 
			// TileMapViewer
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.Controls.Add(this.horizontalLine1);
			this.Controls.Add(this._tileMapDisplay);
			this.Controls.Add(this._controlPanel);
			this.MinimumSize = new System.Drawing.Size(275, 0);
			this.Name = "TileMapViewer";
			this.Size = new System.Drawing.Size(378, 622);
			this._controlPanel.ResumeLayout(false);
			this._controlPanel.PerformLayout();
			this._displayButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private TileMapRender _tileMapDisplay;
		private System.Windows.Forms.Panel _controlPanel;
		private System.Windows.Forms.FlowLayoutPanel _layerButtonPanel;
		private System.Windows.Forms.FlowLayoutPanel _displayButtonPanel;
		private System.Windows.Forms.CheckBox _viewportButton;
		private System.Windows.Forms.CheckBox _scaleButton;
		private StatusView.HorizontalLine horizontalLine1;
	}
}
