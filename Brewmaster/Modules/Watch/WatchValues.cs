using Brewmaster.Emulation;
using Brewmaster.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.Modules.Watch
{
	public partial class WatchValues : UserControl
	{
		public Action<MemoryState> SetData { get; set; }
		public Func<string, DebugSymbol> GetSymbol { set { watchList.GetSymbol = value; } }
		public Action<int, Breakpoint.Types, Breakpoint.AddressTypes, string> AddBreakpoint { set { watchList.AddBreakpoint = value; } }

		public WatchValues()
		{
			InitializeComponent();

			SetData = watchList.SetData;
		}


		public void AddWatch(string expression, bool word = false)
		{
			watchList.AddWatch(expression, word);
		}

		public List<WatchValueData> GetSerializableData()
		{
			return watchList.Items.OfType<WatchValue>().Select(v => new WatchValueData
			{
				Expression = v.Text,
				Decimal = v.ShowAsDecimal,
				Word = v.ShowAsWord
			}).ToList();
		}

		public void SetWatchValues(IEnumerable<WatchValueData> watchData)
		{
			watchList.ClearValues();
			foreach (var watch in watchData) watchList.AddWatch(watch.Expression, watch.Word, watch.Decimal);
		}

		public void Clear()
		{
			watchList.ClearValues();
		}
	}

	[Serializable]
	public class WatchValueData
	{
		public string Expression;
		public bool Word;
		public bool Decimal;
	}
}
