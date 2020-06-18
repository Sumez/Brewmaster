using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class ConfigurationSettings : UserControl
	{
		private AsmProject _project;

		public AsmProject Project
		{
			get { return _project; }
			set
			{
				_project = value;
				if (_project == null) return;
				CalculateSnesChecksum.Visible = ChecksumLabel.Visible = _project.Platform == TargetPlatform.Snes;
			}
		}

		public ConfigurationSettings()
		{
			InitializeComponent();
			UseCustom.CheckedChanged += UpdateCustomSelection;
		}

		private void UpdateCustomSelection(object sender, EventArgs e)
		{
			if (UseCustom.Checked)
			{
				_customPanel.Top = _integratedPanel.Top;
				_customPanel.Visible = true;
				_integratedPanel.Visible = false;
			}
			else
			{
				_customPanel.Visible = false;
				_integratedPanel.Visible = true;
			}
		}

		public ConfigurationSettings(NesCartridge configuration, AsmProject project) : this()
		{
			Project = project;
			ConfigurationName.Text = configuration.Name;
			OutputFile.Text = configuration.Filename;
			GenerateMapFile.Checked = configuration.MapFile != null;
			ConfigurationFile.Text = configuration.LinkerConfigFile;
			CalculateSnesChecksum.Checked = configuration.CalculateChecksum;
			UseCustom.Checked = configuration.Custom;
			UseIntegrated.Checked = !configuration.Custom;
			Symbols.Text = string.Join(Environment.NewLine, configuration.Symbols);

			if (configuration.ScriptCommands != null) CustomScript.Text = string.Join(Environment.NewLine, configuration.ScriptCommands);
			CustomOutputFile.Text = configuration.Filename;
			CustomDebugFile.Text = configuration.DebugFile;
		}

		public bool SetConfigurationSettings(NesCartridge configuration)
		{
			if (!IsValid()) return false;

			string outputFile;
			string debugFile = null;
			if (UseCustom.Checked)
			{
				outputFile = CustomOutputFile.Text.Replace('\\', '/');
				debugFile = CustomDebugFile.Text.Replace('\\', '/');
			}
			else
			{
				outputFile = OutputFile.Text.Replace('\\', '/');
			}
			var buildPath = Path.GetDirectoryName(outputFile);
			if (string.IsNullOrEmpty(buildPath)) buildPath = "";
			else buildPath = buildPath.Replace('\\', '/') + "/";

			configuration.Name = ConfigurationName.Text;
			configuration.Filename = outputFile;
			configuration.DebugFile = debugFile ?? buildPath + Path.GetFileNameWithoutExtension(outputFile) + ".dbg";
			configuration.LinkerConfigFile = ConfigurationFile.Text;
			configuration.BuildPath = buildPath + "obj";
			configuration.MapFile = GenerateMapFile.Checked
				? buildPath + Path.GetFileNameWithoutExtension(outputFile) + ".map.txt"
				: null;
			configuration.Symbols = GetSymbols().ToList();
			configuration.ChrBuildPath = null;
			configuration.ChrFile = null;
			configuration.PrgFile = null;
			configuration.PrgBuildPath = null;

			configuration.ScriptCommands = CustomScript.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

			configuration.CalculateChecksum = CalculateSnesChecksum.Checked;
			configuration.Custom = UseCustom.Checked;

			return true;
		}

		private IEnumerable<string> GetSymbols()
		{
			return Symbols.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Regex.Replace(s.Trim(), @"\s+=\s+", "="));
		}

		public bool IsValid()
		{
			return UseCustom.Checked ? IsCustomValid() : IsIntegratedValid();
		}

		private bool IsCustomValid()
		{
			if (string.IsNullOrWhiteSpace(CustomScript.Text))
			{
				MessageBox.Show("At least one command is necessary for the build process to execute");
				return false;
			}
			if (string.IsNullOrWhiteSpace(CustomOutputFile.Text))
			{
				MessageBox.Show("An output ROM file is required");
				return false;
			}
			if (string.IsNullOrWhiteSpace(CustomDebugFile.Text))
			{
				MessageBox.Show("An output debug file is required");
				return false;
			}

			if (!CheckValidPath(CustomOutputFile)) return false;
			if (!CheckValidPath(CustomDebugFile)) return false;

			return true;
		}

		private bool CheckValidPath(TextBox textBox)
		{
			if (NewProjectPath.InvalidPathChars.Any(character => textBox.Text.Contains(character)))
			{
				MessageBox.Show(string.Format("Invalid filename: '{0}'", textBox.Text));
				return false;
			}
			return true;
		}

		private bool IsIntegratedValid()
		{
			if (string.IsNullOrWhiteSpace(OutputFile.Text))
			{
				MessageBox.Show("Invalid filename");
				return false;
			}

			if (!CheckValidPath(OutputFile)) return false;

			foreach (var symbol in GetSymbols())
			{
				if (!Regex.IsMatch(symbol, @"^[a-zA-Z][^\s]*(=[^\s]+)?$"))
				{
					MessageBox.Show(string.Format("Invalid symbol definition: '{0}'", symbol));
					return false;
				}
			}

			return true;
		}

		private void BrowseConfigFile_Click(object sender, EventArgs e)
		{
			using (var dialog = new OpenFileDialog())
			{
				dialog.CheckFileExists = false;
				dialog.Filter = "Configuration files (*.cfg;*.ini)|*.cfg;*.ini|All files (*.*)|*.*";
				dialog.DefaultExt = ".cfg";
				dialog.InitialDirectory = Project.Directory.FullName;
				if (!string.IsNullOrWhiteSpace(ConfigurationFile.Text))
				{
					try
					{
						var file = new FileInfo(Project.Directory.FullName + @"\" + ConfigurationFile.Text);
						dialog.FileName = file.Name;
						dialog.InitialDirectory = file.DirectoryName.Replace('/', '\\');
					}
					catch
					{
						// Eugh, exception driven logic, but there's not really any other reliable way
					}
				}

				if (dialog.ShowDialog() != DialogResult.OK) return;

				ConfigurationFile.Text = Project.GetRelativePath(dialog.FileName);
			}
		}

		public void SetDefaults()
		{
			ConfigurationName.Text = Project.Platform == TargetPlatform.Snes ? "SNES" : "NES";
			UseIntegrated.Checked = true;
			var defaultConfigFile = Project.Files.FirstOrDefault(f => f.Mode == CompileMode.LinkerConfig);
			if (defaultConfigFile != null) ConfigurationFile.Text = defaultConfigFile.GetRelativePath();
			OutputFile.Text = Project.Platform == TargetPlatform.Snes ? "bin/game.sfc" : "bin/game.nes";
		}
	}
}
	