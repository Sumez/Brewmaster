using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Brewmaster.Modules
{
	public class MenuHelper : Component
	{
		private Action<string> _writeHelp;
		private MenuHelpCollection _helpItems;

		public MenuHelper()
		{
		}

		public MenuHelper(IContainer container)
		{
		}

		public void Prepare(IEnumerable<ToolStrip> toolStrips, Action<string> writeHelp)
		{
			_helpItems = MenuHelpCollection.Load(Path.Combine(Program.WorkingDirectory, @"MenuHelp.xml"));
			_writeHelp = writeHelp;

			foreach (var toolStrip in toolStrips)
			{
				Prepare(toolStrip.Items);
			}
		}

		private void Prepare(ToolStripItemCollection toolStripItems)
		{
			foreach (ToolStripItem item in toolStripItems)
			{
				if (item is ToolStripSeparator) continue;
				if (item is ToolStripMenuItem menuItem) Prepare(menuItem.DropDownItems);
				item.MouseEnter += (s, a) =>
				{
					if (_writeHelp != null && !string.IsNullOrWhiteSpace(item.Name)) _writeHelp(_helpItems.Get(item.Name));
				};
				item.MouseLeave += (s, a) =>
				{
					if (_writeHelp != null) _writeHelp(null);
				};
			}
		}
	}

	[Serializable, XmlRoot("MenuHelpCollection")]
	public class MenuHelpCollection : List<MenuHelp>
	{
		private string _fileName;
		public static MenuHelpCollection Load(string fileName)
		{
			if (!File.Exists(fileName)) return new MenuHelpCollection { _fileName = fileName };
			var serializer = new XmlSerializer(typeof(MenuHelpCollection));
			using (var reader = File.OpenRead(fileName))
			{
				var collection = (MenuHelpCollection)serializer.Deserialize(reader);
				collection._fileName = fileName;
				return collection;
			}
		}

		[XmlIgnore]
		private Dictionary<string, string> _dictionary;

		public string Get(string key)
		{
			if (_dictionary == null) _dictionary = this.ToDictionary(h => h.MenuItem, h => h.HelpText);
			if (!_dictionary.ContainsKey(key)) Add(key, null);
			return _dictionary[key];
		}

		private void Add(string menuItem, string helpText)
		{
			_dictionary.Add(menuItem, helpText);
			Add(new MenuHelp {MenuItem = menuItem, HelpText = helpText ?? " "});
#if DEBUG
			using (var writer = File.OpenWrite(_fileName)) new XmlSerializer(typeof(MenuHelpCollection)).Serialize(writer, this);
#endif
		}
	}
	[Serializable]
	public class MenuHelp
	{
		[XmlElement(ElementName = "MenuItem")]
		public string MenuItem;
		[XmlElement(ElementName = "HelpText")]
		public string HelpText;
	}
}
