using System;
using System.Collections.Generic;
using Brewmaster.ProjectModel;

namespace Brewmaster.Pipeline
{
	public class CustomTileMapPipeline : PipelineOption
	{
		public override string TypeName
		{
			get { return "tilemap.custom";  }
		}
		public override IEnumerable<FileType> SupportedFileTypes { get { return new [] { FileType.TileMap }; } }
		public override void Process(PipelineSettings settings)
		{
			throw new NotImplementedException();
		}

		public override PipelineSettings Create(AsmProjectFile file)
		{
			return CreateGeneric(file, ".inc");
		}

		public override IEnumerable<PipelineProperty> Properties
		{
			get
			{
				return new[]
				{
					new PipelineProperty("script-type", PipelinePropertyType.Select, "Python", new List<string> { /*"JavaScript",*/ "Python", "Lua" }),
					new PipelineProperty("script-file", PipelinePropertyType.ProjectFile, "", GetFileNameFilter)
				};
			}
		}

		private string GetFileNameFilter(PipelineSettings settings)
		{
			switch (settings.GenericSettings["script-type"]) {
				case "Python":
					return "*.py (Python script)|*.py|*.*|*.*";
				case "Lua":
					return "*.lua (Lua script)|*.lua|*.*|*.*";
				case "JavaScript":
					return "*.js (JavaScript/EcmaScript)|*.js|*.*|*.*";
				default:
					return "*.*|*.*";
			}
		}
	}
}
