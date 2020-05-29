using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TilePalette : Control
	{
		public MapEditorState State;
		public event Action<int> TileClick;
		private Palette Palette
		{
			get { return State.Palette; }
		}

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
			var tileWidth = _tileWidth * _metaTileWidth * Zoom;
			var tileHeight = _tileHeight * _metaTileWidth * Zoom;
			var width = tileWidth * _rowWidth;
			var height = tileHeight * _colHeight;

			var grid = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(grid))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				for (var i = 1; i < _rowWidth; i++)
				{
					graphics.DrawLine(_solid, i * tileWidth, 0, i * tileWidth, height);
				}
				for (var i = 1; i < _colHeight; i++)
				{
					graphics.DrawLine(_solid, 0, i * tileHeight, width, i * tileHeight);
				}
			}
			
			if (_grid != null) _grid.Dispose();
			_grid = grid;
		}

		private Image _image = new Bitmap(128, 128, PixelFormat.Format32bppPArgb);
		private Pen _solid;
		private int _zoom = 1;
		private Bitmap _grid;
		private int _selectedTile = -1;
		private int _hoverTile = -1;
		private Palette _palette;
		private SolidBrush _solidBrush;
		private List<MetaTile> _tiles;

		private int _tileWidth = 8;
		private int _tileHeight = 8;
		private int _metaTileWidth = 1;
		private int _rowWidth = 16;
		private int _colHeight = 16;

		public int MetaTileWidth
		{
			get { return _metaTileWidth; }
			set
			{
				if (_metaTileWidth == value) return;
				_metaTileWidth = value;
				var image = new Bitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * _colHeight);
				_image.Dispose();
				_image = image;
				GenerateGrid();
				RefreshImage();
			}
		}

		private byte[] ChrData
		{
			get { return State.ChrData; }
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
		protected int HoverTile
		{
			get { return _hoverTile; }
			set
			{
				if (_hoverTile == value) return;
				_hoverTile = value;
				Invalidate();
			}
		}

		public List<MetaTile> Tiles
		{
			get { return _tiles; }
			set
			{
				_tiles = value;
				RefreshImage();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var x = e.Location.X / (_tileWidth * _metaTileWidth * Zoom);
			var y = e.Location.Y / (_tileHeight * _metaTileWidth * Zoom);

			if (x < 0 || x >= _rowWidth || y < 0 || y >= _colHeight) HoverTile = -1;
			else HoverTile = y * _rowWidth + x;
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			HoverTile = -1;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (HoverTile >= 0 && TileClick != null) TileClick(HoverTile);
			base.OnMouseDown(e);
		}

		public TilePalette()
		{
			DoubleBuffered = true;
			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_solid = new Pen(gridColor, 1);
			_solidBrush = new SolidBrush(gridColor);

			GetTileColors = (tile, i) => Palette.Colors;
		}

		private readonly object _backBufferLock = new object();
		private Task _backBufferTask;
		public void RefreshImage()
		{
			if (Tiles == null) return;
			// TODO: Just cancel task if not finished

			if (_backBufferTask != null && !_backBufferTask.IsCompleted) _backBufferTask.Wait();

			_backBufferTask = Task.Run(() =>
			{
				lock (_backBufferLock)
				{
					var image = new Bitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * _colHeight, PixelFormat.Format32bppPArgb);
					using (var graphics = Graphics.FromImage(image))
					{
						graphics.Clear(Palette.Colors[0]);
						if (ChrData != null)
						{
							var metaTileWidth = _tileWidth * _metaTileWidth;
							var metaTileHeight = _tileHeight * _metaTileWidth;

							for (var i = 0; i < Tiles.Count; i++)
							{
								var metaTile = Tiles[i];
								for (var j = 0; j < metaTile.Tiles.Length; j++)
								{
									using (var tile = TileImage.GetTileImage(ChrData, metaTile.Tiles[j], GetTileColors(metaTile, j)))
									{
										if (tile == null) continue;

										graphics.DrawImageUnscaled(tile.Image,
											(i % _rowWidth) * metaTileWidth + (j % _metaTileWidth) * _tileWidth,
											(i / _rowWidth) * metaTileHeight + (j / _metaTileWidth) * _tileHeight);
									}
								}
							}
						}
					}

					var oldImage = _image;
					_image = image;
					oldImage.Dispose();
				}
				Invalidate();
			});
		}

		public Func<MetaTile, int, List<Color>> GetTileColors;

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

			var tileWidth = _tileWidth * _metaTileWidth * Zoom;
			var tileHeight = _tileHeight * _metaTileWidth * Zoom;
			if (_hoverTile >= 0)
			{
				var x = _hoverTile % _rowWidth;
				var y = _hoverTile / _rowWidth;
				e.Graphics.FillRectangle(_solidBrush, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
			}
			
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			if (_selectedTile >= 0)
			{
				var x = _selectedTile % _rowWidth;
				var y = _selectedTile / _rowWidth;
				e.Graphics.DrawRectangle(Pens.White, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
				e.Graphics.DrawRectangle(Pens.Black, x * tileWidth + 1, y * tileHeight + 1, tileWidth - 2, tileHeight - 2);
			}
		}
	}
}
