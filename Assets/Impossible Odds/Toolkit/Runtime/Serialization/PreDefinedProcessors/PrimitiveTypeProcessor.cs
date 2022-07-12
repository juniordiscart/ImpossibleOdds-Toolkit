namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A (de)serialization processor for primitive types, i.e. int, bool, float, ...
	/// </summary>
	public class PrimitiveTypeProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private const string TrueStringAlternative = "1";
		private const string FalseStringAlternative = "0";

		private readonly static List<Type> primitiveTypeOrder = new List<Type>()
		{
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(char),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(double)
		};

		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public PrimitiveTypeProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempt to serialize the object to a single instance of a primitive type as defined by the serialization definition.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !objectToSerialize.GetType().IsPrimitive)
			{
				serializedResult = null;
				return false;
			}

			Type sourceType = objectToSerialize.GetType();

			// Check whether the type is supported after all...
			if (definition.SupportedTypes.Contains(sourceType))
			{
				serializedResult = objectToSerialize;
				return true;
			}

			// Attempt to convert the source value to a supported primitive type that is higher in the chain.
			if (primitiveTypeOrder.Contains(sourceType))
			{
				for (int i = primitiveTypeOrder.IndexOf(sourceType); i < primitiveTypeOrder.Count; ++i)
				{
					Type targetType = primitiveTypeOrder[i];
					if (!definition.SupportedTypes.Contains(targetType))
					{
						continue;
					}

					serializedResult = Convert.ChangeType(objectToSerialize, targetType, definition.FormatProvider);
					return true;
				}
			}

			// TODO: check whether an implicit or explicit type conversion exists that may have been defined by the user.

			// Last resort: check whether a string-type is accepted.
			if (definition.SupportedTypes.Contains(typeof(string)))
			{
				serializedResult = ((IConvertible)objectToSerialize).ToString(definition.FormatProvider);
				return true;
			}

			throw new SerializationException("Cannot convert the primitive data type {0} to a data type that is supported by the serialization definition of type {1}.", sourceType.Name, definition.GetType().Name);
		}

		/// <summary>
		/// Attempts to deserialize the object to a primitive type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (!targetType.IsPrimitive || (dataToDeserialize == null))
			{
				deserializedResult = null;
				return false;
			}

			// For booleans, we do an additional test.
			if (typeof(bool) == targetType)
			{
				deserializedResult = ConvertToBoolean(dataToDeserialize);
			}
			else
			{
				deserializedResult = Convert.ChangeType(dataToDeserialize, targetType, definition.FormatProvider);
			}
			return true;
		}

		private object ConvertToBoolean(object objToParse)
		{
			if (typeof(string).IsAssignableFrom(objToParse.GetType()))
			{
				string strValue = (string)objToParse;
				switch (strValue)
				{
					case TrueStringAlternative:
						return true;
					case FalseStringAlternative:
						return false;
					default:
						return Convert.ChangeType(objToParse, typeof(bool), definition.FormatProvider);
				}
			}

			return Convert.ChangeType(objToParse, typeof(bool), definition.FormatProvider);
		}
	}
}
