namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	/// <summary>
	/// Serialization definition that provides tools to (de)serialize data to and from index-based data structures.
	/// </summary>
	public interface IIndexSerializationDefinition : ISerializationDefinition
	{
		/// <summary>
		/// The attribute type that defines that an object is to be (de)serialized to a index-based data structure.
		/// </summary>
		Type IndexBasedClassMarkingAttribute
		{
			get;
		}

		/// <summary>
		/// The attribute type to denote that a field is part of the (de)serialization process.
		/// </summary>
		Type IndexBasedFieldAttribute
		{
			get;
		}

		/// <summary>
		/// The type of the index-based data structure to use when (de)serializing data.
		/// </summary>
		Type IndexBasedDataType
		{
			get;
		}
	}

	/// <summary>
	/// Generic (de)serialization definition that provides tools to (de)serialize data to and from index-based data structures.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines that an object is to be (de)serialized to an index-based data structure.</typeparam>
	/// <typeparam name="U">The attribute type to denote that a field is part of the (de)serialization process.</typeparam>
	/// <typeparam name="V">The type of the index-based data structure to use when (de)serializing data.</typeparam>
	public interface IIndexSerializationDefinition<T, U, V> : IIndexSerializationDefinition
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	{ }
}
