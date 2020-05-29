using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapGrid
	{
		private Pen _dotted;
		private Pen _dashed;
		private Pen _solid;
		private Bitmap _grid;

		public MapGrid()
		{
			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_dotted = new Pen(gridColor, 1);
			_dotted.DashStyle = DashStyle.Custom;
			_dotted.DashPattern = new float[] { 1, 3 };

			_dashed = new Pen(gridColor, 1);
			_dashed.DashStyle = DashStyle.Custom;
			_dashed.DashPattern = new float[] { 2, 2 };

			_solid = new Pen(gridColor, 1);
		}

		public static int Factor = 32;
		public void GenerateGrid(TileMap map, int zoom)
		{
			var width = map.ScreenSize.Width * map.BaseTileSize.Width * zoom;
			var height = map.ScreenSize.Height * map.BaseTileSize.Height * zoom;
			var tileWidth = map.BaseTileSize.Width * zoom;
			var tileHeight = map.BaseTileSize.Height * zoom;

			/*var grid = new Bitmap(zoom * map.ScreenSize.Width * map.BaseTileSize.Width, zoom * map.ScreenSize.Height * map.BaseTileSize.Height);
			using (var graphics = Graphics.FromImage(grid))
			{
			*/
				//using (var gridTile = new Bitmap(tileWidth * Factor, tileHeight * Factor))
			var gridTile = new Bitmap(tileWidth * Factor, tileHeight * Factor, PixelFormat.Format32bppPArgb);
			{
				using (var tileGraphics = Graphics.FromImage(gridTile))
				{
					tileGraphics.CompositingMode = CompositingMode.SourceCopy;
					tileGraphics.CompositingQuality = CompositingQuality.HighSpeed;
					for (var i = 0; i < Factor; i++)
					{
						if (zoom == 1 && i % 2 == 1) continue;
						var pen = i % 4 == 0 ? _solid : i % 2 == 0 ? _dashed : _dotted;
						tileGraphics.DrawLine(pen, i * tileWidth, 0, i * tileWidth, tileHeight * Factor);
					}

					for (var i = 0; i < Factor; i++)
					{
						if (zoom == 1 && i % 2 == 1) continue;
						var pen = i % 4 == 0 ? _solid : i % 2 == 0 ? _dashed : _dotted;
						tileGraphics.DrawLine(pen, 0, i * tileHeight, tileWidth * Factor, i * tileHeight);
					}
				}
				/*
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				for (var x = 0; x < map.ScreenSize.Width; x += Factor)
				{
					for (var y = 0; y < map.ScreenSize.Height; y += Factor)
					{
						graphics.DrawImageUnscaled(gridTile, x * tileWidth, y * tileHeight);
					}
				}*/
				if (_grid != null) _grid.Dispose();
				_grid = gridTile;
			}

		}

		public void Draw(Graphics graphics, int x, int y)
		{
			graphics.DrawImageUnscaled(_grid, x, y);
		}
	}
}
