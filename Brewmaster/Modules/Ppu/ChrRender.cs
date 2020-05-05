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

		private Events _events;
		public Events ModuleEvents
		{
			set
			{
				_events = value;
				if (_events == null) return;
				_events.EmulationStateUpdate += (state) => UpdateChrData(state.CharacterData, state.Type);
			}
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

			lock (BackBufferLock)
			using (var graphics = getGraphics())
			{
				graphics.Clear(Color.Black);
				for (var i = 0; i < _data.PixelData.Length; i++)
				{
					var handle = GCHandle.Alloc(_data.PixelData[i], GCHandleType.Pinned);
					using (var source = new Bitmap(128, 128, 4 * 128, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject()))
					{
						graphics.DrawImage(source, 0, 128 * i);
					}
					handle.Free();
				}
			}
		}

	}
}
