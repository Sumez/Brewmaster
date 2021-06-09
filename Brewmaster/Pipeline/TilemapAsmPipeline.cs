﻿using System;
using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public class TilemapAsmPipeline : PipelineOption
	{
		public override IEnumerable<FileType> SupportedFileTypes { get { return new [] {FileType.TileMap};}}
		public override string TypeName { get { return "tilemap.asm"; } }
		public override void Process(PipelineSettings dataPipelineSettings)
		{
			throw new NotImplementedException();
		}
		
		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".inc");
		}
	}
}