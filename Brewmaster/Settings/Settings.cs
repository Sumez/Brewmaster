using Brewmaster.Modules.Watch;
using Brewmaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Brewmaster.EditorWindows;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.Settings
{
	[Serializable]
	public class Settings
	{
		public bool ReOpenLastProject = true;
		public string CurrentProject;

		[NonSerialized] private string _filePath;

		[XmlElement(ElementName = "WindowState")]
		public FormWindowState WindowState = FormWindowState.Normal;
		[XmlElement(ElementName = "WindowX")]
		public int? WindowX;
		[XmlElement(ElementName = "WindowY")]
		public int? WindowY;

		[XmlElement(ElementName = "RecentProject")]
		public List<string> RecentProjects = new List<string>();
		[XmlElement(ElementName = "UpdateRate")]
		public int UpdateRate = 30;

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
		[XmlElement(ElementName = "ShowScrollOverlay")]
		public bool ShowScrollOverlay = true;
		[XmlElement(ElementName = "ResizeTileMap")]
		public bool ResizeTileMap = true;
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
				CurrentConfiguration = project.CurrentConfiguration != null ? project.CurrentConfiguration.Name : null,
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
		LoadState = 6,
		SaveState = 7,
		CreateNew = 8,
		Open = 9,
		Save = 10,
		SaveAll = 11,
		Print = 12,
		Exit = 13,
		CloseWindow = 15,
		Find = 16,
		FindAll = 17,
		FindNext = 18,
		Replace = 19,
		GoToDefinition = 20,
		AddToWatch = 21,
		AutoComplete = 22,
		GoToLine = 23,
		GoToAll = 24,

		Cut = 30,
		Copy = 31,
		Paste = 32,
		SelectAll = 35,

		Build = 40,
		Run = 41,
		RunNewBuild = 42,
		Pause = 43,
		Stop = 44,
		Restart = 45,
		StepOver = 60,
		StepInto = 61,
		StepOut = 62,
		StepBack = 63,
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
					new KeyBinding(Feature.CloseWindow, Keys.W | Keys.Control),
					new KeyBinding(Feature.AutoComplete, Keys.Space | Keys.Control),

					new KeyBinding(Feature.CreateNew, Keys.N | Keys.Control),
					new KeyBinding(Feature.Open, Keys.O | Keys.Control),
					new KeyBinding(Feature.Save, Keys.S | Keys.Control),
					new KeyBinding(Feature.SaveAll, Keys.S | Keys.Control | Keys.Shift),
					new KeyBinding(Feature.Print, Keys.P | Keys.Control),
					new KeyBinding(Feature.Exit, Keys.F4 | Keys.Alt),

					new KeyBinding(Feature.Cut, Keys.X | Keys.Control),
					new KeyBinding(Feature.Copy, Keys.C | Keys.Control),
					new KeyBinding(Feature.Paste, Keys.V | Keys.Control),
					new KeyBinding(Feature.SelectAll, Keys.A | Keys.Control),

					new KeyBinding(Feature.Find, Keys.F | Keys.Control),
					new KeyBinding(Feature.FindAll, Keys.F | Keys.Control | Keys.Shift),
					new KeyBinding(Feature.FindNext, Keys.F3),
					new KeyBinding(Feature.Replace, Keys.H | Keys.Control),
					new KeyBinding(Feature.GoToLine, Keys.G | Keys.Control),
					new KeyBinding(Feature.GoToAll, Keys.Oemcomma | Keys.Control),

					new KeyBinding(Feature.Build, Keys.B | Keys.Control | Keys.Shift),
					new KeyBinding(Feature.Run, Keys.F5),
					new KeyBinding(Feature.RunNewBuild, Keys.F5 | Keys.Control),
					new KeyBinding(Feature.Restart, Keys.R | Keys.Control),

					new KeyBinding(Feature.StepOver, Keys.F10),
					new KeyBinding(Feature.StepInto, Keys.F11),
					new KeyBinding(Feature.StepOut, Keys.F11 | Keys.Shift),
					new KeyBinding(Feature.StepBack, Keys.F10 | Keys.Shift)
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
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Up, MappedTo = (int)ControllerButtons.Up,  Name = "Up" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Down, MappedTo = (int)ControllerButtons.Down, Name = "Down" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Left, MappedTo = (int)ControllerButtons.Left, Name = "Left" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Right, MappedTo = (int)ControllerButtons.Right, Name = "Right" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Select, MappedTo = (int)ControllerButtons.Select, Name = "Select" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Start, MappedTo = (int)ControllerButtons.Start, Name = "Start" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.B, MappedTo = (int)ControllerButtons.B, Name = "B" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.A, MappedTo = (int)ControllerButtons.A, Name = "A" },
			};
		} }
		public static List<KeyboardMapping> SnesDefaults
		{
			get
			{
				return new List<KeyboardMapping>() {
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Up, MappedTo = (int)ControllerButtons.Up,  Name = "Up" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Down, MappedTo = (int)ControllerButtons.Down, Name = "Down" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Left, MappedTo = (int)ControllerButtons.Left, Name = "Left" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Right, MappedTo = (int)ControllerButtons.Right, Name = "Right" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Select, MappedTo = (int)ControllerButtons.Select, Name = "Select" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Start, MappedTo = (int)ControllerButtons.Start, Name = "Start" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.Y, MappedTo = (int)ControllerButtons.Y, Name = "Y" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.X, MappedTo = (int)ControllerButtons.X, Name = "X" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.B, MappedTo = (int)ControllerButtons.B, Name = "B" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.A, MappedTo = (int)ControllerButtons.A, Name = "A" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.L, MappedTo = (int)ControllerButtons.L, Name = "L" },
				new KeyboardMapping { TargetKey = (int)ControllerButtons.R, MappedTo = (int)ControllerButtons.R, Name = "R" },
			};
			}
		}
	}

	public enum ControllerButtons
	{
		Up = 294, Down = 296, Left = 293, Right = 295, B = 90, A = 88, Y = 65, X = 83, L = 81, R = 87, Select = 32, Start = 13
	}
}
