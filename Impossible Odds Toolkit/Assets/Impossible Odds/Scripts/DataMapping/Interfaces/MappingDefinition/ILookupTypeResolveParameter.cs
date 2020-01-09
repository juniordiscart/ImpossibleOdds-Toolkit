namespace ImpossibleOdds.DataMapping
{
	/// <summary>
	/// Defines a specific type resolve in a lookup data structure.
	/// </summary>
	public interface ILookupTypeResolveParameter : IMappingTypeResolveParameter
	{
		/// <summary>
		/// The key that the type resolve information will be stored as in the lookup data structure.
		/// </summary>
		object Key
		{
			get;
		}

		/// <summary>
		/// The value that uniquely links to the target type in the lookup data structure.
		/// </summary>
		object Value
		{
			get;
		}
	}
}
