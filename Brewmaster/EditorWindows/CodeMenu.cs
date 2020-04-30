using System;
using System.Windows.Forms;
using Brewmaster.Modules;
using Brewmaster.ProjectModel;
using Brewmaster.Settings;

namespace Brewmaster.EditorWindows
{
	public class CodeMenu : TextEditorMenu
	{
		public Action GoToDefinition;
		public Action ToggleBreakpoint;
		public Action AddToWatch;
		public Action<Breakpoint.Types> BreakOnAccess;

		public CodeMenu(Events events) : base(events) { }

		protected override void InitializeComponent()
		{
			Items.Add(GoToDefinitionOption = new ToolStripMenuItem("Go To Definition", null, GoToDefinition_Click));
			Items.Add(AddToWatchOption = new ToolStripMenuItem("Add To Watch", null, AddToWatch_Click));
			Items.Add(BreakOnWriteOption = new ToolStripMenuItem("Break On Write", null, (s, e) => { if (BreakOnAccess != null) BreakOnAccess(Breakpoint.Types.Write); }));
			Items.Add(BreakOnReadOption = new ToolStripMenuItem("Break On Read", null, (s, e) => { if (BreakOnAccess != null) BreakOnAccess(Breakpoint.Types.Read); }));
			Items.Add(new ToolStripSeparator());
			Items.Add(AddBreakpointOption = new ToolStripMenuItem("Toggle Breakpoint", null, AddBreakpoint_Click));

			Program.BindKey(Feature.GoToDefinition, (keys) => GoToDefinitionOption.ShortcutKeys = keys);
			Program.BindKey(Feature.AddToWatch, (keys) => AddToWatchOption.ShortcutKeys = keys);

			Closing += (sender, args) => { AddToWatchOption.Enabled = GoToDefinitionOption.Enabled = true; };
			Items.Add(new ToolStripSeparator());

			base.InitializeComponent();
		}

		private void AddBreakpoint_Click(object sender, EventArgs e)
		{
			if (ToggleBreakpoint != null) ToggleBreakpoint();
		}

		private void AddToWatch_Click(object sender, EventArgs e)
		{
			if (AddToWatch != null) AddToWatch();
		}

		public ToolStripMenuItem GoToDefinitionOption { get; private set; }
		public ToolStripMenuItem AddToWatchOption { get; private set; }
		public ToolStripMenuItem BreakOnWriteOption { get; private set; }
		public ToolStripMenuItem BreakOnReadOption { get; private set; }
		public ToolStripMenuItem AddBreakpointOption { get; private set; }

		public AsmWord CurrentWord
		{
			set
			{
				if (value == null)
				{
					BreakOnWriteOption.Enabled = BreakOnReadOption.Enabled = AddToWatchOption.Enabled = GoToDefinitionOption.Enabled = false;
				}
				else
				{
					GoToDefinitionOption.Enabled = (value.WordType == AsmWord.AsmWordType.LabelAbsolute || value.WordType == AsmWord.AsmWordType.LabelReference);
					BreakOnWriteOption.Enabled = BreakOnReadOption.Enabled = AddToWatchOption.Enabled = (value.WordType == AsmWord.AsmWordType.LabelReference || value.WordType == AsmWord.AsmWordType.LabelDefinition || value.WordType == AsmWord.AsmWordType.NumberByte || value.WordType == AsmWord.AsmWordType.NumberWord);
				}
				AddToWatchOption.Text = AddToWatchOption.Enabled ? string.Format("Add '{0}' To Watch", value.Word)  : "Add To Watch";
			}
		}


		private void GoToDefinition_Click(object sender, EventArgs e)
		{
			if (GoToDefinition != null) GoToDefinition();
		}
	}
}
