using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Brewmaster.EditorWindows.Code;
using Brewmaster.Settings;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace Brewmaster.EditorWindows.Text
{
	public class TextEditor : TextEditorControl
	{
		public TextEditor()
		{
			Text = "";
			TextEditorProperties = DefaultCodeProperties;

			FeatureActions.Add(Feature.Undo, editactions[Keys.Control | Keys.Z]);
			FeatureActions.Add(Feature.Redo, editactions[Keys.Control | Keys.Y]);
			FeatureActions.Add(Feature.Cut, editactions[Keys.Control | Keys.X]);
			FeatureActions.Add(Feature.Copy, editactions[Keys.Control | Keys.C]);
			FeatureActions.Add(Feature.Paste, editactions[Keys.Control | Keys.V]);
			FeatureActions.Add(Feature.SelectAll, editactions[Keys.Control | Keys.A]);

			foreach (var feature in FeatureActions)
			{
				Program.BindKey(feature.Key, UpdateKeyBinding);
			}
		}

		protected void UpdateKeyBinding(KeyBindingEventArgs args)
		{
			var action = FeatureActions[args.Feature];
			var existingAction = editactions.FirstOrDefault(kvp => kvp.Value == action);
			if (existingAction.Value == action) editactions.Remove(existingAction.Key);
			if (!editactions.ContainsKey(args.Keys)) editactions.Add(args.Keys, action);
		}
		
		protected override void Dispose(bool disposing)
		{
			foreach (var feature in FeatureActions)
			{
				Program.UnbindKey(feature.Key, UpdateKeyBinding);
			}
			base.Dispose(disposing);
		}

		protected Dictionary<Feature, IEditAction> FeatureActions = new Dictionary<Feature, IEditAction>();

		public static ITextEditorProperties DefaultCodeProperties = new DefaultBrewmasterCodeProperties();

		public void GoToWordAt(int line, int column, int? length)
		{
			var lineSegment = Document.GetLineSegment(line - 1);
			if (!length.HasValue)
			{
				var word = lineSegment.GetWord(column);
				if (word == null) throw new Exception("Word not fount at Line " + line + ", column " + column);
				length = word.Length;
			}
			if (length > 0) ActiveTextAreaControl.SelectionManager.SetSelection(new TextLocation(column, line - 1), new TextLocation(column + length.Value, line - 1));
			ActiveTextAreaControl.Caret.Position = new TextLocation(column, line - 1);
		}

		public void FocusLine(int line, bool isBuildLine)
		{
			line -= 1;
			if (isBuildLine)
			{
				var buildLine = Document.BookmarkManager.Marks.OfType<BuildLineMarker>().FirstOrDefault(m => m.BuildLine == line);
				if (buildLine == null) return;
				line = buildLine.LineNumber;
			}
			ActiveTextAreaControl.TextArea.ScrollTo(line);
			ActiveTextAreaControl.Caret.Position = new TextLocation(0, line);
		}

		public virtual void Cut(object sender, EventArgs eventArgs)
		{
			ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(sender, eventArgs);
		}
		public virtual void Paste(object sender, EventArgs eventArgs)
		{
			ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(sender, eventArgs);
		}
		public virtual void Delete(object sender, EventArgs eventArgs)
		{
			ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(sender, eventArgs);
		}
		public virtual void Copy(object sender, EventArgs eventArgs)
		{
			ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(sender, eventArgs);
		}
		public virtual void SelectAll(object sender, EventArgs eventArgs)
		{
			ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(sender, eventArgs);
		}
	}

	public class DefaultBrewmasterCodeProperties : DefaultTextEditorProperties
	{
		public DefaultBrewmasterCodeProperties()
		{
			IsIconBarVisible = true;
			Font = new Font("Courier New", 10, GraphicsUnit.Point);
			VerticalRulerRow = 0;
			EnableFolding = false;
			ShowLineNumbers = true;
			CutCopyWholeLine = false;
		}
	}
}