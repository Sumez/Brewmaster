using System.Collections.Generic;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class FloodFill : PixelTool
	{
		public FloodFill(MapEditorState state, TileMap map) : base(state, map)
		{
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.EnsureRefreshedImage(); // Makes sure screen.Image is reliable
			var fillRegion = screen.Image.GetFillRegion(x, y);

			var changedPixels = new HashSet<int>();
			var tileWidth = _map.BaseTileSize.Width;
			var tileHeight = _map.BaseTileSize.Height;
			var tileSize = _map.BaseTileSize.Width * _map.BaseTileSize.Height;
			foreach (var index in fillRegion)
			{
				var pixelX = index % screen.Image.Width;
				var pixelY = index / screen.Image.Width;
				var tilePixelX = pixelX % tileWidth;
				var tilePixelY = pixelY % tileHeight;
				var pixelTile = screen.GetTile(pixelX / tileWidth, pixelY / tileHeight);

				// Prevent updating the same CHR address more than once, by generating a unique hash for each tile+coordinate combination
				var pixelId = pixelTile * tileSize + tilePixelY * tileWidth + tilePixelX;
				if (changedPixels.Contains(pixelId)) continue;
				changedPixels.Add(pixelId);

				_state.SetPixel(pixelTile, tilePixelX, tilePixelY, SelectedColor);
			}
		}
	}
}