namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	// using System.Collections.Generic;
	using System.Collections.Concurrent;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private static readonly string[] EnumFlagSplit = new string[] { ", " };
		private static ConcurrentDictionary<Type, EnumSerializationReflectionMap> enumTypeCache = new ConcurrentDictionary<Type, EnumSerializationReflectionMap>();

		private readonly ISerializationDefinition definition = null;
		private readonly IEnumAliasSupport enumAliasSupport = null;

		public bool SupportsEnumAlias
		{
			get => enumAliasSupport != null;
		}

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public EnumProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
			this.enumAliasSupport = (definition is IEnumAliasSupport) ? (definition as IEnumAliasSupport) : null;
		}

		/// <summary>
		/// Attempts to serialize the enum value to a supported type defined by the serialization definition. If an alias is defined, then the alias is serialized.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if (objectToSerialize == null)
			{
				serializedResult = null;
				return false;
			}

			Type sourceType = objectToSerialize.GetType();
			if (!sourceType.IsEnum)
			{
				serializedResult = null;
				return false;
			}

			Enum enumValue = objectToSerialize as Enum;
			EnumSerializationReflectionMap enumTypeCache = GetEnumTypeCache(sourceType);

			// If the definition supports enum aliases and is preferred to be sent serialized as string.
			if (SupportsEnumAlias && enumTypeCache.PrefersStringBasedRespresentation(enumAliasSupport))
			{
				serializedResult = enumTypeCache.GetStringRespresentationFor(enumValue, enumAliasSupport);
				return true;
			}

			// By this point, the enum should be serialized using its underlying type.
			serializedResult = Serializer.Serialize(Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType)), Definition);
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the data to an enum value of the target type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// If we're not dealing with an enum target, then don't bother.
			if (!targetType.IsEnum || (dataToDeserialize == null))
			{
				deserializedResult = null;
				return false;
			}

			// Check if the value is a string.
			if (dataToDeserialize is string dataStr)
			{
				// If aliasing is supported, try to process individual values in the enum string
				if (SupportsEnumAlias)
				{
					EnumSerializationReflectionMap enumTypeCache = GetEnumTypeCache(targetType);
					deserializedResult = enumTypeCache.GetEnumValueFor(dataStr, enumAliasSupport);
					return true;
				}
				else
				{
					// If the string represents a numeric value, try to match to an internal value of the enum.
					// This may throw an exception if the value is not a numeric one. We let that fail silently and try a different approach.
					try
					{
						deserializedResult = Enum.ToObject(targetType, Convert.ChangeType(dataStr, Enum.GetUnderlyingType(targetType)));
						return true;
					}
					catch
					{ }
				}

				// Final resort, just try to parse it.
				deserializedResult = Enum.Parse(targetType, dataStr);
				return true;
			}
			else if (dataToDeserialize.GetType().IsValueType)
			{
				Type underlyingEnumType = Enum.GetUnderlyingType(targetType);
				deserializedResult = Enum.ToObject(targetType, Convert.ChangeType(dataToDeserialize, underlyingEnumType));
				return true;
			}
			else
			{
				throw new SerializationException("The provided data to deserialize to enum of type {0} is not a {1} or a value type.", dataToDeserialize.GetType().Name, typeof(string).Name);
			}
		}

		private EnumSerializationReflectionMap GetEnumTypeCache(Type enumType)
		{
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new ArgumentException(string.Format("The type {0} is not an enum.", enumType.Name));
			}

			return enumTypeCache.GetOrAdd(enumType, (type) => new EnumSerializationReflectionMap(enumType));
		}
	}
}
