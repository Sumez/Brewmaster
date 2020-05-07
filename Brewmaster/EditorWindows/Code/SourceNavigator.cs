using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.EditorWindows.Code
{
	public class SourceNavigator : UserControl
	{
		public List<Symbol> Symbols { get; set; }
		public event Action<int> Navigated;

		public int CurrentLine
		{
			get { return _currentLine; }
			set
			{
				_currentLine = value;
				if (!_suppressEvent) UpdateCurrentSymbol();
			}
		}

		public void UpdateCurrentSymbol()
		{
			var currentSymbol = Symbols.LastOrDefault(s => s.Line <= CurrentLine + 1);
			if (currentSymbol != null)
			{
				_suppressEvent = true;
				_symbolSelector.SelectedItem = currentSymbol;
				_suppressEvent = false;
			}
		}
		private void symbolSelector_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_suppressEvent || Navigated == null) return;
			_suppressEvent = true;
			Navigated(_currentLine = ((Symbol) _symbolSelector.SelectedItem).Line);
			_suppressEvent = false;
		}

		public SourceNavigator()
		{
			Symbols = new List<Symbol>();
			InitializeComponent();
		}

		private Panel _bottomBorder;
		private Panel _sideBorder;
		private ComboBox _symbolSelector;
		private int _currentLine;
		private bool _suppressEvent;

		private void InitializeComponent()
		{
			this._symbolSelector = new System.Windows.Forms.ComboBox();
			this._bottomBorder = new System.Windows.Forms.Panel();
			this._sideBorder = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this._symbolSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._symbolSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._symbolSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._symbolSelector.FormattingEnabled = true;
			this._symbolSelector.ItemHeight = 13;
			this._symbolSelector.Location = new System.Drawing.Point(503, 4);
			this._symbolSelector.Name = "_symbolSelector";
			this._symbolSelector.Size = new System.Drawing.Size(263, 21);
			this._symbolSelector.TabIndex = 0;
			this._symbolSelector.SelectedIndexChanged += new System.EventHandler(this.symbolSelector_SelectedIndexChanged);
			// 
			// panel1
			// 
			this._bottomBorder.BackColor = System.Drawing.SystemColors.ControlDark;
			this._bottomBorder.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._bottomBorder.Location = new System.Drawing.Point(0, 29);
			this._bottomBorder.Name = "_bottomBorder";
			this._bottomBorder.Size = new System.Drawing.Size(771, 1);
			this._bottomBorder.TabIndex = 1;
			// 
			// panel2
			// 
			this._sideBorder.BackColor = System.Drawing.SystemColors.ControlDark;
			this._sideBorder.Location = new System.Drawing.Point(0, 0);
			this._sideBorder.Name = "_sideBorder";
			this._sideBorder.Size = new System.Drawing.Size(1, 97);
			this._sideBorder.TabIndex = 2;
			// 
			// SourceNavigator
			// 
			this.Controls.Add(this._sideBorder);
			this.Controls.Add(this._bottomBorder);
			this.Controls.Add(this._symbolSelector);
			this.Name = "SourceNavigator";
			this.Size = new System.Drawing.Size(771, 30);
			this.ResumeLayout(false);

		}

		public void UpdateSymbols(IEnumerable<Symbol> symbols)
		{
			Symbols = symbols.Where(s => !s.Text.StartsWith("@")).OrderBy(s => s.Line).ToList();
			if (Symbols.Count > 0) Symbols.Insert(0, new Symbol { Text = new FileInfo(Symbols[0].Source).Name, Line = 0 });
			_symbolSelector.Items.Clear();
			_symbolSelector.Items.AddRange(Symbols.ToArray());
			UpdateCurrentSymbol();
			_symbolSelector.Enabled = Symbols.Count > 0;
		}

	}
}
