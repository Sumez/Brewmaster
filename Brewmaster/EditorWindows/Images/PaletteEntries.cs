using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BrewMaster.Ide;
using BrewMaster.ProjectExplorer;

namespace BrewMaster.EditorWindows.Images
{
	public class PaletteEntries : Panel
	{
		private List<Color?> _palette;
		private int _maxEntries;
		private Dictionary<Control, int> _entryIndexes;
		public Action OrderUpdated;

		public bool EnableEdit { get; set; }
		public event Action<Color?[]> MouseOverColor;

		public PaletteEntries()
		{
			DoubleBuffered = true;
		}

		public void SetEntries(List<Color?> palette, int maxEntries)
		{
			_maxEntries = maxEntries;
			_palette = palette;

			SuspendLayout();
			Visible = false;
			Controls.Clear();

			if (_entryIndexes != null) foreach (var entry in _entryIndexes.Keys) entry.Dispose();

			_entryIndexes = new Dictionary<Control, int>();
			for (var i = 0; i < palette.Count; i++)
			{
				var entry = new PaletteEntry(palette[i], i >= maxEntries)
				{
					Location = GetEntryPosition(i)
				};
				//if (i >= maxEntries)
				Controls.Add(entry);
				_entryIndexes.Add(entry, i);

				var paletteColor = palette[i];
				entry.MouseDown += (s, e) => { if (MouseOverColor != null) MouseOverColor(new [] { paletteColor }); };
				entry.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) BeginDrag(entry); };
			}

			if (MouseOverColor != null) MouseOverColor(palette.ToArray());

			Visible = true;
			ResumeLayout();
			Refresh();

			var mouseEvents = new OsFeatures.GlobalMouseHandler();
			Application.AddMessageFilter(mouseEvents);
			mouseEvents.MouseMoved += MovedMouse;
			mouseEvents.MouseUp += StopDragging;
			ControlRemoved += (s, a) => Application.RemoveMessageFilter(mouseEvents);
		}

		private int GetEntryIndex(Point location)
		{
			var index = Math.Min(15, Math.Max(0, location.X / 17));
			index += Math.Min(_maxEntries / 16, Math.Max(0, (location.Y - 13) / 30)) * 16;
			index = Math.Min(index, _palette.Count - 1);

			return index;
		}

		private Point GetEntryPosition(int index)
		{
			return new Point(17 * (index % 16), 30 * (index / 16) + 13);
		}

		private bool StopDragging(Point location)
		{
			if (_dragging == null) return false;

			var originalIndex = _entryIndexes[_dragging];
			_dragging.Location = GetEntryPosition(_dragIndex);

			_palette[_dragIndex] = _dragging.PaletteColor;
			foreach (Control entryControl in Controls)
			{
				if (entryControl == _dragging) continue;

				var entryIndex = _entryIndexes[entryControl];
				if (entryIndex >= _dragIndex && entryIndex < originalIndex) entryIndex++;
				else if (entryIndex > originalIndex && entryIndex <= _dragIndex) entryIndex--;
				_palette[entryIndex] = ((PaletteEntry)entryControl).PaletteColor;
			}

			_dragging = null;
			SetEntries(_palette, _maxEntries); // Resetting controls to prevent weird winforms issue where a mousedown event will fire on a control you aren't hovering
			if (OrderUpdated != null) OrderUpdated();
			return true;
		}

		private bool MovedMouse(Point location)
		{
			if (_dragging == null) return false;

			_dragging.Left += location.X - _dragOffset.X;
			_dragging.Top += location.Y - _dragOffset.Y;
			_dragOffset = location;

			var currentIndex = GetEntryIndex(PointToClient(location));
			var originalIndex = _entryIndexes[_dragging];
			foreach (Control entryControl in Controls)
			{
				if (entryControl == _dragging) continue;

				var entryIndex = _entryIndexes[entryControl];
				if (entryIndex >= currentIndex && entryIndex < originalIndex) entryIndex++;
				else if (entryIndex > originalIndex && entryIndex <= currentIndex) entryIndex--;
				entryControl.Location = GetEntryPosition(entryIndex);

			}

			_dragIndex = currentIndex;
			return true;
		}



		private PaletteEntry _dragging = null;
		private Point _dragOffset;
		private int _dragIndex;
		private void BeginDrag(PaletteEntry entry)
		{
			if (!entry.PaletteColor.HasValue || !EnableEdit) return;

			_dragging = entry;
			_dragOffset = Cursor.Position;
			_dragIndex = _entryIndexes[entry];
			Controls.SetChildIndex(_dragging, 0);

			Debug.WriteLine("start drag");
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			using (var labelFont = new Font(Font.FontFamily, 7))
			using (var labelBrush = new SolidBrush(SystemColors.ControlText))
			for (var i = 0; i <= (_maxEntries-1) / 16; i++)
			{
				for (var j = i * 16; j < Math.Min(i * 16 + 16, _maxEntries); j++)
				{
					e.Graphics.DrawString(Convert.ToString(j, 16).ToUpper().PadLeft(2, '0'), labelFont, labelBrush, 17 * (j % 16), 30 * i);
				}
			}
		}
	}

	public class PaletteEntry : Control
	{
		public Color? PaletteColor { get; private set; }
		private readonly bool _dimmed;

		public PaletteEntry(Color? color, bool dimmed)
		{
			PaletteColor = color;
			_dimmed = dimmed;
			Width = 16;
			Height = 16;
			Cursor = Cursors.Hand;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (PaletteColor.HasValue && PaletteColor.Value.A == 0)
			{
				e.Graphics.FillRectangle(SystemBrushes.Control, new Rectangle(0, 0, Width, Height));
				e.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(0, 0, Width / 2, Height / 2));
				e.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(Width / 2, Height / 2, Width / 2, Height / 2));
			}
			else
			{
				using (var brush = new SolidBrush(PaletteColor ?? SystemColors.Control))
				{
					if (_dimmed && PaletteColor.HasValue) brush.Color = Color.FromArgb(PaletteColor.Value.A / 2, PaletteColor.Value);
					e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
				}
			}

			using (var pen = new Pen(Color.Black, 1))
			{
				if (_dimmed)
				{
					pen.DashStyle = DashStyle.Dot;
					pen.Color = Color.Gray;
				}
				e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
			}

			if (!PaletteColor.HasValue)
			{
				e.Graphics.DrawLines(Pens.Black, new []
				{
					new PointF(0, 0), new PointF(Width, Height),
					new PointF(Width, 0), new PointF(0, Height),
				});
			}
		}
	}
}
