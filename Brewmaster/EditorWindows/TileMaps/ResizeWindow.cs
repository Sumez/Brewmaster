using System;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ResizeWindow : Form
	{
		private NumericUpDown _heightInput;
		private Button _nwButton;
		private Button _neButton;
		private Button _swButton;
		private Button _seButton;
		private NumericUpDown _widthInput;
		private Button _cancelButton;
		private Button _okButton;

		public int SetHeight
		{
			get { return (int)_heightInput.Value; }
			private set { _heightInput.Value = value; }
		}
		public int SetWidth
		{
			get { return (int)_widthInput.Value; }
			private set { _widthInput.Value = value; }
		}
		public Anchor ResizeAnchor { get; private set; }
		 
		public ResizeWindow(int width, int height, Func<int, int, Anchor, bool> validate = null)
		{
			InitializeComponent();

			SetWidth = width;
			SetHeight = height;

			var alignmentImages = new ImageList();
			alignmentImages.Images.AddRange(new []
			{
				Resources.alignment_center,
				Resources.arrow_right,
				Resources.arrow_left,
				Resources.arrow_down,
				Resources.arrow_up
			});
			_nwButton.ImageList = _neButton.ImageList = _swButton.ImageList = _seButton.ImageList = alignmentImages;
			_nwButton.Click += (s, a) => { SetAnchor(TileMaps.Anchor.Nw); };
			_neButton.Click += (s, a) => { SetAnchor(TileMaps.Anchor.Ne); };
			_swButton.Click += (s, a) => { SetAnchor(TileMaps.Anchor.Sw); };
			_seButton.Click += (s, a) => { SetAnchor(TileMaps.Anchor.Se); };

			_widthInput.ValueChanged += (s, a) => DisplayAnchor();
			_heightInput.ValueChanged += (s, a) => DisplayAnchor();

			SetAnchor(TileMaps.Anchor.Nw);

			_okButton.Click += (s, a) =>
			{
				if (validate != null && !validate(SetWidth, SetHeight, ResizeAnchor)) return;
				DialogResult = DialogResult.OK;
				Close();
			};
		}

		private void SetAnchor(Anchor anchor)
		{
			ResizeAnchor = anchor;
			DisplayAnchor();
		}

		private void DisplayAnchor()
		{
			_nwButton.ImageIndex = ResizeAnchor == TileMaps.Anchor.Nw ? 0
				: ResizeAnchor == TileMaps.Anchor.Ne ? 2
				: ResizeAnchor == TileMaps.Anchor.Sw ? 4
				: -1;
			_neButton.ImageIndex = ResizeAnchor == TileMaps.Anchor.Ne ? 0
				: ResizeAnchor == TileMaps.Anchor.Nw ? 1
				: ResizeAnchor == TileMaps.Anchor.Se ? 4
				: -1;
			_swButton.ImageIndex = ResizeAnchor == TileMaps.Anchor.Sw ? 0
				: ResizeAnchor == TileMaps.Anchor.Se ? 2
				: ResizeAnchor == TileMaps.Anchor.Nw ? 3
				: -1;
			_seButton.ImageIndex = ResizeAnchor == TileMaps.Anchor.Se ? 0
				: ResizeAnchor == TileMaps.Anchor.Sw ? 1
				: ResizeAnchor == TileMaps.Anchor.Ne ? 3
				: -1;

		}

		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			this._widthInput = new System.Windows.Forms.NumericUpDown();
			this._heightInput = new System.Windows.Forms.NumericUpDown();
			this._nwButton = new System.Windows.Forms.Button();
			this._neButton = new System.Windows.Forms.Button();
			this._swButton = new System.Windows.Forms.Button();
			this._seButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._widthInput)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._heightInput)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(12, 14);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 2;
			label1.Text = "Width:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 40);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 13);
			label2.TabIndex = 4;
			label2.Text = "Height:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(12, 77);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(44, 13);
			label3.TabIndex = 11;
			label3.Text = "Anchor:";
			// 
			// _widthInput
			// 
			this._widthInput.Location = new System.Drawing.Point(59, 12);
			this._widthInput.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this._widthInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._widthInput.Name = "_widthInput";
			this._widthInput.Size = new System.Drawing.Size(66, 20);
			this._widthInput.TabIndex = 1;
			this._widthInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _heightInput
			// 
			this._heightInput.Location = new System.Drawing.Point(59, 38);
			this._heightInput.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this._heightInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._heightInput.Name = "_heightInput";
			this._heightInput.Size = new System.Drawing.Size(66, 20);
			this._heightInput.TabIndex = 3;
			this._heightInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _nwButton
			// 
			this._nwButton.Location = new System.Drawing.Point(59, 77);
			this._nwButton.Name = "_nwButton";
			this._nwButton.Size = new System.Drawing.Size(32, 32);
			this._nwButton.TabIndex = 5;
			this._nwButton.UseVisualStyleBackColor = true;
			// 
			// _neButton
			// 
			this._neButton.Location = new System.Drawing.Point(93, 77);
			this._neButton.Name = "_neButton";
			this._neButton.Size = new System.Drawing.Size(32, 32);
			this._neButton.TabIndex = 6;
			this._neButton.UseVisualStyleBackColor = true;
			// 
			// _swButton
			// 
			this._swButton.Location = new System.Drawing.Point(59, 111);
			this._swButton.Name = "_swButton";
			this._swButton.Size = new System.Drawing.Size(32, 32);
			this._swButton.TabIndex = 7;
			this._swButton.UseVisualStyleBackColor = true;
			// 
			// _seButton
			// 
			this._seButton.Location = new System.Drawing.Point(93, 111);
			this._seButton.Name = "_seButton";
			this._seButton.Size = new System.Drawing.Size(32, 32);
			this._seButton.TabIndex = 8;
			this._seButton.UseVisualStyleBackColor = true;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(96, 163);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 9;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._okButton.Location = new System.Drawing.Point(15, 163);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 10;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// ResizeWindow
			// 
			this.AcceptButton = this._okButton;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(188, 198);
			this.Controls.Add(label3);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._seButton);
			this.Controls.Add(this._swButton);
			this.Controls.Add(this._neButton);
			this.Controls.Add(this._nwButton);
			this.Controls.Add(label2);
			this.Controls.Add(this._heightInput);
			this.Controls.Add(label1);
			this.Controls.Add(this._widthInput);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResizeWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resize";
			((System.ComponentModel.ISupportInitialize)(this._widthInput)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._heightInput)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}

	public enum Anchor
	{
		N = 1,
		S = 2,
		W = 4,
		E = 8,
		Nw = 5,
		Ne = 9,
		Sw = 6,
		Se = 10
	}
}
