using Brewmaster.Emulation;
using Brewmaster.ProjectModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Brewmaster.Settings;

namespace Brewmaster.Modules.Watch
{
	public class WatchList : ListView
	{
		public Func<string, DebugSymbol> GetSymbol;
		public Action<int, Breakpoint.Types, Breakpoint.AddressTypes, string> AddBreakpoint;

		private ContextMenuStrip contextMenu;
		private IContainer components;
		private ToolStripMenuItem removeWatchMenuItem;
		private ToolStripMenuItem copyMenuItem;
		private ToolStripMenuItem copyValueMenuItem;
		private ToolStripMenuItem wordMenuItem;
		private ToolStripMenuItem decimalMenuItem;
		private ToolStripMenuItem breakOnWriteMenuItem;
		private ToolStripMenuItem breakOnReadMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem clearAllMenuItem;
		protected ListViewItem AddRow = new ListViewItem();

		protected WatchValue SelectedValue
		{
			get { return SelectedItems.Count > 0 ? SelectedItems[0] as WatchValue : null; }
		}
		
		public WatchList()
		{
			InitializeComponent();

			OwnerDraw = true;
			ShowGroups = false;
			Activation = ItemActivation.TwoClick;
			GridLines = true;
			LabelEdit = true;
			View = View.Details;
			Columns.AddRange(new[]
			{
				new ColumnHeader { Text = "Expression" },
				new ColumnHeader { Text = "Value" },
				new ColumnHeader { Text = "W", Width = 25},
				new ColumnHeader { Text = "D", Width = 25}
			});
			Items.Add(AddRow);

			wordMenuItem.Click += (s, a) =>
			{
				SelectedValue.ShowAsWord = !SelectedValue.ShowAsWord;
				Refresh();
			};
			decimalMenuItem.Click += (s, a) =>
			{
				SelectedValue.ShowAsDecimal = !SelectedValue.ShowAsDecimal;
				Refresh();
			};
			Program.BindKey(Feature.RemoveFromList, (keys) => removeWatchMenuItem.ShortcutKeys = keys);
			removeWatchMenuItem.Click += (s, a) => RemoveSelectedItems();
			clearAllMenuItem.Click += (s, a) => RemoveAllItems();
			copyMenuItem.Click += (s, a) => Clipboard.SetText(SelectedValue.Text);
			copyValueMenuItem.Click += (s, a) => Clipboard.SetText(SelectedValue.GetValue());
			breakOnWriteMenuItem.Click += (s, a) => { ParseBreakpoint(SelectedValue, Breakpoint.Types.Write); };
			breakOnReadMenuItem.Click += (s, a) => { ParseBreakpoint(SelectedValue, Breakpoint.Types.Read); };

			contextMenu.Opening += (s, a) =>
			{
				if (SelectedValue != null) {
					contextMenu.Enabled = true;
					wordMenuItem.Checked = SelectedValue.ShowAsWord;
					decimalMenuItem.Checked = SelectedValue.ShowAsDecimal;
				}
				else
				{
					wordMenuItem.Checked = decimalMenuItem.Checked = contextMenu.Enabled = false;
				}
			};
			contextMenu.Closing += (s, a) => contextMenu.Enabled = true;

			ContextMenuStrip = contextMenu;

			var updateTimer = new Timer { Interval = 300 };
			updateTimer.Tick += (sender, args) =>
			{
				if (_needsUpdate) UpdateValues();
			};
			updateTimer.Start();

		}

		private void RemoveAllItems()
		{
			foreach (var watchValue in Items.OfType<WatchValue>()) Items.Remove(watchValue);
		}

		private void RemoveSelectedItems()
		{
			foreach (var watchValue in SelectedItems.OfType<WatchValue>()) Items.Remove(watchValue);
		}

		private void ParseBreakpoint(WatchValue watchValue, Breakpoint.Types type)
		{
			if (AddBreakpoint == null) return;
			var parseExpression = watchValue.Text.Trim();
			var addressReference = WatchValue.ParseNumber(parseExpression);
			AddBreakpoint(addressReference, type, Breakpoint.AddressTypes.Cpu, addressReference < 0 ? parseExpression: null);
		}

		protected override bool DoubleBuffered { get { return true;  } }
		[DefaultValue(true)]
		public new bool OwnerDraw { get => base.OwnerDraw; set => base.OwnerDraw = value; }
		[DefaultValue(false)]
		public new bool ShowGroups { get => base.ShowGroups; set => base.ShowGroups = value; }
		[DefaultValue(true)]
		public new bool LabelEdit { get => base.LabelEdit; set => base.LabelEdit = value; }
		public new ItemActivation Activation { get => base.Activation; set => base.Activation = value; }
		public new View View { get => base.View; set => base.View = value; }
		private static ItemActivation DefaultActivation { get { return ItemActivation.OneClick; } }
		private static View DefaultView { get { return View.Details; } }

