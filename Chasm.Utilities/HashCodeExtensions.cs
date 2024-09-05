#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.Utilities
{
    /// <summary>
    ///   <para>Provides a set of extension methods for <see cref="HashCode"/>.</para>
    /// </summary>
    public static class HashCodeExtensions
    {
        public static void AddRange<T>(this HashCode hashCode, ReadOnlySpan<T> items)
        {
            for (int i = 0; i < items.Length; i++)
                hashCode.Add(items[i]);
        }
        public static void AddRange<T>(this HashCode hashCode, ReadOnlySpan<T> items, IEqualityComparer<T>? comparer)
        {
            if (comparer is null) // use AddRange without comparer
            {
                AddRange(hashCode, items);
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                    hashCode.Add(items[i], comparer);
            }
        }

        public static void AddRange<T>(this HashCode hashCode, [InstantHandle] IEnumerable<T> collection)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));

            if (collection is T[] array) // optimized path for arrays
            {
                AddRange(hashCode, (ReadOnlySpan<T>)array);
            }
            else // foreach a generic enumerable
            {
                foreach (T item in collection)
                    hashCode.Add(item);
            }
        }
        public static void AddRange<T>(this HashCode hashCode, [InstantHandle] IEnumerable<T> collection, IEqualityComparer<T>? comparer)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));

            if (comparer is null) // use AddRange without comparer
            {
                AddRange(hashCode, collection);
            }
            else if (collection is T[] array) // optimized path for arrays
            {
                AddRange(hashCode, (ReadOnlySpan<T>)array, comparer);
            }
            else // foreach a generic enumerable
            {
                foreach (T item in collection)
                    hashCode.Add(item, comparer);
            }
        }

    }
}
#endif
