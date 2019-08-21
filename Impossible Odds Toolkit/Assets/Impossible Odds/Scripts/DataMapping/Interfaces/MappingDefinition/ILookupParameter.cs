namespace ImpossibleOdds.DataMapping
{
	/// <summary>
	/// Defines a field-based parameter during the data (un)mapping process.
	/// </summary>
	public interface ILookupParameter
	{
		object Key
		{
			get;
		}
	}

	/// <summary>
	/// Defines a generic field-based parameter during the data (un)mapping process.
	/// </summary>
	public interface ILookupParameter<TKey> : ILookupParameter
	{
		new TKey Key
		{
			get;
		}
	}
}
