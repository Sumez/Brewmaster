using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrewMaster.ProjectModel;

namespace BrewMaster.ProjectWizard
{
	public partial class ConfigurationSettings : UserControl
	{
		public AsmProject Project { get; set; }

		public ConfigurationSettings()
		{
			InitializeComponent();
		}

		public ConfigurationSettings(NesCartridge configuration, AsmProject project)
		{
			InitializeComponent();

			Project = project;
			ConfigurationName.Text = configuration.Name;
			OutputFile.Text = configuration.Filename;
			GenerateMapFile.Checked = configuration.MapFile != null;
			ConfigurationFile.Text = configuration.LinkerConfigFile;
			Symbols.Text = string.Join(Environment.NewLine, configuration.Symbols);
		}

		public bool SetConfigurationSettings(NesCartridge configuration)
		{
			if (!IsValid()) return false;

			var outputFile = OutputFile.Text.Replace('\\', '/');
			var buildPath = Path.GetDirectoryName(outputFile);
			if (string.IsNullOrEmpty(buildPath)) buildPath = "";
			else buildPath = buildPath.Replace('\\', '/') + "/";

			configuration.Name = ConfigurationName.Text;
			configuration.Filename = outputFile;
			configuration.DebugFile = buildPath + Path.GetFileNameWithoutExtension(outputFile) + ".dbg";
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

			return true;
		}

		private IEnumerable<string> GetSymbols()
		{
			return Symbols.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(s => Regex.Replace(s.Trim(), @"\s+=\s+", "="));
		}

		public bool IsValid()
		{
			if (string.IsNullOrWhiteSpace(OutputFile.Text))
			{
				MessageBox.Show("Invalid filename");
				return false;
			}
			foreach (var character in NewProjectPath.InvalidPathChars)
				if (OutputFile.Text.Contains(character))
				{
					MessageBox.Show(string.Format("Invalid filename: '{0}'", OutputFile.Text));
					return false;
				}

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
	}
}
