namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;
	using ImpossibleOdds.Xml.Processors;

	public delegate string ToCDATAImpl(object obj);
	public delegate object FromCDATAImpl(string cdata);

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
		private ToCDATAImpl customObjectToCdata = null;
		private FromCDATAImpl customCdataToObject = null;
		private BinaryFormatter binaryFormatter = new BinaryFormatter();

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

		/// <inheritdoc />
		public Type XmlObjectAttributeType
		{
			get { return typeof(XmlObjectAttribute); }
		}

		/// <inheritdoc />
		public Type XmlElementAttributeType
		{
			get { return typeof(XmlElementAttribute); }
		}

		/// <inheritdoc />
		public Type XmlListElementAttributeType
		{
			get { return typeof(XmlListElementAttribute); }
		}

		/// <inheritdoc />
		public Type XmlAttributeAttributeType
		{
			get { return typeof(XmlAttributeAttribute); }
		}

		/// <inheritdoc />
		public Type XmlTypeAttributeType
		{
			get { return typeof(XmlTypeAttribute); }
		}

		/// <inheritdoc />
		public ToCDATAImpl ToCDATA
		{
			get { return customObjectToCdata != null ? customObjectToCdata : ObjectToCdata; }
			set { customObjectToCdata = value; }
		}

		/// <inheritdoc />
		public FromCDATAImpl FromCDATA
		{
			get { return customCdataToObject != null ? customCdataToObject : CdataToObject; }
			set { customCdataToObject = value; }
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

		/// <summary>
		/// Process a value to a base-64 encoded string using a BinaryFormatter.
		/// </summary>
		/// <param name="value">The value to be processed.</param>
		/// <returns>A base-64 string representation of the object.</returns>
		public string ObjectToCdata(object value)
		{
			if (value == null)
			{
				return null;
			}

			using (MemoryStream ms = new MemoryStream())
			{
				binaryFormatter.Serialize(ms, value);
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		/// <summary>
		/// Processes a base-64 encoded string to an object using a BinaryFormatter.
		/// </summary>
		/// <param name="cdata">The base-64 encoded string.</param>
		/// <returns>The object.</returns>
		public object CdataToObject(string cdata)
		{
			if (string.IsNullOrEmpty(cdata))
			{
				return null;
			}

			byte[] binData = Convert.FromBase64String(cdata);
			using (MemoryStream ms = new MemoryStream(binData))
			{
				return binaryFormatter.Deserialize(ms);
			}
		}
	}
}
