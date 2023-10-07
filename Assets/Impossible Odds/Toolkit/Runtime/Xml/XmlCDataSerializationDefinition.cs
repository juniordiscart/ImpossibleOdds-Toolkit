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

	/// <summary>
	/// Serialization definition for dealing with CDATA sections in the XML document.
	/// </summary>
	public class XmlCDataSerializationDefinition : ISerializationDefinition
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

		public XmlCDataSerializationDefinition()
		{
			SupportedTypes = new HashSet<Type>()
			{
				typeof(string),
				typeof(XCData)
			};

			List<IProcessor> processors = new List<IProcessor>()
			{
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new XmlCDataProcessor(this),
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}
	}
}