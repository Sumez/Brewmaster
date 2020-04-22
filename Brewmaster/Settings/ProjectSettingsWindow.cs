using System;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;
using Brewmaster.ProjectWizard;

namespace Brewmaster.Settings
{
	public partial class ProjectSettingsWindow : Form
	{
		public AsmProject Project { get; private set; }
		private ConfigurationSelection _currentSelection;

		public ProjectSettingsWindow(AsmProject project)
		{
			Project = project;
			InitializeComponent();

			ProjectName.Text = project.Name;
			foreach (var configuration in project.BuildConfigurations)
			{
				var item = new ConfigurationSelection(configuration);
				ConfigurationSelector.Items.Add(item);
				if (project.CurrentConfiguration == configuration)
				{
					ConfigurationSelector.SelectedItem = item;
					LoadSelection(item);
				}
			}
			ConfigurationSelector.Items.Add(new ConfigurationSelection(null));

			ConfigurationSelector.SelectedIndexChanged += (sender, args) =>
			{
				var selection = ConfigurationSelector.SelectedItem as ConfigurationSelection;
				if (selection == null) return;

				if (selection.Configuration == null)
				{
					using (var newConfigurationWindow = new ConfigurationManager(Project))
					{
						newConfigurationWindow.StartPosition = FormStartPosition.CenterParent;
						var result = newConfigurationWindow.ShowDialog();
						if (result == DialogResult.OK)
						{
							Project.BuildConfigurations.Add(newConfigurationWindow.Configuration);
							Project.Pristine = false;
							selection = new ConfigurationSelection(newConfigurationWindow.Configuration);
							ConfigurationSelector.Items.Insert(ConfigurationSelector.Items.Count - 1, selection);
							ConfigurationSelector.SelectedItem = selection;
						}
						else
						{
							ConfigurationSelector.SelectedItem = _currentSelection;
							return;
						}
					}
				}
				LoadSelection(selection);
			};
		}

		private void LoadSelection(ConfigurationSelection selection)
		{
			_currentSelection = selection;
			if (selection.SettingsControl == null)
			{
				selection.SettingsControl = new ConfigurationSettings(selection.Configuration, Project)
				{
					Location = configurationSettings1.Location,
					Size = configurationSettings1.Size
				};
			}
			ConfigurationBox.Controls.Remove(configurationSettings1);
			ConfigurationBox.Controls.Add(configurationSettings1 = selection.SettingsControl);
			ConfigurationBox.Controls.SetChildIndex(selection.SettingsControl, 0);
		}

		
		private void okButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(ProjectName.Text))
			{
				MessageBox.Show("Invalid project name");
				return;
			}

			foreach (var selection in ConfigurationSelector.Items.OfType<ConfigurationSelection>().Where(s => s.SettingsControl != null))
			{
				if (!selection.SettingsControl.IsValid()) return;
			}
			foreach (var selection in ConfigurationSelector.Items.OfType<ConfigurationSelection>().Where(s => s.SettingsControl != null))
			{
				selection.SettingsControl.SetConfigurationSettings(selection.Configuration);
			}

			Project.Name = ProjectName.Text;
			Project.CurrentConfiguration = _currentSelection.Configuration;
			Project.Pristine = false;

			DialogResult = DialogResult.OK;
			Close();
		}

		private class ConfigurationSelection
		{
			public NesCartridge Configuration { get; private set; }
			public ConfigurationSettings SettingsControl { get; set; }

			public ConfigurationSelection(NesCartridge configuration)
			{
				Configuration = configuration;
			}

			public override string ToString()
			{
				return Configuration == null ? "Create new..." : Configuration.ToString();
			}
		}

		private void DeleteConfigurationButton_Click(object sender, EventArgs e)
		{
			if (Project.BuildConfigurations.Count <= 1)
			{
				MessageBox.Show(this,
					"Unable to delete the last remaining build configuration. You must create another one first.",
					"Delete configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			if (MessageBox.Show(this, string.Format("Are you sure you want to delete the build configuration '{0}'?", _currentSelection.Configuration.ToString()), "Delete configuration", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel) return;

			Project.BuildConfigurations.Remove(_currentSelection.Configuration);
			ConfigurationSelector.Items.Remove(_currentSelection);
			ConfigurationSelector.SelectedIndex = 0;
			Project.Pristine = false;
		}
	}

}
