namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Contains type information about the sequence data structure, i.e. whether it restricts its values to be type restricted, and whether it is an array or not.
	/// </summary>
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

		public SequenceCollectionTypeInfo(IList instance)
		: this(instance.GetType())
		{ }

		public SequenceCollectionTypeInfo(Type collectionType)
		{
			collectionType.ThrowIfNull(nameof(collectionType));
			if (!typeof(IList).IsAssignableFrom(collectionType))
			{
				throw new ArgumentException(string.Format("{0} is not a {1}.", collectionType.Name, typeof(IList).Name));
			}

			genericType = SerializationUtilities.GetGenericType(collectionType, typeof(IList<>));
			elementType = (genericType != null) ? genericType.GetGenericArguments()[0] : typeof(object);
			isTypeConstrained = elementType != typeof(object);
			isArray = collectionType.IsArray;
		}
	}
}
