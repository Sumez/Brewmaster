using System;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorToolBar : ToolStrip
	{
		public Action ImportImage { get; set; }

		public MapEditorToolBar()
		{
			Items.AddRange(new [] { new ToolStripButton("import image", Resources.image, (s, a) => ImportImage()) });
		}
	}
}
