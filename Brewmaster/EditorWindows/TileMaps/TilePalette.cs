using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.Modules.Ppu;

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

		private FastBitmap _image = new FastBitmap(128, 128);
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
				var image = new FastBitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * _colHeight);
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

			var tile = y * _rowWidth + x;
			if (x < 0 || x >= _rowWidth || y < 0 || y >= _colHeight || tile >= Tiles.Count) HoverTile = -1;
			else HoverTile = tile;
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

			GetTileColors = (tile, i) => Palette;
		}

		private readonly object _backBufferLock = new object();
		private Task _backBufferTask;
		private Action _cancelBackBufferTask;
		public void RefreshImage()
		{
			if (Tiles == null) return;
			// TODO: Just cancel task if not finished

			if (_backBufferTask != null && !_backBufferTask.IsCompleted)
			{
				_cancelBackBufferTask();
				_backBufferTask = null;
			}

			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;
			_cancelBackBufferTask = () => tokenSource.Cancel();

			_backBufferTask = Task.Run(() =>
			{
				lock (_backBufferLock)
				{
					var image = new FastBitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * Math.Max((Tiles.Count + _rowWidth - 1) / _rowWidth, _colHeight));
					image.Clear(Palette.Colors[0]);
					if (ChrData != null)
					{
						for (var i = 0; i < Tiles.Count; i++)
						{
							if (token.IsCancellationRequested) return;
							var metaTile = Tiles[i];
							for (var j = 0; j < metaTile.Tiles.Length; j++)
							{
								var tile = State.GetTileImage(metaTile.Tiles[j], GetTileColors(metaTile, j));
								if (tile == null) continue;
								tile.CopyTile(image,
									(i % _rowWidth) * _metaTileWidth + (j % _metaTileWidth),
									(i / _rowWidth) * _metaTileWidth + (j / _metaTileWidth));
							}
						}
					}

					var oldImage = _image;
					_image = image;
					oldImage.Dispose();
				}
				Invalidate();
			}, token);
		}

		public Func<MetaTile, int, Palette> GetTileColors;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.DrawImage(_image.Image, new Rectangle(0, 0, _image.Width * Zoom, _image.Height * Zoom));

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
