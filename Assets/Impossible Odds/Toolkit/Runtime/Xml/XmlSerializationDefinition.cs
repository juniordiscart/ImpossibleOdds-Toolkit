namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;
	using ImpossibleOdds.Xml.Processors;

	public class XmlSerializationDefinition :
		ISerializationDefinition,
		ICallbacksSupport<OnXmlSerializingAttribute, OnXmlSerializedAttribute, OnXmlDeserializingAttribute, OnXmlDeserializedAttribute>,
		ILookupTypeResolveSupport<XmlTypeAttribute>,
		IRequiredValueSupport<XmlRequiredAttribute>,
		IParallelProcessingSupport,
		IEnumAliasSupport<XmlEnumStringAttribute, XmlEnumAliasAttribute>
	{
		public const string XmlSchemaURL = "http://www.w3.org/2001/XMLSchema-instance";
		public const string XmlSchemaPrefix = "xsi";
		public const string XmlTypeKey = "type";

		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private ISerializationProcessor[] serializationProcessors = null;
		private IDeserializationProcessor[] deserializationProcessors = null;
		private HashSet<Type> supportedTypes = null;
		private BinaryFormatter binaryFormatter = new BinaryFormatter();
		private ISerializationDefinition attributeDefinition = null;
		private ISerializationDefinition cdataDefinition = null;
		private bool processInParallel = false;

		private XName xmlTypeKey = null;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get => serializationProcessors;
		}

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get => deserializationProcessors;
		}

		/// <inheritdoc />
		public IFormatProvider FormatProvider
		{
			get => formatProvider;
			set => formatProvider = value;
		}

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes
		{
			get => supportedTypes;
		}

		/// <inheritdoc />
		public Type OnSerializationCallbackType
		{
			get => typeof(OnXmlSerializingAttribute);
		}

		/// <inheritdoc />
		public Type OnSerializedCallbackType
		{
			get => typeof(OnXmlSerializedAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializionCallbackType
		{
			get => typeof(OnXmlDeserializingAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializedCallbackType
		{
			get => typeof(OnXmlDeserializedAttribute);
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get => typeof(XmlTypeAttribute);
		}

		/// <inheritdoc />
		object ILookupTypeResolveSupport.TypeResolveKey
		{
			get => TypeResolveKey;
		}

		/// <summary>
		/// The attribute name used for storing type information.
		/// </summary>
		public XName TypeResolveKey
		{
			get => xmlTypeKey;
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get => typeof(XmlRequiredAttribute);
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get => typeof(XmlEnumStringAttribute);
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get => typeof(XmlEnumAliasAttribute);
		}

		/// <summary>
		/// The serialization definition used specifically for processing XML attributes.
		/// </summary>
		public ISerializationDefinition AttributeSerializationDefinition
		{
			get => attributeDefinition;
			set => attributeDefinition = value;
		}

		/// <summary>
		/// The serialization definition used specifically for processing XML CDATA sections.
		/// </summary>
		public ISerializationDefinition CDataSerializationDefinition
		{
			get => cdataDefinition;
			set => cdataDefinition = value;
		}

		/// <inheritdoc />
		bool IParallelProcessingSupport.Enabled
		{
			get => ParallelProcessingEnabled;
		}

		public bool ParallelProcessingEnabled
		{
			get => processInParallel;
			set => processInParallel = value;
		}

		public XmlSerializationDefinition()
		: this(false)
		{ }

		public XmlSerializationDefinition(bool enableParallelProcessing)
		{
			this.processInParallel = enableParallelProcessing;

			XmlPrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_XML_UNITY_TYPES_AS_ATTRIBUTES
				XmlPrimitiveProcessingMethod.ATTRIBUTES;
#else
				XmlPrimitiveProcessingMethod.ELEMENTS;
#endif

			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(char),
				typeof(XElement)
			};

			List<IProcessor> processors = new List<IProcessor>()
			{
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new VersionProcessor(this),
				new GuidProcessor(this),
				new StringProcessor(this),
				new XmlVector2Processor(this, defaultProcessingMethod),
				new XmlVector2IntProcessor(this, defaultProcessingMethod),
				new XmlVector3Processor(this, defaultProcessingMethod),
				new XmlVector3IntProcessor(this, defaultProcessingMethod),
				new XmlVector4Processor(this, defaultProcessingMethod),
				new XmlQuaternionProcessor(this, defaultProcessingMethod),
				new XmlColorProcessor(this, defaultProcessingMethod),
				new XmlColor32Processor(this, defaultProcessingMethod),
				new XmlLookupProcessor(this),
				new XmlSequenceProcessor(this),
				new XmlCustomObjectProcessor(this),
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();

			xmlTypeKey = XNamespace.Get(XmlSchemaURL) + XmlTypeKey;

			attributeDefinition = new XmlAttributeSerializationDefinition();
			cdataDefinition = new XmlCDataSerializationDefinition();
		}

		/// <summary>
		/// Update the registered processors that handle Unity primitive types to switch to a different (de)serialization style.
		/// </summary>
		/// <param name="preferredProcessingMethod">The preferred processing method.</param>
		public void UpdateUnityPrimitiveRepresentation(XmlPrimitiveProcessingMethod preferredProcessingMethod)
		{
			foreach (IProcessor processor in serializationProcessors)
			{
				if (processor is IUnityPrimitiveXmlSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}

			foreach (IProcessor processor in deserializationProcessors)
			{
				if (processor is IUnityPrimitiveXmlSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}
		}
	}
}
