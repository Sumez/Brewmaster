using System;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.ProjectModel;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.EditorWindows.Code
{
	public class ErrorMarker : TextMarker
	{
		public ErrorMarker(int offset, int length, string message, Color color) 
			: base(offset, length, TextMarkerType.WaveLine, color)
		{
			ToolTip = message;
		}
	}
	public class BreakpointMarker : Bookmark
	{
		public int BuildLine { get; set; }
		public bool Healthy { get; set; }
		public Breakpoint GlobalBreakpoint { get; set; }

		public event Action OnRemove;
		public BreakpointMarker(IDocument document, int line, int buildLine, bool isEnabled, bool isHealthy, Action onRemove)
			: base(document, new TextLocation(0, line), isEnabled)
		{
			BuildLine = buildLine;
			Healthy = isHealthy;
			OnRemove = onRemove;
		}
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			var brush = Healthy ? Brushes.OrangeRed : Brushes.Gold;
			var diameter = margin.Size.Width / 1.5f;
			if (IsEnabled)
				g.FillEllipse(brush, p.X, p.Y + (diameter / 4), diameter, diameter);
			else
				g.DrawEllipse(new Pen(brush, 1), p.X, p.Y + (diameter / 4), diameter, diameter);
		}

		public override bool Click(Control parent, MouseEventArgs e)
		{
			base.Click(parent, e);
			if (OnRemove != null) OnRemove();
			return true;
		}
	}
	public class BuildLineMarker : Bookmark
	{
		public int BuildLine { get; set; }
		public BuildLineMarker(IDocument document, int line) : base(document, new TextLocation(0, line), true)
		{
			BuildLine = line;
		}
		public override bool Click(Control parent, MouseEventArgs e)
		{
			return false;
		}
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
		}
	}
	public class PcArrow : Bookmark
	{
		public PcArrow(IDocument document, int line) : base(document, new TextLocation(0, line), true)
		{
		}
		public override bool Click(Control parent, MouseEventArgs e)
		{
			return false;
		}
		public override void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			/*var path = new System.Drawing.Drawing2D.GraphicsPath(new []
			{
				new PointF(p.X, p.Y-3),
				new PointF(p.X+4, p.Y-3),
				new PointF(p.X+4, p.Y-6),
				new PointF(p.X+7, p.Y),
				new PointF(p.X+4, p.Y+6),
				new PointF(p.X+4, p.Y+3),
				new PointF(p.X, p.Y+3),
			}, new Byte[] {
				0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20
			});
			g.FillPath(Brushes.White, path);*/
			margin.DrawArrow(g, p.Y);
		}
	}

}