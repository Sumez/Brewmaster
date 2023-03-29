using System;
using System.Collections.Generic;

namespace Brewsic
{
	public class Pattern
	{
		public Pattern() { }

		/// <summary>
		/// Creates an empty pattern with a set number of rows
		/// </summary>
		/// <param name="rows"></param>
		public Pattern(UInt16 rows)
		{
			Rows = rows;
		}

		public ushort Rows { get; set; }
		public List<PatternNote[]> Channels { get; set; } = new List<PatternNote[]>();
	}
}