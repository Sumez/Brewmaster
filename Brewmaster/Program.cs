using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Brewmaster.ProjectExplorer;
using Brewmaster.Settings;

namespace Brewmaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			string file = null;
			if (args != null && args.Length == 1)
			{
				file = args[0];
				if (!File.Exists(file)) file = null;
			}

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#if !DEBUG
			Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			WorkingDirectory = Application.StartupPath;
			EmulatorDirectory = GetUserFilePath("Mesen");
			var emulatorResources = Path.Combine(EmulatorDirectory, "Resources");
			if (!Directory.Exists(EmulatorDirectory)) Directory.CreateDirectory(EmulatorDirectory);
			if (!Directory.Exists(emulatorResources))
			{
				Directory.CreateDirectory(emulatorResources);
				var dir = new DirectoryInfo(Path.Combine(WorkingDirectory, @"lib\Mesen"));
				foreach (var resource in dir.GetFiles()) resource.CopyTo(Path.Combine(emulatorResources, resource.Name));
			}
			Directory.SetCurrentDirectory(EmulatorDirectory);

			try
			{
#if WINDOWS
				var bwmAssociation = OsFeatures.RecommendedPrograms(".bwm");
				if (bwmAssociation.Count == 0) OsFeatures.SetRecommendedProgram(".bwm", "brewmasterproject", Application.ExecutablePath);
#endif
			}
			catch (Exception ex)
			{
				Error("Failed trying to associate the .BWM file extension", ex);
			}
			new MainForm { RequestFile = file };
			Application.Run(CurrentWindow);
        }

	    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	    {
		    throw new Exception(e.ExceptionObject.ToString());
	    }

	    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
	    {
			Error(e.Exception.Message, e.Exception);
	    }

		public static string WorkingDirectory { get; set; }
        public static string EmulatorDirectory { get; set; }
		public static MainForm CurrentWindow { get; set; }

		public static KeyBindings Keys
		{
			get
			{
				return CurrentWindow?.Settings?.KeyBindings ?? new KeyBindings();
			}
		}

		public static void Error(string message, Exception ex = null)
		{
			if (ex != null)
			try {
				File.AppendAllLines(GetUserFilePath("errors.log"), new [] { string.Format("{0}: {1}", DateTime.Now, ex) });
			}
			catch (Exception loggingError) { }
			
			MessageBox.Show(CurrentWindow, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

	    public static void BindKey(Feature feature, ToolStripMenuItem menuItem)
	    {
		    BindKey(feature, keys => menuItem.ShortcutKeys = keys);
	    }
		public static void BindKey(Feature feature, Action<Keys> callback)
	    {
		    BindKey(feature, (a) => callback(a.Keys));
	    }
		public static void BindKey(Feature feature, Action<KeyBindingEventArgs> callback)
	    {
		    if (!BindingCallbacks.ContainsKey(feature)) BindingCallbacks.Add(feature, new List<Action<KeyBindingEventArgs>>());
		    BindingCallbacks[feature].Add(callback);

		    callback(new KeyBindingEventArgs(Keys[feature], feature));
		}

	    public static void UnbindKey(Feature feature, Action<KeyBindingEventArgs> callback)
	    {
		    if (!BindingCallbacks.ContainsKey(feature) || !BindingCallbacks[feature].Contains(callback)) return;
		    BindingCallbacks[feature].Remove(callback);
	    }

		public static void UpdateKeyBindings(KeyBindings newBindings)
	    {
		    Keys.Clear();
		    foreach (var binding in newBindings)
		    {
			    Keys.Add(binding);
			    var feature = (Feature)binding.Feature;
			    if (!BindingCallbacks.ContainsKey(feature)) continue;

			    try
			    {
				    foreach (var callback in BindingCallbacks[feature]) callback(new KeyBindingEventArgs(Keys[feature], feature));
			    }
				catch (Exception ex)
			    {
				    Error("Unexpeted error remapping key bindings.\nYou may need to restart the program for new mappings to take effect", ex);
			    }
		    }
	    }
		private static readonly Dictionary<Feature, List<Action<KeyBindingEventArgs>>> BindingCallbacks = new Dictionary<Feature, List<Action<KeyBindingEventArgs>>>();

	    public static string GetUserFilePath(string fileName)
	    {
		    return Path.Combine(
			    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			    "Brewmaster", fileName);
	    }
    }
	public class KeyBindingEventArgs
	{
		public KeyBindingEventArgs(Keys keys, Feature feature)
		{
			Keys = keys;
			Feature = feature;
		}
		public Keys Keys { get; private set; }
		public Feature Feature { get; private set; }
	}

}
