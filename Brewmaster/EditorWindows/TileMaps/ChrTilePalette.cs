using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ChrTilePalette : UserControl
	{
		private MapEditorState _state;
		private TilePalette _tilePalette;
		public event Action UserSelectedTile;
		public int SelectedTile
		{
			get { return _tilePalette.SelectedTile; }
			set { _tilePalette.SelectedTile = value; }
		}


		public ChrTilePalette(MapEditorState state)
		{
			_state = state;
			InitializeComponent();

			state.ChrDataChanged += () =>
			{
				RefreshTileCount();
				_tilePalette.RefreshImage();
			};
			state.PaletteChanged += _tilePalette.RefreshImage;

			_tilePalette.Tiles = new List<MetaTile>();
			_tilePalette.State = state;
			RefreshTileCount();
			_tilePalette.TileClick += (index) =>
			{
				_tilePalette.SelectedTile = index;
				if (UserSelectedTile != null) UserSelectedTile();
			};
		}

		private void RefreshTileCount()
		{
			var tiles = _tilePalette.Tiles;
			var targetTiles = _state.ChrData.Length / TileImage.GetTileDataLength();
			for (var i = tiles.Count; i < targetTiles; ++i) tiles.Add(new MetaTile { Tiles = new[] { i }, Attributes = new[] { -1 } });
			while (tiles.Count > targetTiles) tiles.RemoveAt(tiles.Count - 1);
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
			this._tilePalette.Size = new System.Drawing.Size(564, 507);
			this._tilePalette.TabIndex = 0;
			this._tilePalette.Text = "tilePalette1";
			this._tilePalette.Tiles = null;
			this._tilePalette.Zoom = 2;
			// 
			// ChrTilePalette
			// 
			this.Controls.Add(this._tilePalette);
			this.Name = "ChrTilePalette";
			this.Size = new System.Drawing.Size(570, 513);
			this.ResumeLayout(false);

		}
	}
}
