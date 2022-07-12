namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	/// <summary>
	/// Serialization definition that provides tools to (de)serialize lookup-based data structures.
	/// </summary>
	public interface ILookupSerializationDefinition : ISerializationDefinition
	{
		/// <summary>
		/// The attribute type that defines that an object is to be (de)serialize to a lookup-based data structure.
		/// </summary>
		Type LookupBasedClassMarkingAttribute
		{
			get;
		}

		/// <summary>
		/// The attribute type to denote that a field is part of the (de)serialization process.
		/// </summary>
		Type LookupBasedFieldAttribute
		{
			get;
		}

		/// <summary>
		/// The type of the lookup-based data structure to use when (de)serializing data.
		/// </summary>
		Type LookupBasedDataType
		{
			get;
		}

		/// <summary>
		/// Create an instance of the supported lookup-based collection type.
		/// </summary>
		/// <param name="capacity">The capacity the collection should have.</param>
		/// <returns>Instance of the collection.</returns>
		IDictionary CreateLookupInstance(int capacity);
	}

	/// <summary>
	/// Generic serialization definition that provides tools to (de)serialize lookup-based data structures.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines that an object is to be (de)serialized to a lookup-based data structure.</typeparam>
	/// <typeparam name="U">The attribute type to denote that a field is part of the (de)serialization process.</typeparam>
	/// <typeparam name="V">The type of the lookup-based data structure to use when (de)serializing data.</typeparam>
	public interface ILookupSerializationDefinition<T, U, V> : ILookupSerializationDefinition
	where T : Attribute, ILookupTypeObject
	where U : Attribute, ILookupParameter
	where V : IDictionary
	{
		/// <summary>
		/// Create an instance of the supported lookup-based collection type.
		/// </summary>
		/// <param name="capacity">The capacity the collection should have.</param>
		/// <returns>Instance of the collection.</returns>
		new V CreateLookupInstance(int capacity);
	}
}
