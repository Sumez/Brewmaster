using System.Drawing;
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
			_state = state;
			_map = map;
			_offset = Point.Empty;
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
				_offset.Offset(point.X - _panOffset.Value.X, point.Y - _panOffset.Value.Y);
				_panOffset = point;
				if (_singleView != null)
				{
					_singleView.Offset = _offset;
					_singleView.RefreshVisibleTiles();
					_singleView.Refresh();
				}
				return true;
			};

			_grid = new MapGrid();
			_state.ZoomChanged += () =>
			{
				if (_singleView != null) _singleView.RefreshView();
				_grid.GenerateGrid(_map, _state.Zoom);
			};
			_grid.GenerateGrid(_map, _state.Zoom);
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			if (_singleView != null)
			{
				if (_singleView != null) _singleView.RefreshView();
				_singleView.RefreshVisibleTiles();
			}
		}

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private bool _holdingMoveModifier = false;
		private Point? _panOffset;
		private Point _offset;
		private MapScreenView _singleView;
		private MapGrid _grid;

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

		public void Add(MapScreenView screenView)
		{
			_offset = Point.Empty;
			if (_singleView != null)
			{
				Controls.Remove(_singleView);
				_singleView.Dispose();
			}

			screenView.Grid = _grid;
			Controls.Add(_singleView = screenView);
			//Controls.SetChildIndex(_singleView, 0);
		}
	}
}
