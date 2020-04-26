using System;
using Brewmaster.Emulation;

namespace Brewmaster.Modules
{
	// TODO: More organized handling of global events (breakpoints, watch values, status, etc)
	public class Events
	{
		public event Action<EmulationState> EmulationStateUpdate;
		public int SelectedSprite { get; private set; } = -1;
		public event Action<int> SelectedSpriteChanged;

		public void UpdateStates(EmulationState state)
		{
			if (EmulationStateUpdate != null) EmulationStateUpdate(state);
		}

		public void SelectSprite(int spriteIndex)
		{
			if (spriteIndex == SelectedSprite) return;
			SelectedSprite = spriteIndex;
			if (SelectedSpriteChanged != null) SelectedSpriteChanged(SelectedSprite);
		}
	}
}
