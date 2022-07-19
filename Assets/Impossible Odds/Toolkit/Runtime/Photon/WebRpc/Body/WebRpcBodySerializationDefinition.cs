namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// A default implementation to handle (de)serialization of Photon-based webhook calls.
	/// </summary>
	public class WebRpcBodySerializationDefinition :
	IndexAndLookupDefinition<WebRpcArrayAttribute, WebRpcIndexAttribute, object[], WebRpcObjectAttribute, WebRpcFieldAttribute, Dictionary<string, object>>,
	ICallbacksSupport<OnWebRpcSerializingAttribute, OnWebRpcSerializedAttribute, OnWebRpcDeserializingAttribute, OnWebRpcDeserializedAttribute>,
	ILookupTypeResolveSupport<WebRpcTypeAttribute>,
	IEnumAliasSupport<WebRpcEnumStringAttribute, WebRpcEnumAliasAttribute>,
	IRequiredValueSupport<WebRpcRequiredAttribute>
	{
		public const string WebRpcDefaultTypeKey = "jsi:type";

		private readonly ISerializationProcessor[] serializationProcessors = null;
		private readonly IDeserializationProcessor[] deserializationProcessors = null;
		private readonly HashSet<Type> supportedTypes = null;
		private string typeResolveKey = WebRpcDefaultTypeKey;

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
			get => typeof(OnWebRpcSerializingAttribute);
		}

		/// <inheritdoc />
		public Type OnSerializedCallbackType
		{
			get => typeof(OnWebRpcSerializedAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializionCallbackType
		{
			get => typeof(OnWebRpcDeserializingAttribute);
		}

		/// <inheritdoc />
		public Type OnDeserializedCallbackType
		{
			get => typeof(OnWebRpcDeserializedAttribute);
		}

		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get => typeof(WebRpcTypeAttribute);
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
		public Type EnumAsStringAttributeType
		{
			get => typeof(WebRpcEnumStringAttribute);
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get => typeof(WebRpcEnumAliasAttribute);
		}

		/// <inheritdoc />
		public Type RequiredAttributeType
		{
			get => typeof(WebRpcRequiredAttribute);
		}

		public WebRpcBodySerializationDefinition()
		{
			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_WEBHOOK_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.SEQUENCE;
#else
			PrimitiveProcessingMethod.LOOKUP;
#endif

			// Based on the list of types supported by the Photon serializer
			// https://doc.photonengine.com/en-us/realtime/current/reference/webrpc#data_types_conversion
			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(byte[]),
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
		public override object[] CreateSequenceInstance(int capacity)
		{
			return new object[capacity];
		}

		/// <inheritdoc />
		public override Dictionary<string, object> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, object>(capacity);
		}
	}
}
