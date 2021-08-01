using System;
using System.Collections.Generic;
using System.Linq;

namespace Brewsic
{
	public class Sample
	{
		public string Name { get; set; }
		public uint Length { get; set; }
		public Int16[] Data { get; set; }
		public uint LoopStart { get; set; }
		public uint LoopEnd { get; set; }
		public int Panning { get; set; }
		public int Volume { get; set; }
		public int GlobalVolume { get; set; }
		public double C5Speed { get; set; }
		public bool ConvertedFromStereo { get; set; }


		private const float BaseAdjustRate = 0.0004f;

		private void FixLooping(int maximumSampleGrowth, Action<string> output)
		{
			// If the size of our loop (start->end) isn't a multiple of 16, it can't fit exactly into the addresses of the SNES sample format's 9-byte blocks,
			// so we can either unroll the loop by repeating samples until they align, or we can try to "stretch" the sample until it fits, and interpolate
			// between the samples, with a slight hit to audio quality

			if (LoopEnd == 0)
			{
				if (Data.Length % 16 != 0)
				{
					// Pad the begginning, even though sample doesn't loop, we want to start with some silent samples to prevent popping
					var samplesToAdd = 16 - Data.Length % 16;
					var newData = new Int16[Data.Length + samplesToAdd];
					Array.Copy(Data, 0, newData, samplesToAdd, Data.Length);
					Data = newData;
				}
				return;
			}

			// The engine can't play a sample beyond its looping point, so there's no reason to store any samples after LoopEnd:
			var croppedData = new Int16[LoopEnd];
			Array.Copy(Data, 0, croppedData, 0, croppedData.Length);
			Data = croppedData;


			var startAlign = (16 - (LoopStart % 16)) % 16;
			var loopSize = LoopEnd - LoopStart;
			var endAlign = loopSize;

			while ((endAlign % 16) != 0) endAlign <<= 1;

			endAlign -= loopSize;
			endAlign += startAlign;

			var addedBytes = Math.Ceiling(endAlign / 16f) * 9;
			if (endAlign != 0) // If endAlign == 0, the loop is already aligned
			{
				if (addedBytes > maximumSampleGrowth)
				{
					ResampleLoop(16 - (loopSize % 16));
				}
				else
				{
					if (output != null && addedBytes >= 200) output(string.Format("WARNING: Sample \"{0}\" loop not aligned to 16 frames, results in {1} bytes larger file size", Name, addedBytes));
					UnrollLoop(startAlign, endAlign);
				}

			}
			if (LoopStart % 16 != 0)
			{
				// Pad the beginning
				var samplesToAdd = 16 - LoopStart % 16;
				var newData = new Int16[Data.Length + samplesToAdd];
				Array.Copy(Data, 0, newData, samplesToAdd, Data.Length);
				Data = newData;
				LoopStart += (uint)samplesToAdd;
			}


		}

		private void Downsample(int newLength)
		{
			//if (newLength < Data.Length / 2) newLength = Data.Length / 2; // Never cut a sample down to less than half its size
			var resizeFactor = (double)Data.Length / newLength;

			var data = new Int16[newLength];
			for (var t = 0; t < data.Length; t++)
			{
				// Let's say we're interpolating 5 samples into 10, that means position 5 in the new sample
				// should sound like "position" 2.5 in the old one, so we'd need to calculate the sample 50%
				// between index 2 and 3
				var sampleIndex = t * resizeFactor;
				var s1Index = (int)Math.Floor(sampleIndex);
				var s2Index = s1Index + 1;
				double sample1 = Data[s1Index];
				double sample2 = Data[s2Index];

				var ratio = sampleIndex % 1 + 0.5; // Find target distance between the two samples
				var newSample = (int)(Math.Floor(sample1 * (1 - ratio) + sample2 * ratio)); // Interpolate between the two samples

				data[t] = (Int16)Clamp(newSample, 16); // Clip the new sample value if it exceeds 16bit range
			}
			C5Speed /= resizeFactor; // Since the loop becomes a little longer, we need to play it back a little faster
			LoopStart = (UInt32)Math.Round(LoopStart / resizeFactor);
			LoopEnd = (UInt32)Math.Round(LoopEnd / resizeFactor);
			Data = data;
		}

