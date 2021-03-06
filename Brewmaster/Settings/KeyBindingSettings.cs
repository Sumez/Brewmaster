﻿using System.Collections.Generic;
using System.Windows.Forms;
using Brewmaster.Emulation;

namespace Brewmaster.Settings
{
	public class KeyBindingSettings : Control
	{
		public KeyBindingSettings()
		{
		}

		public List<KeyboardMapping> Mappings { get; private set; }

		public void SetMappings(List<KeyboardMapping> mappings)
		{
			Mappings = mappings;
			SuspendLayout();
			Controls.Clear();
			foreach (var mapping in mappings)
			{
				var panel = new Panel();
				panel.Dock = DockStyle.Top;
				panel.Height = 25;

				var button = new Button();
				button.Text = mapping.Name;
				button.Size = new System.Drawing.Size(75, 23);
				button.UseVisualStyleBackColor = true;

				panel.Controls.Add(button);

				var label = new Label();
				label.Location = new System.Drawing.Point(80, 5);
				label.Text = mapping.MappedToName ?? NesEmulatorHandler.GetInputName(mapping.MappedTo);

				panel.Controls.Add(label);
				button.Click += (s, a) =>
				{
					using (var keyAssignDialog = new ButtonAssignment(false))
					{
						keyAssignDialog.StartPosition = FormStartPosition.CenterParent;
						keyAssignDialog.ShowDialog(this);
						mapping.MappedTo = keyAssignDialog.KeyCode;
						mapping.MappedToName = keyAssignDialog.ButtonName;
					}
					label.Text = mapping.MappedToName ?? NesEmulatorHandler.GetInputName(mapping.MappedTo);
					SelectNextControl(button, true, false, true, true);
				};

				Controls.Add(panel);
				Controls.SetChildIndex(panel, 0);
			}
			ResumeLayout();
		}

		protected override void OnLayout(LayoutEventArgs a)
		{
			base.OnLayout(a);
			if (Controls.Count == 0) return;
			Height = Controls[0].Top + Controls[0].Height;
			Parent.PerformLayout(this, "Height");
		}
	}

}
