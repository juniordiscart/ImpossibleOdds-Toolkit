namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlAttributeSerializationDefinition :
	ISerializationDefinition,
	IEnumAliasSupport<XmlEnumStringAttribute, XmlEnumAliasAttribute>
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private List<IProcessor> processors = null;
		private HashSet<Type> supportedTypes = null;

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
		public Type EnumAsStringAttributeType
		{
			get { return typeof(XmlEnumStringAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get { return typeof(XmlEnumAliasAttribute); }
		}

		public XmlAttributeSerializationDefinition()
		{
			supportedTypes = new HashSet<Type>()
			{
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(char),
				typeof(XAttribute)
			};

			processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this),
			};
		}
	}
}
