namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	public interface IIndexAndLookupSerializationDefinition : IIndexSerializationDefinition, ILookupSerializationDefinition
	{ }

	public interface IIndexAndLookupSerializationDefinition<T, U, V, X, Y, Z> :
	IIndexAndLookupSerializationDefinition,
	IIndexSerializationDefinition<T, U, V>,
	ILookupSerializationDefinition<X, Y, Z>
	where T : Attribute
	where U : Attribute, IIndexParameter
	where V : IList
	where X : Attribute
	where Y : Attribute, ILookupParameter
	where Z : IDictionary
	{ }
}
