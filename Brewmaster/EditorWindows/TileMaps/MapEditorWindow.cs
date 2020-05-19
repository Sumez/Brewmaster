using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorWindow : SaveableEditorWindow
	{
		protected static MapEditorToolBar MapEditorToolBar = new MapEditorToolBar();
		private MapScreenView _screenView;
		private int _zoom = 2;
		public override ToolStrip ToolBar { get { return MapEditorToolBar; } }

		public MapEditorWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events)
		{
			Map = new TileMap();
			Map.Screens.Add(new List<TileMapScreen>(new [] { new TileMapScreen(Map.ScreenSize, Map.BaseTileSize) }));
			FocusedScreen = Map.Screens[0][0];

			_screenView = new MapScreenView(Map, FocusedScreen) { Dock = DockStyle.Fill, Zoom = _zoom };
			Controls.Add(_screenView);
		}

		public TileMapScreen FocusedScreen { get; set; }

		public TileMap Map { get; set; }

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (!Visible) return;
			MapEditorToolBar.ImportImage = ImportImage;
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

		public override void Save(Func<FileInfo, string> getNewFileName = null)
		{
			
		}
	}

	public class TileMap
	{
		public Size BaseTileSize = new Size(8, 8);
		public Size AttributeSize = new Size(2, 2);
		public Size ScreenSize = new Size(32, 30);
		public int BitsPerPixel = 2;
		public int Colors { get { return (int)Math.Pow(2, BitsPerPixel); } }
		public List<List<TileMapScreen>> Screens = new List<List<TileMapScreen>>();
	}

	public class TileMapScreen
	{
		public TileMapScreen(Size screenSize, Size tileSize)
		{
			Tiles = new int[screenSize.Width * screenSize.Height];
			Image = new Bitmap(screenSize.Width * tileSize.Width, screenSize.Height * tileSize.Height);
		}

		public Bitmap Image;
		public int[] Tiles;
	}
}
