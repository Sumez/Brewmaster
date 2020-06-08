using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectExplorer;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapScreenView : Control
	{
		private MapEditorTool Tool { get { return _state.Tool; } }

		private readonly TileMap _map;
		private readonly List<TileMapScreen> _screens;
		private List<int> _visibleScreens = new List<int>();
		private Pen _dotted;
		private Pen _solid;
		private Pen _dashed;
		private Bitmap _grid;
		private int _cursorX = -1;
		private int _cursorY = -1;
		private bool _mouseDown;
		private bool _alteredByTool;
		private MapEditorState _state;
		private bool _displayMetaValues;
		private bool _displayGrid;
		private TileMapScreen _cursorScreen;

		public MapScreenView(TileMap map, MapEditorState state)
		{
			_map = map;
			_state = state;
			_screens = new List<TileMapScreen>();

			DoubleBuffered = true;
			ContextMenu = new ContextMenu();

			var gridColor = Color.FromArgb(128, 255, 255, 255);
			_dotted = new Pen(gridColor, 1);
			_dotted.DashStyle = DashStyle.Custom;
			_dotted.DashPattern = new float[] { 1, 3 };

			_dashed = new Pen(gridColor, 1);
			_dashed.DashStyle = DashStyle.Custom;
			_dashed.DashPattern = new float[] { 2, 2 };

			_solid = new Pen(gridColor, 1);

			var mouseHandler = new OsFeatures.GlobalMouseHandler();
			mouseHandler.MouseUp += MouseButtonUp;
			Application.AddMessageFilter(mouseHandler);
			Disposed += (s, a) =>
			{
				mouseHandler.MouseUp -= MouseButtonUp;
				Application.RemoveMessageFilter(mouseHandler);
			};
			state.ToolChanged += () => { Cursor = state.Tool.Pixel ? new Cursor(new MemoryStream(Resources.pen)) : Cursors.Default; };
		}

		public void LoadFullMap()
		{
			AddScreens(_map.Screens.SelectMany(s => s));
		}

		private void AddScreens(IEnumerable<TileMapScreen> screens)
		{
			_screens.AddRange(screens);
			foreach (var screen in _screens.Where(s => s != null))
			{
				screen.TileChanged += (x, y, oldTile, newTile) => RefreshTile(screen, x, y, oldTile, newTile);
			}
		}

		public MapScreenView(TileMap map, TileMapScreen screen, MapEditorState state) : this(map, state)
		{
			AddScreens(new [] { screen });
		}
		public void RefreshView()
		{
			var rowWidth = Math.Min(_screens.Count, _map.Width);
			var colHeight = (_screens.Count + _map.Width - 1) / _map.Width;
			MapSize = new Size(
				_map.ScreenSize.Width * _map.BaseTileSize.Width * Zoom,
				_map.ScreenSize.Height * _map.BaseTileSize.Height * Zoom);

			RenderSize = new Size(rowWidth * MapSize.Width, colHeight * MapSize.Height);
			if (Parent == null) return;

			var width = Parent.DisplayRectangle.Width;
			var height = Parent.DisplayRectangle.Height;
			if (Width != width || Height != height) Size = new Size(width, height);
			RefreshVisibleTiles();
		}

		private TileMapScreen _screen { get { return _screens[0]; } }
		private bool MouseButtonUp(Point location)
		{
			if (_mouseDown && _alteredByTool) _screen.OnEditEnd();

			_mouseDown = false;
			_alteredByTool = false;
			return false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_cursorX < 0 || _cursorY < 0) return;

			switch (e.Button)
			{
				case MouseButtons.Left:
					_mouseDown = true;
					_alteredByTool = false;
					if (Tool != null)
					{
						PaintTool();
						Tool.AfterPaint();
					}
					break;
				case MouseButtons.Right:
					if (Tool != null) Tool.EyeDrop(_cursorX, _cursorY, _screen);
					break;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var x = (e.Location.X - Offset.X) / ToolWidth;
			if (x < 0 || x * (Tool.Size.Width * (Tool.Pixel ? 1 : _map.BaseTileSize.Width)) >= (_map.ScreenSize.Width * _map.BaseTileSize.Width)) x = -1;

			var y = (e.Location.Y - Offset.Y) / ToolHeight;
			if (y < 0 || y * (Tool.Size.Height * (Tool.Pixel ? 1 : _map.BaseTileSize.Height)) >= _map.ScreenSize.Height * _map.BaseTileSize.Height) y = -1;

			if (_cursorX == x && _cursorY == y) return;
			var oldX = _cursorX;
			var oldY = _cursorY;

			if (_mouseDown && x >= 0 && y >= 0)
			{
				InvalidateToolPosition(oldX, oldY);
				if (oldX >= 0 && oldY >= 0) PaintLine(oldX, oldY, x, y);
				else PaintTool(x, y);
				Tool.AfterPaint();
			}
			else
			{
				_cursorX = x;
				_cursorY = y;
				InvalidateToolPosition(oldX, oldY);
				InvalidateToolPosition(_cursorX, _cursorY);
			}

			base.OnMouseMove(e);
		}

		/// <summary>
		/// Bresenham's line algorithm
		/// </summary>
		public void PaintLine(int x, int y, int x2, int y2)
		{
			var w = x2 - x;
			var h = y2 - y;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
			if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
			if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
			var longest = Math.Abs(w);
			var shortest = Math.Abs(h);
			if (!(longest > shortest))
			{
				longest = Math.Abs(h);
				shortest = Math.Abs(w);
				if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
				dx2 = 0;
			}
			var numerator = longest >> 1;

			for (var i = 0; i < longest; i++)
			{
				numerator += shortest;
				if (!(numerator < longest))
				{
					numerator -= longest;
					x += dx1;
					y += dy1;
				}
				else
				{
					x += dx2;
					y += dy2;
				}
				PaintTool(x, y);
			}
		}

		private void PaintTool(int x, int y)
		{
			_cursorX = x;
			_cursorY = y;
			PaintTool();
		}
		private void PaintTool()
		{
			Tool.Paint(_cursorX, _cursorY, _screen);
			_alteredByTool = true;
			InvalidateToolPosition(_cursorX, _cursorY);
		}

		
		protected override void OnMouseLeave(EventArgs e)
		{
			var oldX = _cursorX;
			var oldY = _cursorY;
			_cursorX = _cursorY = -1;
			base.OnMouseLeave(e);
			InvalidateToolPosition(oldX, oldY);
		}

		private void InvalidateToolPosition(int x, int y)
		{
			if (x < 0 || y < 0) return;
			Invalidate(new Rectangle(x * ToolWidth + Offset.X, y * ToolHeight + Offset.Y, ToolWidth + 1, ToolHeight + 1));
		}

		public int Zoom
		{
			get { return _state.Zoom; }
		}
		public Size RenderSize { get; private set; }
		public Size MapSize { get; private set; }
		public Point Offset { get; set; }

		public bool DisplayGrid
		{
			get { return _state.DisplayGrid; }
		}

		public bool DisplayMetaValues
		{
			get { return _state.DisplayMetaValues; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			//var timer = new Stopwatch();
			//timer.Start();
			foreach (var index in _visibleScreens)
			{
				PaintScreen(_screens[index], GetOffset(index), e);
			}

			e.Graphics.Transform = new Matrix();
			e.Graphics.SetClip(e.ClipRectangle);
			e.Graphics.TranslateTransform(Offset.X, Offset.Y);

			if (_cursorX >= 0 && _cursorY >= 0)
			{
				if (Tool.Image == null && Tool.Brush != null) e.Graphics.FillRectangle(Tool.Brush, _cursorX * ToolWidth, _cursorY * ToolHeight, ToolWidth, ToolHeight);
				else if (!Tool.Pixel) e.Graphics.DrawRectangle(Pens.Black, _cursorX * ToolWidth, _cursorY * ToolHeight, ToolWidth, ToolHeight);
			}
			//timer.Stop();
			//Debug.WriteLine("Paint: " + timer.Elapsed.TotalMilliseconds);
		}

		private Point GetOffset(int index)
		{
			var x = index % _map.Width;
			var y = index / _map.Width;
			return new Point(
				Offset.X + x * MapSize.Width,
				Offset.Y + y * MapSize.Height);
		}

		private void PaintScreen(TileMapScreen screen, Point offset, PaintEventArgs e)
		{
			var clipRectangle = Rectangle.Intersect(e.ClipRectangle, new Rectangle(offset.X, offset.Y, MapSize.Width, MapSize.Height));
			if (clipRectangle.IsEmpty) return;

			e.Graphics.Transform = new Matrix();
			e.Graphics.SetClip(clipRectangle);
			e.Graphics.TranslateTransform(offset.X, offset.Y);

			e.Graphics.CompositingMode = CompositingMode.SourceCopy;
			e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			lock (screen.TileDrawLock) e.Graphics.DrawImage(screen.Image.Image, new Rectangle(0, 0, screen.Image.Width * Zoom, screen.Image.Height * Zoom));

			if (_cursorScreen == screen && Tool.Image != null)
			{
				var attributeIndex = (_cursorY / (_map.BaseTileSize.Height * _map.AttributeSize.Height)) * (_map.ScreenSize.Width / _map.AttributeSize.Width) + (_cursorX / (_map.BaseTileSize.Width * _map.AttributeSize.Width));
				Tool.RefreshImage(_map.Palettes[screen.ColorAttributes[attributeIndex] & 0xff]);
				e.Graphics.DrawImage(Tool.Image, new Rectangle(_cursorX * ToolWidth, _cursorY * ToolHeight, Tool.Image.Width * Zoom, Tool.Image.Height * Zoom));
			}

			e.Graphics.CompositingMode = CompositingMode.SourceOver;
			if (DisplayMetaValues)
				lock (screen.MetaImage)
					e.Graphics.DrawImage(screen.MetaImage.Image, new Rectangle(0, 0, screen.MetaImage.Width * _map.BaseTileSize.Width * _map.MetaValueSize.Width * Zoom, screen.MetaImage.Height * _map.BaseTileSize.Height * _map.MetaValueSize.Height * Zoom));

			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			if (DisplayGrid && Grid != null && e.ClipRectangle.Left >= 0)
			{
				var tileWidth = _map.BaseTileSize.Width * Zoom;
				var tileHeight = _map.BaseTileSize.Height * Zoom;
				var minX = Math.Max(0, (int)((e.ClipRectangle.Left - offset.X) / tileWidth) / Grid.Factor * Grid.Factor);
				var minY = Math.Max(0, (int)((e.ClipRectangle.Top - offset.Y) / tileHeight) / Grid.Factor * Grid.Factor);
				var maxX = Math.Min(_map.ScreenSize.Width, (int)((e.ClipRectangle.Right - offset.X) / tileWidth) + Grid.Factor);
				var maxY = Math.Min(_map.ScreenSize.Height, (int)((e.ClipRectangle.Bottom - offset.Y) / tileHeight) + Grid.Factor);
				for (var x = minX; x < maxX; x += Grid.Factor)
				{
					for (var y = minY; y < maxY; y += Grid.Factor)
					{
						Grid.Draw(e.Graphics, x * tileWidth, y * tileHeight);
					}
				}
			}
		}

		public int ToolWidth { get { return (Tool.Pixel ? 1 : _map.BaseTileSize.Width) * Tool.Size.Width * Zoom; } }
		public int ToolHeight { get { return (Tool.Pixel ? 1 : _map.BaseTileSize.Height) * Tool.Size.Height * Zoom; } }
		public MapGrid Grid { get; set; }

		public void RefreshTile(TileMapScreen screen, int x, int y, int oldtile, int newTile)
		{
			screen.RefreshTile(x, y, _state, true, true);
			Invalidate(new Rectangle(x * _map.BaseTileSize.Width * Zoom + Offset.X, y * _map.BaseTileSize.Width * Zoom + Offset.Y, _map.BaseTileSize.Width * Zoom, _map.BaseTileSize.Height * Zoom));
		}

		public void RefreshAllTiles()
		{
			foreach(var screen in _screens.Where(s => s != null)) screen.RefreshAllTiles(_state);
			RefreshVisibleTiles();
			Invalidate();
		}

		public void RefreshVisibleTiles()
		{
			if (Parent == null) return;
			var tileWidth = _map.BaseTileSize.Width * Zoom;
			var tileHeight = _map.BaseTileSize.Height * Zoom;

			_visibleScreens.Clear();
			for (var i = 0; i < _screens.Count; i++)
			{
				if (_screens[i] == null) continue;
				var offset = GetOffset(i);
				var clipRectangle = Rectangle.Intersect(Parent.DisplayRectangle, new Rectangle(offset.X, offset.Y, RenderSize.Width, RenderSize.Height));
				if (clipRectangle.IsEmpty) continue;
				_visibleScreens.Add(i);

				var minX = Math.Max(0, -offset.X) / tileWidth;
				var minY = Math.Max(0, -offset.Y) / tileHeight;
				var maxX = Parent.DisplayRectangle.Width / tileWidth - offset.X / tileWidth + 1;
				var maxY = Parent.DisplayRectangle.Height / tileHeight - offset.Y / tileHeight + 1;

				for (var x = minX; x <= maxX && x < _map.ScreenSize.Width; x++)
				{
					for (var y = minY; y <= maxY && y < _map.ScreenSize.Height; y++)
					{
						_screens[i].RefreshTile(x, y, _state, true);
					}
				}
			}
			Invalidate();
		}
	}
}
