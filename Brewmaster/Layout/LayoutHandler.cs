using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BrewMaster.Ide
{
	public class LayoutHandler
	{
		private readonly MainForm Form;
		private readonly Dictionary<Control, PanelPosition> _memorizedPanelPositions = new Dictionary<Control, PanelPosition>();
		public List<MultiSplitContainer> DockContainers { get; private set; }


		public LayoutHandler(MainForm mainForm)
		{
			Form = mainForm;
			DockContainers = new List<MultiSplitContainer>();
		}

		public void DockPanel(FloatPanel panel)
		{
			if (DockSuggestion.HasValue && _suggestedSplit != null)
			{
				var panelSize = panel.Size;
				var parent = _suggestedSplit.Parent;
				var multiSplitParent = _suggestedSplit.Parent as MultiSplitContainer;
				panel.SuspendLayout();
				Form.SuspendLayout();

				if (multiSplitParent != null)
				{
					AddPanelToSplitContainer(multiSplitParent, panel, _suggestedSplit.Index);
				}
				else { 
					var splitContainer = new SplitContainer();
					splitContainer.Dock = DockStyle.Fill;
					splitContainer.BorderStyle = BorderStyle.FixedSingle;
					if (!_suggestedSplit.Horizontal) splitContainer.Orientation = Orientation.Horizontal;

					var list = new List<Control>();
					foreach (Control control in parent.Controls) list.Add(control);
					foreach (Control control in list) splitContainer.Panel1.Controls.Add(control);
					list.Clear();
					parent.Controls.Add(splitContainer);

					foreach (Control control in panel.Controls) list.Add(control);
					foreach (Control control in list) splitContainer.Panel2.Controls.Add(control);
				}
				Form.ResumeLayout();
				panel.ResumeLayout();
				panel.Close();
				panel.Dispose();

				Form.Focus();
			}
			DockSuggestion = null;
			_suggestedSplit = null;
		}

		private void AddPanelToSplitContainer(MultiSplitContainer multiSplitParent, Control panel, int index)
		{
			var splitPanel = multiSplitParent.AddPanel(index);
			var list = new List<Control>();
			foreach (Control control in panel.Controls) list.Add(control);
			foreach (var control in list) splitPanel.Add(control);
		}

		public void ReleasePanel(Control panel, Point offset)
		{
			var windowSize = panel.Size;
			var windowLocation = panel.PointToScreen(offset);
			var floatPanel = new FloatPanel(DockPanel, SuggestDock);
			floatPanel.SuspendLayout();
			Form.SuspendLayout();
			var list = new List<Control>();
			foreach (Control control in panel.Controls) list.Add(control);
			foreach (Control control in list) floatPanel.Controls.Add(control);
			list.Clear();

			RemovePanelFromSplitContainer(panel);
			
			floatPanel.Show(Form);
			floatPanel.Location = windowLocation;
			floatPanel.Size = windowSize;
			floatPanel.ResumeLayout();
			Form.ResumeLayout();
		}

		public void CreateFloatPanel(IdePanel panel)
		{
			var floatPanel = new FloatPanel(DockPanel, SuggestDock);
			floatPanel.SuspendLayout();
			floatPanel.Controls.Add(panel);

			floatPanel.Show(Form);
			//floatPanel.Location = windowLocation;
			//floatPanel.Size = windowSize;
			floatPanel.ResumeLayout();
		}

		private void RemovePanelFromSplitContainer(Control panel)
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
			}
		}
		private void RemovePanelFromGroupedPanel(IdePanel panel, IdeGroupedPanel groupedPanel)
		{
			var sibling = groupedPanel.Panels.FirstOrDefault(p => p != panel);
			if (sibling != null)
			{
				_memorizedPanelPositions[panel] = new PanelPosition
				{
					Index = groupedPanel.Panels.ToList().IndexOf(panel),
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
			// TODO: If sibling isn't in a groupedpanel anymore, create one
		}



		public void SuggestDock(Point cursorPosition)
		{
			var match = false;
			foreach (var container in DockContainers)
			{
				match = CheckDockLocation(container);
				if (match) break;
			}
			/*match = CheckDockLocation(Form.NorthSouthContainer.Panel2, "south", true) ||
			CheckDockLocation(Form.WestCenterContainer.Panel1, "west", false) ||
			CheckDockLocation(Form.MainEastContainer.Panel2, "east", false);*/

			if (!match && DockSuggestion.HasValue)
			{
				DockSuggestion = null;
				_suggestedSplit = null;

			}
		}

		private Overlay DockOverlay { get; set; }
		private Rectangle? _dockSuggestion;

		public Rectangle? DockSuggestion
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
						DockOverlay = new Overlay(Form);
						DockOverlay.Show(Form);
					}

					DockOverlay.Rectangle = _dockSuggestion.Value;
					DockOverlay.Visible = true;
					DockOverlay.Invalidate();

				}
			}
		}

		private LayoutSplit _suggestedSplit;
		private bool CheckDockLocation(MultiSplitContainer container)
		{
			var horizontal = container.Horizontal;
			var absoluteBounds = container.Parent.RectangleToScreen(container.Bounds);
			if (!absoluteBounds.Contains(Cursor.Position)) return false;

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
			if (DockSuggestion == null || suggestion != DockSuggestion.Value)
			{
				DockSuggestion = suggestion;
				_suggestedSplit = closestSplit;

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

		public void HidePanel(Control windowControl)
		{
			var idePanel = GetPanel(windowControl);
			Control parent = idePanel;
			while (parent != null)
			{
				if (parent is IdeGroupedPanel groupedPanel)
				{
					RemovePanelFromGroupedPanel(idePanel, groupedPanel);
					return;
				}
				parent = parent.Parent;
			}

			var panelForm = windowControl.FindForm();
			if (panelForm != null && !(panelForm is MainForm))
			{
				panelForm.Visible = false;
				return;
			}

			if (idePanel != null) RemovePanelFromSplitContainer(idePanel);
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

		public void ShowPanel(Control windowControl)
		{
			var panelForm = windowControl.FindForm();
			if (panelForm != null && !(panelForm is MainForm))
			{
				panelForm.Visible = true;
				return;
			}

			var idePanel = GetPanel(windowControl);
			if (idePanel == null) return;
			if (!_memorizedPanelPositions.ContainsKey(idePanel))
			{
				// Panel was never visible in the first place, show as new form
				CreateFloatPanel(idePanel);
				return;
			}

			var position = _memorizedPanelPositions[idePanel];
			if (position.Sibling != null)
			{
				AddPanelToGroupedPanel(position.Sibling, idePanel, position.Index);
			}
			else
			{
				AddPanelToSplitContainer(position.SplitContainer, idePanel, Math.Max(position.Index, position.SplitContainer.Panels.Count));
			}
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
}
