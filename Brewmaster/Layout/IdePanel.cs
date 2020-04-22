using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrewMaster.Ide
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
		public HeaderPanel Header = new HeaderPanel { Dock = DockStyle.Top };

		public string Label
		{
			get { return Header.Label; }
			set { Header.Label = value; }
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
			childControl.Dock = DockStyle.Fill;
			Controls.Add(childControl);
			Controls.SetChildIndex(childControl, 0);
			var headerManupulator = childControl as IHeaderManupulator;
			if (headerManupulator != null) headerManupulator.Header = Header;
		}

		private void InitializeComponent()
		{
			Controls.Add(Header);
		}

	}
	public class IdeGroupedPanel : IdePanel
	{
		protected readonly TabControl _tabs = new TabControl { Dock = DockStyle.Fill, Alignment = TabAlignment.Bottom };


		public IdeGroupedPanel() : base()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			Controls.Add(_tabs);
			Controls.SetChildIndex(_tabs, 0);
			_tabs.SelectedIndexChanged += (sender, args) => { RefreshLabel(); };
		}

		private void RefreshLabel()
		{
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
		}

		public void ShowPanel(IdePanel panel)
		{
			var tabPage = _tabs.TabPages.OfType<IdeTabPage>().FirstOrDefault(t => t.Child == panel);
			if (tabPage != null) _tabs.SelectedTab = tabPage;
		}

		public IdeTabPage AddPanel(IdePanel panel, bool selectTab = false, int? index = null)
		{
			panel.ShowHeader = false;
			panel.GroupParent = this;
			var newTab = new IdeTabPage(panel);
			if (index.HasValue && index.Value < _tabs.TabCount) _tabs.TabPages.Insert(index.Value, newTab);
			else _tabs.TabPages.Add(newTab);

			if (selectTab) _tabs.SelectedTab = newTab;
			RefreshLabel();

			return newTab;
		}
		public void RemovePanel(IdePanel panel)
		{
			IdeTabPage removeTab = null;
			foreach (var tab in _tabs.TabPages.OfType<IdeTabPage>())
			{
				if (tab.Child == panel) removeTab = tab;
			}
			if (removeTab != null) _tabs.TabPages.Remove(removeTab);
			panel.ShowHeader = true;
			RefreshLabel();
		}

		public IEnumerable<IdePanel> Panels
		{
			get { return _tabs.TabPages.OfType<IdeTabPage>().Select(p => p.Child); }
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
