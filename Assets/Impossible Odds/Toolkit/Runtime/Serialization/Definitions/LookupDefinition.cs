namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;

	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Abstract generic serialization definiton that only supports lookup-based serialized data.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be (de)serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the lookup-based fields and properties.</typeparam>
	/// <typeparam name="V">Type of the lookup-based container that should get used.</typeparam>
	public abstract class LookupDefinition<T, U, V> : ILookupSerializationDefinition<T, U, V>
	where T : Attribute, ILookupTypeObject
	where U : Attribute, ILookupParameter
	where V : IDictionary
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;

		/// <inheritdoc />
		public Type LookupBasedClassMarkingAttribute
		{
			get => typeof(T);
		}

		/// <inheritdoc />
		public Type LookupBasedFieldAttribute
		{
			get => typeof(U);
		}

		/// <inheritdoc />
		public Type LookupBasedDataType
		{
			get => typeof(V);
		}

		/// <inheritdoc />
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

		public abstract V CreateLookupInstance(int capacity);

		IDictionary ILookupSerializationDefinition.CreateLookupInstance(int capacity)
		{
			return CreateLookupInstance(capacity);
		}
	}
}
