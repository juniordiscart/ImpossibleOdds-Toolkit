namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Lookup-based interface for resolving the type of an object during (de)serialization.
	/// </summary>
	public interface ILookupTypeResolveSupport
	{
		/// <summary>
		/// The type of the attribute that is used to resolve the type in a lookup-based data structure.
		/// </summary>
		Type TypeResolveAttribute
		{
			get;
		}

		/// <summary>
		/// The key that's used to store the type value in the lookup-based datastructure.
		/// </summary>
		object TypeResolveKey
		{
			get;
		}
	}

	/// <summary>
	/// Generic interface for resolving the type of an object during (de)serialization.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines which attribute to use to resolve the type in a lookup-based data structure.</typeparam>
	public interface ILookupTypeResolveSupport<T> : ILookupTypeResolveSupport
	where T : Attribute, ITypeResolveParameter
	{ }
}
