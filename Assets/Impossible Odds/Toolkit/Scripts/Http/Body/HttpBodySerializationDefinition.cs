namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Serialization definition for the body of HTTP requests.
	/// </summary>
	public class HttpBodySerializationDefinition : IndexAndLookupDefinition
	<HttpBodyArrayAttribute, HttpBodyIndexAttribute, List<object>, HttpBodyObjectAttribute, HttpBodyFieldAttribute, Dictionary<string, object>>
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

		public HttpBodySerializationDefinition()
		{
			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this),
				new Vector2Processor(this as ILookupSerializationDefinition),
				new Vector2IntProcessor(this as ILookupSerializationDefinition),
				new Vector3Processor(this as ILookupSerializationDefinition),
				new Vector3IntProcessor(this as ILookupSerializationDefinition),
				new Vector4Processor(this as ILookupSerializationDefinition),
				new QuaternionProcessor(this as ILookupSerializationDefinition),
				new ColorProcessor(this as ILookupSerializationDefinition),
				new Color32Processor(this as ILookupSerializationDefinition),
				new LookupProcessor(this),
				new SequenceProcessor(this),
				new CustomObjectSequenceProcessor(this),
				new CustomObjectLookupProcessor(this),
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
				typeof(float),
				typeof(double),
				typeof(bool),
				typeof(string)
			};
		}

		public override List<object> CreateSequenceInstance(int capacity)
		{
			return new List<object>(capacity);
		}

		public override Dictionary<string, object> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, object>(capacity);
		}
	}
}
