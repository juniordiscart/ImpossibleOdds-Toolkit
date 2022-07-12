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
	/// <typeparam name="TListObject">Attribute type that defines if an object should be (de)serialized as an index-based data structure.</typeparam>
	/// <typeparam name="TListField">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="TListType">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="TLookupObject">Attribute type that defines if an object should be (de)serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="TLookupField">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="TLookupType">Type of the lookup-based container that should get used.</typeparam>
	public abstract class IndexAndLookupDefinition<TListObject, TListField, TListType, TLookupObject, TLookupField, TLookupType> :
	IIndexSerializationDefinition<TListObject, TListField, TListType>,
	ILookupSerializationDefinition<TLookupObject, TLookupField, TLookupType>
	where TListObject : Attribute, IIndexTypeObject
	where TListField : Attribute, IIndexParameter
	where TListType : IList
	where TLookupObject : Attribute, ILookupTypeObject
	where TLookupField : Attribute, ILookupParameter
	where TLookupType : IDictionary
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;

		public Type IndexBasedClassMarkingAttribute
		{
			get => typeof(TListObject);
		}

		public Type IndexBasedFieldAttribute
		{
			get => typeof(TListField);
		}

		public Type IndexBasedDataType
		{
			get => typeof(TListType);
		}

		public Type LookupBasedClassMarkingAttribute
		{
			get => typeof(TLookupObject);
		}

		public Type LookupBasedFieldAttribute
		{
			get => typeof(TLookupField);
		}

		public Type LookupBasedDataType
		{
			get => typeof(TLookupType);
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


		public abstract TListType CreateSequenceInstance(int capacity);
		public abstract TLookupType CreateLookupInstance(int capacity);

		IList IIndexSerializationDefinition.CreateSequenceInstance(int capacity)
		{
			return CreateSequenceInstance(capacity);
		}

		IDictionary ILookupSerializationDefinition.CreateLookupInstance(int capacity)
		{
			return CreateLookupInstance(capacity);
		}
	}
}
