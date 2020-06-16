using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Brewmaster.Modules.Ppu
{
	public abstract class ScaledImageRenderer : Panel
	{
		public int ImageWidth { get; private set; } = 512;
		public int ImageHeight { get; private set; } = 480;
		protected Object BackBufferLock = new Object();

		private bool _fitImage;
		private Bitmap _backBuffer = new Bitmap(512, 480, PixelFormat.Format32bppPArgb);
		private readonly PictureBox _pictureBox;
		private float _scale = 1;
		private int _offsetX = 0;
		private int _offsetY = 0;

		protected ScaledImageRenderer()
		{
			_pictureBox = new PictureBox();
			_pictureBox.Image = new Bitmap(512, 480, PixelFormat.Format32bppPArgb);
			_pictureBox.Width = 512;
			_pictureBox.Height = 480;
			Controls.Add(_pictureBox);
			_pictureBox.MouseDown += (s, a) => ClickedPicture(a);

			AutoScroll = true;
		}

		public bool FitImage
		{
			get { return _fitImage; }
			set { _fitImage = value; RepositionImage(); }
		}

		private void ScaleImage()
		{
			if (_backBuffer == null) return;

			BackColor = SystemColors.ControlDarkDark;
			var image = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppPArgb); // Create new temporary image to prevent errors when drawing a new image while the old is being drawn to screen
			lock (BackBufferLock)
			using (var g = Graphics.FromImage(image))
			{
				g.CompositingMode = CompositingMode.SourceCopy;
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.PixelOffsetMode = PixelOffsetMode.None;
				g.SmoothingMode = SmoothingMode.None;
				g.Clear(Color.Black);
				if (_fitImage) {
					var scaledRectangle = new Rectangle(Math.Max(0, -_offsetX), Math.Max(0, -_offsetY), (int)(ImageWidth * _scale), (int)(ImageHeight * _scale));
					g.InterpolationMode = InterpolationMode.Low;
					g.DrawImage(_backBuffer, scaledRectangle);
				}
				else
				{
					g.DrawImage(_backBuffer, 0, 0);
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
				var t = Width / (float)ImageWidth;
				if (t < _scale) _scale = t;
				t = Height / (float)ImageHeight;
				if (t < _scale) _scale = t;
			}
			_offsetX = Math.Max(0, (int)(Width - ImageWidth * _scale) / 2);
			_offsetY = Math.Max(0, (int)(Height - ImageHeight * _scale) / 2);

			ScaleImage();
			_pictureBox.Left = _offsetX + AutoScrollPosition.X;
			_pictureBox.Top = _offsetY + AutoScrollPosition.Y;

			_pictureBox.Width = (int)(ImageWidth * _scale);
			_pictureBox.Height = (int)(ImageHeight * _scale);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		protected void ClickedPicture(MouseEventArgs e)
		{
			var location = new Point((int)(e.X / _scale), (int)(e.Y / _scale));
			//location.Offset(-_pictureBox.Left, -_pictureBox.Top);
			OnMouseDownScaled(new MouseEventArgs(e.Button, e.Clicks, location.X, location.Y, 0));
		}
		protected virtual void OnMouseDownScaled(MouseEventArgs e) { }
		public void SetImageSize(int width, int height)
		{
			_backBuffer.Dispose();
			ImageWidth = width;
			ImageHeight = height;
			_backBuffer = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format32bppPArgb);
			if (InvokeRequired) BeginInvoke(new Action(RepositionImage));
			else RepositionImage();
		}

		public void RefreshImage()
		{
			if (!Visible) return;
			DrawBackBuffer(() => Graphics.FromImage(_backBuffer));
			ScaleImage();
			if (InvokeRequired) BeginInvoke(new Action(Refresh));
			else Refresh();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (Visible) RefreshImage();
		}

		protected abstract void DrawBackBuffer(Func<Graphics> getGraphics);
	}
}
