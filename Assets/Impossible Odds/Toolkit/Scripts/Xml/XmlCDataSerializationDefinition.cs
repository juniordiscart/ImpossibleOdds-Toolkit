namespace ImpossibleOdds.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
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

		public XmlCDataSerializationDefinition()
		{
			supportedTypes = new HashSet<Type>()
			{
				typeof(string),
				typeof(XCData)
			};

			processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new XmlCDataProcessor(this),
			};
		}
	}
}