		public void AddWatch(string expression, bool showAsWord = false, bool showAsDecimal = false)
		{
			Items.Add(new WatchValue(expression, showAsWord, showAsDecimal, ReadAddress, GetSymbol));
			Items.Remove(AddRow);
			Items.Add(AddRow);
		}

		private MemoryState _memoryState;
		private bool _needsUpdate = false;
		private DateTime _lastUpdated = DateTime.Now;
		private static int _refreshRate = 90; // Keep this high due to list controls slowing down rendering massively

		public void SetData(MemoryState memoryState)
		{
			_memoryState = memoryState;
			var now = DateTime.Now;
			if ((now - _lastUpdated).TotalMilliseconds < _refreshRate)
			{
				_needsUpdate = true;
				return;
			}
			_lastUpdated = now;
			UpdateValues();
		}

		public void UpdateValues()
		{
			_needsUpdate = false;
			BeginUpdate();
			foreach (var watchValue in Items.OfType<WatchValue>())
			{
				// TODO: "Parse" on a new build, and only "update" on data push
				watchValue.Parse();
			}
			EndUpdate();
		}
		private int ReadAddress(int index, bool readWord)
		{
			if (_memoryState == null) return -2;
			return _memoryState == null ? -2 : _memoryState.ReadAddress(index, readWord);
		}

		public void ClearValues()
		{
			Items.Clear();
			Items.Add(AddRow);
		}

		protected override void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);

			if (Items[e.Item] == AddRow && !String.IsNullOrEmpty(e.Label))
			{
				AddWatch(e.Label);
				AddRow.Text = "";
				AddRow.Selected = false;
				return;
			}

			var watchValue = Items[e.Item] as WatchValue;
			if (watchValue == null || e.Label == null) return;

