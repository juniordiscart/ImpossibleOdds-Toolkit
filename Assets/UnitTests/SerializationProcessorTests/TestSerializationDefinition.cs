namespace Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;
	using UnityEngine;

	public class TestSerializationDefinition : ISerializationDefinition
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private HashSet<Type> supportedTypes = null;

		public IEnumerable<ISerializationProcessor> SerializationProcessors => throw new NotImplementedException(nameof(SerializationProcessors));

		public IEnumerable<IDeserializationProcessor> DeserializationProcessors => throw new NotImplementedException(nameof(DeserializationProcessors));

		public HashSet<Type> SupportedTypes
		{
			get => supportedTypes;
			set => supportedTypes = value;
		}

		public IFormatProvider FormatProvider => formatProvider;

		public TestSerializationDefinition()
		{ }

		public TestSerializationDefinition(HashSet<Type> supportedTypes)
		{
			this.supportedTypes = supportedTypes;
		}
	}
}