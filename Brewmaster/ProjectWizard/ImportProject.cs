using System.IO;
using System.Windows.Forms;
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
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		protected override void ChangeStep(int step)
		{
			if (Step == 0 && step == 1)
			{
				var directory = new DirectoryInfo(_importProjectPath.Directory);
				if (Project == null || Project.Directory.Name != directory.Name)
				{
					// TODO: Identify existing .nesproject or .bwm file in directory
					Project = AsmProject.ImportFromDirectory(directory);
					_importFiles.Project = Project;
				}
				Project.Name = _importProjectPath.ProjectName;
				Project.ProjectFile = new FileInfo(_importProjectPath.ProjectFile);
				// TODO: Refresh tree in _importfiles to update project name
			}
			base.ChangeStep(step);
		}

		protected override void Save()
		{
			if (Project == null) return;
			Project.Save();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
