using System.Windows.Forms;
using Brewmaster.Modules;

namespace Brewmaster.EditorWindows
{
	public class TextEditorMenu : ContextMenuStrip
	{
		private readonly ToolStripMenuItem _cutMenuItem;
		private readonly ToolStripMenuItem _copyMenuItem;
		private readonly ToolStripMenuItem _pasteMenuItem;
		private readonly ToolStripMenuItem _deleteMenuItem;
		private readonly ToolStripMenuItem _selectAllMenuItem;

		public TextEditorMenu(Events events)
		{
			_cutMenuItem = new ToolStripMenuItem("Cut", Properties.Resources.cut1, (s, a) => events.Cut());
			_copyMenuItem = new ToolStripMenuItem("Copy", Properties.Resources.copy1, (s, a) => events.Copy());
			_pasteMenuItem = new ToolStripMenuItem("Paste", Properties.Resources.paste1, (s, a) => events.Paste());

			_deleteMenuItem = new ToolStripMenuItem("Delete", null, (s, a) => events.Delete());
			_selectAllMenuItem = new ToolStripMenuItem("Select All", null, (s, a) => events.SelectAll());

			InitializeComponent();
		}


		protected virtual void InitializeComponent()
		{
			Items.AddRange(new ToolStripItem[]
			{
				_cutMenuItem,
				_copyMenuItem,
				_pasteMenuItem,
				new ToolStripSeparator(),
				_deleteMenuItem,
				_selectAllMenuItem
			});
		}
	}
}