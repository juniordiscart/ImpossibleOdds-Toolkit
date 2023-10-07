using System;
using System.Collections;

namespace ImpossibleOdds.Serialization.Processors
{
	public abstract class UnityPrimitiveSequenceProcessor<T> : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }

		public ISequenceSerializationConfiguration Configuration { get; }

		protected UnityPrimitiveSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		{
			definition.ThrowIfNull(nameof(definition));
			configuration.ThrowIfNull(nameof(configuration));
			Definition = definition;
			Configuration = configuration;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			return Serialize((T)objectToSerialize);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			return Deserialize(dataToDeserialize as IList);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return objectToSerialize is T;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				(typeof(T) == targetType) &&    // Don't use AssignableFrom here, as it may trigger implicit conversions for certain types, e.g. Vector2 -> Vector3, ect.
				(dataToDeserialize is IList);
		}

		protected abstract IList Serialize(T value);
		protected abstract T Deserialize(IList sequenceData);
	}
}