		private void ResampleLoop(UInt32 samplesToAdd)
		{
			var loopSize = LoopEnd - LoopStart;
			var newLoopSize = loopSize + samplesToAdd;
			var newLength = (uint)Data.Length + samplesToAdd;
			var newLoopStart = newLength - newLoopSize;

			var resizeFactor = (double)Data.Length / newLength;

			var data = new Int16[newLength];
			for (var t = 0; t < data.Length; t++)
			{
				// Let's say we're interpolating 5 samples into 10, that means position 5 in the new sample
				// should sound like "position" 2.5 in the old one, so we'd need to calculate the sample 50%
				// between index 2 and 3
				var sampleIndex = (double)t * resizeFactor;
				var s1Index = (int)Math.Floor(sampleIndex);
				var s2Index = s1Index + 1;
				double sample1 = s1Index < Data.Length ? Data[s1Index] : Data[s1Index - loopSize];
				double sample2 = s2Index < Data.Length ? Data[s2Index] : Data[s2Index - loopSize];

				var ratio = sampleIndex % 1;
				var newSample = (int)(Math.Floor(sample1 * (1 - ratio) + sample2 * ratio + 0.5)); // Interpolate between the two samples

				data[t] = (Int16)Clamp(newSample, 16); // Clip the new sample value if it exceeds 16bit range
			}

			LoopStart = newLoopStart;
			C5Speed /= resizeFactor; // Since the loop becomes a little longer, we need to play it back a little faster

			Data = data;
		}


		private void UnrollLoop(uint startAlign, uint endAlign)
		{
			var data = new Int16[LoopEnd + endAlign];
			Array.Copy(Data, data, Math.Min(Data.Length, LoopEnd));

			for (var i = 0; i < endAlign; i++) data[LoopEnd + i] = data[LoopStart + i];

			// 16-sample align loop_start
			LoopStart += startAlign;
			Data = data;
		}

		private static bool StartsWithSilence(Int16[] data, int sampleSize = 16, int threshold = 0)
		{
			for (var i = 0; i < sampleSize; i++)
			{
				if (Math.Abs((int)data[i]) > threshold) return false;
			}

			return true;
		}

		public byte[] GetBrr(int maximumSampleGrowth = 500, int maxSampleSize = 0, Action<string> output = null)
		{
			var brrData = new List<byte>();
			if (Data == null || Data.Length == 0) return brrData.ToArray();

			maxSampleSize = maxSampleSize / 9 * 16; // Convert BRR sample size to sample length;
			if (maxSampleSize > 500 && Data.Length > maxSampleSize) Downsample(maxSampleSize);
			//while (C5Speed > 20000) Downsample((int)(Data.Length * 20000 / C5Speed));
			FixLooping(maximumSampleGrowth, output);

			if (!StartsWithSilence(Data, 8))
			{
				// Add a silent block to prevent popping samples
				var newData = new Int16[Data.Length + 16];
				Array.Copy(Data, 0, newData, 16, Data.Length);
				Data = newData;
				LoopStart += (uint)16;
			}

			var data = new Int16[Data.Length];
			Array.Copy(Data, data, Data.Length);//TODO: Remove this copy, since we are copying each block anyway

			var testdata = new Int16[Data.Length];
			Array.Copy(Data, testdata, Data.Length);
			var compare = OldMethod(testdata);

			var blockCount = (int)Math.Ceiling(data.Length / 16f);
			var backBuffer = new Int16[2]; // Maintain a back buffer of the previous two samples for calculating BRR filters

			double totalAccuracy = 0;
			double worstAccuracy = 0;
			double bestAccuracy = 1e20;

			for (var blockIndex = 0; blockIndex < blockCount; blockIndex++)
			{
				var block = CompressBlock(data.Skip(blockIndex * 16).Take(16).ToArray(), blockIndex == 0, backBuffer);
				backBuffer[0] = block.Samples[16];
				backBuffer[1] = block.Samples[17];

				totalAccuracy += block.Accuracy;
				if (block.Accuracy < bestAccuracy) bestAccuracy = block.Accuracy;
				if (block.Accuracy > worstAccuracy) worstAccuracy = block.Accuracy;

				brrData.AddRange(block.Data);
			}
			brrData[brrData.Count - 9] |= (byte)(1 | (LoopEnd > 0 ? 2 : 0)); // Set end bit

			if (!brrData.SequenceEqual(compare))
			{
				//throw new Exception("BRR routine error");
			}
			return brrData.ToArray();
		}

