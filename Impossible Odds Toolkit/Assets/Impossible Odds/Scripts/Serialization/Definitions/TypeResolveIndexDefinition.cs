namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic serialization definition that only supports index-based serialized data structures and has support for type resolvement.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be serialized as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="W">Attribute type for the index-based type resolvement attribute.</typeparam>
	public abstract class TypeResolveIndexDefinition<T, U, V, W> :
	IndexDefinition<T, U, V>,
	IIndexBasedTypeResolve<W>
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
