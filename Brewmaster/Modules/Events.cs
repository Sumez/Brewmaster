﻿using System;
using System.Collections.Generic;
using Brewmaster.Emulation;
using Brewmaster.Modules.Ca65Helper;
using Brewmaster.Modules.OpcodeHelper;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules
{
	// TODO: More organized handling of global events (breakpoints, watch values, status, etc)
	public class Events
	{
		public Action Cut;
		public Action Copy;
		public Action Paste;
		public Action Delete;
		public Action SelectAll;

		public Action<IEnumerable<Breakpoint>> RemoveBreakpoints;
		public Action<Breakpoint> AddBreakpoint;
		public Action UpdatedBreakpoints;
		public Func<AsmProject> GetCurrentProject;

		public event Action<EmulationState> EmulationStateUpdate;
		public int SelectedSprite { get; private set; } = -1;
		public ProjectType ProjectType { get; private set; } = ProjectType.Nes;

		public event Action<int> SelectedSpriteChanged;
		public event Action<ProjectType> ProjectTypeChanged;
		public event Action<Opcode> HighlightedOpcode;
		public event Action<Ca65Command> HighlightedCommand;

		public void HighlightOpcode(Opcode opcode)
		{
			if (HighlightedOpcode != null) HighlightedOpcode(opcode);
		}
		public void HighlightCommand(Ca65Command command)
		{
			if (HighlightedCommand != null) HighlightedCommand(command);
		}

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

		public void SetProjectType(ProjectType projectType)
		{
			if (projectType == ProjectType) return;
			ProjectType = projectType;
			if (ProjectTypeChanged != null) ProjectTypeChanged(projectType);
		}
	}
}
