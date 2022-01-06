namespace ImpossibleOdds.Serialization
{

	/// <summary>
	/// Base interface for type resolve interfaces working in a lookup-like fashion with support for overriding the key value.
	/// </summary>
	public interface ILookupTypeResolveParameter : ITypeResolveParameter
	{
		/// <summary>
		/// Optional value to override the key defined by the definition.
		/// </summary>
		object KeyOverride
		{
			get;
		}
	}
}
