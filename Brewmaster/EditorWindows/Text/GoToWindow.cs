using System;
using System.Windows.Forms;
using ICSharpCode.TextEditor;

namespace BrewMaster.EditorWindows.Text
{
	public partial class GoToWindow : Form
    {
	    private int MaxLines
	    {
		    get { return Math.Max(_editor.TextEditor.Document.TotalNumberOfLines, 1); }
	    }
	    private readonly TextEditorWindow _editor;

	    public GoToWindow(Func<TextEditorWindow> getTargetEditor)
	    {
		    InitializeComponent();
		    _editor = getTargetEditor();
			GoToLabel.Text = string.Format("Go to line (1 - {0}):", MaxLines);
	    }

		private void GoToLine_TextChanged(object sender, EventArgs e)
        {
            GoToButton.Enabled = int.TryParse(GoToLineTextBox.Text, out var line) && line <= MaxLines && line > 0;
        }

		private void GoToButton_Click(object sender, EventArgs e)
		{
			if (!int.TryParse(GoToLineTextBox.Text, out var line)) return;
			_editor.TextEditor.FocusLine(line, false);
			Close();
		}
	}
}
