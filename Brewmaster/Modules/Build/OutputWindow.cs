using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Brewmaster.Modules.Build
{
	public class OutputWindow : Panel
	{
		public Action<string, int> GoTo { get; set; }

		private StringBuilder _logString = new StringBuilder();
		private List<LogData> _log = new List<LogData>();
		private HScrollBar hScrollBar;
		private VScrollBar vScrollBar;
		private int _lineHeight;
		public OutputWindow()
		{
			InitializeComponent();

			Dock = DockStyle.Fill;
			//OutputTextBox.Dock = DockStyle.None;
			//OutputTextBox.Width = 300;
			//OutputTextBox.Height = 4000;
			OutputTextBox.ScrollBars = RichTextBoxScrollBars.Horizontal;
			Controls.Add(OutputTextBox);
			Controls.Add(vScrollBar);
			//Controls.Add(hScrollBar);

			using (var g = OutputTextBox.CreateGraphics())
			{
				_lineHeight = TextRenderer.MeasureText(g, "I", OutputTextBox.Font).Height;
			}

			OutputTextBox.MouseWheel += (s, a) => OnMouseWheel(a);

			vScrollBar.Scroll += (sender, args) => RefreshOutput();

			OutputTextBox.MouseDoubleClick += (sender, args) => { ClickedLine(args.Location.Y / _lineHeight); };

			ResumeLayout();
		}

		private void ClickedLine(int visibleLineNumber)
		{
			var logicalLine = visibleLineNumber + vScrollBar.Value;
			if (logicalLine < 0 || logicalLine >= _log.Count) return;
			var line = _log[logicalLine];
			if (line.Location != null)
			{
				GoTo(line.Location.File, line.Location.Line);
				OutputTextBox.SelectionLength = 0;
			}
		}

		public void LogOutput(LogData logData)
		{
			_log.Add(logData);
			RefreshOutput(true);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0) ScrollAmount(-1);
			if (e.Delta < 0) ScrollAmount(1);
		}

		private void ScrollAmount(int amount)
		{
			vScrollBar.Value = Math.Max(0, Math.Min(vScrollBar.Maximum, vScrollBar.Value + amount));
			RefreshOutput();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			hScrollBar.MaximumSize = new Size(ClientSize.Width - 16, Int32.MaxValue);
			vScrollBar.MaximumSize = new Size(Int32.MaxValue, ClientSize.Height - 16);

			RefreshOutput();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.C))
			{
				OutputTextBox.Copy();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private const string _rtfHeader = @"{{\rtf1\utf8\deff0
{{\colortbl;{0};\red255\green0\blue0;\red255\green100\blue0;}}
\b0\cf1";
		private void RefreshOutput(bool scrollDown = false) {

			SuspendLayout();
			//OutputTextBox.SuspendDraw();

			var defaultColor = string.Format(@"\red{0}\green{1}\blue{2}", ForeColor.R, ForeColor.G, ForeColor.B);

			_logString.Clear();
			_logString.Append(string.Format(_rtfHeader, defaultColor));

			var lines = (OutputTextBox.Height - vScrollBar.Width) / _lineHeight;

			vScrollBar.Maximum = Math.Max(0, _log.Count - lines);
			if (scrollDown)
			{
				vScrollBar.Value = vScrollBar.Maximum;
			}
			var topLine = vScrollBar.Value;

			for (var i = topLine; i < _log.Count; i++)
			{
				var logData = _log[i];
				string newText;
				var escapedString = string.Join(@" \line ", logData.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape));
				switch (logData.Type)
				{
					case LogType.Headline:
						newText = @"\b " + escapedString + @"\b0 \line ";
						break;
					case LogType.Error:
						newText = @"\cf2 " + escapedString + @"\cf1 \line ";
						break;
					case LogType.Warning:
						newText = @"\cf3 " + escapedString + @"\cf1 \line ";
						break;
					default:
						newText = escapedString + @"\line ";
						break;
				}

				if (logData.Location != null)
				{
					//newText = @"{\field{\*\fldinst{HYPERLINK ""File.src;100""}}{\fldrslt{\ul\cf1This is a Google URL}}}";
				}
				_logString.Append(newText);
			}

			OutputTextBox.Rtf = _logString.ToString();
			//OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
			//OutputTextBox.ScrollToCaret();

			//OutputTextBox.ResumeDraw();
			ResumeLayout();
		}

		private RichTextBoxImproved OutputTextBox;

		private void InitializeComponent()
		{
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.OutputTextBox = new Brewmaster.Modules.Build.RichTextBoxImproved();
			this.SuspendLayout();
			// 
			// hScrollBar
			// 
			this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hScrollBar.Location = new System.Drawing.Point(0, 0);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(80, 17);
			this.hScrollBar.TabIndex = 0;
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(0, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 80);
			this.vScrollBar.TabIndex = 0;
			// 
			// OutputTextBox
			// 
			this.OutputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OutputTextBox.HideSelection = false;
			this.OutputTextBox.Location = new System.Drawing.Point(0, 20);
			this.OutputTextBox.Name = "OutputTextBox";
			this.OutputTextBox.ReadOnly = true;
			this.OutputTextBox.Size = new System.Drawing.Size(90, 113);
			this.OutputTextBox.TabIndex = 1;
			this.OutputTextBox.Text = "";
			this.OutputTextBox.WordWrap = false;
			this.ResumeLayout(false);

		}
	}

	public class RichTextBoxImproved : RichTextBox
	{
		private bool _draw = true;
		public void SuspendDraw()
		{
			_draw = false;
		}

		public void ResumeDraw()
		{
			_draw = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_draw) base.OnPaint(e);
		}
	}
}
