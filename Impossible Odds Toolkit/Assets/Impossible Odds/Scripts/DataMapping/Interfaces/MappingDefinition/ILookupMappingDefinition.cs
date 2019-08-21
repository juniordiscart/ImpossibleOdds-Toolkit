namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;

	/// <summary>
	/// Mapping definition that provide tools to map to and from lookup-based data structures.
	/// </summary>
	public interface ILookupMappingDefinition : IMappingDefinition
	{
		/// <summary>
		/// The attribute type that defines that an object is to be mapped to a lookup-based data structure.
		/// </summary>
		/// <value></value>
		Type LookupBasedClassMarkingAttribute
		{
			get;
		}

		/// <summary>
		/// The attribute type to denote that a field is part of mapping process.
		/// </summary>
		/// <value></value>
		Type LookupBasedFieldAttribute
		{
			get;
		}

		/// <summary>
		/// The type of the lookup-based data structure to use when mapping data.
		/// </summary>
		/// <value></value>
		Type LookupBasedMapType
		{
			get;
		}
	}

	/// <summary>
	/// Generic mapping definition that provide tools to map to and from lookup-based data structures.
	/// </summary>
	/// <typeparam name="T">The attribute type that defines that an object is to be mapped to a lookup-based data structure.</typeparam>
	/// <typeparam name="U">The attribute type to denote that a field is part of mapping process.</typeparam>
	/// <typeparam name="V">The type of the lookup-based data structure to use when mapping data.</typeparam>
	public interface ILookupMappingDefinition<T, U, V> : ILookupMappingDefinition
	where T : Attribute, ILookupDataStructure
	where U : Attribute, ILookupParameter
	where V : IDictionary
	{ }
}
