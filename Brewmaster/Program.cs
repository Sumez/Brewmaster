using System;
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
			EmulatorDirectory = Path.Combine(WorkingDirectory, @"Mesen");
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
			throw new Exception(e.Exception.Message);
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

	    public static void BindKey(Feature feature, Action<Keys> callback)
	    {
		    BindingsChanged += () => callback(Keys[feature]);
		    callback(Keys[feature]);
		}

	    public static void UpdateKeyBindings(KeyBindings newBindings)
	    {
		    Keys.Clear();
		    foreach (var binding in newBindings) Keys.Add(binding);

		    try
		    {
			    if (BindingsChanged != null) BindingsChanged();
		    }
		    catch (Exception ex)
		    {
			    Error(
				    "Unexpeted error remapping key bindings.\nYou may need to restart the program for new mappings to take effect",
				    ex);
		    }
	    }
	    private static event Action BindingsChanged;

	    public static string GetUserFilePath(string fileName)
	    {
		    return Path.Combine(
			    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			    "Brewmaster", fileName);
	    }
    }
}
