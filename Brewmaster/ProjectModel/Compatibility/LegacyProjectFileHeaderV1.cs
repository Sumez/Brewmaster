using System;
using System.Xml.Serialization;

namespace BrewMaster.ProjectModel.Compatibility
{
	[Serializable]
	[XmlRoot("ProjectFileHeader")]
	public class LegacyProjectFileHeaderV1 : ProjectFileHeader
	{
		[XmlArray("Files")]
		[XmlArrayItem("ProjectFileFileHeader")]
		public ProjectFileFileHeader[] LegacyFiles;

		[XmlArray("BuildConfigurations")]
		[XmlArrayItem("BuildConfigurationHeader")]
		public BuildConfigurationHeader[] LegacyBuildConfigurations;

		[XmlArray("ExtraDirectories")]
		public string[] LegacyExtraDirectories;

		public virtual void FixCompatibility()
		{
			Files = LegacyFiles;
			ExtraDirectories = LegacyExtraDirectories;
			BuildConfigurations = LegacyBuildConfigurations;
		}
	}
}