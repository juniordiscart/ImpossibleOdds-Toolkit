namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Defines a field-based parameter during the (de)serialization process.
	/// </summary>
	public interface ILookupParameter
	{
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
		new TKey Key
		{
			get;
		}
	}
}
