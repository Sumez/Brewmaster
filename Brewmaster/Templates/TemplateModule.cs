using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Templates
{
	public class TemplateModule
	{
		public TargetPlatform Platform;
		public string Name;
		public string Directory;
		public List<TemplateModule> Dependencies;
		public string InitCode;
	}
}
