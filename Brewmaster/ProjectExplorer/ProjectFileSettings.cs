using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.Pipeline;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectExplorer
{
	public class ProjectFileSettings : UserControl
	{
		public ProjectFileSettings(Events events)
		{
			InitializeComponent();
			_events = events;
			_assemblySetting.SetOptions("No", "Yes", "Build as SPC");
			_assemblySetting.ValueChanged += (value) =>
			{
				if (_file == null || (_file.Type != FileType.Source && _file.Type != FileType.Include)) return;
				if (_file.Mode != CompileMode.Ignore && value == "No")
				{
					_file.Mode = CompileMode.Ignore;
					_file.Project.Pristine = false;
				}
				if (_file.Mode != CompileMode.IncludeInAssembly && value == "Yes")
				{
					_file.Mode = CompileMode.IncludeInAssembly;
					_file.Project.Pristine = false;
				}
				if (_file.Mode != CompileMode.Spc && value == "Build as SPC")
				{
					_file.Mode = CompileMode.Spc;
					_file.Project.Pristine = false;
				}
			};

			_pipelineSetting.SetOptions("No processing");
			_pipelineSetting.ValueChanged += (value) =>
			{
				if (_file == null) return;

				var oldPipeline = _file.Pipeline;
				if (oldPipeline != null) _file.OldPipelines[oldPipeline.Type.TypeName] = oldPipeline;

				if (value is PipelineOption option)
				{
					if (_file.OldPipelines.ContainsKey(option.TypeName))
						_file.Pipeline = _file.OldPipelines[option.TypeName];
					else _file.Pipeline = option.Create(_file);
				}
				else _file.Pipeline = null;

				if (_file.Pipeline != oldPipeline)
				{
					_file.Project.Pristine = false;
					RefreshPipelineSettings();
				}
			};

			Clear();
		}

		private void RefreshPipelineSettings()
		{
			_dynamicSettingsPanel.Controls.Clear();
			if (_file.Pipeline == null)
			{
				_outputSetting.Visible = false;
				return;
			}
			_outputSetting.Visible = true;
			_outputSetting.Value = _file.Pipeline.OutputFiles.Count > 0 ? _file.Pipeline.OutputFiles[0] : "";
			_outputSetting.Disabled = _file.Pipeline.Type is ChrPipeline;

			_outputSetting.ValueChanged += (value) =>
			{
				_file.Pipeline.OutputFiles[0] = value as string;
				_file.Pipeline.SettingChanged();
			};

			foreach (var property in _file.Pipeline.Type.Properties.Reverse())
			{
				var settingControl = new SettingControl {
					Label = Program.Language.Get(_file.Pipeline.Type.TypeName + "." + property.SystemName) + ":",
					InputType = property.Type == PipelinePropertyType.Text ? InputType.Text : property.Type == PipelinePropertyType.ProjectFile ? InputType.Callback : InputType.Dropdown
				};
				switch (property.Type)
				{
					case PipelinePropertyType.Boolean:
						settingControl.SetOptions("No", "Yes");
						settingControl.Value = _file.Pipeline.GenericSettings.GetBoolean(property.SystemName) ? "Yes" : "No";
						break;
					case PipelinePropertyType.Select:
						settingControl.SetOptions(property.Options.Select(o => new Option(o, _file.Pipeline.Type.TypeName + "." + property.SystemName) as object).ToArray());
						settingControl.Value = new Option(_file.Pipeline.GenericSettings[property.SystemName], _file.Pipeline.Type.TypeName + "." + property.SystemName);
						break;
					case PipelinePropertyType.ProjectFile:
						settingControl.Callback = (ref string value) =>
						{
							using (var dialog = new OpenFileDialog { CheckFileExists = false, InitialDirectory = _file.Project.Directory.FullName })
							{
								if (!string.IsNullOrWhiteSpace(value))
								{
									var currentFile = new FileInfo(Path.Combine(_file.Project.Directory.FullName, value));
									if (currentFile.Exists)
									{
										dialog.FileName = currentFile.Name;
										dialog.InitialDirectory = currentFile.DirectoryName;
									}
								}

								dialog.Filter = property.GetFileNameFilter != null ? property.GetFileNameFilter(_file.Pipeline) : "*.*|*.*";
								if (dialog.ShowDialog() != DialogResult.OK) return false;
								var filePath = _file.Project.GetRelativePath(dialog.FileName, true);
								if (filePath == null)
								{
									MessageBox.Show("File must exist inside project directory", "Invalid file path", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									return false;
								}

								var fullPath = Path.Combine(_file.Project.Directory.FullName, filePath);
								if (!System.IO.File.Exists(fullPath)) using (var file = System.IO.File.Create(fullPath)) { }
								_file.Pipeline.GenericSettings[property.SystemName] = value = filePath; // Set path on pipeline settings *before* potentially adding a new file to the project, project explorer would reload the view too early

								var fileInfo = new FileInfo(fullPath);
								var projectFile = _file.Project.Files.FirstOrDefault(f => f.File.FullName == fileInfo.FullName);
								if (projectFile == null)
								{
									projectFile = new AsmProjectFile { Mode = CompileMode.Ignore, Project = _file.Project, File = fileInfo };
									_file.Project.AddNewProjectFile(projectFile);
								}
								_events.OpenFile(projectFile);
								return true;
							}
						};
						settingControl.Value = _file.Pipeline.GenericSettings[property.SystemName];
						break;
					default:
						settingControl.Value = _file.Pipeline.GenericSettings[property.SystemName];
						break;
				}

				settingControl.ValueChanged += (value) =>
				{
					var stringValue = value as string;
					if (property.Type == PipelinePropertyType.Boolean)
					{
						_file.Pipeline.GenericSettings[property.SystemName] = stringValue == "Yes" ? "1" : "0";
					}
					else if (property.Type == PipelinePropertyType.Select)
					{
						var objectValue = (Option)value;
						_file.Pipeline.GenericSettings[property.SystemName] = objectValue.Name;
					}
					else _file.Pipeline.GenericSettings[property.SystemName] = stringValue;

					_file.Pipeline.SettingChanged();
				};
				_dynamicSettingsPanel.Controls.Add(settingControl);
			}

		}
		private struct Option
		{
			public override string ToString()
			{
				return Program.Language.Get(Prefix + "." + Name);
			}

			public Option(string name, string prefix)
			{
				Prefix = prefix;
				Name = name;
			}

			public string Prefix { get; }
			public string Name { get; }
		}

		private Panel _headerPanel;
		private Label _fileNameLabel;
		private SettingControl _pipelineSetting;
		private SettingControl _outputSetting;
		private Panel _settingsPanel;
		private SettingControl _compressionSetting;
		private SettingControl _assemblySetting;
		private Panel _dynamicSettingsPanel;
		private AsmProjectFile _file;
		private readonly Events _events;

		public AsmProjectFile File
		{
			get { return _file; }
			set
			{
				_file = value;
				_fileNameLabel.Text = _file == null ? "" : _file.File.Name;
				foreach (var settingControl in SettingControls)
				{
					settingControl.Disabled = true;
				}
				if (_file != null)
				{
					_pipelineSetting.Disabled = !(_pipelineSetting.Visible = _file.IsDataFile);
					_assemblySetting.Disabled = !(_assemblySetting.Visible = _file.Type == FileType.Source || _file.Type == FileType.Include);
					switch (_file.Mode)
					{
						case CompileMode.Ignore:
							_assemblySetting.Value = "No";
							break;
						case CompileMode.IncludeInAssembly:
							_assemblySetting.Value = "Yes";
							break;
						case CompileMode.Spc:
							_assemblySetting.Value = "No";
							break;
					}

					_pipelineSetting.SetOptions(new object[] { "No processing" }.Concat(PipelineSettings.PipelineOptions.Where(o => o.SupportedFileTypes.Contains(_file.Type))).ToArray());

					if (_file.Pipeline == null) _pipelineSetting.Value = "No processing";
					else _pipelineSetting.Value = _file.Pipeline.Type;

					RefreshPipelineSettings();
				}
			}
		}

		public void Clear()
		{
			File = null;
			foreach (var settingControl in SettingControls)
			{
				settingControl.Visible = false;
			}
		}

		public IEnumerable<SettingControl> SettingControls { get { return _settingsPanel.Controls.OfType<SettingControl>().Concat(_dynamicSettingsPanel.Controls.OfType<SettingControl>()); } }

		private void InitializeComponent()
		{
			this._headerPanel = new System.Windows.Forms.Panel();
			this._fileNameLabel = new System.Windows.Forms.Label();
			this._settingsPanel = new System.Windows.Forms.Panel();
			this._dynamicSettingsPanel = new System.Windows.Forms.Panel();
			this._outputSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._compressionSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._pipelineSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._assemblySetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._headerPanel.SuspendLayout();
			this._settingsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _headerPanel
			// 
			this._headerPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this._headerPanel.Controls.Add(this._fileNameLabel);
			this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerPanel.Location = new System.Drawing.Point(0, 0);
			this._headerPanel.Name = "_headerPanel";
			this._headerPanel.Size = new System.Drawing.Size(476, 18);
			this._headerPanel.TabIndex = 7;
			// 
			// _fileNameLabel
			// 
			this._fileNameLabel.AutoSize = true;
			this._fileNameLabel.Location = new System.Drawing.Point(3, 2);
			this._fileNameLabel.Name = "_fileNameLabel";
			this._fileNameLabel.Size = new System.Drawing.Size(64, 13);
			this._fileNameLabel.TabIndex = 3;
			this._fileNameLabel.Text = "filename.chr";
			// 
			// _settingsPanel
			// 
			this._settingsPanel.AutoScroll = true;
			this._settingsPanel.Controls.Add(this._dynamicSettingsPanel);
			this._settingsPanel.Controls.Add(this._outputSetting);
			this._settingsPanel.Controls.Add(this._compressionSetting);
			this._settingsPanel.Controls.Add(this._pipelineSetting);
			this._settingsPanel.Controls.Add(this._assemblySetting);
			this._settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._settingsPanel.Location = new System.Drawing.Point(0, 18);
			this._settingsPanel.Name = "_settingsPanel";
			this._settingsPanel.Size = new System.Drawing.Size(476, 371);
			this._settingsPanel.TabIndex = 14;
			// 
			// _dynamicSettingsPanel
			// 
			this._dynamicSettingsPanel.AutoSize = true;
			this._dynamicSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._dynamicSettingsPanel.Location = new System.Drawing.Point(0, 88);
			this._dynamicSettingsPanel.Name = "_dynamicSettingsPanel";
			this._dynamicSettingsPanel.Size = new System.Drawing.Size(476, 0);
			this._dynamicSettingsPanel.TabIndex = 15;
			// 
			// _outputSetting
			// 
			this._outputSetting.Label = "Output file:";
			this._outputSetting.Location = new System.Drawing.Point(0, 66);
			this._outputSetting.Name = "_outputSetting";
			this._outputSetting.Size = new System.Drawing.Size(476, 22);
			this._outputSetting.TabIndex = 13;
			this._outputSetting.Visible = false;
			// 
			// _compressionSetting
			// 
			this._compressionSetting.Disabled = true;
			this._compressionSetting.InputType = Brewmaster.ProjectExplorer.InputType.Dropdown;
			this._compressionSetting.Label = "Compression method:";
			this._compressionSetting.Location = new System.Drawing.Point(0, 44);
			this._compressionSetting.Name = "_compressionSetting";
			this._compressionSetting.Size = new System.Drawing.Size(476, 22);
			this._compressionSetting.TabIndex = 12;
			this._compressionSetting.Visible = false;
			// 
			// _pipelineSetting
			// 
			this._pipelineSetting.InputType = Brewmaster.ProjectExplorer.InputType.Dropdown;
			this._pipelineSetting.Label = "Process data:";
			this._pipelineSetting.Location = new System.Drawing.Point(0, 22);
			this._pipelineSetting.Name = "_pipelineSetting";
			this._pipelineSetting.Size = new System.Drawing.Size(476, 22);
			this._pipelineSetting.TabIndex = 11;
			this._pipelineSetting.Visible = false;
			// 
			// _assemblySetting
			// 
			this._assemblySetting.InputType = Brewmaster.ProjectExplorer.InputType.Dropdown;
			this._assemblySetting.Label = "Include in assembly:";
			this._assemblySetting.Location = new System.Drawing.Point(0, 0);
			this._assemblySetting.Name = "_assemblySetting";
			this._assemblySetting.Size = new System.Drawing.Size(476, 22);
			this._assemblySetting.TabIndex = 14;
			this._assemblySetting.Visible = false;
			// 
			// ProjectFileSettings
			// 
			this.Controls.Add(this._settingsPanel);
			this.Controls.Add(this._headerPanel);
			this.Name = "ProjectFileSettings";
			this.Size = new System.Drawing.Size(476, 389);
			this._headerPanel.ResumeLayout(false);
			this._headerPanel.PerformLayout();
			this._settingsPanel.ResumeLayout(false);
			this._settingsPanel.PerformLayout();
			this.ResumeLayout(false);

		}
		
	}
}
