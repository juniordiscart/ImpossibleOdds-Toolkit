using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace Tests
{
	public class TestSerializationDefinition : ISerializationDefinition
	{
		public IEnumerable<ISerializationProcessor> SerializationProcessors { get; set; }

		public IEnumerable<IDeserializationProcessor> DeserializationProcessors { get; set; }

		public HashSet<Type> SupportedTypes { get; set; }

		public IFormatProvider FormatProvider { get; } = CultureInfo.InvariantCulture;

		public TestSerializationDefinition()
		{ }

		public TestSerializationDefinition(HashSet<Type> supportedTypes)
		{
			SupportedTypes = supportedTypes;
		}

		public void SetProcessors(IEnumerable<IProcessor> processors)
		{
			if (processors != null)
			{
				SerializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
				DeserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
			}
		}
	}
}