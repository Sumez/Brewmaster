using System;
using System.Collections.Generic;
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
		public Action<int?> MetaTileTool { get; set; }
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
				_importMenu = new ToolStripDropDownButton("Import File", Resources.import_tilemap) { Name = "ImportTileMap" }, 
				new ToolStripSeparator(),
				GridButton = new ToolStripButton("Display Grid", Resources.grid, (s, a) => ToggleGrid(), "DisplayGrid") { CheckOnClick = true },
				CollisionButton = new ToolStripButton("Display Tile Overlay", Resources.overlay, (s, a) => ToggleMetaValues(), "DisplayTileMapOverlay") { CheckOnClick = true },
				new ToolStripSeparator(),
				new ToolStripButton("Draw Tiles", Resources.tile_tool, (s, a) => TileTool(), "TileTool"),
				MetaTileButton = new ToolStripSplitButton("Draw MetaTiles", Resources.metatile_tool, (s, a) => MetaTileTool(null), "MetaTileTool"),
				new ToolStripButton("Set Color Attribute", Resources.palette_tool, (s, a) => ColorTool(), "ColorAttributeTool"),
				new ToolStripButton("Pixel Pen", Resources.pen_tool, (s, a) => PixelTool(), "PixelPenTool"),
				new ToolStripButton("Edit Tile/Collision Map", Resources.overlay_tool, (s, a) => MetaTool(), "CollisionTool"),
				new ToolStripSeparator(),
			});
			foreach (var item in Items.OfType<ToolStripButton>()) item.DisplayStyle = ToolStripItemDisplayStyle.Image;
			foreach (var item in Items.OfType<ToolStripDropDownItem>()) item.DisplayStyle = ToolStripItemDisplayStyle.Image;
			
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

		public ToolStripSplitButton MetaTileButton { get; private set; }

		public void SetToolSettings(ToolSettings settingsControl)
		{
			if (SettingsControl != null) SettingsControl.Host.Visible = false;
			Items.Add(settingsControl.Host);
			SettingsControl = settingsControl;
			settingsControl.Host.Visible = true;
		}

		public void SetMetaTileSizes(IEnumerable<int> sizes)
		{
			MetaTileButton.DropDownItems.Clear();
			foreach (var size in sizes)
			{
				MetaTileButton.DropDownItems.Add(new ToolStripMenuItem(string.Format("{0}x{0} Tiles", size), null, (s, a) => MetaTileTool(size)));
			}
		}

		public ToolStripButton CollisionButton { get; private set; }
		public ToolStripButton GridButton { get; private set; }
		public ToolSettings SettingsControl { get; private set; }
	}
}
