using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Brewmaster.Properties;
using HtmlAgilityPack;

namespace Brewmaster.Modules.OpcodeHelper
{
	[Flags]
	public enum AffectedFlag
	{
		C = 1,
		Z = 2,
		N = 4,
		V = 8
	}
	public class Opcode
	{
		public string Command;
		public string Title;
		public List<string> Description = new List<string>();
		public List<AddressingMode> AddressingModes = new List<AddressingMode>();
		public AffectedFlag AffectedFlags;
		public Dictionary<AffectedFlag, string> FlagExplanations = new Dictionary<AffectedFlag, string>();
	}

	public class AddressingMode
	{
		public string Name;
		public int Bytes;
		public string Cycles;
	}
	public class OpcodeParser
	{
		private static Dictionary<string, Opcode> _list;

		private static Regex _cleanText = new Regex(@"\s+");
		public static Dictionary<string, Opcode> GetOpcodes()
		{
			if (_list != null) return _list;

			_list = new Dictionary<string, Opcode>();
			var html = new HtmlDocument();
			html.LoadHtml(Resources._6502);
			var opcodeTable = html.DocumentNode.SelectSingleNode("//table");
			var opcodeCells = opcodeTable.SelectNodes(".//a");
			foreach (var cell in opcodeCells)
			{
				var opcode = new Opcode { Command = cell.InnerText };
				_list.Add(opcode.Command, opcode);
				var element = cell.SelectSingleNode(string.Format("//a[@name='{0}']", cell.GetAttributeValue("href", "#").Substring(1))).ParentNode;
				opcode.Title = element.InnerText;

				while (true)
				{
					element = element.SelectSingleNode("./following-sibling::*");
					if (element.Name != "p" || element.SelectSingleNode(".//table") != null || element.InnerText.StartsWith("Processor Status")) break;
					if (!string.IsNullOrWhiteSpace(element.InnerText)) opcode.Description.Add(_cleanText.Replace(element.InnerText, " ").Trim());
				}

				var processorTable = element.Name == "table" ? element : element.SelectSingleNode("./following-sibling::table");
				while (processorTable == null)
				{
					element = element.SelectSingleNode("./following-sibling::p");
					processorTable = element.SelectSingleNode(".//table");
				}

				var cText = processorTable.SelectSingleNode(".//td[a/text()='C']/following-sibling::td/following-sibling::td").InnerText.Trim();
				var nText = processorTable.SelectSingleNode(".//td[a/text()='N']/following-sibling::td/following-sibling::td").InnerText.Trim();
				var zText = processorTable.SelectSingleNode(".//td[a/text()='Z']/following-sibling::td/following-sibling::td").InnerText.Trim();
				var vText = processorTable.SelectSingleNode(".//td[a/text()='V']/following-sibling::td/following-sibling::td").InnerText.Trim();

				if (cText != "Not affected")
				{
					opcode.AffectedFlags |= AffectedFlag.C;
					opcode.FlagExplanations.Add(AffectedFlag.C, cText);
				}

				if (nText != "Not affected")
				{
					opcode.AffectedFlags |= AffectedFlag.N;
					opcode.FlagExplanations.Add(AffectedFlag.N, nText);
				}

				if (zText != "Not affected")
				{
					opcode.AffectedFlags |= AffectedFlag.Z;
					opcode.FlagExplanations.Add(AffectedFlag.Z, zText);
				}

				if (vText != "Not affected")
				{
					opcode.AffectedFlags |= AffectedFlag.V;
					opcode.FlagExplanations.Add(AffectedFlag.V, vText);
				}

				var addressingTable = processorTable.SelectSingleNode("./following-sibling::table");
				var addressingModes = addressingTable.SelectNodes(".//tr[position()>1]");
				foreach (var addressingRow in addressingModes)
				{
					opcode.AddressingModes.Add(new AddressingMode
					{
						Name = _cleanText.Replace(addressingRow.SelectSingleNode(".//td[1]").InnerText, " ").Trim(),
						Bytes = int.Parse(addressingRow.SelectSingleNode(".//td[3]").InnerText.Trim()),
						Cycles = _cleanText.Replace(addressingRow.SelectSingleNode(".//td[4]").InnerText, " ").Trim()
					});
				}
			}
			return _list;
		}
	}
}
