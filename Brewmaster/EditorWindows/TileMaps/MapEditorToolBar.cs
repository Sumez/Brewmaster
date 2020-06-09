using System;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorToolBar : ToolStrip
	{
		private ToolStripMenuItem _importMenu;
		public Action ImportImage { get; set; }
		public Action ImportChr { get; set; }
		public Action ImportMap { get; set; }
		public Action ImportPalette { get; set; }
		public Action ImportPyxelMap { get; set; }
		public Action ImportJsonSession { get; set; }

		public Action TileTool { get; set; }
		public Action ColorTool { get; set; }
		public Action PixelTool { get; set; }
		public Action MetaTool { get; set; }

		public Action ToggleGrid { get; set; }
		public Action ToggleMetaValues { get; set; }

		public MapEditorToolBar()
		{
			GripStyle = ToolStripGripStyle.Hidden;

			Items.AddRange(new ToolStripItem[] {
				_importMenu = new ToolStripMenuItem(Resources.config) { Height = 10 }, 
				new ToolStripButton("import CHR", Resources.image, (s, a) => ImportChr()),

				new ToolStripSeparator(),

				GridButton = new ToolStripButton("Show grid", Resources.chip, (s, a) => ToggleGrid()) { CheckOnClick = true },
				CollisionButton = new ToolStripButton("Show Collisions", Resources.chip, (s, a) => ToggleMetaValues()) { CheckOnClick = true },

				new ToolStripSeparator(),

				new ToolStripButton("Tile", Resources.image, (s, a) => TileTool()),
				new ToolStripButton("Color", Resources.data, (s, a) => ColorTool()),
				new ToolStripButton("Pen", Resources.data, (s, a) => PixelTool()),
				new ToolStripButton("Collisions", Resources.chip, (s, a) => MetaTool()),
			});

			_importMenu.DropDownItems.AddRange(new []
			{
				new ToolStripMenuItem("Import NESST Map...", null, (s, a) => ImportMap()),
				new ToolStripMenuItem("Import NES Palette...", null, (s, a) => ImportPalette()),
				new ToolStripMenuItem("Import PyxelEdit Map...", null, (s, a) => ImportPyxelMap()),
				new ToolStripMenuItem("Import Image...", null, (s, a) => ImportImage()),
				new ToolStripMenuItem("Import JSON session...", null, (s, a) => ImportJsonSession()),
			});
		}

		public ToolStripButton CollisionButton { get; set; }
		public ToolStripButton GridButton { get; set; }
	}
}
