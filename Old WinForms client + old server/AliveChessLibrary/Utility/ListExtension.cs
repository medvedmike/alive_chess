﻿using System;
using System.Collections.Generic;

namespace AliveChessLibrary.Utility
{
    public static class ListExtension
    {
        public static bool Exists<T>(this IList<T> source, Func<T, bool> action)
        {
            foreach (T item in source)
                if (action(item)) return true;
            return false;
        }

        public static int FindIndex<T>(this IList<T> source, Func<T, bool> action)
        {
            for (int i = 0; i < source.Count; i++)
                if (action(source[i])) return i;
            return -1;
        }

        public static T FindElement<T>(this IList<T> source, Func<T, bool> action)
        {
            for (int i = 0; i < source.Count; i++)
                if (action(source[i])) return source[i];
            return default(T);
        }

        public static void RemoveElement<T>(this IList<T> source, Func<T, bool> predicate)
        {
            foreach (T v in source.Next())
                if (predicate(v)) source.Remove(v);
        }

        private static IEnumerable<T> Next<T>(this IList<T> source)
        {
            for (int i = 0; i < source.Count; i++)
                yield return source[i];
        }
    }
}
