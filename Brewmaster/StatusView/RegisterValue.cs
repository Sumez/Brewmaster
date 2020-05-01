using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Brewmaster.StatusView
{
	public class RegisterValue : Control
	{
		public event EventHandler ValueChangedByUser;
		private RegisterSize _registerSize = RegisterSize.EightBit;
		private int _digits = 2;
		public bool ReadOnly { get; set; }
		public RegisterSize RegisterSize
		{
			get { return _registerSize; }
			set
			{
				_registerSize = value;
				switch (_registerSize)
				{
					case RegisterSize.EightBit:
						Width = 20;
						_digits = 2;
						break;
					case RegisterSize.SixteenBit:
						Width = 34;
						_digits = 4;
						break;
					case RegisterSize.TwentyfourBit:
						Width = 55;
						_digits = 6;
						break;
				}
				Value = Value;
			}
		}
		private string _text;
		private int _value;
		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				var text = Convert.ToString(Value, 16).ToUpper().PadLeft(_digits, '0');
				text = text.Substring(text.Length - _digits);
				if (_registerSize == RegisterSize.TwentyfourBit) text = string.Format("{0}:{1}", text.Substring(0, 2), text.Substring(2, 4));
				_text = text;
				Invalidate();
			}
		}
		public override string Text
		{
			get { return _text; }
		}
		private bool _mouseOver = false;
		protected override void OnMouseEnter(EventArgs e)
		{
			_mouseOver = true;
			base.OnMouseEnter(e);
			Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			_mouseOver = false;
			base.OnMouseLeave(e);
			Invalidate();
		}
		private int _editIndex = -1;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			_editIndex = Math.Max(0, Math.Min(_digits - 1, (e.X - LeftPadding) / CharWidth));
			Focus();
			Invalidate();
		}
		protected override void OnGotFocus(EventArgs e)
		{
			if (_editIndex < 0 || _editIndex >= _digits) _editIndex = 0;
			base.OnGotFocus(e);
			Invalidate();
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}

		public RegisterValue()
		{
			Font = new Font("Consolas", 12, FontStyle.Regular, GraphicsUnit.Pixel);
			Cursor = Cursors.IBeam;
			TabStop = true;
			RegisterSize = RegisterSize.EightBit;
			Height = 20;
		}
		private static int CharWidth = 7;
		private static int LeftPadding = 3;
		protected override void OnPaint(PaintEventArgs e)
		{
			var border = new Rectangle(0, 0, Width - 1, Height - 1);
			e.Graphics.FillRectangle(Brushes.White, border);
			if (!ReadOnly && Focused && _editIndex >= 0 && _editIndex < _digits)
				e.Graphics.FillRectangle(Brushes.LightSteelBlue, new Rectangle((CharWidth * _editIndex) + LeftPadding, 2, CharWidth, Height - 4));
			TextRenderer.DrawText(e.Graphics, Text, Font, new Point(0, 3), ForeColor);

			e.Graphics.DrawRectangle(_mouseOver || Focused ? Pens.Gray : Pens.LightGray, border);
		}


		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			if (!ReadOnly && _editIndex >= 0) e.IsInputKey = TestInputKey(e.KeyCode);
			base.OnPreviewKeyDown(e);
		}
		private bool TestInputKey(Keys key)
		{
			if (key == Keys.Escape || key == Keys.Home || key == Keys.End || key == Keys.Left || key == Keys.Right) return true;
			var character = new KeysConverter().ConvertToString(key).ToUpperInvariant().Replace("NUMPAD", "");
			if (Regex.IsMatch(character, "^[0-9A-F]$")) return true;

			return false;
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (_editIndex < 0 || ReadOnly) return;

			switch (e.KeyCode)
			{
				case Keys.Escape:
					_editIndex = -1;
					Invalidate();
					break;
				case Keys.Home:
					_editIndex = 0;
					Invalidate();
					break;
				case Keys.End:
					_editIndex = _digits - 1;
					Invalidate();
					break;
				case Keys.Left:
					_editIndex = Math.Max(0, _editIndex - 1);
					Invalidate();
					break;
				case Keys.Right:
					_editIndex = Math.Min(_digits - 1, _editIndex + 1);
					Invalidate();
					break;
				default:
					var character = new KeysConverter().ConvertToString(e.KeyValue).ToUpperInvariant();
					TypeCharacter(character);
					break;
			}
		}
		private void TypeCharacter(string character)
		{
			if (_editIndex >= _digits) return;
			character = character.Replace("NUMPAD", "");
			if (!Regex.IsMatch(character, "^[0-9A-F]$")) return;
			var mask = (0xF << ((_digits - 1 - _editIndex) * 4)) ^ 0xFFFFFF;
			var setValue = int.Parse(character, System.Globalization.NumberStyles.HexNumber) << ((_digits - 1 - _editIndex) * 4);
			Value = Value & mask | setValue;
			if (ValueChangedByUser != null) ValueChangedByUser(this, new EventArgs());
			_editIndex++;
			Invalidate();
		}
	}
	public enum RegisterSize
	{
		EightBit, SixteenBit, TwentyfourBit
	}
}
