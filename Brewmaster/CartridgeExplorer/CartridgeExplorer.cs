using System.Windows.Forms;

namespace Brewmaster.CartridgeExplorer
{
	public class CartridgeExplorer: TreeView
	{
		private System.ComponentModel.IContainer components;

		public CartridgeExplorer()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
		}

		public void RefreshTree()
		{
			var cartridgeNode = new TreeNode("NROM", 0, 0);
			var prgNode = new TreeNode("PRG", 1, 1);
			var chrNode = new TreeNode("CHR", 1, 1);

			prgNode.Nodes.Add(new TreeNode("Bank 0"));
			chrNode.Nodes.Add(new TreeNode("Bank 0"));

			cartridgeNode.Nodes.Add(prgNode);
			cartridgeNode.Nodes.Add(chrNode);

			Nodes.Clear();
			Nodes.Add(cartridgeNode);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);

		}
	}

}
