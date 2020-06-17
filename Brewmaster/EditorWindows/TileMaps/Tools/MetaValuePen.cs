using System;
using System.Drawing;

namespace Brewmaster.EditorWindows.TileMaps.Tools
{
	public class MetaValuePen : MapEditorTool
	{
		public MetaValuePen(Size metaValueSize, Func<int, Color> getColor)
		{
			_getColor = getColor;
			Size = metaValueSize;
			SelectedValue = 0;
		}

		public event Action SelectedValueChanged;
		private readonly Func<int, Color> _getColor;
		private int _selectedValue;

		public int SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				if (_selectedValue == value) return;
				_selectedValue = value;
				if (Brush != null) Brush.Dispose();
				Brush = new SolidBrush(_getColor(_selectedValue));
				if (SelectedValueChanged != null) SelectedValueChanged();
			}
		}

		public override void Paint(int x, int y, TileMapScreen screen)
		{
			screen.SetMetaValue(x, y, SelectedValue);
		}

		public override void EyeDrop(int x, int y, TileMapScreen screen)
		{
			SelectedValue = screen.GetMetaValue(x, y);
		}
	}
}