using System;
using System.Windows.Forms;
using Brewmaster.Emulation;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules.Ppu
{
	public partial class ChrViewer : UserControl
	{
		private CharacterData _data;

		public ChrViewer(Events events)
		{
			InitializeComponent();

			_bitDepthSelector.Items.Add("2bpp");
			_bitDepthSelector.Items.Add("4bpp");
			_bitDepthSelector.Items.Add("8bpp");
			_bitDepthSelector.Items.Add("Direct color");
			_bitDepthSelector.Items.Add("Mode 7");
			_bitDepthSelector.Items.Add("Mode 7 direct color");

			_bitDepthSelector.SelectedIndexChanged += (s, a) => {
				if (_data == null) return;
				_data.RequestRefresh(_bitDepthSelector.SelectedIndex);
			};
			_bitDepthSelector.SelectedIndex = 1;

			events.EmulationStateUpdate += (state) => UpdateChrData(state.CharacterData, state.Type);
			events.PlatformChanged += (type) =>
			{
				var showOptionPanel = type == TargetPlatform.Snes;
				if (_snesOptionPanel.Visible != showOptionPanel) _snesOptionPanel.Visible = showOptionPanel;
			};
		}

		private void UpdateChrData(CharacterData characterData, TargetPlatform type)
		{
			_data = characterData;
			_chrDisplay.UpdateChrData(characterData, type);
			if (_bitDepthSelector.SelectedIndex != characterData.ColorMode && !_bitDepthSelector.Focused) _bitDepthSelector.SelectedIndex = characterData.ColorMode;
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
		}
	}
}
