using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.ProjectWizard
{
	public partial class WizardDialog : Form
	{
		protected readonly Settings.Settings Settings;

		protected WizardDialog()
		{

		}

		public WizardDialog(Settings.Settings settings)
		{
			Settings = settings;
			InitializeComponent();

			Steps = new List<WizardStep>();
		}

		protected void AddSteps(params WizardStep[] steps)
		{
			foreach (var step in steps)
			{
				step.Visible = false;
				step.Dock = DockStyle.Fill;
				StepPanel.Controls.Add(step);
				Steps.Add(step);
				step.ValidChanged += UpdateButtons;
			}
			Step = 0;
			DisplayStep();
		}

		protected void UpdateButtons()
		{
			_previousButton.Enabled = Step > 0;
			_nextButton.Enabled = Steps[Step].Valid && Step < (Steps.Count - 1);
			_finishButton.Enabled = Steps.All(s => s.Valid);
		}

		protected void DisplayStep()
		{
			if (Step < 0) Step = 0;
			if (Step > Steps.Count - 1) Step = Steps.Count - 1;
			
			SuspendLayout();
			for (var i = 0; i < Steps.Count; i++)
			{
				if (i == Step && !Steps[i].Visible)
				{
					Steps[i].Visible = true;
					Steps[i].OnEnable();
				}
				else Steps[i].Visible = false;
			}

			UpdateButtons();
			ResumeLayout();
		}

		public int Step { get; set; }
		public List<WizardStep> Steps { get; set; }


		private void _finishButton_Click(object sender, EventArgs e)
		{
			Save();
		}

		protected virtual void Save()
		{

		}

		private void _nextButton_Click(object sender, EventArgs e)
		{
			ChangeStep(Step + 1);
		}

		protected virtual void ChangeStep(int step)
		{
			Step = step;
			DisplayStep();
		}

		private void _previousButton_Click(object sender, EventArgs e)
		{
			ChangeStep(Step - 1);
		}
	}


	public class WizardStep : UserControl
	{
		public event Action ValidChanged;
		private bool _valid;
		public bool Valid
		{
			get { return _valid; }
			protected set
			{
				if (_valid == value) return;
				_valid = value;
				if (ValidChanged != null) ValidChanged();
			}

		}

		public virtual void OnEnable() { }
	}
}
