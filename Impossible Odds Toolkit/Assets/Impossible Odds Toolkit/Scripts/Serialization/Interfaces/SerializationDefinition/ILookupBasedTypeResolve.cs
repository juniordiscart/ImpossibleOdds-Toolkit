namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Lookup-based interface for resolving the type of an object during (de)serialization.
	/// </summary>
	public interface ILookupBasedTypeResolve
	{
		/// <summary>
		/// The type of the attribute that is used to resolve the type in a lookup-based data structure.
		/// </summary>
		Type TypeResolveAttribute
		{
			get;
		}
	}

	/// <summary>
	/// Generic interface for resolving the type of an object during (de)serialization.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines which attribute to use to resolve the type in a lookup-based data structure.</typeparam>
	public interface ILookupBasedTypeResolve<T> : ILookupBasedTypeResolve
	where T : Attribute, ILookupTypeResolveParameter
	{ }
}
