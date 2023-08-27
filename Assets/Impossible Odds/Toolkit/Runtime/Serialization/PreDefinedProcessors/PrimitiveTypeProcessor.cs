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

		private static readonly List<Type> primitiveTypeOrder = new List<Type>()
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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			Type sourceType = objectToSerialize.GetType();

			// Check whether the type is supported at all by the serialization definition...
			if (definition.SupportedTypes.Contains(sourceType))
			{
				return objectToSerialize;
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

					return Convert.ChangeType(objectToSerialize, targetType, definition.FormatProvider);
				}
			}

			// TODO: check whether an implicit or explicit type conversion exists that may have been defined by the user.

			// Last resort: check whether a string-type is accepted.
			if (definition.SupportedTypes.Contains(typeof(string)))
			{
				return ((IConvertible)objectToSerialize).ToString(definition.FormatProvider);
			}

			throw new SerializationException("Cannot convert the primitive data type {0} to a data type that is supported by the serialization definition of type {1}.", sourceType.Name, definition.GetType().Name);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// For booleans, we do an additional test.
			return
				typeof(bool) == targetType ?
				ConvertToBoolean(dataToDeserialize) :
				Convert.ChangeType(dataToDeserialize, targetType, definition.FormatProvider);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				objectToSerialize.GetType().IsPrimitive;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				targetType.IsPrimitive &&
				(dataToDeserialize != null) &&
				(primitiveTypeOrder.Contains(dataToDeserialize.GetType()) || dataToDeserialize is string);
		}

		private object ConvertToBoolean(object objToParse)
		{
			if (objToParse is string strValue)
			{
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