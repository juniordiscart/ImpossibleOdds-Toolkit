namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlSerializationDefinition :
	LookupDefinition<XmlObjectAttribute, XmlFieldAttribute, Dictionary<string, object>>,
	IIndexSerializationDefinition,
	ICallbacksSupport<OnXmlSerializingAttribute, OnXmlSerializedAttribute, OnXmlDeserializingAttribute, OnXmlDeserializedAttribute>,
	ILookupTypeResolveSupport<XmlTypeAttribute>,
	IRequiredValueSupport<XmlRequiredAttribute>,
	IEnumAliasSupport<XmlEnumStringAttribute, XmlEnumAliasAttribute>
	{
		public const string XmlTypeKey = "xsi:type";

		private IEnumerable<ISerializationProcessor> serializationProcessors = null;
		private IEnumerable<IDeserializationProcessor> deserializationProcessors = null;
		private HashSet<Type> supportedTypes = null;

		/// <inheritdoc />
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get { return serializationProcessors; }
		}

		/// <inheritdoc />
		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get { return deserializationProcessors; }
		}

		/// <inheritdoc />
		public override HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		/// <inheritdoc />
		public Type OnSerializationCallbackType
		{
			get { return typeof(OnXmlSerializingAttribute); }
		}

		/// <inheritdoc />
		public Type OnSerializedCallbackType
		{
			get { return typeof(OnXmlSerializedAttribute); }
		}

		/// <inheritdoc />
		public Type OnDeserializionCallbackType
		{
			get { return typeof(OnXmlDeserializingAttribute); }
		}

		/// <inheritdoc />
		public Type OnDeserializedCallbackType
		{
			get { return typeof(OnXmlDeserializedAttribute); }
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get { return typeof(XmlTypeAttribute); }
		}

		/// <inheritdoc />
		public object TypeResolveKey
		{
			get { return XmlTypeKey; }
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get { return typeof(XmlRequiredAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get { return typeof(XmlEnumStringAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get { return typeof(XmlEnumAliasAttribute); }
		}

		Type IIndexSerializationDefinition.IndexBasedClassMarkingAttribute
		{
			get { throw new NotImplementedException(); }
		}

		Type IIndexSerializationDefinition.IndexBasedFieldAttribute
		{
			get { throw new NotImplementedException(); }
		}

		Type IIndexSerializationDefinition.IndexBasedDataType
		{
			get { return typeof(List<object>); }
		}

		public XmlSerializationDefinition()
		{
			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(char)
			};

			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this),
				new Vector2Processor(this as ILookupSerializationDefinition),
				new Vector2IntProcessor(this as ILookupSerializationDefinition),
				new Vector3Processor(this as ILookupSerializationDefinition),
				new Vector3IntProcessor(this as ILookupSerializationDefinition),
				new Vector4Processor(this as ILookupSerializationDefinition),
				new QuaternionProcessor(this as ILookupSerializationDefinition),
				new ColorProcessor(this as ILookupSerializationDefinition),
				new Color32Processor(this as ILookupSerializationDefinition),
				new LookupProcessor(this),
				new SequenceProcessor(this),
				new CustomObjectLookupProcessor(this),
			};
		}

		/// <inheritdoc />
		public override Dictionary<string, object> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, object>(capacity);
		}

		/// <inheritdoc />
		IList IIndexSerializationDefinition.CreateSequenceInstance(int capacity)
		{
			return new List<object>(capacity);
		}
	}
}
