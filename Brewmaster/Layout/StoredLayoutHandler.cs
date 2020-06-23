using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Brewmaster.Layout
{
	public class StoredLayoutHandler : LayoutHandler
	{
		private LayoutMode _mode;
		private Dictionary<LayoutMode, PanelLayout> _layouts = new Dictionary<LayoutMode, PanelLayout>();

		public StoredLayoutHandler(MainForm mainForm) : base(mainForm)
		{
			_mode = LayoutMode.None;
		}

		public void StorePanelLayout(Dictionary<string, Control> modules, LayoutMode mode, string filename)
		{
			if (mode == _mode) RememberPanelLayout(modules);
			try
			{
				var parentPath = Directory.GetParent(filename);
				if (!parentPath.Exists) parentPath.Create();
				var xmlDocument = new XmlDocument();
				var serializer = new XmlSerializer(typeof(PanelLayout));
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, _layouts[mode]);
					stream.Position = 0;
					xmlDocument.Load(stream);
					xmlDocument.Save(filename);
				}
			}
			catch (Exception ex)
			{
				Program.Error("Error saving layout: " + ex.Message, ex);
			}
		}

		private void RememberPanelLayout(Dictionary<string, Control> modules)
		{
			if (_mode == LayoutMode.None) return;
			var moduleNames = modules.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
			var layout = new PanelLayout();

			foreach (var floatPanel in Application.OpenForms.OfType<FloatPanel>().Where(p => p.Visible))
			{
				// TODO: Maintain a list of all FloatPanels handled by this LayoutHandler
				var panelLayout = new StoredFloatPanel
				{
					Width = floatPanel.Width,
					Height = floatPanel.Height,
					Left = floatPanel.Left,
					Top = floatPanel.Top
				};
				layout.FloatPanels.Add(panelLayout);
				AddPanelNamesToGroup(moduleNames, floatPanel.ChildPanel, panelLayout.Panels);
			}
			foreach (var container in DockContainers)
			{
				var splitLayout = new StoredSplitPanelLayout
				{
					Name = container.Name,
					Size = (container.Horizontal ? container.Height : container.Width) + 2
				};
				layout.SplitPanels.Add(splitLayout);
				for (var i = 0; i < container.Panels.Count; i++)
				{
					var group = new PanelGroup { Size = container.Splits[i] };
					splitLayout.PanelGroups.Add(group);
					AddPanelNamesToGroup(moduleNames, container.Panels[i].ContainedControl as IdePanel, group.Panels);
				}
			}

			_layouts[_mode] = layout;
		}

		private void AddPanelNamesToGroup(Dictionary<Control, string> moduleNames, IdePanel parent, List<string> groupPanels)
		{
			if (parent == null) return;
			if (parent is IdeGroupedPanel groupedPanel)
			{
				foreach (var childPanel in groupedPanel.Panels)
				{
					if (moduleNames.ContainsKey(childPanel.Child)) groupPanels.Add(moduleNames[childPanel.Child]);
				}
			}
			else if (moduleNames.ContainsKey(parent.Child)) groupPanels.Add(moduleNames[parent.Child]);

		}

		private void LoadLayout(Dictionary<string, Control> modules, PanelLayout layout)
		{
			foreach (var control in modules.Values)
			{
				OnPanelStatusChanged(GetPanel(control), false);
			}
			foreach (var floatPanel in Application.OpenForms.OfType<FloatPanel>().ToList())
			{
				// TODO: Maintain a list of all FloatPanels handled by this LayoutHandler
				floatPanel.Controls.Remove(floatPanel.ChildPanel);
				floatPanel.Close();
			}
			foreach (var panelLayout in layout.SplitPanels)
			{
				var container = DockContainers.FirstOrDefault(p => p.Name == panelLayout.Name);
				if (container == null) continue;
				container.Clear();

				var parent = container.Parent;
				if (panelLayout.Size >= 0)
					while (parent != null)
					{
						if (parent is MultiSplitPanel splitPanel)
						{
							splitPanel.StaticWidth = panelLayout.Size;
							break;
						}
						parent = parent.Parent;
					}

				foreach (var group in panelLayout.PanelGroups)
				{
					AddPanelGroup(group.Panels, p => container.AddPanel(p), modules);
				}
				if (panelLayout.PanelGroups.All(g => g.Size >= 0)) container.SetSplits(panelLayout.PanelGroups.Select(g => g.Size).ToList());
			}

			foreach (var panelLayout in layout.FloatPanels)
			{
				AddPanelGroup(panelLayout.Panels, p => CreateFloatPanel(p, new Point(panelLayout.Left, panelLayout.Top), new Size(panelLayout.Width, panelLayout.Height)), modules);
			}
		}
		public void LoadPanelLayout(Dictionary<string, Control> modules, LayoutMode? requestedMode = null, string filename = null)
		{
			var mode = requestedMode ?? _mode;
			if (filename == null || !File.Exists(filename))
			{
				switch (mode)
				{
					case LayoutMode.Default:
						_layouts[mode] = GetDefaultLayout();
						break;
					case LayoutMode.MapEditor:
						_layouts[mode] = GetMapEditorLayout();
						break;
					default:
						_layouts[mode] = GetNoLayout();
						break;
				}
				return;
			}

			var xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			var xmlString = xmlDocument.OuterXml;

			using (var read = new StringReader(xmlString))
			{
				var serializer = new XmlSerializer(typeof(PanelLayout));
				using (var reader = new XmlTextReader(read)) _layouts[mode] = (PanelLayout) serializer.Deserialize(reader);
			}
		}

		public void RefreshLayout(Dictionary<string, Control> modules)
		{
			LoadLayout(modules, _layouts[_mode]);
		}
		public void LoadEditorLayout(Dictionary<string, Control> modules, LayoutMode? requestedMode = null)
		{
			var mode = requestedMode ?? _mode;
			if (_mode == mode) return;
			RememberPanelLayout(modules);
			_mode = mode;
			if (!_layouts.ContainsKey(_mode)) LoadPanelLayout(modules, _mode);
			RefreshLayout(modules);
		}
		public void ClearLayout(Dictionary<string, Control> modules)
		{
			_mode = LayoutMode.None;
			LoadPanelLayout(modules, LayoutMode.None);
			RefreshLayout(modules);
		}

		private void AddPanelGroup(List<string> panels, Action<IdePanel> add, Dictionary<string, Control> modules)
		{
			if (panels.Count > 1)
			{
				var groupedPanel = new IdeGroupedPanel();
				add(groupedPanel);
				add = p => groupedPanel.AddPanel(p);
			}
			foreach (var moduleName in panels)
			{
				var module = modules.ContainsKey(moduleName) ? modules[moduleName] : null;
				if (module == null) continue;
				var panel = GetPanel(module);
				add(panel);
				OnPanelStatusChanged(panel, true);
			}
		}

		private PanelLayout GetDefaultLayout()
		{
			var layout = new PanelLayout();
			layout.SplitPanels.AddRange(new[]
			{
				new StoredSplitPanelLayout
				{
					Name = "west",
					Size = 250,
					PanelGroups = new []
					{
						new PanelGroup { Panels = new [] { "Project Explorer" }.ToList() },
						new PanelGroup { Panels = new [] { "Opcodes", "Commands", "Number Formats" }.ToList() }
					}.ToList()
				},
				new StoredSplitPanelLayout
				{
					Name = "south",
					Size = 250,
					PanelGroups = new []
					{
						new PanelGroup { Panels = new [] { "Output", "Build Errors" }.ToList() },
						new PanelGroup { Panels = new [] { "Watch", "Breakpoints" }.ToList() },
						new PanelGroup { Panels = new [] { "Memory Viewer", "Sprite List" }.ToList() }
					}.ToList()
				},
				new StoredSplitPanelLayout
				{
					Name = "east",
					Size = 300,
					PanelGroups = new []
					{
						new PanelGroup { Panels = new [] { "Chr", "Nametables", "Sprites", "Palette" }.ToList() },
						new PanelGroup { Panels = new [] { "Console Status" }.ToList() },
						new PanelGroup { Panels = new [] { "Mesen" }.ToList() }
					}.ToList()
				}
			});
			return layout;
		}

		private PanelLayout GetNoLayout()
		{
			var layout = new PanelLayout();
			layout.SplitPanels.AddRange(new[]
			{
				new StoredSplitPanelLayout
				{
					Name = "west",
					Size = 1,
					PanelGroups = new List<PanelGroup>()
				},
				new StoredSplitPanelLayout
				{
					Name = "south",
					Size = 1,
					PanelGroups = new List<PanelGroup>()
				},
				new StoredSplitPanelLayout
				{
					Name = "east",
					Size = 1,
					PanelGroups = new List<PanelGroup>()
				}
			});
			return layout;

		}
		private PanelLayout GetMapEditorLayout()
		{
			var layout = new PanelLayout();
			layout.SplitPanels.AddRange(new[]
			{
				new StoredSplitPanelLayout
				{
					Name = "west",
					Size = 250,
					PanelGroups = new [] {
						new PanelGroup { Panels = new [] { "Project Explorer" }.ToList() }
					}.ToList()
				},
				new StoredSplitPanelLayout
				{
					Name = "south",
					Size = 1,
					PanelGroups = new List<PanelGroup>()
				},
				new StoredSplitPanelLayout
				{
					Name = "east",
					Size = 266,
					PanelGroups = new []
					{
						new PanelGroup { Panels = new [] { "Chr Tiles" }.ToList() },
						new PanelGroup { Panels = new [] { "2x2 Metatiles", "4x4 Metatiles" }.ToList() },
						new PanelGroup { Panels = new [] { "Map Overview" }.ToList() }
					}.ToList()
				}
			});
			return layout;
		}
	}

	public class PanelLayout
	{
		[XmlElement(ElementName = "Parent")] public List<StoredSplitPanelLayout> SplitPanels = new List<StoredSplitPanelLayout>();
		[XmlElement(ElementName = "Float")] public List<StoredFloatPanel> FloatPanels = new List<StoredFloatPanel>();
	}

	public class StoredFloatPanel
	{
		public int Width;
		public int Height;
		public int Left;
		public int Top;
		[XmlElement(ElementName = "Panel")] public List<string> Panels = new List<string>();
	}
	public class StoredSplitPanelLayout
	{
		public int Size = -1;
		public string Name;
		[XmlElement(ElementName = "Group")] public List<PanelGroup> PanelGroups = new List<PanelGroup>();
	}

	public class PanelGroup
	{
		public int Size = -1;
		[XmlElement(ElementName = "Panel")] public List<string> Panels = new List<string>();
	}

	public enum LayoutMode
	{
		None, Default, MapEditor
	}
}
