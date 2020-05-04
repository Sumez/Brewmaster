using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Brewmaster.EditorWindows
{
	public class CompletionWindow : AbstractCompletionWindow
	{
		const int MaxListLength = 10;
		//protected virtual string Filename { get; set; }
		protected CompletionWindow(Form parentForm, TextEditorControl control) : base(parentForm, control)
		{
			DoubleBuffered = true;
			_scrollBar = new VScrollBar
			{
				Minimum = 0,
				Dock = DockStyle.Right,
				SmallChange = 1,
				LargeChange = MaxListLength,
				//Width = 8, // TODO: Create custom scroll bar with overriding OnPaint method

			};
			_scrollBar.ValueChanged += VScrollBarValueChanged;
			Controls.Add(_scrollBar);
			MouseWheel += (s, e) => HandleMouseWheel(e);
			DeclarationViewWindow = new DeclarationViewWindow(parentForm);
			SetDeclarationViewLocation();
			DeclarationViewWindow.ShowDeclarationViewWindow();
			DeclarationViewWindow.MouseMove += ControlMouseMove;

			control.Document.DocumentAboutToBeChanged += DocumentAboutToBeChanged;
		}

		protected DeclarationViewWindow DeclarationViewWindow = null;
		private VScrollBar _scrollBar;
		public static ICompletionDataProvider CompletionDataProvider { get; private set; }
		public bool CloseWhenCaretAtBeginning { get; set; }
		public CodeCompletionListView ListView { get; private set; }

		public static CompletionWindow ShowCompletionWindow(Form parent, TextEditorControl control, string fileName,
			ICompletionDataProvider completionDataProvider, char firstChar)
		{
			CompletionDataProvider = completionDataProvider;
			var completionData = CompletionDataProvider.GenerateCompletionData(fileName, control.ActiveTextAreaControl.TextArea, firstChar);
			if (completionData.Length == 0) return null;

			var window = new CompletionWindow(parent, control);
			window.RefreshList(completionData);
			window.SetLocation();
			window.ShowCompletionWindow();
			return window;
		}

		public void RefreshCompletionData(string fileName, char firstChar)
		{
			var completionData = CompletionDataProvider.GenerateCompletionData(fileName, control.ActiveTextAreaControl.TextArea, firstChar);
			if (completionData.Length == 0)
			{
				Close();
				return;
			};
			RefreshList(completionData);
		}
		
		protected int StartOffset { get; set; }
		protected int EndOffset { get; set; }

		bool _inScrollUpdate;
		void VScrollBarValueChanged(object sender, EventArgs e)
		{
			if (_inScrollUpdate) return;
			_inScrollUpdate = true;
			ListView.FirstItem = _scrollBar.Value;
			ListView.Refresh();
			SetDeclarationViewLocation();
			control.ActiveTextAreaControl.TextArea.Focus();
			_inScrollUpdate = false;
		}

		protected ICompletionData[] _currentCompletionData;
		protected void RefreshList(ICompletionData[] completionData)
		{
			StartOffset = control.ActiveTextAreaControl.Caret.Offset + 1;
			EndOffset = StartOffset;
			if (CompletionDataProvider.PreSelection != null)
			{
				StartOffset -= CompletionDataProvider.PreSelection.Length + 1;
				EndOffset--;
			}

			SuspendLayout();
			
			string previouslySelected = null;
			if (ListView != null)
			{
				if (ListView.SelectedCompletionData != null) previouslySelected = ListView.SelectedCompletionData.Text;
				Controls.Remove(ListView);
				ListView.Dispose();
			}

			_currentCompletionData = completionData;
			ListView = new CodeCompletionListView(completionData);
			ListView.ImageList = CompletionDataProvider.ImageList;
			ListView.Dock = DockStyle.Fill;
			ListView.SelectedItemChanged += (s, e) => UpdateDeclarationView();
			//ListView.DoubleClick += (s, e) => InsertSelectedItem('\0');
			//ListView.MouseDown += (s, e) => control.ActiveTextAreaControl.TextArea.Focus(); // Makes click impossible
			//ListView.GotFocus += (s, e) => control.ActiveTextAreaControl.TextArea.Focus(); // Interfers with scrollbar
			ListView.MouseUp += (s, e) => InsertSelectedItem('\0');
			Controls.Add(ListView);

			if (completionData.Length > MaxListLength)
			{
				_scrollBar.Maximum = completionData.Length - 1;
				_scrollBar.Value = 0;
				ListView.FirstItemChanged += (s, e) =>
				{
					if (_inScrollUpdate) return;
					_inScrollUpdate = true;
					_scrollBar.Value = Math.Min(_scrollBar.Maximum, ListView.FirstItem);
					_inScrollUpdate = false;
				};
				_scrollBar.Visible = true;
			}
			else
			{
				_scrollBar.Visible = false;
			}
			drawingSize = GetListViewSize(completionData.Length);
			Width = drawingSize.Width;
			Height = drawingSize.Height;
			ResumeLayout();

			//control.Focus();
			UpdateDeclarationView();
			var selectIndex = previouslySelected == null ? -1 : Array.FindIndex(completionData, d => d.Text == previouslySelected);
			if (selectIndex >= 0)
			{
				ListView.SelectIndex(selectIndex);
			}
			else if (CompletionDataProvider.DefaultIndex >= 0)
			{
				ListView.SelectIndex(CompletionDataProvider.DefaultIndex);
			}

			if (CompletionDataProvider.PreSelection != null)
			{
				// Need to sort the array after passing it to listview, as its contructor forces a different sorting
				Array.Sort(completionData, (a, b) =>
				{
					var ax = a.Text.StartsWith(CompletionDataProvider.PreSelection, StringComparison.InvariantCultureIgnoreCase);
					var bx = b.Text.StartsWith(CompletionDataProvider.PreSelection, StringComparison.InvariantCultureIgnoreCase);
					return ax == bx ? 0 : (ax ? -1 : 1);
				});
				CaretOffsetChanged(this, EventArgs.Empty);
			}
		}
		void UpdateDeclarationView()
		{
			var data = ListView.SelectedCompletionData;
			if (data != null && data.Description != null && data.Description.Length > 0)
			{
				DeclarationViewWindow.Description = data.Description;
				SetDeclarationViewLocation();
			}
			else
			{
				DeclarationViewWindow.Description = null;
			}

			if (data is IHasFocusAction focusData) focusData.Focus();
		}
		protected void DocumentAboutToBeChanged(object sender, DocumentEventArgs e)
		{
			// => startOffset test required so that this startOffset/endOffset are not incremented again
			//    for BeforeStartKey characters
			if (e.Offset >= StartOffset && e.Offset <= EndOffset)
			{
				if (e.Length > 0)
				{ // length of removed region
					EndOffset -= e.Length;
				}
				if (!string.IsNullOrEmpty(e.Text))
				{
					EndOffset += e.Text.Length;
				}
			}
		}
		protected override void CaretOffsetChanged(object sender, EventArgs e)
		{
			int offset = control.ActiveTextAreaControl.Caret.Offset;
			if (offset == StartOffset)
			{
				if (CloseWhenCaretAtBeginning)
					Close();
				return;
			}
			if (offset < StartOffset || offset > EndOffset)
			{
				Close();
			}
			else
			{
				// Uncomment below if you want to automatically select a list item matching the current word
				//ListView.SelectItemWithStart(control.Document.GetText(StartOffset, offset - StartOffset));
			}
		}

		protected void SetDeclarationViewLocation()
		{
			var workingScreen = Screen.GetWorkingArea(Location);
			
			//  This method uses the side with more free space
			var leftSpace = Bounds.Left - workingScreen.Left;
			var rightSpace = workingScreen.Right - Bounds.Right;
			var pos = new Point(Bounds.Right + 5, Bounds.Top);

			if (ListView != null && ListView.SelectedCompletionData != null) { 
				pos.Y += (Array.IndexOf(_currentCompletionData, ListView.SelectedCompletionData) - ListView.FirstItem) * ListView.ItemHeight;
			}

			// The declaration view window has better line break when used on
			// the right side, so prefer the right side to the left.
			if (rightSpace * 2 > leftSpace)
			{
				DeclarationViewWindow.FixedWidth = false;
			}
			else
			{
				DeclarationViewWindow.FixedWidth = true;
				DeclarationViewWindow.Width = DeclarationViewWindow.GetRequiredLeftHandSideWidth(new Point(Bounds.Left, Bounds.Top));

				if (Bounds.Left < DeclarationViewWindow.Width)
					pos.X = 0;
				else
					pos.X = Bounds.Left - DeclarationViewWindow.Width - 5;

			}
			
			if (DeclarationViewWindow.Location == pos) return;
			DeclarationViewWindow.Location = pos;
			DeclarationViewWindow.Refresh();
		}

		protected override bool ProcessTextAreaKey(Keys keyData)
		{
			if (!Visible)
			{
				return false;
			}

			switch (keyData)
			{
				case Keys.PageDown:
					ListView.PageDown();
					return true;
				case Keys.PageUp:
					ListView.PageUp();
					return true;
				case Keys.Down:
					ListView.SelectNextItem();
					return true;
				case Keys.Up:
					ListView.SelectPrevItem();
					return true;
				case Keys.Tab:
					return InsertSelectedItem('\t');
				case Keys.Return:
					return InsertSelectedItem('\n');
			}
			return base.ProcessTextAreaKey(keyData);
		}
		protected bool InsertSelectedItem(char ch)
		{
			control.Document.DocumentAboutToBeChanged -= DocumentAboutToBeChanged;
			var data = ListView.SelectedCompletionData;
			var result = false;
			if (data != null)
			{
				control.BeginUpdate();

				try
				{
					if (EndOffset - StartOffset > 0)
					{
						control.Document.Remove(StartOffset, EndOffset - StartOffset);
					}
					result = CompletionDataProvider.InsertAction(data, control.ActiveTextAreaControl.TextArea, StartOffset, ch);
				}
				finally
				{
					control.EndUpdate();
				}
			}
			if (result) Close();
			return true;
		}

		private Size GetListViewSize(int itemCount)
		{
			var height = ListView.ItemHeight * Math.Min(MaxListLength, itemCount);
			var width = ListView.ItemHeight * 10;
			/*if (!fixedListViewWidth)
			{
				width = GetListViewWidth(width, height);
			}*/
			return new Size(width, height);
		}

		//public void HandleMouseWheel(MouseEventArgs e);
		//public override bool ProcessKeyEvent(char ch);
		//protected override void CaretOffsetChanged(object sender, EventArgs e);
		//protected override void Dispose(bool disposing);
		//protected override bool ProcessTextAreaKey(Keys keyData);
		//protected override void SetLocation();
		const int WHEEL_DELTA = 120;
		private int mouseWheelDelta;
		public void HandleMouseWheel(MouseEventArgs e)
		{
			mouseWheelDelta += e.Delta;

			var linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);

			var scrollDistance = mouseWheelDelta * linesPerClick / WHEEL_DELTA;
			mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);

			if (scrollDistance == 0) return;

			if (control.TextEditorProperties.MouseWheelScrollDown) scrollDistance = -scrollDistance;

			var newValue = _scrollBar.Value + _scrollBar.SmallChange * scrollDistance;
			_scrollBar.Value = Math.Max(_scrollBar.Minimum, Math.Min(_scrollBar.Maximum - _scrollBar.LargeChange + 1, newValue));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				control.Document.DocumentAboutToBeChanged -= DocumentAboutToBeChanged;
				if (ListView != null)
				{
					ListView.Dispose();
					ListView = null;
				}
				if (DeclarationViewWindow != null)
				{
					DeclarationViewWindow.Dispose();
					DeclarationViewWindow = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}