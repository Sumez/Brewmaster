using System;
using System.Windows.Forms;

namespace Brewmaster.Modules.Ppu
{
	public partial class ChrViewer : UserControl
	{

		public ChrViewer(Events events)
		{
			InitializeComponent();
			_chrDisplay.ModuleEvents = events;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			_chrDisplay.Width = Width;
			_chrDisplay.Height = Height - (_controlPanel.Visible ? _controlPanel.Height : 0) - 1;
			base.OnLayout(e);
		}
		public bool FitImage
		{
			get { return _scaleButton.Checked; }
			set { _scaleButton.Checked = value; }
		}

		private void _scaleButton_CheckedChanged(object sender, EventArgs e)
		{
			_chrDisplay.FitImage = _scaleButton.Checked;
			// Removes confusing focus from current button
			//if (_scaleButton.Focused && _layerButtons.Count > _requestedLayerId) _layerButtons[_requestedLayerId].Focus();
		}
	}
}
