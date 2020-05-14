using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class BuildConfigurationStep : WizardStep
	{
		private ConfigurationSettings _settingsControl;
		private AsmProject _project;

		public AsmProject Project
		{
			get { return _project; }
			set
			{
				_project = value;
				if (_settingsControl != null) Controls.Remove(_settingsControl);
				_settingsControl = new ConfigurationSettings(_project.BuildConfigurations[0], _project);
				_settingsControl.Dock = DockStyle.Fill;
				Controls.Add(_settingsControl);
				Valid = true;
			}
		}

		public bool UpdateConfiguration()
		{
			if (_project.BuildConfigurations.Count == 0)
			{
				var configuration = new NesCartridge();
				if (!_settingsControl.SetConfigurationSettings(configuration)) return false;
				_project.BuildConfigurations.Add(configuration);
				return true;
			}
			return _settingsControl.SetConfigurationSettings(_project.BuildConfigurations[0]);
		}


		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// BuildConfigurationStep
			// 
			this.AutoSize = true;
			this.Name = "BuildConfigurationStep";
			this.Size = new System.Drawing.Size(585, 330);
			this.ResumeLayout(false);

		}
	}
}
