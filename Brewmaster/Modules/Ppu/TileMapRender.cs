using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Brewmaster.Emulation;

namespace Brewmaster.Modules.Ppu
{
	public class TileMapRender : ScaledImageRenderer
	{
		private TileMapData _nametableData;
		private readonly Bitmap _scrollOverlay = new Bitmap(512, 480);

		public bool ShowScrollOverlay
		{
			get { return _showScrollOverlay; }
			set { _showScrollOverlay = value; RefreshImage(); }
		}

		public void UpdateNametableData(TileMapData data)
		{
			_nametableData = data;
			RefreshImage();
		}

		private bool _showScrollOverlay;

		protected override void DrawBackBuffer(Func<Graphics> getGraphics)
		{
			if (_nametableData == null) return;
			//int tileIndexOffset = _state.PPU.ControlFlags.BackgroundPatternAddr == 0x1000 ? 256 : 0;
			//lblMirroringType.Text = ResourceHelper.GetEnumText(_state.Cartridge.Mirroring);

			//var target = this.Image;
			//var target = new Bitmap(512, 480);
			//_nametableImage = new Bitmap(512, 480);

			var fullWidth = _nametableData.NumberOfMaps > 1 ? _nametableData.MapWidth * 2 : _nametableData.MapWidth;
			var fullHeight = _nametableData.NumberOfMaps > 1 ? _nametableData.MapHeight * _nametableData.NumberOfMaps / 2 : _nametableData.MapHeight;
			if (ImageWidth != fullWidth || ImageHeight != fullHeight)
			{
				SetImageSize(fullWidth, fullHeight);
			}

			lock (BackBufferLock)
			using (var graphics = getGraphics())
			{
				graphics.Clear(Color.Transparent);
				var width = _nametableData.MapWidth;
				var height = _nametableData.MapHeight;
				for (int i = 0; i < _nametableData.NumberOfMaps; i++)
				{
					var handle = GCHandle.Alloc(_nametableData.PixelData[i], GCHandleType.Pinned);
					using (var source = new Bitmap(width, height, _nametableData.DataWidth, System.Drawing.Imaging.PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject()))
					try
					{
						graphics.DrawImage(source, new Rectangle(i % 2 == 0 ? 0 : width, i <= 1 ? 0 : height, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
					}
					finally
					{
						handle.Free();
					}
				}
				if (_showScrollOverlay) DrawScrollOverlay(_nametableData.ScrollX, _nametableData.ScrollY, graphics);

			}
			/*
			if (this._gridOverlay == null && (chkShowTileGrid.Checked || chkShowAttributeGrid.Checked))
			{
				this._gridOverlay = new Bitmap(512, 480);

				using (Graphics overlay = Graphics.FromImage(this._gridOverlay))
				{
					if (chkShowTileGrid.Checked)
					{
						using (Pen pen = new Pen(Color.FromArgb(chkShowAttributeGrid.Checked ? 120 : 180, 240, 100, 120)))
						{
							if (chkShowAttributeGrid.Checked)
							{
								pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
							}
							DrawGrid(overlay, pen, 1);
						}
					}

					if (chkShowAttributeGrid.Checked)
					{
						using (Pen pen = new Pen(Color.FromArgb(180, 80, 130, 250)))
						{
							DrawGrid(overlay, pen, 2);
						}
					}
				}
			}
			*/
		}
		private int _lastX;
		private int _lastY;
		private void DrawScrollOverlay(int xScroll, int yScroll, Graphics ntGraphics)
		{
			if (_lastX != xScroll || _lastY != yScroll)
			using (var g = Graphics.FromImage(_scrollOverlay))
			{
				var width = _nametableData.ViewportWidth;
				var height = _nametableData.ViewportHeight;

				g.CompositingMode = CompositingMode.SourceCopy;
				g.Clear(Color.FromArgb(90, 100, 120, 75));
				using (var brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
				{
					g.FillRectangle(brush, xScroll, yScroll, width, height);
					if (xScroll + width >= ImageWidth)
					{
						g.FillRectangle(brush, xScroll - width, yScroll, width, height);
					}

					if (yScroll + height >= ImageHeight)
					{
						g.FillRectangle(brush, xScroll, yScroll - ImageHeight, width, height);
					}

					if (xScroll + width >= ImageWidth && yScroll + height >= ImageHeight)
					{
						g.FillRectangle(brush, xScroll - width, yScroll - ImageHeight, width, height);
					}
				}

				using (var pen = new Pen(Color.FromArgb(230, 150, 150, 150), 2))
				{
					g.DrawRectangle(pen, xScroll, yScroll, width, height);
					if (xScroll + width >= ImageWidth)
					{
						g.DrawRectangle(pen, xScroll - width, yScroll, width, height);
					}

					if (yScroll + height >= ImageHeight)
					{
						g.DrawRectangle(pen, xScroll, yScroll - ImageHeight, width, height);
					}

					if (xScroll + width >= ImageWidth && yScroll + height >= ImageHeight)
					{
						g.DrawRectangle(pen, xScroll - width, yScroll - ImageHeight, width, height);
					}
				}
			}
			ntGraphics.DrawImage(_scrollOverlay, 0, 0);
			_lastY = yScroll;
			_lastX = xScroll;
		}

	}
}
