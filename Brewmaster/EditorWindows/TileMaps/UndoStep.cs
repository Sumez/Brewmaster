using System.Collections.Generic;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class UndoStep
	{
		public byte[] Chr;
		public Dictionary<TileMapScreen, UndoState> States = new Dictionary<TileMapScreen, UndoState>();
		public void AddScreen(TileMapScreen screen)
		{
			States.Add(screen, screen.PreviousState);
			screen.RefreshPreviousState();
		}

		public UndoStep Revert(MapEditorState state)
		{
			var redoStep = new UndoStep();
			foreach (var screenState in States)
			{
				redoStep.States.Add(screenState.Key, screenState.Key.PreviousState);
				screenState.Key.RevertToState(screenState.Value);

				// TODO: Restore tile usages!!!
			}

			if (Chr != null)
			{
				redoStep.Chr = state.PreviousChrData;
				state.RevertChr(Chr);
			}

			return redoStep;
		}

		public void AddChr(MapEditorState state)
		{
			Chr = state.PreviousChrData;
			state.RefreshPreviousState();
		}
	}

	public struct UndoState
	{
		public int[] Tiles;
		public int[] Attributes;
		public int[] MetaValues;
		public MapObject[] Objects;
	}
}