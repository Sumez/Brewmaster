using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.MemoryViewer
{
	public class MemoryViewer : UserControl
	{
		private VScrollBar _vScrollBar;
		private HScrollBar _hScrollBar;
		private Panel _hScrollPanel;
		private HexEditor _editor;

		private int Rows { get; set; }
		public Action<int, byte> DataChanged { set { _editor.DataChanged = value; }}
		public event Action<string, bool> AddWatch { add { _editor.AddWatch += value; } remove { _editor.AddWatch -= value; } }
		public event Action<int, Breakpoint.Types> AddBreakpoint { add { _editor.AddBreakpoint += value; } remove { _editor.AddBreakpoint -= value; } }
		public event Action<IEnumerable<Breakpoint>> RemoveBreakpoints { add { _editor.RemoveBreakpoints += value; } remove { _editor.RemoveBreakpoints -= value; } }

		public MemoryViewer()
		{
			InitializeComponent();

			Visible = false;
			SuspendLayout();

			Font = new Font("Consolas", 10, FontStyle.Regular);
			Margin = Padding.Empty;

			_hScrollBar.LargeChange = 50;
			_editor.HScrollBar = _hScrollBar;
			_editor.VScrollBar = _vScrollBar;

			_hScrollBar.Scroll += (s, a) => _editor.Invalidate();
			_vScrollBar.Scroll += (s, a) => _editor.Invalidate();

			ResumeLayout();
			Visible = true;
		}
		
		public void SetData(byte[] bytes)
		{
			_editor.SetData(bytes);
		}

		private System.ComponentModel.IContainer components;

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			if ((_hScrollBar.Maximum >= _hScrollBar.LargeChange) != _hScrollPanel.Visible)
			{
				_hScrollPanel.Visible = _hScrollBar.Maximum >= _hScrollBar.LargeChange;
			}
			base.OnLayout(levent);
		}

		private void InitializeComponent()
		{
			this._vScrollBar = new System.Windows.Forms.VScrollBar();
			this._hScrollBar = new System.Windows.Forms.HScrollBar();
			this._editor = new Brewmaster.MemoryViewer.HexEditor();
			this._hScrollPanel = new System.Windows.Forms.Panel();
			this._hScrollPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _vScrollBar
			// 
			this._vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this._vScrollBar.Location = new System.Drawing.Point(443, 0);
			this._vScrollBar.Name = "_vScrollBar";
			this._vScrollBar.Size = new System.Drawing.Size(17, 237);
			this._vScrollBar.TabIndex = 0;
			// 
			// _hScrollBar
			// 
			this._hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._hScrollBar.Location = new System.Drawing.Point(0, 0);
			this._hScrollBar.Name = "_hScrollBar";
			this._hScrollBar.Size = new System.Drawing.Size(443, 17);
			this._hScrollBar.TabIndex = 0;
			// 
			// _editor
			// 
			this._editor.BackColor = System.Drawing.SystemColors.Control;
			this._editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._editor.Font = new System.Drawing.Font("Consolas", 10F);
			this._editor.HScrollBar = null;
			this._editor.Location = new System.Drawing.Point(0, 0);
			this._editor.Margin = new System.Windows.Forms.Padding(0);
			this._editor.Name = "_editor";
			this._editor.Size = new System.Drawing.Size(443, 237);
			this._editor.TabIndex = 0;
			this._editor.Text = "hexEditor1";
			this._editor.VScrollBar = null;
			// 
			// _hScrollPanel
			// 
			this._hScrollPanel.AutoSize = true;
			this._hScrollPanel.Controls.Add(this._hScrollBar);
			this._hScrollPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._hScrollPanel.Location = new System.Drawing.Point(0, 237);
			this._hScrollPanel.Name = "_hScrollPanel";
			this._hScrollPanel.Size = new System.Drawing.Size(460, 17);
			this._hScrollPanel.TabIndex = 0;
			// 
			// MemoryViewer
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this._editor);
			this.Controls.Add(this._vScrollBar);
			this.Controls.Add(this._hScrollPanel);
			this.Name = "MemoryViewer";
			this.Size = new System.Drawing.Size(460, 254);
			this._hScrollPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			_editor.SetBreakpoints(breakpoints);
		}
	}
}
