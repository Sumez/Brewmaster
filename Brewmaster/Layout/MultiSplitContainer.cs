using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Brewmaster.ProjectExplorer;

namespace Brewmaster.Layout
{
	[Designer(typeof(ParentControlDesigner))]
	public class MultiSplitPanel : Panel
	{
		private int _staticWidth;
		public Panel ControlContainer { get; set; }
		public Control ContainedControl { get { return ControlContainer.Controls[0]; } }

		public int StaticWidth
		{
			get { return _staticWidth; }
			set
			{
				_staticWidth = value;
				((MultiSplitContainer)Parent).Fit();
			}
		}

		public MultiSplitPanel()
		{
			//newPanel.BorderStyle = BorderStyle.FixedSingle;
			BackColor = SystemColors.ActiveBorder;
			Padding = new Padding(1, 1, 1, 1);

			ControlContainer = new Panel();
			ControlContainer.BackColor = SystemColors.Control;
			ControlContainer.Dock = DockStyle.Fill;
			Controls.Add(ControlContainer);

		}
		
		public void Add(Control control)
		{
			ControlContainer.Controls.Add(control);
			if (control is MultiSplitContainer) Padding = Padding.Empty;
		}
	}

	[Designer(typeof(MultiSplitDesigner))]
	public class MultiSplitContainer : ContainerControl
	{
		public int BorderWidth { get; set; } = 1;
		public List<MultiSplitPanel> Panels { get; private set; }
		public List<int> Splits { get; private set; }
		public bool Horizontal { get; set; }
		private int FullSize { get { return Horizontal ? Width : Height; } }

		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		//[Localizable(false)]
		public MultiSplitPanel FirstPanel { get; private set; }
		public MultiSplitPanel SecondPanel { get; private set; }
		public MultiSplitPanel ThirdPanel { get; private set; }
		//public Panel FourthPanel { get; set; }

		private static Form _overlay;
		public static bool LiveDrag { get; set; }

		public MultiSplitContainer()
		{
			Horizontal = true;

			Panels = new List<MultiSplitPanel>();
			Splits = new List<int>();
			_oldWidth = Width;
			_oldHeight = Height;

			var mouseHandler = new OsFeatures.GlobalMouseHandler();
			mouseHandler.MouseMoved += l => HandleMouseEvent(l, HandleMouseMovement);
			mouseHandler.MouseUp += l => HandleMouseEvent(l, HandleMouseUp);
			mouseHandler.MouseDown += l => HandleMouseEvent(l, HandleMouseDown);

			Application.AddMessageFilter(mouseHandler);
			HandleDestroyed += (s, a) => Application.RemoveMessageFilter(mouseHandler);
		}

		private int _oldWidth;
		private int _oldHeight;
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			if (_oldHeight == Height && _oldWidth == Width && e.AffectedProperty != "Visible") return;
			ResizePanels(Horizontal ? (Width - _oldWidth) : (Height - _oldHeight), true);
			_oldWidth = Width;
			_oldHeight = Height;
		}
		protected virtual void ResizePanels(int deltaWidth, bool fit = false)
		{
			if (Splits.Count == 0) return;

			var currentSplit = 0;
			var oldSplit = 0;
			//var oldWidth = Splits[Splits.Count - 1];
			var oldWidth = 0;
			for (var i = 0; i < Panels.Count; i++)
			{
				if (Panels[i].StaticWidth == 0) oldWidth += Splits[i] - (i == 0 ? 0 : Splits[i - 1]);
			}
			for (var i = 0; i < Panels.Count; i++)
			{
				var splitWidth = Splits[i] - oldSplit;
				var panelDelta = 
					Panels[i].StaticWidth > 0 ? 0
					: (oldWidth > 0 ? ((float)splitWidth / oldWidth) : (1f / Panels.Count(p => p.StaticWidth == 0))) * deltaWidth;
				oldSplit = Splits[i];
				currentSplit = Splits[i] = (int)(currentSplit + splitWidth + panelDelta);
			}

			if (fit) Splits[Splits.Count - 1] = FullSize;
			AdjustPanels();
		}

		public virtual void Fit()
		{
			if (Panels.Count == 0) return;
			for (var i = 0; i < Panels.Count - 1; i++)
			{
				if (Panels[i].StaticWidth > 0)
				{
					Splits[i] = Panels[i].StaticWidth + (i == 0 ? 0 : Splits[i - 1]);
				}
			}
			if (Panels.Count > 1 && Panels[Panels.Count - 1].StaticWidth > 0)
			{
				Splits[Splits.Count - 2] = Splits[Splits.Count - 1] - Panels[Panels.Count - 1].StaticWidth;
			}
			ResizePanels(0, true);
		}
		protected virtual void AdjustPanels()
		{
			for (var i = 0; i < Panels.Count; i++)
			{
				var left = i == 0 ? 0 : (Splits[i - 1] + 2);
				if (Horizontal) { 
					Panels[i].Width = Splits[i] - left - (i == Panels.Count - 1 ? 0 : 2);
					Panels[i].Left = left;
					Panels[i].Top = 0;
					Panels[i].Height = Height;
				}
				else
				{
					Panels[i].Height = Splits[i] - left - (i == Panels.Count - 1 ? 0 : 2);
					Panels[i].Top = left;
					Panels[i].Left = 0;
					Panels[i].Width = Width;
				}
			}
		}

