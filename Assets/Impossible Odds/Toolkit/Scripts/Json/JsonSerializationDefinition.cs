namespace ImpossibleOdds.Json
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// A serialization definition base with custom JSON object and array type definitions.
	/// </summary>
	/// <typeparam name="TJsonObject">Custom JSON object type.</typeparam>
	/// <typeparam name="TJsonArray">Custom JSON array type.</typeparam>
	public abstract class JsonSerializationDefinition<TJsonObject, TJsonArray> :
	IndexAndLookupDefinition<JsonArrayAttribute, JsonIndexAttribute, TJsonArray, JsonObjectAttribute, JsonFieldAttribute, TJsonObject>,
	ISerializationCallbacks<OnJsonSerializingAttribute, OnJsonSerializedAttribute, OnJsonDeserializingAttribute, OnJsonDeserializedAttribute>,
	ILookupBasedTypeResolve<JsonTypeResolveAttribute>
	where TJsonObject : IDictionary
	where TJsonArray : IList
	{
		public Type JsonObjectType
		{
			get { return typeof(TJsonObject); }
		}

		public Type JsonArrayType
		{
			get { return typeof(TJsonArray); }
		}

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

		public Type TypeResolveAttribute
		{
			get { return typeof(JsonTypeResolveAttribute); }
		}

		public Type OnSerializationCallbackType
		{
			get { return typeof(OnJsonSerializingAttribute); }
		}

		public Type OnSerializedCallbackType
		{
			get { return typeof(OnJsonSerializedAttribute); }
		}

		public Type OnDeserializionCallbackType
		{
			get { return typeof(OnJsonDeserializingAttribute); }
		}

		public Type OnDeserializedCallbackType
		{
			get { return typeof(OnJsonDeserializedAttribute); }
		}

		private IEnumerable<ISerializationProcessor> serializationProcessors;
		private IEnumerable<IDeserializationProcessor> deserializationProcessors;
		private HashSet<Type> supportedTypes;

		public JsonSerializationDefinition()
		{
			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
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

			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(char)
			};
		}
	}

	/// <summary>
	/// A serialization definition with native JSON object and array type parameters.
	/// </summary>
	public sealed class JsonDefaultSerializationDefinition : JsonSerializationDefinition<Dictionary<string, object>, List<object>>
	{
		public JsonDefaultSerializationDefinition()
		: base()
		{ }

		public override Dictionary<string, object> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, object>(capacity);
		}

		public override List<object> CreateSequenceInstance(int capacity)
		{
			return new List<object>(capacity);
		}
	}

}
