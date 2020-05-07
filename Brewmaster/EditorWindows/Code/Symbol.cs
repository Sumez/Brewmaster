
using System.Collections.Generic;

namespace Brewmaster.EditorWindows.Code
{
	public class Symbol
	{
		public string Text { get; set; }
		public string Source { get; set; }
		public string Value { get; set; }
		public int Line { get; set; }
		public int Character { get; set; }
		public bool Public { get; set; }
		public string LocalToFile { get; set; }

		public override string ToString()
		{
			return Text;
		}
	}
	public class MacroSymbol : Symbol
	{
		public List<string> Parameters { get; set; }
		public override string ToString()
		{
			return string.Format("{0} {1}", Text, string.Join(", ", Parameters));
		}
	}
}
