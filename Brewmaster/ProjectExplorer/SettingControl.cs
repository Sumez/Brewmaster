using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Brewmaster.Controls;
using Brewmaster.ProjectModel;
using Brewmaster.StatusView;

namespace Brewmaster.ProjectExplorer
{
	public class SettingControl : Panel
	{
		public event Action<object> ValueChanged;

		private Label _label;
		private ComboBox _dropdown;
		private TextBox _textInput;

		public SettingControl()
		{
			Dock = DockStyle.Top;
			AutoSize = false;

			_label = new Label {Location = new Point(3, 4), AutoSize = true, Height = 13};
			_dropdown = new NonBuggyComboBox
			{
				Margin = Padding.Empty,
				DropDownStyle = ComboBoxStyle.DropDownList,
				FlatStyle = FlatStyle.Popup,
				Left = 120,
				Width = Width - 120,
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
			};
			_textInput = new TextBox
			{
				Margin = Padding.Empty,
				AutoSize = false,
				Left = 120,
				Width = Width - 120,
				Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
				//BorderStyle = BorderStyle.None,
				Height = _dropdown.Height
			};
			var border = new HorizontalLine
			{
				LineColor = SystemColors.ControlLight,
				Left = 0, Top = _dropdown.Height, Width = Width,
				Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
			};
			_dropdown.Visible = false;

			_dropdown.SelectedIndexChanged += (s, a) =>
			{
				if (ValueChanged != null) ValueChanged(_dropdown.SelectedItem);
			};
			_textInput.TextChanged += (s, a) =>
			{
				if (ValueChanged != null) ValueChanged(_textInput.Text);
			};

			Controls.Add(_label);
			Controls.Add(_textInput);
			Controls.Add(_dropdown);
			Controls.Add(border);
		}

		public string Label
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		protected override Size DefaultSize
		{
			get { return new Size(300, 22); }
		}

		[DefaultValue(InputType.Text)]
		public InputType InputType {
			get { return _textInput.Visible ? InputType.Text : InputType.Dropdown; }
			set
			{
				_textInput.Visible = value == InputType.Text;
				_dropdown.Visible = value == InputType.Dropdown;
			}
		}

		private bool _disabled;
		[DefaultValue(false)]
		public bool Disabled
		{
			get { return _disabled; }
			set
			{
				_disabled = value;
				_label.ForeColor = _disabled ? SystemColors.GrayText : ForeColor;
				_dropdown.Enabled = !_disabled;
				_textInput.Enabled = !_disabled;
			}
		}

		[DefaultValue(DockStyle.Top)]
		public override DockStyle Dock
		{
			get { return base.Dock; }
			set { base.Dock = value; }
		}

		[DefaultValue(false)]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		public object Value
		{
			set
			{
				if (_dropdown.Visible) _dropdown.SelectedItem = value;
				else _textInput.Text = value.ToString();
			}
		}

		public void SetOptions(params object[] options)
		{
			_dropdown.Items.Clear();
			_dropdown.Items.AddRange(options);
		}
	}

	public enum InputType
	{
		Text, Dropdown
	}
}
