namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Abstract class for processors that work on collection-like data structures, i.e. lists, arrays, dictionaries, ...
	/// </summary>
	public abstract class AbstractCollectionProcessor : AbstractProcessor
	{
		public bool StrictTypeChecking
		{
			get { return strictTypeChecking; }
			set { strictTypeChecking = value; }
		}

		private bool strictTypeChecking;

		public AbstractCollectionProcessor(ISerializationDefinition definition, bool strictTypeChecking = DefaultOptions.StrictTypeChecking)
		: base(definition) { }

		/// <summary>
		/// Checks whether the given value can be assigned to a value of elementType. Also takes into account whether the element type is nullable.
		/// </summary>
		/// <param name="value">The value to be tested.</param>
		/// <param name="elementType">The type to be tested agains.</param>
		/// <returns>True if the value can be assigned. Always returns true if StrictTypeChecking is disabled.</returns>
		protected bool PassesTypeRestriction(object value, Type elementType)
		{
			if (!strictTypeChecking)
			{
				return true;
			}

			bool isNullable = SerializationUtilities.IsNullableType(elementType);
			if ((value == null) && !isNullable)
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("A null value was returned for a data structure that expects elements of type {0} which is not nullable.", elementType.GetType());
#endif
				return false;
			}
			else if ((value != null) && !elementType.IsAssignableFrom(value.GetType()))
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("A value of type {0} cannot be assigned to a data structure that expects elements of type {1}.", value.GetType().Name, elementType.Name);
#endif
				return false;
			}

			return true;
		}

		/// <summary>
		/// Counts the elements of a source value.
		/// </summary>
		/// <param name="sourceValue"></param>
		/// <returns>The number of elements in the source value.</returns>
		protected int CountElements(IEnumerable sourceValue)
		{
			if (sourceValue is IList)
			{
				return (sourceValue as IList).Count;
			}

			int count = 0;
			IEnumerator it = sourceValue.GetEnumerator();
			while (it.MoveNext())
			{
				++count;
			}

			return count;
		}
	}
}
