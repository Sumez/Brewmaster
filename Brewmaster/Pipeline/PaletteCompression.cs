using Brewmaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Brewmaster.Pipeline
{
	public class PaletteCompression : PipelineOption
	{
		public override string TypeName
		{
			get { return "image.palette-compression"; }
		}
		private static Dictionary<Color, float[]> LabIndex = new Dictionary<Color, float[]>();
		public override IEnumerable<FileType> SupportedFileTypes { get { return new[] { FileType.Image }; } }
		public override void Process(PipelineSettings settings, Action<string> output)
		{
			if (!int.TryParse(settings.GenericSettings["bitdepth"], out int bitDepth)) return;
			if (!int.TryParse(settings.GenericSettings["palette-count"], out int maxPalettes)) return;

			var tileWidth = 16;
			var tileHeight = 16;
			var colorCount = (int)Math.Pow(2, bitDepth);

			using (var image = new Bitmap(settings.File.File.FullName))
			{
				output("Reducing color count for individual tiles...");
				var timer = new Stopwatch();
				timer.Start();

				var i = 0;
				var paletteInstances = new List<PaletteInstance>();
				for (var y = 0; y < image.Height; y+= tileHeight)
				{
					for (var x = 0; x < image.Width; x += tileWidth)
					{
						var tilePalette = new List<Color>();
						for (var tY = 0; tY < tileHeight && tY+y < image.Height; tY++)
							for (var tX = 0; tX < tileWidth && tX+x < image.Width; tX++)
							{
								var color = image.GetPixel(x + tX, y + tY);
								if (!tilePalette.Contains(color)) tilePalette.Add(color);
							}
						tilePalette = GetOptimizedPalette(tilePalette, colorCount);

						var matchingPalette = paletteInstances.FirstOrDefault(p => p.Matches(tilePalette));
						if (matchingPalette != null)
						{
							matchingPalette.Tiles.Add(i);
							matchingPalette.Palette = tilePalette.Count > matchingPalette.Palette.Count ? tilePalette : matchingPalette.Palette;
						}
						else
						{
							paletteInstances.Add(new PaletteInstance(tilePalette, i));
						}

						i++;
					}
				}

				timer.Stop();
				output("Reducing palette count of image...");
				timer.Reset();
				timer.Start();

				foreach (var palette in paletteInstances)
				{
					FindNeighbour(palette, paletteInstances);
				}
				var resultPalettes = paletteInstances.ToList();
				while (resultPalettes.Count > maxPalettes)
				{
					var candidates = resultPalettes.OrderBy(p => p.NeighborDistance).Take(resultPalettes.Count - maxPalettes).ToList();
					var leastPopular = candidates.OrderBy(p => p.Tiles.Count).First().Tiles.Count;
					var removePalette = candidates.First(p => p.Tiles.Count == leastPopular);

					resultPalettes.Remove(removePalette);
					removePalette.NearestNeighbor.Tiles.AddRange(removePalette.Tiles);
					foreach (var palette in resultPalettes.Where(p => p.NearestNeighbor == removePalette))
					{
						FindNeighbour(palette, resultPalettes);
					}
				}

				timer.Stop();
				output("Calculated " + resultPalettes.Count + " unique palettes");
				output("Generating reduced image");
				timer.Reset();
				timer.Start();

				using (var destImage = new Bitmap(image.Width, image.Height))
				{
					i = 0;
					for (var y = 0; y < image.Height; y += tileHeight)
					{
						for (var x = 0; x < image.Width; x += tileWidth)
						{
							var palette = resultPalettes.First(p => p.Tiles.Contains(i));
							for (var tY = 0; tY < tileHeight && tY + y < image.Height; tY++)
								for (var tX = 0; tX < tileWidth && tX + x < image.Width; tX++)
								{
									var sourceColor = image.GetPixel(x + tX, y + tY);
									double distance = -1;
									var destColor = Color.Black;
									foreach (var color in palette.Palette)
									{
										var myDistance = GetDistance(sourceColor, color);
										if (distance == -1 || myDistance < distance)
										{
											distance = myDistance;
											destColor = color;
										}
									}
									destImage.SetPixel(x + tX, y + tY, destColor);
								}
							i++;
						}
					}
					destImage.Save(settings.GetFilePath(0), ImageFormat.Png);
					timer.Stop();
					output("Saved reduced image to " + settings.OutputFiles[0]);

				}
			}

		}

		private void FindNeighbour(PaletteInstance palette, List<PaletteInstance> paletteInstances)
		{
			palette.NeighborDistance = -1;
			foreach (var otherPalette in paletteInstances)
			{
				if (otherPalette == palette) continue;
				double paletteDistance = 0;
				for (var j = 0; j < palette.Palette.Count; j++)
				{
					double smallestDistance = -1;
					for (var k = 0; k < otherPalette.Palette.Count; k++)
					{
						var myDistance = GetDistance(palette.Palette[j], otherPalette.Palette[k]);
						if (smallestDistance == -1 || myDistance < smallestDistance) smallestDistance = myDistance;
					}
					paletteDistance += smallestDistance;
				}
				if (palette.NeighborDistance == -1 || paletteDistance < palette.NeighborDistance)
				{
					palette.NeighborDistance = paletteDistance;
					palette.NearestNeighbor = otherPalette;
				}
			}
		}

		private class PaletteInstance
		{
			public PaletteInstance(List<Color> tilePalette, int tileIndex)
			{
				Palette = tilePalette;
				Tiles = new List<int> { tileIndex };
			}

			public List<Color> Palette { get; set; }
			public List<int> Tiles { get; set; }

			public double NeighborDistance { get; set; }
			public PaletteInstance NearestNeighbor { get; set; }

			internal bool Matches(List<Color> otherPalette)
			{
				return (otherPalette.All(c => Palette.Contains(c)) || Palette.All(c => otherPalette.Contains(c)));
			}
		}

		private static List<Color> GetOptimizedPalette(List<Color> tilePalette, int colorCount)
		{
			while (tilePalette.Count > colorCount)
			{
				int removeIndex = 0;
				double removeDistance = -1;
				for (var i = 0; i < tilePalette.Count; i++)
				{
					double myDistance = 0;
					foreach (var color2 in tilePalette)
					{
						myDistance += GetDistance(tilePalette[i], color2);
					}
					if (removeDistance == -1 || removeDistance > myDistance)
					{
						removeIndex = i;
						removeDistance = myDistance;
					}
				}
				tilePalette.RemoveAt(removeIndex);
			}
			return tilePalette;
		}

		private static double GetDistance(Color color1, Color color2)
		{
			var lab1 = LabIndex.ContainsKey(color1) ? LabIndex[color1] : (LabIndex[color1] = rgb2lab(color1.R / 255f, color1.G / 255f, color1.B / 255f));
			var lab2 = LabIndex.ContainsKey(color2) ? LabIndex[color2] : (LabIndex[color2] = rgb2lab(color2.R / 255f, color2.G / 255f, color2.B / 255f));
			return Math.Sqrt(Math.Pow(lab2[0] - lab1[0], 2) + Math.Pow(lab2[1] - lab1[1], 2) + Math.Pow(lab2[2] - lab1[2], 2));
		}
		private static float Gamma(float x)
		{
			return x > 0.04045f ? (float)Math.Pow((x + 0.055f) / 1.055f, 2.4f) : x / 12.92f;
		}

		public static float[] rgb2lab(float var_R, float var_G, float var_B)
		{

			float[] arr = new float[3];
			float B = Gamma(var_B);
			float G = Gamma(var_G);
			float R = Gamma(var_R);
			float X = 0.412453f * R + 0.357580f * G + 0.180423f * B;
			float Y = 0.212671f * R + 0.715160f * G + 0.072169f * B;
			float Z = 0.019334f * R + 0.119193f * G + 0.950227f * B;

			X /= 0.95047f;
			Y /= 1.0f;
			Z /= 1.08883f;

			float FX = X > 0.008856f ? (float)Math.Pow(X, 1.0f / 3.0f) : (7.787f * X + 0.137931f);
			float FY = Y > 0.008856f ? (float)Math.Pow(Y, 1.0f / 3.0f) : (7.787f * Y + 0.137931f);
			float FZ = Z > 0.008856f ? (float)Math.Pow(Z, 1.0f / 3.0f) : (7.787f * Z + 0.137931f);
			arr[0] = Y > 0.008856f ? (116.0f * FY - 16.0f) : (903.3f * Y);
			arr[1] = 500f * (FX - FY);
			arr[2] = 200f * (FY - FZ);
			return arr;

		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".reduced.png");
		}

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("bitdepth", PipelinePropertyType.Select, "2", new List<string> { "2", "4" }),
					new PipelineProperty("palette-count", PipelinePropertyType.Text, "4")
				};
			}
		}
	}
}
