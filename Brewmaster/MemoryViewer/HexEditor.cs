using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Brewmaster.Modules.Watch;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.MemoryViewer
{
	public partial class HexEditor : Control
	{
		public VScrollBar VScrollBar { get; set; }
		public HScrollBar HScrollBar { get; set; }

		private int Rows { get; set; }
		public Action<int, byte> DataChanged;
		public event Action<string, bool> AddWatch;
		public event Action<int, Breakpoint.Types> AddBreakpoint;
		public event Action<IEnumerable<Breakpoint>> RemoveBreakpoints;

		public HexEditor()
		{
			Visible = false;
			SuspendLayout();

			AddressDetails = new ToolTip();
			AddressDetails.AutoPopDelay = 5000;
			AddressDetails.InitialDelay = 0;
			AddressDetails.ReshowDelay = 100;
			AddressDetails.ShowAlways = true;
			AddressDetails.SetToolTip(this, "");

			contextMenu = new ContextMenuStrip();
			contextMenu.Items.AddRange(new ToolStripItem[] {
				watchByteMenuItem = new ToolStripMenuItem("Add To Watch (Byte)"),
				watchWordMenuItem = new ToolStripMenuItem("Add To Watch (Word)"),
				new ToolStripSeparator(),
				breakWriteMenuItem = new ToolStripMenuItem("Break On Write"),
				breakReadMenuItem = new ToolStripMenuItem("Break On Read")
			});

			BackColor = SystemColors.Control;
			Font = new Font("Consolas", 10, FontStyle.Regular);
			Margin = Padding.Empty;

			bankSelector = new SimpleComboBox();
			bankSelector.Location = new Point(6, 0);
			bankSelector.Width = 50;
			bankSelector.Font = new Font(Font.FontFamily, 8);
			bankSelector.SelectedIndexChanged += (s, e) => ChangeSelectedBank();
			bankSelector.Visible = false;
			Controls.Add(bankSelector);

			typeof(Panel).InvokeMember("DoubleBuffered",
				BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
				null, this, new object[] { true });

			ResumeLayout(false);
			Visible = true;

			Program.BindKey(Feature.AddToWatch, (keys) => watchByteMenuItem.ShortcutKeys = keys);
			watchByteMenuItem.Click += (s, e) =>
			{
				if (AddWatch != null) AddWatch("$" + Convert.ToString(_menuAddress, 16).ToUpper().PadLeft(4, '0'), false);
			};
			watchWordMenuItem.Click += (s, e) =>
			{
				if (AddWatch != null) AddWatch("$" + Convert.ToString(_menuAddress, 16).ToUpper().PadLeft(4, '0'), true);
			};
			breakWriteMenuItem.Click += (s, e) =>
			{
				ToggleBreakpoint(Breakpoint.Types.Write, breakWriteMenuItem.Checked);
			};
			breakReadMenuItem.Click += (s, e) =>
			{
				ToggleBreakpoint(Breakpoint.Types.Read, breakReadMenuItem.Checked);
			};

			InitDrawing();
		}

		private void ToggleBreakpoint(Breakpoint.Types type, bool disable)
		{
			if (disable)
			{
				// Only alter breakpoints with no end address
				var breakpoints = _breakpoints.Where(bp => bp.Type.HasFlag(type) && bp.StartAddress == _menuAddress && bp.EndAddress == null).ToList();
				foreach (var breakpoint in breakpoints) breakpoint.Type ^= type;
				if (RemoveBreakpoints != null) RemoveBreakpoints(breakpoints.Where(bp => bp.Type == 0));
			}
			else if (AddBreakpoint != null) AddBreakpoint(_menuAddress, type);
		}
		public void SetBreakpoints(IEnumerable<Breakpoint> breakpoints)
		{
			_breakpoints = breakpoints.Where(bp => (bp.Type & (Breakpoint.Types.Write | Breakpoint.Types.Read)) != 0);
			Invalidate();
		}


		public void SetAddressRange(int bytes)
		{
			if (bytes > 0x10000)
			{
				var banks = bytes / 0x10000;
				bytes = 0x10000;
				while (bankSelector.Items.Count > banks) bankSelector.Items.RemoveAt(banks);
				while (bankSelector.Items.Count < banks) bankSelector.Items.Add(Convert.ToString(bankSelector.Items.Count, 16).PadLeft(2, '0').ToUpper());
				_banked = bankSelector.Visible = true;
				if (bankSelector.SelectedIndex < 0) bankSelector.SelectedIndex = 0;
				if (_currentBank >= banks) bankSelector.SelectedIndex = banks - 1;
			}
			else
			{
				_banked = bankSelector.Visible = false;
				_currentBank = 0;
			}

			Rows = (int)Math.Ceiling((double)bytes / 16);
			VScrollBar.Maximum = Rows;
		}

		public void SetData(byte[] bytes)
		{
			SetAddressRange(bytes.Length);
			_data = bytes;
			Invalidate();
		}

		private void ChangeSelectedBank()
		{
			_currentBank = bankSelector.SelectedIndex;
			Focus();
			Invalidate();
		}

		private byte[] _data = new byte[0];
		private int _currentBank;
		private bool _banked;
		private ToolTip AddressDetails;
		private int _focusAddress = -1;
		private int _editAddress = -1;
		private int _menuAddress = -1;
		private int _editIndex = 0;
		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem watchByteMenuItem;
		private ToolStripMenuItem watchWordMenuItem;
		private ToolStripMenuItem breakWriteMenuItem;
		private ToolStripMenuItem breakReadMenuItem;
		private ComboBox bankSelector;
		private Point _lastLocation;
		private IEnumerable<Breakpoint> _breakpoints = new List<Breakpoint>();

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_lastLocation == e.Location) return;
			var address = GetAddressFromPoint(e.Location);
			if (_focusAddress != address) Invalidate();
			_focusAddress = address;
			_lastLocation = e.Location;
			if (_focusAddress == -1)
			{
				AddressDetails.Active = false;
				return;
			}
			AddressDetails.Active = true;
			var addressHex = WatchValue.FormatHexAddress(_focusAddress);
			var value = _focusAddress >= _data.Length ? 0 : _data[_focusAddress];
			var valueHex = Convert.ToString(value, 16).ToUpper().PadLeft(2, '0');
			var wordValue = _focusAddress + 1 >= _data.Length ? 0 : (_data[_focusAddress + 1] * 256);
			wordValue += value;
			var wordValueHex = Convert.ToString(wordValue, 16).ToUpper().PadLeft(4, '0');

			var text = string.Format("Address: {0}\nValue: ${1} ({2})\nWord value: ${3} ({4})", addressHex, valueHex, value, wordValueHex, wordValue);

			AddressDetails.Show(text, this, e.Location.X + 20, e.Location.Y + 2);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_focusAddress = -1;
			AddressDetails.Active = false;
		}
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Left)
			{
				_editAddress = _focusAddress;
				_editIndex = 0;
				Focus();
				Invalidate();
				return;
			}
			if (_focusAddress >= 0 && _focusAddress < _data.Length && e.Button == MouseButtons.Right)
			{
				_menuAddress = _focusAddress;
				watchByteMenuItem.Enabled = watchWordMenuItem.Enabled = AddWatch != null;
				breakReadMenuItem.Enabled = breakWriteMenuItem.Enabled = AddBreakpoint != null;
				breakReadMenuItem.Checked = _breakpoints.Any(bp => bp.Type.HasFlag(Breakpoint.Types.Read) && bp.StartAddress == _menuAddress && bp.EndAddress == null);
				breakWriteMenuItem.Checked = _breakpoints.Any(bp => bp.Type.HasFlag(Breakpoint.Types.Write) && bp.StartAddress == _menuAddress && bp.EndAddress == null);
				contextMenu.Show(PointToScreen(e.Location));
			}
		}
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			if (_editAddress >= 0) e.IsInputKey = true;
			base.OnPreviewKeyDown(e);
		}
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (_editAddress < 0) return;

			switch (e.KeyCode)
			{
				case Keys.Escape:
					_editAddress = -1;
					Invalidate();
					break;
				case Keys.Left:
					MoveIndex(-1);
					Invalidate();
					break;
				case Keys.Right:
					MoveIndex(1);
					Invalidate();
					break;
				case Keys.Up:
					MoveIndex(-0x20);
					Invalidate();
					break;
				case Keys.Down:
					MoveIndex(0x20);
					Invalidate();
					break;
				case Keys.PageUp:
					MoveIndex(-0x200);
					Invalidate();
					break;
				case Keys.PageDown:
					MoveIndex(0x200);
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
			if (_editAddress >= _data.Length) return;
			character = character.Replace("NUMPAD", "");
			if (!Regex.IsMatch(character, "^[0-9A-F]$")) return;
			var mask = (byte)(0xF << _editIndex * 4);
			var setValue = (byte)(byte.Parse(character, System.Globalization.NumberStyles.HexNumber) << (1 - _editIndex) * 4);
			_data[_editAddress] &= mask;
			_data[_editAddress] |= setValue;
			if (DataChanged != null) DataChanged(_editAddress, _data[_editAddress]);
			MoveIndex(1);
			Invalidate();
		}
		private void MoveIndex(int offset)
		{
			var prevA = _editAddress;
			var prevI = _editIndex;
			_editIndex += offset;
			if (_editIndex > 1 || _editIndex < 1)
			{
				_editAddress += (int)Math.Floor(_editIndex / 2f);
				_editIndex &= 1;
			}
			if (_editAddress < 0 || _editAddress >= _data.Length || (prevA & 0xff0000) != (_editAddress & 0xff0000))
			{
				_editAddress = prevA;
				_editIndex = prevI;
			}
			ScrollToAddress(_editAddress);
		}
		private void ScrollToAddress(int address)
		{
			address &= 0xFFFF;
			var firstVisibleRow = VScrollBar.Value;
			var visibleRows = (int)Math.Round(ClientRectangle.Height / 20f) - 2;
			var targetRow = address / 0x10;

			if (targetRow > firstVisibleRow + visibleRows)
			{
				ScrollViewer(targetRow - firstVisibleRow - visibleRows);
			}
			if (targetRow < firstVisibleRow)
			{
				ScrollViewer(targetRow - firstVisibleRow);
			}
		}

		private int GetAddressFromPoint(Point location)
		{
			if (location.X < _leftMargin) return -1;
			var y = location.Y - TopMargin;
			var x = location.X - _xOffset;
			if (x < 0 || x >= 16 * HDistance) return -1;
			if (y < 0) return -1;

			var address = ((y / VDistance) * 16) + (x / HDistance) + (VScrollBar.Value * 16) + (_currentBank * 0x10000);
			return (address >= _data.Length) ? -1 : address;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0) ScrollViewer(-1);
			if (e.Delta < 0) ScrollViewer(1);
		}

		private void ScrollViewer(int amount)
		{
			VScrollBar.Value = Math.Max(0, Math.Min(VScrollBar.Maximum, VScrollBar.Value + amount));
			Invalidate();
		}

		private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			Invalidate();
		}
	}

	public class SimpleComboBox : ComboBox
	{
		public SimpleComboBox()
		{
			DropDownStyle = ComboBoxStyle.DropDownList;
			FlatStyle = FlatStyle.Flat;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}
	}

}
