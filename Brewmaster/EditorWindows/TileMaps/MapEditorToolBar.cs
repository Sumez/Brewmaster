using System;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorToolBar : ToolStrip
	{
		public Action ImportImage { get; set; }
		public Action ImportChr { get; set; }
		public Action ImportMap { get; set; }

		public MapEditorToolBar()
		{
			Items.AddRange(new[] { new ToolStripButton("import image", Resources.image, (s, a) => ImportImage()) });
			Items.AddRange(new[] { new ToolStripButton("import CHR", Resources.image, (s, a) => ImportChr()) });
			Items.AddRange(new[] { new ToolStripButton("import map", Resources.macro, (s, a) => ImportMap()) });
		}
	}
}
