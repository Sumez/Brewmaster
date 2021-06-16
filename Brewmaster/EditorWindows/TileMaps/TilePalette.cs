using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectExplorer;
using Brewmaster.Settings;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TilePalette : Control
	{
		public MapEditorState State;
		public event Action<int> TileClick;
		public event Action<int> TileHover;
		public event Action<int, int> TileDrag;
		public Action<int> RemoveTile;
		public bool AllowTileDrag { get; set; }
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
			var height = tileHeight * ColHeight;

			var grid = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(grid))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				for (var i = 1; i < _rowWidth; i++)
				{
					graphics.DrawLine(_solid, i * tileWidth, 0, i * tileWidth, height);
				}
				for (var i = 1; i < ColHeight; i++)
				{
					graphics.DrawLine(_solid, 0, i * tileHeight, width, i * tileHeight);
				}
			}
			
			if (_grid != null) _grid.Dispose();
			_grid = grid;
		}
		private void GenerateOverlay()
		{
			var tileWidth = _tileWidth * _metaTileWidth * Zoom;
			var tileHeight = _tileHeight * _metaTileWidth * Zoom;
			var width = tileWidth * _rowWidth;
			var height = tileHeight * ColHeight;

			var overlay = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(overlay))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				foreach (var tileIndex in _highlightedTiles)
				{
					var x = tileIndex % _rowWidth;
					var y = tileIndex / _rowWidth;
					graphics.FillRectangle(_overlayBrush, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
				}

			}

			if (_overlay != null) _overlay.Dispose();
			_overlay = overlay;
		}


		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Right:
					MoveSelection(1);
					return true;
				case Keys.Left:
					MoveSelection(- 1);
					return true;
				case Keys.Up:
					MoveSelection(- _rowWidth);
					return true;
				case Keys.Down:
					MoveSelection(_rowWidth);
					return true;
			}
			if (keyData != Program.Keys[Feature.RemoveFromList] || RemoveTile == null) return base.ProcessCmdKey(ref msg, keyData);
			if (SelectedTile >= 0) RemoveTile(SelectedTile);
			return true;
		}

		private void MoveSelection(int delta)
		{
			var selectTile = SelectedTile + delta;
			if (selectTile >=  0 && selectTile < Tiles.Count && TileClick != null) TileClick(selectTile);
		}

		private FastBitmap _image = new FastBitmap(128, 128);
		private Pen _solid;
		private int _zoom = 1;
		private Bitmap _grid;
		private Bitmap _overlay;
		private int _selectedTile = -1;
		private int _hoverTile = -1;
		private Palette _palette;
		private SolidBrush _solidBrush;
		private SolidBrush _overlayBrush;
		private List<MetaTile> _tiles = new List<MetaTile>();
		private List<int> _highlightedTiles = new List<int>();

		private int _dragTile = -1;
		private Point _dragOrigin;

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
				var image = new FastBitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * ColHeight);
				_image.Dispose();
				_image = image;
				GenerateGrid();
				RefreshImage();
			}
		}

		public int ColHeight { get; private set; }

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
				if (TileHover != null) TileHover(_hoverTile);
				Invalidate();
			}
		}

		public List<MetaTile> Tiles
		{
			get { return _tiles; }
			set
			{
				_tiles = value;
				var colHeight = Math.Max((_tiles.Count + _rowWidth - 1) / _rowWidth, _colHeight);
				if (colHeight > ColHeight)
				{
					ColHeight = colHeight;
					GenerateGrid();
				}
				ColHeight = colHeight;
				RefreshImage();
			}
		}

		public List<int> HighlightedTiles
		{
			get { return _highlightedTiles; }
			set
			{
				if (_highlightedTiles.SequenceEqual(value)) return;
				_highlightedTiles = value;
				GenerateOverlay();
				Invalidate();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var x = e.Location.X / (_tileWidth * _metaTileWidth * Zoom);
			var y = e.Location.Y / (_tileHeight * _metaTileWidth * Zoom);

			var tile = y * _rowWidth + x;
			if (x < 0 || x >= _rowWidth || y < 0 || tile >= Tiles.Count) HoverTile = -1;
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
			Focus();
			if (HoverTile >= 0 && TileClick != null) TileClick(HoverTile);
			if (HoverTile >= 0 && AllowTileDrag && e.Button == MouseButtons.Left)
			{
				_dragTile = HoverTile;
				_dragOrigin = e.Location;
			}

			base.OnMouseDown(e);
		}
		private bool GlobalMouseUp(Point location)
		{
			if (AllowTileDrag && _dragTile >= 0)
			{
				if (_hoverTile >= 0 && _dragTile != _hoverTile && TileDrag != null)
				{
					TileDrag(_dragTile, _hoverTile);
				}
				if (TileClick != null) TileClick(_hoverTile);
				Invalidate();
			}
			_dragTile = -1;
			_dragLocation = null;
			return false;
		}

		private bool GlobalMouseMoved(Point location)
		{
			if (_dragTile < 0) return false;
			location = PointToClient(location);
			if (_dragLocation != null)
			{
				_dragLocation = location;
				Invalidate();
				return false;
			}
			if (Math.Abs(location.X - _dragOrigin.X) + Math.Abs(location.Y - _dragOrigin.Y) < 5) return false;

			_dragLocation = location;
			var dragImage = new FastBitmap(_tileWidth * MetaTileWidth, _tileWidth * MetaTileWidth);
			var metaTile = Tiles[_dragTile];
			for (var j = 0; j < metaTile.Tiles.Length; j++)
			{
				var tile = State.GetTileImage(metaTile.Tiles[j], GetTileColors(metaTile, j));
				if (tile == null) continue;
				tile.CopyTile(dragImage, (j % _metaTileWidth), (j / _metaTileWidth));
			}
			if (_dragImage != null) _dragImage.Dispose();
			_dragImage = dragImage;
			Invalidate();
			return false;
		}

		public TilePalette()
		{
			DoubleBuffered = true;
			SetStyle(ControlStyles.Selectable, true);

			ColHeight = _colHeight;
			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_solid = new Pen(gridColor, 1);
			_solidBrush = new SolidBrush(gridColor);
			_overlayBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));

			GetTileColors = (tile, i) => Palette;

			var mouseHandler = new OsFeatures.GlobalMouseHandler();
			mouseHandler.MouseMoved += GlobalMouseMoved;
			mouseHandler.MouseUp += GlobalMouseUp;
			Application.AddMessageFilter(mouseHandler);
			Disposed += (s, a) => { Application.RemoveMessageFilter(mouseHandler); };	
		}

		private readonly object _backBufferLock = new object();
		private Task _backBufferTask;
		private Action _cancelBackBufferTask;
		private int _expectedHeight = 0;
		public void RefreshImage()
		{
			if (Tiles == null) return;

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
				var image = new FastBitmap(_tileWidth * _metaTileWidth * _rowWidth, _tileHeight * _metaTileWidth * ColHeight);
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

				lock (_backBufferLock)
				{
					var oldImage = _image;
					_image = image;
					oldImage.Dispose();
				}
				Invalidate();
			}, token);
		}

		public Func<MetaTile, int, Palette> GetTileColors;
		private Point? _dragLocation;
		private FastBitmap _dragImage;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var tileWidth = _tileWidth * _metaTileWidth * Zoom;
			var tileHeight = _tileHeight * _metaTileWidth * Zoom;

			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			lock (_backBufferLock)
			{
				if (_image.Height != _expectedHeight) Height = (_expectedHeight = _image.Height) * Zoom;
				e.Graphics.DrawImage(_image.Image, new Rectangle(0, 0, _image.Width * Zoom, _image.Height * Zoom));
			}

			e.Graphics.CompositingMode = CompositingMode.SourceOver;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			if (_highlightedTiles.Any()) e.Graphics.DrawImageUnscaled(_overlay, 0, 0);

			if (_dragTile >= 0 && _dragLocation != null)
			{
				var x = _dragTile % _rowWidth;
				var y = _dragTile / _rowWidth;
				e.Graphics.FillRectangle(Brushes.Black, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
			}
			e.Graphics.DrawImageUnscaled(_grid, 0, 0);

			if (_hoverTile >= 0)
			{
				var x = _hoverTile % _rowWidth;
				var y = _hoverTile / _rowWidth;
				e.Graphics.FillRectangle(_solidBrush, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
			}
			
			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			if (_selectedTile >= 0 && _dragLocation == null)
			{
				var x = _selectedTile % _rowWidth;
				var y = _selectedTile / _rowWidth;
				e.Graphics.DrawRectangle(Pens.White, x * tileWidth, y * tileHeight, tileWidth, tileHeight);
				e.Graphics.DrawRectangle(Pens.Black, x * tileWidth + 1, y * tileHeight + 1, tileWidth - 2, tileHeight - 2);
			}
			if (_dragTile >= 0 && _dragLocation != null && _dragImage != null)
			{
				var dragOffset = new Point(_dragOrigin.X % tileWidth, _dragOrigin.Y % tileHeight);
				var dragTileLocation = new Rectangle(_dragLocation.Value.X - dragOffset.X, _dragLocation.Value.Y - dragOffset.Y, tileWidth, tileHeight);
				e.Graphics.DrawImage(_dragImage.Image, dragTileLocation);
				e.Graphics.DrawRectangle(Pens.White, dragTileLocation);
			}
		}
	}
}
