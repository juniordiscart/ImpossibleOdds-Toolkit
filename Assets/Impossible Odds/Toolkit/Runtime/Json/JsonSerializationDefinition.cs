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
		ILookupTypeResolveSupport<JsonTypeAttribute>
	{
		public const string JsonDefaultTypeKey = "jsi:type";

		private readonly ISerializationProcessor[] serializationProcessors;
		private readonly IDeserializationProcessor[] deserializationProcessors;
		private readonly HashSet<Type> supportedTypes;
		private readonly ParallelProcessingFeature parallelProcessingFeature;
		private string typeResolveKey = JsonDefaultTypeKey;

		/// <inheritdoc />
		public override IEnumerable<ISerializationProcessor> SerializationProcessors => serializationProcessors;

		/// <inheritdoc />
		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors => deserializationProcessors;

		/// <inheritdoc />
		public override HashSet<Type> SupportedTypes => supportedTypes;

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

		public bool ParallelProcessingEnabled
		{
			get => parallelProcessingFeature.Enabled;
			set => parallelProcessingFeature.Enabled = value;
		}

		public JsonSerializationDefinition()
		: this(false)
		{ }

		public JsonSerializationDefinition(bool enableParallelProcessing)
		{
			parallelProcessingFeature = new ParallelProcessingFeature()
			{
				Enabled = enableParallelProcessing
			};

			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.Sequence;
#else
			PrimitiveProcessingMethod.Lookup;
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
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<JsonEnumStringAttribute, JsonEnumAliasAttribute>()
				} ,
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
				new LookupProcessor(this)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new SequenceProcessor(this)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new CustomObjectSequenceProcessor(this)
				{
					CallbackFeature = new CallBackFeature<OnJsonSerializingAttribute, OnJsonSerializedAttribute, OnJsonDeserializingAttribute, OnJsonDeserializedAttribute>(),
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new CustomObjectLookupProcessor(this)
				{
					CallbackFeature = new CallBackFeature<OnJsonSerializingAttribute, OnJsonSerializedAttribute, OnJsonDeserializingAttribute, OnJsonDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<JsonRequiredAttribute>(),
					ParallelProcessingFeature = parallelProcessingFeature,
				},
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
			foreach (ISerializationProcessor processor in serializationProcessors)
			{
				if (processor is IUnityPrimitiveSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}

			foreach (IDeserializationProcessor processor in deserializationProcessors)
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