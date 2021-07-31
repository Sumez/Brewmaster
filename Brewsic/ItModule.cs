using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brewsic
{
	public class ItModule
	{
		public string Title { get; set; }
		public byte Speed { get; set; }
		public byte Tempo { get; set; }
		public List<Channel> Channels { get; set; } = new List<Channel>(64);
		public List<int> OrderList { get; set; }
		public Dictionary<int, Instrument> Instruments = new Dictionary<int, Instrument>();
		public List<Sample> Samples = new List<Sample>();
		public List<Pattern> Patterns = new List<Pattern>();
		private int NextMacroId { get; set; }

		private enum ItNote
		{
			NOTE = 1,
			SAMPLE = 2,
			VOLUME = 4,
			EFFECT = 8,
			SAME_NOTE = 16,
			SAME_SAMPLE = 32,
			SAME_VOLUME = 64,
			SAME_EFFECT = 128,
		}
		private static Pattern LoadPattern(FileStream stream)
		{

			var pattern = new Pattern();
			var bytes = ReadUInt16(stream);
			pattern.Rows = ReadUInt16(stream);
			stream.Position += 4;

			var rowNotes = new PatternNote[64];
			var lastNote = new PatternNote[64];
			var lastMask = new int[64];

			var row = 0;
			while (row < pattern.Rows)
			{
				var chanvar = stream.ReadByte();
				if (chanvar == 255 && stream.Position == stream.Length) return pattern;
				if (chanvar == 0)
				{
					for (var i = 0; i < pattern.Channels.Count; i++)
					{
						pattern.Channels[i][row] = rowNotes[i];
					}
					Array.Clear(rowNotes, 0, 64);
					row++;
					continue;
				}
				var channel = (chanvar - 1) & 63;
				while (pattern.Channels.Count < (channel + 1))
				{
					pattern.Channels.Add(new PatternNote[pattern.Rows]);
				}

				int maskvar;
				if ((chanvar & 128) != 0)
				{
					maskvar = stream.ReadByte();
					lastMask[channel] = maskvar;
				}
				else
				{
					maskvar = lastMask[channel];
				}
				var mask = (ItNote)maskvar;
				if (mask.HasFlag(ItNote.NOTE))
				{
					var c = (byte)stream.ReadByte();
					if (c == 255) c = 255; // Note off
					else if (c == 254) c = 254; // Note cut
					else if (c > 119) c = 253; // Note fade
					//else c += 1; // 0-119 = proper frequencies
					rowNotes[channel].HasNote = true;
					lastNote[channel].Note = rowNotes[channel].Note = c;
				}
				if (mask.HasFlag(ItNote.SAMPLE))
				{
					lastNote[channel].Instrument = rowNotes[channel].Instrument = (byte)stream.ReadByte();
				}
				if (mask.HasFlag(ItNote.VOLUME))
				{
					lastNote[channel].VolumeEffect = rowNotes[channel].VolumeEffect = (byte)stream.ReadByte();
				}
				if (mask.HasFlag(ItNote.EFFECT))
				{
					lastNote[channel].Effect = rowNotes[channel].Effect = (byte)(stream.ReadByte() & 0x1f);
					lastNote[channel].EffectParam = rowNotes[channel].EffectParam = (byte)stream.ReadByte();
				}
				if (mask.HasFlag(ItNote.SAME_NOTE))
				{
					rowNotes[channel].HasNote = true;
					rowNotes[channel].Note = lastNote[channel].Note;
				}
				if (mask.HasFlag(ItNote.SAME_SAMPLE))
					rowNotes[channel].Instrument = lastNote[channel].Instrument;
				if (mask.HasFlag(ItNote.SAME_VOLUME))
					rowNotes[channel].VolumeEffect = lastNote[channel].VolumeEffect;
				if (mask.HasFlag(ItNote.SAME_EFFECT))
				{
					rowNotes[channel].Effect = lastNote[channel].Effect;
					rowNotes[channel].EffectParam = lastNote[channel].EffectParam;
				}
			}

			return pattern;
		}

		private static Sample LoadSample(FileStream stream)
		{
			var sample = new Sample();

			var imps = ReadString(stream, 4);
			var filename = ReadString(stream, 13);
			sample.GlobalVolume = stream.ReadByte(); // 0-64
			var flags = stream.ReadByte();
			sample.Volume = stream.ReadByte(); // 0-64
			sample.Name = ReadString(stream, 26).Trim('\0', ' ');
			var cvt = stream.ReadByte();
			sample.Panning = stream.ReadByte(); // 0-64 (32 = center)
			sample.Length = ReadUInt32(stream);
			sample.LoopStart = ReadUInt32(stream);
			sample.LoopEnd = ReadUInt32(stream);
			sample.C5Speed = (double)ReadUInt32(stream);
			var sustainStart = ReadUInt32(stream);
			var sustainEnd = ReadUInt32(stream);
			var samplePointer = ReadUInt32(stream);
			var vibSpeed = stream.ReadByte();
			var vibDepth = stream.ReadByte();
			var vibRate = stream.ReadByte();
			var vibType = stream.ReadByte();

			if ((flags & 1) != 0)
			{
				var stereo = (flags & 4) != 0;
				var bitDepth = (flags & 2) != 0 ? 16 : 8;
				Compression compression;
				if ((flags & 8) != 0)
				{
					compression = (cvt & 4) != 0 ? Compression.IT215 : Compression.IT214;
				}
				else
				{
					compression = (cvt & 4) != 0 ? Compression.PCMD : (cvt & 1) != 0 ? Compression.PCMS : Compression.PCMU;
				}

				stream.Position = samplePointer;
				sample.ConvertedFromStereo = stereo;
				sample.Data = ReadSampleData(stream, sample, compression, bitDepth, stereo);
			}
			else sample.Length = 0;

			return sample;
		}

		private static Int16[] ReadSampleData(FileStream stream, Sample sample, Compression compression, int bitDepth, bool stereo)
		{
			var size = (int)sample.Length;
			if (bitDepth == 16) size *= 2;
			if (stereo) size *= 2;

			var buffer = new byte[size];
			stream.Read(buffer, 0, size);

			switch (compression)
			{
				case Compression.PCMS:
					break;
				case Compression.PCMU:
					if (bitDepth == 8) for (var i = 0; i < size; i++) buffer[i] -= (byte)0x80; // Prepare the bytes for 16bit signed samples
					break;
				default:
					throw new NotImplementedException("Compression type " + compression + " not yet implemented");
			}
			var sampleData = new Int16[sample.Length * (stereo ? 2 : 1)];
			if (bitDepth == 16)
			{
				Buffer.BlockCopy(buffer, 0, sampleData, 0, size);
			}
			else
			{
				var signedData = new sbyte[size];
				Buffer.BlockCopy(buffer, 0, signedData, 0, size);
				for (var i = 0; i < size; i++) sampleData[i] = (Int16)(signedData[i] << 8);
			}
			if (stereo)
			{
				var monoSample = new List<Int16>();
				for (var i = 0; i < sampleData.Length; i += 2)
				{
					if (sampleData.Length > i + 1) monoSample.Add((Int16)((sampleData[i] + sampleData[i+1]) / 2));
					else monoSample.Add(sampleData[i]);
				}

				sampleData = monoSample.ToArray();
				sample.Length = (uint)sampleData.Length;
			}

			return sampleData;
		}

		private enum Compression
		{
			IT214, IT215, PCMD, PCMS, PCMU
		}

		private static UInt32 ReadUInt32(FileStream stream)
		{
			var buffer = new byte[4];
			var int32 = new UInt32[1];
			stream.Read(buffer, 0, 4);
			Buffer.BlockCopy(buffer, 0, int32, 0, 4);
			return int32[0];
		}
		private static UInt16 ReadUInt16(FileStream stream)
		{
			var buffer = new byte[2];
			var int16 = new UInt16[1];
			stream.Read(buffer, 0, 2);
			Buffer.BlockCopy(buffer, 0, int16, 0, 2);
			return int16[0];
		}

		private static string ReadString(FileStream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return System.Text.Encoding.ASCII.GetString(buffer);
		}

		private static Instrument LoadInstrument(FileStream stream)
		{
			var buffer = new byte[570];
			var int16 = new UInt16[1];
			stream.Read(buffer, 0, buffer.Length);
			var impi = new byte[4];
			var filename = new byte[13];
			var name = new byte[26];

			Buffer.BlockCopy(buffer, 0, impi, 0, 4);
			Buffer.BlockCopy(buffer, 4, filename, 0, 13);

			if (System.Text.Encoding.ASCII.GetString(impi) != "IMPI") throw new Exception("Invalid IT instrument");

			var nna = buffer[17];
			var dct = buffer[18];
			var dca = buffer[19];

			Buffer.BlockCopy(buffer, 20, int16, 0, 2);
			var fadeout = int16[0];

			sbyte pps = (sbyte)buffer[22];
			var ppc = buffer[23];
			var gbv = buffer[24];
			var dfp = buffer[25];
			var rv = buffer[26];
			var rp = buffer[27];

			Buffer.BlockCopy(buffer, 28, int16, 0, 2);
			var trkvers = int16[0];

			var sampleCount = buffer[30];
			var padding = buffer[31];

			Buffer.BlockCopy(buffer, 32, name, 0, 26);

			var ifc = buffer[58];
			var ifr = buffer[59];
			var mch = buffer[60];
			var mpr = buffer[61];

			Buffer.BlockCopy(buffer, 62, int16, 0, 2);
			var midiBank = int16[0];

			var sampleMapping = new byte[240];
			Buffer.BlockCopy(buffer, 64, sampleMapping, 0, 240);

			var instrument = new Instrument();
			instrument.Name = Encoding.ASCII.GetString(name).Trim('\0', ' ');
			instrument.Sample = sampleMapping[121];
			instrument.Sample--; // Sample map uses 1-index
			instrument.FadeOut = (byte)fadeout;
			instrument.Volume = gbv;

			var envelope = new byte[82];
			Buffer.BlockCopy(buffer, 304, envelope, 0, 82);
			instrument.VolumeEnvelope = LoadEnvelope(envelope);
			Buffer.BlockCopy(buffer, 386, envelope, 0, 82);
			Buffer.BlockCopy(buffer, 468, envelope, 0, 82);


			for (var i = 0; i < 120; i++)
			{
				instrument.NoteMap.Add(i, sampleMapping[i * 2]);
			}

			if (fadeout > 255)
			{
				throw new Exception(string.Format("Instrument \"{0}\" FadeOut setting uses incorrect format", instrument.Name));
			}

			return instrument;
		}

		private class EnvelopeNode
		{
			public int Volume;
			public int TickNumber;
		}
		private static byte[] LoadEnvelope(byte[] data)
		{
			var flag = data[0];
			if ((flag & 1) == 0) return null; // Flag 1 = no envelope
			var loop = (flag & 2) != 0;
			var length = (int)data[1];
			var loopStart = data[2];
			var loopEnd = data[3];
			if (loop && loopEnd < length) length = loopEnd + 1;
			if (loopStart > loopEnd) loop = false;

			// TODO: Support sustain loops

			var nodes = new List<EnvelopeNode>();
			for (var i = 6; i < data.Length - 2; i += 3)
			{
				nodes.Add(new EnvelopeNode { Volume = data[i] * 2, TickNumber = data[i + 1] | (data[i + 2] << 8) });
			}
			// Interpolate the envelope curve into the data, so the SPC driver doesn't have to bother with it.
			var output = new List<byte>();
			output.Add((byte)nodes[0].Volume);
			for (var i = 1; i < length; i++)
			{
				var distance = nodes[i].TickNumber - nodes[i - 1].TickNumber;
				if (distance < 1) throw new Exception("Invalid envelope data");
				var delta = nodes[i].Volume - nodes[i - 1].Volume;
				if (distance >= 0x80) throw new NotImplementedException("Envelope step is too damn long.");
				var step = (byte)Math.Round((float)delta / distance);

				output.Add((byte)distance);
				output.Add(step);
			}

			if (loop)
			{
				if (loopStart == loopEnd)
				{
					output.Add(0x80);
				}
				else
				{
					output.Add(1);
					output.Add((byte) (nodes[loopStart].Volume - nodes[length - 1].Volume));
					output.Add((byte) (0x80 | (loopStart * 2 + 1)));
				}
			}
			else output.Add(0);

			return output.ToArray();
		}

		private static Instrument LoadInstrumentOld(FileStream stream)
		{
			throw new NotImplementedException();
		}

		private static List<uint> Read32BitBuffer(Stream stream, int count)
		{
			var buffer = new byte[count * 4];
			var array = new UInt32[count];
			stream.Read(buffer, 0, count * 4);
			Buffer.BlockCopy(buffer, 0, array, 0, count * 4);
			return array.ToList();
		}

		private readonly Dictionary<int, int> _noteProgressionStats = new Dictionary<int, int>();
		private static readonly Dictionary<int, int> NoteProgressionMacros = new Dictionary<int, int>
		{
			{0, 59},
			{12, 58},
			{-12, 57},
			{-1, 56},
			{-2, 55},
			{-3, 54}
		};

		private class PatternMacro
		{
			private readonly ItModule _module;

			public PatternMacro(ItModule module, IEnumerable<PatternNote> pattern)
			{
				_module = module;
				Pattern = pattern.ToArray();
				Data = _module.GetCompressedPatternData(Pattern);
				Value = Data.Length - 1; // Each macro usage has one byte of overhead
				BytesSaved = -Data.Length; // Each macro has an overall overhead of its own size (obviously)
			}

			public int? Id { get; set; }
			public byte[] Data { get; private set; }
			public PatternNote[] Pattern { get; private set; }
			public int Value { get; private set; }

			public int BytesSaved;
			public readonly Dictionary<PatternNote[], int> PatternMatch = new Dictionary<PatternNote[], int>();
			public readonly Dictionary<PatternNote[], int> PatternStart = new Dictionary<PatternNote[], int>();
		}
		private List<PatternMacro> GetPatternMacros(int macroStep)
		{
			// Get a list of unique patterns that exist in the order list
			var allPatterns = new List<PatternNote[]>();
			for (var i = 0; i < Patterns.Count; i++)
			{
				if (!OrderList.Contains(i)) continue;
				foreach (var channelPattern in Patterns[i].Channels)
				{
					if (allPatterns.Any(p => p.SequenceEqual(channelPattern))) continue;
					allPatterns.Add(channelPattern);
				}
			}
			var macros = new List<PatternMacro>();
			var largestRowLength = allPatterns.Max(p => p.Length);
			for (var macroLength = macroStep; macroLength <= largestRowLength / 2; macroLength += macroStep)
			{
				foreach (var pattern in allPatterns)
				{
					for (var i = 0; i < pattern.Length - macroLength; i += macroLength)
					{
						var subPattern = pattern.Skip(i).Take(macroLength);
						if (!macros.Any(m => m.Pattern.SequenceEqual(subPattern))) macros.Add(new PatternMacro(this, subPattern));
					}
				}
			}

			foreach (var macro in macros)
			{
				var macroLength = macro.Pattern.Length;
				foreach (var pattern in allPatterns)
				{
					for (var i = 0; i < pattern.Length - macroLength; i++)
					{
						if (pattern.Skip(i).Take(macroLength).SequenceEqual(macro.Pattern))
						{
							if (!macro.PatternMatch.ContainsKey(pattern))
							{
								macro.PatternMatch.Add(pattern, 0);
								macro.PatternStart[pattern] = i;
							}
							macro.PatternMatch[pattern] += macro.Value;
							macro.BytesSaved += macro.Value;
							i += macroLength;
						}
					}
				}
			}

			var bestMacros = macros.OrderByDescending(m => m.BytesSaved).Take(64).ToList();
			return bestMacros;
		}

		public byte[] GetCompressedPatternData()
		{
			var macroStep = 0;
			var data = GetCompressedPatternData(new List<PatternMacro>());
			var smallestSize = data.Length;
			while (true)
			{
				// Attempt various macro settings and use the data with the best results (smallest file)
				macroStep += 4;
				NextMacroId = 0;
				var patternMacros = GetPatternMacros(macroStep);
				if (!patternMacros.Any()) break;
				var testData = GetCompressedPatternData(patternMacros);
				if (testData.Length >= smallestSize) continue;

				smallestSize = testData.Length;
				data = testData;
			}
			return data;
		}

		private byte[] GetCompressedPatternData(List<PatternMacro> patternMacros)
		{
			var data = new List<byte>();
			var header = new byte[18];
			header[0] = Speed;
			header[1] = (byte)Math.Round(10000f / Tempo);
			for (var i = 0; i < 8; i++)
			{
				header[i*2 + 2] = Channels[i].Volume;
				header[i*2 + 3] = Channels[i].Panning;
			}
			var orderReferences = new UInt16[OrderList.Count * 8 + 1];
			var orderPatternSizes = new byte[OrderList.Count + 1];

			var patternAddress = orderReferences.Length * 2 + orderPatternSizes.Length; // Address counts from end of header

			var patternMap = new Dictionary<int, int>();
			var patternAddressMap = new Dictionary<int, int>();
			var channelPatterns = new List<byte[]>();
			var processedPatterns = new List<PatternNote[]>();
			for (var j = 0; j < Patterns.Count; j++)
			{
				if (!OrderList.Contains(j)) continue; // Don't store unused patterns
				var patternIndex = j * 8;
				var pattern = Patterns[j];
				for (var i = 0; i < 8; i++)
				{
					var patternChannel = i >= pattern.Channels.Count ? null : pattern.Channels[i];
					var existing = patternChannel != null ? processedPatterns.FindIndex(d => d != null && d.SequenceEqual(patternChannel)) : -1;
					if (existing >= 0)
					{
						patternMap[patternIndex + i] = existing;
					}
					else
					{
						byte[] patternData = patternChannel != null ? GetCompressedPatternData(patternChannel, patternMacros) : GetSilentPattern(pattern);

						patternMap[patternIndex + i] = channelPatterns.Count;
						patternAddressMap[channelPatterns.Count] = patternAddress;
						patternAddress += patternData.Length;

						processedPatterns.Add(patternChannel);
						channelPatterns.Add(patternData);
					}
				}
			}
			var instrumentData = new List<byte>();
			for (var i = 0; i <= Instruments.Keys.Max(); i++) // Iterate through instruments once just to determine their block size
			{
				// TODO: Skip unused instruments
				if (!Instruments.ContainsKey(i)) Instruments.Add(i, new Instrument());
				instrumentData.AddRange(GetInstrumentData(Instruments[i]));
			}

			var macroAddress = patternAddress;
			var macroDirectory = new List<UInt16>();
			var macroData = new List<byte>();
			foreach (var macro in patternMacros.Where(m => m.Id.HasValue).OrderBy(m => m.Id))
			{
				macroData.AddRange(macro.Data);
				macroData.Add(0xff);
				macroDirectory.Add((UInt16)macroAddress);
				macroAddress += macro.Data.Length + 1;
			}
			macroDirectory.Add(0xffff);

			var stats = _noteProgressionStats.ToList().OrderByDescending(kvp => kvp.Value);
			for (var i = 0; i < OrderList.Count; i++)
			{
				if (OrderList[i] >= Patterns.Count) continue; // Skip invalid order data

				var patternId = OrderList[i] * 8;
				for (var j = 0; j < 8; j++)
				{
					orderReferences[i * 8 + j] = (UInt16)patternAddressMap[patternMap[patternId + j]];
				}
				if (Patterns[OrderList[i]].Rows > 255) throw new Exception(string.Format("T0o many rows in pattern #{0}. Maximum is 255", OrderList[i]));
				orderPatternSizes[i] = (byte)Patterns[OrderList[i]].Rows;
			}
			orderReferences[OrderList.Count * 8] = 0xffff; // Indicates end of pattern references (address can't be 0xffff)
			orderPatternSizes[OrderList.Count] = 0x00; // Indicates end of pattern sizes (size can't be 0)

			// Fix address offsets to account for header data
			var offset = (UInt16)(macroDirectory.Count * 2 + instrumentData.Count);
			for (var i = 0; i < orderReferences.Length - 1; i++) orderReferences[i] += offset;
			for (var i = 0; i < macroDirectory.Count - 1; i++) macroDirectory[i] += offset;

			var envelopeAddress = macroAddress + offset;
			var envelopeData = new List<byte>();
			foreach (var instrument in Instruments.Values.Where(i => i.VolumeEnvelope != null))
			{
				envelopeData.AddRange(instrument.VolumeEnvelope);
				instrument.VolumeEnvelopeAddress = (UInt16)envelopeAddress;
				envelopeAddress += instrument.VolumeEnvelope.Length;
			}
			instrumentData.Clear();
			for (var i = 0; i <= Instruments.Keys.Max(); i++) // Iterate through instruments again to get data with envelope addresses
			{
				instrumentData.AddRange(GetInstrumentData(Instruments[i]));
			}

			var orderData = new byte[orderReferences.Length * 2];
			Buffer.BlockCopy(orderReferences, 0, orderData, 0, orderData.Length);
			var macroDirectoryData = new byte[macroDirectory.Count * 2];
			Buffer.BlockCopy(macroDirectory.ToArray(), 0, macroDirectoryData, 0, macroDirectoryData.Length);

			data.AddRange(header);
			data.AddRange(orderData);
			data.AddRange(orderPatternSizes);
			data.AddRange(macroDirectoryData);
			data.AddRange(instrumentData);
			foreach (var patternData in channelPatterns) data.AddRange(patternData);
			data.AddRange(macroData);
			data.AddRange(envelopeData);

			return data.ToArray();
		}

		public byte[] GetInstrumentData(Instrument instrument)
		{
			var data = new byte[8];
			if (instrument.Sample >= Samples.Count) return data; // TODO: Remove invalid instruments and adjust instrument index references

			var sample = Samples[instrument.Sample];
			var playbackRate = sample.C5Speed / 8363; // 8363 means C-5 plays at a $042E pitch (~8khz)
			var pitchAdjust = (int)Math.Round(768 * Math.Log(playbackRate) / Math.Log(2)); // Adjust pitch by this number to get the correct playback rate
																						   //var volume = Math.Min(255, Math.Round(instrument.Volume * sample.Volume / 32f)); // Instrument global volume is 0-128, but sample vol is 0-64, adjust to 0-255
			var volume = Math.Min(255, Math.Round(instrument.Volume * sample.Volume / 128f)); // Instrument global volume is 0-128, but sample vol is 0-64, adjust combined range to 0-64

			data[0] = instrument.Sample;
			data[1] = (byte)(pitchAdjust & 0xff);
			data[2] = (byte)((pitchAdjust >> 8) & 0xff);
			data[3] = instrument.FadeOut;
			data[4] = (byte)volume;
			data[5] = (byte)(instrument.VolumeEnvelopeAddress & 0xff);
			data[6] = (byte)((instrument.VolumeEnvelopeAddress >> 8) & 0xff);

			return data;
		}

		private byte[] GetSilentPattern(Pattern pattern)
		{
			var data = new List<byte>();
			var rows = pattern.Rows;
			while (rows > 128)
			{
				data.Add(0x80);
				rows -= 128;
			}
			data.Add((byte)(0x80 | rows));
			return data.ToArray();
		}

		private int GetNextGoodMacroMatch(PatternNote[] pattern, List<PatternMacro> candidatesSortedByPriority, int startIndex)
		{
			foreach (var macro in candidatesSortedByPriority)
			{
				for (var i = startIndex; macro.Pattern.Length <= pattern.Length - i; i++)
				{
					var subPattern = pattern.Skip(i).Take(macro.Pattern.Length);
					if (macro.Pattern.SequenceEqual(subPattern)) return i;
				}
			}
			return pattern.Length;
		}
		private byte[] GetCompressedPatternData(PatternNote[] pattern, List<PatternMacro> patternMacros = null)
		{
			if (patternMacros == null) patternMacros = new List<PatternMacro>();
			patternMacros = patternMacros.Where(m => m.PatternMatch.ContainsKey(pattern)).OrderByDescending(m => m.PatternMatch[pattern]).ToList();
			var firstMacroRow = GetNextGoodMacroMatch(pattern, patternMacros, 0);

			var lastNote = -1;
			var lastInstrument = -1;

			var data = new List<byte>();
			var clearCount = 0;
			var currentBlock = new List<byte>();
			var currentBlockRows = 0;

			void FlushBlock()
			{
				if (currentBlock.Count > 0)
				{
					data.Add((byte)(currentBlockRows - 1));
					data.AddRange(currentBlock);
					currentBlock.Clear();
					currentBlockRows = 0;
				}
			}
			void CheckBlockOverflow()
			{
				if (currentBlockRows == 64)
				{
					data.Add(63);
					data.AddRange(currentBlock);
					currentBlock.Clear();
					currentBlockRows = 0;
				}
			}

			for (var i = 0; i < pattern.Length; i++)
			{
				var usedMacro = false;
				if (i >= firstMacroRow)
				{
					foreach (var macro in patternMacros)
					{
						if (macro.Pattern.Length > pattern.Length - i) continue;

						var subPattern = pattern.Skip(i).Take(macro.Pattern.Length);
						if (!macro.Pattern.SequenceEqual(subPattern)) continue;

						FlushBlock();
						if (clearCount > 0) data.Add((byte)(0x80 | clearCount));
						clearCount = 0;

						if (!macro.Id.HasValue)
						{
							macro.Id = NextMacroId;
							NextMacroId++;
						}
						data.Add((byte)(0x40 | macro.Id.Value));
						i += macro.Pattern.Length;
						firstMacroRow = GetNextGoodMacroMatch(pattern, patternMacros, i);
						i--;
						usedMacro = true;
						break;
					}

					if (usedMacro) continue;
				}

				var note = pattern[i];
				if (!note.HasNote && note.Effect == 0 && note.VolumeEffect == 0)
				{
					if (clearCount == 128)
					{
						FlushBlock();
						data.Add(0x80);
						clearCount = 0;
					}

					clearCount++;
					continue;
				}
				if (clearCount > 2)
				{
					FlushBlock();
					data.Add((byte)(0x80 | clearCount));
					clearCount = 0;
				}
				while (clearCount > 0)
				{
					currentBlock.Add(63);
					currentBlockRows++;
					CheckBlockOverflow();
					clearCount--;
				}
				var instrumentId = note.Instrument - 1;
				if (instrumentId < 0)
				{
					instrumentId = lastInstrument;
					if (instrumentId < 0 && note.Note < 253) note.HasNote = false; // Avoid playing notes with no instrument
				}
				if (instrumentId >= 54) throw new Exception("Maximum of 53 instruments supported in a track, sorry");

				var noteByte = note.Note;
				if (!note.HasNote) instrumentId = 63; // none
				else
				{
					if (noteByte == 255) instrumentId = 62; // off
					else if (noteByte == 254) instrumentId = 60; // cut
					else if (noteByte == 253) instrumentId = 61; // fade
					else
					{
						if (!Instruments.ContainsKey(instrumentId))
						{
							// TODO: Sample mode (if instrument doesn't exist)
							Instruments.Add(instrumentId, new Instrument { Sample = (byte)instrumentId, Volume = Samples[instrumentId].GlobalVolume });
						}

						try
						{
							var instrument = Instruments[instrumentId];
							noteByte = instrument.NoteMap.ContainsKey(noteByte) ? instrument.NoteMap[noteByte] : noteByte;
						}
						catch (Exception ex)
						{

						}

						var delta = noteByte - lastNote;
						//if (!_noteProgressionStats.ContainsKey(delta)) _noteProgressionStats.Add(delta, 0);
						//_noteProgressionStats[delta]++;
						if (instrumentId == lastInstrument && NoteProgressionMacros.ContainsKey(delta))
						{
							instrumentId = NoteProgressionMacros[delta];
							lastNote = noteByte;
						}
						else
						{
							lastInstrument = instrumentId;
							lastNote = noteByte;
						}
					}
				}

				var headerByte = (note.Effect > 0 ? 0x80 : 0) | (note.VolumeEffect > 0 ? 0x40 : 0) | instrumentId;

				currentBlock.Add((byte)headerByte);
				if (note.VolumeEffect > 0) currentBlock.Add(note.VolumeEffect);
				if (note.Effect > 0) currentBlock.Add(note.Effect);
				if (note.Effect > 0) currentBlock.Add(note.EffectParam);
				if (instrumentId < 54) currentBlock.Add(noteByte);

				currentBlockRows++;
				CheckBlockOverflow();
			}
			FlushBlock();
			if (clearCount > 0) data.Add((byte)(0x80 | clearCount));

			return data.ToArray();
		}

		public static ItModule LoadFromStream(FileStream stream, Action<string> output = null)
		{
			var module = new ItModule();

			var buffer = new byte[192];
			stream.Read(buffer, 0, 192);
			var impm = new byte[4];
			var title = new byte[26];

			var int16 = new UInt16[9];
			var int32 = new UInt32[2];
			var chanPan = new byte[64];
			var chanVolume = new byte[64];
			Buffer.BlockCopy(buffer, 0, impm, 0, 4);
			Buffer.BlockCopy(buffer, 4, title, 0, 26);

			Buffer.BlockCopy(buffer, 32, int16, 0, 16);
			Buffer.BlockCopy(buffer, 54, int16, 16, 2);

			Buffer.BlockCopy(buffer, 56, int32, 0, 8);

			Buffer.BlockCopy(buffer, 64, chanPan, 0, 64);
			Buffer.BlockCopy(buffer, 128, chanVolume, 0, 64);

			int highlightMinor = buffer[30];
			int highlightMajor = buffer[31];

			int orderCount = int16[0];
			int instrumentCount = int16[1];
			int sampleCount = int16[2];
			int patternCount = int16[3];

			int cwtv = int16[4];
			int cmwt = int16[5];
			int flags = int16[6];
			int special = int16[7];

			int generalVolume = buffer[48];
			int mixingVolume = buffer[49];
			int panSeparation = buffer[52];
			int pwd = buffer[53];

			int msgLength = int16[8];
			uint msgOffset = int32[0];
			uint reserved = int32[1];

			if (Encoding.ASCII.GetString(impm) != "IMPM") throw new Exception("Invalid Impulse Tracker module");
			module.Title = Encoding.ASCII.GetString(title).Trim('\0', ' ');
			module.Speed = buffer[50];
			module.Tempo = buffer[51];

			for (var i = 0; i < 64; i++)
			{
				module.Channels.Add(new Channel
				{
					Panning = Math.Min((byte)64, chanPan[i]),
					Volume = Math.Min((byte)64, chanVolume[i])
				});
			}

			var orders = new byte[orderCount];
			stream.Read(orders, 0, orderCount);
			module.OrderList = orders.Where(o => o < 255).Select(o => (int)o).ToList();
			var instrumentPointers = Read32BitBuffer(stream, instrumentCount);
			var samplePointers = Read32BitBuffer(stream, sampleCount);
			var patternPointers = Read32BitBuffer(stream, patternCount);

			UInt16 historySize = 0;
			if ((special & 2) != 0)
			{
				var histBuffer = new byte[2];
				stream.Read(histBuffer, 0, 2);
				Buffer.BlockCopy(histBuffer, 0, int16, 0, 2);
				historySize = int16[0];
			}
			if (historySize > 0)
			{
				var histBuffer = new byte[historySize * 8];
				stream.Read(histBuffer, 0, historySize * 8);
			}

			for (var i = 0; i < instrumentPointers.Count; i++)
			{
				if (instrumentPointers[i] == 0) continue;
				stream.Position = instrumentPointers[i];
				module.Instruments.Add(i, cmwt >= 0x0200 ? LoadInstrument(stream) : LoadInstrumentOld(stream));
			}
			foreach (var pointer in samplePointers.Where(p => p > 0))
			{
				stream.Position = pointer;
				module.Samples.Add(LoadSample(stream));
			}

			if (module.Samples.Any(s => s.ConvertedFromStereo) && output != null)
				output("Warning: Module uses stereo samples. It's a good idea to convert these to mono beforehand for better results");

			while (patternPointers[patternPointers.Count - 1] == 0) patternPointers.RemoveAt(patternPointers.Count - 1); // Trim unused patterns in the end
			foreach (var pointer in patternPointers)
			{
				if (pointer == 0)
				{
					module.Patterns.Add(new Pattern(64));
					continue;
				}
				stream.Position = pointer;
				module.Patterns.Add(LoadPattern(stream));
			}

			return module;
		}

		public static ItModule LoadFromFile(string fileName, Action<string> output = null)
		{
			using (var stream = File.OpenRead(fileName)) return LoadFromStream(stream, output);
		}
	}
}