using System;
using System.Collections;

namespace ImpossibleOdds.Serialization.Processors
{
	public abstract class UnityPrimitiveSwitchProcessor<TSequenceProcessor, TLookupProcessor, TPrimitive> : IUnityPrimitiveSwitchProcessor, ISerializationProcessor, IDeserializationProcessor
		where TSequenceProcessor : UnityPrimitiveSequenceProcessor<TPrimitive>
		where TLookupProcessor : UnityPrimitiveLookupProcessor<TPrimitive>
	{
		public TSequenceProcessor SequenceProcessor { get; }

		public TLookupProcessor LookupProcessor { get; }

		public PrimitiveProcessingMethod ProcessingMethod { get; set; }

		public ISerializationDefinition Definition
		{
			get
			{
				return ProcessingMethod switch
				{
					PrimitiveProcessingMethod.Sequence => SequenceProcessor.Definition,
					PrimitiveProcessingMethod.Lookup => LookupProcessor.Definition,
					_ => throw new SerializationException($"Unsupported processing method ('{ProcessingMethod.ToString()}') to retrieve the serialization definition for.")
				};
			}
		}

		public UnityPrimitiveSwitchProcessor(TSequenceProcessor sequenceProcessor, TLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		{
			sequenceProcessor.ThrowIfNull(nameof(sequenceProcessor));
			lookupProcessor.ThrowIfNull(nameof(lookupProcessor));

			SequenceProcessor = sequenceProcessor;
			LookupProcessor = lookupProcessor;
			ProcessingMethod = preferredProcessingMethod;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			return ProcessingMethod switch
			{
				PrimitiveProcessingMethod.Sequence => SequenceProcessor.Serialize(objectToSerialize),
				PrimitiveProcessingMethod.Lookup => LookupProcessor.Serialize(objectToSerialize),
				_ => throw new SerializationException($"Unsupported processing method ('{ProcessingMethod.ToString()}') to serialize the value.")
			};
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// Deserialization does not depend on the processing method, but will pick the
			// appropriate method for the provided data type.
			return dataToDeserialize switch
			{
				IDictionary _ => LookupProcessor.Deserialize(targetType, dataToDeserialize),
				IList _ => SequenceProcessor.Deserialize(targetType, dataToDeserialize),
				_ => throw new SerializationException($"Unsupported data of type {dataToDeserialize.GetType().Name} to deserialize into an instance of type {typeof(TPrimitive).Name}.")
			};
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return ProcessingMethod switch
			{
				PrimitiveProcessingMethod.Sequence => SequenceProcessor.CanSerialize(objectToSerialize),
				PrimitiveProcessingMethod.Lookup => LookupProcessor.CanSerialize(objectToSerialize),
				_ => false
			};
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				return false;
			}

			return ProcessingMethod switch
			{
				PrimitiveProcessingMethod.Sequence => SequenceProcessor.CanDeserialize(targetType, dataToDeserialize),
				PrimitiveProcessingMethod.Lookup => LookupProcessor.CanDeserialize(targetType, dataToDeserialize),
				_ => false
			};
		}
	}
}