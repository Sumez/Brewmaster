using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Layout;
using Brewmaster.Modules;
using Brewmaster.Pipeline;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows.Images
{
	public class ImageWindow : SaveableEditorWindow
	{
		private ImageRenderControl _image;

		public ImageWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events)
		{
			Pristine = true;
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			var splitContainer = new MultiSplitContainer { Dock = DockStyle.Fill };

			_image = new ImageRenderControl(ProjectFile.File.FullName) { Width = 300, Height = 300 };
			splitContainer.AddPanel(new ScrollableView(_image));
			/* Image = new Bitmap(picture.HorizontalResolution, picture.VerticalResolution);

			g = Graphics.FromImage(b);

			rect = new Rectangle(0, 0, 1600, 900);
			//  sourceRect = new Rectangle(new Point(0, 0), new Size(picture.Width, picture.Height));
			sourceRect = new Rectangle(0, 0, picture.Width, picture.Height);

			g.DrawImage(picture, rect, sourceRect, GraphicsUnit.Pixel);*/

			PipelineSettings = new ImagePipelineSettings(ProjectFile, _image);
			PipelineSettings.PipelineChanged += () => { Pristine = false; };
			splitContainer.AddPanel(PipelineSettings).StaticWidth = 310;
			
			Controls.Add(splitContainer);
		}

		public ImagePipelineSettings PipelineSettings { get; set; }

		public override void Save(Func<FileInfo, string> getNewFileName = null)
		{
			if (getNewFileName != null) return;
			if (Pristine) return;

			ProjectFile.Pipeline = PipelineSettings.Pipeline == null ? null : PipelineSettings.Pipeline.Clone();
			Pristine = true;
			ProjectFile.Project.Pristine = false;
		}
	}

	public class ImageRenderControl : Control
	{
		public List<Color> Palette { get; private set; }
		public Bitmap ImageSource;
		public event Action PaletteChanged;

		private readonly List<Bitmap> _previews = new List<Bitmap>();
		private int _scale = 2;
		private int _scrollX = 0;
		private int _scrollY = 0;

		public ImageRenderControl(string filename)
		{
			try
			{
				ImageSource = ChrPipeline.LoadImageFile(filename);
				Palette = new List<Color>();
				RefreshPalette(ImageSource);

				_wantedWidth = ImageSource.Width;
				_wantedHeight = ImageSource.Height;
			}
			catch (Exception ex)
			{
				ImageSource = new Bitmap(1, 1);
				Program.Error(string.Format("Could not open image file '{0}'", new FileInfo(filename).Name), ex);
			}
		}

		public void RefreshPalette(Bitmap imageSource)
		{
			var palette = new List<Color>();
			palette.AddRange(imageSource.Palette.Entries);
			for (var y = 0; y < imageSource.Height; y++)
			{
				for (var x = 0; x < imageSource.Width; x++)
				{
					var color = imageSource.GetPixel(x, y);
					if (!palette.Contains(color)) palette.Add(color);
				}
			}

			if (!Palette.SequenceEqual(palette))
			{
				Palette.Clear();
				Palette.AddRange(palette);
				if (PaletteChanged != null) PaletteChanged();
			}
		}


		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			DoubleBuffered = true;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0) _scale++;
			else _scale--;
			if (_scale < 1) _scale = 1;
			if (_scale > 50) _scale = 50;
			Invalidate();
			PerformLayout();
		}

		private int _wantedWidth;
		private int _wantedHeight;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			var size = new Rectangle(0, 0, ImageSource.Width, ImageSource.Height);
			var offset = 0;
			var destinationSize = new Rectangle(-_scrollX, -_scrollY, ImageSource.Width * _scale, ImageSource.Height * _scale);
			e.Graphics.DrawImage(ImageSource, destinationSize, size, GraphicsUnit.Pixel);
			offset += destinationSize.Height + 2;

			foreach (var preview in _previews)
			{
				size = new Rectangle(0, 0, preview.Width, preview.Height);
				destinationSize = new Rectangle(-_scrollX, -_scrollY + offset, preview.Width * _scale, preview.Height * _scale);
				e.Graphics.DrawImage(preview, destinationSize, size, GraphicsUnit.Pixel);
				offset += destinationSize.Height + 2;
			}

			_wantedHeight = offset - 2;
			_wantedWidth = destinationSize.Width;

			if (_wantedHeight != Height || _wantedWidth != Width) Size = new Size(_wantedWidth, _wantedHeight);
		}

		protected override void Dispose(bool disposing)
		{
			ImageSource.Dispose();
			foreach (var preview in _previews)
			{
				preview.Dispose();
			}
			base.Dispose(disposing);
		}

		public void SetPreviews(params Bitmap[] previews)
		{
			foreach (var preview in _previews)
			{
				if (!previews.Contains(preview)) preview.Dispose();
			}
			_previews.Clear();
			_previews.AddRange(previews);

			RefreshPalette(_previews.Count > 0 ? _previews[0] : ImageSource);
			Refresh();
			PerformLayout();
		}
	}
}