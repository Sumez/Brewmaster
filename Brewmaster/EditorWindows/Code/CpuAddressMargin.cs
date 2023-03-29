using System;
using System.Drawing;
using Brewmaster.Modules.Watch;
using Brewmaster.ProjectModel;
using ICSharpCode.TextEditor;

namespace Brewmaster.EditorWindows.Code
{
	public class CpuAddressMargin : AbstractMargin
	{
		private readonly Func<int, DebugLine> _getDebugLine;
		private readonly int _addressWidth;

		public CpuAddressMargin(TextArea textArea, Func<int, DebugLine> getDebugLine, int addressWidth) : base(textArea)
		{
			_getDebugLine = getDebugLine;
			_addressWidth = addressWidth;
		}

		public override bool IsVisible
		{
			get
			{
				if (textArea.MotherTextEditorControl is Ca65Editor editor) return editor.ShowCpuAddresses;
				return false;
			}
		}
		public override Size Size
		{
			get { return new Size(textArea.TextView.WideSpaceWidth * (_addressWidth + 3), -1); }
		}

		public override void Paint(Graphics graphics, Rectangle bounds)
		{
			var lineAddressColor = textArea.Document.HighlightingStrategy.GetColorFor("CPU Addresses");
			graphics.FillRectangle(BrushRegistry.GetBrush(lineAddressColor.BackgroundColor), bounds);

			var font = lineAddressColor.GetFont(TextEditorProperties.FontContainer);
			var brush = BrushRegistry.GetBrush(lineAddressColor.Color);

			for (var y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / textArea.TextView.FontHeight + 1; ++y)
			{
				var backgroundRectangle = new Rectangle(drawingPosition.X,
					2 + drawingPosition.Y + textArea.TextView.FontHeight * y - textArea.TextView.VisibleLineDrawingRemainder,
					drawingPosition.Width + 10,
					textArea.TextView.FontHeight);

				if (!bounds.IntersectsWith(backgroundRectangle)) continue;
			
				var currentLine = textArea.Document.GetFirstLogicalLine(textArea.TextView.FirstPhysicalLine + y);
				if (currentLine < textArea.Document.TotalNumberOfLines)
				{
					var debugLine = _getDebugLine(currentLine);
					if (debugLine == null || !debugLine.CpuAddress.HasValue) continue;
					graphics.DrawString(WatchValue.FormatHexAddress(debugLine.CpuAddress.Value, _addressWidth, 4), font, brush, backgroundRectangle);
				}
			}
		}
	}
}