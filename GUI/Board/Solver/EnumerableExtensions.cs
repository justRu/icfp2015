using System;
using System.Collections.Generic;

namespace Solver
{
	public static class EnumerableExtensions
	{
		public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keyGetter)
			where TKey : struct, IComparable<TKey>
		{
			if (source == null)
				throw new ArgumentNullException("source");
			TKey? max = null;
			T result = default(T);
			foreach (var item in source)
			{
				var key = keyGetter(item);
				if (!max.HasValue || key.CompareTo(max.Value) > 0)
				{
					max = key;
					result = item;
				}
			}
			return result;
		}

		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
		{
			yield return item;
			foreach (var elem in source)
			{
				yield return elem;
			}
		}

		public static bool ContainsSubsequence<T>(this T[] array, T[] subarray)
			where T : IEquatable<T>
		{
			for (int i = 0; i < array.Length - subarray.Length; i++)
			{
				if (array[i].Equals(subarray[0]))
				{
					int j = 0;
					while (j < subarray.Length && array[i + j].Equals(subarray[j]))
					{
						j++;
					}
					if (j == subarray.Length)
						return true;
				}
			}
			return false;
		}
	}
}
