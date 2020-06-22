﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class NewProjectPath : WizardStep
	{
		private bool _folderNameChanged;

		public string ProjectPath { get { return Valid ? _projectPathPreview.Text : null; } }
		public string ProjectName { get { return Valid ? _projectName.Text : null; } }

		public TargetPlatform Platform
		{
			get { return _snesProjectOption.Checked ? TargetPlatform.Snes : TargetPlatform.Nes; }
			set
			{
				_nesProjectOption.Checked = value == TargetPlatform.Nes;
				_snesProjectOption.Checked = value == TargetPlatform.Snes;
			}
		}

		public string Directory
		{
			get { return _projectDirectory.Text; }
			set { _projectDirectory.Text = value; }
		}

		public NewProjectPath(ProjectType projectType)
		{
			_projectType = projectType;
			InitializeComponent();

			_platformSelection.Visible = _projectType == ProjectType.Game;
		}

		private void _nesImage_Click(object sender, MouseEventArgs e)
		{
			_nesProjectOption.PerformClick();
		}

		private void _snesImage_Click(object sender, MouseEventArgs e)
		{
			_snesProjectOption.PerformClick();
		}

		private void _projectName_TextChanged(object sender, EventArgs e)
		{
			if (!_folderNameChanged)
			{
				_projectFolderName.Text = _projectName.Text;
				_folderNameChanged = false;
			}
		}

		private void _projectFolderName_TextChanged(object sender, EventArgs e)
		{
			_folderNameChanged = true;
			RefreshPreviewText();
		}

		public static readonly IEnumerable<char> InvalidPathChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).Except(new [] {'\\', '/' });
		public static readonly IEnumerable<char> InvalidFileChars = Path.GetInvalidFileNameChars();
		private ProjectType _projectType;

		private void RefreshPreviewText()
		{
			var valid = GetFilePath(out var path);

			_projectPathPreview.ForeColor = valid ? SystemColors.ControlText : Color.Red;
			_projectPathPreview.Text = path;

			Valid = valid;
		}

		private bool GetFilePath(out string path)
		{
			string error = null;
			var filename = _projectName.Text;
			foreach (var character in InvalidFileChars) filename = filename.Replace(character.ToString(), "");
			if (string.IsNullOrWhiteSpace(filename)) error = "Invalid project name";

			var folderName = _projectFolderName.Text;
			foreach (var character in InvalidPathChars) if (folderName.Contains(character)) error = "Invalid characters in folder name";

			try
			{
				if (!new DirectoryInfo(_projectDirectory.Text).Exists) error = "Directory does not exist";
			}
			catch
			{
				error = "Directory does not exist";
			}

			filename = filename.Trim();
			folderName = folderName.Trim();

			path = error ??
			       new DirectoryInfo(Path.Combine(
				       _projectDirectory.Text,
				       folderName,
				       filename + ".bwm")).FullName;

			return error == null;
		}

		private void _projectDirectory_TextChanged(object sender, EventArgs e)
		{
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
