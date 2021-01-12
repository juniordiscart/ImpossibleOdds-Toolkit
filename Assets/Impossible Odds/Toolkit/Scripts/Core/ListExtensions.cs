/// <summary>
/// Sorted list extensions found on: https://www.jacksondunstan.com/articles/3189
/// </summary>

namespace ImpossibleOdds
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// List extensions.
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Shuffles a list's elements with the provided random number generator.
		/// Source: https://stackoverflow.com/a/22668974/892910
		/// </summary>
		/// <param name="list">The list whose elements should be shuffled.</param>
		/// <param name="rnd">The random number generator.</param>
		/// <typeparam name="T">The element type of the list.</typeparam>
		public static void Shuffle<T>(this IList<T> list, Random rnd)
		{
			rnd.ThrowIfNull(nameof(rnd));

			for (var i = 0; i < list.Count - 1; i++)
			{
				list.Swap(i, rnd.Next(i, list.Count));
			}
		}

		/// <summary>
		/// Swaps two elements in place.
		/// Source: https://stackoverflow.com/a/22668974/892910
		/// </summary>
		/// <param name="list">The list in which to swap the elements.</param>
		/// <param name="i">The index of the first element to swap.</param>
		/// <param name="j">The index of the second element to swap.</param>
		/// <typeparam name="T">The element type of the list.</typeparam>
		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			if ((i < 0) || (i >= list.Count))
			{
				throw new ArgumentOutOfRangeException(nameof(i));
			}
			else if ((j < 0) || (j >= list.Count))
			{
				throw new ArgumentOutOfRangeException(nameof(j));
			}

			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}

		/// <summary>
		/// Inserts a value in the list, assuming it is already sorted, preserving the order of elements.
		/// </summary>
		/// <param name="list">The list in which to insert the element.</param>
		/// <param name="value">Value to insert.</param>
		/// <typeparam name="T">The element type of the list.</typeparam>
		public static void SortedInsert<T>(this IList<T> list, T value)
		where T : IComparable<T>
		{
			SortedInsert(list, value, (a, b) => a.CompareTo(b));
		}

		/// <summary>
		/// Inserts a value in the list, assuming it is already sorted, preserving the order of elements.
		/// </summary>
		/// <param name="list">The list in which to insert the element.</param>
		/// <param name="value">Value to insert.</param>
		/// <param name="comparison">Comparison operator to determine the order of elements.</param>
		/// <typeparam name="T">The element type of the list.</typeparam>
		public static void SortedInsert<T>(this IList<T> list, T value, Comparison<T> comparison)
		{
			int startIndex = 0;
			int endIndex = list.Count;
			while (endIndex > startIndex)
			{
				int windowSize = endIndex - startIndex;
				int middleIndex = startIndex + (windowSize / 2);
				T middleValue = list[middleIndex];
				int compareToResult = comparison(middleValue, value);
				if (compareToResult == 0)
				{
					list.Insert(middleIndex, value);
					return;
				}
				else if (compareToResult < 0)
				{
					startIndex = middleIndex + 1;
				}
				else
				{
					endIndex = middleIndex;
				}
			}
			list.Insert(startIndex, value);
		}

		/// <summary>
		/// Inserts a value in the list, assuming it is already sorted, preserving the order of elements.
		/// </summary>
		/// <param name="list">The list in which to insert the element.</param>
		/// <param name="value">Value to insert.</param>
		public static void SortedInsert(this IList list, IComparable value)
		{
			SortedInsert(list, value, (a, b) => a.CompareTo(b));
		}

		/// <summary>
		/// Inserts a value in the list, assuming it is already sorted, preserving the order of elements.
		/// </summary>
		/// <param name="list">The list in which to insert the element.</param>
		/// <param name="value">Value to insert.</param>
		/// <param name="comparison">Comparison operator to determine the order of elements.</param>
		public static void SortedInsert(this IList list, IComparable value, Comparison<IComparable> comparison)
		{
			int startIndex = 0;
			int endIndex = list.Count;
			while (endIndex > startIndex)
			{
				int windowSize = endIndex - startIndex;
				int middleIndex = startIndex + (windowSize / 2);
				IComparable middleValue = (IComparable)list[middleIndex];
				int compareToResult = comparison(middleValue, value);
				if (compareToResult == 0)
				{
					list.Insert(middleIndex, value);
					return;
				}
				else if (compareToResult < 0)
				{
					startIndex = middleIndex + 1;
				}
				else
				{
					endIndex = middleIndex;
				}
			}
			list.Insert(startIndex, value);
		}
	}
}
