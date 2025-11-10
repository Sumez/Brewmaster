using NAudio.Wave;

namespace Brewsic.Playback
{
	public class PreviewPlayer
	{
		private WaveOutEvent WaveOut = new WaveOutEvent();
		private BrrSampleProvider BrrProvider;
		private WaveStream WaveProvider;

		public PreviewPlayer()
		{
			WaveOut.PlaybackStopped += WaveOut_PlaybackStopped;
		}

		private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
		{
			if (WaveProvider != null)
			{
				WaveProvider.Dispose();
				WaveProvider = null;
			}
		}

		public void PlaySilence()
		{
			PlayBrr(new byte[0], 32000);
		}

		public void PlayBrr(byte[] data, int sampleRate, int loopStart = 0)
		{
			if (BrrProvider == null)
			{
				BrrProvider = new BrrSampleProvider(data, sampleRate) { LoopPointOffset = loopStart };
			}
			else
			{
				WaveOut.Stop();
				BrrProvider.SetSampleRate(sampleRate);
				BrrProvider.Reset(data);
				BrrProvider.LoopPointOffset = loopStart;
			}
			WaveOut.Init(BrrProvider);
			WaveOut.Play();
		}
		public void PlayFile(string filePath)
		{
			WaveOut.Stop();
			if (WaveProvider != null) WaveProvider.Dispose();
			WaveProvider = AudioFile.GetWaveStreamFromFile(filePath);
			WaveOut.Init(WaveProvider);
			WaveOut.Play();
		}
	}
}
