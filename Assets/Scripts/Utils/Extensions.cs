using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public static class ICollectionExtensions
	{
		public static int IndexOf<T>(this IList<T> source, T value)
		{
			return source.IndexOf(value);
		}

		public static int IndexOf<T>(this IList<T> source, System.Predicate<T> match)
		{
			if (match == null)
			{
				return -1;
			}

			int index = 0;
			foreach (T item in source)
			{
				if (match(item))
				{
					return index;
				}
				++index;
			}
			return -1;
		}

		public static bool Contains<T>(this IList<T> source, T value)
		{
			return source.Contains(value);
		}

		public static bool Contains<T>(this IList<T> source, System.Predicate<T> match)
		{
			if (match == null)
			{
				return false;
			}

			foreach (T item in source)
			{
				if (match(item))
				{
					return true;
				}
			}
			return false;
		}

		public static void Foreach<T>(this IList<T> source, System.Action<T> action)
		{
			if (action == null)
			{
				return;
			}

			foreach (T item in source)
			{
				action(item);
			}
		}

		public static T First<T>(this IList<T> source, T defaultValue = default(T))
		{
			if (source.Count > 0)
			{
				return source[0];
			}
			return defaultValue;
		}

		public static T Last<T>(this IList<T> source, T defaultValue = default(T))
		{
			int count = source.Count;
			if (count > 0)
			{
				return source[count - 1];
			}
			return defaultValue;
		}

		public static TKey FirstKey<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> source, TKey defaultValue = default(TKey))
		{
			foreach (KeyValuePair<TKey, TValue> pair in source)
			{
				return pair.Key;
			}
			return defaultValue;
		}

		public static TValue FirstValue<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> source, TValue defaultValue = default(TValue))
		{
			foreach (KeyValuePair<TKey, TValue> pair in source)
			{
				return pair.Value;
			}
			return defaultValue;
		}
	}

	public static class UnityExtensionsUtil
	{
		/// <returns>if comp is Transform, return comp as Transform.</returns>
		public static Transform GetTrans(this Component comp)
		{
			return comp.GetComp<Transform>();
		}

		/// <returns>if comp is T, return comp as T.</returns>
		public static T GetComp<T>(this Component comp) where T : Component
		{
			return comp is T ? comp as T : comp.GetComponent<T>();
		}
	}
}
