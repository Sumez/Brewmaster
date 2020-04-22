﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.TextEditor;

namespace BrewMaster.EditorWindows.Text
{
	public partial class FindWindow : Form
    {
		private static string StoredQuery;
		private readonly Func<TextEditorWindow> _getTargetEditor;

	    public FindWindow(Func<TextEditorWindow> getTargetEditor)
	    {
		    InitializeComponent();
			_getTargetEditor = getTargetEditor;
			if (!string.IsNullOrEmpty(StoredQuery)) SearchQuery.Text = StoredQuery;
		    
		    // TODO: Allow the option "find in all files", and use that if no files are currently open
		    var currentEditor = getTargetEditor();
	    }

		private void SearchQuery_TextChanged(object sender, EventArgs e)
        {
            StatusLabel.Text = "";
            FindNextButton.Enabled = !string.IsNullOrEmpty(SearchQuery.Text);
        }

		private void FindNextButton_Click(object sender, EventArgs e)
        {
			//TODO: Move into own method + remember search string for next occurance!
	        var query = SearchQuery.Text;
	        var editor = _getTargetEditor();

	        if (string.IsNullOrEmpty(query) || editor == null) return;
	        var results = FindNext(SearchQuery.Text, editor.TextEditor);


			StatusLabel.Text = string.Format("Found {0} result{1}", results.Count, results.Count == 1 ? "" : "s");
	        StatusLabel.ForeColor = results.Any() ? SystemColors.ControlText : Color.Red;
			StoredQuery = query;
        }

	    public static void FindNext(TextEditorControl textEditor)
	    {
		    if (string.IsNullOrEmpty(StoredQuery)) return;
		    FindNext(StoredQuery, textEditor);

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
    }
}
