using System;
using System.Threading;

namespace Brewmaster.Emulation
{
	public abstract class EmulatorHandler
	{
		public event Action<int> OnFpsUpdate;
		public int UpdateRate { get; set; }

		public abstract bool IsRunning();

		private int _updateCounter;
		
		private byte _frameCount = 0;
		private Timer _timer;
		protected virtual void GameLoaded()
		{
			if (_timer != null) _timer.Dispose();
			_timer = new Timer(SampleFrameRate, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}

		private void SampleFrameRate(object state)
		{

			if (IsRunning() && OnFpsUpdate != null) OnFpsUpdate(_frameCount);
			_frameCount = 0;
		}

		protected virtual void CountFrame()
		{
			_frameCount++;

			if (UpdateRate == 0) return;
			_updateCounter++;
			if (_updateCounter < UpdateRate) return;
			EmitDebugData();
			_updateCounter = 0;
		}

		protected abstract void EmitDebugData();
	}
}
