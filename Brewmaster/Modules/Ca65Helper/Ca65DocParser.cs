using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Brewmaster.Properties;
using HtmlAgilityPack;

namespace Brewmaster.Modules.Ca65Helper
{
	public class Ca65Command
	{
		public List<string> Aliases = new List<string>();
		public List<string> SeeAlso = new List<string>();
		public List<Ca65CommandDescription> Description = new List<Ca65CommandDescription>();

		public override string ToString()
		{
			return string.Join(Environment.NewLine, Description.Select(d => d.Text));
		}
	}
	public class Ca65CommandDescription
	{
		public string Text;
		public bool CodeExample;
	}

	public class Ca65Parser
	{
		private static Dictionary<string, Ca65Command> _list;

		private static Regex _cleanText = new Regex(@"\s+");
		public static Dictionary<string, Ca65Command> GetCommands()
		{
			if (_list != null) return _list;

			_list = new Dictionary<string, Ca65Command>();
			var html = new HtmlDocument();
			html.LoadHtml(Resources.ca65);
			var header = html.DocumentNode.SelectSingleNode("//a[@name='pseudo-variables']").ParentNode;
			var lastHeader = html.DocumentNode.SelectSingleNode("//a[@name='macros']").ParentNode;

			while ((header = header.SelectSingleNode("./following-sibling::h2")) != lastHeader)
			{
				var commandHeader = header.SelectSingleNode(".//code");
				if (commandHeader == null) continue;
				var ca65Command = new Ca65Command();
				var commands = commandHeader.InnerText.Trim().Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
				foreach (var command in commands.Where(c => c.StartsWith(".")))
				{
					ca65Command.Aliases.Add(command);
					_list[command] = ca65Command;
				}

				if (ca65Command.Aliases.Count == 0) continue;

				var descriptionNode = header;
				while ((descriptionNode = descriptionNode.SelectSingleNode("./following-sibling::*")).Name != "h2")
				{
					var text = _cleanText.Replace(descriptionNode.InnerText, " ").Trim();
					if (string.IsNullOrWhiteSpace(text)) continue;
					
					if (text.StartsWith("See also") || text.StartsWith("See:"))
					{
						foreach (var referenceNode in descriptionNode.SelectNodes(".//a"))
						{
							if (referenceNode.InnerText.Trim().StartsWith(".")) ca65Command.SeeAlso.Add(referenceNode.InnerText.Trim());
						}
						continue;
					}

					var isCode = descriptionNode.Name == "blockquote";
					ca65Command.Description.Add(new Ca65CommandDescription
					{
						Text = isCode ? descriptionNode.SelectSingleNode(".//pre").InnerText : text,
						CodeExample = isCode
					});
				}
			}

			return _list;
		}

		public static Ca65Command GetCommandFromWord(string word)
		{
			var knownCommands = GetCommands();
			if (knownCommands.ContainsKey(word.ToUpper())) return knownCommands[word.ToUpper()];
			else return null;
		}
	}
}
