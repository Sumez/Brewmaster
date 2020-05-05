using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Brewmaster.Emulation;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules.Ppu
{
	public class ChrRender : ScaledImageRenderer
	{
		private CharacterData _data;
		private ProjectType _type;

		public ChrRender()
		{
			SetImageSize(128, 256);
		}

		public void UpdateChrData(CharacterData data, ProjectType type)
		{
			_data = data;
			_type = type;
			RefreshImage();
		}

		protected override void DrawBackBuffer(Func<Graphics> getGraphics)
		{
			if (_data == null) return;

			if (ImageWidth != _data.Width || ImageHeight != _data.Height * _data.PixelData.Length)
			{
				SetImageSize(_data.Width, _data.Height * _data.PixelData.Length);
			}

			lock (BackBufferLock)
			using (var graphics = getGraphics())
			{
				graphics.Clear(Color.Black);
				for (var i = 0; i < _data.PixelData.Length; i++)
				{
					var handle = GCHandle.Alloc(_data.PixelData[i], GCHandleType.Pinned);
					using (var source = new Bitmap(_data.Width, _data.Height, 4 * _data.Width, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject()))
					{
						graphics.DrawImage(source, 0, _data.Height * i);
					}
					handle.Free();
				}
			}
		}

	}
}
