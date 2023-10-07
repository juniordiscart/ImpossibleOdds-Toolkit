using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml
{

	public class XmlAttributeSerializationDefinition : ISerializationDefinition
	{
		private readonly ISerializationProcessor[] serializationProcessors;
		private readonly IDeserializationProcessor[] deserializationProcessors;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors => serializationProcessors;

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors => deserializationProcessors;

		/// <inheritdoc />
		public IFormatProvider FormatProvider { get; set; } = CultureInfo.InvariantCulture;

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes { get; }

		public XmlAttributeSerializationDefinition()
		{
			SupportedTypes = new HashSet<Type>()
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
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}
	}
}