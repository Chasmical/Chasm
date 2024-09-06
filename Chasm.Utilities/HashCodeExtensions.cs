#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
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
        /// <summary>
        ///   <para>Adds the specified span of values to the hash code.</para>
        /// </summary>
        /// <typeparam name="T">The type of the values to add to the hash code.</typeparam>
        /// <param name="hashCode">The hash code instance to add the specified span of values to.</param>
        /// <param name="items">The span of values to add to the hash code.</param>
        public static void AddRange<T>(this HashCode hashCode, ReadOnlySpan<T> items)
        {
            for (int i = 0; i < items.Length; i++)
                hashCode.Add(items[i]);
        }
        /// <summary>
        ///   <para>Adds the specified span of values to the hash code, using the specified <paramref name="comparer"/>'s hash code function.</para>
        /// </summary>
        /// <typeparam name="T">The type of the values to add to the hash code.</typeparam>
        /// <param name="hashCode">The hash code instance to add the specified span of values to.</param>
        /// <param name="items">The span of values to add to the hash code.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use to calculate the hash code. This value can be <see langword="null"/>, which will use the default equality comparer for <typeparamref name="T"/>.</param>
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

        /// <summary>
        ///   <para>Adds the specified collection of values to the hash code.</para>
        /// </summary>
        /// <typeparam name="T">The type of the values to add to the hash code.</typeparam>
        /// <param name="hashCode">The hash code instance to add the specified collection of values to.</param>
        /// <param name="collection">The collection of values to add to the hash code.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
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
        /// <summary>
        ///   <para>Adds the specified collection of values to the hash code, using the specified <paramref name="comparer"/>'s hash code function.</para>
        /// </summary>
        /// <typeparam name="T">The type of the values to add to the hash code.</typeparam>
        /// <param name="hashCode">The hash code instance to add the specified collection of values to.</param>
        /// <param name="collection">The collection of values to add to the hash code.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use to calculate the hash code. This value can be <see langword="null"/>, which will use the default equality comparer for <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
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