		public MultiSplitPanel AddPanel(Control addControl, int index = -1)
		{
			addControl.Dock = DockStyle.Fill;
			var panel = AddPanel(index);
			panel.Add(addControl);
			return panel;
		}
		public MultiSplitPanel AddPanel(int index = -1)
		{
			SuspendLayout();

			if (index < 0) index = Panels.Count;
			var addSize = FullSize / (Panels.Count + 1);
			ResizePanels(-addSize);
			Splits.Add(FullSize);

			var newPanel = new MultiSplitPanel
			{
				Height = Height,
				Top = 0,
				Padding = new Padding(BorderWidth, BorderWidth, BorderWidth, BorderWidth)
			};

			Panels.Insert(index, newPanel);
			Controls.Add(newPanel);
			Fit();
			AdjustPanels();

			if (index == 0) FirstPanel = newPanel;
			if (index == 1) SecondPanel = newPanel;
			if (index == 2) ThirdPanel = newPanel;


			ResumeLayout();
			return newPanel;
		}

		public void RemovePanel(MultiSplitPanel panel)
		{
			var index = Panels.IndexOf(panel);
			if (index >= 0)
			{
				SuspendLayout();
				Panels.RemoveAt(index);
				Splits.RemoveAt(index);
				Controls.Remove(panel);
				if (Splits.Count > 0) Splits[Splits.Count - 1] = FullSize;
				AdjustPanels();
				ResumeLayout();
			}
		}
		public void Clear()
		{
			Panels = new List<MultiSplitPanel>();
			Splits = new List<int>();
			Controls.Clear();
		}


		private int _dragIndex = -1;
		private bool _dragging = false;
		private int _dragOffset;
		private int _dragTarget;

		private bool HandleMouseEvent(Point cursorPosition, Func<int, bool, bool> action)
		{
			var control = OsFeatures.GlobalMouseHandler.GetCurrentControl();
			var detectedControl = control != null;
			while (control != null && control != this) control = control.Parent;
			var point = PointToClient(cursorPosition);
			return action(Horizontal ? point.X : point.Y, detectedControl ? control == this : Bounds.Contains(Parent.PointToClient(cursorPosition)));
		}
		private bool HandleMouseMovement(int location, bool inside)
		{
			if (_dragging)
			{
				ShowDragOverlay(location);
				if (LiveDrag) HandleDrag(location);
				return true;
			}
			else if (inside)
			{ 
				_dragIndex = -1;
				for (var i = 0; i < Panels.Count - 1; i++)
				{
					if (location < Splits[i] + 5 && location > Splits[i] - 5)
					{
						_dragIndex = i;
						Cursor = Cursor.Current = Horizontal ? Cursors.SizeWE : Cursors.SizeNS;
						return true;
					}
				}
				if (Cursor != DefaultCursor) Cursor = DefaultCursor;
			}

			return false;
		}

		private void ShowDragOverlay(int location)
		{
			if (_overlay == null)
			{
				_overlay = new Form
				{
					FormBorderStyle = FormBorderStyle.None,
					ShowInTaskbar = false,
					Opacity = 0.5,
					BackColor = SystemColors.HotTrack
				};
				_overlay.Show(ParentForm);
			}

			var min = _dragIndex > 0 ? Splits[_dragIndex - 1] : 0;
			var max = _dragIndex == Splits.Count - 1 ? FullSize : Splits[_dragIndex + 1];
			location = Math.Min(max, Math.Max(min, Splits[_dragIndex] + location - _dragOffset - 2));
			var overlayLocation = new Rectangle(Point.Empty, Size);
			if (Horizontal)
			{
				overlayLocation.Width = 4;
				overlayLocation.X = location;
			}
			else
			{
				overlayLocation.Height = 4;
				overlayLocation.Y = location;
			}

			_overlay.Bounds = RectangleToScreen(overlayLocation);
			_overlay.Visible = true;
		}

		private void HandleDrag(int location)
		{
			Cursor = Cursor.Current = Horizontal ? Cursors.SizeWE : Cursors.SizeNS;
			_dragTarget += location - _dragOffset;

			var min = _dragIndex > 0 ? Splits[_dragIndex - 1] : 0;
			var max = _dragIndex == Splits.Count - 1 ? FullSize : Splits[_dragIndex + 1];
			Splits[_dragIndex] = Math.Min(max, Math.Max(min, _dragTarget));
			_dragOffset = location;
			AdjustPanels();

		}
		private bool HandleMouseDown(int location, bool inside)
		{
			if (_dragIndex < 0 || !inside) return false;
			
			SuspendLayout();

			_dragging = true;
			_dragOffset = location;
			_dragTarget = Splits[_dragIndex];
			ShowDragOverlay(location);

			return true;

		}
		private bool HandleMouseUp(int location, bool inside)
		{
			if (!_dragging) return false;

			HandleDrag(location);
			_dragging = false;
			_overlay.Visible = false;
			ResumeLayout();
			return true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		public void SetSplits(List<int> newSplits)
		{
			if (Splits.Count != newSplits.Count) return;
			Splits = newSplits;
			if (Splits.Count > 0) _oldWidth = _oldHeight = Splits[Splits.Count - 1]; // "Hack" to ensure fitting stored sizes into a new layout.
			PerformLayout();
		}

		internal void AddPanel(object p)
		{
			throw new NotImplementedException();
		}
	}

	public class MultiSplitDesigner : ControlDesigner
	{
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			/*this.EnableDesignMode(((MultiSplitContainer)this.Control).FirstPanel, "FirstPanel");
			this.EnableDesignMode(((MultiSplitContainer)this.Control).SecondPanel, "SecondPanel");
			this.EnableDesignMode(((MultiSplitContainer)this.Control).ThirdPanel, "ThirdPanel");*/
		}
	}

}
