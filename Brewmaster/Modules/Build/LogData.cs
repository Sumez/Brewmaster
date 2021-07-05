using System;
using Brewmaster.BuildProcess;

namespace Brewmaster.Modules.Build
{
	public enum LogType
	{
		Normal, Error, Headline, Warning
	}
	public class LogData
	{
		public string Text { get; set; }
		public LogType Type { get; set; }
		public IFileLocation Location { get; set; }

		public LogData(string text, LogType type = LogType.Normal, IFileLocation fileLocation = null)
		{
			if (text == null) throw new ArgumentNullException("text", "Log message cannot be null");
			Text = text;
			Type = type;
			Location = fileLocation;
		}
	}
}
