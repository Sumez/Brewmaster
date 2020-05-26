using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MetaTilePalette : UserControl
	{
		private TileMap _map;
		private readonly int _metaTileSize;
		private readonly Dictionary<TileMapScreen, List<int[]>> _screenCollections;
		private TilePalette _tilePalette;
		private List<int[]> _metaTiles;
		public int[] GetMetaTile(int index)
		{
			if (index < 0 || index >= _metaTiles.Count) return new int[0];
			return _metaTiles[index];
		}

		public event Action UserSelectedTile;
		public int SelectedTile
		{
			get { return _tilePalette.SelectedTile; }
			set { _tilePalette.SelectedTile = value; }
		}

		public void SelectMetaTile(int[] metaTile)
		{
			SelectedTile = _metaTiles.FindIndex(mt => mt.SequenceEqual(metaTile));
		}

		public MetaTilePalette(TileMap map, int metaTileSize, MapEditorState state)
		{
			_map = map;
			_metaTileSize = metaTileSize;
			_screenCollections = new Dictionary<TileMapScreen, List<int[]>>();

			InitializeComponent();

			state.ChrDataChanged += _tilePalette.RefreshImage;
			state.PaletteChanged += _tilePalette.RefreshImage;

			_tilePalette.State = state;
			_tilePalette.MetaTileWidth = metaTileSize;
			RefreshMetaTiles();
			_tilePalette.TileClick += (index) =>
			{
				_tilePalette.SelectedTile = index;
				if (UserSelectedTile != null) UserSelectedTile();
			};
		}

		private void GetMetaTilesFromScreen(TileMapScreen screen)
		{
			var metaTiles = new List<int[]>();
			for (var y = 0; y < _map.ScreenSize.Height; y += _metaTileSize)
			for (var x = 0; x < _map.ScreenSize.Width; x += _metaTileSize)
			{
				var metaTile = new int[_metaTileSize * _metaTileSize];
				metaTiles.Add(metaTile);

				for (var i = 0; i < _metaTileSize * _metaTileSize; i++)
				{
					var iX = x + (i % _metaTileSize);
					var iY = (y + i / _metaTileSize);
					if (iX >= _map.ScreenSize.Width || iY >= _map.ScreenSize.Height) metaTile[i] = -1;
					else metaTile[i] = screen.Tiles[iY * _map.ScreenSize.Width + iX];
				}
			}
			_screenCollections[screen] = metaTiles.Distinct(new MetaTileComparer()).ToList();
		}

		public void RefreshMetaTiles(TileMapScreen screen = null)
		{
			if (screen == null)
			{
				foreach (var s in _map.Screens.SelectMany(s => s).Where(s => s != null))
				{
					GetMetaTilesFromScreen(s);
				}
			}
			else GetMetaTilesFromScreen(screen);

			int[] selectedMetaTile = null;
			if (SelectedTile >= 0) selectedMetaTile = GetMetaTile(SelectedTile);
			_metaTiles = _screenCollections.Values.SelectMany(mt => mt).Distinct(new MetaTileComparer()).ToList();
			_tilePalette.Tiles = _metaTiles;
			if (selectedMetaTile != null) SelectMetaTile(selectedMetaTile);
		}

		private void InitializeComponent()
		{
			this._tilePalette = new Brewmaster.EditorWindows.TileMaps.TilePalette();
			this.SuspendLayout();
			// 
			// _tilePalette
			// 
			this._tilePalette.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tilePalette.Location = new System.Drawing.Point(3, 3);
			this._tilePalette.MetaTileWidth = 1;
			this._tilePalette.Name = "_tilePalette";
			this._tilePalette.SelectedTile = -1;
			this._tilePalette.Size = new System.Drawing.Size(339, 317);
			this._tilePalette.TabIndex = 0;
			this._tilePalette.Text = "tilePalette1";
			this._tilePalette.Tiles = null;
			this._tilePalette.Zoom = 1;
			// 
			// MetaTilePalette
			// 
			this.Controls.Add(this._tilePalette);
			this.Name = "MetaTilePalette";
			this.Size = new System.Drawing.Size(345, 323);
			this.ResumeLayout(false);

		}
	}

	public class MetaTileComparer : IEqualityComparer<int[]>
	{
		public bool Equals(int[] a, int[] b) { return a.SequenceEqual(b); }

		public int GetHashCode(int[] obj) { return 0; }
	}
}
