using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class ImportProjectPath : WizardStep
	{
		private bool _projectNameChanged;
		public override event Action ValidChanged;

		public string Directory
		{
			get { return _projectDirectory.Text; }
			set { _projectDirectory.Text = value; }
		}

		public string ProjectName
		{
			get { return _projectName.Text; }
			set { _projectName.Text = value; }
		}
		public string ProjectFile
		{
			get { return _projectPathPreview.Text; }
			set { _projectPathPreview.Text = value; }
		}

		public ImportProjectPath()
		{
			InitializeComponent();
		}

		private void _projectName_TextChanged(object sender, EventArgs e)
		{
			_projectNameChanged = true;
			RefreshPreviewText();
		}

		public static readonly IEnumerable<char> InvalidPathChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).Except(new [] {'\\', '/' });
		public static readonly IEnumerable<char> InvalidFileChars = Path.GetInvalidFileNameChars();

		private void RefreshPreviewText()
		{
			var valid = GetFilePath(out var path);

			_projectPathPreview.ForeColor = valid ? SystemColors.ControlText : Color.Red;
			_projectPathPreview.Text = path;

			if (valid == Valid) return;
			Valid = valid;
			if (ValidChanged != null) ValidChanged();
		}

		private bool GetFilePath(out string path)
		{
			string error = null;
			var filename = _projectName.Text;
			foreach (var character in InvalidFileChars) filename = filename.Replace(character.ToString(), "");
			if (string.IsNullOrWhiteSpace(filename)) error = "Invalid project name";

			try
			{
				if (!new DirectoryInfo(_projectDirectory.Text).Exists) error = "Directory does not exist";
			}
			catch
			{
				error = "Directory does not exist";
			}

			filename = filename.Trim();

			path = error ??
			       new DirectoryInfo(Path.Combine(
				       _projectDirectory.Text,
				       filename + ".bwm")).FullName;

			return error == null;
		}

		private void _projectDirectory_TextChanged(object sender, EventArgs e)
		{
			if ((!_projectNameChanged || string.IsNullOrWhiteSpace(_projectName.Text)) && !string.IsNullOrWhiteSpace(_projectDirectory.Text))
			{
				var dirs = _projectDirectory.Text.Split(new[] {'\\', '/'}, StringSplitOptions.RemoveEmptyEntries);
				if (dirs.Length > 0)
				{
					_projectName.Text = dirs.Last().Trim();
					_projectNameChanged = false;
				}
			}
			RefreshPreviewText();
		}
		
		private void _browseProjectPath_Click(object sender, EventArgs e)
		{
			using (var dialog = new FolderBrowserDialog())
			{
				dialog.ShowNewFolderButton = false;
				dialog.SelectedPath = _projectDirectory.Text;
				if (dialog.ShowDialog(this) == DialogResult.OK) _projectDirectory.Text = dialog.SelectedPath;
			}
		}
	}
}
