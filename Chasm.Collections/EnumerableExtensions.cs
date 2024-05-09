using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a set of extension methods for <see cref="IEnumerable{T}"/>.</para>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///   <para>Returns non-null elements from a sequence.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to remove <see langword="null"/> elements from.</param>
        /// <returns>A sequence that contains non-null elements from the source sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        [Pure, LinqTunnel, ItemNotNull]
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            return (default(T) is not null ? source : source.Where(static item => item is not null))!;
        }

        /// <summary>
        ///   <para>Invokes the specified <paramref name="action"/> on each element of the sequence.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to enumerate.</param>
        /// <param name="action">The <see cref="Action{T}"/> to invoke with each element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
        public static void ForEach<T>([InstantHandle] this IEnumerable<T> source, [InstantHandle] Action<T> action)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (action is null) throw new ArgumentNullException(nameof(action));
            foreach (T item in source)
                action(item);
        }

        /// <summary>
        ///   <para>Returns the elements of the specified sequence, or an empty sequence if it's <see langword="null"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return the elements from, if it's not <see langword="null"/>.</param>
        /// <returns><paramref name="source"/>, if it's not <see langword="null"/>; otherwise, an empty sequence.</returns>
        [Pure, LinqTunnel]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
            => source ?? Enumerable.Empty<T>();

        /// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
        public static string Join<T>(this IEnumerable<T> values, char separator)
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return string.Join(separator, values);
#else
            return string.Join(separator.ToString(), values);
#endif
        }
        /// <inheritdoc cref="string.Join{T}(string, IEnumerable{T})"/>
        public static string Join<T>(this IEnumerable<T> values, string? separator)
            => string.Join(separator, values);

    }
}
