using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Modules.Watch;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class MetaValuePalette : Control
	{
		private TileMap _map;
		private int _selectedValue;
		private SolidBrush _selectedBrush;

		public MetaValuePalette(TileMap map)
		{
			_map = map;
			Width = 140;
			DoubleBuffered = true;
			SelectedValue = 0;
		}

		public int SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				_selectedValue = value;
				var color = _map.GetMetaValueColor(_selectedValue);
				if (_selectedBrush != null) _selectedBrush.Dispose();
				_selectedBrush = new SolidBrush(color.A == 0 ? color : Color.FromArgb(color.R, color.G, color.B));
				Invalidate();
			}
		}

		public event Action UserSelectedValue;

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			var bit = 0;
			var location = PointToClient(Cursor.Position);
			foreach (var rectangle in GetBitButtons())
			{
				if (rectangle.Contains(location))
				{
					SelectedValue ^= 1 << bit;
					if (UserSelectedValue != null) UserSelectedValue();
					Invalidate();
					return;
				}
				bit++;
			}
		}

		private List<Rectangle> GetBitButtons()
		{
			var locations = new List<Rectangle>();
			for (var i = 0; i < 8; i++) locations.Add(new Rectangle(Width - 11 - i * 12, 3, 10, 15));
			return locations;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			using (var pen = new Pen(Color.DimGray))
			using (var brush = new SolidBrush(Color.DimGray))
			{
				var bit = 0;
				foreach (var rectangle in GetBitButtons())
				{
					if (((SelectedValue >> bit) & 1) == 1) e.Graphics.FillRectangle(brush, rectangle);
					e.Graphics.DrawRectangle(pen, rectangle);
					bit++;
				}
				e.Graphics.DrawString(WatchValue.FormatHex(SelectedValue, 2), Font, brush, Width - 120, 4);

				e.Graphics.FillRectangle(_selectedBrush, Width - 140, 6, 10, 10);
				e.Graphics.DrawRectangle(pen, Width - 140, 6, 10, 10);
			}
		}
	}
}
