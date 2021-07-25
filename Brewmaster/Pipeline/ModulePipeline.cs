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
			var module = new ItModule();
			using (var stream = File.OpenRead(settings.File.File.FullName))
			{
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
				module.Title = System.Text.Encoding.ASCII.GetString(title);

				for (var i = 0; i < 64; i++) {
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
				foreach (var pointer in patternPointers.Where(p => p > 0))
				{
					stream.Position = pointer;
					module.Patterns.Add(LoadPattern(stream));
				}

				var sampleDirectoryLength = (byte)(module.Samples.Count * 2);
				var trackDirectoryLength = 1;

				var sampleDirectory = new List<Int16>(sampleDirectoryLength);
				var trackDirectory = new List<Int16>(trackDirectoryLength);
				var baseAddress = 0xC00;
				var sampleAddress = (Int16)(baseAddress + sampleDirectoryLength * 2 + trackDirectoryLength * 2);
				var sampleData = new List<byte>();
				var trackData = new List<byte>();

				foreach (var sample in module.Samples)
				{
					var filename = sample.Name;
					foreach (var ch in Path.GetInvalidFileNameChars())
					{
						filename = filename.Replace(ch.ToString(), "");
					}
					using (var stream2 = new MemoryStream())
					{
						var data = new byte[sample.Data.Length * 2];
						Buffer.BlockCopy(sample.Data, 0, data, 0, data.Length);
						WriteWavHeader(stream2, false, 1, 16, 32000, data.Length);
						stream2.Write(data, 0, data.Length);
						var waveBytes = new byte[stream2.Length];
						stream2.Position = 0;
						stream2.Read(waveBytes, 0, waveBytes.Length);
						File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".wav", waveBytes);
					}
					var brr = sample.GetBrr(output);
					File.WriteAllBytes(settings.GetFilePath(0) + "." + filename + ".brr", brr);

					sampleDirectory.Add(sampleAddress); // Start address
					sampleDirectory.Add(sampleAddress); // Loop address

					sampleAddress += (Int16)brr.Length;
					sampleData.AddRange(brr);
				}

				var trackAddress = sampleAddress;
				//foreach (var module in modules)
				{
					var compressedTrack = module.GetCompressedPatternData();
					trackDirectory.Add(trackAddress);
					trackData.AddRange(compressedTrack);
				}

				using (var bankStream = File.OpenWrite(settings.GetFilePath(0)))
				{
					var sampleDirectoryData = new byte[sampleDirectoryLength * 2];
					var trackDirectoryData = new byte[trackDirectoryLength * 2];

					Buffer.BlockCopy(sampleDirectory.ToArray(), 0, sampleDirectoryData, 0, sampleDirectoryData.Length);
					Buffer.BlockCopy(trackDirectory.ToArray(), 0, trackDirectoryData, 0, trackDirectoryData.Length);

					bankStream.WriteByte(sampleDirectoryLength);
					bankStream.Write(sampleDirectoryData, 0, sampleDirectoryData.Length);
					bankStream.Write(trackDirectoryData, 0, trackDirectoryData.Length);
					bankStream.Write(sampleData.ToArray(), 0, sampleData.Count);
					bankStream.Write(trackData.ToArray(), 0, trackData.Count);

					bankStream.Close();
				}
			}
		}

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
		private Pattern LoadPattern(FileStream stream)
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
				while (pattern.Channels.Count < (channel+1))
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

		private Sample LoadSample(FileStream stream)
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
			var c5speed = ReadUInt32(stream);
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

		private Int16[] ReadSampleData(FileStream stream, Sample sample, Compression compression, int bitDepth, bool stereo)
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

		private enum Compression
		{
			IT214, IT215, PCMD, PCMS, PCMU
		}

		private UInt32 ReadUInt32(FileStream stream)
		{
			var buffer = new byte[4];
			var int32 = new UInt32[1];
			stream.Read(buffer, 0, 4);
			Buffer.BlockCopy(buffer, 0, int32, 0, 4);
			return int32[0];
		}
		private UInt16 ReadUInt16(FileStream stream)
		{
			var buffer = new byte[2];
			var int16 = new UInt16[1];
			stream.Read(buffer, 0, 2);
			Buffer.BlockCopy(buffer, 0, int16, 0, 2);
			return int16[0];
		}

		private string ReadString(FileStream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return System.Text.Encoding.ASCII.GetString(buffer);
		}

		private Instrument LoadInstrument(FileStream stream)
		{
			var instrument = new Instrument();

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

			var notetrans = new byte[240];
			Buffer.BlockCopy(buffer, 64, notetrans, 0, 240);

			var envelope = new byte[82];
			Buffer.BlockCopy(buffer, 304, notetrans, 0, 82);
			Buffer.BlockCopy(buffer, 386, notetrans, 0, 82);
			Buffer.BlockCopy(buffer, 468, notetrans, 0, 82);


			var test2 = System.Text.Encoding.ASCII.GetString(filename);
			var test3 = System.Text.Encoding.ASCII.GetString(name);

			return instrument;
		}
		private Instrument LoadInstrumentOld(FileStream stream)
		{
			throw new NotImplementedException();
		}

		private List<uint> Read32BitBuffer(Stream stream, int count)
		{
			var buffer = new byte[count * 4];
			var array = new UInt32[count];
			stream.Read(buffer, 0, count * 4);
			Buffer.BlockCopy(buffer, 0, array, 0, count * 4);
			return array.ToList();
		}
	}

	public class ItModule
	{
		public string Title;
		public List<Channel> Channels = new List<Channel>(64);

		public List<int> OrderList { get; set; }
		public List<Instrument> Instruments = new List<Instrument>();
		public List<Sample> Samples = new List<Sample>();
		public List<Pattern> Patterns = new List<Pattern>();

		public byte[] GetCompressedPatternData()
		{
			var data = new List<byte>();
			var orderReferences = new UInt16[OrderList.Count * 8 + 1];
			var orderPatternSizes = new byte[OrderList.Count];
			var patternAddress = orderReferences.Length * 2 + orderPatternSizes.Length;

			//var pattern0 = Patterns[0].Channels[2];
			//data.AddRange(GetCompressedPatternData(pattern0));

			var patternMap = new Dictionary<int, int>();
			var patternAddressMap = new Dictionary<int, int>();
			var channelPatterns = new List<byte[]>();
			var patternIndex = 0;
			foreach (var pattern in Patterns)
			{
				for (var i = 0; i < 8; i++)
				{
					byte[] patternData = i >= pattern.Channels.Count ? GetSilentPattern(pattern) : GetCompressedPatternData(pattern.Channels[i]);
					//byte[] patternData = i >= pattern.Channels.Count ? GetSilentPattern(pattern) : GetCompressedPatternData(pattern.Channels[3]);
					var existing = channelPatterns.FindIndex(d => d.SequenceEqual(patternData));
					if (existing >= 0)
					{
						patternMap[patternIndex + i] = existing;
					}
					else
					{
						patternMap[patternIndex + i] = channelPatterns.Count;
						patternAddressMap[channelPatterns.Count] = patternAddress;
						patternAddress += patternData.Length;
						
						channelPatterns.Add(patternData);
					}
				}
				patternIndex += 8;
			}
			// TODO: Skip storing unused patterns if any
			for (var i = 0; i < OrderList.Count; i++) {
				var patternId = OrderList[i] * 8;
				for (var j = 0; j < 8; j++)
				{
					orderReferences[i * 8 + j] = (UInt16)patternAddressMap[patternMap[patternId + j]];
				}
				if (Patterns[OrderList[i]].Rows > 255) throw new Exception(string.Format("Two many rows in pattern #{0}. Maximum is 255", OrderList[i]));
				orderPatternSizes[i] = (byte)Patterns[OrderList[i]].Rows;
			}
			orderReferences[OrderList.Count * 8] = 0xffff;
			var orderData = new byte[orderReferences.Length * 2];
			Buffer.BlockCopy(orderReferences, 0, orderData, 0, orderData.Length);

			data.AddRange(orderData);
			data.AddRange(orderPatternSizes);
			foreach (var patternData in channelPatterns) data.AddRange(patternData);
			
			return data.ToArray();
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

		private byte[] GetCompressedPatternData(PatternNote[] pattern)
		{
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

			foreach (var note in pattern)
			{
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
				var instrument = note.Instrument - 1;
				if (instrument >= 55) throw new Exception("Maximum of 54 instruments supported in a track, sorry");
				if (!note.HasNote) instrument = 63; // none
				if (note.Note == 255) instrument = 62; // off
				if (note.Note == 254) instrument = 61; // cut
				if (note.Note == 253) instrument = 60; // fade
				var header = (note.Effect > 0 ? 0x80 : 0) | (note.VolumeEffect > 0 ? 0x40 : 0) | instrument;

				currentBlock.Add((byte)header);
				if (instrument < 55) currentBlock.Add(note.Note);
				if (note.VolumeEffect > 0) currentBlock.Add(note.VolumeEffect);
				if (note.Effect > 0) currentBlock.Add(note.Effect);
				if (note.Effect > 0) currentBlock.Add(note.EffectParam);
				currentBlockRows++;
			}
			FlushBlock();
			if (clearCount > 0) data.Add((byte)(0x80 | clearCount));

			return data.ToArray();
		}
	}
	public class Channel
	{
		public int Panning { get; internal set; }
		public int Volume { get; internal set; }
	}
	public class Instrument
	{

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

		public byte[] GetBrr(Action<string> output = null)
		{
			var brrData = new List<byte>();

			var data = new Int16[Data.Length];
			Array.Copy(Data, data, Data.Length);
			if (LoopEnd > 0)
			{
				// TODO: SNESBRR style super wasteful loop padding, make it possible to disable this in process settings (and probably by default)
				// Instead warn the user when loops don't align up to 16 samples

				var startAlign = (16 - (LoopStart & 15)) & 15;
				var loopSize = LoopEnd - LoopStart;
				var endAlign = loopSize;

				while ((endAlign & 15) != 0) endAlign <<= 1;

				// remove the existing loop block from the alignment
				endAlign -= loopSize;

				// also include the loop_start alignment
				endAlign += startAlign;

				if (endAlign != 0)
				{
					if (output != null) {
						var addedBytes = Math.Ceiling(endAlign / 16f) * 9;
						output(string.Format("WARNING: Sample \"{0}\" loop not aligned to 16 frames, results in {1} bytes larger file size", Name, addedBytes));
					}

					data = new Int16[LoopEnd + endAlign];
					Array.Copy(Data, data, Math.Min(Data.Length, LoopEnd));

					for (var i = 0; i < endAlign; i++) data[LoopEnd + i] = data[LoopStart + i];
					
					// 16-sample align loop_start
					LoopStart += startAlign;
				}
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
