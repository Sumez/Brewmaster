using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BrewMaster
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
}
