namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
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
	IEnumAliasSupport<XmlEnumStringAttribute, XmlEnumAliasAttribute>
	{
		public const string XmlSchemaURL = "http://www.w3.org/2001/XMLSchema-instance";
		public const string XmlSchemaPrefix = "xsi";
		public const string XmlTypeKey = "type";

		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private List<IProcessor> processors = null;
		private HashSet<Type> supportedTypes = null;
		private BinaryFormatter binaryFormatter = new BinaryFormatter();
		private ISerializationDefinition attributeDefinition = null;
		private ISerializationDefinition cdataDefinition = null;

		private XName xmlTypeKey = null;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get
			{
				foreach (IProcessor processor in processors)
				{
					if (processor is ISerializationProcessor serializationProcessor)
					{
						yield return serializationProcessor;
					}
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get
			{
				foreach (IProcessor processor in processors)
				{
					if (processor is IDeserializationProcessor deserializationProcessor)
					{
						yield return deserializationProcessor;
					}
				}
			}
		}

		/// <inheritdoc />
		public IFormatProvider FormatProvider
		{
			get { return formatProvider; }
			set { formatProvider = value; }
		}

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes
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
		object ILookupTypeResolveSupport.TypeResolveKey
		{
			get { return TypeResolveKey; }
		}

		/// <summary>
		/// The attribute name used for storing type information.
		/// </summary>
		public XName TypeResolveKey
		{
			get { return xmlTypeKey; }
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

		/// <summary>
		/// The serialization definition used specifically for processing XML attributes.
		/// </summary>
		public ISerializationDefinition AttributeSerializationDefinition
		{
			get { return attributeDefinition; }
			set { attributeDefinition = value; }
		}

		/// <summary>
		/// The serialization definition used specifically for processing XML CDATA sections.
		/// </summary>
		public ISerializationDefinition CDataSerializationDefinition
		{
			get { return cdataDefinition; }
			set { cdataDefinition = value; }
		}

		public XmlSerializationDefinition()
		{
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

			processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
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
			foreach (IProcessor processor in processors)
			{
				if (processor is IUnityPrimitiveXmlSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}
		}
	}
}
