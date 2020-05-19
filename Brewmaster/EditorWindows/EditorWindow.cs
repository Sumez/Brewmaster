using System;
using System.IO;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;

namespace Brewmaster.EditorWindows
{
	public class EditorWindow : TabPage
	{
		protected MainForm MainWindow { get; set; }
		public AsmProjectFile ProjectFile { get; protected set; }
		public Events ModuleEvents { get; protected set; }
		public virtual ToolStrip ToolBar { get; }

		public EditorWindow(MainForm form, AsmProjectFile file, Events events)
		{
			MainWindow = form;
			ProjectFile = file;
			ModuleEvents = events;
			Text = ProjectFile.File.Name;
		}
	}

	public abstract class SaveableEditorWindow : EditorWindow, ISaveable
	{
		protected SaveableEditorWindow(MainForm form, AsmProjectFile file, Events events) : base(form, file, events) { }
		public abstract void Save(Func<FileInfo, string> getNewFileName = null);

		private bool _pristine;
		public bool Pristine
		{
			get { return _pristine; }
			protected set
			{
				var updated = _pristine != value;
				_pristine = value;
				RefreshTitle();
				if (updated && PristineChanged != null) PristineChanged();

				if (!_pristine) ProjectFile.Project.UpdatedSinceLastBuild = true;
			}
		}
		public event Action PristineChanged;
		protected void RefreshTitle()
		{
			Text = WindowTitle + (Pristine ? "" : " *");
		}

		protected virtual string WindowTitle
		{
			get { return ProjectFile.File.Name; }
		}
	}

}