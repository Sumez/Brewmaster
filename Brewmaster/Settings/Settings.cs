using BrewMaster.Modules.Watch;
using BrewMaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using BrewMaster.EditorWindows;
using ICSharpCode.TextEditor.Document;

namespace BrewMaster.Settings
{
	[Serializable]
	public class Settings
	{
		public bool ReOpenLastProject = true;
		public string CurrentProject;

		[NonSerialized] private string _filePath;

		[XmlElement(ElementName = "RecentProject")]
		public List<string> RecentProjects = new List<string>();
		[XmlElement(ElementName = "UpdateRate")]
		public int UpdateRate = 1;

		[XmlElement(ElementName = "EmuIntegerScaling")]
		public bool EmuIntegerScaling = true;
		[XmlElement(ElementName = "EmuRandomPowerOn")]
		public bool EmuRandomPowerOn = true;
		[XmlElement(ElementName = "EmuPlayAudio")]
		public bool EmuPlayAudio = true;
		[XmlElement(ElementName = "EmuPlayPulse1")]
		public bool EmuPlayPulse1 = true;
		[XmlElement(ElementName = "EmuPlayPulse2")]
		public bool EmuPlayPulse2 = true;
		[XmlElement(ElementName = "EmuPlayTriangle")]
		public bool EmuPlayTriangle = true;
		[XmlElement(ElementName = "EmuPlayNoise")]
		public bool EmuPlayNoise = true;
		[XmlElement(ElementName = "EmuPlayPcm")]
		public bool EmuPlayPcm = true;
		[XmlElement(ElementName = "EmuDisplayNesBg")]
		public bool EmuDisplayNesBg = true;
		[XmlElement(ElementName = "EmuDisplaySprites")]
		public bool EmuDisplaySprites = true;
		[XmlElement(ElementName = "EmuDisplaySnesBg1")]
		public bool EmuDisplaySnesBg1 = true;
		[XmlElement(ElementName = "EmuDisplaySnesBg2")]
		public bool EmuDisplaySnesBg2 = true;
		[XmlElement(ElementName = "EmuDisplaySnesBg3")]
		public bool EmuDisplaySnesBg3 = true;
		[XmlElement(ElementName = "EmuDisplaySnesBg4")]
		public bool EmuDisplaySnesBg4 = true;
		[XmlElement(ElementName = "EmuBackground")]
		public string SerializedEmuBackground = "Black";
		[XmlIgnore]
		public Color EmuBackgroundColor
		{
			get { return ColorTranslator.FromHtml(SerializedEmuBackground); }
			set { SerializedEmuBackground = ColorTranslator.ToHtml(value); }
		}

		[XmlElement(ElementName = "DefaultFontFamily")]
		public string DefaultFontFamily = "Courier New";
		[XmlElement(ElementName = "DefaultFontSize")]
		public float DefaultFontSize = 10;
		[XmlIgnore]
		public Font DefaultFont
		{
			get { return new Font(DefaultFontFamily, DefaultFontSize, GraphicsUnit.Point); }
			set { DefaultFontFamily = value.FontFamily.Name; DefaultFontSize = value.SizeInPoints; }
		}

		[XmlElement(ElementName = "AsmHighlighting")]
		public HighlightingColors AsmHighlighting;

		[XmlElement(ElementName = "ShowStatusBar")]
		public bool ShowStatusBar = true;
		[XmlElement(ElementName = "ShowToolbar")]
		public bool ShowToolbar = true;
		[XmlElement(ElementName = "ShowLineNumbers")]
		public bool ShowLineNumbers = true;
		[XmlElement(ElementName = "ShowLineAddresses")]
		public bool ShowLineAddresses = false;

		[XmlElement(ElementName = "DefaultProjectDirectory")]
		public string DefaultProjectDirectory;



		[XmlElement(ElementName = "ProjectState")]
		public List<ProjectUserSettings> ProjectStates = new List<ProjectUserSettings>();
		[XmlElement(ElementName = "NesKeyMapping")]
		public List<KeyboardMapping> NesMappings;
		[XmlElement(ElementName = "SnesKeyMapping")]
		public List<KeyboardMapping> SnesMappings;
		[XmlElement(ElementName = "SystemKey")]
		public KeyBindings KeyBindings;

