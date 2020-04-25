using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Brewmaster.Emulation;

namespace Brewmaster.Ppu
{
	public class TileMapRender : Panel
	{
		private TileMapData _nametableData;
		private Bitmap _nametableImage = new Bitmap(512, 480);
		private Bitmap _scrollOverlay = new Bitmap(512, 480);
		private readonly PictureBox _pictureBox;
		private Object _backBufferLock = new Object();
		private float _scale = 1;
		private int _offsetX = 0;
		private int _offsetY = 0;

		public TileMapRender()
		{
			BackColor = Color.Black;
			//BackgroundImage = new Bitmap(512, 480);
			//BackgroundImageLayout = ImageLayout.Center;
			_pictureBox = new PictureBox();
			_pictureBox.Image = new Bitmap(512, 480);
//			_pictureBox.Dock = DockStyle.Fill;
			_pictureBox.Width = 512;
			_pictureBox.Height = 480;
			//_pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			Controls.Add(_pictureBox);
			//DoubleBuffered = true;

			AutoScroll = true;
		}

		public bool FitImage
		{
			get { return _fitImage; }
			set { _fitImage = value; RepositionImage(); }
		}
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

		private void ScaleImage()
		{
			if (_nametableData == null) return;

			var image = new Bitmap(_width, _height); // Create new temporary image to prevent errors when drawing a new image while the old is being drawn to screen
			lock (_backBufferLock)
			using (var g = Graphics.FromImage(image))
			{
				g.CompositingMode = CompositingMode.SourceCopy;
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.PixelOffsetMode = PixelOffsetMode.None;
				g.SmoothingMode = SmoothingMode.None;
				g.Clear(Color.Black);
				if (_fitImage) {
					var scaledRectangle = new Rectangle(Math.Max(0, -_offsetX), Math.Max(0, -_offsetY), (int)(_width * _scale), (int)(_height * _scale));
					g.InterpolationMode = InterpolationMode.Low;
					g.DrawImage(_nametableImage, scaledRectangle);
				}
				else
				{
					g.DrawImage(_nametableImage, 0, 0);
				}
			}
			var oldImage = _pictureBox.Image;
			_pictureBox.Image = image;
			oldImage.Dispose();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			RepositionImage();
			base.OnClientSizeChanged(e);
			PerformLayout();
		}

		private void RepositionImage()
		{
			_scale = 1f;

			if (_fitImage)
			{
				var t = Width / (float)_width;
				if (t < _scale) _scale = t;
				t = Height / (float)_height;
				if (t < _scale) _scale = t;
			}
			_offsetX = (int)(Width - _width * _scale) / 2;
			_offsetY = (int)(Height - _height * _scale) / 2;

			ScaleImage();
			//_pictureBox.Left = Math.Max(0, _offsetX);
			//_pictureBox.Top = Math.Max(0, _offsetY);

			_pictureBox.Width = (int)(_width * _scale);
			_pictureBox.Height = (int)(_height * _scale);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		private int _width = 512;
		private int _height = 480;
		private bool _fitImage;
		private bool _showScrollOverlay;

		public void RefreshImage()
		{
			if (_nametableData == null) return;
			//int tileIndexOffset = _state.PPU.ControlFlags.BackgroundPatternAddr == 0x1000 ? 256 : 0;
			//lblMirroringType.Text = ResourceHelper.GetEnumText(_state.Cartridge.Mirroring);

			//var target = this.Image;
			//var target = new Bitmap(512, 480);
			//_nametableImage = new Bitmap(512, 480);

			var fullWidth = _nametableData.NumberOfMaps > 1 ? _nametableData.MapWidth * 2 : _nametableData.MapWidth;
			var fullHeight = _nametableData.NumberOfMaps > 1 ? _nametableData.MapHeight * _nametableData.NumberOfMaps / 2 : _nametableData.MapHeight;
			if (_width != fullWidth || _height != fullHeight)
			{
				_nametableImage.Dispose();
				_width = fullWidth;
				_height = fullHeight;
				_nametableImage = new Bitmap(_width, _height);
				BeginInvoke(new Action(RepositionImage));
			}

			lock (_backBufferLock)
			using (var graphics = Graphics.FromImage(_nametableImage))
			{
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
			ScaleImage();
			/*
			using (var g = Graphics.FromImage(target))
			{
				g.DrawImage(_nametableImage, 0, 0);

				for (var i = 0; i < 4; i++)
				{
					if (_chrViewer.SelectedTileIndex >= 0 && this.chkHighlightChrTile.Checked)
					{
						HighlightChrViewerTile(tileIndexOffset, g, i);
					}
				}

				if (this._gridOverlay != null)
				{
					g.DrawImage(this._gridOverlay, 0, 0);
				}

				if (chkShowPpuScrollOverlay.Checked)
				{
					DrawScrollOverlay(_xScroll, _yScroll, g);
				}

				if (chkHighlightAttributeUpdates.Checked || chkHighlightTileUpdates.Checked)
				{
					DrawEditHighlights(g);
				}
			}
			*/

			//Image = target;

			_pictureBox.Invalidate();
		}
		private void DrawScrollOverlay(int xScroll, int yScroll, Graphics ntGraphics)
		{
			using (var g = Graphics.FromImage(_scrollOverlay))
			{
				var width = _nametableData.ViewportWidth;
				var height = _nametableData.ViewportHeight;

				g.CompositingMode = CompositingMode.SourceCopy;
				g.Clear(Color.FromArgb(90, 100, 120, 75));
				using (var brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
				{
					g.FillRectangle(brush, xScroll, yScroll, width, height);
					if (xScroll + width >= _width)
					{
						g.FillRectangle(brush, xScroll - width, yScroll, width, height);
					}

					if (yScroll + height >= _height)
					{
						g.FillRectangle(brush, xScroll, yScroll - _height, width, height);
					}

					if (xScroll + width >= _width && yScroll + height >= _height)
					{
						g.FillRectangle(brush, xScroll - width, yScroll - _height, width, height);
					}
				}

				using (var pen = new Pen(Color.FromArgb(230, 150, 150, 150), 2))
				{
					g.DrawRectangle(pen, xScroll, yScroll, width, height);
					if (xScroll + width >= _width)
					{
						g.DrawRectangle(pen, xScroll - width, yScroll, width, height);
					}

					if (yScroll + height >= _height)
					{
						g.DrawRectangle(pen, xScroll, yScroll - _height, width, height);
					}

					if (xScroll + width >= _width && yScroll + height >= _height)
					{
						g.DrawRectangle(pen, xScroll - width, yScroll - _height, width, height);
					}
				}
			}
			ntGraphics.DrawImage(_scrollOverlay, 0, 0);
		}

	}
}
