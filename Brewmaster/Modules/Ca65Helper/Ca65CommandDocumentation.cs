using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Modules.OpcodeHelper;

namespace Brewmaster.Modules.Ca65Helper
{
	public class Ca65CommandDocumentation : UserControl
	{
		private readonly Events _events;
		private Panel descriptionPanel;
		private Label title;
		private ToolTip toolTip;
		private System.ComponentModel.IContainer components;
		private FlowLayoutPanel seeAlsoPanel;
		private Panel seeAlsoContainer;

		public Ca65CommandDocumentation()
		{
			InitializeComponent();
			_codeFont = new Font(new FontFamily("Courier New"), 12, FontStyle.Regular, GraphicsUnit.Pixel);
		}

		public Ca65CommandDocumentation(Events events) : this()
		{
			_events = events;
			_events.HighlightedCommand += SelectCommand;

		}

		private readonly Font _codeFont;
		private Ca65Command _selected;
		private List<Control> _adjustControls = new List<Control>();
		private void SelectCommand(Ca65Command command)
		{
			if (command == _selected) return;
			_selected = command;
			_adjustControls = new List<Control>();

			var allCommands = Ca65Parser.GetCommands();
			title.Text = string.Join(", ", command.Aliases);
			SuspendLayout();

			descriptionPanel.Controls.Clear();
			for (var i = command.Description.Count - 1; i >= 0; i--)
			{
				var description = command.Description[i];
				var line = new Label
				{
					Text = description.Text,
					Padding = new Padding(0, 5, 0, 0),
					Dock = DockStyle.Top,
					AutoSize = true,
					MaximumSize = description.CodeExample ? Size.Empty : new Size(descriptionPanel.Width, 0)
				};
				if (!description.CodeExample) _adjustControls.Add(line);
				if (description.CodeExample) line.Font = _codeFont;
				descriptionPanel.Controls.Add(line);
			}

			seeAlsoContainer.Visible = command.SeeAlso.Count > 0;
			seeAlsoPanel.Controls.Clear();
			foreach (var reference in command.SeeAlso)
			{
				var label = new LinkLabel { Text = reference, AutoSize = true };
				if (allCommands.ContainsKey(reference)) label.Click += (a, s) => SelectCommand(allCommands[reference]);
				seeAlsoPanel.Controls.Add(label);
			}

			ResumeLayout();
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			foreach (var control in _adjustControls) control.MaximumSize = new Size(descriptionPanel.Width, 0);
			base.OnLayout(e);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			Brewmaster.StatusView.HorizontalLine horizontalLine2;
			System.Windows.Forms.Panel panel;
			System.Windows.Forms.Label seeAlsoLabel;
			this.seeAlsoContainer = new System.Windows.Forms.Panel();
			this.seeAlsoPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.descriptionPanel = new System.Windows.Forms.Panel();
			this.title = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			horizontalLine2 = new Brewmaster.StatusView.HorizontalLine();
			panel = new System.Windows.Forms.Panel();
			seeAlsoLabel = new System.Windows.Forms.Label();
			panel.SuspendLayout();
			this.seeAlsoContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// horizontalLine2
			// 
			horizontalLine2.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine2.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine2.Location = new System.Drawing.Point(0, 20);
			horizontalLine2.Name = "horizontalLine2";
			horizontalLine2.Size = new System.Drawing.Size(238, 1);
			horizontalLine2.TabIndex = 4;
			horizontalLine2.Text = "horizontalLine2";
			// 
			// panel
			// 
			panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			panel.AutoScroll = true;
			panel.Controls.Add(this.seeAlsoContainer);
			panel.Controls.Add(this.descriptionPanel);
			panel.Controls.Add(horizontalLine2);
			panel.Controls.Add(this.title);
			panel.Location = new System.Drawing.Point(3, 6);
			panel.Name = "panel";
			panel.Size = new System.Drawing.Size(238, 487);
			panel.TabIndex = 2;
			// 
			// seeAlsoContainer
			// 
			this.seeAlsoContainer.AutoSize = true;
			this.seeAlsoContainer.Controls.Add(seeAlsoLabel);
			this.seeAlsoContainer.Controls.Add(this.seeAlsoPanel);
			this.seeAlsoContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.seeAlsoContainer.Location = new System.Drawing.Point(0, 41);
			this.seeAlsoContainer.MinimumSize = new System.Drawing.Size(0, 20);
			this.seeAlsoContainer.Name = "seeAlsoContainer";
			this.seeAlsoContainer.Size = new System.Drawing.Size(238, 43);
			this.seeAlsoContainer.TabIndex = 3;
			// 
			// seeAlsoLabel
			// 
			seeAlsoLabel.AutoSize = true;
			seeAlsoLabel.Location = new System.Drawing.Point(-3, 8);
			seeAlsoLabel.Name = "seeAlsoLabel";
			seeAlsoLabel.Size = new System.Drawing.Size(51, 13);
			seeAlsoLabel.TabIndex = 1;
			seeAlsoLabel.Text = "See also:";
			// 
			// seeAlsoPanel
			// 
			this.seeAlsoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.seeAlsoPanel.Location = new System.Drawing.Point(49, 8);
			this.seeAlsoPanel.Name = "seeAlsoPanel";
			this.seeAlsoPanel.Size = new System.Drawing.Size(186, 32);
			this.seeAlsoPanel.TabIndex = 0;
			// 
			// descriptionPanel
			// 
			this.descriptionPanel.AutoSize = true;
			this.descriptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.descriptionPanel.Location = new System.Drawing.Point(0, 21);
			this.descriptionPanel.MinimumSize = new System.Drawing.Size(0, 20);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.Size = new System.Drawing.Size(238, 20);
			this.descriptionPanel.TabIndex = 2;
			// 
			// title
			// 
			this.title.Dock = System.Windows.Forms.DockStyle.Top;
			this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.title.Location = new System.Drawing.Point(0, 0);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(238, 20);
			this.title.TabIndex = 0;
			// 
			// toolTip
			// 
			this.toolTip.AutomaticDelay = 2;
			this.toolTip.AutoPopDelay = 0;
			this.toolTip.InitialDelay = 2;
			this.toolTip.ReshowDelay = 0;
			// 
			// Ca65CommandDocumentation
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(panel);
			this.Name = "Ca65CommandDocumentation";
			this.Size = new System.Drawing.Size(244, 496);
			panel.ResumeLayout(false);
			panel.PerformLayout();
			this.seeAlsoContainer.ResumeLayout(false);
			this.seeAlsoContainer.PerformLayout();
			this.ResumeLayout(false);

		}

	}

}
