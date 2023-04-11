using System.Collections.Generic;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class FloodFill : PixelTool
	{
		public FloodFill(MapEditorState state, TileMap map) : base(state, map)
		{
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			var createNewTile = CreateNewTile || Control.ModifierKeys.HasFlag(Keys.Control);

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
				if (createNewTile && _state.GetTileUsage(pixelTile) > 1)
				{
					pixelTile = _state.CopyTile(pixelTile);
					screen.PrintTile(pixelX / tileWidth, pixelY / tileHeight, pixelTile);
				}

				// Prevent updating the same CHR address more than once, by generating a unique hash for each tile+coordinate combination
				var pixelId = pixelTile * tileSize + tilePixelY * tileWidth + tilePixelX;
				if (changedPixels.Contains(pixelId)) continue;
				changedPixels.Add(pixelId);

				_state.SetPixel(pixelTile, tilePixelX, tilePixelY, SelectedColor);
			}
		}
	}
}