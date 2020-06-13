using System;
using System.Collections;
using System.Collections.Generic;

namespace ImpossibleOdds.Serialization
{
	public struct LookupCollectionTypeInfo
	{
		/// <summary>
		/// Generic type of the lookup data structure.
		/// </summary>
		public readonly Type genericType;
		/// <summary>
		/// The types of the key and value in the lookup data structure.
		/// </summary>
		public readonly Type[] genericParams;
		/// <summary>
		/// The type of keys.
		/// </summary>
		public readonly Type keyType;
		/// <summary>
		/// The type of the values.
		/// </summary>
		public readonly Type valueType;
		/// <summary>
		/// Is the key type constrained?
		/// </summary>
		public readonly bool isKeyTypeConstrained;
		/// <summary>
		/// Is the value type constrained?
		/// </summary>
		public readonly bool isValueTypeConstrained;

		public LookupCollectionTypeInfo(ILookupSerializationDefinition definition)
		{
			genericType = SerializationUtilities.GetGenericType(definition.LookupBasedDataType, typeof(IDictionary<,>));
			genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
			keyType = (genericParams != null) ? genericParams[0] : typeof(object);
			valueType = (genericParams != null) ? genericParams[1] : typeof(object);
			isKeyTypeConstrained = (genericParams != null) && (keyType != typeof(object));
			isValueTypeConstrained = (genericParams != null) && (valueType != typeof(object));
		}

		public LookupCollectionTypeInfo(IDictionary instance)
		{
			instance.ThrowIfNull(nameof(instance));
			Type instanceType = instance.GetType();

			genericType = SerializationUtilities.GetGenericType(instanceType, typeof(IDictionary<,>));
			genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
			keyType = (genericParams != null) ? genericParams[0] : typeof(object);
			valueType = (genericParams != null) ? genericParams[1] : typeof(object);
			isKeyTypeConstrained = (genericParams != null) && (keyType != typeof(object));
			isValueTypeConstrained = (genericParams != null) && (valueType != typeof(object));
		}
	}
}
