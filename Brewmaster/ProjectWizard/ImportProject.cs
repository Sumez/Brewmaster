﻿using System.Collections.Generic;
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
					// TODO: Identify existing .nesproject or .bwm file in directory
					Project = AsmProject.ImportFromDirectory(directory);
					_selectBuildProcessType.Project = _importFiles.Project = Project;
				}
				Project.Name = _importProjectPath.ProjectName;
				Project.ProjectFile = new FileInfo(_importProjectPath.ProjectFile);
				// TODO: Refresh tree in _importfiles to update project name
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
						if (defaultConfigFile != null) ConfigurationFile.Text = defaultConfigFile.GetRelativePath();
						OutputFile.Text = Project.Type == ProjectType.Snes ? "bin/game.sfc" : "bin/game.nes";
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
