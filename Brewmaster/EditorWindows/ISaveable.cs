using System;
using System.IO;

namespace Brewmaster.EditorWindows
{
	public interface ISaveable
	{
		void Save(Func<FileInfo, string> getNewFileName = null);
		bool Pristine { get; }
		event Action PristineChanged;
	}
}
