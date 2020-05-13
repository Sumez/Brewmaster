using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.ProjectWizard
{
	public partial class ProjectTemplates: WizardStep
	{
		private bool _folderNameChanged;

		public ProjectTemplates()
		{
			InitializeComponent();
		}

	}
}
