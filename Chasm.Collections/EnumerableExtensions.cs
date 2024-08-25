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
            if (default(T) is not null) return source!;
            return source.Where(static item => item is not null)!;
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
            => source ?? [];

        /// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
        [Pure] public static string Join<T>([InstantHandle] this IEnumerable<T> values, char separator)
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return string.Join(separator, values);
#elif NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET40_OR_GREATER
            return string.Join(separator.ToString(), values);
#else
            return string.Join(separator.ToString(), values.Select(v => v?.ToString()).ToArray());
#endif
        }
        /// <inheritdoc cref="string.Join{T}(string, IEnumerable{T})"/>
        [Pure] public static string Join<T>([InstantHandle] this IEnumerable<T> values, string? separator)
        {
#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET40_OR_GREATER
            return string.Join(separator, values);
#else
            return string.Join(separator, values.Select(v => v?.ToString()).ToArray());
#endif
        }

        /// <summary>
        ///   <para>Returns the only element of the sequence. The sequence must contain exactly one element, no less, no more.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to enumerate.</param>
        /// <returns>The only element of the source sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> contains 0 or 2 or more elements.</exception>
        [Pure] public static T Only<T>([InstantHandle] this IEnumerable<T> source)
        {
            if (OnlyCore(source, out T? result)) return result!;
            throw new ArgumentException($"{nameof(source)} contains 0 or 2 or more elements.", nameof(source));
        }
        /// <summary>
        ///   <para>Returns the only element of the sequence that satisfies the specified <paramref name="predicate"/>. The sequence must contain exactly one such element, no less, no more.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to enumerate.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The only element of the source sequence that satisfies the specified <paramref name="predicate"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/> contains 0 or 2 or more elements that satisfy the specified <paramref name="predicate"/>.</exception>
        [Pure] public static T Only<T>([InstantHandle] this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (OnlyCore(source, predicate, out T? result)) return result!;
            throw new ArgumentException($"{nameof(source)} contains 0 or 2 or more elements.", nameof(source));
        }

        /// <summary>
        ///   <para>Returns the only element of the sequence, or <see langword="default"/>(<typeparamref name="T"/>), if the sequence contains 0 or 2 or more elements.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to enumerate.</param>
        /// <returns>The only element of the source sequence, or <see langword="default"/>(<typeparamref name="T"/>), if the sequence contains 0 or 2 or more elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        [Pure] public static T? OnlyOrDefault<T>([InstantHandle] this IEnumerable<T> source)
        {
            OnlyCore(source, out T? result);
            return result;
        }
        /// <summary>
        ///   <para>Returns the only element of the sequence that satisfies the specified <paramref name="predicate"/>, or <see langword="default"/>(<typeparamref name="T"/>), if the sequence contains 0 or 2 or more such elements.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of elements to enumerate.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The only element of the source sequence that satisfies the specified <paramref name="predicate"/>, or <see langword="default"/>(<typeparamref name="T"/>), if the sequence contains 0 or 2 or more such elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        [Pure] public static T? OnlyOrDefault<T>([InstantHandle] this IEnumerable<T> source, Func<T, bool> predicate)
        {
            OnlyCore(source, predicate, out T? result);
            return result;
        }

        [Pure] private static bool OnlyCore<T>([InstantHandle] this IEnumerable<T> source, out T? result)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    result = enumerator.Current;
                    if (!enumerator.MoveNext()) return true;
                }
                result = default;
                return false;
            }
        }
        [Pure] private static bool OnlyCore<T>([InstantHandle] this IEnumerable<T> source, Func<T, bool> predicate, out T? result)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                bool found = false;
                result = default;

                while (enumerator.MoveNext())
                {
                    T item = enumerator.Current;
                    if (!predicate(item)) continue;

                    if (found)
                    {
                        result = default;
                        return false;
                    }
                    result = item;
                    found = true;
                }
                return found;
            }
        }

    }
}
