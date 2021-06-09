using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Brewmaster.Modules
{
	[Serializable, XmlRoot("Language")]
	public class LanguageHandler : List<LanguageString>
	{
		private string _fileName;
		public static LanguageHandler Load(string fileName)
		{
			if (!File.Exists(fileName)) return new LanguageHandler { _fileName = fileName };
			var serializer = new XmlSerializer(typeof(LanguageHandler));
			using (var reader = File.OpenRead(fileName))
			{
				var collection = (LanguageHandler)serializer.Deserialize(reader);
				collection._fileName = fileName;
				return collection;
			}
		}

		[XmlIgnore]
		private Dictionary<string, string> _dictionary;

		public string Get(string key)
		{
			if (_dictionary == null) _dictionary = this.ToDictionary(h => h.Key, h => h.Value);
			if (!_dictionary.ContainsKey(key)) Add(key, null);
			return _dictionary[key];
		}

		private void Add(string key, string value)
		{
			_dictionary.Add(key, value);
			Add(new LanguageString { Key = key, Value = value ?? " " });
#if DEBUG
			using (var writer = File.OpenWrite(_fileName)) new XmlSerializer(typeof(LanguageHandler)).Serialize(writer, this);
#endif
		}
	}

	[Serializable]
	public class LanguageString
	{
		[XmlElement(ElementName = "Key")]
		public string Key;
		[XmlElement(ElementName = "Value")]
		public string Value;
	}

}