using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Brewmaster.Emulation
{
	public abstract class EmulatorHandler
	{
		public event Action<int> OnFpsUpdate;
		public int UpdateRate { get; set; }

		public abstract bool IsRunning();

		private readonly double[] _frameTimes = Enumerable.Repeat(16.666d, 60).ToArray();
		private readonly Stopwatch _frameTimer = new Stopwatch();
		private byte _frameCount = 0;
		private int _updateCounter;

		private byte _frameCount2 = 0;
		private Timer _timer;
		protected virtual void GameLoaded()
		{
			_frameTimer.Reset();
			_frameTimer.Start();

			if (_timer != null) _timer.Dispose();
			_timer = new Timer(SampleFrameRate, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}

		private void SampleFrameRate(object state)
		{

			if (IsRunning() && OnFpsUpdate != null) OnFpsUpdate(_frameCount2);
			_frameCount2 = 0;
		}

		protected virtual void CountFrame()
		{
			_frameCount2++;
			_frameCount++;
			if (_frameCount == 60) _frameCount = 0;
			_frameTimes[_frameCount] = _frameTimer.Elapsed.TotalMilliseconds;
			_frameTimer.Reset();
			_frameTimer.Start();
			var frameSum = _frameTimes.Sum();
			var fps = (int)(60000 / (frameSum > 0 ? frameSum : 1000));
			//if (_frameCount % 15 == 0 && OnFpsUpdate != null) OnFpsUpdate(fps);

			if (UpdateRate == 0) return;
			_updateCounter++;
			if (_updateCounter < UpdateRate) return;
			EmitDebugData();
			_updateCounter = 0;
		}

		protected abstract void EmitDebugData();
	}
}
