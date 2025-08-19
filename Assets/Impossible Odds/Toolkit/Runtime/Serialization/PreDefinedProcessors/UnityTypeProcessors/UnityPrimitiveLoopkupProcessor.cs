using System;
using System.Collections;

namespace ImpossibleOdds.Serialization.Processors
{
	public abstract class UnityPrimitiveLookupProcessor<TPrimitive> : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }
		public ILookupSerializationConfiguration Configuration { get; }
		
		/// <summary>
		/// The required keys for the collection, as defined by the type that's being handled.
		/// </summary>
		public abstract string[] Keys { get; }

		protected UnityPrimitiveLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		{
			definition.ThrowIfNull(nameof(definition));
			configuration.ThrowIfNull(nameof(configuration));
			Definition = definition;
			Configuration = configuration;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			return Serialize((TPrimitive)objectToSerialize);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);
			return Deserialize((IDictionary)dataToDeserialize);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return objectToSerialize is TPrimitive;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			
			return
				(typeof(TPrimitive) == targetType) &&	// Don't use AssignableFrom here, as it may trigger implicit conversions for certain types, e.g. Vector2 -> Vector3, ect.
				(dataToDeserialize is IDictionary dictionaryToDeserialize) &&
				Array.TrueForAll(Keys, key => dictionaryToDeserialize.Contains(key));
		}

		protected abstract IDictionary Serialize(TPrimitive value);
		protected abstract TPrimitive Deserialize(IDictionary lookupData);
	}
}