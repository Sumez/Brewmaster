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

			state.ChrDataChanged += _tilePalette.RefreshImage;
			state.PaletteChanged += _tilePalette.RefreshImage;

			var tiles = new List<int[]>();
			for (var i = 0; i < 256; i++) tiles.Add(new [] { i });
			_tilePalette.State = state;
			_tilePalette.Tiles = tiles;
			_tilePalette.TileClick += (index) =>
			{
				_tilePalette.SelectedTile = index;
				if (UserSelectedTile != null) UserSelectedTile();
			};
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
