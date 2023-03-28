using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.MemoryViewer
{
	public class MemoryTabs : TabControl
	{
		public MemoryTabs(Modules.Events events)
		{
			CpuMemoryTab = new TabPage("CPU");
			PpuMemoryTab = new TabPage("PPU");
			OamMemoryTab = new TabPage("OAM");
			SpcMemoryTab = new TabPage("SPC") { Visible = false };

			CpuMemoryTab.Controls.Add(Cpu = new MemoryViewer { Dock = DockStyle.Fill });
			PpuMemoryTab.Controls.Add(Ppu = new MemoryViewer { Dock = DockStyle.Fill });
			OamMemoryTab.Controls.Add(Oam = new MemoryViewer { Dock = DockStyle.Fill });
			SpcMemoryTab.Controls.Add(Spc = new MemoryViewer { Dock = DockStyle.Fill });

			Alignment = TabAlignment.Bottom;
			Controls.AddRange(new Control[] { CpuMemoryTab, PpuMemoryTab, OamMemoryTab, SpcMemoryTab });

			events.EmulationStateUpdate += (state) =>
			{
				Cpu.SetData(state.Memory.CpuData);
				Ppu.SetData(state.Memory.PpuData);
				Oam.SetData(state.Memory.OamData);
				if (state.Type == TargetPlatform.Snes) Spc.SetData(state.Memory.SpcData);
			};

			events.PlatformChanged += platform => SpcMemoryTab.Visible = platform == TargetPlatform.Snes;
		}

		public MemoryViewer Cpu { get; set; }
		public MemoryViewer Ppu { get; set; }
		public MemoryViewer Oam { get; set; }
		public MemoryViewer Spc { get; set; }

		public TabPage OamMemoryTab { get; set; }
		public TabPage PpuMemoryTab { get; set; }
		public TabPage CpuMemoryTab { get; set; }
		public TabPage SpcMemoryTab { get; set; }
	}
}
