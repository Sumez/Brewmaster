using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Brewmaster.Emulation;

namespace Brewmaster.Ppu
{
	public class SpriteRender : ScaledImageRenderer
	{
		private SpriteData _data;

		public SpriteRender()
		{
			SetImageSize(256, 240);
		}
		public void UpdateSpriteData(SpriteData data)
		{
			_data = data;
			RefreshImage();
		}
		protected override void DrawBackBuffer(Func<Graphics> getGraphics)
		{
			if (_data == null) return;

			lock (BackBufferLock)
			using (var graphics = getGraphics())
			{
				var handle = GCHandle.Alloc(_data.PixelData, GCHandleType.Pinned);
				var source = new Bitmap(256, 240, 4 * 256, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject());
				graphics.DrawImage(source, 0, 0);
				handle.Free();
			}

			if (_data.SelectedSprite >= 0)
			{
				//ctrlImagePanel.Selection = _data.SelectedBounds;
			}
			else
			{
				//ctrlImagePanel.Selection = Rectangle.Empty;
			}
		}

	}
}
