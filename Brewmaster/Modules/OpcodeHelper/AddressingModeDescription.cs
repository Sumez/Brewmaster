using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brewmaster.Modules.OpcodeHelper
{
	public class AddressingModeDescription : UserControl
	{
		private Label bytes;
		private Label cycles;
		private FlowLayoutPanel flowLayoutPanel1;
		private Label description;

		private void InitializeComponent()
		{
			this.description = new System.Windows.Forms.Label();
			this.bytes = new System.Windows.Forms.Label();
			this.cycles = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// description
			// 
			this.description.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.description.Location = new System.Drawing.Point(3, 0);
			this.description.Name = "description";
			this.description.Size = new System.Drawing.Size(71, 15);
			this.description.TabIndex = 0;
			this.description.Text = "Zero Page, X";
			// 
			// bytes
			// 
			this.bytes.AutoSize = true;
			this.bytes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
			this.bytes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bytes.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.bytes.Location = new System.Drawing.Point(80, 0);
			this.bytes.Name = "bytes";
			this.bytes.Size = new System.Drawing.Size(40, 15);
			this.bytes.TabIndex = 1;
			this.bytes.Text = "2 bytes";
			// 
			// cycles
			// 
			this.cycles.AutoSize = true;
			this.cycles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(250)))), ((int)(((byte)(220)))));
			this.cycles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.cycles.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.cycles.Location = new System.Drawing.Point(126, 0);
			this.cycles.Name = "cycles";
			this.cycles.Size = new System.Drawing.Size(64, 15);
			this.cycles.TabIndex = 2;
			this.cycles.Text = "3 (+2) cycles";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.description);
			this.flowLayoutPanel1.Controls.Add(this.bytes);
			this.flowLayoutPanel1.Controls.Add(this.cycles);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(230, 15);
			this.flowLayoutPanel1.TabIndex = 3;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// AddressingModeDescription
			// 
			this.AutoSize = true;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "AddressingModeDescription";
			this.Size = new System.Drawing.Size(321, 18);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public AddressingModeDescription(AddressingMode mode, ToolTip toolTip)
		{
			InitializeComponent();

			description.Text = mode.Name;
			bytes.Text = string.Format("{0} bytes", mode.Bytes);

			var match = Regex.Match(mode.Cycles, "^([0-9]+)(.*)$");
			if (!(cycles.Visible = match.Success)) return;

			var cycleCount = int.Parse(match.Groups[1].Value);
			var cycleText = string.Format("{0} cycles", cycleCount);
			if (match.Groups.Count > 2)
			{
				var cycleExplanation = match.Groups[2].Value.Trim();
				var match2 = Regex.Match(cycleExplanation, @"\+[0-9]+(?!.*\+.*)");
				//if (match2.Success) cycleText = string.Format("{0} ({1}) cycles", cycleCount, match2.Value);
				if (match2.Success) cycleText = string.Format("{0} (+) cycles", cycleCount);
				toolTip.SetToolTip(cycles, cycleExplanation);
			}
			cycles.Text = cycleText;
		}
	}
}
