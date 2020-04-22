using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BrewMaster.Modules.Watch;
using BrewMaster.ProjectModel;
using BrewMaster.Settings;

namespace BrewMaster.MemoryViewer
{
	public class MemoryViewer : Panel
	{
		private VScrollBar ScrollBar;
		private int Rows { get; set; }
		public event Action<int, byte> DataChanged;
		public event Action<string, bool> AddWatch;
		public event Action<int, Breakpoint.Types> AddBreakpoint;
		public event Action<IEnumerable<Breakpoint>> RemoveBreakpoints;

		public MemoryViewer()
		{
			InitializeComponent();

			Visible = false;
			SuspendLayout();

			Font = new Font("Consolas", 10, FontStyle.Regular);

			Controls.Add(ScrollBar);

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

			ResumeLayout();
			Visible = true;

			AddressDetails.ShowAlways = true;
			AddressDetails.SetToolTip(this, "");

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
			
			Rows = (int) Math.Ceiling((double) bytes / 16);
			ScrollBar.Maximum = Rows;
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
		private System.ComponentModel.IContainer components;
		private int _focusAddress = -1;
		private int _editAddress = -1;
		private int _menuAddress = -1;
		private int _editIndex = 0;
		private const int Distance = 30;
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

			AddressDetails.Show(text, this, e.Location.X + 20, e.Location.Y);
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
			var firstVisibleRow = ScrollBar.Value;
			var visibleRows = (int)Math.Round(ClientRectangle.Height / 20f) - 2;
			var targetRow = address / 0x10;

			if (targetRow > firstVisibleRow + visibleRows)
			{
				Scroll(targetRow - firstVisibleRow - visibleRows);
			}
			if (targetRow < firstVisibleRow)
			{
				Scroll(targetRow - firstVisibleRow);
			}
		}

		private int GetAddressFromPoint(Point location)
		{
			var y = location.Y - 20;
			var x = location.X - (_banked ? 70 : 50);
			if (x < 0 || x >= 16 * Distance) return -1;
			if (y < 0) return -1;

			var address = ((y / 20) * 16) + (x / Distance) + (ScrollBar.Value * 16) + (_currentBank * 0x10000);
			return (address >= _data.Length) ? -1 : address;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var xOffset = _banked ? 70 : 50;

			var graphics = e.Graphics;
			var headerFont = new Font(Font, FontStyle.Bold);
			var letterSize = graphics.MeasureString("0", headerFont, PointF.Empty, StringFormat.GenericTypographic);
			var letterWidth = letterSize.Width;
			var letterHeight = letterSize.Height;
			for (var i = 0; i < 16; i++)
			{
				var hex = Convert.ToString(i, 16).ToUpper();
				graphics.DrawString(hex, headerFont, Brushes.Black, i * Distance + xOffset + letterWidth, 0);
			}

			var firstVisibleRow = ScrollBar.Value;
			var visibleRows = (ClientRectangle.Height / 20);

			graphics.FillRectangle(Brushes.White, xOffset, 20, Distance * 16, visibleRows * 20);

			var bankHex = (Convert.ToString(_currentBank, 16).ToUpper()).PadLeft(2, '0') + ":";
			for (var i = 0; i < visibleRows; i++)
			{
				var row = firstVisibleRow + i;
				if (row >= Rows) break;

				var rowHex = (Convert.ToString(row, 16).ToUpper()+"0").PadLeft(4, '0');
				graphics.DrawString(rowHex, headerFont, Brushes.Black, letterWidth + (_banked ? letterWidth * 3 : 0), i * 20 + 20);

				if (_banked) graphics.DrawString(bankHex, Font, Brushes.Black, letterWidth, i * 20 + 20);

				for (var j = 0; j < 16; j++)
				{
					var index = row * 16 + j + (_currentBank * 0x10000);
					var hex = (index >= _data.Length) ? "00" : Convert.ToString(_data[index], 16).ToUpper().PadLeft(2, '0');

					if (index == _editAddress)
					{
						var letterBox = new RectangleF(j * Distance + xOffset + letterWidth * _editIndex + 2, i * 20 + 20, letterWidth, letterHeight);
						graphics.FillRectangle(Brushes.LightSteelBlue, letterBox);
						graphics.DrawLine(new Pen(Color.Black, 1), letterBox.Left, letterBox.Bottom, letterBox.Right, letterBox.Bottom);
					}
					else
					{
						Brush bgBrush = null;
						if (_breakpoints.Any(bp => bp.StartAddress == index || bp.StartAddress < index && bp.EndAddress > index))
						{
							bgBrush = Brushes.PaleVioletRed;
							if (index == _focusAddress) bgBrush = Brushes.MediumVioletRed;
						}
						else if (index == _focusAddress) bgBrush = Brushes.LightGray;
						if (bgBrush != null) graphics.FillRectangle(bgBrush, j * Distance + xOffset, i * 20 + 20, letterWidth * 2.5f, letterHeight);
					}
					graphics.DrawString(hex, Font, Brushes.Black, j * Distance + xOffset, i * 20 + 20);
				}
			}

			headerFont.Dispose();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			this.ScrollBar = new System.Windows.Forms.VScrollBar();
			this.AddressDetails = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.watchByteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.watchWordMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.breakWriteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.breakReadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(184, 6);
			// 
			// ScrollBar
			// 
			this.ScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.ScrollBar.Location = new System.Drawing.Point(0, 0);
			this.ScrollBar.Name = "ScrollBar";
			this.ScrollBar.Size = new System.Drawing.Size(17, 80);
			this.ScrollBar.TabIndex = 0;
			this.ScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
			// 
			// AddressDetails
			// 
			this.AddressDetails.AutoPopDelay = 5000;
			this.AddressDetails.InitialDelay = 0;
			this.AddressDetails.ReshowDelay = 100;
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.watchByteMenuItem,
            this.watchWordMenuItem,
            toolStripSeparator1,
            this.breakWriteMenuItem,
            this.breakReadMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(188, 98);
			// 
			// watchByteMenuItem
			// 
			this.watchByteMenuItem.Name = "watchByteMenuItem";
			this.watchByteMenuItem.Size = new System.Drawing.Size(187, 22);
			this.watchByteMenuItem.Text = "Add To Watch (Byte)";
			// 
			// watchWordMenuItem
			// 
			this.watchWordMenuItem.Name = "watchWordMenuItem";
			this.watchWordMenuItem.Size = new System.Drawing.Size(187, 22);
			this.watchWordMenuItem.Text = "Add To Watch (Word)";
			// 
			// breakWriteMenuItem
			// 
			this.breakWriteMenuItem.Name = "breakWriteMenuItem";
			this.breakWriteMenuItem.Size = new System.Drawing.Size(187, 22);
			this.breakWriteMenuItem.Text = "Break On Write";
			// 
			// breakReadMenuItem
			// 
			this.breakReadMenuItem.Name = "breakReadMenuItem";
			this.breakReadMenuItem.Size = new System.Drawing.Size(187, 22);
			this.breakReadMenuItem.Text = "Break On Read";
			// 
			// MemoryViewer
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0) Scroll(-1);
			if (e.Delta < 0) Scroll(1);
		}

		private void Scroll(int amount)
		{
			ScrollBar.Value = Math.Max(0, Math.Min(ScrollBar.Maximum, ScrollBar.Value + amount));
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
