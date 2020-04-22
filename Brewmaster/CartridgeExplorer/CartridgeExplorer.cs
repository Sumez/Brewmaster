using System.Windows.Forms;

namespace Brewmaster.CartridgeExplorer
{
	public class CartridgeExplorer: TreeView
	{
		private ImageList CartridgeIconList;
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CartridgeExplorer));
			this.CartridgeIconList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// CartridgeIconList
			// 
			this.CartridgeIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CartridgeIconList.ImageStream")));
			this.CartridgeIconList.TransparentColor = System.Drawing.Color.Transparent;
			this.CartridgeIconList.Images.SetKeyName(0, "cartridge.png");
			this.CartridgeIconList.Images.SetKeyName(1, "chip.png");
			// 
			// CartridgeExplorer
			// 
			this.ImageIndex = 0;
			this.ImageList = this.CartridgeIconList;
			this.SelectedImageIndex = 0;
			this.ResumeLayout(false);

		}
	}

}
