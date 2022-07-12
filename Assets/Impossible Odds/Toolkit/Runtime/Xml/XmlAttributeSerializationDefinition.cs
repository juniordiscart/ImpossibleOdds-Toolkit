namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlAttributeSerializationDefinition :
	ISerializationDefinition,
	IEnumAliasSupport<XmlEnumStringAttribute, XmlEnumAliasAttribute>
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private ISerializationProcessor[] serializationProcessors = null;
		private IDeserializationProcessor[] deserializationProcessors = null;
		private HashSet<Type> supportedTypes = null;

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
		public Type EnumAsStringAttributeType
		{
			get => typeof(XmlEnumStringAttribute);
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get => typeof(XmlEnumAliasAttribute);
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
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}
	}
}
