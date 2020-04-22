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
	public class ChrPipeline : DataPipeline
	{
		public ChrOutputType Type = ChrOutputType.Bpp2;
		// TODO: Implement (and save) the settings below
		public bool DiscardRedundantTiles = false;
		public bool ReducePalette = false;
		public Dictionary<Color, int> PaletteAssignment;
		public List<Dictionary<Color, int>> TilePalettes { get; set; }

		public override IEnumerable<string> OutputFiles
		{
			get
			{
				return new[] { ChrOutput, PaletteOutput };
			}
		}

		public string ChrOutput;
		public string PaletteOutput;

		public ChrPipeline(AsmProjectFile file, string chrOutput, DateTime? lastProcessed = null) : base(file, lastProcessed)
		{
			ChrOutput = chrOutput;
			PaletteAssignment = new Dictionary<Color, int>();
		}

		public int BitDepth
		{
			get
			{
				switch (Type)
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

		public List<List<Color>> GetUniqueTilePalettes(Bitmap image)
		{
			var returnValue = new List<List<Color>>();
			var tileCount = (image.Width / 8) * (image.Height / 8);
			for (var i = 0; i < tileCount; i++)
			{
				var tilePalette = new List<Color>();
				var offset = i * 8;
				var offsetY = (offset / image.Width) * 8;
				var offsetX = (offset % image.Width);
				for (var y = 0; y < 8; y++)
				for (var x = 0; x < 8; x++)
				{
					var color = image.GetPixel(offsetX + x, offsetY + y);
					if (!tilePalette.Contains(color)) tilePalette.Add(color);;
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
		public Bitmap GetReducedImage(Bitmap image)
		{
			var bitmap = new Bitmap(image.Width, image.Height);
			var tileCount = (image.Width / 8) * (image.Height / 8);

			var basePalette = TilePalettes.OrderByDescending(p => p.Count >= 4).First().ToDictionary(k => k.Value, k => k.Key);

			for (var i = 0; i < tileCount; i++)
			{
				// TODO: Reduce redundancy
				var tilePalette = new List<Color>();
				var offset = i * 8;
				var offsetY = (offset / image.Width) * 8;
				var offsetX = (offset % image.Width);
				for (var y = 0; y < 8; y++)
				for (var x = 0; x < 8; x++)
				{
					var color = image.GetPixel(offsetX + x, offsetY + y);
					if (!tilePalette.Contains(color)) tilePalette.Add(color);
				}

				var paletteReference = TilePalettes.FirstOrDefault(p => tilePalette.All(p.ContainsKey));
				if (paletteReference == null) paletteReference = tilePalette.Select((color, index) => new { color, index }).ToDictionary(x => x.color, x => x.index); // If no matching palettes are found, new colors has been added since we generated our reduced palette. Just ignore these

				for (var y = 0; y < 8; y++)
				for (var x = 0; x < 8; x++)
				{
					var color = image.GetPixel(offsetX + x, offsetY + y);
					bitmap.SetPixel(offsetX + x, offsetY + y, basePalette[paletteReference[color]]);
				}

			}

			return bitmap;
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

		private Bitmap GetImageSource()
		{
			if (!ReducePalette || TilePalettes == null) return LoadImageFile(File.File.FullName);
			
			using (var image = LoadImageFile(File.File.FullName)) return GetReducedImage(image);
		}

		public override void Process()
		{
			var projectType = File.Project.Type;
			var bitDepth = BitDepth;

			var tileSize = 8 * bitDepth;
			var paletteEntries = (int)Math.Pow(2, bitDepth);

			using (var image = GetImageSource())
			{
				var tileCount = (image.Width / 8) * (image.Height / 8);
				var bytes = new byte[tileCount * tileSize];
				var exportedTiles = new List<byte[]>();

				var palette = new Color[paletteEntries];
				var knownEntries = 0;

				foreach (var assignment in PaletteAssignment)
				{
					if (assignment.Value >= paletteEntries) continue;

					palette[assignment.Value] = assignment.Key;
					knownEntries = Math.Max(knownEntries, assignment.Value + 1);
				}
				if (image.Palette.Entries.Length >= paletteEntries)
				{
					var j = 0;
					for (var i = knownEntries; i < paletteEntries; i++)
					{
						palette[i] = image.Palette.Entries[j];
						knownEntries++;
						j++;
					}
				}
				if (knownEntries > paletteEntries) throw new Exception("Too many colors in image");

				var tileByteOffset = 0;
				for (var i = 0; i < tileCount; i++)
				{
					var tileData = new byte[tileSize];
					var offset = i * 8;
					var offsetY = (offset / image.Width) * 8;
					var offsetX = (offset % image.Width);

					for (var y = 0; y < 8; y++)
					for (var x = 0; x < 8; x++)
					{
						var color = image.GetPixel(offsetX + x, offsetY + y);
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

							var byte0Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 : y);
							var byte1Index = (8 * j) + (projectType == ProjectType.Snes ? y * 2 + 1 : y + 8);

							tileData[byte0Index] |= mask0;
							tileData[byte1Index] |= mask1;
						}
					}

					if (DiscardRedundantTiles && exportedTiles.Any(t => t.SequenceEqual(tileData))) continue;
					exportedTiles.Add(tileData);
					tileData.CopyTo(bytes, tileByteOffset);
					tileByteOffset += tileSize;
				}

				using (var outputFile = System.IO.File.Create(ChrOutput))
				{
					outputFile.Write(bytes, 0, tileByteOffset);
					outputFile.Close();
				}
			}
			LastProcessed = DateTime.Now;
		}

		public override void GetSettings(ProjectModel.Properties headerSettings)
		{
			base.GetSettings(headerSettings);

			headerSettings["DiscardRedundant"] = DiscardRedundantTiles ? "1" : "0";
			headerSettings["ReducePalette"] = ReducePalette ? "1" : "0";
			headerSettings["ChrType"] = Type.ToString();
			headerSettings["Palette"] = SerializePalette(PaletteAssignment);
			if (TilePalettes != null) headerSettings["TilePalettes"] = string.Join(":", TilePalettes.Select(SerializePalette));
		}

		public override DataPipeline Clone(bool toEditor = false)
		{
			Func<string, string> convertFilePath = (s) => Path.Combine(File.Project.Directory.FullName, s);
			if (toEditor) convertFilePath = (s) => File.Project.GetRelativePath(s);

			var pipeline = new ChrPipeline(File, convertFilePath(ChrOutput));
			pipeline.PaletteOutput = String.IsNullOrWhiteSpace(PaletteOutput) ? null : convertFilePath(PaletteOutput);
			var settings = new ProjectModel.Properties();
			GetSettings(settings);
			pipeline.SetSettings(settings);
			return pipeline;
		}

		private static string SerializePalette(Dictionary<Color, int> palette)
		{
			return string.Join(";", palette
				.Select(a => string.Join(",", a.Value.ToString(), a.Key.R.ToString(), a.Key.G.ToString(), a.Key.B.ToString(), a.Key.A.ToString())));
		}
		private static Dictionary<Color, int> DeserializePalette(string source)
		{
			return source.Split(';').Select(p => p.Split(',').Select(int.Parse).ToArray())
				.ToDictionary(
					p => Color.FromArgb(p[4], p[1], p[2], p[3]),
					p => p[0]
				);
		}

		public void SetSettings(ProjectModel.Properties headerSettings)
		{
			DiscardRedundantTiles = (headerSettings["DiscardRedundant"] == "1");
			ReducePalette = (headerSettings["ReducePalette"] == "1");
			Enum.TryParse(headerSettings["ChrType"], true, out Type);
			if (!string.IsNullOrWhiteSpace(headerSettings["Palette"]))
			{
				PaletteAssignment = DeserializePalette(headerSettings["Palette"]);
			}

			if (!string.IsNullOrWhiteSpace(headerSettings["TilePalettes"]))
			{
				TilePalettes = headerSettings["TilePalettes"].Split(':').Select(DeserializePalette).ToList();
			}
		}


	}
}