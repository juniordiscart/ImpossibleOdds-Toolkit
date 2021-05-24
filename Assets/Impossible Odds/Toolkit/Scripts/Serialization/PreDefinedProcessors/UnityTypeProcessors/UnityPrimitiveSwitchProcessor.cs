namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	public abstract class UnityPrimitiveSwitchProcessor<TSequenceProcessor, TLookupProcessor, TPrimitive> : IUnityPrimitiveSwitchProcessor, ISerializationProcessor, IDeserializationProcessor
	where TSequenceProcessor : UnityPrimitiveSequenceProcessor<TPrimitive>
	where TLookupProcessor : UnityPrimitiveLookupProcessor<TPrimitive>
	{
		private TSequenceProcessor sequenceProcessor = null;
		private TLookupProcessor lookupProcessor = null;
		private PrimitiveProcessingMethod processingMethod;

		public TSequenceProcessor SequenceProcessor
		{
			get { return sequenceProcessor; }
		}

		public TLookupProcessor LookupProcessor
		{
			get { return lookupProcessor; }
		}

		public PrimitiveProcessingMethod ProcessingMethod
		{
			get { return processingMethod; }
			set { processingMethod = value; }
		}

		public ISerializationDefinition Definition
		{
			get
			{
				switch (processingMethod)
				{
					case PrimitiveProcessingMethod.SEQUENCE:
						return sequenceProcessor.Definition;
					case PrimitiveProcessingMethod.LOOKUP:
						return lookupProcessor.Definition;
					default:
						throw new SerializationException("Unsupported processing method ('{0}') to retrieve the serialization definition for.", processingMethod.ToString());
				}
			}
		}

		public UnityPrimitiveSwitchProcessor(TSequenceProcessor sequenceProcessor, TLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		{
			sequenceProcessor.ThrowIfNull(nameof(sequenceProcessor));
			lookupProcessor.ThrowIfNull(nameof(lookupProcessor));

			this.sequenceProcessor = sequenceProcessor;
			this.lookupProcessor = lookupProcessor;
			this.processingMethod = preferredProcessingMethod;
		}

		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			switch (processingMethod)
			{
				case PrimitiveProcessingMethod.SEQUENCE:
					return sequenceProcessor.Serialize(objectToSerialize, out serializedResult);
				case PrimitiveProcessingMethod.LOOKUP:
					return lookupProcessor.Serialize(objectToSerialize, out serializedResult);
				default:
					throw new SerializationException("Unsupported processsing method ('{0}') to serialize the value.", processingMethod.ToString());
			}
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			switch (processingMethod)
			{
				case PrimitiveProcessingMethod.SEQUENCE:
					return sequenceProcessor.Deserialize(targetType, dataToDeserialize, out deserializedResult);
				case PrimitiveProcessingMethod.LOOKUP:
					return lookupProcessor.Deserialize(targetType, dataToDeserialize, out deserializedResult);
				default:
					throw new SerializationException("Unsupported processing method ('{0}') to deserialize the value.", processingMethod.ToString());
			}
		}
	}
}
