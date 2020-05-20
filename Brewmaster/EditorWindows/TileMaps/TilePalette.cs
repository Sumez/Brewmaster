using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TilePalette : Control
	{
		public List<Color> Palette { get; set; }
		public int Zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = value;
				GenerateGrid();
				Invalidate();
			}
		}

		private void GenerateGrid()
		{
			var width = 128 * Zoom;
			var height = 128 * Zoom;
			var tileWidth = 8 * Zoom;
			var tileHeight = 8 * Zoom;

			var grid = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(grid))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				for (var i = 1; i < 16; i++)
				{
					graphics.DrawLine(_solid, i * tileWidth, 0, i * tileWidth, height);
				}
				for (var i = 1; i < 16; i++)
				{
					graphics.DrawLine(_solid, 0, i * tileHeight, width, i * tileHeight);
				}
			}
			
			if (_grid != null) _grid.Dispose();
			_grid = grid;
		}

		private Image _image = new Bitmap(128, 128);
		private byte[] _chrData;
		private Pen _solid;
		private int _zoom;
		private Bitmap _grid;
		private int _selectedTile = -1;

		public byte[] ChrData
		{
			get { return _chrData; }
			set {
				_chrData = value;
				RefreshImage();
			}
		}

		public int SelectedTile
		{
			get { return _selectedTile; }
			set
			{
				_selectedTile = value;
				Invalidate();
			}
		}

		public TilePalette()
		{
			DoubleBuffered = true;
			Palette = new List<Color>(new [] { Color.Blue, Color.DarkGray, Color.Brown, Color.Black });
			/*Palette = new List<Color>(new[] { Color.Black,
				Color.SaddleBrown, Color.Brown, Color.Red, Color.Orange,
				Color.DarkSlateGray, Color.DarkCyan, Color.DodgerBlue, Color.Cyan,
				Color.DarkBlue, Color.DodgerBlue, Color.DeepSkyBlue, Color.LightSeaGreen, Color.SaddleBrown, Color.SandyBrown, Color.Bisque
			});*/
			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_solid = new Pen(gridColor, 1);
			Zoom = 2;
		}

		private void RefreshImage()
		{
			using (var graphics = Graphics.FromImage(_image))
			{
				graphics.Clear(Palette[0]);

				for (var i = 0; i < 256; i++)
				{
					using (var tile = GetTileImage(i))
					{
						if (tile == null) continue;
						graphics.DrawImageUnscaled(tile, (i % 16) * 8, (i / 16) * 8);
					}
				}
			}
			Invalidate();
		}

		public Bitmap GetTileImage(int index)
		{
			return GetTileImage(ChrData, index, Palette);
		}

		public static Bitmap GetTileImage(byte[] data, int index, List<Color> palette, int bitDepth = 2, ProjectType projectType = ProjectType.Nes)
		{
			var tileSize = 8 * bitDepth;
			var offset = index * tileSize;
			if (data.Length < offset + tileSize) return null;

			var tile = new Bitmap(8, 8);
			for (var y = 0; y < 8; y++)
			{
				for (var x = 0; x < 8; x++)
				{
					var colorIndex = 0;
					for (var j = 0; j <= (bitDepth / 2); j += 2)
					{
						var byte0Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 : y);
						var byte1Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 + 1 : y + 8);

						var byte0 = data[offset + byte0Index];
						var byte1 = data[offset + byte1Index];

						var bit0 = (byte0 >> (7 - x)) & 1;
						var bit1 = (byte1 >> (7 - x)) & 1;

						colorIndex |= bit0 << j;
						colorIndex |= bit1 << (j + 1);
					}
					tile.SetPixel(x, y, palette[colorIndex]);
				}
			}

			return tile;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.DrawImage(_image, new Rectangle(0, 0, _image.Width * Zoom, _image.Height * Zoom));

			e.Graphics.CompositingMode = CompositingMode.SourceOver;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			e.Graphics.DrawImageUnscaled(_grid, 0, 0);

			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			if (_selectedTile >= 0)
			{
				var x = _selectedTile % 16;
				var y = _selectedTile / 16;
				e.Graphics.DrawRectangle(Pens.Black, x * 8 * Zoom, y * 8 * Zoom, 8 * Zoom, 8 * Zoom);
				e.Graphics.DrawRectangle(Pens.White, x * 8 * Zoom + 1, y * 8 * Zoom + 1, 8 * Zoom - 2, 8 * Zoom - 2);
			}
		}
	}
}
