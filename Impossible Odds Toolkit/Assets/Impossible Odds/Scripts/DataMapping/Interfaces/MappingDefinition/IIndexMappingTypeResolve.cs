namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Index-based interface for resolving the type of an object.
	/// </summary>
	public interface IIndexTypeResolve
	{
		/// <summary>
		/// The type of the attribute that is used to resolve the type in an index-based data structure.
		/// </summary>
		/// <value></value>
		Type TypeResolveAttribute
		{
			get;
		}
	}

	/// <summary>
	/// Generic interface for resolving the type of an object.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines which attribute to use to resolve the type in an index-based mapping data structure.</typeparam>
	public interface IIndexTypeResolve<T> : IIndexTypeResolve
	where T : Attribute, IIndexTypeResolveParameter
	{ }
}
