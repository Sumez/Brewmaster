namespace BrewMaster.Settings
{
	partial class SettingsWindow
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

		#region Windows Form Designer generated code

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
			System.Windows.Forms.Panel panel1;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.TabPage styleTab;
			System.Windows.Forms.TabPage keyBindingTab;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.TabPage emulatorTab;
			System.Windows.Forms.Label label9;
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.fontSizeSelector = new System.Windows.Forms.ComboBox();
			this.backgroundColorButton = new System.Windows.Forms.Button();
			this.textColorButton = new System.Windows.Forms.Button();
			this.fontSelector = new System.Windows.Forms.ComboBox();
			this.transparentCheckBox = new System.Windows.Forms.CheckBox();
			this.italicCheckBox = new System.Windows.Forms.CheckBox();
			this.boldCheckBox = new System.Windows.Forms.CheckBox();
			this.backgroundColor = new System.Windows.Forms.Panel();
			this.textColor = new System.Windows.Forms.Panel();
			this.styleList = new System.Windows.Forms.ListBox();
			this.editorPreview = new BrewMaster.Settings.DummyCa65Editor();
			this.defaultKeysButton = new System.Windows.Forms.Button();
			this.reassignKeyButton = new System.Windows.Forms.Button();
			this.shortcutText = new System.Windows.Forms.TextBox();
			this.featureList = new System.Windows.Forms.ListBox();
			this.snesKeyBindings = new BrewMaster.Settings.KeyBindingSettings();
			this.nesKeyBindings = new BrewMaster.Settings.KeyBindingSettings();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.assemblerTab = new System.Windows.Forms.TabPage();
			this.emulatorBackground = new System.Windows.Forms.Panel();
			this.emulatorBackgroundButton = new System.Windows.Forms.Button();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			panel1 = new System.Windows.Forms.Panel();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			styleTab = new System.Windows.Forms.TabPage();
			keyBindingTab = new System.Windows.Forms.TabPage();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			emulatorTab = new System.Windows.Forms.TabPage();
			label9 = new System.Windows.Forms.Label();
			panel1.SuspendLayout();
			styleTab.SuspendLayout();
			keyBindingTab.SuspendLayout();
			emulatorTab.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.assemblerTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(216, 68);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(57, 13);
			label1.TabIndex = 8;
			label1.Text = "Text color:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(216, 6);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(31, 13);
			label2.TabIndex = 10;
			label2.Text = "Font:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(216, 110);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(94, 13);
			label3.TabIndex = 11;
			label3.Text = "Background color:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(445, 6);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(30, 13);
			label4.TabIndex = 14;
			label4.Text = "Size:";
			// 
			// panel1
			// 
			panel1.Controls.Add(this.okButton);
			panel1.Controls.Add(this.cancelButton);
			panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			panel1.Location = new System.Drawing.Point(0, 353);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(602, 38);
			panel1.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(436, 8);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(517, 8);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(3, 3);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(51, 13);
			label5.TabIndex = 1;
			label5.Text = "Features:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(157, 3);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(50, 13);
			label6.TabIndex = 4;
			label6.Text = "Shortcut:";
			// 
			// styleTab
			// 
			styleTab.BackColor = System.Drawing.SystemColors.Control;
			styleTab.Controls.Add(this.fontSizeSelector);
			styleTab.Controls.Add(label4);
			styleTab.Controls.Add(this.backgroundColorButton);
			styleTab.Controls.Add(this.textColorButton);
			styleTab.Controls.Add(label3);
			styleTab.Controls.Add(label2);
			styleTab.Controls.Add(this.fontSelector);
			styleTab.Controls.Add(label1);
			styleTab.Controls.Add(this.transparentCheckBox);
			styleTab.Controls.Add(this.italicCheckBox);
			styleTab.Controls.Add(this.boldCheckBox);
			styleTab.Controls.Add(this.backgroundColor);
			styleTab.Controls.Add(this.textColor);
			styleTab.Controls.Add(this.styleList);
			styleTab.Controls.Add(this.editorPreview);
			styleTab.Location = new System.Drawing.Point(4, 26);
			styleTab.Name = "styleTab";
			styleTab.Padding = new System.Windows.Forms.Padding(3);
			styleTab.Size = new System.Drawing.Size(594, 361);
			styleTab.TabIndex = 0;
			styleTab.Text = "Fonts and Colors";
			// 
			// fontSizeSelector
			// 
			this.fontSizeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fontSizeSelector.FormattingEnabled = true;
			this.fontSizeSelector.Location = new System.Drawing.Point(448, 22);
			this.fontSizeSelector.Name = "fontSizeSelector";
			this.fontSizeSelector.Size = new System.Drawing.Size(52, 21);
			this.fontSizeSelector.TabIndex = 15;
			// 
			// backgroundColorButton
			// 
			this.backgroundColorButton.Location = new System.Drawing.Point(275, 126);
			this.backgroundColorButton.Name = "backgroundColorButton";
			this.backgroundColorButton.Size = new System.Drawing.Size(75, 25);
			this.backgroundColorButton.TabIndex = 13;
			this.backgroundColorButton.Text = "Change...";
			this.backgroundColorButton.UseVisualStyleBackColor = true;
			// 
			// textColorButton
			// 
			this.textColorButton.Location = new System.Drawing.Point(275, 84);
			this.textColorButton.Name = "textColorButton";
			this.textColorButton.Size = new System.Drawing.Size(75, 25);
			this.textColorButton.TabIndex = 12;
			this.textColorButton.Text = "Change...";
			this.textColorButton.UseVisualStyleBackColor = true;
			// 
			// fontSelector
			// 
			this.fontSelector.BackColor = System.Drawing.Color.White;
			this.fontSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fontSelector.FormattingEnabled = true;
			this.fontSelector.Location = new System.Drawing.Point(219, 22);
			this.fontSelector.Name = "fontSelector";
			this.fontSelector.Size = new System.Drawing.Size(223, 21);
			this.fontSelector.TabIndex = 9;
			// 
			// transparentCheckBox
			// 
			this.transparentCheckBox.AutoSize = true;
			this.transparentCheckBox.Location = new System.Drawing.Point(359, 130);
			this.transparentCheckBox.Name = "transparentCheckBox";
			this.transparentCheckBox.Size = new System.Drawing.Size(83, 17);
			this.transparentCheckBox.TabIndex = 7;
			this.transparentCheckBox.Text = "Transparent";
			this.transparentCheckBox.UseVisualStyleBackColor = true;
			// 
			// italicCheckBox
			// 
			this.italicCheckBox.AutoSize = true;
			this.italicCheckBox.Location = new System.Drawing.Point(272, 48);
			this.italicCheckBox.Name = "italicCheckBox";
			this.italicCheckBox.Size = new System.Drawing.Size(48, 17);
			this.italicCheckBox.TabIndex = 6;
			this.italicCheckBox.Text = "Italic";
			this.italicCheckBox.UseVisualStyleBackColor = true;
			// 
			// boldCheckBox
			// 
			this.boldCheckBox.AutoSize = true;
			this.boldCheckBox.Location = new System.Drawing.Point(219, 48);
			this.boldCheckBox.Name = "boldCheckBox";
			this.boldCheckBox.Size = new System.Drawing.Size(47, 17);
			this.boldCheckBox.TabIndex = 5;
			this.boldCheckBox.Text = "Bold";
			this.boldCheckBox.UseVisualStyleBackColor = true;
			// 
			// backgroundColor
			// 
			this.backgroundColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.backgroundColor.Location = new System.Drawing.Point(219, 126);
			this.backgroundColor.Name = "backgroundColor";
			this.backgroundColor.Size = new System.Drawing.Size(50, 25);
			this.backgroundColor.TabIndex = 4;
			// 
			// textColor
			// 
			this.textColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textColor.Location = new System.Drawing.Point(219, 84);
			this.textColor.Name = "textColor";
			this.textColor.Size = new System.Drawing.Size(50, 25);
			this.textColor.TabIndex = 3;
			// 
			// styleList
			// 
			this.styleList.FormattingEnabled = true;
			this.styleList.Location = new System.Drawing.Point(8, 6);
			this.styleList.Name = "styleList";
			this.styleList.Size = new System.Drawing.Size(202, 160);
			this.styleList.TabIndex = 2;
			// 
			// editorPreview
			// 
			this.editorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.editorPreview.EnableFolding = false;
			this.editorPreview.IsIconBarVisible = true;
			this.editorPreview.IsReadOnly = true;
			this.editorPreview.Location = new System.Drawing.Point(8, 172);
			this.editorPreview.Name = "editorPreview";
			this.editorPreview.Size = new System.Drawing.Size(578, 148);
			this.editorPreview.TabIndex = 0;
			this.editorPreview.VRulerRow = 0;
			// 
			// keyBindingTab
			// 
			keyBindingTab.BackColor = System.Drawing.SystemColors.Control;
			keyBindingTab.Controls.Add(this.defaultKeysButton);
			keyBindingTab.Controls.Add(label6);
			keyBindingTab.Controls.Add(this.reassignKeyButton);
			keyBindingTab.Controls.Add(this.shortcutText);
			keyBindingTab.Controls.Add(label5);
			keyBindingTab.Controls.Add(this.featureList);
			keyBindingTab.Location = new System.Drawing.Point(4, 26);
			keyBindingTab.Name = "keyBindingTab";
			keyBindingTab.Padding = new System.Windows.Forms.Padding(3);
			keyBindingTab.Size = new System.Drawing.Size(594, 361);
			keyBindingTab.TabIndex = 1;
			keyBindingTab.Text = "Keyboard Shortcuts";
			// 
			// defaultKeysButton
			// 
			this.defaultKeysButton.Location = new System.Drawing.Point(161, 298);
			this.defaultKeysButton.Name = "defaultKeysButton";
			this.defaultKeysButton.Size = new System.Drawing.Size(148, 23);
			this.defaultKeysButton.TabIndex = 5;
			this.defaultKeysButton.Text = "Reset to defaults";
			this.defaultKeysButton.UseVisualStyleBackColor = true;
			this.defaultKeysButton.Click += new System.EventHandler(this.defaultKeysButton_Click);
			// 
			// reassignKeyButton
			// 
			this.reassignKeyButton.Location = new System.Drawing.Point(160, 45);
			this.reassignKeyButton.Name = "reassignKeyButton";
			this.reassignKeyButton.Size = new System.Drawing.Size(79, 23);
			this.reassignKeyButton.TabIndex = 3;
			this.reassignKeyButton.Text = "Reassign";
			this.reassignKeyButton.UseVisualStyleBackColor = true;
			this.reassignKeyButton.Click += new System.EventHandler(this.reassignKeyButton_Click);
			// 
			// shortcutText
			// 
			this.shortcutText.Location = new System.Drawing.Point(160, 19);
			this.shortcutText.Name = "shortcutText";
			this.shortcutText.ReadOnly = true;
			this.shortcutText.ShortcutsEnabled = false;
			this.shortcutText.Size = new System.Drawing.Size(79, 20);
			this.shortcutText.TabIndex = 2;
			this.shortcutText.TabStop = false;
			// 
			// featureList
			// 
			this.featureList.FormattingEnabled = true;
			this.featureList.Location = new System.Drawing.Point(6, 19);
			this.featureList.Name = "featureList";
			this.featureList.Size = new System.Drawing.Size(148, 303);
			this.featureList.TabIndex = 0;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(6, 23);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(60, 13);
			label7.TabIndex = 0;
			label7.Text = "CA65 path:";
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(6, 66);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(60, 13);
			label8.TabIndex = 1;
			label8.Text = "LD65 path:";
			// 
			// emulatorTab
			// 
			emulatorTab.BackColor = System.Drawing.SystemColors.Control;
			emulatorTab.Controls.Add(this.emulatorBackgroundButton);
			emulatorTab.Controls.Add(label9);
			emulatorTab.Controls.Add(this.emulatorBackground);
			emulatorTab.Controls.Add(this.snesKeyBindings);
			emulatorTab.Controls.Add(this.nesKeyBindings);
			emulatorTab.Location = new System.Drawing.Point(4, 26);
			emulatorTab.Name = "emulatorTab";
			emulatorTab.Padding = new System.Windows.Forms.Padding(3);
			emulatorTab.Size = new System.Drawing.Size(594, 361);
			emulatorTab.TabIndex = 3;
			emulatorTab.Text = "Emulators";
			// 
			// snesKeyBindings
			// 
			this.snesKeyBindings.Location = new System.Drawing.Point(419, 6);
			this.snesKeyBindings.Name = "snesKeyBindings";
			this.snesKeyBindings.Size = new System.Drawing.Size(167, 306);
			this.snesKeyBindings.TabIndex = 1;
			this.snesKeyBindings.Text = "keyBindingSettings2";
			// 
			// nesKeyBindings
			// 
			this.nesKeyBindings.Location = new System.Drawing.Point(251, 6);
			this.nesKeyBindings.Name = "nesKeyBindings";
			this.nesKeyBindings.Size = new System.Drawing.Size(162, 306);
			this.nesKeyBindings.TabIndex = 0;
			this.nesKeyBindings.Text = "keyBindingSettings1";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(styleTab);
			this.tabControl.Controls.Add(keyBindingTab);
			this.tabControl.Controls.Add(this.assemblerTab);
			this.tabControl.Controls.Add(emulatorTab);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = new System.Drawing.Point(10, 5);
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(602, 391);
			this.tabControl.TabIndex = 0;
			// 
			// assemblerTab
			// 
			this.assemblerTab.BackColor = System.Drawing.SystemColors.Control;
			this.assemblerTab.Controls.Add(label8);
			this.assemblerTab.Controls.Add(label7);
			this.assemblerTab.Location = new System.Drawing.Point(4, 26);
			this.assemblerTab.Name = "assemblerTab";
			this.assemblerTab.Padding = new System.Windows.Forms.Padding(3);
			this.assemblerTab.Size = new System.Drawing.Size(594, 361);
			this.assemblerTab.TabIndex = 2;
			this.assemblerTab.Text = "Assembler";
			// 
			// emulatorBackground
			// 
			this.emulatorBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.emulatorBackground.Location = new System.Drawing.Point(9, 23);
			this.emulatorBackground.Name = "emulatorBackground";
			this.emulatorBackground.Size = new System.Drawing.Size(50, 25);
			this.emulatorBackground.TabIndex = 4;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(6, 7);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(150, 13);
			label9.TabIndex = 5;
			label9.Text = "Emulator window background:";
			// 
			// emulatorBackgroundButton
			// 
			this.emulatorBackgroundButton.Location = new System.Drawing.Point(65, 23);
			this.emulatorBackgroundButton.Name = "emulatorBackgroundButton";
			this.emulatorBackgroundButton.Size = new System.Drawing.Size(75, 25);
			this.emulatorBackgroundButton.TabIndex = 13;
			this.emulatorBackgroundButton.Text = "Change...";
			this.emulatorBackgroundButton.UseVisualStyleBackColor = true;
			// 
			// SettingsWindow
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(602, 391);
			this.Controls.Add(panel1);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(618, 430);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(618, 430);
			this.Name = "SettingsWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Settings";
			panel1.ResumeLayout(false);
			styleTab.ResumeLayout(false);
			styleTab.PerformLayout();
			keyBindingTab.ResumeLayout(false);
			keyBindingTab.PerformLayout();
			emulatorTab.ResumeLayout(false);
			emulatorTab.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.assemblerTab.ResumeLayout(false);
			this.assemblerTab.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private DummyCa65Editor editorPreview;
		private System.Windows.Forms.ListBox styleList;
		private System.Windows.Forms.CheckBox italicCheckBox;
		private System.Windows.Forms.CheckBox boldCheckBox;
		private System.Windows.Forms.Panel backgroundColor;
		private System.Windows.Forms.Panel textColor;
		private System.Windows.Forms.CheckBox transparentCheckBox;
		private System.Windows.Forms.Button backgroundColorButton;
		private System.Windows.Forms.Button textColorButton;
		private System.Windows.Forms.ComboBox fontSelector;
		private System.Windows.Forms.ComboBox fontSizeSelector;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ListBox featureList;
		private System.Windows.Forms.Button reassignKeyButton;
		private System.Windows.Forms.TextBox shortcutText;
		private System.Windows.Forms.Button defaultKeysButton;
		private System.Windows.Forms.TabPage assemblerTab;
		private KeyBindingSettings snesKeyBindings;
		private KeyBindingSettings nesKeyBindings;
		private System.Windows.Forms.Panel emulatorBackground;
		private System.Windows.Forms.Button emulatorBackgroundButton;
	}
}
