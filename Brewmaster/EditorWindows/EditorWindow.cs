using System;
using System.Windows.Forms;
using BrewMaster.ProjectModel;

namespace BrewMaster.EditorWindows
{
	public class EditorWindow : TabPage
	{
		protected MainForm MainWindow { get; set; }
		public AsmProjectFile ProjectFile { get; protected set; }

		public EditorWindow(MainForm form, AsmProjectFile file)
		{
			MainWindow = form;
			ProjectFile = file;
			Text = ProjectFile.File.Name;
		}
	}

	public abstract class SaveableEditorWindow : EditorWindow, ISaveable
	{
		protected SaveableEditorWindow(MainForm form, AsmProjectFile file) : base(form, file) { }
		public abstract void Save();

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