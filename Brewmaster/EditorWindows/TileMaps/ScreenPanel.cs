﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.ProjectExplorer;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ScreenPanel : Panel, IMessageFilter
	{
		private readonly MapEditorState _state;
		private readonly TileMap _map;

		public ScreenPanel(MapEditorState state, TileMap map)
		{
			_vScrollBar = new VScrollBar { LargeChange = 128 };
			_hScrollBar = new HScrollBar { LargeChange = 128 };
			Controls.Add(_vScrollBar);
			Controls.Add(_hScrollBar);
			_vScrollBar.Location = new Point(Width - _vScrollBar.Width, 0);
			_vScrollBar.Height = Height - _hScrollBar.Height;
			_hScrollBar.Location = new Point(0, Height - _hScrollBar.Height);
			_hScrollBar.Width = Width - _vScrollBar.Width;
			_vScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			_hScrollBar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
			_panel = new Panel { Width = Width - _vScrollBar.Width, Height = Height - _hScrollBar.Height, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };
			Controls.Add(_panel);

			_state = state;
			_map = map;
			Offset = Point.Empty;

			_hScrollBar.Scroll += (s, a) =>
			{
				Offset = new Point(- _hScrollBar.Value, Offset.Y);
			};
			_vScrollBar.Scroll += (s, a) =>
			{
				Offset = new Point(Offset.X, - _vScrollBar.Value);
			};

			var mouseHandler = new OsFeatures.GlobalMouseHandler();
			Application.AddMessageFilter(mouseHandler);
			Application.AddMessageFilter(this);
			Disposed += (s, a) =>
			{
				Application.RemoveMessageFilter(mouseHandler);
				Application.RemoveMessageFilter(this);
			};

			mouseHandler.MouseDown += (point) =>
			{
				if (!Visible || !_holdingMoveModifier || !new Rectangle(Point.Empty, Size).Contains(PointToClient(point))) return false;
				_panOffset = point;
				return true;
			};
			mouseHandler.MouseUp += (point) =>
			{
				_panOffset = null;
				return false;
			};
			mouseHandler.MouseMoved += (point) =>
			{
				if (!_panOffset.HasValue) return false;
				Offset = Point.Add(Offset, new Size(point.X - _panOffset.Value.X, point.Y - _panOffset.Value.Y));
				_panOffset = point;
				return true;
			};

			_grid = new MapGrid();
			_state.ZoomChanged += (oldZoom, newZoom) =>
			{
				var hotSpot = _panel.PointToClient(Cursor.Position);
				if (!_panel.DisplayRectangle.Contains(hotSpot)) hotSpot = Point.Empty;

				_grid.GenerateGrid(_map, _state.Zoom);

				var x = ((_offset.X - hotSpot.X) * newZoom) / oldZoom;
				var y = ((_offset.Y - hotSpot.Y) * newZoom) / oldZoom;
				Offset = new Point(x + hotSpot.X, y + hotSpot.Y);
				RefreshView();
			};
			_state.DisplayGridChanged += InvalidateVisibleViews;
			_state.DisplayMetaValuesChanged += InvalidateVisibleViews;
			_state.ChrDataChanged += RefreshVisibleTiles;
			_state.AfterUndo += (step) =>
			{
				foreach (var screen in step.States.Keys) screen.RefreshAllTiles(_state);
				RefreshVisibleTiles();
			};

			_grid.GenerateGrid(_map, _state.Zoom);
		}

		private void RefreshVisibleTiles()
		{
			if (_singleView != null) _singleView.RefreshVisibleTiles();
		}

		private void RefreshView()
		{
			if (_singleView != null)
			{
				_singleView.RefreshView();
				_hScrollBar.Maximum = Math.Max(_singleView.RenderSize.Width - _panel.Width + _hScrollBar.LargeChange, -_offset.X);
				_vScrollBar.Maximum = Math.Max(_singleView.RenderSize.Height - _panel.Height + _vScrollBar.LargeChange, -_offset.Y);
			}
		}

		public void InvalidateVisibleViews()
		{
			if (_singleView != null) _singleView.Invalidate();
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			RefreshView();
		}

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private bool _holdingMoveModifier = false;
		private Point? _panOffset;
		private Point _offset;
		private MapScreenView _singleView;
		private MapGrid _grid;
		private VScrollBar _vScrollBar;
		private HScrollBar _hScrollBar;
		private Panel _panel;
		private TileMapScreen _loadedScreen;

		private Point Offset
		{
			get { return _offset; }
			set
			{
				if (_offset == value) return;
				_offset = value;

				if (_singleView != null)
				{
					_singleView.Offset = _offset;
					_singleView.RefreshVisibleTiles();
					_singleView.Refresh();

					_hScrollBar.Maximum = Math.Max(_singleView.RenderSize.Width - _panel.Width + _hScrollBar.LargeChange, -_offset.X);
					_vScrollBar.Maximum = Math.Max(_singleView.RenderSize.Height - _panel.Height + _vScrollBar.LargeChange, -_offset.Y);
				}

				_hScrollBar.Minimum = Math.Min(0, -_offset.X);
				_vScrollBar.Minimum = Math.Min(0, -_offset.Y);
				_hScrollBar.Value = Math.Min(_hScrollBar.Maximum, - _offset.X);
				_vScrollBar.Value = Math.Min(_vScrollBar.Maximum, - _offset.Y);
			}
		}

		public bool PreFilterMessage(ref Message m)
		{
			if ((m.Msg != WM_KEYDOWN && m.Msg != WM_KEYUP) || (Keys) m.WParam != Keys.Space) return false;
			if (m.Msg == WM_KEYDOWN)
			{
				Cursor = Cursors.SizeAll;
				_holdingMoveModifier = true;
			}

			if (m.Msg == WM_KEYUP)
			{
				Cursor = Cursors.Default;
				_holdingMoveModifier = false;
			}
			return false;
		}

		public void Pan(int deltaX, int deltaY)
		{
			Offset = new Point(
				Math.Max(- (_hScrollBar.Maximum), Math.Min(- _hScrollBar.Minimum, Offset.X - deltaX)),
				Math.Max(- (_vScrollBar.Maximum), Math.Min(- _vScrollBar.Minimum, Offset.Y - deltaY))
			);
		}

		public void RefreshAllTiles()
		{
			if (_singleView != null) _singleView.RefreshAllTiles();
		}

		public void AddSingleScreen(TileMapScreen screen)
		{
			_loadedScreen = screen;
			Add(new MapScreenView(_map, screen, _state));
		}

		public void AddFullMap(Point? focus = null)
		{
			//if (_singleView == null || _loadedScreen != null)
			{
				_loadedScreen = null;
				var screenView = new MapScreenView(_map, _state);
				screenView.LoadFullMap();
				Add(screenView);
			}
			if (focus.HasValue)
			{
				Offset = new Point(- focus.Value.X * _singleView.MapSize.Width, - focus.Value.Y * _singleView.MapSize.Height);
			}
		}

		private void Add(MapScreenView screenView)
		{
			Offset = Point.Empty;
			if (_singleView != null)
			{
				_panel.Controls.Remove(_singleView);
				_singleView.Dispose();
			}

			screenView.Grid = _grid;
			_panel.Controls.Add(_singleView = screenView);
			RefreshView();
			RefreshAllTiles();
		}

		public void RefreshMapScreens()
		{
			if (_loadedScreen == null)
			{
				var offset = Offset;
				AddFullMap();
				if ((-offset.X) >= _singleView.RenderSize.Width) offset.X = 0;
				if ((-offset.Y) >= _singleView.RenderSize.Height) offset.Y = 0;
				Offset = offset;
				return;
			}

			var remainingScreens = _map.Screens.SelectMany(s => s).Where(s => s != null).ToArray();
			if (remainingScreens.Contains(_loadedScreen)) return;
			AddSingleScreen(remainingScreens[0]);
		}
	}
}
