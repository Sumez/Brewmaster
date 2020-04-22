using System;
using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public abstract class DataPipeline
	{
		// DATA ESSENTIAL TO OUR STORED FILE
		public AsmProjectFile File { get; set; }
		public abstract IEnumerable<string> OutputFiles { get; }
		public DateTime? LastProcessed { get; protected set; }

		// END

		protected DataPipeline(AsmProjectFile file, DateTime? lastProcessed = null)
		{
			File = file;
			LastProcessed = lastProcessed;
		}
		public abstract void Process();

		public virtual void GetSettings(ProjectModel.Properties headerSettings)
		{
			
		}

		public virtual void SettingChanged()
		{
			LastProcessed = null;
		}

		public abstract DataPipeline Clone(bool toEditor = false);
	}
}


