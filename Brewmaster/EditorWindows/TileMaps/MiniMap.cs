using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MiniMap : Control
	{
		private Brush _hoverBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
		private TileMap _map;
		public TileMap Map
		{
			get { return _map; }
			set { _map = value; PerformLayout(); }
		}
		public float Scale = 0.1f;
		private int _hoverX = -1;
		private int _hoverY = -1;
		public Action<int, int> ActivateScreen;
		protected override bool DoubleBuffered { get { return true; } }

		protected override void OnLayout(LayoutEventArgs levent)
		{
			if (Map == null) return;
			Width = (int)Math.Ceiling(Scale * Map.BaseTileSize.Width * Map.ScreenSize.Width * Map.Width);
			Height = (int)Math.Ceiling(Scale * Map.BaseTileSize.Height * Map.ScreenSize.Height * Map.Height);
			base.OnLayout(levent);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var screenWidth = Map.BaseTileSize.Width * Map.ScreenSize.Width * Scale;
			var screenHeight = Map.BaseTileSize.Height * Map.ScreenSize.Height * Scale;

			var x = (int)(e.Location.X / screenWidth);
			var y = (int)(e.Location.Y / screenHeight);

			if (x < 0 || x >= Map.Width || y < 0 || y >= Map.Height)
			{
				_hoverY = _hoverX = -1;
			}
			else
			{
				_hoverX = x;
				_hoverY = y;
			}
			base.OnMouseMove(e);
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_hoverY = _hoverX = -1;
			base.OnMouseLeave(e);
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left) return;
			if (_hoverX >= 0 && _hoverY >= 0 && ActivateScreen != null) ActivateScreen(_hoverX, _hoverY);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Map == null) return;
			/*
			var t0 = Width / (float) (Map.Width * Map.ScreenSize.Width);
			var t1 = Height / (float)(Map.Height * Map.ScreenSize.Height);

			var minTileSize = 1;
			var t = (int)Math.Max(minTileSize, Math.Min(t0, t1));
			*/
			var screenWidth = Map.BaseTileSize.Width * Map.ScreenSize.Width * Scale;
			var screenHeight = Map.BaseTileSize.Height * Map.ScreenSize.Height * Scale;

			e.Graphics.Clear(Color.Transparent);
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.FillRectangle(Brushes.Black, 0, 0, screenWidth * Map.Width, screenHeight * Map.Height);
			for (var y = 0; y < Map.Height; y++)
			{
				if (Map.Screens.Count <= y) break;
				var row = Map.Screens[y];
				for (var x = 0; x < Map.Width; x++)
				{
					if (row.Count <= x) break;
					var screen = row[x];
					if (screen == null || screen.Image == null) continue;

					// TODO: Draw grid of scaled screen images to a backbuffer
					lock (screen.TileDrawLock) e.Graphics.DrawImage(screen.Image, x * screenWidth, y * screenHeight, screenWidth, screenHeight);
				}
			}

			if (_hoverX >= 0 && _hoverY >= 0)
			{
				e.Graphics.CompositingMode = CompositingMode.SourceOver;
				e.Graphics.FillRectangle(_hoverBrush, _hoverX * screenWidth, _hoverY * screenHeight, screenWidth, screenHeight);
			}
		}

		public void UpdateScreenImage(int x, int y)
		{
			var screenWidth = Map.BaseTileSize.Width * Map.ScreenSize.Width * Scale;
			var screenHeight = Map.BaseTileSize.Height * Map.ScreenSize.Height * Scale;
			Invalidate(new Region(new RectangleF(screenWidth * x, screenHeight * y, screenWidth, screenHeight)));
		}
	}
}
