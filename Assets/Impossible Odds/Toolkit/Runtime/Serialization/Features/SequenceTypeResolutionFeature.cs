using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Generic index-based type resolution feature object that provides the type of the attribute used
    /// to store type information in the data structure.
    /// </summary>
    /// <typeparam name="TTypeResolution">The type of the attribute to be defined atop of a class, struct or interface.</typeparam>
    public class SequenceTypeResolutionFeature<TTypeResolution> : ISequenceTypeResolutionFeature
        where TTypeResolution : ISequenceTypeResolutionParameter
    {
        public SequenceTypeResolutionFeature(int typeResolutionIndex)
        {
            TypeResolutionIndex = typeResolutionIndex;
        }

        /// <inheritdoc />
        public Type TypeResolutionAttribute => typeof(TTypeResolution);

        /// <inheritdoc />
        public int TypeResolutionIndex { get; }

        /// <inheritdoc />
        public Type FindTypeInSourceData(Type targetType, IList sourceData, ISerializationDefinition definition)
        {
            targetType.ThrowIfNull(nameof(targetType));
            sourceData.ThrowIfNull(nameof(sourceData));
            definition.ThrowIfNull(nameof(definition));

            SequenceCollectionTypeInfo sourceCollectionInfo = SerializationUtilities.GetCollectionTypeInfo(sourceData);
            ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(targetType).GetTypeResolveParameters(TypeResolutionAttribute);

            Type resolvedType = targetType;
            foreach (ISequenceTypeResolutionParameter typeResolutionParameter in typeResolutionParameters.Where(trp => trp is ISequenceTypeResolutionParameter).Cast<ISequenceTypeResolutionParameter>())
            {
                // If we're considering the same type again, or the type would be a step backwards, then don't bother checking further.
                if ((typeResolutionParameter.Target == resolvedType) ||
                    resolvedType.IsSubclassOf(typeResolutionParameter.Target) ||
                    !targetType.IsAssignableFrom(typeResolutionParameter.Target))
                {
                    continue;
                }

                int index = (typeResolutionParameter.IndexOverride < 0) ? TypeResolutionIndex : typeResolutionParameter.IndexOverride;

                // Check that the index is valid in the source data.
                if ((sourceData.Count >= index) || (sourceData[index] == null))
                {
                    continue;
                }

                // Process the value on the attribute and try to match it with the entry in the source data.
                object processedValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
                processedValue = sourceCollectionInfo.PostProcessValue(Serializer.Serialize(processedValue, definition));

                if (!Equals(sourceData[index], processedValue))
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
        public void InsertTypeInData(Type sourceType, IList serializedData, ISerializationDefinition definition)
        {
            sourceType.ThrowIfNull(nameof(sourceType));
            serializedData.ThrowIfNull(nameof(serializedData));
            definition.ThrowIfNull(nameof(definition));

            SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(serializedData);
            ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(TypeResolutionAttribute);
            Dictionary<int, Type> insertedTypeInfo = new Dictionary<int, Type>();

            foreach (ISequenceTypeResolutionParameter typeResolutionParameter in typeResolutionParameters.Where(trp => trp is ISequenceTypeResolutionParameter).Cast<ISequenceTypeResolutionParameter>())
            {
                if (!typeResolutionParameter.Target.IsAssignableFrom(sourceType))
                {
                    continue;
                }

                int index = (typeResolutionParameter.IndexOverride < 0) ? TypeResolutionIndex : typeResolutionParameter.IndexOverride;

                // If the information was already present before this function added the type information, then the type information
                // is assumed to be part of the object's serialized data already. If it was added by this function, then it should check
                // that the information from the most basic type available is used.
                if ((!insertedTypeInfo.ContainsKey(index) && (serializedData[index] != null)) ||
                    (insertedTypeInfo.ContainsKey(index) && insertedTypeInfo[index].IsAssignableFrom(typeResolutionParameter.Target)))
                {
                    continue;
                }

                object typeValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
                typeValue = Serializer.Serialize(typeValue, definition);
                SerializationUtilities.InsertInSequence(serializedData, collectionInfo, index, typeValue);
                insertedTypeInfo[index] = typeResolutionParameter.Target;
            }
        }
    }
}