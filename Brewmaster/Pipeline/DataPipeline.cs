using System;
using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public class PipelineSettings
	{
		public static readonly List<PipelineOption> PipelineOptions = new List<PipelineOption>
		{
			new TilemapAsmPipeline(),
			new TilemapBinaryPipeline(),
			new ChrPipeline()
		};


		// DATA ESSENTIAL TO OUR STORED FILE
		public AsmProjectFile File { get; set; }
		public virtual List<string> OutputFiles { get; } = new List<string>();
		public DateTime? LastProcessed { get; protected set; }
		public PipelineOption Type { get; }
		// END

		public PipelineSettings(PipelineOption type, AsmProjectFile file, DateTime? lastProcessed = null)
		{
			File = file;
			LastProcessed = lastProcessed;
			Type = type;
		}

		public void Process()
		{
			Type.Process(this);
			LastProcessed = DateTime.Now;
		}

		public PipelineSettings Clone()
		{
			return Type.Clone(this);
		}

		public virtual void SettingChanged()
		{
			LastProcessed = null;
		}
	}
}


