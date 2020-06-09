using System;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorToolBar : ToolStrip
	{
		private ToolStripDropDownItem _importMenu;
		public Action ImportImage { get; set; }
		public Action ImportChr { get; set; }
		public Action ImportMap { get; set; }
		public Action ImportPalette { get; set; }
		public Action ImportPyxelMap { get; set; }
		public Action ImportJsonSession { get; set; }

		public Action TileTool { get; set; }
		public Action MetaTileTool { get; set; }
		public Action ColorTool { get; set; }
		public Action PixelTool { get; set; }
		public Action MetaTool { get; set; }

		public Action ToggleGrid { get; set; }
		public Action ToggleMetaValues { get; set; }

		public MapEditorToolBar()
		{
			//GripStyle = ToolStripGripStyle.Hidden;

			Items.AddRange(new ToolStripItem[] {

				new ToolStripLabel("Map Editor"),
				new ToolStripSeparator(),
				_importMenu = new ToolStripDropDownButton("Import File", Resources.import_tilemap) { DisplayStyle = ToolStripItemDisplayStyle.Image, Name = "ImportTileMap" }, 
				new ToolStripSeparator(),
				GridButton = new ToolStripButton("Display Grid", Resources.grid, (s, a) => ToggleGrid(), "DisplayGrid") { CheckOnClick = true },
				CollisionButton = new ToolStripButton("Display Tile Overlay", Resources.overlay, (s, a) => ToggleMetaValues(), "DisplayTileMapOverlay") { CheckOnClick = true },
				new ToolStripSeparator(),
				new ToolStripButton("Draw Tiles", Resources.tile_tool, (s, a) => TileTool(), "TileTool"),
				new ToolStripButton("Draw MetaTiles", Resources.metatile_tool, (s, a) => MetaTileTool(), "MetaTileTool"),
				new ToolStripButton("Set Color Attribute", Resources.palette_tool, (s, a) => ColorTool(), "ColorAttributeTool"),
				new ToolStripButton("Pixel Pen", Resources.pen_tool, (s, a) => PixelTool(), "PixelPenTool"),
				new ToolStripButton("Edit Tile/Collision Map", Resources.overlay_tool, (s, a) => MetaTool(), "CollisionTool"),
			});
			foreach (var item in Items.OfType<ToolStripButton>()) item.DisplayStyle = ToolStripItemDisplayStyle.Image;

			_importMenu.DropDownItems.AddRange(new ToolStripItem[]
			{
				new ToolStripMenuItem("Import NESST Map...", null, (s, a) => ImportMap(), "ImportNesstMap"),
				new ToolStripMenuItem("Import NES Palette...", null, (s, a) => ImportPalette(), "ImportNesPalette"),
				new ToolStripMenuItem("Import PyxelEdit Map...", null, (s, a) => ImportPyxelMap(), "ImportPyxelMap"),
				new ToolStripMenuItem("Import Image...", null, (s, a) => ImportImage(), "ImportImage"),
				new ToolStripMenuItem("import CHR", null, (s, a) => ImportChr()),
				new ToolStripMenuItem("Import JSON session...", null, (s, a) => ImportJsonSession(), "ImportJsonSession"),
			});
		}

		public ToolStripButton CollisionButton { get; set; }
		public ToolStripButton GridButton { get; set; }
	}
}
