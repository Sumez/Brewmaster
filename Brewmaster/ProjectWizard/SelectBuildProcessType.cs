using System;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public class SelectBuildProcessType : WizardStep
	{
		public BuildProcessPreset SelectedType = BuildProcessPreset.None;
		public SelectBuildProcessType()
		{
			InitializeComponent();

			_useIntegrated.CheckedChanged += CheckSelection;
			_useCustom.CheckedChanged += CheckSelection;
		}

		private void CheckSelection(object sender, EventArgs e)
		{
			SelectedType = _useIntegrated.Checked
				? BuildProcessPreset.Integrated
				: _useCustom.Checked
					? BuildProcessPreset.Custom
					: BuildProcessPreset.None;

			Valid = SelectedType != BuildProcessPreset.None;
		}

		public AsmProject Project { get; set; }

		public override void OnEnable()
		{
			base.OnEnable();
			if (Project.BuildConfigurations.Count > 0)
			{
				_useIntegrated.Checked = !Project.BuildConfigurations[0].Custom;
				_useCustom.Checked = Project.BuildConfigurations[0].Custom;
			}
		}

		private System.Windows.Forms.RadioButton _useIntegrated;
		private System.Windows.Forms.Label _integratedDescription;
		private System.Windows.Forms.Label _customDescription;
		private System.Windows.Forms.RadioButton _useCustom;

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectBuildProcessType));
			this._useIntegrated = new System.Windows.Forms.RadioButton();
			this._useCustom = new System.Windows.Forms.RadioButton();
			this._integratedDescription = new System.Windows.Forms.Label();
			this._customDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _useIntegrated
			// 
			this._useIntegrated.AutoSize = true;
			this._useIntegrated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._useIntegrated.Location = new System.Drawing.Point(53, 31);
			this._useIntegrated.Name = "_useIntegrated";
			this._useIntegrated.Size = new System.Drawing.Size(246, 17);
			this._useIntegrated.TabIndex = 0;
			this._useIntegrated.TabStop = true;
			this._useIntegrated.Text = "Use Brewmaster\'s integrated build process";
			this._useIntegrated.UseVisualStyleBackColor = true;
			// 
			// _useCustom
			// 
			this._useCustom.AutoSize = true;
			this._useCustom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._useCustom.Location = new System.Drawing.Point(53, 116);
			this._useCustom.Name = "_useCustom";
			this._useCustom.Size = new System.Drawing.Size(227, 17);
			this._useCustom.TabIndex = 1;
			this._useCustom.TabStop = true;
			this._useCustom.Text = "Use a custom defined build process";
			this._useCustom.UseVisualStyleBackColor = true;
			// 
			// _integratedDescription
			// 
			this._integratedDescription.Location = new System.Drawing.Point(71, 51);
			this._integratedDescription.Name = "_integratedDescription";
			this._integratedDescription.Size = new System.Drawing.Size(366, 58);
			this._integratedDescription.TabIndex = 2;
			this._integratedDescription.Text = resources.GetString("_integratedDescription.Text");
			// 
			// _customDescription
			// 
			this._customDescription.Location = new System.Drawing.Point(71, 136);
			this._customDescription.Name = "_customDescription";
			this._customDescription.Size = new System.Drawing.Size(366, 66);
			this._customDescription.TabIndex = 3;
			this._customDescription.Text = "Allows you to define a set of command lines to run whenever the build option is s" +
    "tarted, running custom scripts, makefiles, etc.";
			// 
			// SelectBuildProcessType
			// 
			this.Controls.Add(this._customDescription);
			this.Controls.Add(this._integratedDescription);
			this.Controls.Add(this._useCustom);
			this.Controls.Add(this._useIntegrated);
			this.Name = "SelectBuildProcessType";
			this.Size = new System.Drawing.Size(577, 235);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}

	public enum BuildProcessPreset
	{
		None, Integrated, Custom
	}
}
