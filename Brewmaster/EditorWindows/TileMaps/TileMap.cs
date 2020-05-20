using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TileMap
	{
		public Size BaseTileSize = new Size(8, 8);
		public Size AttributeSize = new Size(2, 2);
		public Size ScreenSize = new Size(32, 30);
		public int BitsPerPixel = 2;
		public int ColorCount { get { return (int)Math.Pow(2, BitsPerPixel); } }
		public List<List<TileMapScreen>> Screens = new List<List<TileMapScreen>>();

		public SerializableTileMap GetSerializable()
		{
			return new SerializableTileMap
			{
				Width = Screens[0].Count,
				Height = Screens.Count,
				ScreenSize = ScreenSize,
				BitsPerPixel = BitsPerPixel,
				Screens = Screens.SelectMany(l => l).Select(s => new SerializableScreen {Tiles = s.Tiles}).ToArray()
			};
		}
	}
	public class TileMapScreen
	{
		private readonly Size _screenSize;
		private readonly Size _tileSize;

		public TileMapScreen(Size screenSize, Size tileSize)
		{
			_screenSize = screenSize;
			_tileSize = tileSize;
			Tiles = new int[screenSize.Width * screenSize.Height];
			Image = new Bitmap(screenSize.Width * tileSize.Width, screenSize.Height * tileSize.Height);
		}

		public Bitmap Image;
		public int[] Tiles;
		public event Action<int, int> TileChanged;

		public void PrintTile(int x, int y, int index)
		{
			Tiles[y * _screenSize.Width + x] = index;
			if (TileChanged != null) TileChanged(x, y);
		}

		public int GetTile(int x, int y)
		{
			return Tiles[y * _screenSize.Width + x];
		}
	}

	[Serializable]
	public class SerializableTileMap
	{
		public Size ScreenSize;
		public int BitsPerPixel;
		public int Width;
		public int Height;
		public string ChrSource;
		public SerializableScreen[] Screens;

		public TileMap GetMap()
		{
			var map = new TileMap
			{
				ScreenSize = ScreenSize,
				BitsPerPixel = BitsPerPixel
			};
			for (var y = 0; y < Height; y++)
			{
				var row = new List<TileMapScreen>();
				map.Screens.Add(row);
				for (var x = 0; x < Width; x++)
				{
					var screen = Screens[y * Width + x];
					row.Add(new TileMapScreen(ScreenSize, map.BaseTileSize) { Tiles = screen.Tiles });
				}
			}
			return map;
		}
	}

	[Serializable]
	public class SerializableScreen
	{
		public int[] Tiles;
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