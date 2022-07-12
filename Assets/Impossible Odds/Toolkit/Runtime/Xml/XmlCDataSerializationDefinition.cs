namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;
	using ImpossibleOdds.Xml.Processors;

	/// <summary>
	/// Serialization definition for dealing with CDATA sections in the XML document.
	/// </summary>
	public class XmlCDataSerializationDefinition :
	ISerializationDefinition
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

		public XmlCDataSerializationDefinition()
		{
			supportedTypes = new HashSet<Type>()
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
