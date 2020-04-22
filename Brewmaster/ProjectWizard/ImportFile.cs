using System;
using System.Windows.Forms;
using BrewMaster.ProjectModel;

namespace BrewMaster.ProjectWizard
{
	public partial class ImportFile : UserControl
	{
		private ImportProjectFiles.FileImportInfo _file;

		public ImportFile()
		{
			InitializeComponent();

			ContentType.Items.Add("Source file");
			ContentType.Items.Add("Data file");
			ContentType.Items.Add("Linker configuration");
			ContentType.Items.Add("Other");
		}

		public ImportProjectFiles.FileImportInfo File
		{
			set
			{
				_file = value;
				EnableFile.Text = _file.ProjectFile.GetRelativePath();
				ContentType.Enabled = EnableFile.Checked = _file.IncludeInProject;

				switch (_file.ProjectFile.Mode)
				{
					case CompileMode.IncludeInAssembly:
						ContentType.SelectedIndex = 0;
						break;
					case CompileMode.ContentPipeline:
						ContentType.SelectedIndex = 1;
						break;
					case CompileMode.LinkerConfig:
						ContentType.SelectedIndex = 2;
						break;
					default:
						ContentType.SelectedIndex = 3;
						break;
				}

				EnableProcessing.Visible = false;
			}
		}
		private void ContentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (ContentType.SelectedIndex)
			{
				case 0:
					_file.ProjectFile.Mode = CompileMode.IncludeInAssembly;
					break;
				case 1:
					_file.ProjectFile.Mode = CompileMode.ContentPipeline;
					break;
				case 2:
					_file.ProjectFile.Mode = CompileMode.LinkerConfig;
					break;
				case 3:
					_file.ProjectFile.Mode = CompileMode.Ignore;
					break;
			}
		}
		private void EnableFile_CheckedChanged(object sender, System.EventArgs e)
		{
			_file.IncludeInProject = ContentType.Enabled = EnableFile.Checked;
		}

		public FocusType GetFocus()
		{
			return ContentType.ContainsFocus ? FocusType.ContentType
				: EnableFile.ContainsFocus ? FocusType.Enable
				: EnableProcessing.ContainsFocus ? FocusType.Process
				: FocusType.None;
		}

		public enum FocusType
		{
			None, Enable, ContentType, Process
		}

		public void Focus(FocusType focusType)
		{
			Focus();
			switch (focusType)
			{
				case FocusType.Enable:
					EnableFile.Focus();
					break;
				case FocusType.ContentType:
					ContentType.Select();
					ContentType.Focus();
					break;
				case FocusType.Process:
					EnableProcessing.Focus();
					break;
			}
		}

	}
}
