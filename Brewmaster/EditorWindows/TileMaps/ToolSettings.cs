using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Modules.Watch;

namespace Brewmaster.EditorWindows.TileMaps
{
	public class ToolSettings : Control
	{
		public ToolSettings(TileMap map, MapEditorState state)
		{
			AutoSize = true;

			Controls.Add(MetaValues = new MetaValueSettings(map) {Visible = false, Dock = DockStyle.Right});
			Controls.Add(PixelSettings = new PixelPenSettings() { Visible = false, Dock = DockStyle.Right });


			state.ToolChanged += () =>
			{
				if (ActiveControl != null) ActiveControl.Visible = false;
				ActiveControl = null;
				if (state.Tool is MetaValuePen metaPen)
				{
					metaPen.SelectedValue = MetaValues.SelectedValue;
					metaPen.SelectedValueChanged += () =>
					{
						MetaValues.SelectedValue = metaPen.SelectedValue;
					};
					(ActiveControl = MetaValues).Visible = true;
				}
				if (state.Tool is PixelPen pixelPen)
				{
					PixelSettings.SetCurrentTool(pixelPen);
					(ActiveControl = PixelSettings).Visible = true;
				}

				if (ActiveControl != null) Width = ActiveControl.Width;
			};
		}

		public PixelPenSettings PixelSettings { get; set; }
		public Control ActiveControl { get; set; }
		public MetaValueSettings MetaValues { get; set; }
	}

	public class PixelPenSettings : Control
	{
		private Color _color = Color.Black;
		private PixelPen _tool;

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				Invalidate();
			}
		}

		public PixelPenSettings()
		{
			Width = 140;
			Controls.Add(CreateNewTileOption = new CheckBox {Text = "Create new tiles", AutoSize = true, Top = 4});
			CreateNewTileOption.Click += (s, a) =>
			{
				if (_tool == null) return;
				_tool.CreateNewTile = CreateNewTileOption.Checked;
			};
		}

		public CheckBox CreateNewTileOption { get; private set; }

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			using (var brush = new SolidBrush(Color)) e.Graphics.FillRectangle(brush, Width - 20, 3, 16, 16);
			e.Graphics.DrawRectangle(Pens.Black, Width - 20, 3, 16, 16);
		}

		public void SetCurrentTool(PixelPen pixelPen)
		{
			_tool = pixelPen;
			pixelPen.PreviewSourceChanged += UpdatePixelColor;
			pixelPen.SelectedColorChanged += UpdatePixelColor;
			UpdatePixelColor();
			pixelPen.CreateNewTile = CreateNewTileOption.Checked;
		}

		private void UpdatePixelColor()
		{
			Color = _tool.PreviewSource.Colors[_tool.SelectedColor];
		}
	}
	public class MetaValueSettings : Control
	{

		private TileMap _map;
		private int _selectedValue;
		private SolidBrush _selectedBrush;

		public MetaValueSettings(TileMap map)
		{
			_map = map;
			DoubleBuffered = true;
			SelectedValue = 0;
			Width = 140;
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
