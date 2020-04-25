using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Brewmaster.Ide
{
	public class LayoutHandler
	{
		private readonly MainForm _form;
		private readonly Dictionary<IdePanel, PanelPosition> _memorizedPanelPositions = new Dictionary<IdePanel, PanelPosition>();
		public List<MultiSplitContainer> DockContainers { get; private set; }


		public LayoutHandler(MainForm mainForm)
		{
			_form = mainForm;
			DockContainers = new List<MultiSplitContainer>();
		}

		public void DockPanel(FloatPanel panel)
		{
			if (DockSuggestion == null) return;

			panel.SuspendLayout();
			_form.SuspendLayout();
			if (DockSuggestion.LayoutSplit != null)
			{
				var panelSize = panel.Size;
				var parent = DockSuggestion.LayoutSplit.Parent;
				var multiSplitParent = DockSuggestion.LayoutSplit.Parent as MultiSplitContainer;

				if (multiSplitParent != null)
				{
					AddPanelToSplitContainer(multiSplitParent, panel.ChildPanel, DockSuggestion.LayoutSplit.Index);
				}
				else { 
					var splitContainer = new SplitContainer();
					splitContainer.Dock = DockStyle.Fill;
					splitContainer.BorderStyle = BorderStyle.FixedSingle;
					if (!DockSuggestion.LayoutSplit.Horizontal) splitContainer.Orientation = Orientation.Horizontal;

					var list = new List<Control>();
					foreach (Control control in parent.Controls) list.Add(control);
					foreach (Control control in list) splitContainer.Panel1.Controls.Add(control);
					list.Clear();
					parent.Controls.Add(splitContainer);

					foreach (Control control in panel.Controls) list.Add(control);
					foreach (Control control in list) splitContainer.Panel2.Controls.Add(control);
				}
			}
			else if (DockSuggestion.JoinPanel is IdeGroupedPanel groupedPanel)
			{
				groupedPanel.AddPanel(panel.ChildPanel, true);
			}
			else
			{
				JoinPanels(DockSuggestion.JoinPanel, panel.ChildPanel);
			}
			_form.ResumeLayout();
			panel.ResumeLayout();
			panel.Close();
			panel.Dispose();

			_form.Focus();
			DockSuggestion = null;
		}

		private void AddPanelToSplitContainer(MultiSplitContainer multiSplitParent, IdePanel panel, int index)
		{
			var splitPanel = multiSplitParent.AddPanel(index);
			splitPanel.Add(panel);
		}

		public void ReleasePanel(IdePanel panel, Point offset)
		{
			var windowSize = panel.Size;
			var windowLocation = panel.PointToScreen(offset);
			RemovePanelFromSplitContainer(panel);

			_form.SuspendLayout();
			CreateFloatPanel(panel, windowLocation, windowSize);
			_form.ResumeLayout();
		}
		private void CreateFloatPanel(IdePanel panel, Point? location = null, Size? size = null)
		{
			var floatPanel = new FloatPanel(DockPanel, SuggestDock);
			floatPanel.SuspendLayout();
			floatPanel.SetChildPanel(panel);
			floatPanel.Show(_form);

			if (location.HasValue) floatPanel.Location = location.Value;
			if (size.HasValue) floatPanel.Size = size.Value;
			floatPanel.ResumeLayout();
		}

		private void RemovePanelFromSplitContainer(IdePanel panel)
		{
			var multiSplitPanel = panel.Parent as MultiSplitPanel;
			if (multiSplitPanel == null) multiSplitPanel = panel.Parent.Parent as MultiSplitPanel;
			if (multiSplitPanel != null)
			{
				var splitContainer = multiSplitPanel.Parent as MultiSplitContainer;
				_memorizedPanelPositions[panel] = new PanelPosition
					{
						SplitContainer = splitContainer,
						Index = splitContainer.Panels.IndexOf(multiSplitPanel)
					};
				splitContainer.RemovePanel(multiSplitPanel);
				if (splitContainer.Splits.Count == 0)
				{
					// TODO: Last panel removed, hide it but keep the dock suggestion available
				}
			}
		}
		private void RemovePanelFromGroupedPanel(IdePanel panel, IdeGroupedPanel groupedPanel)
		{
			var sibling = groupedPanel.Panels.FirstOrDefault(p => p != panel);
			if (sibling != null)
			{
				_memorizedPanelPositions[panel] = new PanelPosition
				{
					Index = groupedPanel.Tabs.FindIndex(t => t.Panel == panel),
					Sibling = sibling
				};
			}
			groupedPanel.RemovePanel(panel);
		}
		private void AddPanelToGroupedPanel(IdePanel sibling, IdePanel idePanel, int index)
		{
			// TODO: Method for identifying specific parent controls
			var parent = sibling.Parent;
			while (parent != null)
			{
				if (parent is IdeGroupedPanel groupedPanel)
				{
					groupedPanel.AddPanel(idePanel, true, index);
					return;
				}
				parent = parent.Parent;
			}
			// Sibling isn't in a groupedpanel anymore, so join panels in a new one
			JoinPanels(sibling, idePanel, index);
		}

		private void JoinPanels(IdePanel existingPanel, IdePanel newPanel, int newIndex = 1)
		{
			var parent = existingPanel.Parent;
			var groupedPanel = new IdeGroupedPanel {Dock = DockStyle.Fill};
			groupedPanel.AddPanel(newPanel);
			groupedPanel.AddPanel(existingPanel, false, newIndex > 0 ? 0 : 1);
			parent.Controls.Add(groupedPanel);
		}


		public void SuggestDock(Point cursorPosition)
		{
			foreach (var container in DockContainers)
			{
				if (CheckDockLocation(container)) return;
			}
			DockSuggestion = null;
		}

		private Overlay DockOverlay { get; set; }
		private DockSuggestion _dockSuggestion;

		public DockSuggestion DockSuggestion
		{
			get { return _dockSuggestion; }
			set
			{
				_dockSuggestion = value;
				if (_dockSuggestion == null && DockOverlay != null)
				{
					DockOverlay.Visible = false;
				}

				if (_dockSuggestion != null)
				{
					if (DockOverlay == null)
					{
						DockOverlay = new Overlay(_form);
						DockOverlay.Show(_form);
					}

					DockOverlay.Rectangle = _dockSuggestion.Bounds;
					DockOverlay.Visible = true;
					DockOverlay.Invalidate();

				}
			}
		}

		private bool CheckDockLocation(MultiSplitContainer container)
		{
			var horizontal = container.Horizontal;
			var absoluteBounds = container.Parent.RectangleToScreen(container.Bounds);
			if (!absoluteBounds.Contains(Cursor.Position)) return false;

			foreach (var panel in container.Panels)
			{
				var idePanel = panel.ControlContainer.Controls[0] as IdePanel;
				if (idePanel != null)
				{
					var headerBounds = idePanel.RectangleToScreen(idePanel.Header.Bounds);
					if (headerBounds.Contains(Cursor.Position))
					{
						DockSuggestion = new DockSuggestion
						{
							Bounds = headerBounds,
							JoinPanel = idePanel
						};
						return true;
					}
				}
			}

			var splits = new List<LayoutSplit>();
			var splitIndex = 0;
			var closestSplit = new LayoutSplit
			{
				Location = horizontal ? absoluteBounds.Left : absoluteBounds.Top,
				Index = splitIndex,
				Parent = container,
				Horizontal = horizontal
			};
			foreach (var split in container.Splits)
				splits.Add(new LayoutSplit
				{
					Location = closestSplit.Location + split,
					Index = ++splitIndex,
					Parent = container,
					Horizontal = horizontal
				});
			SuggestClosestSplit(closestSplit, splits, absoluteBounds, horizontal);
			return true;
		}

		private void SuggestClosestSplit(LayoutSplit closestSplit, List<LayoutSplit> splits, Rectangle absoluteBounds, bool horizontal)
		{
			var cursorIndex = (horizontal ? Cursor.Position.X : Cursor.Position.Y);
			var closestSplitDistance = Math.Abs(cursorIndex - closestSplit.Location);
			foreach (var split in splits)
			{
				var distance = Math.Abs(cursorIndex - split.Location);
				if (distance < closestSplitDistance)
				{
					closestSplitDistance = distance;
					closestSplit = split;
				}
			}

			Rectangle suggestion;
			if (horizontal)
			{
				var targetWidth = absoluteBounds.Width / (splits.Count + 1);
				suggestion = new Rectangle(closestSplit.Location - (targetWidth / 2), absoluteBounds.Top, targetWidth, absoluteBounds.Height);
				var oversized = suggestion.Right - absoluteBounds.Right;
				if (oversized > 0) suggestion.X -= oversized;
				oversized = absoluteBounds.Left - suggestion.Left;
				if (oversized > 0) suggestion.X += oversized;
			}
			else
			{
				var targetHeight = absoluteBounds.Height / (splits.Count + 1);
				suggestion = new Rectangle(absoluteBounds.Left, closestSplit.Location - (targetHeight / 2), absoluteBounds.Width, targetHeight);
				var oversized = suggestion.Bottom - absoluteBounds.Bottom;
				if (oversized > 0) suggestion.Y -= oversized;
				oversized = absoluteBounds.Top - suggestion.Top;
				if (oversized > 0) suggestion.Y += oversized;
			}
			if (DockSuggestion == null || suggestion != DockSuggestion.Bounds)
			{
				DockSuggestion = new DockSuggestion
				{
					Bounds = suggestion,
					LayoutSplit = closestSplit
				};
			}
		}

		private bool CheckDockLocation(SplitterPanel container, string name, bool horizontal)
		{
			var relativePosition = container.PointToClient(Cursor.Position);
			var absoluteBounds = container.Parent.RectangleToScreen(container.Bounds);

			if (!absoluteBounds.Contains(Cursor.Position)) return false;

			var splits = new List<LayoutSplit>();

			GetSplits(container, splits, horizontal);

			var closestSplit = new LayoutSplit
			{
				Location = horizontal ? absoluteBounds.Left : absoluteBounds.Top,
				Parent = container,
				Horizontal = horizontal
			};
			SuggestClosestSplit(closestSplit, splits, absoluteBounds, horizontal);
			return true;
		}

		private void GetSplits(Control container, List<LayoutSplit> splits, bool horizontal)
		{
			var splitChild = container.Controls.OfType<SplitContainer>().FirstOrDefault();

			if (splitChild == null)
			{
				splits.Add(new LayoutSplit
				{
					Location = horizontal ? container.Parent.RectangleToScreen(container.Bounds).Right : container.Parent.RectangleToScreen(container.Bounds).Bottom,
					Parent = container,
					Horizontal = horizontal
				});
				return;
			}

			GetSplits(splitChild.Panel1, splits, horizontal);
			GetSplits(splitChild.Panel2, splits, horizontal);
		}

		public void SetDockContainers(params MultiSplitContainer[] containers)
		{
			DockContainers = containers.ToList();
		}

		public void HidePanel(IdePanel panel)
		{
			Control parent = panel;
			while (parent != null)
			{
				if (parent is IdeGroupedPanel groupedPanel)
				{
					RemovePanelFromGroupedPanel(panel, groupedPanel);
					return;
				}
				parent = parent.Parent;
			}

			var panelForm = panel.FindForm();
			if (panelForm != null && !(panelForm is MainForm))
			{
				_memorizedPanelPositions.Remove(panel);
				panelForm.Visible = false;
				return;
			}

			RemovePanelFromSplitContainer(panel);
		}

		public static IdePanel GetPanel(Control control)
		{
			do
			{
				control = control.Parent;
			}
			while (control != null && !(control is IdePanel));

			return control as IdePanel;
		}

		public void ShowPanel(IdePanel panel)
		{
			var panelForm = panel.FindForm();
			if (panelForm != null && !(panelForm is MainForm))
			{
				panelForm.Visible = true;
				return;
			}

			if (!_memorizedPanelPositions.ContainsKey(panel))
			{
				// Panel was never visible in the first place, show as new form
				CreateFloatPanel(panel);
				return;
			}

			var position = _memorizedPanelPositions[panel];
			_form.SuspendLayout();
			if (position.Sibling != null)
				AddPanelToGroupedPanel(position.Sibling, panel, position.Index);
			else
				AddPanelToSplitContainer(position.SplitContainer, panel, Math.Min(position.Index, position.SplitContainer.Panels.Count));
			_form.ResumeLayout();
		}


		private struct PanelPosition
		{
			public int Index;
			public MultiSplitContainer SplitContainer;
			public IdePanel Sibling;
		}
	}


	public class LayoutSplit
	{
		public int Index { get; set; }
		public int Location { get; set; }
		public Control Parent { get; set; }
		public bool Horizontal { get; set; }
	}

	public class DockSuggestion
	{
		public Rectangle Bounds { get; set; }
		public LayoutSplit LayoutSplit { get; set; }
		public IdePanel JoinPanel { get; set; }
	}
}
