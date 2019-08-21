namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;

	/// <summary>
	/// Mapping definition that provides tools to map data to and from index-based data structures.
	/// </summary>
	public interface IIndexMappingDefinition : IMappingDefinition
	{
		/// <summary>
		/// The attribute type that defines that an object is to be mapped to a index-based data structure.
		/// </summary>
		/// <value></value>
		Type IndexBasedClassMarkingAttribute
		{
			get;
		}

		/// <summary>
		/// The attribute type to denote that a field is part of mapping process.
		/// </summary>
		/// <value></value>
		Type IndexBasedFieldAttribute
		{
			get;
		}

		/// <summary>
		/// The type of the index-based data structure to use when mapping data.
		/// </summary>
		/// <value></value>
		Type IndexBasedMapType
		{
			get;
		}
	}

	/// <summary>
	/// Generic mapping definition that provides tools to map data to and from index-based data structures.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines that an object is to be mapped to a index-based data structure.</typeparam>
	/// <typeparam name="U">The attribute type to denote that a field is part of mapping process.</typeparam>
	/// <typeparam name="V">The type of the index-based data structure to use when mapping data.</typeparam>
	public interface IIndexMappingDefinition<T, U, V> : IIndexMappingDefinition
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	{ }
}
