using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TileMap
	{
		public int Width = 1;
		public int Height = 1;
		public List<int> MetaTileResolutions = new List<int> {2, 4};
		public List<Palette> Palettes = new List<Palette>();
		public Size BaseTileSize = new Size(8, 8);
		public Size AttributeSize = new Size(2, 2);
		public Size MetaValueSize = new Size(2, 2);
		public Size ScreenSize = new Size(32, 30);
		public int BitsPerPixel = 2;
		public int ColorCount { get { return (int)Math.Pow(2, BitsPerPixel); } }
		public List<List<TileMapScreen>> Screens = new List<List<TileMapScreen>>();
		public Color[] MetaValueColors = { Color.FromArgb(0, 0, 0, 0), Color.FromArgb(128, 255, 255, 255), Color.FromArgb(128, 255, 0, 0), Color.FromArgb(128, 0, 0, 255) };


		public SerializableTileMap GetSerializable()
		{
			return new SerializableTileMap
			{
				Width = Width,
				Height = Height,
				ScreenSize = ScreenSize,
				AttributeSize = AttributeSize,
				MetaValueSize = MetaValueSize,
				BitsPerPixel = BitsPerPixel,
				Screens = GetScreenArray(),
				Palettes = Palettes.Select(p => p.Colors).ToList()
			};
		}

		private SerializableScreen[] GetScreenArray()
		{
			var screens = new SerializableScreen[Width * Height];
			for (var y = 0; y < Height; y++)
			for (var x = 0; x < Width; x++)
			{
				if (Screens.Count <= y || Screens[y].Count <= x || Screens[y][x] == null) continue;
				screens[y * Width + x] = new SerializableScreen
				{
					Tiles = Screens[y][x].Tiles,
					ColorAttributes = Screens[y][x].ColorAttributes,
					MetaValues = Screens[y][x].MetaValues,
					Objects = Screens[y][x].Objects
				};
			}
			return screens;
		}

		public Color GetMetaValueColor(int value)
		{
			return MetaValueColors.Length <= value ? MetaValueColors[1] : MetaValueColors[value];
		}
	}
	public class TileMapScreen
	{
		public int[] Tiles;
		public int[] ColorAttributes;
		public int[] MetaValues;
		public MapObject[] Objects;

		public FastBitmap Image;
		public FastBitmap MetaImage;

		public event TileChange TileChanged;
		public event Action EditEnd;
		public event Action ImageUpdated;

		public delegate void TileChange(int x, int y, int oldTile, int newTile);

		private readonly TileMap _map;

		public TileMapScreen(TileMap map)
		{
			_map = map;
			Tiles = new int[map.ScreenSize.Width * map.ScreenSize.Height];
			ColorAttributes = new int[(map.ScreenSize.Width / map.AttributeSize.Width) * (map.ScreenSize.Height / map.AttributeSize.Height)];
			MetaValues = new int[(map.ScreenSize.Width / map.MetaValueSize.Width) * (map.ScreenSize.Height / map.MetaValueSize.Height)];
			Image = new FastBitmap(map.ScreenSize.Width * map.BaseTileSize.Width, map.ScreenSize.Height * map.BaseTileSize.Height, PixelFormat.Format32bppPArgb);
			MetaImage = new FastBitmap(map.ScreenSize.Width / map.MetaValueSize.Width, map.ScreenSize.Height / map.MetaValueSize.Height, PixelFormat.Format32bppArgb);
			Objects = new MapObject[0];

			RefreshPreviousState();
		}

		public void Unload()
		{
			if (_cancelRefresh != null) _cancelRefresh();
			TileChanged = null;
			EditEnd = null;
			ImageUpdated = null;
		}


		public void PrintTile(int x, int y, int tileIndex)
		{
			var index = y * _map.ScreenSize.Width + x;
			if (index >= Tiles.Length || tileIndex < 0) return;
			var oldTile = Tiles[index];
			Tiles[index] = tileIndex;
			if (TileChanged != null) TileChanged(x, y, oldTile, tileIndex);
		}

		public bool ReplaceTiles(Dictionary<int, int> changes)
		{
			var changed = false;
			for (var i = 0; i < Tiles.Length; i++)
			{
				var oldTile = Tiles[i];
				if (!changes.ContainsKey(oldTile)) continue;

				changed = true;
				Tiles[i] = changes[oldTile];
				if (TileChanged != null) TileChanged(i % _map.ScreenSize.Width, i / _map.ScreenSize.Width, oldTile, Tiles[i]);
			}
			return changed;
		}


		public int GetTile(int x, int y)
		{
			var index = y * _map.ScreenSize.Width + x;
			return Tiles.Length > index ? Tiles[index] : -1;
		}

		public void SetColorAttribute(int x, int y, int paletteIndex)
		{
			var attributeIndex = y * (_map.ScreenSize.Width / _map.AttributeSize.Width) + x;
			if (attributeIndex >= ColorAttributes.Length) return;
			ColorAttributes[attributeIndex] = (ColorAttributes[attributeIndex] & 0xF8) | paletteIndex;
			
			if (TileChanged == null) return;
			for (var i = 0; i < _map.AttributeSize.Width; i++)
			for (var j = 0; j < _map.AttributeSize.Height; j++)
			{
				TileChanged(x * _map.AttributeSize.Width + i, y * _map.AttributeSize.Height + j, -1, -1);
			}
		}
		public void SetMetaValue(int x, int y, int value)
		{
			var index = y * (_map.ScreenSize.Width / _map.MetaValueSize.Width) + x;
			MetaValues[index] = value;
			RefreshMetaValueTile(x, y);
		}
		public int GetMetaValue(int x, int y)
		{
			var index = y * (_map.ScreenSize.Width / _map.MetaValueSize.Width) + x;
			return MetaValues[index];
		}

		public int GetColorAttribute(int x, int y)
		{
			var attributeIndex = y * (_map.ScreenSize.Width / _map.AttributeSize.Width) + x;
			//return attributeIndex >= ColorAttributes.Length ? 0xff : ColorAttributes[y * (_map.ScreenSize.Width / _map.AttributeSize.Width) + x] & 0x07;
			return attributeIndex >= ColorAttributes.Length ? 0 : ColorAttributes[y * (_map.ScreenSize.Width / _map.AttributeSize.Width) + x] & 0x07;
		}

		public void SetColorTile(int x, int y, int paletteIndex)
		{
			SetColorAttribute(x / _map.AttributeSize.Width, y / _map.AttributeSize.Height, paletteIndex);
		}
		public int GetColorTile(int x, int y)
		{
			return GetColorAttribute(x / _map.AttributeSize.Width, y / _map.AttributeSize.Height);
		}

		public readonly object TileDrawLock = new Object();
		public void RefreshTile(int x, int y, MapEditorState state, bool force = false, bool pushChange = false)
		{
			var index = y * _map.ScreenSize.Width + x;
			if (_updatedTiles == null || !force && _updatedTiles.ContainsKey(index)) return;

			var attributeIndex = (y / _map.AttributeSize.Height) * (_map.ScreenSize.Width / _map.AttributeSize.Width) + (x / _map.AttributeSize.Width);
			if (attributeIndex >= ColorAttributes.Length) return;
			var paletteIndex = ColorAttributes[attributeIndex];
			var tile = state.GetTileImage(Tiles[index], _map.Palettes[paletteIndex]);
			if (tile == null) return;

			lock (TileDrawLock) tile.CopyTile(Image, x, y);
			
			lock (_updatedTiles)
			if (!_updatedTiles.ContainsKey(index)) _updatedTiles.Add(index, true);
			if (pushChange) _pendingTileImageChange = true;
		}

		private Task _fullRefreshTask;
		private Dictionary<int, bool> _updatedTiles;
		private Action _cancelRefresh;
		private bool _pendingTileImageChange;

		public void RefreshAllTiles(MapEditorState state)
		{
			_updatedTiles = new Dictionary<int, bool>();

			if (_cancelRefresh != null)
			{
				_cancelRefresh();
				try
				{
					_fullRefreshTask.Wait();
				}
				catch (AggregateException ex)
				{
					if (ex.InnerException != null && !(ex.InnerException is TaskCanceledException)) throw ex.InnerException;
				}
			}
			var tokenSource = new CancellationTokenSource();
			_cancelRefresh = () => tokenSource.Cancel();
			var token = tokenSource.Token;
			_fullRefreshTask = Task.Run(() =>
			{
				//var timer = new Stopwatch();
				//timer.Start();
				for (var mx = 0; mx < _map.ScreenSize.Width / _map.MetaValueSize.Width; mx++)
				for (var my = 0; my < _map.ScreenSize.Height / _map.MetaValueSize.Height; my++)
				{
					RefreshMetaValueTile(mx, my);
				}

				for (var x = 0; x < _map.ScreenSize.Width; x++)
				for (var y = 0; y < _map.ScreenSize.Height; y++)
				{
					RefreshTile(x, y, state);
					if (token.IsCancellationRequested) return;
				}

				//timer.Stop();
				//Debug.WriteLine("Full redraw: " + timer.Elapsed.TotalMilliseconds);
				if (ImageUpdated != null) ImageUpdated();
			}, token);
		}
		public void EnsureRefreshedImage()
		{
			if (_fullRefreshTask != null && !_fullRefreshTask.IsCompleted) _fullRefreshTask.Wait();
		}


		private void RefreshMetaValueTile(int x, int y)
		{
			var value = MetaValues[y * (_map.ScreenSize.Width / _map.MetaValueSize.Width) + x];
			lock (MetaImage) MetaImage.SetPixel(x, y, _map.GetMetaValueColor(value));
		}

		public void OnEditEnd()
		{
			if (EditEnd != null) EditEnd();
			if (_pendingTileImageChange && ImageUpdated != null) ImageUpdated();
			_pendingTileImageChange = false;
		}

		public MetaTile GetMetaTile(int x, int y, int size)
		{
			var tiles = new int[size * size];
			var attributes = new int[(size / _map.AttributeSize.Width) * (size / _map.AttributeSize.Height)];
			for (var i = 0; i < tiles.Length; i++)
			{
				var iX = x * size + (i % size); // (xoffset) + (local X)
				var iY = y * size + (i / size); // (yoffset) + (local Y)
				//attributes[((i/size) / _map.AttributeSize.Height) * _map.AttributeSize.Width + ((i % size) / _map.AttributeSize.Width)] = GetColorTile(iX, iY);
				tiles[i] = GetTile(iX, iY);
			}

			for (var i = 0; i < attributes.Length; i++)
			{
				var iX = x * size + (i % (size / _map.AttributeSize.Width)) * _map.AttributeSize.Width;
				var iY = y * size + (i / (size / _map.AttributeSize.Width)) * _map.AttributeSize.Height;
				attributes[i] = GetColorTile(iX, iY);
			}

			return new MetaTile
			{
				Tiles = tiles,
				Attributes = attributes
			};
		}

		public void PrintMetaTile(int x, int y, MetaTile metaTile, int size)
		{
			for (var i = 0; i < metaTile.Tiles.Length; i++)
			{
				var iX = x * size + (i % size);
				var iY = y * size + (i / size);
				PrintTile(iX, iY, metaTile.Tiles[i]);
			}
			for (var i = 0; i < metaTile.Attributes.Length; i++)
			{
				var iX = x * size + (i % (size / _map.AttributeSize.Width)) * _map.AttributeSize.Width;
				var iY = y * size + (i / (size / _map.AttributeSize.Width)) * _map.AttributeSize.Height;
				SetColorTile(iX, iY, metaTile.Attributes[i]);
			}
		}

		public void RefreshPreviousState()
		{
			PreviousState = new ScreenUndoState
			{
				Attributes = new int[ColorAttributes.Length],
				Tiles = new int[Tiles.Length],
				MetaValues = new int[MetaValues.Length],
				Objects = new MapObject[Objects.Length]
			};
			//var timer = new Stopwatch();
			//timer.Start();
			Buffer.BlockCopy(ColorAttributes, 0, PreviousState.Attributes, 0, Buffer.ByteLength(ColorAttributes));
			Buffer.BlockCopy(Tiles, 0, PreviousState.Tiles, 0, Buffer.ByteLength(Tiles));
			Buffer.BlockCopy(MetaValues, 0, PreviousState.MetaValues, 0, Buffer.ByteLength(MetaValues));
			if (Objects.Length > 0) Buffer.BlockCopy(Objects, 0, PreviousState.Objects, 0, Buffer.ByteLength(Objects));
			//timer.Stop();
			//Debug.WriteLine("Copy state: " + timer.Elapsed.TotalMilliseconds);
		}

		public ScreenUndoState PreviousState { get; private set; }

		public void RevertToState(ScreenUndoState state)
		{
			if (ColorAttributes.Length != state.Attributes.Length || Tiles.Length != state.Tiles.Length ||
			    MetaValues.Length != state.MetaValues.Length) return;

			Buffer.BlockCopy(state.Attributes, 0, ColorAttributes, 0, Buffer.ByteLength(ColorAttributes));
			Buffer.BlockCopy(state.Tiles, 0, Tiles, 0, Buffer.ByteLength(Tiles));
			Buffer.BlockCopy(state.MetaValues, 0, MetaValues, 0, Buffer.ByteLength(MetaValues));

			if (Objects.Length != state.Objects.Length) Objects = new MapObject[state.Objects.Length];
			if (Objects.Length > 0) Buffer.BlockCopy(state.Objects, 0, Objects, 0, Buffer.ByteLength(Objects));

			PreviousState = state;
			OnEditEnd();
		}
	}

	[Serializable]
	public struct MapObject
	{
		public int X;
		public int Y;
		public int Id;
	}

	[Serializable]
	public class SerializableTileMap
	{
		public Size ScreenSize = new Size(32, 30);
		public Size AttributeSize = new Size(2, 2);
		public Size MetaValueSize = new Size(2, 2);
		public int BitsPerPixel;
		public int Width;
		public int Height;
		public string ChrSource;
		public SerializableScreen[] Screens;
		public List<List<Color>> Palettes;

		public TileMap GetMap()
		{
			var map = new TileMap
			{
				Width = Width,
				Height = Height,
				ScreenSize = ScreenSize,
				BitsPerPixel = BitsPerPixel,
				AttributeSize = AttributeSize,
				MetaValueSize = MetaValueSize,
				Palettes = Palettes != null ? Palettes.Select(c => new Palette { Colors = c }).ToList() : new List<Palette>()
			};
			for (var y = 0; y < Height; y++)
			{
				var row = new List<TileMapScreen>();
				map.Screens.Add(row);
				for (var x = 0; x < Width; x++)
				{
					if (Screens.Length <= y * Width + x) break;
					var screenSource = Screens[y * Width + x];
					if (screenSource == null)
					{
						row.Add(null);
						continue;
					}
					var screen = new TileMapScreen(map);
					if (screenSource.Tiles != null) screen.Tiles = screenSource.Tiles;
					if (screenSource.ColorAttributes != null) screen.ColorAttributes = screenSource.ColorAttributes;
					if (screenSource.MetaValues != null) screen.MetaValues = screenSource.MetaValues;
					if (screenSource.Objects != null) screen.Objects = screenSource.Objects;
					screen.RefreshPreviousState();
					row.Add(screen);
				}
			}
			
			return map;
		}
	}

	[Serializable]
	public class SerializableScreen
	{
		public int[] Tiles;
		public int[] ColorAttributes;
		public int[] MetaValues;
		public MapObject[] Objects;
	}

	[Serializable]
	[XmlRoot("tilemap")]
	public class PyxelMap
	{
		[XmlAttribute(AttributeName = "tileswide")]
		public int Width;
		[XmlAttribute(AttributeName = "tileshigh")]
		public int Height;

		[XmlElement(ElementName = "layer")]
		public List<Layer> Layers;

		public class Layer
		{
			[XmlAttribute(AttributeName = "number")]
			public int Number;
			[XmlAttribute(AttributeName = "name")]
			public string Name;

			[XmlElement(ElementName = "tile")]
			public List<Tile> Tiles;
		}

		public class Tile
		{
			[XmlAttribute(AttributeName = "x")]
			public int X;
			[XmlAttribute(AttributeName = "y")]
			public int Y;
			[XmlAttribute(AttributeName = "index")]
			public int Index;
		}
	}
}