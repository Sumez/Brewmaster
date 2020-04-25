using System.Windows.Forms;

namespace Brewmaster.ProjectExplorer
{
	/// <summary>
	/// Fixes issues with the default WinForms TreeView class, allowing the user to mark an item on mouse
	/// down, yet only start editing node labels when clicking on an already selected item.
	/// To programmatically start editing a node, the EditNode() method should always be used.
	/// </summary>
	public class SelectableTreeView : TreeView
	{
		private TreeNode _previouslySelectedNode;

		public SelectableTreeView()
		{
			MouseDown += (s, a) =>
			{
				_previouslySelectedNode = SelectedNode;
				SelectedNode = GetNodeAt(a.X, a.Y);
			};
		}

		protected void EditNode(TreeNode node)
		{
			_previouslySelectedNode = node;
			node.BeginEdit();
		}

		protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnBeforeLabelEdit(e);
			if (_previouslySelectedNode != e.Node) e.CancelEdit = true;
		}
	}
}