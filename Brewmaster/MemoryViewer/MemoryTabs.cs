using System.Windows.Forms;

namespace Brewmaster.MemoryViewer
{
	public class MemoryTabs : TabControl
	{
		public MemoryTabs(Modules.Events events)
		{
			CpuMemoryTab = new TabPage("CPU");
			PpuMemoryTab = new TabPage("PPU");
			OamMemoryTab = new TabPage("OAM");

			CpuMemoryTab.Controls.Add(Cpu = new MemoryViewer { Dock = DockStyle.Fill });
			PpuMemoryTab.Controls.Add(Ppu = new MemoryViewer { Dock = DockStyle.Fill });
			OamMemoryTab.Controls.Add(Oam = new MemoryViewer { Dock = DockStyle.Fill });

			Alignment = TabAlignment.Bottom;
			Controls.AddRange(new Control[] { CpuMemoryTab, PpuMemoryTab, OamMemoryTab });

			events.EmulationStateUpdate += (state) =>
			{
				Cpu.SetData(state.Memory.CpuData);
				Ppu.SetData(state.Memory.PpuData);
				Oam.SetData(state.Memory.OamData);
			};
		}

		public MemoryViewer Cpu { get; set; }
		public MemoryViewer Ppu { get; set; }
		public MemoryViewer Oam { get; set; }

		public TabPage OamMemoryTab { get; set; }
		public TabPage PpuMemoryTab { get; set; }
		public TabPage CpuMemoryTab { get; set; }
	}
}
