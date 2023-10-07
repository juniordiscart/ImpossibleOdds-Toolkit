using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	public class JsonOptions
	{
		/// <summary>
		/// Enable/disable whether the output should waste as little space as possible.
		/// </summary>
		public bool CompactOutput
		{
			get;
			set;
		} = true;

		/// <summary>
		/// Enable/disable escaping the '/' character in the output.
		/// </summary>
		public bool EscapeSlashCharacter
		{
			get;
			set;
		} = true;

		/// <summary>
		/// Serialization definition to alter the default behaviour of the JSON processor.
		/// </summary>
		public ISerializationDefinition SerializationDefinition
		{
			get;
			set;
		}

		/// <summary>
		/// Optional serialization configuration for lookup objects.
		/// </summary>
		public ILookupSerializationConfiguration LookupSerializationConfiguration
		{
			get;
			set;
		}

		/// <summary>
		/// Optional serialization configuration for sequence objects.
		/// </summary>
		public ISequenceSerializationConfiguration SequenceSerializationConfiguration
		{
			get;
			set;
		}
	}
}