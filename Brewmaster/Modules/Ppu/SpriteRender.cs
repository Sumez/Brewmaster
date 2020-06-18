using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Brewmaster.Emulation;
using Brewmaster.ProjectModel;

namespace Brewmaster.Modules.Ppu
{
	public class SpriteRender : ScaledImageRenderer
	{
		private SpriteData _data;
		private TargetPlatform _type;

		public SpriteRender()
		{
			SetImageSize(256, 240);
		}

		private Events _events;
		public Events ModuleEvents
		{
			set
			{
				_events = value;
				if (_events == null) return;
				_events.EmulationStateUpdate += (state) => UpdateSpriteData(state.Sprites, state.Type);
				_events.SelectedSpriteChanged += index => RefreshImage();
			}
		}

		public void UpdateSpriteData(SpriteData data, TargetPlatform type)
		{
			_data = data;
			_type = type;
			RefreshImage();
		}

		protected override void OnMouseDownScaled(MouseEventArgs e)
		{
			if (_data == null) return;

			for (var i = 0; i < _data.Details.Count; i++)
			{
				var sprite = _data.Details[i];
				if (sprite.Bounds.Contains(e.Location))
				{
					_events.SelectSprite(i);
					break;
				}
			}

		}

		protected override void DrawBackBuffer(Func<Graphics> getGraphics)
		{
			if (_data == null) return;

			lock (BackBufferLock)
			using (var graphics = getGraphics())
			{
				graphics.Clear(Color.Transparent);
				var handle = GCHandle.Alloc(_data.PixelData, GCHandleType.Pinned);
				if (_type == TargetPlatform.Nes)
				{
					using (var source = new Bitmap(64, 128, 4 * 64, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject()))
					{
						for (var i = 63; i >= 0; i--)
						{
							var sprite = _data.Details[i];
							if (sprite.Y < 240)
								graphics.DrawImage(source,
									sprite.Bounds,
									new Rectangle((i % 8) * 8, (i / 8) * 16, sprite.Width, sprite.Height),
									GraphicsUnit.Pixel);
						}
					}
				}
				else
				{
					using (var source = new Bitmap(256, 240, 4 * 256, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject()))
						graphics.DrawImage(source, 0, 0);
				}
				handle.Free();

				if (_events.SelectedSprite >= 0 && _events.SelectedSprite < _data.Details.Count)
				{
					var scale = 1;
					var sprite = _data.Details[_events.SelectedSprite];
					var selection = sprite.Bounds;
					graphics.DrawRectangle(Pens.White, selection.Left * scale, selection.Top * scale, selection.Width * scale + 0.5f, selection.Height * scale + 0.5f);
					graphics.DrawRectangle(Pens.Gray, selection.Left * scale - 1, selection.Top * scale - 1, selection.Width * scale + 2.5f, selection.Height * scale + 2.5f);
				}
			}
		}

	}
}
