using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImpossibleOdds.Json;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Serialization definition for the body of HTTP requests.
	/// </summary>
	public class HttpBodySerializationDefinition : ISerializationDefinition
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

		public HttpBodySerializationDefinition()
			:this (false)
		{ }

		public HttpBodySerializationDefinition(bool enableParallelProcessing)
		{
			parallelProcessingFeature = new ParallelProcessingFeature()
			{
				Enabled = enableParallelProcessing
			};

			ILookupSerializationConfiguration lookupConfiguration = new HttpBodyLookupConfiguration();
			ISequenceSerializationConfiguration sequenceConfiguration = new HttpBodySequenceConfiguration();

			PrimitiveProcessingMethod defaultProcessingMethod =
#if IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY
			PrimitiveProcessingMethod.Sequence;
#else
			PrimitiveProcessingMethod.Lookup;
#endif

			// Basic set of types
			SupportedTypes = new HashSet<Type>()
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

			List<IProcessor> processors = new List<IProcessor>()
			{
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<HttpEnumStringAttribute, HttpEnumAliasAttribute>()
				},
				new PrimitiveTypeProcessor(this),
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
					TypeResolutionFeature = new SequenceTypeResolutionFeature<HttpTypeIndexAttribute>(0),
					CallbackFeature = new CallBackFeature<OnHttpBodySerializingAttribute, OnHttpBodySerializedAttribute, OnHttpBodyDeserializingAttribute, OnHttpBodyDeserializedAttribute>(),
					ParallelProcessingFeature = parallelProcessingFeature,
				},
				new CustomObjectLookupProcessor(this, lookupConfiguration)
				{
					TypeResolutionFeature = new LookupTypeResolutionFeature<HttpTypeAttribute>(JsonSerializationDefinition.JsonDefaultTypeKey),
					CallbackFeature = new CallBackFeature<OnHttpBodySerializingAttribute, OnHttpBodySerializedAttribute, OnHttpBodyDeserializingAttribute, OnHttpBodyDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<HttpBodyRequiredAttribute>(),
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