using System;

namespace ImpossibleOdds.Serialization.Processors
{
    public static class ProcessorExtensions
    {
        /// <summary>
        /// Throws a SerializationException if the processor's serialization test fails.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="objectToSerialize"></param>
        /// <exception cref="SerializationException"></exception>
        public static void ThrowIfCantSerialize(this ISerializationProcessor processor, object objectToSerialize)
        {
            processor.ThrowIfNull(nameof(processor));
            if (!processor.CanSerialize(objectToSerialize))
            {
                throw new SerializationException($"The provided data cannot be serialized by this processor of type {processor.GetType().Name}.");
            }
        }

        /// <summary>
        /// Throws a SerializationException if the processor's deserialization test fails.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="targetType"></param>
        /// <param name="dataToDeserialize"></param>
        /// <exception cref="SerializationException"></exception>
        public static void ThrowIfCantDeserialize(this IDeserializationProcessor processor, Type targetType, object dataToDeserialize)
        {
            processor.ThrowIfNull(nameof(processor));
            if (!processor.CanDeserialize(targetType, dataToDeserialize))
            {
                throw new SerializationException($"The provided data cannot be deserialized by this processor of type {processor.GetType().Name}.");
            }
        }

        /// <summary>
        /// Throws a SerializationException if the processor's deserialization test fails.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="deserializationTarget"></param>
        /// <param name="dataToDeserialize"></param>
        /// <exception cref="SerializationException"></exception>
        public static void ThrowIfCantDeserializeToTarget(this IDeserializationToTargetProcessor processor, object deserializationTarget, object dataToDeserialize)
        {
            processor.ThrowIfNull(nameof(processor));
            if (!processor.CanDeserialize(deserializationTarget, dataToDeserialize))
            {
                throw new SerializationException($"The provided data cannot be deserialized by this processor of type {processor.GetType().Name}.");
            }
        }
    }
}

