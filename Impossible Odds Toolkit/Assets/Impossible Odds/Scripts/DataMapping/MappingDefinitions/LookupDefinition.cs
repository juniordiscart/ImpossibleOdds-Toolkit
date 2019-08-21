namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using ImpossibleOdds.DataMapping.Processors;

	/// <summary>
	/// Abstract generic mapping definiton that implements only a lookup-based mapping definition.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as a lookup-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="V">Type of the lookup-based container that should get used.</typeparam>
	public abstract class LookupDefinition<T, U, V> : ILookupMappingDefinition<T, U, V>
	where T : Attribute, ILookupDataStructure
	where U : Attribute, ILookupParameter
	where V : IDictionary
	{

		public Type LookupBasedClassMarkingAttribute
		{
			get { return typeof(T); }
		}

		public Type LookupBasedFieldAttribute
		{
			get { return typeof(U); }
		}

		public Type LookupBasedMapType
		{
			get { return typeof(V); }
		}

		public abstract IEnumerable<IMapToDataStructureProcessor> ToDataStructureProcessors { get; }
		public abstract IEnumerable<IMapFromDataStructureProcessor> FromDataStructureProcessors { get; }
		public abstract HashSet<Type> SupportedProcessingTypes { get; }
	}
}
