using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using ImpossibleOdds.Xml.Processors;

namespace ImpossibleOdds.Xml
{
	public class XmlSerializationDefinition : IXmlSerializationDefinition
	{
		public const string XmlSchemaURL = "http://www.w3.org/2001/XMLSchema-instance";
		public const string XmlSchemaPrefix = "xsi";
		public const string XmlTypeKey = "type";

		private readonly ISerializationProcessor[] serializationProcessors;
		private readonly IDeserializationProcessor[] deserializationProcessors;
		private readonly ParallelProcessingFeature parallelProcessingFeature;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors => serializationProcessors;

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors => deserializationProcessors;

		/// <inheritdoc />
		public IFormatProvider FormatProvider { get; set; } = CultureInfo.InvariantCulture;

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes { get; }

		/// <inheritdoc />
		public ISerializationDefinition AttributeSerializationDefinition { get; set; }

		/// <inheritdoc />
		public ISerializationDefinition CDataSerializationDefinition { get; set; }

		public bool ParallelProcessingEnabled
		{
			get => parallelProcessingFeature.Enabled;
			set => parallelProcessingFeature.Enabled = value;
		}

		public XmlSerializationDefinition()
		: this(false)
		{ }

		public XmlSerializationDefinition(bool enableParallelProcessing)
		{
			parallelProcessingFeature = new ParallelProcessingFeature()
			{
				Enabled = enableParallelProcessing
			};

			XmlPrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_XML_UNITY_TYPES_AS_ATTRIBUTES
				XmlPrimitiveProcessingMethod.Attributes;
#else
				XmlPrimitiveProcessingMethod.Elements;
#endif

			SupportedTypes = new HashSet<Type>()
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
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<XmlEnumStringAttribute, XmlEnumAliasAttribute>()
				},
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
				new XmlLookupProcessor(this)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new XmlSequenceProcessor(this)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new XmlCustomObjectProcessor(this, new XmlTypeResolutionFeature())
				{
					CallbackFeature = new CallBackFeature<OnXmlSerializingAttribute, OnXmlSerializedAttribute, OnXmlDeserializingAttribute, OnXmlDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<XmlRequiredAttribute>(),
					ParallelProcessingFeature = parallelProcessingFeature
				},
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();

			AttributeSerializationDefinition = new XmlAttributeSerializationDefinition();
			CDataSerializationDefinition = new XmlCDataSerializationDefinition();
		}

		/// <summary>
		/// Update the registered processors that handle Unity primitive types to switch to a different (de)serialization style.
		/// </summary>
		/// <param name="preferredProcessingMethod">The preferred processing method.</param>
		public void UpdateUnityPrimitiveRepresentation(XmlPrimitiveProcessingMethod preferredProcessingMethod)
		{
			foreach (ISerializationProcessor processor in serializationProcessors)
			{
				if (processor is IUnityPrimitiveXmlSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}

			foreach (IDeserializationProcessor processor in deserializationProcessors)
			{
				if (processor is IUnityPrimitiveXmlSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}
		}
	}
}