using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.EditorWindows;
using Brewmaster.ProjectModel;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.Settings
{
	public partial class SettingsWindow : Form
	{
		public Settings Settings { get; private set; }
		public Ca65Highlighting Highlighting { get; private set; }
		public KeyBindings KeyBindings { get; private set; }

		public int SelectedTab
		{
			get { return tabControl.SelectedIndex; }
			set { tabControl.SelectTab(value); }
		}

		public SettingsWindow(Settings settings)
		{
			Settings = settings;
			InitializeComponent();

			Highlighting = new Ca65Highlighting(ProjectType.Nes);
			editorPreview.IsReadOnly = true;
			editorPreview.Text = @".include ""nes.inc""

testRoutine:
	lda #$C6		; Initial position of our object
	sta PositionX
rts
";
			editorPreview.Document.HighlightingStrategy = Highlighting;

			foreach (var font in FontFamily.Families)
			{
				fontSelector.Items.Add(font.Name);
			}

			for (var size = 6; size <= 24; size++)
			{
				fontSizeSelector.Items.Add(size.ToString());
			}

			foreach (var colors in Highlighting.Colors)
			{
				styleList.Items.Add(colors.Key);
			}
			
			styleList.SelectedValueChanged += (s, e) => UpdateStyleInfo();
			styleList.SelectedIndex = 0;
			editorPreview.Text = editorPreview.Text;

			textColorButton.Click += (s, e) => SelectColor(textColor, true);
			textColor.Click += (s, e) => SelectColor(textColor, true);
			backgroundColorButton.Click += (s, e) => SelectColor(backgroundColor, true);
			backgroundColor.Click += (s, e) =>
			{
				if (!transparentCheckBox.Checked) SelectColor(backgroundColor, true);
			};
			transparentCheckBox.CheckedChanged += (s, e) =>
			{
				EditCurrentStyle();
				backgroundColorButton.Enabled = !transparentCheckBox.Checked;
			};
			boldCheckBox.CheckedChanged += (s, e) => EditCurrentStyle();
			italicCheckBox.CheckedChanged += (s, e) => EditCurrentStyle();
			fontSelector.SelectedIndexChanged += (s, e) => EditSelectedFont();
			fontSizeSelector.SelectedIndexChanged += (s, e) => EditSelectedFont();



			KeyBindings = new KeyBindings();
			foreach (var binding in Program.Keys)
			{
				KeyBindings.Add(binding);
			}

			foreach (var feature in (Feature[])Enum.GetValues(typeof(Feature)))
			{
				featureList.Items.Add(feature.ToString());
			}
			featureList.SelectedValueChanged += (s, e) => UpdateKeyboardShortcut();
			featureList.SelectedIndex = 0;

			nesKeyBindings.SetMappings(settings.NesMappings.Select(m => m.Clone()).ToList());
			snesKeyBindings.SetMappings(settings.SnesMappings.Select(m => m.Clone()).ToList());

			emulatorBackground.BackColor = settings.EmuBackgroundColor;
			emulatorBackground.Click += (s, e) => SelectColor(emulatorBackground);
			emulatorBackgroundButton.Click += (s, e) => SelectColor(emulatorBackground);
		}

		private void SelectColor(Panel target, bool isStyleColor = false)
		{
			using (var colorDialog = new ColorDialog {Color = target.BackColor})
			{
				colorDialog.ShowDialog(this);
				target.BackColor = colorDialog.Color;
			}
			if (isStyleColor) EditCurrentStyle();
		}
		
		private void EditCurrentStyle()
		{
			Highlighting.SetColor(styleList.SelectedItem.ToString(), 
				transparentCheckBox.Checked
					? new HighlightColor(textColor.BackColor, boldCheckBox.Checked, italicCheckBox.Checked)
					: new HighlightColor(textColor.BackColor, backgroundColor.BackColor, boldCheckBox.Checked, italicCheckBox.Checked));
			editorPreview.Text = editorPreview.Text;

		}
		private void EditSelectedFont()
		{
			var oldFont = editorPreview.Font;
			editorPreview.Font =
				new Font(fontSelector.SelectedItem.ToString(),
					int.Parse(fontSizeSelector.SelectedItem.ToString()), 
					oldFont.Style, oldFont.Unit);
			editorPreview.Invalidate();
		}
		private void UpdateStyleInfo()
		{
			var style = Highlighting.Colors[styleList.SelectedItem.ToString()];
			textColor.BackColor = style.Color;
			backgroundColor.BackColor = style.HasBackground ? style.BackgroundColor : Highlighting.DefaultTextColor.BackgroundColor;
			boldCheckBox.Checked = style.Bold;
			italicCheckBox.Checked = style.Italic;
			backgroundColorButton.Enabled = !(transparentCheckBox.Checked = !style.HasBackground);

			fontSelector.SelectedItem = editorPreview.Font.Name;
			fontSizeSelector.SelectedItem = editorPreview.Font.Size.ToString();
			transparentCheckBox.Enabled = !(fontSizeSelector.Enabled = fontSelector.Enabled = styleList.SelectedItem.ToString() == "Default");
		}

		private void reassignKeyButton_Click(object sender, EventArgs e)
		{
			if (!Enum.TryParse(featureList.SelectedItem.ToString(), out Feature feature)) return;
			using (var keyAssignDialog = new ButtonAssignment(true))
			{
				keyAssignDialog.StartPosition = FormStartPosition.CenterParent;
				keyAssignDialog.ShowDialog(this);
				KeyBindings[feature] = keyAssignDialog.KeyboardInput;
			}
			UpdateKeyboardShortcut();
		}

		private void UpdateKeyboardShortcut()
		{
			if (!Enum.TryParse(featureList.SelectedItem.ToString(), out Feature feature)) return;
			var keys = KeyBindings[feature];

			if (keys == Keys.None)
			{
				shortcutText.Text = "";
			}
			else
			{
				/*var keyString = new StringBuilder();
				if (keys.HasFlag(Keys.Control)) keyString.Append("Ctrl+");
				if (keys.HasFlag(Keys.Alt)) keyString.Append("Alt+");
				if (keys.HasFlag(Keys.Shift)) keyString.Append("Shift+");
				keyString.Append((keys & Keys.KeyCode).ToString());
				shortcutText.Text = keyString.ToString();*/

				shortcutText.Text = ButtonAssignment.GetString(keys);

			}

		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Program.UpdateKeyBindings(KeyBindings);
			Settings.DefaultFont = TextEditor.DefaultCodeProperties.Font = editorPreview.Font;
			Settings.AsmHighlighting = new HighlightingColors {Data = Ca65Highlighting.DefaultColors = Highlighting.Colors};

			Settings.NesMappings = nesKeyBindings.Mappings.Select(m => m.Clone()).ToList();
			Settings.SnesMappings = snesKeyBindings.Mappings.Select(m => m.Clone()).ToList();
			Settings.EmuBackgroundColor = emulatorBackground.BackColor;

			Settings.Save();
			DialogResult = DialogResult.OK;
			Close();
		}

		private void defaultKeysButton_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to return to the default?\nYou will lose any custom settings",
				"Reset to defaults", MessageBoxButtons.OKCancel);
			if (result != DialogResult.OK) return;

			KeyBindings = KeyBindings.Defaults;
			UpdateKeyboardShortcut();
		}
	}

	public class DummyCa65Editor : TextEditor
	{
		public DummyCa65Editor()
		{
			TextEditorProperties = new DefaultBrewmasterCodeProperties
			{
				Font = DefaultCodeProperties.Font
			};
		}
	}
}