			watchValue.Text = e.Label;
			watchValue.Parse();
			if (e.Label == "") Items.Remove(watchValue);
		}
		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			base.OnDrawColumnHeader(e);
			e.DrawDefault = true;
		}

		protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
		{
			if (e.ColumnIndex >= 2)
			{
				e.NewWidth = 25;
				e.Cancel = true;
			}
			else
			{
				base.OnColumnWidthChanging(e);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			FitWidth();
		}

		private void FitWidth()
		{
			if (Width < 100) return;
			var delta = Width - Columns[0].Width - Columns[1].Width - Columns[2].Width - Columns[3].Width;
			var halfDelta = delta / 2;
			Columns[0].Width += halfDelta;
			Columns[1].Width += delta - halfDelta - 1;
		}

		protected override void OnItemActivate(EventArgs e)
		{
			SelectedItems[0].BeginEdit();
			base.OnItemActivate(e);
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			var location = HitTest(e.Location);
			var watchValue = location.Item as WatchValue;
			if (watchValue == null) return;
			if (SelectedValue != null && SelectedValue != watchValue) SelectedValue.Selected = false;
			watchValue.Selected = true;
			if (e.Button != MouseButtons.Left) return;

			if (location.SubItem == location.Item.SubItems[2]) watchValue.ShowAsWord = !watchValue.ShowAsWord;
			if (location.SubItem == location.Item.SubItems[3]) watchValue.ShowAsDecimal = !watchValue.ShowAsDecimal;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			var location = HitTest(e.Location);
			var watchValue = location.Item as WatchValue;
			if (watchValue == null) return;
			watchValue.Selected = true;
		}

		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			base.OnDrawSubItem(e);
			var watchValue = e.Item as WatchValue;
			if (e.ColumnIndex < 2)
			{
				e.DrawDefault = true;
				return;
			}
			if (watchValue == null) return;
			var isChecked = (e.ColumnIndex == 2 && watchValue.ShowAsWord) || (e.ColumnIndex == 3 && watchValue.ShowAsDecimal);
			CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.X + 3, e.Bounds.Y), isChecked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyValueMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.wordMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.decimalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.breakOnWriteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.breakOnReadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.removeWatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyMenuItem,
            this.copyValueMenuItem,
            toolStripSeparator1,
            this.wordMenuItem,
            this.decimalMenuItem,
            toolStripSeparator2,
            this.breakOnWriteMenuItem,
            this.breakOnReadMenuItem,
            this.toolStripSeparator3,
            this.removeWatchMenuItem,
            this.clearAllMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(190, 198);
			// 
			// copyMenuItem
			// 
			this.copyMenuItem.Name = "copyMenuItem";
			this.copyMenuItem.Size = new System.Drawing.Size(189, 22);
			this.copyMenuItem.Text = "Copy";
			// 
			// copyValueMenuItem
			// 
			this.copyValueMenuItem.Name = "copyValueMenuItem";
			this.copyValueMenuItem.Size = new System.Drawing.Size(189, 22);
			this.copyValueMenuItem.Text = "Copy Value";
			// 
			// wordMenuItem
			// 
			this.wordMenuItem.Name = "wordMenuItem";
			this.wordMenuItem.Size = new System.Drawing.Size(189, 22);
			this.wordMenuItem.Text = "Word (16 Bit) Value";
			// 
			// decimalMenuItem
			// 
			this.decimalMenuItem.Name = "decimalMenuItem";
			this.decimalMenuItem.Size = new System.Drawing.Size(189, 22);
			this.decimalMenuItem.Text = "Display Decimal Value";
			// 
			// breakOnWriteMenuItem
			// 
			this.breakOnWriteMenuItem.Name = "breakOnWriteMenuItem";
			this.breakOnWriteMenuItem.Size = new System.Drawing.Size(189, 22);
			this.breakOnWriteMenuItem.Text = "Break On Write";
			// 
			// breakOnReadMenuItem
			// 
			this.breakOnReadMenuItem.Name = "breakOnReadMenuItem";
			this.breakOnReadMenuItem.Size = new System.Drawing.Size(189, 22);
			this.breakOnReadMenuItem.Text = "Break On Read";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(186, 6);
			// 
			// removeWatchMenuItem
			// 
			this.removeWatchMenuItem.Name = "removeWatchMenuItem";
			this.removeWatchMenuItem.Size = new System.Drawing.Size(189, 22);
			this.removeWatchMenuItem.Text = "Remove Watch";
			// 
			// clearListMenuItem
			// 
			this.clearAllMenuItem.Name = "clearAllMenuItem";
			this.clearAllMenuItem.Size = new System.Drawing.Size(189, 22);
			this.clearAllMenuItem.Text = "Clear All";
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}

	public class WatchValue : ListViewItem
	{
		private readonly Func<int, bool, int> _readAddress;
		private readonly Func<string, DebugSymbol> _getSymbol;
		private int _addressReference = -1;

		public bool ShowAsDecimal
		{
			get { return _showAsDecimal; }
			set { _showAsDecimal = value; Update(); }
		}

		public bool ShowAsWord
		{
			get { return _showAsWord; }
			set { _showAsWord = value; Update(); }
		}

		public bool Changed { get; private set; }
		public Color DefaultColor;
		public Color ChangedColor = Color.Red;
		private bool _showAsDecimal;
		private bool _showAsWord;


		public WatchValue(string expression, bool showAsWord, bool showAsDecimal, Func<int, bool, int> readAddress, Func<string, DebugSymbol> getSymbol)
		{
			_readAddress = readAddress;
			_getSymbol = getSymbol;
			DefaultColor = ForeColor;
			Text = expression;
			_showAsWord = showAsWord;
			_showAsDecimal = showAsDecimal;
			Changed = false;
			SubItems.Add("");
			SubItems.Add("W");
			SubItems.Add("D");
			Parse();
		}

		private static Regex _parseHex = new Regex(@"^(?:0x|\$|&)([0-9a-f]+)$", RegexOptions.IgnoreCase);
		private static Regex _parseDec = new Regex(@"^([0-9]+)$");

		public void Parse()
		{
			var parseExpression = Text.Trim();
			
			_addressReference = ParseNumber(parseExpression);
			if (_addressReference < 0 && _getSymbol != null)
			{
				var symbol = _getSymbol(parseExpression);
				if (symbol != null) _addressReference = symbol.Value;
			}
			Update();
		}

		public static int ParseNumber(string input)
		{
			var match = _parseHex.Match(input);
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
			}
			match = _parseDec.Match(input);
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value);
			}

			return -1;
		}

		public void Update()
		{
			var text = GetValue();
			Changed = SubItems[1].Text != text;
			SubItems[1].ForeColor = Changed ? ChangedColor : DefaultColor;
			SubItems[1].Text = text;
		}

		public string GetValue()
		{
			return _addressReference < 0 ? "<unable to parse>" : Format(_readAddress(_addressReference, ShowAsWord));
		}

		private string Format(int value)
		{
			if (value < 0) return value == -2 ? "" : "<out of range>";
			return ShowAsDecimal ? value.ToString() : FormatHex(value, ShowAsWord ? 4 : 2);
		}

		public static string FormatHex(int value, int width)
		{
			return string.Format("${0}", Convert.ToString(value, 16).ToUpper().PadLeft(width, '0'));
		}
		public static string FormatHexAddress(int value, int addressWidth = 4)
		{
			return value > 0xffff || addressWidth > 4
				? string.Format("${0}:{1}", Convert.ToString(value >> 16, 16).ToUpper().PadLeft(2, '0'), Convert.ToString(value & 0xffff, 16).ToUpper().PadLeft(4, '0'))
				: string.Format("${0}", Convert.ToString(value, 16).ToUpper().PadLeft(value > 0xff ? 4 : 2, '0'));
		}
	}
}
