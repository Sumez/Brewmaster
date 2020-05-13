using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class ImportProjectFiles : WizardStep
	{
		public ProjectExplorer.ProjectExplorer ProjectExplorer { get; }
		private AsmProject _project;
		private readonly List<ImportFile> _fileControls = new List<ImportFile>();
		private int _focusControl = -1;
		private ImportFile.FocusType _focusType = ImportFile.FocusType.None;
		private int _firstIndex;

		public ImportProjectFiles()
		{
			InitializeComponent();

			ProjectExplorer = new ProjectExplorer.ProjectExplorer(new Events(), true, false) { Dock = DockStyle.Fill };
			FilePanel.Controls.Add(ProjectExplorer);
			return;

			ScrollBar.Scroll += (sender, args) =>
			{
				RefreshScroll(args.NewValue);
			};
			FileList.MouseWheel += (sender, args) =>
			{
				ScrollBar.Value = Math.Max(0, Math.Min(ScrollBar.Maximum, ScrollBar.Value - (args.Delta / 20)));
				RefreshScroll(ScrollBar.Value);
			};
			for (var i = 0; i < 12; i++)
			{
				var fileControl = new ImportFile {Dock = DockStyle.Top};
				FileList.Controls.Add(fileControl);
				_fileControls.Insert(0, fileControl);
			}
		}

		private void RefreshScroll(int scrollAmount)
		{

			var oldIndex = _firstIndex;
			_firstIndex = scrollAmount / 6;

			for (var i = 0; i < 12; i++)
			{
				if (_fileControls[i].ContainsFocus)
				{
					_focusControl = oldIndex + i;
					_focusType = _fileControls[i].GetFocus();
				}

			}

			FileList.Top = (scrollAmount % 6) * -4;
			ScrollBar.Focus();

			for (var i = 0; i < 12; i++)
			{
				if (i + _firstIndex >= ImportSettings.Count)
				{
					_fileControls[i].Visible = false;
				}
				else
				{
					_fileControls[i].Visible = true;
					_fileControls[i].File = ImportSettings[i + _firstIndex];
					if (_focusControl == i + _firstIndex) _fileControls[i].Focus(_focusType);
				}
			}
		}

		public AsmProject Project
		{
			get { return _project; }
			set
			{
				_project = value;
				ProjectExplorer.SetProject(_project);
				ProjectExplorer.Nodes[0].EnsureVisible();
				Valid = true;
				return;
				
				ImportSettings = value.Files.Select(f => new FileImportInfo(f)).ToList();

				ScrollBar.Minimum = 0;
				ScrollBar.Maximum = Math.Max(0, (value.Files.Count - 5) * 6);
				ScrollBar.Value = 0;
				RefreshScroll(0);
			}
		}

		public List<FileImportInfo> ImportSettings { get; set; }
		public class FileImportInfo
		{
			public AsmProjectFile ProjectFile { get; }
			public bool IncludeInProject { get; set; }

			public FileImportInfo(AsmProjectFile projectFile)
			{
				ProjectFile = projectFile;
				IncludeInProject = true;

				if (projectFile.GetRelativePath().StartsWith("bin/")) IncludeInProject = false;
				if (projectFile.File.Name.StartsWith(".")) IncludeInProject = false;
				if (projectFile.File.Extension == ".nes") IncludeInProject = false;
			}
		}
	}
}
