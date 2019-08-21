namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic mapping definiton that implements only an index-based mapping definition with support for type resolvement.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="W">Attribute type for the index-based type resolvement attribute.</typeparam>
	public abstract class TypeResolveIndexDefinition<T, U, V, W> : IndexDefinition<T, U, V>, IIndexTypeResolve<W>
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	where W : Attribute, IIndexTypeResolveParameter
	{
		public Type TypeResolveAttribute
		{
			get { return typeof(W); }
		}
	}
}
