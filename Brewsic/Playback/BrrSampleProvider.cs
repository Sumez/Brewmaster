using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewsic.Playback
{
	public class BrrSampleProvider : ISampleProvider
	{
		public int LoopPointOffset { get; set; } = 0;

		private byte[] _brrData;
		private WaveFormat _waveFormat;
		private int _readPosition;
		private readonly byte[] _currentBlock = new byte[8];
		private short _prevSample = 0;
		private short _prevPrevSample = 0;
		private int _bufferPosition = 16;
		private float[] _inputBuffer = new float[16];
		private byte[] _newSampleData = null;

		public BrrSampleProvider() : this(new byte[0]) { }
		public BrrSampleProvider(byte[] brrData, int sampleRate = 32000)
		{
			SetSampleRate(sampleRate);
			Reset(brrData);
		}

		public int Read(float[] outputBuffer, int offset, int count)
		{
			var counter = count;
			while (counter > 0)
			{
				if (_bufferPosition == 16)
				{
					FillInputBuffer();
					_bufferPosition = 0;
				}

				outputBuffer[offset++] = _inputBuffer[_bufferPosition++];
				counter--;
			}
			return count;
		}

		private void FillInputBuffer()
		{
			if (_newSampleData != null)
			{
				_brrData = _newSampleData;
				_newSampleData = null;
				_readPosition = 0;
			}

			if (_readPosition >= _brrData.Length)
			{
				for (var i = 0; i < 16; i++) _inputBuffer[i] = 0;
				return;
			}

			var header = _brrData[_readPosition++];
			Buffer.BlockCopy(_brrData, _readPosition, _currentBlock, 0, 8);
			_readPosition += 8;

			var shift = (header & 0xF0) >> 4;
			var filter = (header & 0x0F) >> 2;

			var sample = new short[16];
			for (var n = 0; n < 16; n++)
			{
				var nibble = (n % 2 != 0) ? (_currentBlock[n / 2] & 0x0F) : (_currentBlock[n / 2] >> 4);
				Int32 currentSample = (Int16)(nibble << 12) >> 12; // Turns unsigned nibble into a singed integer from -8 to 7
				if (shift <= 12)
				{
					currentSample = (currentSample << shift) >> 1;
				}
				else
				{
					currentSample = (nibble >> 3) << 11;
				}

				currentSample += GetFilterAdjustment(filter, _prevSample, _prevPrevSample);

				// Clamp sample into 16bit space
				if (currentSample > 0x7FFF) currentSample = 0x7FFF;
				else if (currentSample < -0x8000) currentSample = -0x8000;

				// wrap to 15 bits, sign-extend to 16 bits
				sample[n] = (Int16)((Int16)(currentSample << 1) >> 1);

				_prevPrevSample = _prevSample;
				_prevSample = sample[n];
			}

			for (var i = 0; i < 16; i++)
			{
				_inputBuffer[i] = sample[i] / ((float)Int16.MaxValue);
			}
			if ((header & 0x03) == 0x01) _readPosition = _brrData.Length;
			if ((header & 0x03) == 0x03) _readPosition = LoopPointOffset;
		}

		private int GetFilterAdjustment(int filter, Int16 oneSampleBack, Int16 twoSamplesBack)
		{
			var adjustment = 0;
			switch (filter)
			{
				case 1:
					adjustment = oneSampleBack;  // add 16/16
					adjustment += -oneSampleBack >> 4;  // add (-1)/16
					break;

				case 2:
					adjustment = oneSampleBack << 1;  // add 64/32
					adjustment += -(oneSampleBack + (oneSampleBack << 1)) >> 5;  // add (-3)/32
					adjustment += -twoSamplesBack;  // add (-16)/16
					adjustment += twoSamplesBack >> 4;  // add 1/16
					break;

				case 3:
					adjustment = oneSampleBack << 1;  // add 128/64
					adjustment += -(oneSampleBack + (oneSampleBack << 2) + (oneSampleBack << 3)) >> 6;  // add (-13)/64
					adjustment += -twoSamplesBack;  // add (-16)/16
					adjustment += (twoSamplesBack + (twoSamplesBack << 1)) >> 4;  // add 3/16
					break;
			}

			return adjustment;
		}

		public void Reset(byte[] newSampleData)
		{
			_newSampleData = newSampleData;
		}
		public void SetSampleRate(int sampleRate)
		{
			_waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
		}

		public WaveFormat WaveFormat => _waveFormat;
	}
}
