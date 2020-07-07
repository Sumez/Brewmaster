using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapOverview : UserControl
	{
		public event Action MapSizeChanged;
		public MapOverview(TileMap map)
		{
			_map = map;
			InitializeComponent();
			_miniMap.Map = map;
			_miniMap.Invalidate();

			UpdateDataView();

			_mapSizeButton.LinkClicked += (s, a) => { ChangeMapSize(); };
			_screenSizeButton.LinkClicked += (s, a) => { ChangeScreenSize(); };
		}

		private bool ValidateMapResize(int width, int height, Anchor anchor)
		{
			if (width >= _map.Width && height >= _map.Height) return true;
			return MessageBox.Show("Screens will be deleted when shrinking the global map size.\nThese cannot be recovered!", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
		}
		private bool ValidateScreenResize(int width, int height, Anchor anchor)
		{
			var deltaW = width - _map.ScreenSize.Width;
			var deltaH = height - _map.ScreenSize.Height;
			if (anchor.HasFlag(TileMaps.Anchor.E))
			{
				if (deltaW % _map.AttributeSize.Width != 0) return RefuseSizeChange(string.Format("Cropping on the left side must be a multiple of the attribute width ({0})", _map.AttributeSize.Width));
				if (deltaW % _map.MetaValueSize.Width != 0) return RefuseSizeChange(string.Format("Cropping on the left side must be a multiple of the collision resolution ({0})", _map.MetaValueSize.Width));
			}

			if (anchor.HasFlag(TileMaps.Anchor.S))
			{
				if (deltaH % _map.AttributeSize.Height != 0) return RefuseSizeChange(string.Format("Cropping on the top must be a multiple of the attribute height ({0})", _map.AttributeSize.Height));
				if (deltaH % _map.MetaValueSize.Height != 0) return RefuseSizeChange(string.Format("Cropping on the top must be a multiple of the collision resolution ({0})", _map.MetaValueSize.Height));
			}

			if (width >= _map.ScreenSize.Width && height >= _map.ScreenSize.Height) return true;
			return MessageBox.Show("Tiles outside the new screen size will be deleted.\nThese cannot be recovered!", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
		}

		private bool RefuseSizeChange(string message)
		{
			MessageBox.Show(message, "Cannot change size", MessageBoxButtons.OK);
			return false;
		}

		private void ChangeMapSize()
		{
			using (var window = new ResizeWindow(_map.Width, _map.Height, ValidateMapResize))
			{
				if (window.ShowDialog(this) != DialogResult.OK) return;
				var newWidth = window.SetWidth;
				var newHeight = window.SetHeight;
				if (window.ResizeAnchor.HasFlag(TileMaps.Anchor.E))
				{
					foreach (var screenRow in _map.Screens)
					{
						for (var i = _map.Width; i < newWidth; i++) screenRow.Insert(0, null);
						for (var i = _map.Width; i > newWidth; i--)
						{
							if (screenRow[0] != null) screenRow[0].Unload();
							screenRow.RemoveAt(0);
						}
					}
				}
				else
				{
					foreach (var screenRow in _map.Screens)
					{
						while (screenRow.Count > newWidth)
						{
							if (screenRow[screenRow.Count - 1] != null) screenRow[screenRow.Count - 1].Unload();
							screenRow.RemoveAt(screenRow.Count - 1);
						}
					}
				}

				if (window.ResizeAnchor.HasFlag(TileMaps.Anchor.S))
				{
					for (var i = _map.Height ; i < newHeight; i++) _map.Screens.Insert(0, new List<TileMapScreen>());
					for (var i = _map.Height; i > newHeight; i--)
					{
						foreach (var screen in _map.Screens[0].Where(s => s != null)) screen.Unload();
						_map.Screens.RemoveAt(0);
					}
				}
				else
				{
					while (_map.Screens.Count > newHeight)
					{
						foreach (var screen in _map.Screens[_map.Screens.Count - 1].Where(s => s != null)) screen.Unload();
						_map.Screens.RemoveAt(_map.Screens.Count - 1);
					}
				}
				_map.Width = newWidth;
				_map.Height = newHeight;

				if (MapSizeChanged != null) MapSizeChanged();
				UpdateDataView();
				_miniMap.PerformLayout();
				_miniMap.Invalidate();
			}
		}
		private void ChangeScreenSize()
		{
			using (var window = new ResizeWindow(_map.ScreenSize.Width, _map.ScreenSize.Height, ValidateScreenResize))
			{
				if (window.ShowDialog(this) != DialogResult.OK) return;
				foreach (var screen in _map.GetAllScreens())
				{
					ResizeTileMap(ref screen.Tiles, _map.ScreenSize.Width, _map.ScreenSize.Height, window.SetWidth, window.SetHeight, window.ResizeAnchor);
					ResizeTileMap(ref screen.ColorAttributes,
						_map.ScreenSize.Width / _map.AttributeSize.Width, _map.ScreenSize.Height / _map.AttributeSize.Height, 
						window.SetWidth / _map.AttributeSize.Width, window.SetHeight / _map.AttributeSize.Height, window.ResizeAnchor);
					ResizeTileMap(ref screen.MetaValues,
						_map.ScreenSize.Width / _map.MetaValueSize.Width, _map.ScreenSize.Height / _map.MetaValueSize.Height,
						window.SetWidth / _map.MetaValueSize.Width, window.SetHeight / _map.MetaValueSize.Height, window.ResizeAnchor);

					screen.Image = new FastBitmap(window.SetWidth * _map.BaseTileSize.Width, window.SetHeight * _map.BaseTileSize.Height, PixelFormat.Format32bppPArgb);
					screen.MetaImage = new FastBitmap(window.SetWidth / _map.MetaValueSize.Width, window.SetHeight / _map.MetaValueSize.Height, PixelFormat.Format32bppArgb);
				}
				_map.ScreenSize = new Size(window.SetWidth, window.SetHeight);

				if (MapSizeChanged != null) MapSizeChanged();
				UpdateDataView();
				_miniMap.PerformLayout();
				_miniMap.Invalidate();
			}
		}

		private void ResizeTileMap(ref int[] tiles, int oldWidth, int oldHeight, int newWidth, int newHeight, Anchor anchor)
		{
			var deltaWidth = newWidth - oldWidth;
			var deltaHeight = newHeight - oldHeight;

			var newTiles = new int[newWidth * newHeight];
			var sourceIndex = anchor.HasFlag(TileMaps.Anchor.E) ? Math.Max(0, -deltaWidth) : 0;
			var targetIndex = anchor.HasFlag(TileMaps.Anchor.E) ? Math.Max(0, deltaWidth) : 0;

			if (anchor.HasFlag(TileMaps.Anchor.S) && deltaHeight > 0) targetIndex += deltaHeight * newWidth;
			if (anchor.HasFlag(TileMaps.Anchor.S) && deltaHeight < 0) sourceIndex -= deltaHeight * oldWidth;

			while (targetIndex < newTiles.Length && sourceIndex < tiles.Length)
			{
				Buffer.BlockCopy(tiles, sourceIndex * 4, newTiles, targetIndex * 4, Math.Min(oldWidth, newWidth) * 4);
				sourceIndex += oldWidth;
				targetIndex += newWidth;
			}

			tiles = newTiles;
		}


		public Action<int, int> ActivateScreen
		{
			get { return _miniMap.ActivateScreen; }
			set { _miniMap.ActivateScreen = value; }
		}

		private void UpdateDataView()
		{
			_mapSizeButton.Text = string.Format("{0}x{1}", _map.Width, _map.Height);
			_screenSizeButton.Text = string.Format("{0}x{1}", _map.ScreenSize.Width, _map.ScreenSize.Height);
		}
		private Panel _mapPanel;
		private MiniMap _miniMap;
		private LinkLabel _mapSizeButton;
		private LinkLabel _screenSizeButton;
		private readonly TileMap _map;

		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this._mapPanel = new System.Windows.Forms.Panel();
			this._miniMap = new Brewmaster.EditorWindows.TileMaps.MiniMap();
			this._mapSizeButton = new System.Windows.Forms.LinkLabel();
			this._screenSizeButton = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this._mapPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(52, 13);
			label1.TabIndex = 0;
			label1.Text = "Map size:";
			// 
			// _mapPanel
			// 
			this._mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._mapPanel.AutoScroll = true;
			this._mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._mapPanel.Controls.Add(this._miniMap);
			this._mapPanel.Location = new System.Drawing.Point(6, 16);
			this._mapPanel.Name = "_mapPanel";
			this._mapPanel.Size = new System.Drawing.Size(428, 285);
			this._mapPanel.TabIndex = 8;
			// 
			// _miniMap
			// 
			this._miniMap.Location = new System.Drawing.Point(0, 0);
			this._miniMap.Map = null;
			this._miniMap.Name = "_miniMap";
			this._miniMap.Size = new System.Drawing.Size(75, 23);
			this._miniMap.TabIndex = 0;
			this._miniMap.Text = "miniMap1";
			// 
			// _mapSizeButton
			// 
			this._mapSizeButton.AutoSize = true;
			this._mapSizeButton.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this._mapSizeButton.Location = new System.Drawing.Point(53, 1);
			this._mapSizeButton.Name = "_mapSizeButton";
			this._mapSizeButton.Size = new System.Drawing.Size(36, 13);
			this._mapSizeButton.TabIndex = 9;
			this._mapSizeButton.TabStop = true;
			this._mapSizeButton.Text = "20x20";
			// 
			// _screenSizeButton
			// 
			this._screenSizeButton.AutoSize = true;
			this._screenSizeButton.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this._screenSizeButton.Location = new System.Drawing.Point(158, 1);
			this._screenSizeButton.Name = "_screenSizeButton";
			this._screenSizeButton.Size = new System.Drawing.Size(36, 13);
			this._screenSizeButton.TabIndex = 11;
			this._screenSizeButton.TabStop = true;
			this._screenSizeButton.Text = "32x30";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(95, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(65, 13);
			label2.TabIndex = 10;
			label2.Text = "Screen size:";
			// 
			// MapOverview
			// 
			this.Controls.Add(this._screenSizeButton);
			this.Controls.Add(label2);
			this.Controls.Add(this._mapSizeButton);
			this.Controls.Add(this._mapPanel);
			this.Controls.Add(label1);
			this.Name = "MapOverview";
			this.Size = new System.Drawing.Size(437, 304);
			this._mapPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public void InvalidateScreenImage(TileMapScreen screen)
		{
			for (var y = 0; y < _map.Screens.Count; y++)
			{
				var x = _map.Screens[y].IndexOf(screen);
				if (x >= 0) _miniMap.InvalidateScreenImage(x, y);
			}
		}
	}
}