		public static Settings Load(string filePath)
		{
			if (!File.Exists(filePath))
			{
				var settings = new Settings();
				return Prepare(settings, filePath);
			}
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			string xmlString = xmlDocument.OuterXml;

			using (var read = new StringReader(xmlString))
			{
				var serializer = new XmlSerializer(typeof(Settings));
				using (var reader = new XmlTextReader(read))
				{
					var settings = (Settings)serializer.Deserialize(reader);
					return Prepare(settings, filePath);
				}
			}
		}
		private static Settings Prepare(Settings settings, string path)
		{
			settings._filePath = path;
			if (settings.KeyBindings == null || !settings.KeyBindings.Any())
			{
				settings.KeyBindings = KeyBindings.Defaults;
			}
			if (settings.NesMappings == null || !settings.NesMappings.Any())
			{
				settings.NesMappings = KeyboardMapping.NesDefaults;
			}
			if (settings.SnesMappings == null || !settings.SnesMappings.Any())
			{
				settings.SnesMappings = KeyboardMapping.SnesDefaults;
			}
			//settings.KeyBindings = KeyBindings.Defaults;
			//settings.NesMappings = KeyboardMapping.NesDefaults;
			//settings.SnesMappings = KeyboardMapping.SnesDefaults;
			return settings;
		}

