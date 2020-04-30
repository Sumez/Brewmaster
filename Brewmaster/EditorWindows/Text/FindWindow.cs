using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.Modules;
using ICSharpCode.TextEditor;

namespace Brewmaster.EditorWindows.Text
{
	public partial class FindWindow : Form
    {
		private static string _storedQuery;
	    private FindMode _mode;
	    private FindResults _resultsWindow;
	    private Events _events;

	    public FindMode Mode
	    {
		    get { return _mode; }
		    set
		    {
			    _mode = value;
			    StatusLabel.Visible = AllFiles.Visible = _mode != FindMode.Replace;
			    ReplaceButton.Visible = ReplaceAllButton.Visible = ReplaceLabel.Visible = ReplaceWith.Visible = _mode == FindMode.Replace;
			    AllFiles.Checked = _mode == FindMode.FindInAllFiles;
			    FindNextButton.Text = _mode == FindMode.FindInAllFiles ? "Find All" : "Find Next";
		    }
	    }

		private TextEditorWindow GetTargetEditor() { return _events.GetCurrentTextEditor(); }

		public FindWindow(Events events, FindMode mode)
	    {
		    InitializeComponent();
			_events = events;
			if (!string.IsNullOrEmpty(_storedQuery)) SearchQuery.Text = _storedQuery;

		    if (GetTargetEditor() == null && mode == FindMode.FindInCurrentFile) mode = FindMode.FindInAllFiles;
		    Mode = mode;
	    }

		private void SearchQuery_TextChanged(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            ReplaceButton.Enabled = ReplaceAllButton.Enabled = FindNextButton.Enabled = !string.IsNullOrEmpty(SearchQuery.Text);
        }

		private void FindNextButton_Click(object sender, EventArgs e)
		{
			if (Mode == FindMode.FindInAllFiles) FindInFiles();
			else FindNextOccurence();
		}

	    private void FindInFiles()
	    {
		    var query = SearchQuery.Text;
		    if (_resultsWindow == null)
		    {
				_resultsWindow = new FindResults();
				_resultsWindow.Show(this);
		    }

		    var project = _events.GetCurrentProject();

			_storedQuery = query;
		}

	    private void FindNextOccurence()
	    {
			var query = SearchQuery.Text;
		    var editor = GetTargetEditor();

		    if (string.IsNullOrEmpty(query) || editor == null) return;
		    var results = FindNext(SearchQuery.Text, editor.TextEditor);


		    StatusLabel.Text = string.Format("Found {0} result{1}", results.Count, results.Count == 1 ? "" : "s");
		    StatusLabel.ForeColor = results.Any() ? SystemColors.ControlText : Color.Red;
		    _storedQuery = query;
		}

		public static void FindNext(TextEditorControl textEditor)
	    {
		    if (string.IsNullOrEmpty(_storedQuery)) return;
		    FindNext(_storedQuery, textEditor);
	    }
		private static List<int> FindNext(string query, TextEditorControl textEditor)
	    {
		    var textArea = textEditor.ActiveTextAreaControl;
		    var results = GetOccurrences(query, textEditor);
		    if (results.Any())
		    {
			    var caret = textArea.Caret;
			    var currentOffset = caret.Offset;
			    var selection = textArea.SelectionManager.SelectionCollection.FirstOrDefault();
			    if (selection != null) currentOffset = selection.Offset;

			    var targetOffset = results.Any(r => r > currentOffset)
				    ? results.First(r => r > currentOffset)
				    : results[0];

			    var selectFrom = textEditor.Document.OffsetToPosition(targetOffset);
			    var selectTo = textEditor.Document.OffsetToPosition(targetOffset + query.Length);

			    textArea.SelectionManager.SetSelection(selectFrom, selectTo);
			    textArea.Caret.Position = selectTo;
			    //editor.TextEditor.ActiveTextAreaControl.ScrollToCaret();
		    }

		    return results;
	    }
	    private static List<int> GetOccurrences(string query, TextEditorControl editor)
	    {
		    var results = new List<int>();
		    var index = -1;
		    do
		    {
			    // Hallelujah! After 20+ years of coding, finally a use for do {} while()
			    index = editor.Document.TextContent.IndexOf(query, index + 1, StringComparison.CurrentCultureIgnoreCase);
			    if (index >= 0) results.Add(index);

		    } while (index >= 0);

		    return results;
	    }

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	    {
		    if (keyData == Keys.Escape)
		    {
			    Close();
			    return true;
		    }
		    return base.ProcessCmdKey(ref msg, keyData);
	    }

		private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

		private void AllFiles_CheckedChanged(object sender, EventArgs e)
		{
			if (Mode == FindMode.Replace) return;
			Mode = AllFiles.Checked ? FindMode.FindInAllFiles : FindMode.FindInCurrentFile;
		}

		private void ReplaceButton_Click(object sender, EventArgs e)
		{
			var query = SearchQuery.Text;
			var editor = GetTargetEditor();
			var replaced = false;

			if (string.IsNullOrEmpty(query) || editor == null) return;
			if (editor.TextEditor.ActiveTextAreaControl.SelectionManager.SelectedText == query)
			{
				editor.TextEditor.ActiveTextAreaControl.TextArea.InsertString(ReplaceWith.Text);
				replaced = true;
			}

			FindNextOccurence();
			if (!replaced && StatusLabel.ForeColor == Color.Red) StatusAlert("Replace text");
		}

	    private void ReplaceAllButton_Click(object sender, EventArgs e)
	    {
		    var query = SearchQuery.Text;
		    var editor = GetTargetEditor();
		    if (string.IsNullOrEmpty(query) || editor == null) return;

		    var textArea = editor.TextEditor.ActiveTextAreaControl;
		    var results = GetOccurrences(query, editor.TextEditor);
		    foreach (var offset in results)
		    {
			    var selectFrom = editor.TextEditor.Document.OffsetToPosition(offset);
			    var selectTo = editor.TextEditor.Document.OffsetToPosition(offset + query.Length);
			    textArea.SelectionManager.SetSelection(selectFrom, selectTo);
			    textArea.TextArea.InsertString(ReplaceWith.Text);
			}
			StatusLabel.Text = string.Format("Replaced {0} occurence{1}", results.Count, results.Count == 1 ? "" : "s");
		    StatusLabel.ForeColor = results.Any() ? SystemColors.ControlText : Color.Red;
			StatusAlert("Replace text");
		}
		
		private void StatusAlert(string caption)
	    {
		    if (string.IsNullOrWhiteSpace(StatusLabel.Text)) return;
		    MessageBox.Show(StatusLabel.Text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
	    }
	}

	public enum FindMode
	{
		FindInCurrentFile, FindInAllFiles, Replace
	}
}
