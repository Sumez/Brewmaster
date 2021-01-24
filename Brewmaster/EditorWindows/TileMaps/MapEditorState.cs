using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.Modules.Ppu;
using Brewmaster.Modules.Watch;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MapEditorState
	{
		private Dictionary<int, Dictionary<Palette, TileImage>> _cachedTiles = new Dictionary<int, Dictionary<Palette, TileImage>>();

		private MapEditorTool _tool;
		private byte[] _chrData = new byte[16]; // TODO: Set to size of single tile when opening a "clean" map with no CHR reference
		private Palette _palette;
		private int _zoom = 2;
		private bool _displayGrid = true;
		private bool _displayMetaValues = false;


		public MapEditorTool Tool
		{
			get { return _tool; }
			set
			{
				if (_tool != null)
				{
					if (_tool is PixelTool oldPixelTool && value is PixelTool newPixelTool)
					{
						// TODO: Store selected color in global palette control along with selected palette, instead of this crappy solution :)
						newPixelTool.SelectedColor = oldPixelTool.SelectedColor;
					}
					_tool.Unselect();
				}
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
				RefreshPreviousChrState();
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
			set
			{
				var oldValue = _zoom; _zoom = value; OnZoomChanged(oldValue, _zoom); }
		}

		public bool DisplayMetaValues
		{
			get { return _displayMetaValues; }
			set
			{
				_displayMetaValues = value;
				OnDisplayMetaValuesChanged();
			}
		}

		public bool DisplayGrid
		{
			get { return _displayGrid; }
			set
			{
				_displayGrid = value;
				OnDisplayGridChanged();
			}
		}

		public byte[] PreviousChrData { get; private set; }
		public bool ChrWasChanged { get; set; }

		public event Action PaletteChanged;
		public event Action ChrDataChanged;
		public event Action ToolChanged;
		public event Action<int, int> ZoomChanged;
		public event Action DisplayMetaValuesChanged;
		public event Action DisplayGridChanged;
		public event Action<UndoStep> AfterUndo;
		public event Action<Dictionary<int, int>, UndoStep> TilesMoved;

		public void OnPaletteChanged()
		{
			if (PaletteChanged != null) PaletteChanged();
		}
		public void OnChrDataChanged()
		{
			ChrWasChanged = true;
			if (ChrDataChanged != null) ChrDataChanged();
		}
		public void OnToolChanged()
		{
			if (ToolChanged != null) ToolChanged();
		}
		public void OnZoomChanged(int oldValue, int newValue)
		{
			if (ZoomChanged != null) ZoomChanged(oldValue, newValue);
		}
		private void OnDisplayMetaValuesChanged()
		{
			if (DisplayMetaValuesChanged != null) DisplayMetaValuesChanged();
		}
		private void OnDisplayGridChanged()
		{
			if (DisplayGridChanged != null) DisplayGridChanged();
		}


		public TileImage GetTileImage(int index, Palette palette)
		{
			lock (_cachedTiles)
			{
				if (!_cachedTiles.ContainsKey(index)) _cachedTiles.Add(index, new Dictionary<Palette, TileImage>());
				if (!_cachedTiles[index].ContainsKey(palette)) _cachedTiles[index].Add(palette, TileImage.GetTileImage(ChrData, index, palette.Colors));
			}
			return _cachedTiles[index][palette];
		}
		public void ClearTileCache()
		{
			IEnumerable<TileImage> oldImages;
			lock (_cachedTiles)
			{
				oldImages = _cachedTiles.Values.SelectMany(x => x.Values).ToList();
				_cachedTiles = new Dictionary<int, Dictionary<Palette, TileImage>>();
			}
			Task.Run(() => { foreach (var image in oldImages.Where(i => i != null)) image.Dispose(); });
		}

		public int GetPixel(int tileIndex, int x, int y)
		{
			return TileImage.GetTilePixel(ChrData, tileIndex, x, y);
		}

		public void SetPixel(int tileIndex, int x, int y, int colorIndex)
		{
			TileImage.SetTilePixel(ChrData, tileIndex, x, y, colorIndex);
			lock (_cachedTiles)
				foreach (var tileCache in _cachedTiles[tileIndex])
				{
					// It's a little cheaper to just modify the cached image than to clear the cache entirely
					tileCache.Value.SetPixel(x, y, tileCache.Key.Colors[colorIndex]);
				}
		}
		public void FlipTile(int tileIndex, bool vertical)
		{
			TileImage.FlipTile(ChrData, tileIndex, vertical);

			lock (_cachedTiles)
				_cachedTiles.Remove(tileIndex);
		}

		private readonly Dictionary<TileMapScreen, Dictionary<int, int>> _screenTileUsage = new Dictionary<TileMapScreen, Dictionary<int, int>>();
		private Dictionary<int, int> _tileUsage = new Dictionary<int, int>();
		public void RefreshTileUsage(TileMapScreen screen, int oldTile, int newTile)
		{
			if (oldTile >= 0)
			{
				_screenTileUsage[screen][oldTile]--;
				_tileUsage[oldTile]--;
			}

			if (newTile < 0) return;

			if (_screenTileUsage[screen].ContainsKey(newTile)) _screenTileUsage[screen][newTile]++;
			else _screenTileUsage[screen][newTile] = 1;

			if (_tileUsage.ContainsKey(newTile)) _tileUsage[newTile]++;
			else _tileUsage[newTile] = 1;	
		}
		public void RefreshTileUsage(TileMapScreen screen)
		{
			_screenTileUsage[screen] = screen.Tiles.GroupBy(tile => tile).ToDictionary(g => g.Key, g => g.ToArray().Length);
			_tileUsage = _screenTileUsage.SelectMany(kvp => kvp.Value).GroupBy(kvp => kvp.Key, kvp => kvp.Value).ToDictionary(g => g.Key, g => g.Sum());
		}

		public void RefreshTileUsage(TileMap map)
		{
			// TODO: Remove screens from list if removed from full map
			_screenTileUsage.Clear();
			foreach (var screen in map.GetAllScreens())
			{
				_screenTileUsage[screen] = screen.Tiles.GroupBy(tile => tile).ToDictionary(g => g.Key, g => g.ToArray().Length);
			}
			_tileUsage = _screenTileUsage.SelectMany(kvp => kvp.Value).GroupBy(kvp => kvp.Key, kvp => kvp.Value).ToDictionary(g => g.Key, g => g.Sum());
		}

		public int GetTileUsage(int tile)
		{
			return _tileUsage.ContainsKey(tile) ? _tileUsage[tile] : 0;
		}

		public int CopyTile(int tile)
		{
			var tileSize = TileImage.GetTileDataLength();
			var newData = new byte[_chrData.Length + tileSize];
			Buffer.BlockCopy(_chrData, 0, newData, 0, _chrData.Length);
			Buffer.BlockCopy(_chrData, tileSize * tile, newData, _chrData.Length, tileSize);
			_chrData = newData;
			return (newData.Length / tileSize) - 1;
		}
		public void MoveChrTile(int fromTile, int toTile)
		{
			var tileSize = TileImage.GetTileDataLength();
			var length = Math.Abs(fromTile - toTile);
			var sourceOffset = fromTile > toTile ? toTile : (fromTile + 1);
			var destinationOffset = fromTile > toTile ? (toTile + 1) : fromTile;

			var buffer = new byte[tileSize];

			//Below code copies inbetween tiles to a temporary buffer. But it seems .NET's blockcopy works fine without it
			//Buffer.BlockCopy(_chrData, sourceOffset * tileSize, buffer, 0, length * tileSize);
			//Buffer.BlockCopy(buffer, 0, _chrData, destinationOffset * tileSize, length * tileSize);

			Buffer.BlockCopy(_chrData, fromTile * tileSize, buffer, 0, tileSize); // Backup moved tile
			Buffer.BlockCopy(_chrData, sourceOffset * tileSize, _chrData, destinationOffset * tileSize, length * tileSize); // Shift inbetween tiles
			Buffer.BlockCopy(buffer, 0, _chrData, toTile * tileSize, tileSize); // Restore moved tile at new location

			var changedTiles = new Dictionary<int, int>();
			changedTiles.Add(fromTile, toTile);
			for (var i = fromTile; i < toTile; i++) changedTiles.Add(i + 1, i);
			for (var i = fromTile; i > toTile; i--) changedTiles.Add(i - 1, i);

			AdjustForMovedTiles(changedTiles);
		}
		public void RemoveChrTile(int tile)
		{
			if (GetTileUsage(tile) > 0) return;

			var tileSize = TileImage.GetTileDataLength();
			var tileCount = _chrData.Length / tileSize;
			if (tileCount == 1) return;

			var chrData = new byte[_chrData.Length - tileSize];
			var moveTiles = tileCount - tile - 1;

			Buffer.BlockCopy(_chrData, 0, chrData, 0, tile * tileSize);
			if (moveTiles > 0) Buffer.BlockCopy(_chrData, (tile + 1) * tileSize, chrData, tile * tileSize, moveTiles * tileSize);
			_chrData = chrData; // Don't use ChrData setter, as this method manually controls when events are fired and previous chr buffer is updated

			var changedTiles = new Dictionary<int, int>();
			for (var i = tile + 1; i < tileCount; i++) changedTiles.Add(i, i - 1);

			lock (_cachedTiles) _cachedTiles.Remove(tile);
			AdjustForMovedTiles(changedTiles);
		}

		private void AdjustForMovedTiles(Dictionary<int, int> changedTiles)
		{
			var undoStep = new UndoStep();
			undoStep.AddChr(this);
			lock (_cachedTiles)
				foreach (var changedTile in changedTiles.Keys) _cachedTiles.Remove(changedTile);
			if (TilesMoved != null) TilesMoved(changedTiles, undoStep);
			AddUndoStep(undoStep);

			OnChrDataChanged();
		}

		public string GetTileInfo(int tileIndex)
		{
			if (tileIndex < 0) return null;
			return string.Format("CHR tile: {0}. Usages in map: {1}",
				WatchValue.FormatHex(tileIndex, 2), GetTileUsage(tileIndex));
		}


		private readonly LinkedList<UndoStep> _undoStack = new LinkedList<UndoStep>();
		private readonly LinkedList<UndoStep> _redoStack = new LinkedList<UndoStep>();
		
		public void AddUndoStep(UndoStep undoStep)
		{
			_redoStack.Clear();
			_undoStack.AddLast(undoStep);
			if (_undoStack.Count > 100) _undoStack.RemoveFirst();
		}

		public void ClearUndoStack()
		{
			_undoStack.Clear();
			_redoStack.Clear();
		}
		public void Undo()
		{
			if (_undoStack.Count == 0) return;

			var step = _undoStack.Last.Value;
			_undoStack.RemoveLast();
			var redoStep = step.Revert(this);
			_redoStack.AddLast(redoStep);

			if (AfterUndo != null) AfterUndo(step);
		}

		public void Redo()
		{
			if (_redoStack.Count == 0) return;

			var step = _redoStack.Last.Value;
			_redoStack.RemoveLast();
			var undoStep = step.Revert(this);
			_undoStack.AddLast(undoStep);

			if (AfterUndo != null) AfterUndo(step);
		}

		public void RefreshPreviousChrState()
		{
			PreviousChrData = new byte[ChrData.Length];
			Buffer.BlockCopy(ChrData, 0, PreviousChrData, 0, Buffer.ByteLength(ChrData));
		}

		public void RevertChr(byte[] chr)
		{
			var chrData = new byte[chr.Length];
			Buffer.BlockCopy(chr, 0, chrData, 0, Buffer.ByteLength(chr));
			ChrData = chrData;
		}
	}
}