namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Abstract generic serialization definiton that only supports lookup-based serialized data.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be (de)serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="V">Type of the lookup-based container that should get used.</typeparam>
	public abstract class LookupDefinition<T, U, V> : ILookupSerializationDefinition<T, U, V>
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

		public Type LookupBasedDataType
		{
			get { return typeof(V); }
		}

		public abstract IEnumerable<ISerializationProcessor> SerializationProcessors { get; }
		public abstract IEnumerable<IDeserializationProcessor> DeserializationProcessors { get; }
		public abstract HashSet<Type> SupportedTypes { get; }

		public abstract V CreateLookupInstance(int capacity);

		IDictionary ILookupSerializationDefinition.CreateLookupInstance(int capacity)
		{
			return CreateLookupInstance(capacity);
		}
	}
}
