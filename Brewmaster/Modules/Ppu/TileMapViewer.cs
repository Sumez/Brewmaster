using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Emulation;

namespace Brewmaster.Modules.Ppu
{
	public partial class TileMapViewer : UserControl
	{
		public TileMapViewer()
		{
			InitializeComponent();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			_tileMapDisplay.Width = Width;
			_tileMapDisplay.Height = Height - (_controlPanel.Visible ? _controlPanel.Height : 0) - 1;
			base.OnLayout(e);
		}

		private List<CheckBox> _layerButtons = new List<CheckBox>();
		private int _numberOfLayers = 0;
		private TileMapData _data;
		private int _requestedLayerId = 1;

		public void UpdateNametableData(TileMapData data)
		{
			_data = data;
			_tileMapDisplay.UpdateNametableData(data);
			if (data.NumberOfPages != _numberOfLayers)
			{
				BeginInvoke(new Action<int, int>(UpdateLayerAmount), _data.NumberOfPages, _data.GetPage);
			}

			if (data.GetPage != _requestedLayerId) // Forced to another tilemap page, probably because a new emulator was loaded
			{
				BeginInvoke(new Action<int>(SetCurrentLayer), data.GetPage);
			}
		}

		private void UpdateLayerAmount(int layers, int selectedLayer)
		{
			_numberOfLayers = layers;
			while (_layerButtons.Count < _numberOfLayers)
			{
				var layerId = _layerButtons.Count;
				var button = new CheckBox
				{
					Checked = selectedLayer == layerId,
					Appearance = Appearance.Button,
					TextAlign = ContentAlignment.MiddleCenter,
					Width = 30,
					Height = 20,
					Text = (layerId + 1).ToString(),
					Margin = new Padding(2, 2, 0, 2)
				};
				button.Click += (s, a) => SetCurrentLayer(layerId);
				_layerButtons.Add(button);
				_layerButtonPanel.Controls.Add(button);
			}
			SuspendLayout();
			var visibleButtons = _numberOfLayers > 1 ? _numberOfLayers : 0;
			for (var i = 0; i < visibleButtons; i++) if (!_layerButtons[i].Visible) _layerButtons[i].Visible = true;
			for (var i = visibleButtons; i < _layerButtons.Count; i++) if (_layerButtons[i].Visible) _layerButtons[i].Visible = false;
			ResumeLayout();
		}

		private void SetCurrentLayer(int layerId)
		{
			_data.RequestRefresh(_requestedLayerId = layerId);
			for (var i = 0; i < _layerButtons.Count; i++)
			{
				_layerButtons[i].Checked = i == layerId;
			}
		}

		public bool FitImage
		{
			get { return _scaleButton.Checked; }
			set { _scaleButton.Checked = value; }
		}
		public bool ShowScrollOverlay
		{
			get { return _viewportButton.Checked; }
			set { _viewportButton.Checked = value; }
		}

		private void _scaleButton_CheckedChanged(object sender, EventArgs e)
		{
			_tileMapDisplay.FitImage = _scaleButton.Checked;
			// Removes confusing focus from current button
			if (_scaleButton.Focused && _layerButtons.Count > _requestedLayerId) _layerButtons[_requestedLayerId].Focus();
		}

		private void _viewportButton_CheckedChanged(object sender, EventArgs e)
		{
			_tileMapDisplay.ShowScrollOverlay = _viewportButton.Checked;
			if (_viewportButton.Focused && _layerButtons.Count > _requestedLayerId) _layerButtons[_requestedLayerId].Focus();
		}
	}
}
