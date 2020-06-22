using System;
using System.IO;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class NewAssetProject : WizardDialog
	{
		private readonly NewProjectPath _newProjectPath;
		public AsmProject Project { get; set; }
		public TargetPlatform Platform
		{
			get { return _newProjectPath.Platform; }
			private set { _newProjectPath.Platform = value; }
		}

		public NewAssetProject(Settings.Settings settings, TargetPlatform targetPlatform) : base(settings)
		{
			_newProjectPath = new NewProjectPath(ProjectType.AssetOnly);

			Platform = targetPlatform;
			_newProjectPath.Directory =
				string.IsNullOrWhiteSpace(Settings.DefaultProjectDirectory)
					? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
					: Settings.DefaultProjectDirectory;

			AddSteps(_newProjectPath);
		}

		protected override void Save()
		{
			Settings.DefaultProjectDirectory = _newProjectPath.Directory;
			Settings.Save();

			Project = AsmProject.Create(_newProjectPath.ProjectPath, _newProjectPath.ProjectName, ProjectType.AssetOnly, Platform);
			if (!Project.Directory.Exists) Directory.CreateDirectory(Project.Directory.FullName);
			if (Project.ProjectFile.Exists && MessageBox.Show("The project file already exists in this directory.\nAre you sure you want to overwrite?", "File already exists", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				Project = null;
				return;
			}
			Project.Save();
			DialogResult = DialogResult.OK;
		}
	}
}
