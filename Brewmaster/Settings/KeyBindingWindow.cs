using System.Windows.Forms;

namespace Brewmaster.Settings
{
	public class KeyBindingWindow : Form
	{
		private Button _cancelButton;
		private Button _okButton;
		public KeyBindingSettings KeyBindingSettings;

		public KeyBindingWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.KeyBindingSettings = new Brewmaster.Settings.KeyBindingSettings();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(163, 365);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(82, 365);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// KeyBindingSettings
			// 
			this.KeyBindingSettings.Location = new System.Drawing.Point(7, 9);
			this.KeyBindingSettings.Name = "KeyBindingSettings";
			this.KeyBindingSettings.Size = new System.Drawing.Size(227, 321);
			this.KeyBindingSettings.TabIndex = 0;
			this.KeyBindingSettings.Text = "keyBindingSettings1";
			// 
			// KeyBindingWindow
			// 
			this.AcceptButton = this._okButton;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(250, 400);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this.KeyBindingSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "KeyBindingWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Key bindings";
			this.ResumeLayout(false);

		}

		private void _okButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			Height = KeyBindingSettings.Height + 100;
		}
	}
}