﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Brewmaster.Layout;
using Brewmaster.Modules;
using Brewmaster.Modules.Ppu;
using Brewmaster.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorWindow : SaveableEditorWindow
	{
		protected static MapEditorToolBar MapEditorToolBar = new MapEditorToolBar();
		private MapScreenView _screenView;
		private ChrTilePalette _tilePalette;
		private MapOverview _mapOverview;
		private ColorPaletteView _colorPalette;
		private Dictionary<int, MetaTilePalette> _metaTilePalettes;
		private ScreenPanel _screenPanel;
		public override ToolStrip ToolBar { get { return MapEditorToolBar; } }
		public override LayoutMode LayoutMode { get { return LayoutMode.MapEditor; } }

		public MapEditorState State { get; set; }


		public MapEditorWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events)
		{
			State = new MapEditorState();
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


			State.Palette = Map.Palettes[0];

			_mapOverview = new MapOverview(Map);
			_mapOverview.MapSizeChanged += () => { Pristine = false; };
			_mapOverview.ActivateScreen = ActivateScreen;

			_tilePalette = new ChrTilePalette(State) { Width = 256, Height = 256, Top = 0, Left = Width - 256 };
			_tilePalette.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			//Controls.Add(_tilePalette);
			_tilePalette.UserSelectedTile += () => SelectTilePen(_tilePalette.SelectedTile);

			_screenPanel = new ScreenPanel(State, Map) { Dock = DockStyle.Fill };
			Controls.Add(_screenPanel);

			_colorPalette = new ColorPaletteView(Map.Palettes) { Width = 256, Height = 40 };
			_colorPalette.Dock = DockStyle.Bottom;
			Controls.Add(_colorPalette);
			_colorPalette.SelectedPaletteChanged += (paletteIndex) =>
			{
				State.Palette = Map.Palettes[paletteIndex];
				SetToolImage(_screenView.Tool as TilePen);
			};
			_colorPalette.PalettesChanged += () =>
			{
				State.ClearTileCache();
				State.OnPaletteChanged();
				_screenView.RefreshAllTiles();
				SetToolImage(_screenView.Tool as TilePen);
				if (_screenView.Tool is PixelPen pixelPen) pixelPen.PrepareImages(Map);
				Pristine = false;
			};

			_metaTilePalettes = new Dictionary<int, MetaTilePalette>();
			foreach (var metaTileSize in Map.MetaTileResolutions)
			{
				var includeMetaValues = metaTileSize >= 2;
				var includeAttributes = metaTileSize >= 4;
				_metaTilePalettes[metaTileSize] = new MetaTilePalette(Map, metaTileSize, State, includeMetaValues, includeAttributes) { Width = 256, Height = 256 };
				_metaTilePalettes[metaTileSize].UserSelectedTile += () => SelectMetaTilePen(metaTileSize, _metaTilePalettes[metaTileSize], _metaTilePalettes[metaTileSize].SelectedTile);
			}
			//form.LayoutHandler.ShowPanel(new IdePanel(_colorPalette) { Label = "Tile Map Palettes" });

			LoadChrSource();
			SelectTilePen(0);

			foreach (var screen in Map.Screens.SelectMany(l => l).Where(s => s != null)) InitScreen(screen);
			ActivateScreen(0, 0);
		}

		public override void TabActivated()
		{
			MainWindow.LoadModule(_mapOverview, "Map Overview");
			MainWindow.LoadModule(_tilePalette, "Chr Tiles");
			foreach (var metaTile in _metaTilePalettes)
			{
				MainWindow.LoadModule(metaTile.Value, string.Format("{0}x{0} Metatiles", metaTile.Key));
			}

			MapEditorToolBar.ImportImage = ImportImage;
			MapEditorToolBar.ImportChr = ImportChr;
			MapEditorToolBar.ImportMap = ImportMap;
			MapEditorToolBar.ImportPalette = ImportPalette;

			MapEditorToolBar.TileTool = () => SelectTilePen(0);
			MapEditorToolBar.ColorTool = SelectPalettePen;
			MapEditorToolBar.PixelTool = SelectPixelPen;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			foreach (var screen in Map.Screens.SelectMany(s => s)) screen.Unload();
		}

		private void ActivateScreen(int x, int y)
		{
			while (y >= Map.Screens.Count) Map.Screens.Add(new List<TileMapScreen>());
			while (x >= Map.Screens[y].Count) Map.Screens[y].Add(null);

			if (Map.Screens[y][x] == null)
			{
				Map.Screens[y][x] = new TileMapScreen(Map);
				InitScreen(Map.Screens[y][x]);
			}
			FocusedScreen = Map.Screens[y][x];
			_screenView = new MapScreenView(Map, FocusedScreen, State);
			_screenView.ContextMenu = new ContextMenu();
			_screenPanel.Add(_screenView);
			_screenView.RefreshAllTiles();
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
				BeginInvoke(new Action(() => _mapOverview.UpdateScreenImage(screen)));
			};
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
		private void SelectMetaTilePen(int metaTileSize, MetaTilePalette tileSelector, int selectTile)
		{
			var tool = new MetaTilePen(metaTileSize);
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
			var tool = new PixelPen();
			tool.PrepareImages(Map);
			State.Tool = tool;
			tool.SelectedColor = 3;
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
			if (_screenView != null) _screenView.RefreshAllTiles();
		}

		public TileMapScreen FocusedScreen { get; set; }

		public TileMap Map { get; set; }

		private void ImportPalette()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			using (var stream = File.OpenRead(fileName))
			{
				_colorPalette.ImportPaletteData(stream);
			}
			Pristine = false;
		}
		private void ImportMap()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
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
						var i = x / 4 + (y / 4) * (Map.ScreenSize.Width / 4);
						var attribute = attributeBytes[i];
						if (x % 4 == 2) attribute >>= 2;
						if (y % 4 == 2) attribute >>= 4;
						screen.SetColorTile(x, y, attribute & 3);
					}

				ActivateScreen(0, 0);
			}

			Pristine = false;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			var zoom = State.Zoom;
			if (e.Delta > 0) zoom++;
			else zoom--;
			if (zoom < 1) zoom = 1;
			if (zoom > 20) zoom = 20;
			State.Zoom = zoom;
		}

		private void ImportImage()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			using (var sourceImage = new Bitmap(fileName))
			{
				using (var graphics = Graphics.FromImage(FocusedScreen.Image))
				{
					graphics.DrawImageUnscaled(sourceImage, 0, 0);
				}
			}
			_screenView.Invalidate();
		}
		private void ImportChr()
		{
			using (var dialog = new OpenFileDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK) return;
				ChrSource = dialog.FileName;
			}
			LoadChrSource();
			Pristine = false;
		}

		public string ChrSource { get; set; }

		public override void Save(Func<FileInfo, string> getNewFileName = null)
		{
			var map = Map.GetSerializable();
			map.ChrSource = ProjectFile.Project.GetRelativePath(ChrSource);
			var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() };
			jsonSettings.Converters.Add(new CondensedArrayConverter());
			var json = JsonConvert.SerializeObject(map, jsonSettings);
			File.WriteAllText(ProjectFile.File.FullName, json);
			Pristine = true;
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

	public class MapEditorState
	{
		private MapEditorTool _tool;
		private byte[] _chrData = new byte[0x1000];
		private Palette _palette;
		private int _zoom = 2;

		public MapEditorTool Tool
		{
			get { return _tool; }
			set
			{
				if (_tool != null) _tool.Unselect();
				_tool = value;
				OnToolChanged();
			}
		}
		public byte[] ChrData
		{
			get { return _chrData; }
			set
			{
				_chrData = value;
				ClearTileCache();
				OnChrDataChanged();
			}
		}

		public Palette Palette
		{
			get { return _palette; }
			set
			{
				_palette = value;
				OnPaletteChanged();
			}
		}

		public int Zoom
		{
			get { return _zoom; }
			set { _zoom = value; OnZoomChanged(); }
		}


		public event Action PaletteChanged;
		public event Action ChrDataChanged;
		public event Action ToolChanged;
		public event Action ZoomChanged;
		public void OnPaletteChanged()
		{
			if (PaletteChanged != null) PaletteChanged();
		}
		public void OnChrDataChanged()
		{
			if (ChrDataChanged != null) ChrDataChanged();
		}
		public void OnToolChanged()
		{
			if (ToolChanged != null) ToolChanged();
		}
		public void OnZoomChanged()
		{
			if (ZoomChanged != null) ZoomChanged();
		}

		private Dictionary<int, Dictionary<Palette, Image>> _cachedTiles = new Dictionary<int, Dictionary<Palette, Image>>();
		public Image GetTileImage(int index, Palette palette)
		{
			lock (_cachedTiles)
			{
				if (!_cachedTiles.ContainsKey(index)) _cachedTiles.Add(index, new Dictionary<Palette, Image>());
				if (!_cachedTiles[index].ContainsKey(palette)) _cachedTiles[index].Add(palette, TilePalette.GetTileImage(ChrData, index, palette.Colors));
			}
			return _cachedTiles[index][palette];
		}
		public void ClearTileCache()
		{
			IEnumerable<Image> oldImages;
			lock (_cachedTiles)
			{
				oldImages = _cachedTiles.Values.SelectMany(x => x.Values);
				_cachedTiles = new Dictionary<int, Dictionary<Palette, Image>>();
			}
			Task.Run(() => { foreach (var image in oldImages) image.Dispose(); });
		}
	}

}
