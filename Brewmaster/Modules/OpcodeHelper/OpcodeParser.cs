using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Brewmaster.ProjectModel;
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
		[XmlElement(ElementName = "Description")]
		public List<string> Description = new List<string>();
		[XmlElement(ElementName = "AddressingMode")]
		public List<AddressingMode> AddressingModes = new List<AddressingMode>();
		public AffectedFlag AffectedFlags;

		[XmlIgnore]
		public Dictionary<AffectedFlag, string> FlagExplanations = new Dictionary<AffectedFlag, string>();

		[XmlElement(ElementName = "FlagExplanation")]
		public List<FlagExplanation> SerialiazableFlagExplanations;
	}

	public class FlagExplanation
	{
		public string Explanation;
		public AffectedFlag Flag;
	}

	public class AddressingMode
	{
		public string Name;
		public string Bytes;
		public string Cycles;
	}
	public class OpcodeParser
	{
		private static Dictionary<ProjectType, Dictionary<string, Opcode>> _list = new Dictionary<ProjectType, Dictionary<string, Opcode>>();

		private static Regex _cleanText = new Regex(@"\s+");
		private static Regex _filterOpcode = new Regex(@"^[A-Z]{3}");
		public static Dictionary<string, Opcode> GetOpcodes(ProjectType type)
		{
			if (_list.ContainsKey(type)) return _list[type];

			switch (type)
			{
				case ProjectType.Nes:
					_list[type] = Parse6502Reference();
					break;
				case ProjectType.Snes:
					_list[type] = Parse65826Reference();
					break;
				default:
					return new Dictionary<string, Opcode>();
			}

#if DEBUG
			foreach (var opcode in _list[type].Values)
			{
				opcode.SerialiazableFlagExplanations = opcode.FlagExplanations.Select(kvp => new FlagExplanation { Explanation = kvp.Value, Flag = kvp.Key }).ToList();
			}
			using (var writer = File.OpenWrite(Path.Combine(Program.WorkingDirectory, "Opcodes." + type + ".xml"))) new XmlSerializer(typeof(List<Opcode>), new XmlRootAttribute("Opcodes")).Serialize(writer, _list[type].Values.ToList());
#endif
			return _list[type];
		}

		private static Dictionary<string, Opcode> Parse65826Reference()
		{
			var list = new Dictionary<string, Opcode>();
			var html = new HtmlDocument();
			html.LoadHtml(Resources._65816);
			var opcodeTables = html.DocumentNode.SelectNodes("//table");
			foreach (var table in opcodeTables)
			{
				var implied = !table.SelectSingleNode(".//tr/th[2]").InnerText.Equals("Addressing Mode", StringComparison.InvariantCultureIgnoreCase);
				foreach (var row in table.SelectNodes(".//tr"))
				{
					var cells = row.SelectNodes(".//td");
					if (cells == null || cells.Count < 6) continue;

					var match = _filterOpcode.Match(cells[0].InnerText.Trim());
					if (!match.Success) continue;
					var command = match.Value;
					Opcode opcode;
					if (!list.ContainsKey(command))
					{
						list.Add(command, opcode = new Opcode { Command = command });
						var element = table.SelectSingleNode("./preceding-sibling::h1[1]");
						opcode.Title = implied
							? string.Format("{0} - {1}", command, cells[1].InnerText.Trim())
							: element.InnerText.Trim();

						var parsedFlags = false;
						while ((element = element.SelectSingleNode("./following-sibling::*")) != table)
						{
							if (element.InnerText.Trim().StartsWith("Flags affected"))
							{

								foreach (var flagsAffectedLabel in element.SelectNodes(".//strong"))
								{
									if (!flagsAffectedLabel.InnerText.Trim().Contains(command) && parsedFlags) continue;

									var flagIndicator = flagsAffectedLabel.SelectSingleNode("./following-sibling::code")
										.InnerText.ToLower();
									opcode.AffectedFlags = 0;
									if (flagIndicator.Contains('c')) opcode.AffectedFlags |= AffectedFlag.C;
									if (flagIndicator.Contains('z')) opcode.AffectedFlags |= AffectedFlag.Z;
									if (flagIndicator.Contains('v')) opcode.AffectedFlags |= AffectedFlag.V;
									if (flagIndicator.Contains('n')) opcode.AffectedFlags |= AffectedFlag.N;

									parsedFlags = true;
								}
								continue;
							}

							if (!implied) opcode.Description.Add(element.InnerText.Trim());
						}
					}
					else  opcode = list[command];

					var addressingMode = new AddressingMode
					{
						Name = implied ? "Implied" : cells[1].InnerText.Trim(),
						Bytes = cells[3].InnerText.Trim(),
						Cycles = string.IsNullOrWhiteSpace(cells[5].InnerText)
							? string.Format("{0}", cells[4].InnerText.Trim())
							: string.Format("{0} ({1})", cells[4].InnerText.Trim(), cells[5].InnerText.Trim())
					};
					opcode.AddressingModes.Add(addressingMode);
				}
			}

			return list;
		}

		private static Dictionary<string, Opcode> Parse6502Reference()
		{
			var list = new Dictionary<string, Opcode>();
			var html = new HtmlDocument();
			html.LoadHtml(Resources._6502);
			var opcodeTable = html.DocumentNode.SelectSingleNode("//table");
			var opcodeCells = opcodeTable.SelectNodes(".//a");
			foreach (var cell in opcodeCells)
			{
				var opcode = new Opcode { Command = cell.InnerText };
				list.Add(opcode.Command, opcode);
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
						//Bytes = int.Parse(addressingRow.SelectSingleNode(".//td[3]").InnerText.Trim()),
						Bytes = addressingRow.SelectSingleNode(".//td[3]").InnerText.Trim(),
						Cycles = _cleanText.Replace(addressingRow.SelectSingleNode(".//td[4]").InnerText, " ").Trim()
					});
				}
			}
			return list;

		}
	}
}
