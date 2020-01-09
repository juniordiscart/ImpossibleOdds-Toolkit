namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Lookup-based interface for resolving the type of an object.
	/// </summary>
	public interface ILookupTypeResolve
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
	/// Generic interface for resolving the type of an object.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines which attribute to use to resolve the type in a lookup-based mapping data structure.</typeparam>
	public interface ILookupTypeResolve<T> : ILookupTypeResolve
	where T : Attribute, ILookupTypeResolveParameter
	{ }
}
