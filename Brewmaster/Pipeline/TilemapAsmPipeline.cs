using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brewmaster.EditorWindows.TileMaps;
using Brewmaster.EditorWindows.TileMaps.Tools;
using Brewmaster.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Brewmaster.Pipeline
{
	public class TileMapAsmPipeline : PipelineOption
	{
		public override IEnumerable<FileType> SupportedFileTypes { get { return new [] {FileType.TileMap};}}
		public override string TypeName { get { return "tilemap.asm"; } }
		public override void Process(PipelineSettings settings, Action<string> action)
		{
			var json = File.ReadAllText(settings.File.File.FullName);
			var deserializedMap = JsonConvert.DeserializeObject<SerializableTileMap>(json, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
			if (deserializedMap == null) return;
			
			var map = deserializedMap.GetMap();
			List<MetaTile> previousMetaTiles = null;
			var previousMetaTileSize = 0;
			var compressedMetaTiles = new Dictionary<int, List<MetaTile>>();
			foreach (var metaTileSize in map.MetaTileResolutions.OrderBy(s => s))
			{
				var includeMetaValues = metaTileSize >= 2;
				var includeAttributes = metaTileSize >= 4;
				var allMetaTiles = new List<MetaTile>();

				foreach (var screen in map.GetAllScreens())
				{
					var metaTiles = new List<MetaTile>();
					var metaTileRow = map.ScreenSize.Width / metaTileSize;
					var metaTileCol = map.ScreenSize.Height / metaTileSize;
					for (var y = 0; y < metaTileCol; y++)
					for (var x = 0; x < metaTileRow; x++)
					{
						var metaTile = screen.GetMetaTile(x, y, metaTileSize);
						if (metaTile.Attributes.Length == 1) metaTile.Attributes[0] = metaTile.Attributes[0] & 0xff00 | 0xff;
						metaTiles.Add(metaTile);
					}
					allMetaTiles.AddRange(metaTiles);
				}
				
				allMetaTiles = allMetaTiles.Distinct().ToList();
				if (previousMetaTiles == null) compressedMetaTiles[metaTileSize] = allMetaTiles;
				else
				{
					compressedMetaTiles[metaTileSize] = new List<MetaTile>();
					foreach (var rawMetaTile in allMetaTiles)
					{
						var subTileCount = metaTileSize / previousMetaTileSize;
						var subTiles = new List<int>();
						var subAttributes = new List<int>();
						for (var i = 0; i < (subTileCount * subTileCount); i++)
						{
							var baseIndex = (i / subTileCount) * (metaTileSize * previousMetaTileSize) + (i % subTileCount) * previousMetaTileSize;
							var tiles = new List<int>();
							for (var j = 0; j < (previousMetaTileSize * previousMetaTileSize); j++)
							{
								tiles.Add(rawMetaTile.Tiles[baseIndex + (j / previousMetaTileSize) * metaTileSize + (j % previousMetaTileSize)]);
							}
							// TODO: Handle dynamic attribute size, and option to include metadata/collisions
							var smallerMetaTile = new MetaTile
							{
								Tiles = tiles.ToArray(),
								Attributes = new [] { rawMetaTile.Attributes[i] }
							};
							subTiles.Add(previousMetaTiles.IndexOf(smallerMetaTile));
							subAttributes.Add(rawMetaTile.Attributes[i]);
						}
						compressedMetaTiles[metaTileSize].Add(new MetaTile
						{
							Tiles = subTiles.ToArray(),
							Attributes = subAttributes.ToArray()
						});
					}
				}

				previousMetaTileSize = metaTileSize;
				previousMetaTiles = allMetaTiles;
			}

			var output = new StringBuilder();
			var exports = new List<string>();

			foreach (var kvp in compressedMetaTiles)
			{
				var tileSetName = "metatiles" + kvp.Key;
				var tileCount = kvp.Value[0].Tiles.Length;
				exports.Add(tileSetName);
				var bytes = new List<string>[tileCount + (kvp.Value[0].Attributes.Length == 4 ? 1 : 0)];
				for (var i = 0; i < bytes.Length; i++) bytes[i] = new List<string>(kvp.Value.Count);

				output.AppendLine(".align 256");
				output.AppendLine(tileSetName + ":");
				foreach (var tile in kvp.Value)
				{
					for (var i = 0; i < tileCount; i++)
					{
						var index = tile.Tiles.Length > i ? tile.Tiles[i] : 0;
						if (index > 255) throw new Exception("Too many metatiles");
						bytes[i].Add(ByteString(index));
					}

					if (bytes.Length > tileCount)
					{
						bytes[tileCount].Add(ByteString(tile.Attributes[0] + tile.Attributes[1] * 4 + tile.Attributes[2] * 16 + tile.Attributes[3] * 64));
					}
				}
				for (var i = 0; i < bytes.Length; i++)
				{
					var description =
						i == 0 ? "0 (top left)" :
						i == 1 ? "1 (top right)" :
						i == 2 ? "2 (bottom left)" :
						i == 3 ? "3 (bottom right)" :
						i == 4 ? "4 (1 byte color attribute for all four tiles)" : i.ToString();

					output.AppendLine(".align 256\t;Sub-tiles index " + description);
					output.AppendLine(".byte " + string.Join(",", bytes[i]));
				}
			}
			output.AppendLine();

			var stageSymbol = Regex.Replace(Path.GetFileNameWithoutExtension(settings.File.File.Name), @"\s", "");
			exports.Add(stageSymbol);

			output.AppendLine(stageSymbol + ":");
			//TODO: Initialization data?
			output.AppendLine(".byte " + map.Width + ", " + map.Height + " ; Stage width, height");
			output.AppendLine(".word " + stageSymbol + "_tiles");
			output.AppendLine(".word " + stageSymbol + "_collisions");
			output.AppendLine(".word " + stageSymbol + "_objects");
			output.AppendLine();

			var groupCollisionsInColumns = false;
			var metaBitDepth = 2;
			var objectCoordBitDepth = 4; // 1-16

			var screenCount = map.GetAllScreens().Count();
			var columns = groupCollisionsInColumns ? map.Screens[0].Count : screenCount;
			var collisionLabels = new string[columns];
			var collisionColumns = new List<string>[columns];
			var objectLabels = new string[screenCount];
			var objects = new List<string>[screenCount];

			var objectCoordMask = (1 << objectCoordBitDepth) - 1;

			var column = 0;
			var screenNumber = 0;
			var screenAliases = new List<string>();
			foreach (var row in map.Screens)
			{
				if (groupCollisionsInColumns) column = 0;
				foreach (var screen in row)
				{
					if (screen == null)
					{
						column++;
						screenNumber++;
						continue;
					}

					if (collisionColumns[column] == null)
					{
						collisionLabels[column] = stageSymbol + "_col_" + column;
						collisionColumns[column] = new List<string>();
					}
					screenAliases.Add(stageSymbol + "_part" + screenNumber);
					var bitBuffer = new StringBuilder();
					for (var y = 0; y < (map.ScreenSize.Height / map.MetaValueSize.Height); y++)
					for (var x = 0; x < (map.ScreenSize.Width / map.MetaValueSize.Width); x++)
					{
						var metaValue = screen.GetMetaValue(x, y);
						if (metaValue < 0 || metaValue >= (1 << metaBitDepth))
							throw new Exception("Map meta data exceeds expected bit depth of " + metaBitDepth);
						bitBuffer.Append(Convert.ToString(metaValue, 2).PadLeft(metaBitDepth, '0'));
					}

					collisionColumns[column].AddRange(Regex.Replace(bitBuffer.ToString(), @"([01]{1,8})", @"%$1,").Trim(',').Split(','));

					objects[screenNumber] = new List<string>();
					objectLabels[screenNumber] = stageSymbol + "_obj_" + screenNumber;
					var sortedObjects = screen.Objects.ToArray().OrderBy(o => o.X);
					foreach (var mapObject in sortedObjects)
					{
						objects[screenNumber].Add(mapObject.Id.ToString());
						var xyPosition = mapObject.X & objectCoordMask | ((mapObject.Y & objectCoordMask) << objectCoordBitDepth);
						// TODO: If coordinate bitdepth > 4, split into multiple bytes
						objects[screenNumber].Add(ByteString(xyPosition));
					}
					objects[screenNumber].Add("$FF");

					column++;
					screenNumber++;
				}
			}
			output.AppendLine(stageSymbol + "_tiles:");
			output.AppendLine(".word " + string.Join(", ", screenAliases));
			output.AppendLine();

			output.AppendLine(stageSymbol + "_collisions:");
			output.AppendLine(".word " + string.Join(", ", collisionLabels));
			output.AppendLine();

			for (var j = 0; j < columns; j++)
			{
				output.AppendLine(collisionLabels[j] + ":");
				var bytes = collisionColumns[j];
				while (bytes.Any())
				{
					// Organizes metadata columns into readable rows
					var byteRow = bytes.Take(4).ToArray();
					bytes.RemoveRange(0, Math.Min(bytes.Count, 4));

					output.AppendLine(".byte " + string.Join(",", byteRow));
				}
			}

			output.AppendLine();

			output.AppendLine(stageSymbol + "_objects:");
			output.AppendLine(".word " + string.Join(", ", objectLabels));
			for (var j = 0; j < screenCount; j++)
			{
				output.AppendLine(objectLabels[j] + ":");
				output.AppendLine(".byte " + string.Join(",", objects[j]));
			}

			output.AppendLine();
			screenNumber = 0;
			var highestMetaTile = compressedMetaTiles.Last();
			var tilesX = map.ScreenSize.Width / highestMetaTile.Key;
			var tilesY = map.ScreenSize.Height / highestMetaTile.Key;
			foreach (var screen in map.GetAllScreens())
			{
				output.AppendLine(stageSymbol + "_part" + screenNumber + ":");
				
				var bytes = new List<string>();
				for (var y = 0; y < tilesY; y++)
				for (var x = 0; x < tilesX; x++)
				{
						var metaTileIndex = previousMetaTiles.IndexOf(screen.GetMetaTile(x, y, highestMetaTile.Key));
					if (metaTileIndex < 0) throw new Exception(string.Format("Unidentified metatile at {0},{1}", x, y));
					if (metaTileIndex > 255) throw new Exception("Metatile count exceeded 255, requires word size storage");

					bytes.Add(ByteString(metaTileIndex));
				}

				output.AppendLine(".byte " + string.Join(",", bytes));

				screenNumber++;
			}

			var finalOutput = new StringBuilder();
			finalOutput.AppendLine(".export " + string.Join(", ", exports));
			finalOutput.Append(output);

			File.WriteAllText(settings.GetFilePath(0), finalOutput.ToString());
		}

		public static string ByteString(int number)
		{
			return "$" + Convert.ToString(number, 16).ToUpperInvariant().PadLeft(2, '0');
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".inc");
		}
	}
}