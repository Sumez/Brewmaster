using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BrewMaster.ProjectExplorer
{
	public static class OsFeatures
	{
		public class FileTypeInfo
		{
			public string Name { get; set; }
			public string Open { get; set; }
			public string Icon { get; set; }
		}
		public static List<FileTypeInfo> RecommendedPrograms(string ext)
		{
			var progs = new List<FileTypeInfo>();

			string baseKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + ext;
			string commandKey = @"Software\Classes\";

			using (var rk = Registry.CurrentUser.OpenSubKey(baseKey + @"\OpenWithProgids"))
			{
				if (rk == null) return progs;
				foreach (var item in rk.GetValueNames())
				{
					string command = null, icon = null;

					using (var rkCommand = Registry.CurrentUser.OpenSubKey(commandKey + item + @"\shell\open\command"))
					using (var rkIcon = Registry.CurrentUser.OpenSubKey(commandKey + item + @"\DefaultIcon"))
					{
						if (rkCommand != null) command = (string)rkCommand.GetValue("");
						if (rkIcon != null) icon = (string)rkIcon.GetValue("");

						if (rkCommand == null)
						{
							using (var rkCommand2 = Registry.LocalMachine.OpenSubKey(commandKey + item + @"\shell\open\command"))
							using (var rkIcon2 = Registry.LocalMachine.OpenSubKey(commandKey + item + @"\DefaultIcon"))
							{
								if (rkCommand2 != null) command = (string)rkCommand2.GetValue("");
								if (rkIcon2 != null) icon = (string)rkIcon2.GetValue("");
							}
						}
					}

					if (command == null) continue;
					progs.Add(new FileTypeInfo { Name = item, Open = command, Icon = icon });
				}
			}

			return progs;
		}

		public static void SetRecommendedProgram(string ext, string item, string program)
		{
			string baseKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + ext;
			string commandKey = @"Software\Classes\";

			using (var rk = Registry.CurrentUser.CreateSubKey(baseKey + @"\OpenWithProgids"))
				rk.SetValue(item, new byte[0], RegistryValueKind.None);

			using (var rkCommand = Registry.CurrentUser.CreateSubKey(commandKey + item + @"\shell\open\command"))
				rkCommand.SetValue("", string.Format("\"{0}\" \"%1\"", program));

			//using (var rkIcon = Registry.CurrentUser.CreateSubKey(commandKey + item + @"\DefaultIcon"))
		}

		public static void AddFileToRecent(string path)
		{
#if WINDOWS
			SHAddToRecentDocs(ShellAddToRecentDocsFlags.Path, path);
#endif
		}

		private enum ShellAddToRecentDocsFlags
		{
			Pidl = 0x001,
			Path = 0x002,
		}
#if WINDOWS
		[DllImport("shell32.dll", CharSet = CharSet.Ansi)]
		private static extern void SHAddToRecentDocs(ShellAddToRecentDocsFlags flag, string path);
#endif
		public static void OpenFolder(string path)
		{
			Process.Start(path);
		}


		public delegate bool MouseEvent(Point location);
		
		public class GlobalMouseHandler : IMessageFilter
		{
			private const int WM_MOUSEMOVE = 0x0200;
			private const int WM_LBUTTONDOWN = 0x201;
			private const int WM_LBUTTONUP = 0x202;

			public event MouseEvent MouseMoved;
			public event MouseEvent MouseDown;
			public event MouseEvent MouseUp;

			public bool PreFilterMessage(ref Message m)
			{
				if (m.Msg == WM_MOUSEMOVE && MouseMoved != null) return MouseMoved(Cursor.Position);
				if (m.Msg == WM_LBUTTONDOWN && MouseDown != null) return MouseDown(Cursor.Position);
				if (m.Msg == WM_LBUTTONUP && MouseUp != null) return MouseUp(Cursor.Position);

				return false;
			}

#if WINDOWS
			[System.Runtime.InteropServices.DllImport("user32.dll")]
			private static extern IntPtr WindowFromPoint(Point pnt);
#endif

			public static Control GetCurrentControl()
			{
#if WINDOWS
				var hWnd = WindowFromPoint(Control.MousePosition);
				if (hWnd != IntPtr.Zero)
				{
					return Control.FromHandle(hWnd);
				}
#endif
				return null;
			}
		}

		public static bool IsInDirectory(DirectoryInfo testChildDirectory, string parentPath)
		{
			while (testChildDirectory != null)
			{
				if (testChildDirectory.FullName == parentPath) return true;
				testChildDirectory = testChildDirectory.Parent;
			}
			return false;
		}

	}
}
