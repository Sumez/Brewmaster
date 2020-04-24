using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Brewmaster.Emulation;
using Brewmaster.Ide;

namespace Brewmaster.Ppu
{
	public class NametableDisplay : Panel, IHeaderManupulator
	{
		private NametableData _nametableData;
		private readonly Bitmap _nametableImage = new Bitmap(512, 480);
		private readonly Bitmap _scrollOverlay = new Bitmap(512, 480);
		private readonly PictureBox _pictureBox;
		private Object _backBufferLock = new Object();
		private float _scale = 1;
		private int _offsetX = 0;
		private int _offsetY = 0;
		private readonly CheckBox _scaleToggle;
		private readonly CheckBox _scrollToggle;

		public NametableDisplay()
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

			_scaleToggle = new CheckBox();
			_scaleToggle.Appearance = Appearance.Button;
			_scaleToggle.Text = "scale";
			_scaleToggle.Dock = DockStyle.Right;
			_scaleToggle.Width = 40;
			_scaleToggle.FlatStyle = FlatStyle.Popup;

			_scrollToggle = new CheckBox();
			_scrollToggle.Appearance = Appearance.Button;
			_scrollToggle.Text = "viewport";
			_scrollToggle.Dock = DockStyle.Right;
			_scrollToggle.Width = 60;
			_scrollToggle.FlatStyle = FlatStyle.Popup;
			_scrollToggle.Checked = true;

			_scaleToggle.CheckedChanged += (sender, args) => RepositionImage();
			_scrollToggle.CheckedChanged += (sender, args) => RefreshImage();
		}

		private HeaderPanel _header;
		public HeaderPanel Header
		{
			set
			{
				if (_header != null)
				{
					_header.Controls.Remove(_scaleToggle);
					_header.Controls.Remove(_scrollToggle);
				}
				_header = value;
				if (_header != null)
				{
					_header.Controls.Add(_scaleToggle);
					_header.Controls.Add(_scrollToggle);
				}
			}
			get { return _header; }
		}

		public void UpdateNametableData(NametableData data)
		{
			_nametableData = data;
			RefreshImage();
		}

		private void ScaleImage()
		{
			lock (_backBufferLock)
			using (var g = Graphics.FromImage(_pictureBox.Image))
			{
				g.CompositingMode = CompositingMode.SourceCopy;
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.PixelOffsetMode = PixelOffsetMode.None;
				g.SmoothingMode = SmoothingMode.None;
				g.Clear(Color.Black);
				if (_scaleToggle.Checked) {
					var scaledRectangle = new Rectangle(Math.Max(0, -_offsetX), Math.Max(0, -_offsetY), (int)(512 * _scale), (int)(480 * _scale));
					g.InterpolationMode = InterpolationMode.Low;
					g.DrawImage(_nametableImage, scaledRectangle);
				}
				else
				{
					g.DrawImage(_nametableImage, 0, 0);
				}
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			RepositionImage();
		}

		private void RepositionImage()
		{
			_scale = 1f;

			if (_scaleToggle.Checked)
			{
				var t = Width / 512f;
				if (t < _scale) _scale = t;
				t = Height / 480f;
				if (t < _scale) _scale = t;
			}
			_offsetX = (int)(Width - 512 * _scale) / 2;
			_offsetY = (int)(Height - 480 * _scale) / 2;

			ScaleImage();
			_pictureBox.Left = _offsetX;
			_pictureBox.Top = _offsetY;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

		}

		public void RefreshImage()
		{
			//int tileIndexOffset = _state.PPU.ControlFlags.BackgroundPatternAddr == 0x1000 ? 256 : 0;
			//lblMirroringType.Text = ResourceHelper.GetEnumText(_state.Cartridge.Mirroring);

			//var target = this.Image;
			//var target = new Bitmap(512, 480);
			//_nametableImage = new Bitmap(512, 480);

			lock (_backBufferLock)
			using (var gNametable = Graphics.FromImage(_nametableImage))
			{
				for (int i = 0; i < 4; i++)
				{
					var handle = GCHandle.Alloc(_nametableData.PixelData[i], GCHandleType.Pinned);
					using (var source = new Bitmap(256, 240, 4 * 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject()))
					try
					{
						gNametable.DrawImage(source, new Rectangle(i % 2 == 0 ? 0 : 256, i <= 1 ? 0 : 240, 256, 240), new Rectangle(0, 0, 256, 240), GraphicsUnit.Pixel);
					}
					finally
					{
						handle.Free();
					}
				}
				if (_scrollToggle.Checked) DrawScrollOverlay(_nametableData.ScrollX, _nametableData.ScrollY, gNametable);
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
				g.CompositingMode = CompositingMode.SourceCopy;
				g.Clear(Color.FromArgb(90, 100, 120, 75));
				using (var brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
				{
					g.FillRectangle(brush, xScroll, yScroll, 256, 240);
					if (xScroll + 256 >= 512)
					{
						g.FillRectangle(brush, 0, yScroll, xScroll - 256, 240);
					}

					if (yScroll + 240 >= 480)
					{
						g.FillRectangle(brush, xScroll, 0, 256, yScroll - 240);
					}

					if (xScroll + 256 >= 512 && yScroll + 240 >= 480)
					{
						g.FillRectangle(brush, 0, 0, xScroll - 256, yScroll - 240);
					}
				}

				using (var pen = new Pen(Color.FromArgb(230, 150, 150, 150), 2))
				{
					g.DrawRectangle(pen, xScroll, yScroll, 256, 240);
					if (xScroll + 256 >= 512)
					{
						g.DrawRectangle(pen, 0, yScroll, xScroll - 256, 240);
					}

					if (yScroll + 240 >= 480)
					{
						g.DrawRectangle(pen, xScroll, 0, 256, yScroll - 240);
					}

					if (xScroll + 256 >= 512 && yScroll + 240 >= 480)
					{
						g.DrawRectangle(pen, 0, 0, xScroll - 256, yScroll - 240);
					}
				}
			}
			ntGraphics.DrawImage(_scrollOverlay, 0, 0);
		}

	}
}
