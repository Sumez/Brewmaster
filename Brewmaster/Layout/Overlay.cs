using System;
using System.Drawing;
using System.Windows.Forms;

namespace Brewmaster.Layout
{
	public class Overlay : Form
	{
		public Rectangle Rectangle
		{
			get { return Bounds; }
			set { Bounds = value; }
		}

		public Overlay(Form parent)
		{

			FormBorderStyle = FormBorderStyle.None;
			//BackColor = Color.Magenta;
			//TransparencyKey = Color.Magenta;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			//Size = parent.ClientSize;
			//DoubleBuffered = true;
			
			//var panel = new Panel();
			//panel.BackColor = Color.FromArgb(25, Color.CadetBlue);
			//panel.Dock = DockStyle.Fill;
			//Controls.Add(panel);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			//BackColor = Color.Magenta;
			//TransparencyKey = Color.Magenta;
			BackColor = SystemColors.HotTrack; // Color.CadetBlue;
			Opacity = 0.5;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			return;
			//e.Graphics.DrawLine(Pens.Yellow, 0, 0, 100, 100);
			var rectangle = Bounds;
			rectangle.Location = new Point(0, 0);
			rectangle.Inflate(-1, -1);
			var rectangle2 = rectangle;
			rectangle2.Inflate(-6, -6);

			using (var brush = new SolidBrush(Color.CadetBlue))
			{
				//e.Graphics.DrawRectangle(new Pen(Color.CadetBlue, 2), rectangle);
				//e.Graphics.DrawRectangle(new Pen(Color.CadetBlue, 2), rectangle);
				//e.Graphics.DrawRectangle(new Pen(Color.CadetBlue, 2), rectangle2);
			}
		}
	}
}
