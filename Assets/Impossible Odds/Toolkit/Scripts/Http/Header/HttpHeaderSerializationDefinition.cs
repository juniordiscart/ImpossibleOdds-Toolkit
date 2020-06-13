namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Serialization definition for the header of HTTP requests.
	/// </summary>
	public class HttpHeaderSerializationDefinition : LookupDefinition
	<HttpObjectAttribute, HttpHeaderFieldAttribute, Dictionary<string, string>>
	{
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get { return serializationProcessors; }
		}

		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get { return deserializationProcessors; }
		}

		public override HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		private IEnumerable<ISerializationProcessor> serializationProcessors;
		private IEnumerable<IDeserializationProcessor> deserializationProcessors;
		private HashSet<Type> supportedTypes;

		public HttpHeaderSerializationDefinition()
		{
			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this)
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>();

			// Basic set of types
			supportedTypes = new HashSet<Type>()
			{
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(double),
				typeof(bool),
				typeof(string)
			};
		}

		public override Dictionary<string, string> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, string>(capacity);
		}
	}
}
