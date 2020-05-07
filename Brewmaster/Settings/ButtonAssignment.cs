using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using Brewmaster.Emulation;

namespace Brewmaster.Settings
{
	public class ButtonAssignment : Form, IMessageFilter
	{
		public ButtonAssignment(bool shortcut)
		{
			_emulator = new NesEmulatorHandler(this);
			_shortcut = shortcut;
			InitializeComponent();
		}

		private bool _shortcut;
		public Keys KeyboardInput { get; private set; }
		public int KeyCode { get; private set; }
		public string ButtonName { get; private set; }
		public Timer Timer { get; private set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!_shortcut)
			{
				Application.AddMessageFilter(this);
				Timer = new Timer {Interval = 10};
				Timer.Tick += (s, a) => CheckEmulatorInput();
				Timer.Start();

				NesEmulatorHandler.InitiateInputCheck();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!_shortcut)
			{
				Application.RemoveMessageFilter(this);
				Timer.Stop();
				Timer.Dispose();
				NesEmulatorHandler.EndInputCheck();
			}

			base.OnClosing(e);
		}
		protected void CheckEmulatorInput()
		{
			var button = NesEmulatorHandler.GetCurrentInput();
			if (button == null) return;
			KeyCode = (int)button.Item1;
			ButtonName = button.Item2;
			Close();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (!_shortcut) return false;

			if (keyData == Keys.Escape)
			{
				Close();
				return true;
			}

			var pressedKey = keyData & Keys.KeyCode;
			if (pressedKey == Keys.ShiftKey
			    || pressedKey == Keys.ControlKey
			    || pressedKey == Keys.LWin
			    || pressedKey == Keys.RWin
			    || pressedKey == Keys.Menu) return true;

			try
			{
				testMenu.ShortcutKeys = keyData;
			}
			catch (InvalidEnumArgumentException)
			{
				return true;
			}

			KeyboardInput = keyData;
			Close();
			return true;
		}

		public static string GetString(object keys)
		{
			return new KeysConverter().ConvertTo(null, CultureInfo.CurrentUICulture, keys, typeof(string))?.ToString()
				.Replace("Oemcomma", ",")
				.Replace("OemPeriod", ".")
				.Replace("Oem", "");
		}

		private ToolStripMenuItem testMenu = new ToolStripMenuItem("test");
		private IEmulatorHandler _emulator;


		private const int WM_KEYDOWN = 0x100;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYDOWN = 0x104;
		private const int WM_SYSKEYUP = 0x105;

		public bool PreFilterMessage(ref Message m)
		{

			var scanCode = (Int32)(((Int64)m.LParam & 0x1FF0000) >> 16);
			if (m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP)
			{
				_emulator.SetKeyState(scanCode, false);
			}
			else if (ContainsFocus && (m.Msg == WM_SYSKEYDOWN || m.Msg == WM_KEYDOWN))
			{
				_emulator.SetKeyState(scanCode, true);
			}
			return false;
		}


		private void InitializeComponent()
		{
			System.Windows.Forms.Label label1;
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(66, 57);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(105, 13);
			label1.TabIndex = 0;
			label1.Text = "Press key(s) to use...";
			// 
			// ButtonAssignment
			// 
			this.ClientSize = new System.Drawing.Size(231, 140);
			this.ControlBox = false;
			this.Controls.Add(label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ButtonAssignment";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}