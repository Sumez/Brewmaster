namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class FloodFill : PixelTool
	{
		public FloodFill(MapEditorState state, TileMap map) : base(state, map)
		{
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			var tileX = x / _map.BaseTileSize.Width;
			var tileY = y / _map.BaseTileSize.Height;

			var tile = screen.GetTile(tileX, tileY);
			var palette = screen.GetColorTile(tileX, tileY);

			//screen.Image.FillRegion(screen.Image.GetFillRegion(x, y), _map.Palettes[palette].Colors[SelectedColor]);

			var fillRegion = screen.Image.GetFillRegion(x, y);
			foreach (var index in fillRegion)
			{
				var pixelX = index % screen.Image.Width;
				var pixelY = index / screen.Image.Width;
				var pixelTile = screen.GetTile(pixelX / _map.BaseTileSize.Width, pixelY / _map.BaseTileSize.Height);

				_state.SetPixel(pixelTile, pixelX % _map.BaseTileSize.Width, pixelY % _map.BaseTileSize.Height, SelectedColor);
			}
			//_state.SetPixel(tile, x % _map.BaseTileSize.Width, y % _map.BaseTileSize.Height, SelectedColor);
		}
	}
}