		private class CompressedBlock
		{
			public double Accuracy = double.MaxValue;
			public byte[] Data = new byte[9];
			public Int16[] Samples = new Int16[18];
			public UInt16 Overflow = 0;
		}
		private CompressedBlock CompressBlock(Int16[] data, bool firstBlock, Int16[] previousSamples)
		{
			var currentAdjustRate = BaseAdjustRate;
			CompressedBlock block;
			do
			{
				block = CompressBlock2(data, firstBlock, previousSamples);
				if (block.Overflow != 0)
				{
					// If block creates overflow, adjust samples, loop back, and test the same block with a different adjustment rate
					var f = new float[16];
					for (var i = 0; i < 16; i++) f[i] = currentAdjustRate;

					var overflow = block.Overflow;
					for (var sampleIndex = 0; sampleIndex < 16; sampleIndex++, overflow <<= 1)
						if ((overflow & 0x8000) != 0)
						{
							var t = 0.05f;
							for (var i = sampleIndex; i >= 0; --i, t *= 0.1f) f[i] *= 1.0f + t;

							t = 0.05f * 0.1f;
							for (var i = sampleIndex + 1; i < 16; ++i, t *= 0.1f) f[i] *= 1.0f + t;
						}

					for (var i = 0; i < 16; i++) data[i] = (Int16)(data[i] * (1.0 - f[i]));
					currentAdjustRate *= 1.1f;
				}

			} while (block.Overflow != 0);

			return block;
		}

