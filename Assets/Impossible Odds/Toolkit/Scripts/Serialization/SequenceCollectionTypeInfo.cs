namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public struct SequenceCollectionTypeInfo
	{
		/// <summary>
		/// Generic type info of the sequence.
		/// </summary>
		public readonly Type genericType;
		/// <summary>
		/// The type of the elements in the sequence.
		/// </summary>
		public readonly Type elementType;
		/// <summary>
		/// Is the element type constrained?
		/// </summary>
		public readonly bool isTypeConstrained;
		/// <summary>
		/// Is the sequence an array?
		/// </summary>
		public readonly bool isArray;

		public SequenceCollectionTypeInfo(IIndexSerializationDefinition definition)
		{
			genericType = SerializationUtilities.GetGenericType(definition.IndexBasedDataType, typeof(IList<>));
			elementType = (genericType != null) ? genericType.GetGenericArguments()[0] : typeof(object);
			isTypeConstrained = elementType != typeof(object);
			isArray = definition.IndexBasedDataType.IsArray;
		}

		public SequenceCollectionTypeInfo(IList instance)
		{
			instance.ThrowIfNull(nameof(instance));
			Type instanceType = instance.GetType();

			genericType = SerializationUtilities.GetGenericType(instanceType, typeof(IList<>));
			elementType = (genericType != null) ? genericType.GetGenericArguments()[0] : typeof(object);
			isTypeConstrained = elementType != typeof(object);
			isArray = instanceType.IsArray;
		}
	}
}
