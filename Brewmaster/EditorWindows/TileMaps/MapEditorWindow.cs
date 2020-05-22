using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
		private TilePalette _tilePalette;
		private int _zoom = 2;
		private ColorPaletteView _colorPalette;
		public override ToolStrip ToolBar { get { return MapEditorToolBar; } }

		public MapEditorWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events)
		{
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
				Map.Screens.Add(new List<TileMapScreen>(new[] { new TileMapScreen(Map.ScreenSize, Map.BaseTileSize, Map.AttributeSize) }));
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


			foreach (var screen in Map.Screens.SelectMany(l => l))
			{
				screen.TileChanged += (x, y) => Pristine = false;
			}
			FocusedScreen = Map.Screens[0][0];

			_tilePalette = new TilePalette { Width = 256, Height = 256, Top = 0, Left = Width - 256 };
			_tilePalette.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			//Controls.Add(_tilePalette);
			_tilePalette.Palette = Map.Palettes[0];
			_tilePalette.UserSelectedTile += SelectTilePen;

			_colorPalette = new ColorPaletteView(Map.Palettes) { Width = 256, Height = 40 };
			_colorPalette.Dock = DockStyle.Bottom;
			Controls.Add(_colorPalette);
			_colorPalette.SelectedPaletteChanged += (paletteIndex) => _tilePalette.Palette = Map.Palettes[paletteIndex];
			_colorPalette.PalettesChanged += () =>
			{
				_tilePalette.Palette = _tilePalette.Palette;
				_screenView.RefreshAllTiles(_tilePalette);
				Pristine = false;
			};

			_screenView = new MapScreenView(Map, FocusedScreen) { Dock = DockStyle.Fill, Zoom = _zoom, TilePalette = _tilePalette };
			Controls.Add(_screenView);
			_screenView.ContextMenu = new ContextMenu();

			form.LayoutHandler.ShowPanel(new IdePanel(_tilePalette) { Label = "CHR Tiles" });
			//form.LayoutHandler.ShowPanel(new IdePanel(_colorPalette) { Label = "Tile Map Palettes" });

			LoadChrSource();
			SelectTilePen();
		}

		private void SelectTilePen()
		{
			var tool = new TilePen();
			if (_tilePalette.SelectedTile >= 0) tool.SelectedTile = _tilePalette.SelectedTile;
			_screenView.Tool = tool;
			tool.SelectedTileChanged += () => { _tilePalette.SelectedTile = tool.SelectedTile; };
			_tilePalette.SelectedTile = tool.SelectedTile;
			InitPaletteTool(tool);
		}
		private void SelectPalettePen()
		{
			var tool = new PalettePen(Map.AttributeSize);
			_screenView.Tool = tool;
			InitPaletteTool(tool);
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
				_tilePalette.ChrData = data;
			}
			_screenView.RefreshAllTiles(_tilePalette);
		}

		public TileMapScreen FocusedScreen { get; set; }

		public TileMap Map { get; set; }

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!Visible) return;
			MapEditorToolBar.ImportImage = ImportImage;
			MapEditorToolBar.ImportChr = ImportChr;
			MapEditorToolBar.ImportMap = ImportMap;

			MapEditorToolBar.TileTool = SelectTilePen;
			MapEditorToolBar.ColorTool = SelectPalettePen;
		}

		private void ImportMap()
		{
			string fileName = null;
			using (var dialog = new OpenFileDialog())
			{
				if (dialog.ShowDialog() != DialogResult.OK) return;
				fileName = dialog.FileName;
			}

			PyxelMap map;
			using (var stream = File.OpenRead(fileName))
			{
				map = (PyxelMap) new XmlSerializer(typeof(PyxelMap)).Deserialize(stream);
			}

			foreach (var tile in map.Layers[0].Tiles)
			{
				if (tile.Index < 0) continue;
				FocusedScreen.PrintTile(tile.X, tile.Y, tile.Index);
			}

			Pristine = false;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0) _zoom++;
			else _zoom--;
			if (_zoom < 1) _zoom = 1;
			if (_zoom > 50) _zoom = 50;
			_screenView.Zoom = _zoom;
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
}
