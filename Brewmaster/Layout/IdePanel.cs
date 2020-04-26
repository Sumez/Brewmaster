using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.Ide
{
	public static class IdeExtensions
	{
		public static void UpdateLabel(this Control control, string newLabel)
		{
			var parentPanel = control.Parent as IdePanel;
			if (parentPanel != null)
			{
				parentPanel.Label = newLabel;
				var tabPage = parentPanel.Parent as IdeTabPage;
				if (tabPage != null) tabPage.ResetText();
			}
		}
	}

	public class IdePanel : Panel
	{
		public event Action<string> LabelChanged;
		public HeaderPanel Header = new HeaderPanel { Dock = DockStyle.Top };

		public string Label
		{
			get { return Header.Label; }
			set
			{
				Header.Label = value;
				if (LabelChanged != null) LabelChanged(value);
			}
		}

		public bool ShowHeader
		{
			get { return Header.Visible; }
			set { Header.Visible = value; }
		}
		public IdeGroupedPanel GroupParent { get; set; }

		public IdePanel()
		{
			InitializeComponent();
		}

		public IdePanel(Control childControl): this()
		{
			Child = childControl;
			childControl.Dock = DockStyle.Fill;
			Controls.Add(childControl);
			Controls.SetChildIndex(childControl, 0);
			var headerManupulator = childControl as IHeaderManupulator;
			if (headerManupulator != null) headerManupulator.Header = Header;
		}

		public Control Child { get; protected set; }

		private void InitializeComponent()
		{
			Controls.Add(Header);
		}
	}

	public class IdeGroupedPanel : IdePanel
	{
		private IdeTab _selectedTab;
		public List<IdeTab> Tabs = new List<IdeTab>();

		private void SelectTab(IdeTab targetTab)
		{
			if (targetTab == _selectedTab) return;
			_selectedTab = targetTab;
			SuspendLayout();
			foreach (var tab in Tabs.Where(t => t != _selectedTab)) tab.Panel.Visible = false;
			_selectedTab.Panel.Visible = true;
			ResumeLayout();
		}


		public IdeGroupedPanel() : base()
		{
		}

		private void RefreshLabel()
		{
			/*
			Label = _tabs.TabPages.Count > 0 ? (_tabs.SelectedTab ?? _tabs.TabPages[0]).Text : "";

			if (_tabs.TabPages.Count > 1)
			{
				_tabs.ItemSize = Size.Empty;
				_tabs.SizeMode = TabSizeMode.Normal;
			}
			else
			{
				_tabs.ItemSize = new Size(0, 1);
				_tabs.SizeMode = TabSizeMode.Fixed;
			}
			*/
		}

		public void ShowPanel(IdePanel panel)
		{
			var tab = Tabs.FirstOrDefault(t => t.Panel == panel);
			if (tab != null) tab.Selected = true;
		}

		public void AddPanel(IdePanel panel, bool selectTab = false, int? index = null)
		{
			if (panel is IdeGroupedPanel groupedPanel)
			{
				foreach (var newTab in groupedPanel.Tabs)
				{
					AddPanel(newTab.Panel, selectTab && newTab.Selected);
				}
				groupedPanel.Dispose();
				return;
			}
			panel.ShowHeader = false;
			panel.GroupParent = this;

			if (index.HasValue && index.Value >= Tabs.Count) index = null;

			var tab = index.HasValue
				? Header.InsertTab(index.Value, panel)
				: Header.AddTab(panel);
			panel.Dock = DockStyle.Fill;

			tab.WasSelected += () => SelectTab(tab);
			if (index.HasValue) Tabs.Insert(index.Value, tab);
			else Tabs.Add(tab);
			if (selectTab || Tabs.Count == 1) tab.Selected = true;
			else panel.Visible = false;

			Controls.Add(panel);
			Controls.SetChildIndex(panel, 0);
			RefreshLabel();
		}

		public void RemovePanel(IdePanel panel)
		{
			var tab = Tabs.FirstOrDefault(t => t.Panel == panel);
			if (tab == null) return;
			if (Tabs.Count == 1) throw new Exception("Can't remove the last tab in a grouped panel");
			Controls.Remove(tab.Panel);
			Tabs.Remove(tab);
			tab.Panel.Visible = true;
			tab.Panel.ShowHeader = true;
			tab.Panel.GroupParent = null;

			if (Tabs.Count == 1) // Only one tab remaining, so remove grouped panel
			{
				var lastPanel = Tabs[0].Panel;
				var myParent = Parent;
				// Remove myself before adding child panel to avoid confusing floatpanels
				myParent.Controls.Remove(this);
				myParent.Controls.Add(lastPanel);
				lastPanel.Visible = true;
				lastPanel.ShowHeader = true;
				lastPanel.GroupParent = null;
				Dispose();
				return;
			}
			if (tab.Selected)
			{
				Tabs.First(t => t != tab).Selected = true;
			}
			Header.RemoveTab(tab);
			RefreshLabel();
		}

		public IEnumerable<IdePanel> Panels
		{
			get { return Tabs.Select(t => t.Panel); }
		}

	}

	public class IdeTabPage : TabPage
	{
		private IdePanel _child;

		public override string Text
		{
			get { return _child != null ? _child.Label : ""; }
		}

		public IdePanel Child
		{
			get { return _child; }
		}

		public IdeTabPage(IdePanel child) : base()
		{
			_child = child;
			_child.Dock = DockStyle.Fill;
			Controls.Add(child);
		}
	}

	public interface IHeaderManupulator
	{
		HeaderPanel Header { get; set; }
	}
}
