namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Defines a specific type resolve in a lookup data structure.
	/// </summary>
	public interface ILookupTypeResolveParameter : ISerializationTypeResolveParameter
	{
		/// <summary>
		/// The value that uniquely links to the target type in the lookup data structure.
		/// </summary>
		object Value
		{
			get;
		}
	}
}
