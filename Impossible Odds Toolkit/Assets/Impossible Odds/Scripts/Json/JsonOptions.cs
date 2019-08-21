namespace ImpossibleOdds.Json
{
	using ImpossibleOdds.DataMapping;

	public class JsonOptions
	{
		/// <summary>
		/// Enable/disable whether the output should get minified.
		/// </summary>
		/// <value></value>
		public bool Minify
		{
			get; set;
		}

		/// <summary>
		/// Enable/disable escaping the '/' character in the output.
		/// </summary>
		/// <value></value>
		public bool EscapeSlashCharacter
		{
			get; set;
		}

		/// <summary>
		/// Mapping definition to alter the default behaviour of the JSON processor.
		/// </summary>
		/// <value></value>
		public IMappingDefinition CustomMappingDefinition
		{
			get; set;
		}

		public JsonOptions()
		{
			Minify = true;
			EscapeSlashCharacter = true;
			CustomMappingDefinition = null;
		}
	}
}
