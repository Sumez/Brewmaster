using System.Collections.Generic;
using System.Windows.Forms;
using Brewmaster.Emulation;
using Brewmaster.Modules.Watch;
using Brewmaster.ProjectModel;
using Mesen.GUI.Debugger.PpuViewer;

namespace Brewmaster.Modules.SpriteList
{
	public class VirtualListView : ListView
	{
		public VirtualListView()
		{
			GridLines = true;
			View = View.Details;
			VirtualMode = true;
			DoubleBuffered = true;
		}
	}
	public class SpriteList : Control
	{
		private readonly Events _events;
		private ListView _listView;
		private SpriteData _spriteInfo;

		public SpriteList(Events events)
		{
			_events = events;
			_events.EmulationStateUpdate += UpdateSpriteList;
			_events.SelectedSpriteChanged += index =>
			{
				if (_listView.SelectedIndices.Contains(index)) return;
				_listView.SelectedIndices.Clear();
				_listView.SelectedIndices.Add(index);
			};
			_listView = new VirtualListView
			{
				Size = Size,
				FullRowSelect = true,
				HideSelection = false,
				Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
			};
			_listView.RetrieveVirtualItem += GetListItem;
			_listView.ItemSelectionChanged += SelectedItem;
			Controls.Add(_listView);

			_listView.Columns.Add("#", 40);
			_listView.Columns.Add("Tile");
			_listView.Columns.Add("Position");
			_listView.Columns.Add("Palette");
			_listView.Columns.Add("Priority");
			_listView.Columns.Add("Flip");
			_listView.Columns.Add("Size");
			_listView.VirtualListSize = 0;
		}

		private void SelectedItem(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			_events.SelectSprite(e.ItemIndex);
		}

		private void UpdateSpriteList(EmulationState state)
		{
			_spriteInfo = state.Sprites;
			_listView.BeginUpdate();
			_listView.VirtualListSize = state.Type == ProjectType.Snes ? 128 : 64;
			_listView.EndUpdate();
		}

		private void GetListItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			var sprite = _spriteInfo.Details[e.ItemIndex];

			e.Item = new ListViewItem(new[]
			{
				e.ItemIndex.ToString(),
				string.Format(@"{0}{1}", WatchValue.FormatHex(sprite.TileIndex, 2), sprite.UseSecondTable ? " (2nd)" : ""),
				string.Format(@"{0}, {1}", sprite.X, sprite.Y),
				sprite.Palette.ToString(),
				sprite.Priority.ToString(),
				string.Format(@"{0}{1}", sprite.FlipH ? "H" : "", sprite.FlipV ? "V" : ""),
				string.Format(@"{0}x{1}", sprite.Width, sprite.Height)
			});
		}
	}
}
