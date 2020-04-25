using System.IO;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class ImportProject : WizardDialog
	{
		public AsmProject Project { get; set; }

		private readonly ImportProjectPath _importProjectPath;
		private readonly ImportProjectFiles _importFiles;

		public ImportProject(Settings.Settings settings) : base(settings)
		{
			_importProjectPath = new ImportProjectPath();
			_importFiles = new ImportProjectFiles();

			AddSteps(_importProjectPath, _importFiles);
		}

		protected override void ChangeStep(int step)
		{
			if (Step == 0 && step == 1)
			{
				var directory = new DirectoryInfo(_importProjectPath.Directory);
				if (Project == null || Project.Directory.Name != directory.Name)
				{
					_importFiles.Project = Project = AsmProject.ImportFromDirectory(directory);
				}
				Project.Name = _importProjectPath.ProjectName;
				Project.ProjectFile = new FileInfo(_importProjectPath.ProjectFile);
			}
			base.ChangeStep(step);
		}

	}
}
