using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.ProjectExplorer;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ScreenPanel : Panel, IMessageFilter
	{
		public ScreenPanel()
		{
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
				Controls[0].Location = _offset;
				return true;
			};
		}

		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private bool _holdingMoveModifier = false;
		private Point? _panOffset;
		private Point _offset;

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
			while (Controls.Count > 0) Controls.Remove(Controls[0]);
			Controls.Add(screenView);
		}
	}
}
