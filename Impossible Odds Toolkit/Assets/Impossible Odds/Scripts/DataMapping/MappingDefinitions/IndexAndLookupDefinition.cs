namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using ImpossibleOdds.DataMapping.Processors;

	/// <summary>
	/// Abstract generic mapping definiton that implements both index-based and lookup-based mapping definitions.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="X">Attribute type that defines if an object should be mapped as a lookup-based data structure.</typeparam>
	/// <typeparam name="Y">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="Z">Type of the lookup-based container that should get used.</typeparam>
	public abstract class IndexAndLookupDefinition<T, U, V, X, Y, Z> : IIndexMappingDefinition<T, U, V>, ILookupMappingDefinition<X, Y, Z>
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	where X : Attribute, ILookupDataStructure
	where Y : Attribute, ILookupParameter
	where Z : IDictionary
	{
		public Type IndexBasedClassMarkingAttribute
		{
			get { return typeof(T); }
		}

		public Type IndexBasedFieldAttribute
		{
			get { return typeof(U); }
		}

		public Type IndexBasedMapType
		{
			get { return typeof(V); }
		}

		public Type LookupBasedClassMarkingAttribute
		{
			get { return typeof(X); }
		}

		public Type LookupBasedFieldAttribute
		{
			get { return typeof(Y); }
		}

		public Type LookupBasedMapType
		{
			get { return typeof(Z); }
		}

		public abstract IEnumerable<IMapToDataStructureProcessor> ToDataStructureProcessors { get; }
		public abstract IEnumerable<IMapFromDataStructureProcessor> FromDataStructureProcessors { get; }
		public abstract HashSet<Type> SupportedProcessingTypes { get; }
	}
}
