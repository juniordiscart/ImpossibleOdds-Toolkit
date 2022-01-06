namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Index-based interface for resolving the type of an object during (de)serialization.
	/// </summary>
	public interface IIndexTypeResolveSupport
	{
		/// <summary>
		/// The type of the attribute that is used to resolve the type in an index-based data structure.
		/// </summary>
		Type TypeResolveAttribute
		{
			get;
		}

		/// <summary>
		/// Index of where the type information can be found in the index-based data structure.
		/// </summary>
		int TypeResolveIndex
		{
			get;
		}
	}

	/// <summary>
	/// Generic interface for resolving the type of an object during (de)serialization.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines which attribute to use to resolve the type in an index-based data structure.</typeparam>
	public interface IIndexTypeResolveSupport<T> : IIndexTypeResolveSupport
	where T : Attribute, ITypeResolveParameter
	{ }
}
