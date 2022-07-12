namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Defines a field-based parameter during the (de)serialization process.
	/// </summary>
	public interface ILookupParameter
	{
		/// <summary>
		/// The key used in the lookup data structure.
		/// </summary>
		object Key
		{
			get;
		}
	}

	/// <summary>
	/// Defines a generic field-based parameter during the (de)serialization process.
	/// </summary>
	public interface ILookupParameter<TKey> : ILookupParameter
	{
		/// <summary>
		/// The key used in the lookup data structure.
		/// </summary>
		new TKey Key
		{
			get;
		}
	}
}
