using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrewMaster.ProjectModel;

namespace BrewMaster.ProjectWizard
{
	public partial class ProjectTemplates: WizardStep
	{
		private bool _folderNameChanged;
		public override event Action ValidChanged;

		public ProjectTemplates()
		{
			InitializeComponent();
		}

	}
}
