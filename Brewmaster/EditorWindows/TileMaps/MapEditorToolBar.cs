using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Controls;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.Properties;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorToolBar : AutoFocusToolStrip
	{
		private readonly ToolStripDropDownItem _importMenu;
		private readonly ToolStripButton _tileToolButton;
		private readonly ToolStripButton _attributeToolButton;
		private readonly ToolStripButton _metaDataToolButton;
		private readonly ToolStripButton _penToolButton;
		private readonly ToolStripButton _fillToolButton;
		private readonly ToolStripButton _flipHToolButton;
		private readonly ToolStripButton _flipVToolButton;

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
		public Action FloodFillTool { get; set; }
		public Action MetaTool { get; set; }
		public Action<bool> FlipTool { get; set; }

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
				_tileToolButton = new ToolStripButton("Draw Tiles", Resources.tile_tool, (s, a) => TileTool(), "TileTool"),
				MetaTileButton = new CheckedToolStripSplitButton("Draw MetaTiles", Resources.metatile_tool, (s, a) => MetaTileTool(null), "MetaTileTool"),
				_attributeToolButton = new ToolStripButton("Set Color Attribute", Resources.palette_tool, (s, a) => ColorTool(), "ColorAttributeTool"),
				_metaDataToolButton = new ToolStripButton("Edit Tile/Collision Map", Resources.overlay_tool, (s, a) => MetaTool(), "CollisionTool"),
				new ToolStripSeparator(),
				_penToolButton = new ToolStripButton("Pixel Pen", Resources.pen_tool, (s, a) => PixelTool(), "PixelPenTool"),
				_fillToolButton = new ToolStripButton("Flood Fill", Resources.bucket_tool, (s, a) => FloodFillTool(), "FloodFillTool"),
				new ToolStripSeparator(),
				_flipHToolButton = new ToolStripButton("Flip Horizontal", Resources.flip_h, (s, a) => FlipTool(false), "FlipHorizontalTool"),
				_flipVToolButton = new ToolStripButton("Flip Vertical", Resources.flip_v, (s, a) => FlipTool(true), "FlipVerticalTool"),
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
		public void SetSelectedTool(MapEditorTool tool)
		{
			_tileToolButton.Checked = tool is TilePen && !(tool is MetaTilePen);
			MetaTileButton.Checked = tool is MetaTilePen;
			foreach (var kvp in _metaTileButtons) kvp.Value.CheckState = tool is MetaTilePen metaTileTool && metaTileTool.MetaTileSize == kvp.Key ? CheckState.Indeterminate : CheckState.Unchecked;
			_attributeToolButton.Checked = tool is PalettePen;
			_metaDataToolButton.Checked = tool is MetaValuePen;
			_penToolButton.Checked = tool is PixelPen;
			_fillToolButton.Checked = tool is FloodFill;
			_flipHToolButton.Checked = tool is FlipTool flip1 && !flip1.Vertical;
			_flipVToolButton.Checked = tool is FlipTool flip2 && flip2.Vertical;
		}


		public CheckedToolStripSplitButton MetaTileButton { get; private set; }

		public void SetToolSettings(ToolSettings settingsControl)
		{
			if (SettingsControl != null) SettingsControl.Host.Visible = false;
			Items.Add(settingsControl.Host);
			SettingsControl = settingsControl;
			settingsControl.Host.Visible = true;
		}
		private Dictionary<int, ToolStripMenuItem> _metaTileButtons = new Dictionary<int, ToolStripMenuItem>();
		public void SetMetaTileSizes(IEnumerable<int> sizes)
		{
			MetaTileButton.DropDownItems.Clear();
			_metaTileButtons.Clear();
			foreach (var size in sizes)
			{
				MetaTileButton.DropDownItems.Add(_metaTileButtons[size] = new ToolStripMenuItem(string.Format("{0}x{0} Tiles", size), null, (s, a) => MetaTileTool(size)));
			}
		}

		public ToolStripButton CollisionButton { get; private set; }
		public ToolStripButton GridButton { get; private set; }
		public ToolSettings SettingsControl { get; private set; }
	}
}
