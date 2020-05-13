namespace ImpossibleOdds.Json
{
	using ImpossibleOdds.Serialization;

	public class JsonOptions
	{
		/// <summary>
		/// Enable/disable whether the output should get minified.
		/// </summary>
		public bool Minify
		{
			get; set;
		}

		/// <summary>
		/// Enable/disable escaping the '/' character in the output.
		/// </summary>
		public bool EscapeSlashCharacter
		{
			get; set;
		}

		/// <summary>
		/// Serialization definition to alter the default behaviour of the JSON processor.
		/// </summary>
		public ISerializationDefinition CustomSerializationDefinition
		{
			get; set;
		}

		public JsonOptions()
		{
			Minify = true;
			EscapeSlashCharacter = true;
			CustomSerializationDefinition = null;
		}
	}
}
