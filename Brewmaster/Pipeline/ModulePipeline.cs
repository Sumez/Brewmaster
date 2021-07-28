using Brewmaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brewmaster.Pipeline
{
	public class ModulePipeline : PipelineOption
	{
		public override string TypeName => "tracker.module";

		public override IEnumerable<FileType> SupportedFileTypes => new[] { FileType.Audio };

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".bin");
		}

		public override void Process(PipelineSettings settings, Action<string> output)
		{
			var modules = new List<ItModule> { ItModule.LoadFromFile(settings.File.File.FullName) };
			output(string.Format("Converting song \"{0}\" to Brewsic music format", modules[0].Title));

			var sampleDirectoryLength = (byte)(modules[0].Samples.Count * 2);
			var trackDirectoryLength = 1;

			var sampleDirectory = new List<int>(sampleDirectoryLength);
			var trackDirectory = new List<int>(trackDirectoryLength);
			var baseAddress = 0x1000;
			var sampleAddress = baseAddress + sampleDirectoryLength * 2 + trackDirectoryLength * 2;
			var sampleData = new List<byte>();
			var trackData = new List<byte>();

			foreach (var sample in modules[0].Samples)
			{
				var filename = sample.Name;
				foreach (var ch in Path.GetInvalidFileNameChars())
				{
					filename = filename.Replace(ch.ToString(), "");
				}
				/*using (var stream2 = new MemoryStream())
				{
					var data = new byte[sample.Data.Length * 2];
					Buffer.BlockCopy(sample.Data, 0, data, 0, data.Length);
					WriteWavHeader(stream2, false, 1, 16, 32000, data.Length);
					stream2.Write(data, 0, data.Length);
					var waveBytes = new byte[stream2.Length];
					stream2.Position = 0;
					stream2.Read(waveBytes, 0, waveBytes.Length);
					File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".wav", waveBytes);
				}*/
				var brr = sample.GetBrr(500, output);
				//File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".brr", brr);

				sampleDirectory.Add(sampleAddress); // Start address
				sampleDirectory.Add(sampleAddress); // Loop address

				sampleAddress += brr.Length;
				sampleData.AddRange(brr);
			}



			var trackAddress = sampleAddress;
			foreach (var module in modules)
			{
				var compressedTrack = module.GetCompressedPatternData();
				trackDirectory.Add(trackAddress);
				trackData.AddRange(compressedTrack);
			}
			output(string.Format("Sample data size: {0} bytes", sampleData.Count));
			output(string.Format("Track data size: {0} bytes", trackData.Count));

			var sampleDirectoryData = GetAsByteArray(sampleDirectory);
			var trackDirectoryData = GetAsByteArray(trackDirectory);
			using (var bankStream = File.Create(settings.GetFilePath(0)))
			{
				bankStream.WriteByte(sampleDirectoryLength);
				bankStream.Write(sampleDirectoryData, 0, sampleDirectoryData.Length);
				bankStream.Write(trackDirectoryData, 0, trackDirectoryData.Length);
				bankStream.Write(sampleData.ToArray(), 0, sampleData.Count);
				bankStream.Write(trackData.ToArray(), 0, trackData.Count);

				bankStream.Close();
			}
		}

		private static byte[] GetAsByteArray(IReadOnlyList<int> list)
		{
			var array = new byte[list.Count * 2];
			for (var i = 0; i < list.Count; i++)
			{
				var value = list[i];
				if (value > 0xffff) throw new Exception("Module is too large to store in 64kb");
				array[i * 2] = (byte)(value & 0xff);
				array[i * 2 + 1] = (byte)((value >> 8) & 0xff);
			}
			return array;
		}

		private void WriteWavHeader(MemoryStream stream, bool isFloatingPoint, int channelCount, int bitDepth, int sampleRate, int size)
		{
			stream.Position = 0;
			stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
			// Chunk size.
			stream.Write(BitConverter.GetBytes(size + 36), 0, 4);
			stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

			// Sub-chunk 1.
			stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
			// Sub-chunk 1 size.
			stream.Write(BitConverter.GetBytes(16), 0, 4);
			// Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
			stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);
			// Channels.
			stream.Write(BitConverter.GetBytes(channelCount), 0, 2);
			// Sample rate.
			stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
			// Bytes rate.
			stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);
			// Block align.
			stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);
			// Bits per sample.
			stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);

			// Sub-chunk 2.
			stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
			// Sub-chunk 2 size.
			stream.Write(BitConverter.GetBytes(size), 0, 4);
		}
	}

	public class ItModule
	{
		public string Title { get; set; }
		public List<Channel> Channels { get; set; } = new List<Channel>(64);
		public List<int> OrderList { get; set; }
		public List<Instrument> Instruments = new List<Instrument>();
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
			sample.Name = ReadString(stream, 26).Trim('\0');
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
			if (stereo)
			{
				// TODO: Interpolate channels ((L + R) >> 1)
				throw new NotImplementedException("Stereo samples not currently supported. Tell Sumez to implement stereo->mono conversion, or just use mono samples");
			}
			var sampleData = new Int16[sample.Length];
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

			var envelope = new byte[82];
			Buffer.BlockCopy(buffer, 304, envelope, 0, 82);
			Buffer.BlockCopy(buffer, 386, envelope, 0, 82);
			Buffer.BlockCopy(buffer, 468, envelope, 0, 82);


			var instrument = new Instrument();
			instrument.Name = System.Text.Encoding.ASCII.GetString(name);
			instrument.Sample = sampleMapping[121];
			instrument.Sample--; // Sample map uses 1-index
			instrument.FadeOut = (byte)fadeout;

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
			var orderReferences = new UInt16[OrderList.Count * 8 + 1];
			var orderPatternSizes = new byte[OrderList.Count + 1];

			var instrumentData = new List<byte>();
			for (var i = 0; i < Instruments.Count; i++)
			{
				instrumentData.AddRange(GetInstrumentData(Instruments[i]));
			}

			var patternAddress = orderReferences.Length * 2 + orderPatternSizes.Length + instrumentData.Count;

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
					var existing = patternChannel != null ? processedPatterns.FindIndex(d => d.SequenceEqual(patternChannel)) : -1;
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

						if (patternChannel != null) processedPatterns.Add(patternChannel);
						channelPatterns.Add(patternData);
					}
				}
			}

			var macroAddress = patternAddress;
			var macroDirectory = new List<UInt16>();
			var macroData = new List<byte>();
			foreach (var macro in patternMacros.Where(m => m.Id.HasValue).OrderBy(m => m.Id))
			{
				macroData.AddRange(macro.Data);
				macroDirectory.Add((UInt16)macroAddress);
				macroAddress += macro.Data.Length;
			}
			macroDirectory.Add(0xffff);

			var stats = _noteProgressionStats.ToList().OrderByDescending(kvp => kvp.Value);
			for (var i = 0; i < OrderList.Count; i++) {
				var patternId = OrderList[i] * 8;
				for (var j = 0; j < 8; j++)
				{
					orderReferences[i * 8 + j] = (UInt16)patternAddressMap[patternMap[patternId + j]];
				}
				if (Patterns[OrderList[i]].Rows > 255) throw new Exception(string.Format("Two many rows in pattern #{0}. Maximum is 255", OrderList[i]));
				orderPatternSizes[i] = (byte)Patterns[OrderList[i]].Rows;
			}
			orderReferences[OrderList.Count * 8] = 0xffff; // Indicates end of pattern references (address can't be 0xffff)
			orderPatternSizes[OrderList.Count] = 0x00; // Indicates end of pattern sizes (size can't be 0)

			// Fix address offsets to account for header data
			var offset = (UInt16)(macroDirectory.Count * 2);
			for (var i = 0; i < orderReferences.Length - 1; i++) orderReferences[i] += offset;
			for (var i = 0; i < macroDirectory.Count - 1; i++) macroDirectory[i] += offset;

			var orderData = new byte[orderReferences.Length * 2];
			Buffer.BlockCopy(orderReferences, 0, orderData, 0, orderData.Length);
			var macroDirectoryData = new byte[macroDirectory.Count * 2];
			Buffer.BlockCopy(macroDirectory.ToArray(), 0, macroDirectoryData, 0, macroDirectoryData.Length);

			data.AddRange(orderData);
			data.AddRange(orderPatternSizes);
			data.AddRange(instrumentData);
			foreach (var patternData in channelPatterns) data.AddRange(patternData);

			
			return data.ToArray();
		}

		public byte[] GetInstrumentData(Instrument instrument)
		{
			var data = new byte[4];
			if (instrument.Sample >= Samples.Count) return data; // TODO: Remove invalid instruments and adjust instrument index references

			var sample = Samples[instrument.Sample];
			var playbackRate = sample.C5Speed / 8363; // 8363 means C-5 plays at a $042E pitch (~8khz)
			var pitchAdjust = (int)Math.Round(768 * Math.Log(playbackRate) / Math.Log(2)); // Adjust pitch by this number to get the correct playback rate

			data[0] = instrument.Sample;
			data[1] = (byte)(pitchAdjust & 0xff);
			data[2] = (byte)((pitchAdjust >> 8) & 0xff);
			data[3] = instrument.FadeOut;
			
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
					if (currentBlockRows > 64) throw new NotImplementedException("Handle blocks longer than 64 rows");
					data.Add((byte)(currentBlockRows - 1));
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
						if (clearCount > 0) data.Add((byte) (0x80 | clearCount));
						clearCount = 0;

						if (!macro.Id.HasValue) macro.Id = NextMacroId;
						NextMacroId++;
						data.Add((byte) (0x40 | macro.Id.Value));
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
					clearCount--;
				}
				var instrumentId = note.Instrument - 1;
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
						if (Instruments.Count <= instrumentId)
						{
							// TODO: Sample mode (if instrument doesn't exist)
							throw new Exception(string.Format("Invalid instrument reference ({0})", instrumentId));
						}
						var instrument = Instruments[instrumentId];
						noteByte = instrument.NoteMap[noteByte];

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
			}
			FlushBlock();
			if (clearCount > 0) data.Add((byte)(0x80 | clearCount));

			return data.ToArray();
		}

		public static ItModule LoadFromStream(FileStream stream)
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
			int initialSpeed = buffer[50];
			int initialTempo = buffer[51];
			int panSeparation = buffer[52];
			int pwd = buffer[53];

			int msgLength = int16[8];
			uint msgOffset = int32[0];
			uint reserved = int32[1];

			if (Encoding.ASCII.GetString(impm) != "IMPM") throw new Exception("Invalid Impulse Tracker module");
			module.Title = Encoding.ASCII.GetString(title).Trim('\0');

			for (var i = 0; i < 64; i++)
			{
				module.Channels.Add(new Channel
				{
					Panning = Math.Min(64, (int)chanPan[i]),
					Volume = Math.Min(64, (int)chanVolume[i])
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

			foreach (var pointer in instrumentPointers.Where(p => p > 0))
			{
				stream.Position = pointer;
				module.Instruments.Add(cmwt >= 0x0200 ? LoadInstrument(stream) : LoadInstrumentOld(stream));
			}
			foreach (var pointer in samplePointers.Where(p => p > 0))
			{
				stream.Position = pointer;
				module.Samples.Add(LoadSample(stream));
			}
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

		public static ItModule LoadFromFile(string fileName)
		{
			using (var stream = File.OpenRead(fileName)) return LoadFromStream(stream);
		}
	}
	public class Channel
	{
		public int Panning { get; internal set; }
		public int Volume { get; internal set; }
	}
	public class Instrument
	{
		public string Name { get; set; }
		public byte Sample { get; set; }
		public byte FadeOut { get; set; }
		public Dictionary<int, byte> NoteMap { get; set; } = new Dictionary<int, byte>();
	}

	public struct PatternNote
	{
		public bool HasNote;
		public byte Note;
		public byte Instrument;
		public byte Effect;
		public byte EffectParam;
		public byte VolumeEffect;

		public override int GetHashCode()
		{
			return (int)Note | (Instrument << 8) | (Effect << 16) | (EffectParam << 24) | (VolumeEffect << 32) | (HasNote ? (1 << 40) : 0);
		}
	}
	public class Pattern
	{
		public Pattern() {}

		/// <summary>
		/// Creates an empty pattern with a set number of rows
		/// </summary>
		/// <param name="rows"></param>
		public Pattern(UInt16 rows)
		{
			Rows = rows;
		}

		public ushort Rows { get; set; }
		public List<PatternNote[]> Channels { get; set; } = new List<PatternNote[]>();
	}
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

		private Int16[] ResampleLoop(uint samplesToAdd)
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

			return data;
		}


		private Int16[] UnrollLoop(uint startAlign, uint endAlign)
		{
			var data = new Int16[LoopEnd + endAlign];
			Array.Copy(Data, data, Math.Min(Data.Length, LoopEnd));

			for (var i = 0; i < endAlign; i++) data[LoopEnd + i] = data[LoopStart + i];

			// 16-sample align loop_start
			LoopStart += startAlign;
			return data;
		}
		
		public byte[] GetBrr(int maximumSampleGrowth = 500, Action<string> output = null)
		{
			var brrData = new List<byte>();
			if (Data == null || Data.Length == 0) return brrData.ToArray();

			var data = new Int16[Data.Length];
			Array.Copy(Data, data, Data.Length);
			if (LoopEnd > 0)
			{
				// Align our sample loop so it's a multiple of 16

				var startAlign = (16 - (LoopStart % 16)) % 16;
				var loopSize = LoopEnd - LoopStart;
				var endAlign = loopSize;

				while ((endAlign % 16) != 0) endAlign <<= 1;

				// remove the existing loop block from the alignment
				endAlign -= loopSize;

				// also include the loop_start alignment
				endAlign += startAlign;

				var addedBytes = Math.Ceiling(endAlign / 16f) * 9;
				if (endAlign != 0) // If endAlign == 0, the loop is already aligned
				{
					if (addedBytes > maximumSampleGrowth)
					{
						data = ResampleLoop(16 - (loopSize % 16));
					}
					else
					{
						if (output != null) output(string.Format("WARNING: Sample \"{0}\" loop not aligned to 16 frames, results in {1} bytes larger file size", Name, addedBytes));
						data = UnrollLoop(startAlign, endAlign);
					}

				}
			}
			// TODO: Is is better to add zeroes at the *end* of non-looping samples?
			if (data.Length % 16 != 0)
			{
				var samplesToAdd = 16 - data.Length % 16;
				var newData = new Int16[data.Length + samplesToAdd];
				Array.Copy(data, 0, newData, samplesToAdd, data.Length);
			}


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
	}
}
