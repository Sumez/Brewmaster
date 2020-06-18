using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Brewmaster.EditorWindows.Code;
using Brewmaster.EditorWindows.Text;
using Brewmaster.ProjectModel;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.Settings
{
	public partial class SettingsWindow : Form
	{
		private List<KeyboardMapping> _nesMappings;
		private List<KeyboardMapping> _snesMappings;
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

			Highlighting = new Ca65Highlighting(TargetPlatform.Nes);
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
				featureList.Items.Add(new FeatureSelection(feature));
			}
			featureList.SelectedValueChanged += (s, e) => UpdateKeyboardShortcut();
			featureList.SelectedIndex = 0;

			_nesMappings = settings.NesMappings.Select(m => m.Clone()).ToList();
			_snesMappings = settings.SnesMappings.Select(m => m.Clone()).ToList();

			var updateValue = _updateRates.IndexOf(Settings.UpdateRate);
			if (updateValue >= _updateRate.Minimum && updateValue <= _updateRate.Maximum) _updateRate.Value = updateValue;
			UpdateRateHelp();

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
			var selectedFeature = featureList.SelectedItem as FeatureSelection;
			if (selectedFeature == null) return;
			using (var keyAssignDialog = new ButtonAssignment(true))
			{
				keyAssignDialog.StartPosition = FormStartPosition.CenterParent;
				keyAssignDialog.ShowDialog(this);
				var existing = KeyBindings.FirstOrDefault(b => b.Binding == (int)keyAssignDialog.KeyboardInput && b.Feature != (int)selectedFeature.Feature);
				if (keyAssignDialog.KeyboardInput != Keys.None && existing.Binding == (int)keyAssignDialog.KeyboardInput)
					MessageBox.Show(string.Format("This shortcut is already assigned to '{0}'", FeatureSelection.GetFeatureName((Feature)existing.Feature)), 
						"Keyboard shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					KeyBindings[selectedFeature.Feature] = keyAssignDialog.KeyboardInput;
			}
			UpdateKeyboardShortcut();
		}

		private void UpdateKeyboardShortcut()
		{
			var selectedFeature = featureList.SelectedItem as FeatureSelection;
			if (selectedFeature == null) return;
			var keys = KeyBindings[selectedFeature.Feature];

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

			Settings.NesMappings = _nesMappings.Select(m => m.Clone()).ToList();
			Settings.SnesMappings = _snesMappings.Select(m => m.Clone()).ToList();
			Settings.EmuBackgroundColor = emulatorBackground.BackColor;
			Settings.UpdateRate = _updateRates[_updateRate.Value];

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

		private void _nesControllerButton_Click(object sender, EventArgs e)
		{
			using (var keyBindings = new KeyBindingWindow())
			{
				keyBindings.StartPosition = FormStartPosition.CenterParent;
				keyBindings.KeyBindingSettings.SetMappings(_nesMappings.Select(m => m.Clone()).ToList());
				if (keyBindings.ShowDialog(this) == DialogResult.OK)
					_nesMappings = keyBindings.KeyBindingSettings.Mappings.Select(m => m.Clone()).ToList();
			}
		}

		private void _snesControllerButton_Click(object sender, EventArgs e)
		{
			using (var keyBindings = new KeyBindingWindow())
			{
				keyBindings.StartPosition = FormStartPosition.CenterParent;
				keyBindings.KeyBindingSettings.SetMappings(_snesMappings.Select(m => m.Clone()).ToList());
				if (keyBindings.ShowDialog(this) == DialogResult.OK)
					_snesMappings = keyBindings.KeyBindingSettings.Mappings.Select(m => m.Clone()).ToList();
			}
		}

		private readonly List<int> _updateRates = new List<int>
		{
			0, 120, 90, 60, 45, 30, 15, 10, 5, 2, 1
		};
		private void _updateRate_Scroll(object sender, EventArgs e)
		{
			UpdateRateHelp();
		}

		private void UpdateRateHelp()
		{
			var refreshRate = _updateRates[_updateRate.Value];
			switch (refreshRate)
			{
				case 0:
					_updateRateHelp.Text = @"Only updates on break";
					break;
				case 1:
					_updateRateHelp.Text = @"Updates every frame";
					break;
				default:
					_updateRateHelp.Text = string.Format(@"Updates every {0} frames", refreshRate);
					break;
			}
		}
	}

	public class FeatureSelection
	{
		public Feature Feature { get; private set; }
		public FeatureSelection(Feature feature)
		{
			Feature = feature;
		}

		public override string ToString()
		{
			return GetFeatureName(Feature);
		}

		public static string GetFeatureName(Feature feature)
		{
			return Regex.Replace(feature.ToString(), "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
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
