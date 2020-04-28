using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Brewmaster.Modules.OpcodeHelper
{
	public class OpcodeHelper : UserControl
	{
		private readonly Events _events;
		private Panel buttonPanelContainer;
		private Panel panel2;
		private Label title;
		private Label description;
		private Panel descriptionPanel;
		private Label affectFlags;
		private FlagIndicator flagC;
		private FlagIndicator flagN;
		private FlagIndicator flagV;
		private FlagIndicator flagZ;
		private ToolTip toolTip;
		private System.ComponentModel.IContainer components;
		private FlowLayoutPanel buttonPanel;
		private Label subDescription;
		private FlowLayoutPanel addressingModesPanel;
		private readonly Dictionary<Opcode, OpcodeButton> _buttons = new Dictionary<Opcode, OpcodeButton>();

		public OpcodeHelper()
		{
			InitializeComponent();

			_opcodes = OpcodeParser.GetOpcodes().Values.ToList();
			foreach (var opcode in _opcodes)
			{
				var button = new OpcodeButton(opcode) {Margin = new Padding(0, 0, 1, 1)};
				buttonPanel.Controls.Add(button);
				_buttons.Add(opcode, button);
				button.Click += (s, a) => SelectOpcode(opcode);
				toolTip.SetToolTip(button, opcode.Title);
			}
			SelectOpcode(_opcodes.First());
		}

		public OpcodeHelper(Events events) : this()
		{
			_events = events;
			_events.HighlightedOpcode += SelectOpcode;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (_selected == null) return base.ProcessCmdKey(ref msg, keyData);
			if (keyData == Keys.Up) return ChangeSelectedY(-1);
			if (keyData == Keys.Down) return ChangeSelectedY(1);
			if (keyData == Keys.Left) return ChangeSelectedX(-1);
			if (keyData == Keys.Right) return ChangeSelectedX(1);
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private bool ChangeSelectedX(int offsetX)
		{
			if (_selected == null) return false;
			var index = _opcodes.IndexOf(_selected) + offsetX;

			if (index >= 0 && index < _opcodes.Count) SelectOpcode(_opcodes[index]);
			return true;
		}

		private bool ChangeSelectedY(int offsetY)
		{
			if (_selected == null) return false;
			var currentButton = _buttons[_selected];
			var targetPosition = new Point(currentButton.Left, currentButton.Top + (currentButton.Height / 2) + (offsetY * currentButton.Height));
			foreach (var opcode in _opcodes)
			{
				if (!_buttons[opcode].Bounds.Contains(targetPosition)) continue;

				SelectOpcode(opcode);
				return true;
			}
			return true;
		}

		private Opcode _selected;
		private List<Opcode> _opcodes;

		public void SelectOpcode(Opcode opcode)
		{
			if (opcode == _selected) return;
			_selected = opcode;
			
			var button = _buttons[opcode];
			button.Selected = true;
			foreach (var otherButton in buttonPanel.Controls.OfType<OpcodeButton>().Where(b => b != button)) otherButton.Selected = false;
			buttonPanelContainer.ScrollControlIntoView(button);

			title.Text = opcode.Title;
			var stringBuilder = new StringBuilder();
			for (var i = 0; i < opcode.Description.Count; i++)
			{
				if (i == 0 && opcode.Description.Count > 1) subDescription.Text = opcode.Description[i];
				else stringBuilder.AppendLine(opcode.Description[i]);
			}
			subDescription.Visible = opcode.Description.Count > 1;
			description.Text = stringBuilder.ToString();
			flagC.Checked = opcode.AffectedFlags.HasFlag(AffectedFlag.C);
			flagZ.Checked = opcode.AffectedFlags.HasFlag(AffectedFlag.Z);
			flagV.Checked = opcode.AffectedFlags.HasFlag(AffectedFlag.V);
			flagN.Checked = opcode.AffectedFlags.HasFlag(AffectedFlag.N);
			toolTip.SetToolTip(flagC, opcode.FlagExplanations.ContainsKey(AffectedFlag.C) ? opcode.FlagExplanations[AffectedFlag.C] : null);
			toolTip.SetToolTip(flagZ, opcode.FlagExplanations.ContainsKey(AffectedFlag.Z) ? opcode.FlagExplanations[AffectedFlag.Z] : null);
			toolTip.SetToolTip(flagV, opcode.FlagExplanations.ContainsKey(AffectedFlag.V) ? opcode.FlagExplanations[AffectedFlag.V] : null);
			toolTip.SetToolTip(flagN, opcode.FlagExplanations.ContainsKey(AffectedFlag.N) ? opcode.FlagExplanations[AffectedFlag.N] : null);

			addressingModesPanel.SuspendLayout();
			addressingModesPanel.Controls.Clear();
			foreach (var addressingMode in opcode.AddressingModes)
			{
				addressingModesPanel.Controls.Add(new AddressingModeDescription(addressingMode, toolTip) { Margin = Padding.Empty });
			}
			addressingModesPanel.ResumeLayout();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Panel flagPanel;
			Brewmaster.StatusView.HorizontalLine horizontalLine2;
			Brewmaster.StatusView.HorizontalLine horizontalLine1;
			this.flagN = new Brewmaster.Modules.OpcodeHelper.FlagIndicator();
			this.flagV = new Brewmaster.Modules.OpcodeHelper.FlagIndicator();
			this.flagZ = new Brewmaster.Modules.OpcodeHelper.FlagIndicator();
			this.flagC = new Brewmaster.Modules.OpcodeHelper.FlagIndicator();
			this.affectFlags = new System.Windows.Forms.Label();
			this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonPanelContainer = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.descriptionPanel = new System.Windows.Forms.Panel();
			this.addressingModesPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.description = new System.Windows.Forms.Label();
			this.subDescription = new System.Windows.Forms.Label();
			this.title = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			flagPanel = new System.Windows.Forms.Panel();
			horizontalLine2 = new Brewmaster.StatusView.HorizontalLine();
			horizontalLine1 = new Brewmaster.StatusView.HorizontalLine();
			flagPanel.SuspendLayout();
			this.buttonPanelContainer.SuspendLayout();
			this.panel2.SuspendLayout();
			this.descriptionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// flagPanel
			// 
			flagPanel.Controls.Add(this.flagN);
			flagPanel.Controls.Add(this.flagV);
			flagPanel.Controls.Add(this.flagZ);
			flagPanel.Controls.Add(this.flagC);
			flagPanel.Controls.Add(this.affectFlags);
			flagPanel.Dock = System.Windows.Forms.DockStyle.Top;
			flagPanel.Location = new System.Drawing.Point(0, 21);
			flagPanel.Margin = new System.Windows.Forms.Padding(0);
			flagPanel.Name = "flagPanel";
			flagPanel.Size = new System.Drawing.Size(238, 28);
			flagPanel.TabIndex = 3;
			// 
			// flagN
			// 
			this.flagN.Checked = false;
			this.flagN.Flag = "N";
			this.flagN.Location = new System.Drawing.Point(123, 5);
			this.flagN.Name = "flagN";
			this.flagN.Size = new System.Drawing.Size(18, 18);
			this.flagN.TabIndex = 6;
			// 
			// flagV
			// 
			this.flagV.Checked = false;
			this.flagV.Flag = "V";
			this.flagV.Location = new System.Drawing.Point(104, 5);
			this.flagV.Name = "flagV";
			this.flagV.Size = new System.Drawing.Size(18, 18);
			this.flagV.TabIndex = 5;
			// 
			// flagZ
			// 
			this.flagZ.Checked = false;
			this.flagZ.Flag = "Z";
			this.flagZ.Location = new System.Drawing.Point(85, 5);
			this.flagZ.Name = "flagZ";
			this.flagZ.Size = new System.Drawing.Size(18, 18);
			this.flagZ.TabIndex = 4;
			// 
			// flagC
			// 
			this.flagC.Checked = false;
			this.flagC.Flag = "C";
			this.flagC.Location = new System.Drawing.Point(66, 5);
			this.flagC.Name = "flagC";
			this.flagC.Size = new System.Drawing.Size(18, 18);
			this.flagC.TabIndex = 3;
			// 
			// affectFlags
			// 
			this.affectFlags.AutoSize = true;
			this.affectFlags.Location = new System.Drawing.Point(0, 7);
			this.affectFlags.Name = "affectFlags";
			this.affectFlags.Size = new System.Drawing.Size(68, 13);
			this.affectFlags.TabIndex = 2;
			this.affectFlags.Text = "Affects flags:";
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
			// horizontalLine1
			// 
			horizontalLine1.Dock = System.Windows.Forms.DockStyle.Top;
			horizontalLine1.LineColor = System.Drawing.SystemColors.ButtonShadow;
			horizontalLine1.Location = new System.Drawing.Point(0, 59);
			horizontalLine1.Name = "horizontalLine1";
			horizontalLine1.Size = new System.Drawing.Size(244, 1);
			horizontalLine1.TabIndex = 0;
			horizontalLine1.Text = "horizontalLine1";
			// 
			// buttonPanel
			// 
			this.buttonPanel.AutoSize = true;
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonPanel.Location = new System.Drawing.Point(0, 0);
			this.buttonPanel.MinimumSize = new System.Drawing.Size(0, 50);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new System.Drawing.Size(244, 50);
			this.buttonPanel.TabIndex = 0;
			// 
			// buttonPanelContainer
			// 
			this.buttonPanelContainer.AutoScroll = true;
			this.buttonPanelContainer.Controls.Add(this.buttonPanel);
			this.buttonPanelContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.buttonPanelContainer.Location = new System.Drawing.Point(0, 0);
			this.buttonPanelContainer.Name = "buttonPanelContainer";
			this.buttonPanelContainer.Size = new System.Drawing.Size(244, 59);
			this.buttonPanelContainer.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.Window;
			this.panel2.Controls.Add(this.descriptionPanel);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 60);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(244, 436);
			this.panel2.TabIndex = 2;
			// 
			// descriptionPanel
			// 
			this.descriptionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptionPanel.AutoScroll = true;
			this.descriptionPanel.Controls.Add(this.addressingModesPanel);
			this.descriptionPanel.Controls.Add(this.description);
			this.descriptionPanel.Controls.Add(this.subDescription);
			this.descriptionPanel.Controls.Add(flagPanel);
			this.descriptionPanel.Controls.Add(horizontalLine2);
			this.descriptionPanel.Controls.Add(this.title);
			this.descriptionPanel.Location = new System.Drawing.Point(3, 6);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.Size = new System.Drawing.Size(238, 427);
			this.descriptionPanel.TabIndex = 2;
			// 
			// addressingModesPanel
			// 
			this.addressingModesPanel.AutoSize = true;
			this.addressingModesPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.addressingModesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.addressingModesPanel.Location = new System.Drawing.Point(0, 85);
			this.addressingModesPanel.Name = "addressingModesPanel";
			this.addressingModesPanel.Size = new System.Drawing.Size(238, 0);
			this.addressingModesPanel.TabIndex = 6;
			this.addressingModesPanel.WrapContents = false;
			// 
			// description
			// 
			this.description.AutoSize = true;
			this.description.Dock = System.Windows.Forms.DockStyle.Top;
			this.description.Location = new System.Drawing.Point(0, 67);
			this.description.MaximumSize = new System.Drawing.Size(200, 0);
			this.description.Name = "description";
			this.description.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.description.Size = new System.Drawing.Size(0, 18);
			this.description.TabIndex = 1;
			// 
			// subDescription
			// 
			this.subDescription.AutoSize = true;
			this.subDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.subDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.subDescription.Location = new System.Drawing.Point(0, 49);
			this.subDescription.Name = "subDescription";
			this.subDescription.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.subDescription.Size = new System.Drawing.Size(0, 18);
			this.subDescription.TabIndex = 5;
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
			// OpcodeHelper
			// 
			this.Controls.Add(this.panel2);
			this.Controls.Add(horizontalLine1);
			this.Controls.Add(this.buttonPanelContainer);
			this.Name = "OpcodeHelper";
			this.Size = new System.Drawing.Size(244, 496);
			flagPanel.ResumeLayout(false);
			flagPanel.PerformLayout();
			this.buttonPanelContainer.ResumeLayout(false);
			this.buttonPanelContainer.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.descriptionPanel.ResumeLayout(false);
			this.descriptionPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			buttonPanelContainer.Height = Math.Min(110, buttonPanel.Height);
			subDescription.MaximumSize = description.MaximumSize = new Size(descriptionPanel.Width, 0);
			base.OnLayout(e);
		}

	}

	public class OpcodeButton : UserControl
	{
		public bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value;
				Invalidate();
			}
		}

		private readonly Opcode _opcode;
		private bool _hover;
		private bool _selected;

		public OpcodeButton(Opcode opcode)
		{
			_opcode = opcode;
			Width = 30;
			Height = 20;
			Cursor = Cursors.Hand;
			DoubleBuffered = true;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			_hover = true;
			base.OnMouseEnter(e);
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_hover = false;
			base.OnMouseLeave(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.FillRectangle(Selected ? Brushes.DodgerBlue : _hover ? Brushes.LightGray : Brushes.White, ClientRectangle);
			e.Graphics.DrawString(_opcode.Command, Font, Selected ? Brushes.White : Brushes.Black, ClientRectangle, new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
		}
	}

	public class FlagIndicator : UserControl
	{
		private bool _checked;
		public string Flag { get; set; }
		public bool Checked
		{
			get { return _checked; }
			set
			{
				_checked = value;
				Invalidate();
			}
		}

		public FlagIndicator()
		{
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			ButtonRenderer.DrawButton(e.Graphics, ClientRectangle, Checked ? PushButtonState.Pressed : PushButtonState.Disabled);
			e.Graphics.DrawString(Flag, Font, Brushes.Black, ClientRectangle, new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
		}
	}
}
