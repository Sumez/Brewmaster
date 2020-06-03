using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapGrid
	{
		private Pen[][] _pens;
		private Bitmap _grid;

		public MapGrid()
		{
			var vagueColor = Color.FromArgb(128, 255, 255, 255);
			var midColor = Color.FromArgb(160, 255, 255, 255);
			var solidColor = Color.FromArgb(255, 255, 255, 255);
			_pens = new []
			{
				new [] {new Pen(vagueColor, 1), new Pen(vagueColor, 1), new Pen(vagueColor, 1)},
				new [] {new Pen(midColor, 1), new Pen(solidColor, 1), new Pen(solidColor, 1)}
			};
			_pens[0][0].DashStyle = _pens[0][1].DashStyle = DashStyle.Custom;
			_pens[0][0].DashPattern = new float[] { 1, 3 };
			_pens[0][1].DashPattern = new float[] { 2, 2 };
		}

		public int Factor = 32;
		public static int PixelStep = 14;

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
			Factor = zoom > PixelStep ? 4 : 32;
			var gridTile = new Bitmap(tileWidth * Factor, tileHeight * Factor, PixelFormat.Format32bppPArgb);
			{
				using (var tileGraphics = Graphics.FromImage(gridTile))
				{
					tileGraphics.CompositingMode = CompositingMode.SourceCopy;
					tileGraphics.CompositingQuality = CompositingQuality.HighSpeed;

					if (zoom > PixelStep)
						for (var i = 0; i < Factor; i++)
						{
							for (var j = 1; j < map.BaseTileSize.Width; j++)
							{
								tileGraphics.DrawLine(_pens[0][1], i * tileWidth + j * zoom, 0, i * tileWidth + j * zoom, tileHeight * Factor);
							}
							for (var j = 1; j < map.BaseTileSize.Height; j++)
							{
								tileGraphics.DrawLine(_pens[0][1], 0, i * tileHeight + j * zoom, tileWidth * Factor, i * tileHeight + j * zoom);
							}
						}

					for (var i = 0; i < Factor; i++)
					{
						if (zoom == 1 && i % 2 == 1) continue;

						var pen = _pens[zoom > PixelStep ? 1 : 0][i % 4 == 0 ? 2 : i % 2 == 0 ? 1 : 0];
						if (zoom <= PixelStep)
						{
							tileGraphics.DrawLine(Pens.Transparent, i * tileWidth, 0, i * tileWidth, tileHeight * Factor);
							tileGraphics.DrawLine(Pens.Transparent, 0, i * tileHeight, tileWidth * Factor, i * tileHeight);
						}
						tileGraphics.DrawLine(pen, i * tileWidth, 0, i * tileWidth, tileHeight * Factor);
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
