using System;
using System.Collections.Generic;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor for primitive types, i.e. int, bool, float, ...
	/// If a primitive type is not supported by the serialization definition, it will
	/// attempt to promote it to a type with a larger value range that is supported.
	/// Note: this may not be suitable with binary serializers that will attempt to directly
	/// match the the type to the value's binary representation. The type is recommended to
	/// be directly supported by the serialization definition instead.
	/// </summary>
	public class PrimitiveTypeProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private const string TrueStringAlternative = "1";
		private const string FalseStringAlternative = "0";

		private static readonly List<Type> PrimitiveTypeOrder = new List<Type>()
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

		public ISerializationDefinition Definition { get; }

		public PrimitiveTypeProcessor(ISerializationDefinition definition)
		{
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			Type sourceType = objectToSerialize.GetType();

			// Check whether the type is supported at all by the serialization definition...
			if (Definition.SupportedTypes.Contains(sourceType))
			{
				return objectToSerialize;
			}

			// Attempt to convert the source value to a supported primitive type that is higher in the chain.
			if (PrimitiveTypeOrder.TryFindIndex(sourceType, out int index))
			{
				for (int i = index; i < PrimitiveTypeOrder.Count; ++i)
				{
					Type targetType = PrimitiveTypeOrder[i];
					if (!Definition.SupportedTypes.Contains(targetType))
					{
						continue;
					}

					return Convert.ChangeType(objectToSerialize, targetType, Definition.FormatProvider);
				}
			}

			throw new SerializationException($"Cannot convert the primitive data type {sourceType.Name} to a data type that is supported by the serialization definition of type {Definition.GetType().Name}.");
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// For booleans, we do an additional test.
			return
				typeof(bool) == targetType ?
					ConvertToBoolean(dataToDeserialize) :
					Convert.ChangeType(dataToDeserialize, targetType, Definition.FormatProvider);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return (objectToSerialize != null) && objectToSerialize.GetType().IsPrimitive;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				targetType.IsPrimitive &&
				(dataToDeserialize != null) &&
				(PrimitiveTypeOrder.Contains(dataToDeserialize.GetType()) || dataToDeserialize is string);
		}

		private object ConvertToBoolean(object objToParse)
		{
			if (objToParse is string strValue)
			{
				return strValue switch
				{
					TrueStringAlternative => true,
					FalseStringAlternative => false,
					_ => Convert.ChangeType(objToParse, typeof(bool), Definition.FormatProvider)
				};
			}

			return Convert.ChangeType(objToParse, typeof(bool), Definition.FormatProvider);
		}
	}
}