﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrewMaster.EditorWindows.Text;
using BrewMaster.Ide;
using BrewMaster.ProjectModel;

namespace BrewMaster.EditorWindows
{
	public class TextEditorWindow : SaveableEditorWindow
	{
		private FileSystemWatcher _fileSystemWatcher;
		public bool RefreshWarningVisible { get; private set; }
		public bool SavingFile { get; private set; }
		private readonly object _savingLock;
		public TextEditor TextEditor;
		public SourceNavigator SourceNavigator;

		public Action ThreadSafeRefreshWarning { get; set; }
		public Action ThreadSafeRefresh { get; set; }
		public TextEditorWindow(MainForm form, AsmProjectFile file) : base(form, file)
		{
			switch (file.Type)
			{
				case FileType.Source:
				case FileType.Include:
					TextEditor = new Ca65Editor(ProjectFile);
					break;
				case FileType.Text:
				default:
					TextEditor = new TextEditor();
					break;
			}
			
			InitializeComponent();

			ThreadSafeRefresh = RefreshEditorContents;
			ThreadSafeRefreshWarning = RefreshWarning;
			TextEditor.TextChanged += (o, args) =>
									{
										Pristine = false;
										RefreshTitle();
									};

			ProjectFile.ParsedLocalSymbols += ThreadSafeUpdateSymbols;

			var codeEditor = TextEditor as CodeEditor;
			if (codeEditor != null)
			{
				SourceNavigator.Visible = true;
				if (ProjectFile.LocalSymbols != null) SourceNavigator.UpdateSymbols(ProjectFile.LocalSymbols);

				codeEditor.ParseErrors = (code) => form.BuildHandler.ParseErrors(code, ProjectFile);
				codeEditor.AddToWatch = form.AddWatch;
				codeEditor.AddAddressBreakpoint = (address, types) => { form.AddBreakpoint(address, types, Breakpoint.AddressTypes.Cpu); };
				codeEditor.AddSymbolBreakpoint = (symbol, types) => { form.AddBreakpoint(-1, types, Breakpoint.AddressTypes.Cpu, symbol); };
				codeEditor.GetCpuMemory = form.GetCpuMemory;
				codeEditor.ActiveTextAreaControl.Caret.PositionChanged += (sender, args) =>
				{
					SourceNavigator.CurrentLine = codeEditor.ActiveTextAreaControl.Caret.Line;
				};
				SourceNavigator.Navigated += line =>
				{
					codeEditor.FocusLine(line, false);
					codeEditor.Focus();
				};
			}
			_savingLock = new object();
		}

		private void ThreadSafeUpdateSymbols()
		{
			BeginInvoke(new Action(() =>
			{
				// TODO: Instead of updating symbols directly from file parsing, use the CodeEditor to add them as line markers
				SourceNavigator.UpdateSymbols(ProjectFile.LocalSymbols.Where(s => s.Source == ProjectFile.File.FullName));
			}));
		}

		public void RefreshWarning()
		{
			var refresh = (MessageBox.Show(MainWindow, ProjectFile.File.Name + " was changed by another process. Reload now?", "File changed", MessageBoxButtons.YesNo) == DialogResult.Yes);
			if (refresh) RefreshEditorContents();
			else Pristine = false;
			RefreshWarningVisible = false;
		}

		// Update editor contents to file contents. Only done when opening the tab or file was changed from outside
		public void RefreshEditorContents()
		{

			//first read text of a file
			var fileText = File.ReadAllText(ProjectFile.File.FullName);

			if (Regex.IsMatch(fileText, @"\r\r\n") && Prompt.ShowDialog(string.Format("This file ({0}) has an issue with line endings.\nFix endings by removing double carriage returns?", ProjectFile.File.Name), "Open File"))
			{
				TextEditor.Text = Regex.Replace(fileText, @"\r\r\n", "\r\n", RegexOptions.Singleline); // Fix Nesicide oddity
				Pristine = false;
			}
			else
			{
				TextEditor.Text = fileText;
				Pristine = true;
			}

			RefreshEditorBreakpoints();

			string textType;
			switch (ProjectFile.File.Extension)
			{
				case "s":
					textType = "Java";
					break;
				default:
					textType = "Text";
					break;
			}
			//AddLanguages(textType);
			Invalidate();

			if (_fileSystemWatcher == null)
			{
				_fileSystemWatcher = new FileSystemWatcher(ProjectFile.File.DirectoryName, ProjectFile.File.Name);
				_fileSystemWatcher.Changed += (sender, args) =>
											{
												lock (_savingLock)
												{
													if (RefreshWarningVisible || SavingFile) return;
												}
												RefreshWarningVisible = true;
												Task.Run(() => { Invoke(ThreadSafeRefreshWarning); });
											};
				_fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		public void RefreshEditorBreakpoints()
		{
			var codeEditor = TextEditor as CodeEditor;
			if (codeEditor != null)
			{
				codeEditor.UpdateBreakpointsInEditor();
				if (ProjectFile.DebugLines != null) codeEditor.UpdateBreakpointsWithBuildInfo();
			}
		}


		protected override void Dispose(bool disposing)
		{
			ProjectFile.ParsedLocalSymbols -= ThreadSafeUpdateSymbols;
			if (_fileSystemWatcher != null) _fileSystemWatcher.Dispose();
			base.Dispose(disposing);
		}

		public override void Save()
		{
			if (Pristine) return;
			lock (_savingLock)
			{
				SavingFile = true;
			}

			var filename = ProjectFile.File.FullName;
			if (!File.Exists(filename)) throw new Exception(string.Format("File {0} does not exist", ProjectFile.File.Name));

			if (_fileSystemWatcher != null) _fileSystemWatcher.EnableRaisingEvents = false;
			using (RichTextBox rtb = new RichTextBox())
			{
				rtb.Text = TextEditor.Text;
				File.WriteAllText(filename, "");
				using (var strwriter = File.AppendText(filename))
				{
					strwriter.Write(rtb.Text);
					strwriter.Close();
				}
			}
			if (_fileSystemWatcher != null) _fileSystemWatcher.EnableRaisingEvents = true;

			lock (_savingLock)
			{
				SavingFile = false;
			}
			Pristine = true;

			ProjectFile.Project.LoadAllSymbolsAsync();
		}

		private void InitializeComponent()
		{
			this.SourceNavigator = new SourceNavigator();
			this.SuspendLayout();
			// 
			// SourceNavigator
			// 
			this.SourceNavigator.Location = new System.Drawing.Point(-91, 0);
			this.SourceNavigator.Name = "SourceNavigator";
			this.SourceNavigator.Size = new System.Drawing.Size(275, 30);
			this.SourceNavigator.Anchor = AnchorStyles.Right | AnchorStyles.Top;
			//this.SourceNavigator.Dock = DockStyle.Top;
			this.SourceNavigator.Visible = false;
			this.SourceNavigator.TabIndex = 0;
			// 
			// TextEditor
			// 
			this.TextEditor.IsReadOnly = false;
			this.TextEditor.Location = new System.Drawing.Point(0, 0);
			this.TextEditor.Name = "TextEditor";
			this.TextEditor.Dock = DockStyle.Fill;
			this.TextEditor.Size = new System.Drawing.Size(100, 100);
			this.TextEditor.TabIndex = 1;
			// 
			// TextEditorWindow
			// 
			this.Controls.Add(this.SourceNavigator);
			this.Controls.Add(this.TextEditor);
			this.ResumeLayout(false);
		}
	}

	public interface ISaveable
	{
		void Save();
		bool Pristine { get; }
		event Action PristineChanged;
	}
}
