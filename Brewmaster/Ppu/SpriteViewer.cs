using System;
using System.Windows.Forms;

namespace Brewmaster.Ppu
{
	public partial class SpriteViewer : UserControl
	{
		public SpriteViewer()
		{
			InitializeComponent();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			_tileMapDisplay.Width = Width;
			_tileMapDisplay.Height = Height - (_controlPanel.Visible ? _controlPanel.Height : 0) - 1;
			base.OnLayout(e);
		}
		public bool FitImage
		{
			get { return _scaleButton.Checked; }
			set { _scaleButton.Checked = value; }
		}

		private void _scaleButton_CheckedChanged(object sender, EventArgs e)
		{
			_tileMapDisplay.FitImage = _scaleButton.Checked;
			// Removes confusing focus from current button
			//if (_scaleButton.Focused && _layerButtons.Count > _requestedLayerId) _layerButtons[_requestedLayerId].Focus();
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
}