		private CompressedBlock CompressBlock2(Int16[] data, bool firstBlock, Int16[] previousSamples)
		{
			var block = new CompressedBlock();
			var backBuffer = new Int16[18];

			// Carry over the two last samples from the previous block, to test against filters in this block
			backBuffer[0] = previousSamples[0];
			backBuffer[1] = previousSamples[1];

			for (var filter = 0; filter <= 3; filter++)
			{
				if (filter != 0 && firstBlock) continue; // Filters rely on previous samples, so never use them on the first block

				// Ranges 0, 13, 14, 15 are "invalid", so they are not used for encoding.
				// The values produced by these ranges are fully covered by the other
				// range values, so there will be no loss in quality.
				for (byte range = 12; range > 0; range--)
				{
					var halfRange = (1 << range) / 2;
					double accuracy = 0; // The lower the number, the closer to the original sample
					var blockData = new byte[16];

					for (var sampleIndex = 0; sampleIndex < 16; sampleIndex++)
					{
						// Simulate the way the SNES DSP handles each filter, and compare the results to store the best data in the current block
						var filterAdjustment = GetFilterAdjustment(filter, backBuffer[sampleIndex + 1], backBuffer[sampleIndex]);
						var originalSample = data[sampleIndex] >> 1;

						// undo 16 -> 15 bit wrapping
						// check both possible 16-bit values
						Int32 sample1 = (Int16)(originalSample & 0x7FFF);
						Int32 sample2 = (Int16)(originalSample | 0x8000);

						// undo filtering
						sample1 -= filterAdjustment;
						sample2 -= filterAdjustment;

						// restore low bit lost during range decoding
						sample1 <<= 1;
						sample2 <<= 1;

						// reduce s to range with nearest value rounding
						// range = 2, rhalf = 2
						// s(-6, -5, -4, -3) = -1
						// s(-2, -1,  0,  1) =  0
						// s( 2,  3,  4,  5) =  1
						sample1 = (sample1 + halfRange) >> range;
						sample2 = (sample2 + halfRange) >> range;

						sample1 = Clamp(sample1, 4);
						sample2 = Clamp(sample2, 4);

						var rs1 = (byte)(sample1 & 0x0F);
						var rs2 = (byte)(sample2 & 0x0F);

						// -16384 to 16383
						sample1 = (sample1 << range) >> 1;
						sample2 = (sample2 << range) >> 1;

						// BRR accumulates to 17 bits, saturates to 16 bits, and then wraps to 15 bits
						if (filter >= 2)
						{
							sample1 = Clamp(sample1 + filterAdjustment, 16);
							sample2 = Clamp(sample2 + filterAdjustment, 16);
						}
						else
						{
							// don't clamp - result does not overflow 16 bits
							sample1 += filterAdjustment;
							sample2 += filterAdjustment;
						}

						// wrap to 15 bits, sign-extend to 16 bits
						sample1 = ((Int16)(sample1 << 1) >> 1);
						sample2 = ((Int16)(sample2 << 1) >> 1);

						double sampleAccuracy1 = originalSample - sample1;
						double sampleAccuracy2 = originalSample - sample2;

						sampleAccuracy1 *= sampleAccuracy1;
						sampleAccuracy2 *= sampleAccuracy2;

						// If accuracy is equal, prefer sample 2 over s sample 1.
						if (sampleAccuracy1 < sampleAccuracy2)
						{
							accuracy += sampleAccuracy1;
							backBuffer[sampleIndex + 2] = (Int16)sample1;
							blockData[sampleIndex] = rs1;
						}
						else
						{
							accuracy += sampleAccuracy2;
							backBuffer[sampleIndex + 2] = (Int16)sample2;
							blockData[sampleIndex] = rs2;
						}
					}

					// Update block if current accuracy is better than the one currently stored with the block
					if (accuracy < block.Accuracy)
					{
						block.Accuracy = accuracy;

						for (uint n = 0; n < 16; ++n) block.Samples[n + 2] = backBuffer[n + 2];

						block.Data[0] = (byte)((range << 4) | (filter << 2) & 0xFF);
						for (uint n = 0; n < 8; ++n) block.Data[n + 1] = (byte)(((blockData[n * 2] << 4) | blockData[n * 2 + 1]) & 0xFF);
					}
				}
			}

			for (var i = 0; i < 16; i++)
			{
				var test = new Int16[3];
				Buffer.BlockCopy(block.Samples, i * 2, test, 0, 3 * 2);
				byte b = TestOverflow(test);
				block.Overflow = (UInt16)((block.Overflow << 1) | b);
			}

			return block;
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

		private static int Clamp(int x, int N)
		{
			var low = -1 << (N - 1);
			var high = (1 << (N - 1)) - 1;

			if (x > high) x = high;
			else if (x < low) x = low;

			return x;
		}
		private static byte TestOverflow(Int16[] ls)
		{
			byte r;

			// p = -256; gauss_table[255, 511, 256]
			r = TestGauss(370, 1305, 374, ls);

			// p = -255; gauss_table[254, 510, 257]
			r |= TestGauss(366, 1305, 378, ls);

			// p = -247; gauss_table[246, 502, 265]
			r |= TestGauss(336, 1303, 410, ls);

			return r;
		}
		private static byte TestGauss(int g4, int g3, int g2, Int16[] ls)
		{
			var s = (Int32)(g4 * ls[0]) >> 11;
			s += (Int32)(g3 * ls[1]) >> 11;
			s += (Int32)(g2 * ls[2]) >> 11;
			return (s > 0x3FFF) || (s < -0x4000) ? (byte)1 : (byte)0;
		}



		private byte[] OldMethod(Int16[] data)
		{
			var brrData = new List<byte>();

			const float base_adjust_rate = 0.0004f;
			float adjust_rate = base_adjust_rate;
			var loopBlock = (int)Math.Round(LoopStart / 16f);
			var blockCount = (int)Math.Ceiling(data.Length / 16f);

			var best_samp = new Int16[18];

			best_samp[0] = 0;
			best_samp[1] = 0;

			double total_error = 0;
			double max_error = 0;
			double min_error = 1e20;

			for (var blockIndex = 0; blockIndex < blockCount; blockIndex++)
			{
				var p = new Int16[16];
				var copyCount = Math.Min(16, data.Length - (blockIndex * 16));
				Buffer.BlockCopy(data, 16 * 2 * blockIndex, p, 0, copyCount * 2);

				double best_err = 1e20;
				var blk_samp = new Int16[18];
				var best_data = new byte[9];

				blk_samp[0] = best_samp[0];
				blk_samp[1] = best_samp[1];

				for (byte filter = 0; filter <= 3; filter++)
				{
					if (filter != 0 && blockIndex == 0) continue; // Filters rely on previous samples, so never use them on the first block

					// Ranges 0, 13, 14, 15 are "invalid", so they are not used for encoding.
					// The values produced by these ranges are fully covered by the other
					// range values, so there will be no loss in quality.
					for (byte range = 12; range >= 1; range--)
					{
						Int32 rhalf = (1 << range) >> 1;
						double blk_err = 0;
						var blk_data = new byte[16];

						for (var n = 0; n < 16; n++)
						{

							Int32 filter_s;

							switch (filter)
							{
								default:
								case 0:
									filter_s = 0;
									break;

								case 1:
									filter_s = blk_samp[1 + n];  // add 16/16
									filter_s += -blk_samp[1 + n] >> 4;  // add (-1)/16
									break;

								case 2:
									filter_s = blk_samp[1 + n] << 1;  // add 64/32
									filter_s += -(blk_samp[1 + n] + (blk_samp[1 + n] << 1)) >> 5;  // add (-3)/32
									filter_s += -blk_samp[n];  // add (-16)/16
									filter_s += blk_samp[n] >> 4;  // add 1/16
									break;

								case 3:
									filter_s = blk_samp[1 + n] << 1;  // add 128/64
									filter_s += -(blk_samp[1 + n] + (blk_samp[1 + n] << 2) + (blk_samp[1 + n] << 3)) >> 6;  // add (-13)/64
									filter_s += -blk_samp[n];  // add (-16)/16
									filter_s += (blk_samp[n] + (blk_samp[n] << 1)) >> 4;  // add 3/16
									break;
							}

							// undo 15 -> 16 bit conversion
							Int32 xs = p[n] >> 1;

							// undo 16 -> 15 bit wrapping
							// check both possible 16-bit values
							Int32 s1 = (Int16)(xs & 0x7FFF);
							Int32 s2 = (Int16)(xs | 0x8000);

							// undo filtering
							s1 -= filter_s;
							s2 -= filter_s;

							// restore low bit lost during range decoding
							s1 <<= 1;
							s2 <<= 1;

							// reduce s to range with nearest value rounding
							// range = 2, rhalf = 2
							// s(-6, -5, -4, -3) = -1
							// s(-2, -1,  0,  1) =  0
							// s( 2,  3,  4,  5) =  1
							s1 = (s1 + rhalf) >> range;
							s2 = (s2 + rhalf) >> range;

							s1 = Clamp(s1, 4);
							s2 = Clamp(s2, 4);

							var rs1 = (byte)(s1 & 0x0F);
							var rs2 = (byte)(s2 & 0x0F);

							// -16384 to 16383
							s1 = (s1 << range) >> 1;
							s2 = (s2 << range) >> 1;

							// BRR accumulates to 17 bits, saturates to 16 bits, and then wraps to 15 bits

							if (filter >= 2)
							{
								s1 = Clamp(s1 + filter_s, 16);
								s2 = Clamp(s2 + filter_s, 16);
							}
							else
							{
								// don't clamp - result does not overflow 16 bits
								s1 += filter_s;
								s2 += filter_s;
							}

							// wrap to 15 bits, sign-extend to 16 bits
							s1 = ((Int16)(s1 << 1) >> 1);
							s2 = ((Int16)(s2 << 1) >> 1);

							double d1 = xs - s1;
							double d2 = xs - s2;

							d1 *= d1;
							d2 *= d2;

							// If d1 == d2, prefer s2 over s1.
							if (d1 < d2)
							{
								blk_err += d1;
								blk_samp[2 + n] = (Int16)s1;
								blk_data[n] = rs1;
							}
							else
							{
								blk_err += d2;
								blk_samp[2 + n] = (Int16)s2;
								blk_data[n] = rs2;
							}
						}  // block loop

						// Use < for comparison. This will cause the encoder to prefer
						// less complex filters and higher ranges when error rates are equal.
						// This will then result in a slightly lower average error rate.
						if (blk_err < best_err)
						{
							best_err = blk_err;

							for (uint n = 0; n < 16; ++n) best_samp[n + 2] = blk_samp[n + 2];

							best_data[0] = (byte)((range << 4) | (filter << 2) & 0xFF);
							for (uint n = 0; n < 8; ++n) best_data[n + 1] = (byte)(((blk_data[n * 2] << 4) | blk_data[n * 2 + 1]) & 0xFF);
						}
					}  // range loop
				}  // filter loop

				UInt16 overflow = 0;

				for (var n = 0; n < 16; ++n)
				{
					var test = new Int16[3];
					Buffer.BlockCopy(best_samp, n * 2, test, 0, 3 * 2);
					byte b = TestOverflow(test);
					overflow = (UInt16)((overflow << 1) | b);
				}

				if (overflow != 0)
				{
					var f = new float[16];

					for (uint n = 0; n < 16; ++n) f[n] = adjust_rate;

					for (int n = 0; n < 16; ++n, overflow <<= 1)
						if ((overflow & 0x8000) != 0)
						{
							float t = 0.05f;

							for (int i = n; i >= 0; --i, t *= 0.1f) f[i] *= 1.0f + t;

							t = 0.05f * 0.1f;
							for (int i = n + 1; i < 16; ++i, t *= 0.1f) f[i] *= 1.0f + t;
						}

					for (uint n = 0; n < 16; ++n) data[16 * blockIndex + n] = (Int16)(p[n] * (1.0 - f[n]));
					adjust_rate *= 1.1f;

					blockIndex--; // Repeat block with the samples adjusted to prevent overslow
				}
				else
				{
					adjust_rate = base_adjust_rate;
					best_samp[0] = best_samp[16];
					best_samp[1] = best_samp[17];

					total_error += best_err;

					if (best_err < min_error)
						min_error = best_err;

					if (best_err > max_error)
						max_error = best_err;

					brrData.AddRange(best_data);
				}
			}  // wave loop
			brrData[brrData.Count - 9] |= (byte)(1 | (LoopEnd > 0 ? 2 : 0)); // Set end bit
			return brrData.ToArray();
		}

		public static Int16[] DecompressItSample(byte[] buffer, uint length, bool v215, int bitDepth, bool stereo)
		{
			var srcbuf = 0;          // current position in source buffer
			var destpos = 0;                // position in destination buffer which will be returned
			UInt16 blklen;                // length of compressed data block in samples
			UInt16 blkpos;                // position in block
			byte width;                  // actual "bit width"
			UInt32 value;                 // value read from file to be processed
			Int16 d1, d2;                  // integrator buffers (d2 for it2.15)
			Int16 v;                       // sample value
			UInt32 bitbuf, bitnum;        // state for it_readbits

			var maxWidth = bitDepth == 16 ? 17 : 9;
			var data = new List<Int16>();
			// now unpack data till the dest buffer is full
			while (length > 0)
			{
				// read a new block of compressed data and reset variables
				// block layout: word size, <size> bytes data
				if (srcbuf + 2 > buffer.Length || srcbuf + 2 + (buffer[srcbuf] | (buffer[srcbuf + 1] << 8)) > buffer.Length)
				{
					// truncated!
					return null;
				}
				srcbuf += 2;
				bitbuf = bitnum = 0;

				blklen = (UInt16)Math.Min(bitDepth == 16 ? 0x4000 : 0x8000, length);
				blkpos = 0;

				width = (byte)(maxWidth); // start with width of 9 bits
				d1 = d2 = 0; // reset integrator buffers

				// now uncompress the data block
				while (blkpos < blklen)
				{
					if (width > maxWidth) throw new Exception(string.Format("Illegal bit width {0} for 8-bit sample", width));
					value = it_readbits(buffer, width, ref bitbuf, ref bitnum, ref srcbuf);

					if (width < 7)
					{
						// method 1 (1-6 bits)
						// check for "100..."
						if (value == (1 << (width - 1)))
						{
							// yes!
							value = it_readbits(buffer, bitDepth == 16 ? 4 : 3, ref bitbuf, ref bitnum, ref srcbuf) + 1; // read new width
							width = (byte)((value < width) ? value : value + 1); // and expand it
							continue; // ... next value
						}
					}
					else if (width < maxWidth)
					{
						// method 2 (7-8 bits)
						var border = bitDepth == 16 // lower border for width chg
							? (0xFFFF >> (17 - width)) - 8
							: (0xFF >> (9 - width)) - 4;
						if (value > border && value <= (border + bitDepth))
						{
							value -= (UInt32)border; // convert width to 1-8
							width = (byte)((value < width) ? value : value + 1); // and expand it
							continue; // ... next value
						}
					}
					else
					{
						// method 3 (9 bits)
						// bit 8 set?
						if ((value & (bitDepth == 16 ? 0x10000 : 0x100)) != 0)
						{
							width = (byte)((value + 1) & 0xff); // new width...
							continue; // ... and next value
						}
					}

					// now expand value to signed byte
					if (width < bitDepth)
					{
						var shift = (byte)(bitDepth - width);
						v = (Int16)(value << shift);
						v >>= shift;
					}
					else
					{
						v = (Int16)value;
					}

					// integrate upon the sample values
					d1 += v;
					d2 += d1;

					// .. and store it into the buffer
					data.Add((Int16)(v215 ? d2 : d1));
					if (stereo) data.Add(0);
					blkpos++;
				}

				// now subtract block length from total length and go on
				length -= blklen;
			}
			return data.ToArray();
		}
		private static UInt32 it_readbits(byte[] buffer, int size, ref UInt32 bitbuf, ref UInt32 bitnum, ref int ibuf)
		{
			UInt32 value = 0;
			var i = size;

			// this could be better
			while (i-- > 0) {
				if (bitnum == 0) {
					bitbuf = buffer[ibuf];
					ibuf++;
					bitnum = 8;
				}
				value >>= 1;
				value |=  bitbuf << 31;
				bitbuf >>= 1;
				bitnum--;
			}
			return value >> (32 - size);
		}
	}
}