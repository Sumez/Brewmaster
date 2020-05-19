using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapScreenView : Control
	{
		private readonly TileMap _map;
		private TileMapScreen _screen;
		private int _zoom;
		private Pen _dotted;
		private Pen _solid;
		private Pen _dashed;

		public MapScreenView(TileMap map, TileMapScreen screen)
		{
			Zoom = 2;
			_map = map;
			_screen = screen;

			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_dotted = new Pen(gridColor, 1);
			_dotted.DashStyle = DashStyle.Custom;
			_dotted.DashPattern = new float[] { 1, 3 };

			_dashed = new Pen(gridColor, 1);
			_dashed.DashStyle = DashStyle.Custom;
			_dashed.DashPattern = new float[] { 2, 2 };

			_solid = new Pen(gridColor, 1);
		}

		public int Zoom
		{
			get { return _zoom; }
			set { _zoom = value; Invalidate(); }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.DrawImage(_screen.Image, new Rectangle(0, 0, _screen.Image.Width * Zoom, _screen.Image.Height * Zoom));
			var width = _map.ScreenSize.Width * _map.BaseTileSize.Width * Zoom;
			var height = _map.ScreenSize.Height * _map.BaseTileSize.Height * Zoom;
			var tileWidth = _map.BaseTileSize.Width * Zoom;
			var tileHeight = _map.BaseTileSize.Height * Zoom;

			e.Graphics.CompositingMode = CompositingMode.SourceOver;
			for (var i = 1; i < _map.ScreenSize.Width; i++)
			{
				var pen = i % 4 == 0 ? _solid : i % 2 == 0 ? _dashed : _dotted;
				e.Graphics.DrawLine(pen, i * tileWidth, 0, i * tileWidth, height);
			}
			for (var i = 1; i < _map.ScreenSize.Height; i++)
			{
				var pen = i % 4 == 0 ? _solid : i % 2 == 0 ? _dashed : _dotted;
				e.Graphics.DrawLine(pen, 0, i * tileHeight, width, i * tileHeight);
			}
		}
	}
}
