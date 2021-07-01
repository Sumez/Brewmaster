﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.Layout;
using Brewmaster.Modules;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorWindow : SaveableEditorWindow, IUndoable
	{
		public static MapEditorToolBar MapEditorToolBar = new MapEditorToolBar();
		private ChrTilePalette _tilePalette;
		private MapOverview _mapOverview;
		private ColorPaletteView _colorPalette;
		private Dictionary<int, MetaTilePalette> _metaTilePalettes;
		private ScreenPanel _screenPanel;
		private ToolSettings _toolSettings;
		public override ToolStrip ToolBar { get { return MapEditorToolBar; } }
		public override LayoutMode LayoutMode { get { return LayoutMode.MapEditor; } }

		public MapEditorState State { get; set; }


		public MapEditorWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events)
		{
			State = new MapEditorState();
			if (file.Project != null) State.TargetPlatform = file.Project.Platform;

			var json = File.ReadAllText(ProjectFile.File.FullName);
			var map = JsonConvert.DeserializeObject<SerializableTileMap>(json, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
			if (map != null)
			{
				Map = map.GetMap();
				if (!string.IsNullOrWhiteSpace(map.ChrSource))
				{
					ChrSource = Path.Combine(file.Project.Directory.FullName, map.ChrSource);
				}
				Pristine = true;
			}
			else
			{
				Map = new TileMap();
				Map.Screens.Add(new List<TileMapScreen>(new[] { new TileMapScreen(Map) }));
			}

			var paletteBase = 1;
			while (Map.Palettes.Count < 4)
			{
				var palette = new Palette();
				palette.Colors.AddRange(new[]
				{
					NesColorPicker.NesPalette.Colors[0x0F],
					NesColorPicker.NesPalette.Colors[paletteBase | 0x10],
					NesColorPicker.NesPalette.Colors[paletteBase | 0x20],
					NesColorPicker.NesPalette.Colors[paletteBase | 0x30]
				});
				Map.Palettes.Add(palette);
				paletteBase += 3;
			}

			State.BitDepth = Map.BitsPerPixel;
			State.BitDepthChanged += () =>
			{
				Map.BitsPerPixel = State.BitDepth;
				foreach (var palette in Map.Palettes)
				{
					while (palette.Colors.Count < Map.ColorCount) palette.Colors.Add(Color.Black);
				}
				_colorPalette.RefreshPaletteView(Map.Palettes, Map.ColorCount, State.TargetPlatform);
				State.ClearTileCache();
				State.OnPaletteChanged();
				_screenPanel.RefreshAllTiles();
				Pristine = false;
			};

			State.Palette = Map.Palettes[0];

			State.TilesMoved += (changes, undoStep) =>
			{
				foreach (var screen in Map.GetAllScreens())
				{
					if (!screen.ReplaceTiles(changes)) continue;

					State.RefreshTileUsage(screen);
					undoStep.AddScreen(screen);
					screen.OnEditEnd();
				}
			};

			_mapOverview = new MapOverview(Map);
			_mapOverview.MapSizeChanged += () =>
			{
				Pristine = false;
				State.ClearUndoStack();
				State.RefreshTileUsage(Map);
				foreach (var metaTilePalette in _metaTilePalettes.Values)
				{
					metaTilePalette.RefreshMetaTiles();
				}
				for (var x = 0; x < Map.Width; x++)
				for (var y = 0; y < Map.Height; y++)
					EnsureScreen(x, y);

				_screenPanel.RefreshMapScreens();
			};
			_mapOverview.ActivateScreen = ActivateScreen;

			_tilePalette = new ChrTilePalette(State) { Width = 256, Height = 256, Top = 0, Left = Width - 256 };
			_tilePalette.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_tilePalette.UserSelectedTile += () => SelectTilePen(_tilePalette.SelectedTile);
			_tilePalette.HoverTile += (index) => MainWindow.WriteStatus(State.GetTileInfo(index));

			_screenPanel = new ScreenPanel(State, Map) { Dock = DockStyle.Fill };
			Controls.Add(_screenPanel);

			_colorPalette = new ColorPaletteView(Map.Palettes, Map.ColorCount, State.TargetPlatform) { Width = 256, Height = 40 };
			_colorPalette.Dock = DockStyle.Bottom;
			_colorPalette.SelectedPaletteChanged += (paletteIndex) =>
			{
				State.Palette = Map.Palettes[paletteIndex];
			};
			_colorPalette.PalettesModified += () =>
			{
				State.ClearTileCache();
				State.OnPaletteChanged();
				_screenPanel.RefreshAllTiles();
				Pristine = false;
			};
			_colorPalette.SelectedColorIndexChanged += (colorIndex, paletteIndex) => { State.ColorIndex = colorIndex; };

			State.ColorIndexChanged += () =>
			{
				_colorPalette.ShowSelectedColorIndex(State.ColorIndex);
				if (State.Tool is PixelTool pixelTool) pixelTool.UpdatePreviewSource();
			};
			State.PaletteChanged += () =>
			{
				SetToolImage(State.Tool as TilePen);
				if (State.Tool is PixelTool pixelTool) pixelTool.UpdatePreviewSource();
			};
			State.ToolChanged += () =>
			{
				MapEditorToolBar.SetSelectedTool(State.Tool);
			};

			_toolSettings = new ToolSettings(Map, State) { Dock = DockStyle.Right };
			_toolSettings.MetaValues.UserSelectedValue += SelectMetaPen;

			var bottomPanel = new Panel { AutoSize = true, Dock = DockStyle.Bottom };
			bottomPanel.Controls.Add(_colorPalette);
			//bottomPanel.Controls.Add(new HorizontalLine { Dock = DockStyle.Top });
			Controls.Add(bottomPanel);

			_metaTilePalettes = new Dictionary<int, MetaTilePalette>();
			foreach (var metaTileSize in Map.MetaTileResolutions)
			{
				var includeMetaValues = metaTileSize >= 2;
				var includeAttributes = metaTileSize >= 4;
				_metaTilePalettes[metaTileSize] = new MetaTilePalette(Map, metaTileSize, State, includeMetaValues, includeAttributes) { Width = 256, Height = 256 };
				_metaTilePalettes[metaTileSize].UserSelectedTile += () => SelectMetaTilePen(metaTileSize, _metaTilePalettes[metaTileSize].SelectedTile);
			}
			//form.LayoutHandler.ShowPanel(new IdePanel(_colorPalette) { Label = "Tile Map Palettes" });

			LoadChrSource();
			SelectTilePen(0);

			foreach (var screen in Map.GetAllScreens()) InitScreen(screen);
			State.RefreshTileUsage(Map);
			State.AfterUndo += (step) => State.RefreshTileUsage(Map);
			State.ColorIndex = 0;

			_screenPanel.AddFullMap();
			//ActivateScreen(0, 0);
		}

		public override void TabActivated()
		{
			MainWindow.LoadModule(_mapOverview, "Map Overview");
			MainWindow.LoadModule(_tilePalette, "Chr Tiles");
			foreach (var metaTile in _metaTilePalettes)
			{
				MainWindow.LoadModule(metaTile.Value, string.Format("{0}x{0} Metatiles", metaTile.Key));
			}

			MapEditorToolBar.SetToolSettings(_toolSettings);
			MapEditorToolBar.SetMetaTileSizes(Map.MetaTileResolutions);

			MapEditorToolBar.ImportImage = ImportImage;
			MapEditorToolBar.ImportChr = ImportChr;
			MapEditorToolBar.ImportMap = ImportMap;
			MapEditorToolBar.ImportJsonSession = ImportJson;
			MapEditorToolBar.ImportPalette = ImportPalette;

			MapEditorToolBar.TileTool = () => SelectTilePen(0);
			MapEditorToolBar.ColorTool = SelectPalettePen;
			MapEditorToolBar.PixelTool = SelectPixelPen;
			MapEditorToolBar.FloodFillTool = SelectFloodFill;
			MapEditorToolBar.MetaTool = SelectMetaPen;
			MapEditorToolBar.MetaTileTool = (size) => SelectMetaTilePen(size, 0);
			MapEditorToolBar.FlipTool = SelectFlipTool;

			MapEditorToolBar.GridButton.Checked = State.DisplayGrid;
			MapEditorToolBar.CollisionButton.Checked = State.DisplayMetaValues;

			MapEditorToolBar.ToggleGrid = () => { State.DisplayGrid = MapEditorToolBar.GridButton.Checked; };
			MapEditorToolBar.ToggleMetaValues = () => { State.DisplayMetaValues = MapEditorToolBar.CollisionButton.Checked; };

			MapEditorToolBar.SetSelectedTool(State.Tool);
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			foreach (var screen in Map.GetAllScreens()) screen.Unload();
		}

		private void ActivateScreen(int x, int y)
		{
			EnsureScreen(x, y);
			//_screenPanel.AddSingleScreen(Map.Screens[y][x]);
			_screenPanel.AddFullMap(new Point(x, y));
		}

		private void EnsureScreen(int x, int y)
		{
			while (y >= Map.Screens.Count) Map.Screens.Add(new List<TileMapScreen>());
			while (x >= Map.Screens[y].Count) Map.Screens[y].Add(null);

			if (Map.Screens[y][x] == null)
			{
				Map.Screens[y][x] = new TileMapScreen(Map);
				InitScreen(Map.Screens[y][x]);
				State.RefreshTileUsage(Map.Screens[y][x]);
			}
		}

		private void InitScreen(TileMapScreen screen)
		{
			screen.EditEnd += () =>
			{
				Pristine = false;
				foreach (var metaTilePalette in _metaTilePalettes.Values)
				{
					metaTilePalette.RefreshMetaTiles(screen);
				}
			};
			screen.ImageUpdated += () =>
			{
				if (Parent == null) return;
				BeginInvoke(new Action(() => _mapOverview.InvalidateScreenImage(screen)));
			};
			screen.TileChanged += (x, y, oldTile, newTile, changedGraphics) => { State.RefreshTileUsage(screen, oldTile, newTile); };
			screen.RefreshAllTiles(State);
		}

		private void SelectTilePen(int selectTile)
		{
			var tool = new TilePen();
			State.Tool = tool;
			tool.SelectedTileChanged += () =>
			{
				_tilePalette.SelectedTile = tool.SelectedTile;
				SetToolImage(tool);
			};
			tool.Unselected += () => _tilePalette.SelectedTile = -1;
			InitPaletteTool(tool);
			tool.SelectedTile = selectTile >= 0 ? selectTile : 0;
		}

		private int? _lastSelectedMetaTileSize;
		private void SelectMetaTilePen(int? metaTileSize, int selectTile)
		{
			if (metaTileSize == null)
			{
				metaTileSize = _lastSelectedMetaTileSize ?? Map.MetaTileResolutions[0];
			}

			_lastSelectedMetaTileSize = metaTileSize;
			var tool = new MetaTilePen(metaTileSize.Value);
			var tileSelector = _metaTilePalettes[metaTileSize.Value];
			State.Tool = tool;
			tool.MetaTileChanged += () =>
			{
				tileSelector.SelectMetaTile(tool.MetaTile);
				SetToolImage(tool);
			};
			tool.Unselected += () => tileSelector.SelectedTile = -1;
			InitPaletteTool(tool);
			tool.MetaTile = tileSelector.GetMetaTile(selectTile);
		}
		
		private void SetToolImage(TilePen tool)
		{
			if (tool == null) return;
			tool.SetImage(State.ChrData, Map);
		}

		private void SelectPalettePen()
		{
			var tool = new PalettePen(Map.AttributeSize);
			State.Tool = tool;
			InitPaletteTool(tool);
		}
		private void SelectPixelPen()
		{
			var tool = new PixelPen(State, Map);
			State.Tool = tool;
			InitPaletteTool(tool);
		}

		private void SelectFloodFill()
		{
			var tool = new FloodFill(State, Map);
			State.Tool = tool;
			InitPaletteTool(tool);
		}
		private void SelectFlipTool(bool vertical)
		{
			var tool = new FlipTool(State, Map, vertical);
			State.Tool = tool;
		}
		private void SelectMetaPen()
		{
			MapEditorToolBar.CollisionButton.Checked = State.DisplayMetaValues = true;
			var tool = new MetaValuePen(Map.MetaValueSize, Map.GetMetaValueColor);
			State.Tool = tool;
		}

		private void InitPaletteTool(IPaletteTool tool)
		{
			tool.GetSelectedPalette = () => _colorPalette.SelectedPaletteIndex;
			tool.SetSelectedPalette = (index) => _colorPalette.SelectedPaletteIndex = index;
		}

		private void LoadChrSource()
		{
			if (!File.Exists(ChrSource)) return;
			using (var stream = File.OpenRead(ChrSource))
			{
				var data = new byte[stream.Length];
				stream.Read(data, 0, data.Length);
				State.ChrData = data;
			}

			State.ChrWasChanged = false;
			State.ClearUndoStack();
			_screenPanel.RefreshAllTiles(); // TODO: Should probably react to ChrDataChanged event, but unsure how much overhead that creates while drawing tiles
		}

		private void TrySaveChrData()
		{
			if (ChrSource != null)
			{
				if (MessageBox.Show(string.Format("Overwrite the file '{0}'?", ProjectFile.Project.GetRelativePath(ChrSource)), "CHR data was changed", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					var newChrFile = GetChrFileName();
					if (newChrFile == null) return;
					ChrSource = newChrFile;
				}
			}
			else
			{
				if (MessageBox.Show("Save CHR data?", "CHR data was changed", MessageBoxButtons.YesNo) == DialogResult.No) return;
				var newChrFile = GetChrFileName();
				if (newChrFile == null) return;
				ChrSource = newChrFile;
			}
			using (var stream = File.Open(ChrSource, FileMode.Create))
			{
				stream.Write(State.ChrData, 0, State.ChrData.Length);
			}
			State.ChrWasChanged = false;
		}

		public string GetChrFileName()
		{
			using (var chrFileDialog = new SaveFileDialog())
			{
				chrFileDialog.DefaultExt = "*.chr";
				chrFileDialog.Filter = "CHR data|*.chr";
				chrFileDialog.InitialDirectory = ProjectFile.File.DirectoryName;
				return chrFileDialog.ShowDialog(this) == DialogResult.OK ? chrFileDialog.FileName : null;
			}
		}

		public TileMap Map { get; set; }

		private void ImportPalette()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "*.pal|*.pal|*.*|*.*";
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			using (var stream = File.OpenRead(fileName))
			{
				_colorPalette.ImportPaletteData(stream);
			}
			Pristine = false;
		}
		private void ImportJson()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "*.json|*.json|*.*|*.*";
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			var json = File.ReadAllText(fileName);
			int stageNumber = 0;
			using (var prompt = new Form { Size = new Size(100, 100) })
			{
				var textBox = new TextBox() { Width = 50, Text = "0" } ;
				prompt.Controls.Add(textBox);
				var button = new Button() { Text = "Ok", Width = 50, Top = 30 };
				prompt.Controls.Add(button);
				button.Click += (s, a) => { prompt.Close(); };

				prompt.ShowDialog(this);
				if (!int.TryParse(textBox.Text, out stageNumber)) return;
			}
			dynamic data = JObject.Parse(json);
			var paletteData = JsonConvert.DeserializeObject<List<List<List<int>>>>(data.palettes.ToString());
			for (var i = 0; i < paletteData.Count; i++)
			{
				Map.Palettes[i].Colors.Clear();
				foreach (var color in paletteData[i])
				{
					Map.Palettes[i].Colors.Add(Color.FromArgb(color[0], color[1], color[2]));
				}
			}
			var stages = JsonConvert.DeserializeObject<dynamic[]>(data.stages.ToString());
			if (stageNumber < 0 || stageNumber >= stages.Length) stageNumber = 0;
			var stage = stages[stageNumber];
			Map.Width = stage.width != null ? stage.width : stage.screens.Count;
			Map.Height = stage.height != null ? stage.height : 1;
			Map.AttributeSize = new Size(2, 2);
			Map.ScreenSize = new Size(32, stage.screens[0].tiles.Count / 32);
			Map.Screens = new List<List<TileMapScreen>>();
			List<TileMapScreen> row = null;
			foreach (var sourceScreen in stage.screens)
			{
				if (row == null || row.Count == Map.Width)
				{
					row = new List<TileMapScreen>();
					Map.Screens.Add(row);
				}
				var screen = new TileMapScreen(Map);
				row.Add(screen);
				for (var i = 0; i < sourceScreen.tiles.Count && i < screen.Tiles.Length; i++)
				{
					if (sourceScreen.tiles[i] != null) screen.Tiles[i] = sourceScreen.tiles[i];
				}
				for (var i = 0; i < sourceScreen.colors.Count && i < screen.ColorAttributes.Length; i++)
				{
					if (sourceScreen.colors[i] != null) screen.ColorAttributes[i] = sourceScreen.colors[i];
				}
				for (var i = 0; i < sourceScreen.collisions.Count && i < screen.MetaValues.Length; i++)
				{
					if (sourceScreen.collisions[i] != null) screen.MetaValues[i] = sourceScreen.collisions[i];
				}

			}
			foreach (var screen in Map.GetAllScreens()) InitScreen(screen);
			ActivateScreen(0, 0);
			_colorPalette.Invalidate();

			State.ClearUndoStack();
			State.RefreshTileUsage(Map);
			Pristine = false;
		}
		private void ImportMap()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "*.map|*.map|*.*|*.*";
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			/*
			PyxelMap map;
			using (var stream = File.OpenRead(fileName))
			{
				map = (PyxelMap) new XmlSerializer(typeof(PyxelMap)).Deserialize(stream);
			}

			foreach (var tile in map.Layers[0].Tiles)
			{
				if (tile.Index < 0) continue;
				FocusedScreen.PrintTile(tile.X, tile.Y, tile.Index);
			}*/

			using (var stream = File.OpenRead(fileName))
			{
				stream.Position = stream.Length - 4;
				var width = stream.ReadByte() | (stream.ReadByte() << 8);
				var height = stream.ReadByte() | (stream.ReadByte() << 8);
				Map.ScreenSize = new Size(width, height);
				Map.AttributeSize = new Size(2, 2);
				
				var screen = new TileMapScreen(Map);
				Map.Screens[0] = new List<TileMapScreen>(new[] { screen });


				stream.Position = 0;
				for (var y = 0; y < height; y++)
					for (var x = 0; x < width; x++)
						screen.PrintTile(x, y, stream.ReadByte());

				var attributeBytes = new byte[stream.Length - stream.Position - 4];
				stream.Read(attributeBytes, 0, attributeBytes.Length);
				for (var y = 0; y < height; y += 2)
					for (var x = 0; x < width; x += 2)
					{
						var i = x / 4 + (y / 4) * ((Map.ScreenSize.Width + 3) / 4);
						var attribute = attributeBytes[i];
						if (x % 4 == 2) attribute >>= 2;
						if (y % 4 == 2) attribute >>= 4;
						screen.SetColorTile(x, y, attribute & 3);
					}

				ActivateScreen(0, 0);
			}
			
			State.ClearUndoStack();
			Pristine = false;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (ModifierKeys.HasFlag(Keys.Control))
			{
				var zoom = State.Zoom;
				if (e.Delta > 0) zoom++;
				else zoom--;
				if (zoom < 1) zoom = 1;
				if (zoom > 40) zoom = 40;
				State.Zoom = zoom;

				return;
			}

			if (ModifierKeys.HasFlag(Keys.Shift))
			{
				_screenPanel.Pan(-e.Delta, 0);
			}
			else _screenPanel.Pan(0, -e.Delta);
		}

		private void ImportImage()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			throw new NotImplementedException();
			_screenPanel.InvalidateVisibleViews();
		}
		private void ImportChr()
		{
			using (var dialog = new OpenFileDialog())
			{
				dialog.Filter = "*.chr|*.chr|*.*|*.*";
				if (dialog.ShowDialog() != DialogResult.OK) return;
				ChrSource = dialog.FileName;
			}
			LoadChrSource();
			Pristine = false;
		}

		public string ChrSource { get; set; }

		public override void Save(Func<FileInfo, string> getNewFileName = null)
		{
			string filename;
			if (getNewFileName != null)
			{
				// TODO: Redundant across editor windows
				filename = getNewFileName(ProjectFile.File);
				if (filename == null) return;
				ProjectFile.File = new FileInfo(filename);
				ProjectFile.Project.Pristine = false;
				ModuleEvents.OnFilenameChanged(ProjectFile);
			}
			else
			{
				filename = ProjectFile.File.FullName;
			}

			var map = Map.GetSerializable();
			if (State.ChrWasChanged) TrySaveChrData();
			if (ChrSource != null) map.ChrSource = ProjectFile.Project.GetRelativePath(ChrSource);
			var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() };
			jsonSettings.Converters.Add(new CondensedArrayConverter());
			var json = JsonConvert.SerializeObject(map, jsonSettings);
			File.WriteAllText(filename, json);
			Pristine = true;
		}

		public void Undo()
		{
			State.Undo();
		}

		public void Redo()
		{
			State.Redo();
		}
	}

	public class CondensedArrayConverter : JsonConverter
	{
		public override bool CanConvert(Type type)
		{
			return type == typeof(int[]) || type == typeof(List<Color>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
		}
	}
}
