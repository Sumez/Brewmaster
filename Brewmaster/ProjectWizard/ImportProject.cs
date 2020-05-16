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
					_importFiles.Project = Project;
				}
				Project.Name = _importProjectPath.ProjectName;
				Project.ProjectFile = new FileInfo(_importProjectPath.ProjectFile);
				_importFiles.RefreshTree();
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
