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
	public class JsonSerializationDefinition :
		IndexAndLookupDefinition<JsonArrayAttribute, JsonIndexAttribute, ArrayList, JsonObjectAttribute, JsonFieldAttribute, Dictionary<string, object>>,
		ICallbacksSupport<OnJsonSerializingAttribute, OnJsonSerializedAttribute, OnJsonDeserializingAttribute, OnJsonDeserializedAttribute>,
		ILookupTypeResolveSupport<JsonTypeAttribute>,
		IRequiredValueSupport<JsonRequiredAttribute>,
		IEnumAliasSupport<JsonEnumStringAttribute, JsonEnumAliasAttribute>,
		IParallelProcessingSupport
	{
		public const string JsonDefaultTypeKey = "jsi:type";

		private readonly ISerializationProcessor[] serializationProcessors = null;
		private readonly IDeserializationProcessor[] deserializationProcessors = null;
		private readonly HashSet<Type> supportedTypes = null;
		private string typeResolveKey = JsonDefaultTypeKey;
		private bool processInParallel = false;

		/// <inheritdoc />
		public Type JsonObjectType
		{
			get => typeof(Dictionary<string, object>);
		}

		/// <inheritdoc />
		public Type JsonArrayType
		{
			get => typeof(ArrayList);
		}

		/// <inheritdoc />
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get => serializationProcessors;
		}

		/// <inheritdoc />
		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get => deserializationProcessors;
		}

		/// <inheritdoc />
		public override HashSet<Type> SupportedTypes
		{
			get => supportedTypes;
		}

		/// <inheritdoc />
		public Type OnSerializationCallbackType
		{
			get => typeof(OnJsonSerializingAttribute);
		}

		/// <inheritdoc />
		public Type OnSerializedCallbackType
		{
			get => typeof(OnJsonSerializedAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializionCallbackType
		{
			get => typeof(OnJsonDeserializingAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializedCallbackType
		{
			get => typeof(OnJsonDeserializedAttribute);
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get => typeof(JsonTypeAttribute);
		}

		/// <inheritdoc />
		object ILookupTypeResolveSupport.TypeResolveKey
		{
			get => typeResolveKey;
		}

		/// <inheritdoc />
		public string TypeResolveKey
		{
			get => typeResolveKey;
			set
			{
				value.ThrowIfNullOrEmpty(nameof(value));
				typeResolveKey = value;
			}
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get => typeof(JsonRequiredAttribute);
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get => typeof(JsonEnumStringAttribute);
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get => typeof(JsonEnumAliasAttribute);
		}

		/// <inheritdoc />
		bool IParallelProcessingSupport.Enabled
		{
			get => ParallelProcessingEnabled;
		}

		public bool ParallelProcessingEnabled
		{
			get => processInParallel;
			set => processInParallel = value;
		}

		public JsonSerializationDefinition()
		: this(false)
		{ }

		public JsonSerializationDefinition(bool enableParallelProcessing)
		{
			this.processInParallel = enableParallelProcessing;

			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.SEQUENCE;
#else
			PrimitiveProcessingMethod.LOOKUP;
#endif
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
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new VersionProcessor(this),
				new GuidProcessor(this),
				new StringProcessor(this),
				new Vector2Processor(this, this, defaultProcessingMethod),
				new Vector2IntProcessor(this, this, defaultProcessingMethod),
				new Vector3Processor(this, this, defaultProcessingMethod),
				new Vector3IntProcessor(this, this, defaultProcessingMethod),
				new Vector4Processor(this, this, defaultProcessingMethod),
				new QuaternionProcessor(this, this, defaultProcessingMethod),
				new ColorProcessor(this, this, defaultProcessingMethod),
				new Color32Processor(this, this, defaultProcessingMethod),
				new LookupProcessor(this),
				new SequenceProcessor(this),
				new CustomObjectSequenceProcessor(this),
				new CustomObjectLookupProcessor(this),
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}

		/// <summary>
		/// Update the registered processors that handle Unity primitive types to switch to a different (de)serialization style.
		/// </summary>
		/// <param name="preferredProcessingMethod">The preferred processing method.</param>
		public void UpdateUnityPrimitiveRepresentation(PrimitiveProcessingMethod preferredProcessingMethod)
		{
			foreach (IProcessor processor in serializationProcessors)
			{
				if (processor is IUnityPrimitiveSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}

			foreach (IProcessor processor in deserializationProcessors)
			{
				if (processor is IUnityPrimitiveSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}
		}

		/// <inheritdoc />
		public override ArrayList CreateSequenceInstance(int capacity)
		{
			return new ArrayList(capacity);
		}

		/// <inheritdoc />
		public override Dictionary<string, object> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, object>(capacity);
		}
	}
}
