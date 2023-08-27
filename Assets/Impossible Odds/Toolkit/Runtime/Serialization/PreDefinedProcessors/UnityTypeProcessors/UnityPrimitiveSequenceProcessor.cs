namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;

	public abstract class UnityPrimitiveSequenceProcessor<T> : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly IIndexSerializationDefinition definition = null;

		public IIndexSerializationDefinition Definition
		{
			get => definition;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => Definition;
		}

		public UnityPrimitiveSequenceProcessor(IIndexSerializationDefinition definition)
		{
			this.definition = definition;
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
			return
				(objectToSerialize != null) &&
				(objectToSerialize is T);
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
