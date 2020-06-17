using System.Windows.Forms;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class PixelPen : PixelTool
	{
		public PixelPen(MapEditorState state, TileMap map) : base(state, map)
		{
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			var createNewTile = CreateNewTile || Control.ModifierKeys.HasFlag(Keys.Control);
			var tileX = x / _map.BaseTileSize.Width;
			var tileY = y / _map.BaseTileSize.Height;

			var tile = screen.GetTile(tileX, tileY);
			var palette = screen.GetColorTile(tileX, tileY);

			screen.Image.SetPixel(x, y, _map.Palettes[palette].Colors[SelectedColor]);

			if (createNewTile && _state.GetTileUsage(tile) > 1)
			{
				tile = _state.CopyTile(tile);
				screen.PrintTile(tileX, tileY, tile);
			}
			_state.SetPixel(tile, x % _map.BaseTileSize.Width, y % _map.BaseTileSize.Height, SelectedColor);
		}
	}
}