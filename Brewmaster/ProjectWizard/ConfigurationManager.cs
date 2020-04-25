using System;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class ConfigurationManager : Form
	{
		public ConfigurationManager(AsmProject project)
		{
			InitializeComponent();

			ConfigurationSettings.Project = project;
			ConfigurationSettings.ConfigurationName.Text = project.Type == ProjectType.Snes ? "SNES" : "NES";
			var defaultConfigFile = project.Files.FirstOrDefault(f => f.Mode == CompileMode.LinkerConfig);
			if (defaultConfigFile != null)
				ConfigurationSettings.ConfigurationFile.Text = defaultConfigFile.GetRelativePath();

			ConfigurationSettings.OutputFile.Text = project.Type == ProjectType.Snes ? "bin/game.sfc" : "bin/game.nes";
		}

		public NesCartridge Configuration { get; set; }

		private void okButton_Click(object sender, EventArgs e)
		{
			var configurationName = ConfigurationSettings.ConfigurationName.Text;
			if (ConfigurationSettings.Project.BuildConfigurations.Any(c => c.Name == configurationName))
			{
				MessageBox.Show(string.Format("A build configuration with the name '{0}' already exists.", configurationName));
				return;
			}

			Configuration = new NesCartridge();
			if (ConfigurationSettings.SetConfigurationSettings(Configuration))
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}

	}
}
