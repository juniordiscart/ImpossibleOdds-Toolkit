namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;

	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Abstract generic serialization definiton that only supports index-based serialized data.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be (de)serialized as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	public abstract class IndexDefinition<T, U, V> : IIndexSerializationDefinition<T, U, V>
	where T : Attribute, IIndexTypeObject
	where U : Attribute, IIndexParameter
	where V : IList
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;

		public Type IndexBasedClassMarkingAttribute
		{
			get => typeof(T);
		}

		public Type IndexBasedFieldAttribute
		{
			get => typeof(U);
		}

		public Type IndexBasedDataType
		{
			get => typeof(V);
		}

		public IFormatProvider FormatProvider
		{
			get => formatProvider;
			set => formatProvider = value;
		}

		public abstract IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get;
		}

		public abstract IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get;
		}

		public abstract HashSet<Type> SupportedTypes
		{
			get;
		}

		public abstract V CreateSequenceInstance(int capacity);

		IList IIndexSerializationDefinition.CreateSequenceInstance(int capacity)
		{
			return CreateSequenceInstance(capacity);
		}
	}
}
