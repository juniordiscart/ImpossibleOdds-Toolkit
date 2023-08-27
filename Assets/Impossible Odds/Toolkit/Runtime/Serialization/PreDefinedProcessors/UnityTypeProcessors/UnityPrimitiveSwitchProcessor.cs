namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;

	public abstract class UnityPrimitiveSwitchProcessor<TSequenceProcessor, TLookupProcessor, TPrimitive> : IUnityPrimitiveSwitchProcessor, ISerializationProcessor, IDeserializationProcessor
	where TSequenceProcessor : UnityPrimitiveSequenceProcessor<TPrimitive>
	where TLookupProcessor : UnityPrimitiveLookupProcessor<TPrimitive>
	{
		private readonly TSequenceProcessor sequenceProcessor = null;
		private readonly TLookupProcessor lookupProcessor = null;
		private PrimitiveProcessingMethod processingMethod;

		public TSequenceProcessor SequenceProcessor
		{
			get => sequenceProcessor;
		}

		public TLookupProcessor LookupProcessor
		{
			get => lookupProcessor;
		}

		public PrimitiveProcessingMethod ProcessingMethod
		{
			get => processingMethod;
			set => processingMethod = value;
		}

		public ISerializationDefinition Definition
		{
			get
			{
				switch (processingMethod)
				{
					case PrimitiveProcessingMethod.Sequence:
						return sequenceProcessor.Definition;
					case PrimitiveProcessingMethod.Lookup:
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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			switch (processingMethod)
			{
				case PrimitiveProcessingMethod.Sequence:
					return sequenceProcessor.Serialize(objectToSerialize);
				case PrimitiveProcessingMethod.Lookup:
					return lookupProcessor.Serialize(objectToSerialize);
				default:
					throw new SerializationException("Unsupported processing method ('{0}') to serialize the value.", processingMethod.ToString());
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// Deserialization does not depend on the processing method, but will pick the
			// appropriate method for the provided data type.
			if (dataToDeserialize is IDictionary)
			{
				return lookupProcessor.Deserialize(targetType, dataToDeserialize);
			}
			else if (dataToDeserialize is IList)
			{
				return sequenceProcessor.Deserialize(targetType, dataToDeserialize);
			}

			throw new SerializationException("Unsupported data of type {0} to deserialize into an instance of type {1}.", dataToDeserialize.GetType().Name, typeof(TPrimitive).Name);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			switch(processingMethod)
			{
				case PrimitiveProcessingMethod.Sequence:
					return SequenceProcessor.CanSerialize(objectToSerialize);
				case PrimitiveProcessingMethod.Lookup:
					return LookupProcessor.CanSerialize(objectToSerialize);
				default:
					return false;
			}
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				return false;
			}

			switch(processingMethod)
			{
				case PrimitiveProcessingMethod.Sequence:
					return SequenceProcessor.CanDeserialize(targetType, dataToDeserialize);
				case PrimitiveProcessingMethod.Lookup:
					return LookupProcessor.CanDeserialize(targetType, dataToDeserialize);
				default:
					return false;
			}

		}
	}
}
