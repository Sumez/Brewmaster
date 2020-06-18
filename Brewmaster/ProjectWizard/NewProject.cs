using System;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class NewProject : WizardDialog
	{
		private NewProjectPath _newProjectPath;
		private ProjectTemplates _projectTemplates;
		public AsmProject Project { get; set; }
		public TargetPlatform Platform
		{
			get { return _newProjectPath.Platform; }
			private set { _newProjectPath.Platform = value; }
		}

		public NewProject(Settings.Settings settings, TargetPlatform projectType) : base(settings)
		{
			_newProjectPath = new NewProjectPath();
			_projectTemplates = new ProjectTemplates();

			Platform = projectType;
			_newProjectPath.Directory =
				string.IsNullOrWhiteSpace(Settings.DefaultProjectDirectory)
					? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
					: Settings.DefaultProjectDirectory;

			AddSteps(_newProjectPath, _projectTemplates);
		}

		protected override void Save()
		{
			Settings.DefaultProjectDirectory = _newProjectPath.Directory;
			Settings.Save();
		}
	}

}
