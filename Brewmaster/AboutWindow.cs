using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace Brewmaster
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();

	        var version = "?.?.?";
	        try
	        {
		        var assembly = Assembly.GetExecutingAssembly();
		        version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
	        }
	        catch { }
			text.Text = string.Format("Version {0}\r\n\r\nAn open source homebrew IDE for Windows\r\n\r\n2019-2021 Created by Sumez", version);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://brewmaster.dev");
			}
			catch { }
		}
	}
	public class LogoPicture : PictureBox
	{
		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{
			paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			base.OnPaint(paintEventArgs);
		}
	}
}
