using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MetaTilePalette : UserControl
	{
		private TileMap _map;
		private readonly int _metaTileSize;
		private readonly bool _includeMetaValues;
		private readonly bool _includeAttributes;
		private readonly Dictionary<TileMapScreen, List<MetaTile>> _screenCollections;
		private TilePalette _tilePalette;
		private List<MetaTile> _metaTiles;
		public MetaTile GetMetaTile(int index)
		{
			if (index < 0 || index >= _metaTiles.Count) return new MetaTile();
			return _metaTiles[index].Clone();
		}

		public event Action UserSelectedTile;
		public int SelectedTile
		{
			get { return _tilePalette.SelectedTile; }
			set { _tilePalette.SelectedTile = value; }
		}

		public void SelectMetaTile(MetaTile metaTile)
		{
			SelectedTile = _metaTiles.FindIndex(mt => mt.Equals(metaTile));
		}
		
		public MetaTilePalette(TileMap map, int metaTileSize, MapEditorState state, bool includeMetaValues, bool includeAttributes)
		{
			_map = map;
			_metaTileSize = metaTileSize;
			_includeMetaValues = includeMetaValues;
			_includeAttributes = includeAttributes;
			_screenCollections = new Dictionary<TileMapScreen, List<MetaTile>>();

			InitializeComponent();

			state.ChrDataChanged += _tilePalette.RefreshImage;
			state.PaletteChanged += _tilePalette.RefreshImage;

			_tilePalette.State = state;
			_tilePalette.MetaTileWidth = metaTileSize;
			_tilePalette.GetTileColors = (tile, i) =>
			{
				var x = (i % metaTileSize) / map.AttributeSize.Width;
				var y = (i / metaTileSize) / map.AttributeSize.Height;
				i = y * (metaTileSize / map.AttributeSize.Width) + x;
				return (tile.Attributes[i] & 0xff) == 0xff ? state.Palette : map.Palettes[tile.Attributes[i] & 0xff];
			};
			RefreshMetaTiles();
			_tilePalette.TileClick += (index) =>
			{
				_tilePalette.SelectedTile = index;
				if (UserSelectedTile != null) UserSelectedTile();
			};
		}

		private void GetMetaTilesFromScreen(TileMapScreen screen, CancellationToken token)
		{
			var metaTiles = new List<MetaTile>();
			var metaTileRow = _map.ScreenSize.Width / _metaTileSize;
			var metaTileCol = _map.ScreenSize.Height / _metaTileSize;
			for (var y = 0; y < metaTileCol; y++)
			for (var x = 0; x < metaTileRow; x++)
			{
				var metaTile = screen.GetMetaTile(x, y, _metaTileSize);
				if (metaTile.Attributes.Length == 1) metaTile.Attributes[0] = metaTile.Attributes[0] & 0xff00 | 0xff;
				metaTiles.Add(metaTile);

				if (token.IsCancellationRequested) return;
			}
			var distinctMetaTiles = metaTiles.Distinct().TakeWhile((tile, i) => !token.IsCancellationRequested).ToList();
			if (token.IsCancellationRequested) return;
			lock (_allMetaTilesLock) _screenCollections[screen] = distinctMetaTiles;
		}

		private Task _refreshAllTask;
		private readonly Dictionary<TileMapScreen, Action> _cancelRefresh = new Dictionary<TileMapScreen, Action>();

		public void RefreshMetaTiles(TileMapScreen screen = null)
		{
			if (screen != null && _cancelRefresh.ContainsKey(screen)) _cancelRefresh[screen]();
			if (_refreshAllTask != null)
			{
				_refreshAllTask.Wait();
				_refreshAllTask = null;
			}
			
			var tokenSource = new CancellationTokenSource();
			if (screen != null) _cancelRefresh[screen] = () => tokenSource.Cancel();

			var token = tokenSource.Token;
			var task = Task.Run(() =>
			{
				if (screen == null)
				{
					foreach (var s in _map.Screens.SelectMany(s => s).Where(s => s != null))
					{
						GetMetaTilesFromScreen(s, token);
					}
				}
				else GetMetaTilesFromScreen(screen, token);

				MetaTile selectedMetaTile = null;
				if (SelectedTile >= 0) selectedMetaTile = GetMetaTile(SelectedTile);
				lock (_allMetaTilesLock)
				{
					var distinctMetaTiles = _screenCollections.Values.SelectMany(mt => mt).Distinct()
						.TakeWhile((tile, i) => !token.IsCancellationRequested).ToList();
					if (token.IsCancellationRequested) return;

					_metaTiles = distinctMetaTiles;
					_tilePalette.Tiles = _metaTiles;
					if (selectedMetaTile != null) SelectMetaTile(selectedMetaTile);
				}

			}, token);
			if (screen == null) _refreshAllTask = task;
		}

		private object _allMetaTilesLock = new object();

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

	public class MetaTileComparer : IEqualityComparer<MetaTile>
	{
		public bool Equals(MetaTile a, MetaTile b) { return a.Equals(b); }

		public int GetHashCode(MetaTile obj) { return obj.GetHashCode(); }
	}
}
