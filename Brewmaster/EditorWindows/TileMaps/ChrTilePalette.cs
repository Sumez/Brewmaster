﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Brewmaster.EditorWindows.TileMaps.Tools;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ChrTilePalette : UserControl
	{
		private readonly MapEditorState _state;
		private TilePalette _tilePalette;
		public event Action<int> HoverTile;
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
			_tilePalette.AllowTileDrag = true;

			state.ChrDataChanged += RefreshTileCount;
			state.PaletteChanged += _tilePalette.RefreshImage;

			_tilePalette.Tiles = new List<MetaTile>();
			RefreshTileCount();
			_tilePalette.TileClick += SelectTile;
			_tilePalette.TileDrag += state.MoveChrTile;
			_tilePalette.TileHover += (index) => { if (HoverTile != null) HoverTile(index); };
			_tilePalette.RemoveTile = (index) =>
			{
				var usages = state.GetTileUsage(index);
				if (usages > 0)
				{
					MessageBox.Show(string.Format("Cannot delete tile used in {0} locations", usages), "Delete CHR tile", MessageBoxButtons.OK);
					return;
				}
				state.RemoveChrTile(index);
			};
		}

		private void SelectTile(int index)
		{
			_tilePalette.SelectedTile = index;
			if (UserSelectedTile != null) UserSelectedTile();
		}

		private void RefreshTileCount()
		{
			var tiles = _tilePalette.Tiles;
			var targetTiles = _state.ChrData.Length / TileImage.GetTileDataLength();
			if (targetTiles == tiles.Count)
			{
				_tilePalette.RefreshImage();
				return;
			}

			for (var i = tiles.Count; i < targetTiles; ++i) tiles.Add(new MetaTile { Tiles = new[] { i }, Attributes = new[] { -1 } });
			while (tiles.Count > targetTiles) tiles.RemoveAt(tiles.Count - 1);

			_tilePalette.Tiles = tiles;
			if (_tilePalette.SelectedTile >= targetTiles) SelectTile(targetTiles - 1);
		}

		private void InitializeComponent()
		{
			this._tilePalette = new Brewmaster.EditorWindows.TileMaps.TilePalette();

			this.SuspendLayout();
			// 
			// _tilePalette
			// 
			this._tilePalette.State = _state;
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
