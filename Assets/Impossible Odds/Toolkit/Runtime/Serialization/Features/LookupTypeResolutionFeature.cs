using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Generic lookup-based type resolution feature object that provides the type of the attribute used
	/// to store type information in the data structure.
	/// </summary>
	/// <typeparam name="TTypeResolution">The type of the attribute to be defined atop of a class, struct or interface.</typeparam>
	public class LookupTypeResolutionFeature<TTypeResolution> : ILookupTypeResolutionFeature
		where TTypeResolution : ITypeResolutionParameter
	{
		public LookupTypeResolutionFeature(object typeResolutionKey)
		{
			TypeResolutionKey = typeResolutionKey;
		}

		/// <inheritdoc />
		public Type TypeResolutionAttribute => typeof(TTypeResolution);

		/// <inheritdoc />
		public object TypeResolutionKey { get; }

		/// <inheritdoc />
		public Type FindTypeInSourceData(Type targetType, IDictionary sourceData, ISerializationDefinition definition)
		{
			targetType.ThrowIfNull(nameof(targetType));
			sourceData.ThrowIfNull(nameof(sourceData));
			definition.ThrowIfNull(nameof(definition));

			LookupCollectionTypeInfo sourceCollectionInfo = SerializationUtilities.GetCollectionTypeInfo(sourceData);
			ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(targetType).GetTypeResolveParameters(TypeResolutionAttribute);

			Type resolvedType = targetType;

			foreach (ILookupTypeResolutionParameter typeResolutionParameter in typeResolutionParameters.Where(trp => trp is ILookupTypeResolutionParameter).Cast<ILookupTypeResolutionParameter>())
			{
				// If we're considering the same type again, or the type would be a step backwards, then don't bother checking further.
				if ((typeResolutionParameter.Target == resolvedType) ||
				    resolvedType.IsSubclassOf(typeResolutionParameter.Target) ||
				    !targetType.IsAssignableFrom(typeResolutionParameter.Target))
				{
					continue;
				}

				// Process the key
				object processedKey = typeResolutionParameter.KeyOverride ?? TypeResolutionKey;
				processedKey = sourceCollectionInfo.PostProcessKey(Serializer.Serialize(processedKey, definition));

				// Check that the key exists in the source data.
				if (!sourceData.Contains(processedKey) || (sourceData[processedKey] == null))
				{
					continue;
				}

				// Process the value on the attribute and try to match it with the entry in the source data.
				object processedValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
				processedValue = sourceCollectionInfo.PostProcessValue(Serializer.Serialize(processedValue, definition));

				if (!Equals(sourceData[processedKey], processedValue))
				{
					continue;
				}

				resolvedType = typeResolutionParameter.Target;
			}

			// If a different type was found other than the original, then attempt to search further.
			if (targetType != resolvedType)
			{
				resolvedType = FindTypeInSourceData(resolvedType, sourceData, definition);
			}

			return resolvedType;
		}

		/// <inheritdoc />
		public void InsertTypeInData(Type sourceType, IDictionary serializedData, ISerializationDefinition definition)
		{
			sourceType.ThrowIfNull(nameof(sourceType));
			serializedData.ThrowIfNull(nameof(serializedData));
			definition.ThrowIfNull(nameof(definition));

			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(serializedData);
			ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(TypeResolutionAttribute);
			Dictionary<object, Type> insertedTypeInfo = new Dictionary<object, Type>();

			foreach (ILookupTypeResolutionParameter typeResolutionParameter in typeResolutionParameters.Where(trp => trp is ILookupTypeResolutionParameter).Cast<ILookupTypeResolutionParameter>())
			{
				if (!typeResolutionParameter.Target.IsAssignableFrom(sourceType))
				{
					continue;
				}

				object typeKey = typeResolutionParameter.KeyOverride ?? TypeResolutionKey;
				typeKey = Serializer.Serialize(typeKey, definition);

				// If the information was already present before this function added the type information, then the type information
				// is assumed to be part of the object's serialized data already. If it was added by this function, then it should check
				// that the information from the most basic available type is used.
				if ((!insertedTypeInfo.ContainsKey(typeKey) && serializedData.Contains(typeKey)) ||
				    insertedTypeInfo.ContainsKey(typeKey) && insertedTypeInfo[typeKey].IsAssignableFrom(typeResolutionParameter.Target))
				{
					continue;
				}

				object typeValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
				typeValue = Serializer.Serialize(typeValue, definition);
				SerializationUtilities.InsertInLookup(serializedData, collectionInfo, typeKey, typeValue);
				insertedTypeInfo[typeKey] = typeResolutionParameter.Target;
			}
		}
	}
}