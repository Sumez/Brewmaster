using System;
using System.Collections.Generic;
using System.IO;
using Brewmaster.EditorWindows.TileMaps;
using Brewmaster.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Brewmaster.Pipeline
{
	public class TileMapBinaryPipeline : PipelineOption
	{
		public override IEnumerable<FileType> SupportedFileTypes { get { return new[] { FileType.TileMap }; } }
		public override string TypeName { get { return "tilemap.binary"; } }
		public override void Process(PipelineSettings settings)
		{
			var json = File.ReadAllText(settings.File.File.FullName);
			var deserializedMap = JsonConvert.DeserializeObject<SerializableTileMap>(json, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
			if (deserializedMap == null) return;

			var map = deserializedMap.GetMap();


			var rle = settings.GenericSettings.GetBoolean("rle");
			var includeAttributes = settings.GenericSettings.GetBoolean("include-attributes");

			var output = new List<byte>();
			foreach (var screen in map.GetAllScreens())
			{
				var bytes = new List<byte>(includeAttributes ? 1024 : 960);
				foreach (var tile in screen.Tiles)
				{
					if (tile > 255) throw new Exception("Cannot export binary nametable when tile count exceeds 256");
					bytes.Add((byte)tile);
				}
				if (includeAttributes)
					for (var y = 0; y < map.ScreenSize.Height; y += 4)
					{
						for (var x = 0; x < map.ScreenSize.Width; x += 4)
						{
							var topLeft = screen.GetColorAttribute(x / 2, y / 2) & 0b11;
							var topRight = screen.GetColorAttribute(x / 2 + 1, y / 2) & 0b11;
							var bottomLeft = screen.GetColorAttribute(x / 2, y / 2 + 1) & 0b11;
							var bottomRight = screen.GetColorAttribute(x / 2 + 1, y / 2 + 1) & 0b11;

							bytes.Add((byte)(topLeft | (topRight << 2) | (bottomLeft << 4) | (bottomRight << 6)));
						}
					}

				if (rle)
				{
					var rleBytes = new List<byte>();
					var repeats = 0;
					var previousValue = -1;
					byte tag = 0;
					for (byte i = 1; i <= 255; i++)
					{
						if (bytes.Contains(i)) continue;
						tag = i;
						break;
					}
					if (tag == 0) throw new Exception("Cannot identify an unused tile/attribute byte to use as tag for RLE compression. Map is not valid due to the format's limitations");

					rleBytes.Add(tag);
					void AddRepetition()
					{
						if (repeats == 0) return;
						if (repeats == 1)
						{
							rleBytes.Add((byte)previousValue);
							repeats = 0;
							return;
						}
						while (repeats >= 256)
						{
							repeats -= 256;
							rleBytes.Add(tag);
							rleBytes.Add(255);
							rleBytes.Add((byte)previousValue);
						}

						rleBytes.Add(tag);
						rleBytes.Add((byte)repeats);
						repeats = 0;
					}

					foreach (var value in bytes)
					{
						if (value == previousValue || value == tag) repeats++;
						else
						{
							AddRepetition();
							rleBytes.Add(value);
						}
						previousValue = value;
					}
					AddRepetition();
					rleBytes.Add(tag);
					rleBytes.Add(0);

					bytes = rleBytes;
				}

				output.AddRange(bytes);
			}

			File.WriteAllBytes(settings.GetFilePath(0), output.ToArray());
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".nam");
		}

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("include-attributes", PipelinePropertyType.Boolean, true),
					new PipelineProperty("rle", PipelinePropertyType.Boolean, false)
				};
			}
		}
	}
}