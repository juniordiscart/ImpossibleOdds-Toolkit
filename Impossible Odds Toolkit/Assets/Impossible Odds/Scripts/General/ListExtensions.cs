namespace ImpossibleOdds
{
	using System;
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
			if (rnd == null)
			{
				throw new ArgumentNullException("rnd");
			}

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
				throw new ArgumentOutOfRangeException("i");
			}
			else if ((j < 0) || (j >= list.Count))
			{
				throw new ArgumentOutOfRangeException("j");
			}

			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}
