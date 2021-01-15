namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;

	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Abstract generic serialization definiton that implements both index-based and lookup-based (de)serialization.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be (de)serialized as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="X">Attribute type that defines if an object should be (de)serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="Y">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="Z">Type of the lookup-based container that should get used.</typeparam>
	public abstract class IndexAndLookupDefinition<T, U, V, X, Y, Z> :
	IIndexSerializationDefinition<T, U, V>,
	ILookupSerializationDefinition<X, Y, Z>
	where T : Attribute
	where U : Attribute, IIndexParameter
	where V : IList
	where X : Attribute
	where Y : Attribute, ILookupParameter
	where Z : IDictionary
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;

		public Type IndexBasedClassMarkingAttribute
		{
			get { return typeof(T); }
		}

		public Type IndexBasedFieldAttribute
		{
			get { return typeof(U); }
		}

		public Type IndexBasedDataType
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

		public Type LookupBasedDataType
		{
			get { return typeof(Z); }
		}

		public IFormatProvider FormatProvider
		{
			get { return formatProvider; }
			set { formatProvider = value; }
		}

		public abstract IEnumerable<ISerializationProcessor> SerializationProcessors { get; }
		public abstract IEnumerable<IDeserializationProcessor> DeserializationProcessors { get; }
		public abstract HashSet<Type> SupportedTypes { get; }

		public abstract V CreateSequenceInstance(int capacity);
		public abstract Z CreateLookupInstance(int capacity);

		IList IIndexSerializationDefinition.CreateSequenceInstance(int capacity)
		{
			return (this as IIndexSerializationDefinition<T, U, V>).CreateSequenceInstance(capacity);
		}

		IDictionary ILookupSerializationDefinition.CreateLookupInstance(int capacity)
		{
			return (this as ILookupSerializationDefinition<X, Y, Z>).CreateLookupInstance(capacity);
		}
	}
}
