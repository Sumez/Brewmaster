using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectExplorer
{
	public class ProjectFileSettings : UserControl
	{
		public ProjectFileSettings()
		{
			InitializeComponent();
			_assemblySetting.SetOptions("No", "Yes");
			_assemblySetting.ValueChanged += (value) =>
			{
				if (_file == null || (_file.Type != FileType.Source && _file.Type != FileType.Include)) return;
				if (_file.Mode == CompileMode.IncludeInAssembly && value == "No")
				{
					_file.Mode = CompileMode.Ignore;
					_file.Project.Pristine = false;
				}

				if (_file.Mode != CompileMode.IncludeInAssembly && value == "Yes")
				{
					_file.Mode = CompileMode.IncludeInAssembly;
					_file.Project.Pristine = false;
				}
			};

			Clear();
		}
		private Panel headerPanel;
		private Label _fileNameLabel;
		private SettingControl _pipelineSetting;
		private SettingControl _outputSetting;
		private Panel _settingsPanel;
		private SettingControl _compressionSetting;
		private SettingControl _assemblySetting;
		private AsmProjectFile _file;

		public AsmProjectFile File
		{
			get { return _file; }
			set
			{
				_file = value;
				_fileNameLabel.Text = _file == null ? "" : _file.File.Name;
				foreach (var settingControl in _settingsPanel.Controls.OfType<SettingControl>())
				{
					settingControl.Disabled = true;
				}
				if (_file != null)
				{
					_pipelineSetting.Disabled = !(_pipelineSetting.Visible = _file.IsDataFile);
					_assemblySetting.Disabled = !(_assemblySetting.Visible = _file.Type == FileType.Source || _file.Type == FileType.Include);
					_assemblySetting.Value = _file.Mode == CompileMode.IncludeInAssembly ? "Yes" : "No";
				}
			}
		}

		public void Clear()
		{
			File = null;
			foreach (var settingControl in _settingsPanel.Controls.OfType<SettingControl>())
			{
				settingControl.Visible = false;
			}
		}

		private void InitializeComponent()
		{
			this.headerPanel = new System.Windows.Forms.Panel();
			this._fileNameLabel = new System.Windows.Forms.Label();
			this._settingsPanel = new System.Windows.Forms.Panel();
			this._outputSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._compressionSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._pipelineSetting = new Brewmaster.ProjectExplorer.SettingControl();
			this._assemblySetting = new Brewmaster.ProjectExplorer.SettingControl();
			this.headerPanel.SuspendLayout();
			this._settingsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// headerPanel
			// 
			this.headerPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.headerPanel.Controls.Add(this._fileNameLabel);
			this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerPanel.Location = new System.Drawing.Point(0, 0);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(476, 18);
			this.headerPanel.TabIndex = 7;
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
			// _outputSetting
			// 
			this._outputSetting.Disabled = true;
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
			this.Controls.Add(this.headerPanel);
			this.Name = "ProjectFileSettings";
			this.Size = new System.Drawing.Size(476, 389);
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			this._settingsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		
	}
}
