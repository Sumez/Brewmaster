using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public enum ChrOutputType
	{
		Bpp2 = 0,
		Bpp3 = 1,
		Bpp4 = 2,
		Bpp8 = 3
	}

	public class ChrPipeline : PipelineOption
	{
		public override IEnumerable<FileType> SupportedFileTypes { get { return new[] { FileType.Image }; } }

		public override string TypeName { get { return "chr"; } }

		public static List<List<Color>> GetUniqueTilePalettes(Bitmap image)
		{
			var returnValue = new List<List<Color>>();
			var tileCount = GetTileCount(image);
			for (var i = 0; i < tileCount; i++)
			{
				var tilePalette = new List<Color>();
				var offset = GetOffset(image, i);
				for (var y = 0; y < 8; y++)
					for (var x = 0; x < 8; x++)
					{
						var color = GetPixel(image, offset.Width + x, offset.Height + y) ?? tilePalette[0];
						if (!tilePalette.Contains(color)) tilePalette.Add(color); ;
					}

				//var orderedPalette = tilePalette.OrderBy(c => c.A).ThenBy(c => c.R).ThenBy(c => c.G).ThenBy(c => c.B);
				for (var j = 0; j < returnValue.Count; j++)
				{
					if (returnValue[j].All(c => tilePalette.Contains(c)))
					{
						returnValue[j] = tilePalette;
						tilePalette = null;
						break;
					}
				}
				if (tilePalette != null)
					//				if (tilePalette.Count > 1 || tilePalette[0].A > 0) // Ignore palettes with only transparency
					returnValue.Add(tilePalette);

				// Remove redundant entries
				for (var j = 0; j < returnValue.Count; j++)
				{
					if (returnValue.Any(p => p.Count > returnValue[j].Count && returnValue[j].All(c => p.Contains(c))))
					{
						returnValue.RemoveAt(j);
						j--;
					}
				}
			}

			return returnValue;
		}

		public static Bitmap GetReducedImage(ChrPipelineSettings settings, Bitmap image)
		{
			var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppPArgb);
			var tileCount = GetTileCount(image);

			var basePalette = settings.TilePalettes.OrderByDescending(p => p.Count).First().ToDictionary(k => k.Value, k => k.Key);

			for (var i = 0; i < tileCount; i++)
			{
				// TODO: Reduce redundancy
				var tilePalette = new List<Color>();
				var offset = GetOffset(image, i);
				for (var y = 0; y < 8; y++)
					for (var x = 0; x < 8; x++)
					{
						var color = GetPixel(image, offset.Width + x, offset.Height + y) ?? tilePalette[0];
						if (!tilePalette.Contains(color)) tilePalette.Add(color);
					}

				var paletteReference = settings.TilePalettes.FirstOrDefault(p => tilePalette.All(p.ContainsKey));
				if (paletteReference == null) paletteReference = tilePalette.Select((color, index) => new { color, index }).ToDictionary(x => x.color, x => x.index); // If no matching palettes are found, new colors has been added since we generated our reduced palette. Just ignore these

				for (var y = 0; y < 8; y++)
					for (var x = 0; x < 8; x++)
					{
						if (bitmap.Width <= offset.Width + x || bitmap.Height <= offset.Height + y) continue;
						var color = GetPixel(image, offset.Width + x, offset.Height + y) ?? paletteReference.First().Key;
						bitmap.SetPixel(offset.Width + x, offset.Height + y, basePalette[paletteReference[color]]);
					}

			}

			return bitmap;
		}

		private static int GetTileCount(Image image)
		{
			return ((image.Width + 7) / 8) * ((image.Height + 7) / 8); // +7 results in the ceiling number after an uneven division (84+7 / 8 = 11 when all numbers are integer)
		}
		private static Size GetOffset(Bitmap image, int tileIndex)
		{
			var width = ((image.Width + 7) / 8);
			return new Size((tileIndex % width) * 8, (tileIndex / width) * 8);
		}

		public static Bitmap LoadImageFile(string filename)
		{
			using (var fileSource = new Bitmap(filename))
			{
				var frameDimension = new FrameDimension(fileSource.FrameDimensionsList[0]);
				var frames = fileSource.GetFrameCount(frameDimension);
				if (frames == 1) return (Bitmap)fileSource.Clone();

				var imageSource = new Bitmap(fileSource.Width * frames, fileSource.Height, fileSource.PixelFormat);
				using (var graphics = Graphics.FromImage(imageSource))
					for (var i = 0; i < frames; i++)
					{
						fileSource.SelectActiveFrame(frameDimension, i);
						graphics.DrawImageUnscaled(fileSource, i * fileSource.Width, 0);
					}

				return imageSource;
			}
		}

		private static Bitmap GetImageSource(ChrPipelineSettings settings)
		{
			if (!settings.ReducePalette || settings.TilePalettes == null) return LoadImageFile(settings.File.File.FullName);
			using (var image = LoadImageFile(settings.File.File.FullName)) return GetReducedImage(settings, image);
		}

		public override void Process(PipelineSettings settings)
		{
			var chrSettings = settings as ChrPipelineSettings;
			if (chrSettings == null) throw new Exception("Invalid settings for CHR pipeline");

			var platform = chrSettings.File.Project.Platform;
			var bitDepth = chrSettings.BitDepth;
			var discardRedundantTiles = chrSettings.DiscardRedundantTiles && !chrSettings.BigTiles;

			var tileSize = 8 * bitDepth;
			var paletteEntries = (int)Math.Pow(2, bitDepth);

			var vRamRowSize = 16 * tileSize;

			using (var image = GetImageSource(chrSettings))
			{
				var tileCount = GetTileCount(image);
				var bigTileRowLength = (image.Width + 16 - 1) / 16;

				var bytes = new byte[tileCount * tileSize];
				var exportedTiles = new List<byte[]>();

				var palette = new Color[paletteEntries];
				var knownEntries = 0;

				foreach (var assignment in chrSettings.PaletteAssignment)
				{
					if (assignment.Value >= paletteEntries) continue;

					palette[assignment.Value] = assignment.Key;
					knownEntries = Math.Max(knownEntries, assignment.Value + 1);
				}
				if (image.Palette.Entries.Length > knownEntries)
				{
					var j = 0;
					for (var i = knownEntries; i < paletteEntries; i++)
					{
						palette[i] = image.Palette.Entries[j];
						knownEntries++;
						j++;
					}
				}
				for (var y = 0; y < image.Height && knownEntries < paletteEntries; y++)
				{
					for (var x = 0; x < image.Width && knownEntries < paletteEntries; x++)
					{
						var color = image.GetPixel(x, y);
						if (!palette.Contains(color))
						{
							palette[knownEntries] = color;
							knownEntries++;
						}
					}
				}
				if (knownEntries > paletteEntries) throw new Exception("Too many colors in image");

				var tileByteOffset = 0;
				for (var i = 0; i < tileCount; i++)
				{
					var tileData = new byte[tileSize];
					var offset = GetOffset(image, i);

					for (var y = 0; y < 8; y++)
						for (var x = 0; x < 8; x++)
						{
							var color = GetPixel(image, offset.Width + x, offset.Height + y) ?? palette[0];
							var colorIndex = -1;

							for (var c = 0; c < knownEntries; c++)
							{
								if (color == palette[c])
								{
									colorIndex = c;
									break;
								}
							}

							if (colorIndex == -1)
							{
								if (knownEntries >= paletteEntries) throw new Exception("Too many colors in image");
								palette[knownEntries] = color;
								colorIndex = knownEntries;
								knownEntries++;
							}
							for (var j = 0; j <= (bitDepth / 2); j += 2)
							{
								var byte0 = (colorIndex & (1 << j)) == 0 ? 0 : 1;
								var byte1 = (colorIndex & (1 << (j + 1))) == 0 ? 0 : 1;

								var mask0 = (byte)(byte0 << (7 - x));
								var mask1 = (byte)(byte1 << (7 - x));

								var byte0Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 : y);
								var byte1Index = (8 * j) + (platform == TargetPlatform.Snes ? y * 2 + 1 : y + 8);

								tileData[byte0Index] |= mask0;
								tileData[byte1Index] |= mask1;
							}
						}

					if (discardRedundantTiles && exportedTiles.Any(t => t.SequenceEqual(tileData))) continue;
					exportedTiles.Add(tileData);

					if (chrSettings.BigTiles)
					{
						var tileX = offset.Width / 8;
						var tileY = offset.Height / 8;
						var bigTileIndex = offset.Width / 16 + (offset.Height / 16) * bigTileRowLength;
						var xOffset = (bigTileIndex * tileSize * 2) % vRamRowSize;
						var yOffset = vRamRowSize * (bigTileIndex / 8) * 2;

						tileByteOffset = xOffset + yOffset;
						if (tileX % 2 == 1) tileByteOffset += tileSize;
						if (tileY % 2 == 1) tileByteOffset += vRamRowSize;
					}

					tileData.CopyTo(bytes, tileByteOffset);
					tileByteOffset += tileSize;
				}

				using (var outputFile = File.Create(chrSettings.ChrOutputFullPath))
				{
					outputFile.Write(bytes, 0, discardRedundantTiles ? tileByteOffset : bytes.Length);
					outputFile.Close();
				}

				if (chrSettings.ExportPalette)
				{
					var paletteData = new byte[knownEntries * 2];
					for (var i = 0; i < knownEntries; i++)
					{
						var color = GetColorValue(palette[i]);
						paletteData[i * 2] = (byte)(color & 0xFF);
						paletteData[i * 2 + 1] = (byte)((color >> 8) & 0xFF);
					}
					using (var outputFile = File.Create(chrSettings.PaletteOutputFullPath))
					{
						outputFile.Write(paletteData, 0, paletteData.Length);
						outputFile.Close();
					}

				}
			}
		}

		private static Color? GetPixel(Bitmap image, int x, int y)
		{
			return (x >= image.Width || y >= image.Height) ? null : image.GetPixel(x, y) as Color?;
		}

		public static int GetColorValue(Color color)
		{
			var red = color.R / 8;
			var green = color.G / 8;
			var blue = color.B / 8;

			return red | (green << 5) | (blue << 10);
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			var baseFile = file.GetRelativeDirectory(true) + Path.GetFileNameWithoutExtension(file.File.Name);
			return new ChrPipelineSettings(this, file, baseFile + ".chr", baseFile + ".pal");
		}

		public override PipelineSettings Load(AsmProject project, PipelineHeader pipelineHeader)
		{
			var chrOutput = pipelineHeader.Output[0];
			var paletteOutput = pipelineHeader.Output.Length < 2 || string.IsNullOrWhiteSpace(pipelineHeader.Output[1]) ? null : pipelineHeader.Output[1];

			var pipeline = new ChrPipelineSettings(this, null, chrOutput, paletteOutput, pipelineHeader.LastProcessed);
			SetSettings(pipeline, pipelineHeader.Settings);
			return pipeline;
		}

		public override void GetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{
			base.GetSettings(settings, headerSettings);
			var pipeline = settings as ChrPipelineSettings;

			headerSettings["DiscardRedundant"] = pipeline.DiscardRedundantTiles ? "1" : "0";
			headerSettings["ReducePalette"] = pipeline.ReducePalette ? "1" : "0";
			headerSettings["ExportPalette"] = pipeline.ExportPalette ? "1" : "0";
			headerSettings["BigTiles"] = pipeline.BigTiles ? "1" : "0";
			headerSettings["ChrType"] = pipeline.PaletteType.ToString();
			headerSettings["Palette"] = SerializePalette(pipeline.PaletteAssignment);
			if (pipeline.TilePalettes != null) headerSettings["TilePalettes"] = string.Join(":", pipeline.TilePalettes.Select(SerializePalette));
		}

		private static string SerializePalette(Dictionary<Color, int> palette)
		{
			return string.Join(";", palette.Select(a => string.Join(",", a.Value.ToString(), a.Key.R.ToString(), a.Key.G.ToString(), a.Key.B.ToString(), a.Key.A.ToString())));
		}
		private static Dictionary<Color, int> DeserializePalette(string source)
		{
			return source.Split(';').Select(p => p.Split(',').Select(int.Parse).ToArray())
				.ToDictionary(
					p => Color.FromArgb(p[4], p[1], p[2], p[3]),
					p => p[0]
				);
		}

		public override void SetSettings(PipelineSettings settings, ProjectModel.Properties headerSettings)
		{
			base.GetSettings(settings, headerSettings);
			var pipeline = settings as ChrPipelineSettings;

			pipeline.DiscardRedundantTiles = (headerSettings["DiscardRedundant"] == "1");
			pipeline.ReducePalette = (headerSettings["ReducePalette"] == "1");
			pipeline.ExportPalette = (headerSettings["ExportPalette"] == "1");
			pipeline.BigTiles = (headerSettings["BigTiles"] == "1");
			Enum.TryParse(headerSettings["ChrType"], true, out pipeline.PaletteType);
			if (!string.IsNullOrWhiteSpace(headerSettings["Palette"]))
			{
				pipeline.PaletteAssignment = DeserializePalette(headerSettings["Palette"]);
			}

			if (!string.IsNullOrWhiteSpace(headerSettings["TilePalettes"]))
			{
				pipeline.TilePalettes = headerSettings["TilePalettes"].Split(':').Select(DeserializePalette).ToList();
			}
		}

	}


	public class ChrPipelineSettings : PipelineSettings
	{
		public ChrOutputType PaletteType = ChrOutputType.Bpp2;
		public bool ReducePalette = false;
		public bool ExportPalette = false;
		public bool BigTiles = false;
		public Dictionary<Color, int> PaletteAssignment;
		public List<Dictionary<Color, int>> TilePalettes { get; set; }
		public bool DiscardRedundantTiles = false;

		public override List<string> OutputFiles
		{
			get
			{
				return new List<string> { ChrOutput, PaletteOutput };
			}
		}

		public string ChrOutput;
		public string PaletteOutput;

		public ChrPipelineSettings(PipelineOption type, AsmProjectFile file, string chrOutput, string paletteOutput, DateTime? lastProcessed = null) : base(type, file, lastProcessed)
		{
			ChrOutput = chrOutput;
			PaletteOutput = paletteOutput;
			PaletteAssignment = new Dictionary<Color, int>();
		}

		public int BitDepth
		{
			get
			{
				switch (PaletteType)
				{
					case ChrOutputType.Bpp3:
						return 3;
					case ChrOutputType.Bpp4:
						return 4;
					case ChrOutputType.Bpp8:
						return 8;
					case ChrOutputType.Bpp2:
					default:
						return 2;
				}
			}
		}

		public string ChrOutputFullPath { get { return Path.Combine(File.Project.Directory.FullName, ChrOutput); } }
		public string PaletteOutputFullPath { get { return Path.Combine(File.Project.Directory.FullName, PaletteOutput); } }
	}
}