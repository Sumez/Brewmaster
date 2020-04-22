using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.Emulation
{
	public class MesenRenderer : UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		public MesenRenderer()
		{
			InitializeComponent();

			_renderSurfaces = new Dictionary<ProjectType, UserControl>();

			SizeChanged += (obj, args) =>
			{
				ResizeSurface();
			};
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// MesenRenderer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(256F, 240F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "MesenRenderer";
			this.BackColor = Color.Black;
			this.ResumeLayout(false);

		}
		#endregion

		public Func<double, bool> ScaleChanged { get; set; }
		public bool IntegerScaling { get; set; }

		private UserControl _currentRenderSurface;
		private readonly Dictionary<ProjectType, UserControl> _renderSurfaces;

		public void ResizeSurface()
		{
			if (_currentRenderSurface == null) return;
			var t = Math.Min((double)Width / 256, (double)Height / 240);
			if (IntegerScaling) t = Math.Floor(t);
			var targetWidth = (int)(256 * t);
			var targetHeight = (int)(240 * t);
			//renderSurface.SizeChanged

			if (ScaleChanged == null || !ScaleChanged(t)) _currentRenderSurface.Size = new Size(targetWidth, targetHeight);
			_currentRenderSurface.Location = new Point((Size.Width - _currentRenderSurface.Size.Width) / 2, (Size.Height - _currentRenderSurface.Size.Height) / 2);

		}

		public Control GetRenderSurface(ProjectType projectType)
		{
			if (!_renderSurfaces.ContainsKey(projectType))
			{
				var renderSurface = new UserControl();
				renderSurface.Dock = DockStyle.None;
				renderSurface.Location = new Point(0, 0);
				renderSurface.TabIndex = 1;
				_renderSurfaces.Add(projectType, renderSurface);
			}
			return _renderSurfaces[projectType];
		}

		public void SwitchRenderSurface(ProjectType projectType)
		{
			if (_currentRenderSurface != null) Controls.Remove(_currentRenderSurface);
			_currentRenderSurface = _renderSurfaces[projectType];
			Controls.Add(_currentRenderSurface);
			ResizeSurface();
		}
	}

}