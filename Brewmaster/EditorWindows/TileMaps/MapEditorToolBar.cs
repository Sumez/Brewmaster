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
		public Action ImportPalette { get; set; }
		public Action ImportJsonSession { get; set; }

		public Action TileTool { get; set; }
		public Action ColorTool { get; set; }
		public Action PixelTool { get; set; }
		public Action MetaTool { get; set; }

		public MapEditorToolBar()
		{
			GripStyle = ToolStripGripStyle.Hidden;

			Items.AddRange(new[] { new ToolStripButton("import image", Resources.image, (s, a) => ImportImage()) });
			Items.AddRange(new[] { new ToolStripButton("import CHR", Resources.image, (s, a) => ImportChr()) });
			Items.AddRange(new[] { new ToolStripButton("import map", Resources.macro, (s, a) => ImportMap()) });
			Items.AddRange(new[] { new ToolStripButton("import Palette", Resources.image, (s, a) => ImportPalette()) });
			Items.AddRange(new[] { new ToolStripButton("import JSON", Resources.macro, (s, a) => ImportJsonSession()) });

			Items.AddRange(new[] { new ToolStripButton("Tile", Resources.image, (s, a) => TileTool()) });
			Items.AddRange(new[] { new ToolStripButton("Color", Resources.data, (s, a) => ColorTool()) });
			Items.AddRange(new[] { new ToolStripButton("Pen", Resources.data, (s, a) => PixelTool()) });
			Items.AddRange(new[] { new ToolStripButton("Collisions", Resources.chip, (s, a) => MetaTool()) });

		}
	}
}
