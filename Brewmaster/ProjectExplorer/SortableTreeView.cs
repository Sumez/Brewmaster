using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.ProjectExplorer
{
	/// <summary>
	/// A lot of messy basic code useful for any tree view with drag/drop and sorting features, making it
	/// behave more like people would usually expect. Moved to its own class to prevent clutter in ProjectExplorer.
	/// </summary>
	public class SortableTreeView : SelectableTreeView
	{
		private TreeNode _dragTarget;

		public SortableTreeView()
		{
			ItemDrag += (s, a) =>
			{
				if (!AllowDrop) return;
				if (!OnBeforeDrag(a.Item)) return;
				DoDragDrop(a.Item, DragDropEffects.Move | DragDropEffects.Scroll);
			};
			DragOver += (s, a) =>
			{
				a.Effect = DragDropEffects.Move | DragDropEffects.Scroll;
				var node = GetNodeAt(PointToClient(new Point(a.X, a.Y)));
				if (node == null || node == _dragTarget) return;

				if (_dragTarget != null)
				{
					_dragTarget.BackColor = BackColor;
					_dragTarget.ForeColor = ForeColor;
				}
				_dragTarget = node;
				node.BackColor = SystemColors.HotTrack;
				node.ForeColor = SystemColors.HighlightText;
			};
			DragLeave += (s, a) =>
			{
				if (_dragTarget == null) return;
				_dragTarget.BackColor = BackColor;
				_dragTarget.ForeColor = ForeColor;
				_dragTarget = null;
			};
			DragDrop += (s, a) =>
			{
				if (_dragTarget == null) return;
				_dragTarget.BackColor = BackColor;
				_dragTarget.ForeColor = ForeColor;

				OnAfterDrag(a.Data, _dragTarget);
				_dragTarget = null;
			};

		}

		protected virtual bool OnBeforeDrag(object draggedItem)
		{
			return true;
		}
		protected virtual void OnAfterDrag(IDataObject data, TreeNode dragTarget)
		{
		}
	}
}