		public void Save()
		{
			try
			{
				var parentPath = Directory.GetParent(_filePath);
				if (!parentPath.Exists) parentPath.Create();
				var xmlDocument = new XmlDocument();
				var serializer = new XmlSerializer(GetType());
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, this);
					stream.Position = 0;
					xmlDocument.Load(stream);
					xmlDocument.Save(_filePath);
				}
			}
			catch (Exception ex)
			{
				Program.Error("Error saving settings: " + ex.Message, ex);
			}
		}


		public void SetProjectState(AsmProject project, IEnumerable<EditorWindow> openWindows, IEnumerable<WatchValueData> watchData, IEnumerable<Breakpoint> breakpoints)
		{
			if (project.ProjectFile == null) return;
			var newState = new ProjectUserSettings
			{
				Filename = project.ProjectFile.FullName,
				CurrentConfiguration = project.CurrentConfiguration.Name,
				OpenFiles = openWindows.Select(w => w.ProjectFile.File.FullName).ToArray(),
				WatchData = watchData.ToArray(),
				Breakpoints = breakpoints.Select(bp => bp.GetSerializable()).ToArray()
			};
			ProjectStates.RemoveAll(p => p.Filename == project.ProjectFile.FullName);
			ProjectStates.Add(newState);
		}
	}
	[Serializable]
	public class ProjectUserSettings
	{
		public string CurrentConfiguration;
		public string Filename;
		[XmlElement(ElementName = "OpenFile")]
		public string[] OpenFiles;
		[XmlElement(ElementName = "WatchValue")]
		public WatchValueData[] WatchData;
		[XmlElement(ElementName = "Breakpoint")]
		public BreakpointData[] Breakpoints;
	}
	[Serializable]
	public class BreakpointData
	{
		public int StartAddress;
		public int? EndAddress;
		public int Type;
		public int AddressType;
		public bool Automatic;
		public string Symbol;
		public string File;
		public int Line;
		public bool Disabled;

		public Breakpoint GetBreakpoint(AsmProjectFile file = null)
		{
			return new Breakpoint
			{
				AddressType = (Breakpoint.AddressTypes)AddressType,
				Automatic = Automatic,
				CurrentLine = Line,
				Disabled = Disabled,
				StartAddress = StartAddress,
				EndAddress = EndAddress,
				Symbol = Symbol,
				Type = (Breakpoint.Types)Type,
				File = file
			};
		}
	}
	[Serializable]
	public enum Feature
	{
		Undo = 0,
		Redo = 1,
		Rename = 2,
		ActivateItem = 4,
		RemoveFromList = 5,
		GoToDefinition = 20,
		AddToWatch = 21,
		AutoComplete = 22
	}
	[Serializable]
	public class HighlightingColors {
		[XmlIgnore]
		public Dictionary<string, HighlightColor> Data
		{
			get
			{
				var dictionary = new Dictionary<string, HighlightColor>( );
				foreach (var color in SerializedData)
				{
					if (color.BgColor != null) dictionary[color.Name] = new HighlightColor(ColorTranslator.FromHtml(color.Color), ColorTranslator.FromHtml(color.BgColor), color.Bold, color.Italic);
					else dictionary[color.Name] = new HighlightColor(ColorTranslator.FromHtml(color.Color), color.Bold, color.Italic);
				}
				return dictionary;
			}
			set
			{
				SerializedData = value.Select(kvp => new SerializedHighlightColor
				{
					Name = kvp.Key,
					Color = ColorTranslator.ToHtml(kvp.Value.Color),
					BgColor = kvp.Value.HasBackground ? ColorTranslator.ToHtml(kvp.Value.BackgroundColor) : null,
					Bold = kvp.Value.Bold,
					Italic = kvp.Value.Italic
				}).ToList();
			}
		}

		[XmlElement(ElementName = "HighlightColor")]
		public List<SerializedHighlightColor> SerializedData;
	}

	[Serializable]
	public class SerializedHighlightColor
	{
		[XmlElement(ElementName = "Name")] public string Name;
		[XmlElement(ElementName = "Color")] public string Color;
		[XmlElement(ElementName = "BgColor")] public string BgColor;
		[XmlElement(ElementName = "Bold")] public bool Bold;
		[XmlElement(ElementName = "Italic")] public bool Italic;
	}

	[Serializable]
	public class KeyBindings : List<KeyBinding>
	{
		public Keys this[Feature key]    // Indexer declaration  
		{
			set
			{
				this.RemoveAll(p => p.Feature == (int)key);
				this.Add(new KeyBinding(key, value));
			}
			get { return (Keys)this.FirstOrDefault(p => p.Feature == (int)key).Binding; }
		}

		public static KeyBindings Defaults
		{
			get
			{
				return new KeyBindings
				{
					new KeyBinding(Feature.Undo, Keys.Z | Keys.Control),
					new KeyBinding(Feature.Redo, Keys.Y | Keys.Control),
					new KeyBinding(Feature.GoToDefinition, Keys.F12),
					new KeyBinding(Feature.RemoveFromList, Keys.Delete),
					new KeyBinding(Feature.Rename, Keys.F2),
					new KeyBinding(Feature.AutoComplete, Keys.Space | Keys.Control),
				};
			}
		}
	}

	[Serializable]
	public struct KeyBinding
	{
		public KeyBinding(Feature feature, Keys binding)
		{
			Feature = (int)feature;
			Binding = (int)binding;
		}
		public int Feature
		{ get; set; }

		public int Binding
		{ get; set; }
	}

	[Serializable]
	public class KeyboardMapping
	{
		public string Name;
		public int TargetKey;
		public int MappedTo;
		public string MappedToName;

		public KeyboardMapping Clone()
		{
			return new KeyboardMapping
			{
				Name = Name,
				TargetKey = TargetKey,
				MappedTo = MappedTo,
				MappedToName = MappedToName
			};
		}

		public static List<KeyboardMapping> NesDefaults { get
			{
				return new List<KeyboardMapping>() {
				new KeyboardMapping { TargetKey = 328, MappedTo = (int)Keys.Up,  Name = "Up" },
				new KeyboardMapping { TargetKey = 336, MappedTo = (int)Keys.Down, Name = "Down" },
				new KeyboardMapping { TargetKey = 331, MappedTo = (int)Keys.Left, Name = "Left" },
				new KeyboardMapping { TargetKey = 333, MappedTo = (int)Keys.Right, Name = "Right" },
				new KeyboardMapping { TargetKey = 16, MappedTo = (int)Keys.Q, Name = "Select" },
				new KeyboardMapping { TargetKey = 17, MappedTo = (int)Keys.W, Name = "Start" },
				new KeyboardMapping { TargetKey = 30, MappedTo = (int)Keys.Z, Name = "B" },
				new KeyboardMapping { TargetKey = 31, MappedTo = (int)Keys.X, Name = "A" },
			};
		} }
		public static List<KeyboardMapping> SnesDefaults
		{
			get
			{
				return new List<KeyboardMapping>() {
				new KeyboardMapping { TargetKey = 328, MappedTo = (int)Keys.Up,  Name = "Up" },
				new KeyboardMapping { TargetKey = 336, MappedTo = (int)Keys.Down, Name = "Down" },
				new KeyboardMapping { TargetKey = 331, MappedTo = (int)Keys.Left, Name = "Left" },
				new KeyboardMapping { TargetKey = 333, MappedTo = (int)Keys.Right, Name = "Right" },
				new KeyboardMapping { TargetKey = 18, MappedTo = (int)Keys.Left, Name = "Select" },
				new KeyboardMapping { TargetKey = 32, MappedTo = (int)Keys.Left, Name = "Start" },
				new KeyboardMapping { TargetKey = 44, MappedTo = (int)Keys.Left, Name = "Y" },
				new KeyboardMapping { TargetKey = 45, MappedTo = (int)Keys.Left, Name = "X" },
				new KeyboardMapping { TargetKey = 30, MappedTo = (int)Keys.Left, Name = "B" },
				new KeyboardMapping { TargetKey = 31, MappedTo = (int)Keys.Left, Name = "A" },
				new KeyboardMapping { TargetKey = 16, MappedTo = (int)Keys.Left, Name = "L" },
				new KeyboardMapping { TargetKey = 17, MappedTo = (int)Keys.Left, Name = "R" },
			};
			}
		}
	}
}
