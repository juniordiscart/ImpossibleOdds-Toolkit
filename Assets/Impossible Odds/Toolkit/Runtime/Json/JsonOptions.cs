namespace ImpossibleOdds.Json
{
	using ImpossibleOdds.Serialization;

	public class JsonOptions
	{
		/// <summary>
		/// Enable/disable whether the output should waste as little space as possible.
		/// </summary>
		public bool CompactOutput
		{
			get;
			set;
		}

		/// <summary>
		/// Enable/disable escaping the '/' character in the output.
		/// </summary>
		public bool EscapeSlashCharacter
		{
			get;
			set;
		}

		/// <summary>
		/// Serialization definition to alter the default behaviour of the JSON processor.
		/// </summary>
		public ISerializationDefinition SerializationDefinition
		{
			get;
			set;
		}

		public JsonOptions()
		{
			CompactOutput = true;
			EscapeSlashCharacter = true;
			SerializationDefinition = null;
		}
	}
}
