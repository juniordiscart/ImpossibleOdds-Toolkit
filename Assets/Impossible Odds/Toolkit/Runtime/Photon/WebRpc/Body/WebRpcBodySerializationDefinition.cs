using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImpossibleOdds.Json;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Photon.WebRpc
{
	/// <summary>
	/// A default implementation to handle (de)serialization of Photon-based webhook calls.
	/// </summary>
	public class WebRpcBodySerializationDefinition : ISerializationDefinition
	{
		private readonly ISerializationProcessor[] serializationProcessors;
		private readonly IDeserializationProcessor[] deserializationProcessors;
		private readonly ParallelProcessingFeature parallelProcessingFeature;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors => serializationProcessors;

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors => deserializationProcessors;

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes { get; }

		/// <inheritdoc />
		public IFormatProvider FormatProvider { get; set; } = CultureInfo.InvariantCulture;

		/// <summary>
		/// Enable or disable the parallel processing of data across the data processors.
		/// </summary>
		public bool ParallelProcessingEnabled
		{
			get => parallelProcessingFeature.Enabled;
			set => parallelProcessingFeature.Enabled = value;
		}

		public WebRpcBodySerializationDefinition()
			:this (false)
		{ }

		public WebRpcBodySerializationDefinition(bool enableParallelProcessing)
		{
			parallelProcessingFeature = new ParallelProcessingFeature()
			{
				Enabled = enableParallelProcessing
			};

			ILookupSerializationConfiguration lookupConfiguration = new WebRpcBodyLookupConfiguration();
			ISequenceSerializationConfiguration sequenceConfiguration = new WebRpcBodySequenceConfiguration();

			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_WEBHOOK_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.Sequence;
#else
			PrimitiveProcessingMethod.Lookup;
#endif

			// Based on the list of types supported by the Photon serializer
			// https://doc.photonengine.com/en-us/realtime/current/reference/webrpc#data_types_conversion
			SupportedTypes = new HashSet<Type>()
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
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<WebRpcEnumStringAttribute, WebRpcEnumAliasAttribute>()
				},
				new PrimitiveTypeProcessor(this),
				new DecimalProcessor(this),
				new DateTimeProcessor(this),
				new VersionProcessor(this),
				new GuidProcessor(this),
				new StringProcessor(this),
				new Vector2Processor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new Vector2IntProcessor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new Vector3Processor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new Vector3IntProcessor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new Vector4Processor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new QuaternionProcessor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new ColorProcessor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new Color32Processor(this, sequenceConfiguration, lookupConfiguration, defaultProcessingMethod),
				new LookupProcessor(this, lookupConfiguration)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new SequenceProcessor(this, sequenceConfiguration)
				{
					ParallelProcessingFeature = parallelProcessingFeature
				},
				new CustomObjectSequenceProcessor(this, sequenceConfiguration)
				{
					TypeResolutionFeature = new SequenceTypeResolutionFeature<WebRpcTypeIndexAttribute>(0),
					CallbackFeature = new CallBackFeature<OnWebRpcSerializingAttribute, OnWebRpcSerializedAttribute, OnWebRpcDeserializingAttribute, OnWebRpcDeserializedAttribute>(),
					ParallelProcessingFeature = parallelProcessingFeature,
				},
				new CustomObjectLookupProcessor(this, lookupConfiguration)
				{
					TypeResolutionFeature = new LookupTypeResolutionFeature<WebRpcTypeAttribute>(JsonSerializationDefinition.JsonDefaultTypeKey),
					CallbackFeature = new CallBackFeature<OnWebRpcSerializingAttribute, OnWebRpcSerializedAttribute, OnWebRpcDeserializingAttribute, OnWebRpcDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<WebRpcRequiredAttribute>(),
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
	}
}