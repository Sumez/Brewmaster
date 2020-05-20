using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Brewmaster.Modules;
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
				Map.Screens.Add(new List<TileMapScreen>(new[] { new TileMapScreen(Map.ScreenSize, Map.BaseTileSize) }));
			}

			foreach (var screen in Map.Screens.SelectMany(l => l))
			{
				screen.TileChanged += (x, y) => Pristine = false;
			}
			FocusedScreen = Map.Screens[0][0];

			_tilePalette = new TilePalette { Width = 256, Height = 256, Top = 0, Left = Width - 256 };
			_tilePalette.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Controls.Add(_tilePalette);

			_screenView = new MapScreenView(Map, FocusedScreen) { Dock = DockStyle.Fill, Zoom = _zoom, TilePalette = _tilePalette };
			Controls.Add(_screenView);
			_screenView.ContextMenu = new ContextMenu();

			LoadChrSource();
			SelectTilePen();
		}

		private void SelectTilePen()
		{
			var tool = new TilePen();
			_screenView.Tool = tool;
			tool.SelectedTileChanged += () => { _tilePalette.SelectedTile = tool.SelectedTile; };
			_tilePalette.SelectedTile = tool.SelectedTile;
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
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(int[]);
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
