using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class ImportProject : WizardDialog
	{
		public AsmProject Project { get; set; }

		private readonly ImportProjectPath _importProjectPath;
		private readonly ImportProjectFiles _importFiles;
		private readonly SelectBuildProcessType _selectBuildProcessType;
		private BuildConfigurationStep _buildConfiguration;

		public ImportProject(Settings.Settings settings) : base(settings)
		{
			AddSteps(
				_importProjectPath = new ImportProjectPath(),
				_importFiles = new ImportProjectFiles(),
				_selectBuildProcessType = new SelectBuildProcessType(),
				_buildConfiguration = new BuildConfigurationStep()
			);
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		protected override void ChangeStep(int step)
		{
			if (Step == 0 && step == 1)
			{
				// After selecting import path
				var directory = new DirectoryInfo(_importProjectPath.Directory);
				if (Project == null || Project.Directory.Name != directory.Name)
				{
					Project = null;
					var nesProject = directory.GetFiles().FirstOrDefault(f => f.Extension.ToLower() == ".nesproject");
					if (nesProject != null && MessageBox.Show(this, string.Format("Do you want to try loading '{0}'?", nesProject.Name), "Import file", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						try
						{
							Project = AsmProject.ImportFromFile(nesProject);
						}
						catch (Exception ex)
						{
							Program.Error("An error occured while trying to load the file:\n\n" + ex.Message, ex);
							Project = null;
						}
					}
					if (Project == null) Project = AsmProject.ImportFromDirectory(directory);
					_selectBuildProcessType.Project = _importFiles.Project = Project;
				}
				Project.Name = _importProjectPath.ProjectName;
				Project.ProjectFile = new FileInfo(_importProjectPath.ProjectFile);
				_importFiles.RefreshTree();
			}

			if (Step == 2 && step == 3)
			{
				// After selecting build configuration type
				var configuration = Project.BuildConfigurations.FirstOrDefault();
				var isCustom = _selectBuildProcessType.SelectedType == BuildProcessPreset.Custom;
				if (_buildConfiguration.Project == null || (configuration != null && configuration.Custom != isCustom))
				{
					Project.BuildConfigurations.Clear();
					configuration = new NesCartridge { Custom = isCustom };
					if (!isCustom)
					{
						var defaultConfigFile = Project.Files.FirstOrDefault(f => f.Mode == CompileMode.LinkerConfig);
						if (defaultConfigFile != null) configuration.LinkerConfigFile = defaultConfigFile.GetRelativePath();
						configuration.Filename = string.Format("bin/{0}.{1}", _importProjectPath.BaseFilename, Project.Type == ProjectType.Snes ? "sfc" : "nes");
					}
					Project.BuildConfigurations.Add(configuration);
					_buildConfiguration.Project = Project;
				}
			}

			if (Step == 3 && step == 4)
			{
				// After editing build configuration
				if (!_buildConfiguration.UpdateConfiguration()) return;
			}
			base.ChangeStep(step);
		}

		protected override void Save()
		{
			if (Project == null) return;
			if (!_buildConfiguration.UpdateConfiguration()) return;
			Project.Save();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
