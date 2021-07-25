namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using ImpossibleOdds.Json;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Serialization definition for the body of HTTP requests.
	/// </summary>
	public class HttpBodySerializationDefinition :
	IndexAndLookupDefinition<HttpBodyArrayAttribute, HttpBodyIndexAttribute, ArrayList, HttpBodyObjectAttribute, HttpBodyFieldAttribute, Dictionary<string, object>>,
	IEnumAliasSupport<HttpEnumStringAttribute, HttpEnumAliasAttribute>,
	ILookupTypeResolveSupport<HttpTypeAttribute>,
	IRequiredValueSupport<HttpBodyRequiredAttribute>
	{
		private List<IProcessor> processors = null;
		private HashSet<Type> supportedTypes = null;
		private string typeResolveKey = JsonSerializationDefinition.JsonTypeKey;

		/// <inheritdoc />
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
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
		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
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
		public override HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get { return typeof(HttpTypeAttribute); }
		}

		/// <inheritdoc />
		object ILookupTypeResolveSupport.TypeResolveKey
		{
			get { return typeResolveKey; }
		}

		/// <inheritdoc />
		public string TypeResolveKey
		{
			get { return typeResolveKey; }
			set
			{
				value.ThrowIfNullOrEmpty(nameof(value));
				typeResolveKey = value;
			}
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get { return typeof(HttpEnumStringAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get { return typeof(HttpEnumAliasAttribute); }
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get { return typeof(HttpBodyRequiredAttribute); }
		}

		public HttpBodySerializationDefinition()
		{
			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.SEQUENCE;
#else
			PrimitiveProcessingMethod.LOOKUP;
#endif

			processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
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

		/// <summary>
		/// Update the registered processors that handle Unity primitive types to switch to a different (de)serialization style.
		/// </summary>
		/// <param name="preferredProcessingMethod">The preferred processing method.</param>
		public void UpdateUnityPrimitiveRepresentation(PrimitiveProcessingMethod preferredProcessingMethod)
		{
			foreach (IProcessor processor in processors)
			{
				if (processor is IUnityPrimitiveSwitchProcessor switchProcessor)
				{
					switchProcessor.ProcessingMethod = preferredProcessingMethod;
				}
			}
		}
	}
}
