using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Brewmaster
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://brewmaster.dev");
			}
			catch (Exception ex)
			{
				
			}
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
