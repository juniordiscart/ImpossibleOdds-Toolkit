namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;

	public class AssemblyDefinition
	{
#pragma warning disable 0649
		public string name;
		public List<string> references;
		public List<string> includePlatforms;
		public List<string> excludePlatforms;
#pragma warning restore 0649
	}
}