using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using Brewmaster.Modules.Ppu;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class TileMap
	{
		public List<Palette> Palettes = new List<Palette>();
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
				AttributeSize = AttributeSize,
				BitsPerPixel = BitsPerPixel,
				Screens = Screens.SelectMany(l => l).Select(s => new SerializableScreen {Tiles = s.Tiles, ColorAttributes = s.ColorAttributes}).ToArray(),
				Palettes = Palettes.Select(p => p.Colors).ToList()
			};
		}
	}
	public class TileMapScreen
	{
		private readonly Size _screenSize;
		private readonly Size _tileSize;
		private readonly Size _attributeSize;

		public TileMapScreen(Size screenSize, Size tileSize, Size attributeSize)
		{
			_screenSize = screenSize;
			_tileSize = tileSize;
			_attributeSize = attributeSize;
			Tiles = new int[screenSize.Width * screenSize.Height];
			ColorAttributes = new int[(screenSize.Width / attributeSize.Width) * (screenSize.Height / attributeSize.Height)];
			Image = new Bitmap(screenSize.Width * tileSize.Width, screenSize.Height * tileSize.Height);
		}

		public Bitmap Image;
		public int[] Tiles;
		public int[] ColorAttributes;
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

		public void SetColorAttribute(int x, int y, int paletteIndex)
		{
			ColorAttributes[y * (_screenSize.Width / _attributeSize.Width) + x] = paletteIndex;
			
			if (TileChanged == null) return;
			for (var i = 0; i < _attributeSize.Width; i++)
			for (var j = 0; j < _attributeSize.Height; j++)
			{
				TileChanged(x * _attributeSize.Width + i, y * _attributeSize.Height + j);
			}
		}

		public int GetColorAttribute(int x, int y)
		{
			return ColorAttributes[y * (_screenSize.Width / _attributeSize.Width) + x];
		}

		public void SetColorTile(int x, int y, int paletteIndex)
		{
			SetColorAttribute(x / _attributeSize.Width, y / _attributeSize.Height, paletteIndex);
		}
		public int GetColorTile(int x, int y)
		{
			return GetColorAttribute(x / _attributeSize.Width, y / _attributeSize.Height);
		}
	}

	[Serializable]
	public class SerializableTileMap
	{
		public Size ScreenSize = new Size(32, 30);
		public Size AttributeSize = new Size(2, 2);
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
				ScreenSize = ScreenSize,
				BitsPerPixel = BitsPerPixel,
				AttributeSize = AttributeSize,
				Palettes = Palettes != null ? Palettes.Select(c => new Palette { Colors = c }).ToList() : new List<Palette>()
			};
			for (var y = 0; y < Height; y++)
			{
				var row = new List<TileMapScreen>();
				map.Screens.Add(row);
				for (var x = 0; x < Width; x++)
				{
					var screenSource = Screens[y * Width + x];
					var screen = new TileMapScreen(ScreenSize, map.BaseTileSize, map.AttributeSize);
					if (screenSource.Tiles != null) screen.Tiles = screenSource.Tiles;
					if (screenSource.ColorAttributes != null) screen.ColorAttributes = screenSource.ColorAttributes;
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