namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Base interface for type resolution interfaces working in a lookup-like fashion with support for overriding the key value.
	/// </summary>
	public interface ILookupTypeResolutionParameter : ITypeResolutionParameter
	{
		/// <summary>
		/// Optional value to override the key defined by the type resolution feature.
		/// </summary>
		object KeyOverride
		{
			get;
		}
	}
}