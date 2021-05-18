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
	ICallbacksSupport<OnJsonSerializingAttribute, OnJsonSerializedAttribute, OnJsonDeserializingAttribute, OnJsonDeserializedAttribute>,
	ILookupTypeResolveSupport<JsonTypeAttribute>,
	IRequiredValueSupport<JsonRequiredAttribute>,
	IEnumAliasSupport<JsonEnumStringAttribute, JsonEnumAliasAttribute>
	where TJsonObject : IDictionary
	where TJsonArray : IList
	{
		public const string JsonTypeKey = "jsi:type";

		private IEnumerable<ISerializationProcessor> serializationProcessors = null;
		private IEnumerable<IDeserializationProcessor> deserializationProcessors = null;
		private HashSet<Type> supportedTypes = null;

		/// <inheritdoc />
		public Type JsonObjectType
		{
			get { return typeof(TJsonObject); }
		}

		/// <inheritdoc />
		public Type JsonArrayType
		{
			get { return typeof(TJsonArray); }
		}

		/// <inheritdoc />
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get { return serializationProcessors; }
		}

		/// <inheritdoc />
		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get { return deserializationProcessors; }
		}

		/// <inheritdoc />
		public override HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		/// <inheritdoc />
		public Type OnSerializationCallbackType
		{
			get { return typeof(OnJsonSerializingAttribute); }
		}

		/// <inheritdoc />
		public Type OnSerializedCallbackType
		{
			get { return typeof(OnJsonSerializedAttribute); }
		}

		/// <inheritdoc />
		public Type OnDeserializionCallbackType
		{
			get { return typeof(OnJsonDeserializingAttribute); }
		}

		/// <inheritdoc />
		public Type OnDeserializedCallbackType
		{
			get { return typeof(OnJsonDeserializedAttribute); }
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get { return typeof(JsonTypeAttribute); }
		}

		/// <inheritdoc />
		public virtual object TypeResolveKey
		{
			get { return JsonTypeKey; }
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get { return typeof(JsonRequiredAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get { return typeof(JsonEnumStringAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get { return typeof(JsonEnumAliasAttribute); }
		}

		public JsonSerializationDefinition()
		{
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

			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this),
				new Vector2LookupProcessor(this),
				new Vector2IntLookupProcessor(this),
				new Vector3LookupProcessor(this),
				new Vector3IntLookupProcessor(this),
				new Vector4LookupProcessor(this),
				new QuaternionLookupProcessor(this),
				new ColorLookupProcessor(this),
				new Color32LookupProcessor(this),
				new LookupProcessor(this),
				new SequenceProcessor(this),
				new CustomObjectSequenceProcessor(this),
				new CustomObjectLookupProcessor(this),
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>();
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
