using System;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public class TilemapAsmPipeline : PipelineOption
	{
		public override string TypeName { get { return "tilemap.asm"; } }
		public override void Process(PipelineSettings dataPipelineSettings)
		{
			throw new NotImplementedException();
		}

		public override PipelineSettings Clone(PipelineSettings dataPipelineSettings)
		{
			throw new NotImplementedException();
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return new PipelineSettings(this, file);
		}

		public override PipelineSettings Load(AsmProject project, PipelineHeader pipelineHeader)
		{
			throw new NotImplementedException();
		}
	}
}