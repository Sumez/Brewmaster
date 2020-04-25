using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules.NumberHelper
{
	public partial class NumberHelper: UserControl
	{
		public static char[] BinaryChars = { '0', '1' };
		public static char[] HexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
		public static char[] DecimalChars = { '0', '1', '3', '4', '5', '6', '7', '8', '9' };

		public NumberHelper()
		{
			InitializeComponent();

			BinaryNumber.GotFocus += UpdateFocus;
			HexNumber.GotFocus += UpdateFocus;
			DecimalNumber.GotFocus += UpdateFocus;
			LostFocus += UpdateFocus;

			BinaryNumber.KeyPress += (sender, args) =>
			{
				if (!char.IsControl(args.KeyChar) && !BinaryChars.Contains(args.KeyChar)) args.Handled = true;
			};
			BinaryNumber.KeyDown += (sender, args) =>
			{
				if (args.KeyCode == Keys.Up)
				{
					args.Handled = true;
					HexNumber.Focus();
					HexNumber.SelectAll();
				}
			};
			HexNumber.KeyPress += (sender, args) =>
			{
				if (!char.IsControl(args.KeyChar) && !HexChars.Contains(char.ToUpperInvariant(args.KeyChar))) args.Handled = true;
				args.KeyChar = char.ToUpperInvariant(args.KeyChar);
			};
			HexNumber.KeyDown += (sender, args) =>
			{
				if (args.KeyCode == Keys.Up)
				{
					args.Handled = true;
					DecimalNumber.Focus();
					DecimalNumber.SelectAll();
				}
				else if (args.KeyCode == Keys.Down)
				{
					args.Handled = true;
					BinaryNumber.Focus();
					BinaryNumber.SelectAll();
				}
			};
			DecimalNumber.KeyPress += (sender, args) =>
			{
				if (!char.IsControl(args.KeyChar) && !DecimalChars.Contains(args.KeyChar)) args.Handled = true;
			};
			DecimalNumber.KeyDown += (sender, args) =>
			{
				if (args.KeyCode == Keys.Down)
				{
					args.Handled = true;
					HexNumber.Focus();
					HexNumber.SelectAll();
				}
			};

			DecimalNumber.TextChanged += (sender, args) =>
			{
				if (Regex.IsMatch(DecimalNumber.Text, @"[^0-9]")) DecimalNumber.Text = Regex.Replace(DecimalNumber.Text, @"[^0-9]", "");
				var number = string.IsNullOrEmpty(DecimalNumber.Text) ? 0 : Convert.ToInt32(DecimalNumber.Text);

				SetHex(number);
				SetBinary(number);
			};
			HexNumber.TextChanged += (sender, args) =>
			{
				if (Regex.IsMatch(HexNumber.Text, @"[^0-9A-F]")) HexNumber.Text = Regex.Replace(HexNumber.Text, @"[^0-9A-F]", "");
				var number = string.IsNullOrEmpty(HexNumber.Text) ? 0 : Convert.ToInt32(HexNumber.Text, 16);

				SetDecimal(number);
				SetBinary(number);
			};
			BinaryNumber.TextChanged += (sender, args) =>
			{
				if (Regex.IsMatch(BinaryNumber.Text, @"[^01]")) BinaryNumber.Text = Regex.Replace(BinaryNumber.Text, @"[^01]", "");
				var number = string.IsNullOrEmpty(BinaryNumber.Text) ? 0 : Convert.ToInt32(BinaryNumber.Text, 2);

				SetHex(number);
				SetDecimal(number);
			};
		}

		private void SetDecimal(int number)
		{
			var newValue = Convert.ToString(number);
			if (!DecimalNumber.Focused && newValue != DecimalNumber.Text) DecimalNumber.Text = newValue;
		}

		private void SetBinary(int number)
		{
			var newValue = Convert.ToString(number, 2);
			newValue = new string('0', newValue.Length % 8) + newValue;
			if (!BinaryNumber.Focused && newValue != BinaryNumber.Text) BinaryNumber.Text = newValue;
		}

		private void SetHex(int number)
		{
			var newValue = Convert.ToString(number, 16).ToUpperInvariant();
			newValue = new string('0', newValue.Length % 2) + newValue;
			if (!HexNumber.Focused && newValue != HexNumber.Text) HexNumber.Text = newValue;
		}

		private void UpdateFocus(object sender, EventArgs e)
		{
			BinaryNumber.Font = new Font(Font.FontFamily, Font.Size, BinaryNumber.Focused ? FontStyle.Bold : FontStyle.Regular);
			HexNumber.Font = new Font(Font.FontFamily, Font.Size, HexNumber.Focused ? FontStyle.Bold : FontStyle.Regular);
			DecimalNumber.Font = new Font(Font.FontFamily, Font.Size, DecimalNumber.Focused ? FontStyle.Bold : FontStyle.Regular);
		}
	}
}
