namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using ImpossibleOdds.DataMapping.Processors;

	/// <summary>
	/// Abstract generic mapping definiton that implements only an index-based mapping definition.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	public abstract class IndexDefinition<T, U, V> : IIndexMappingDefinition<T, U, V>
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
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

		public abstract IEnumerable<IMapToDataStructureProcessor> ToDataStructureProcessors { get; }
		public abstract IEnumerable<IMapFromDataStructureProcessor> FromDataStructureProcessors { get; }
		public abstract HashSet<Type> SupportedProcessingTypes { get; }
	}
}
