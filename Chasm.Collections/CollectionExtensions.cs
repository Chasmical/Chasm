using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a set of extension methods for generic collections.</para>
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        ///   <para>Adds the specified <paramref name="key"/> and <paramref name="value"/> pair to the collection.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the collection.</typeparam>
        /// <typeparam name="TValue">The type of the values in the collection.</typeparam>
        /// <param name="list">The collection to add the specified <paramref name="key"/> and <paramref name="value"/> pair to.</param>
        /// <param name="key">The key to add to the collection.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
        public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> list, TKey key, TValue value)
        {
            if (list is null) throw new ArgumentNullException(nameof(list));
            list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        /// <summary>
        ///   <para>Adds the specified <paramref name="entry"/> to the dictionary.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary to add the specified <paramref name="entry"/> to.</param>
        /// <param name="entry">The entry to add to the dictionary.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <see langword="null"/>.</exception>
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> entry) where TKey : notnull
        {
            if (dictionary is null) throw new ArgumentNullException(nameof(dictionary));
            dictionary.Add(entry.Key, entry.Value);
        }

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET47_OR_GREATER
#pragma warning disable CS1573, CS1712
        /// <summary>
        ///   <para>Adds a tuple of the specified values to the collection.</para>
        /// </summary>
        /// <typeparam name="T1">The type of the first element of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second element of the tuple.</typeparam>
        /// <param name="list">The collection to add the specified tuple to.</param>
        /// <param name="t1">The first element of the tuple.</param>
        /// <param name="t2">The second element of the tuple.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
        public static void Add<T1, T2>(this ICollection<(T1, T2)> list, T1 t1, T2 t2)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2));
        /// <inheritdoc cref="Add{T1, T2}(ICollection{ValueTuple{T1, T2}}, T1, T2)"/>
        /// <typeparam name="T3">The type of the third element of the tuple.</typeparam>
        /// <param name="t3">The third element of the tuple.</param>
        public static void Add<T1, T2, T3>(this ICollection<(T1, T2, T3)> list, T1 t1, T2 t2, T3 t3)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2, t3));
        /// <inheritdoc cref="Add{T1, T2, T3}(ICollection{ValueTuple{T1, T2, T3}}, T1, T2, T3)"/>
        /// <typeparam name="T4">The type of the fourth element of the tuple.</typeparam>
        /// <param name="t4">The fourth element of the tuple.</param>
        public static void Add<T1, T2, T3, T4>(this ICollection<(T1, T2, T3, T4)> list, T1 t1, T2 t2, T3 t3, T4 t4)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2, t3, t4));
        /// <inheritdoc cref="Add{T1, T2, T3, T4}(ICollection{ValueTuple{T1, T2, T3, T4}}, T1, T2, T3, T4)"/>
        /// <typeparam name="T5">The type of the fifth element of the tuple.</typeparam>
        /// <param name="t5">The fifth element of the tuple.</param>
        public static void Add<T1, T2, T3, T4, T5>(this ICollection<(T1, T2, T3, T4, T5)> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2, t3, t4, t5));
        /// <inheritdoc cref="Add{T1, T2, T3, T4, T5}(ICollection{ValueTuple{T1, T2, T3, T4, T5}}, T1, T2, T3, T4, T5)"/>
        /// <typeparam name="T6">The type of the sixth element of the tuple.</typeparam>
        /// <param name="t6">The sixth element of the tuple.</param>
        public static void Add<T1, T2, T3, T4, T5, T6>(this ICollection<(T1, T2, T3, T4, T5, T6)> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2, t3, t4, t5, t6));
        /// <inheritdoc cref="Add{T1, T2, T3, T4, T5, T6}(ICollection{ValueTuple{T1, T2, T3, T4, T5, T6}}, T1, T2, T3, T4, T5, T6)"/>
        /// <typeparam name="T7">The type of the seventh element of the tuple.</typeparam>
        /// <param name="t7">The seventh element of the tuple.</param>
        public static void Add<T1, T2, T3, T4, T5, T6, T7>(this ICollection<(T1, T2, T3, T4, T5, T6, T7)> list, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
            => (list ?? throw new ArgumentNullException(nameof(list))).Add((t1, t2, t3, t4, t5, t6, t7));
#pragma warning restore CS1573, CS1712
#endif

        /// <summary>
        ///   <para>Converts the specified key-value <paramref name="pair"/> to an equivalent <see cref="DictionaryEntry"/> struct.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="pair">The <see cref="KeyValuePair{TKey,TValue}"/> struct to convert.</param>
        /// <returns>The <see cref="DictionaryEntry"/> struct equivalent to the specified key-value <paramref name="pair"/>.</returns>
        [Pure] public static DictionaryEntry AsEntry<TKey, TValue>(this KeyValuePair<TKey, TValue> pair)
        {
            // constructor throws on null key only on .NET Framework 1.0 and 1.1
            return new DictionaryEntry(pair.Key!, pair.Value);
        }
        /// <summary>
        ///   <para>Casts the specified dictionary <paramref name="entry"/> to the specified <see cref="KeyValuePair{TKey,TValue}"/> type.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the key to cast to.</typeparam>
        /// <typeparam name="TValue">The type of the value to cast to.</typeparam>
        /// <param name="entry">The <see cref="DictionaryEntry"/> to cast to the specified <see cref="KeyValuePair{TKey,TValue}"/> type.</param>
        /// <returns>The <see cref="KeyValuePair{TKey,TValue}"/> struct equivalent to the specified dictionary <paramref name="entry"/>.</returns>
        /// <exception cref="InvalidCastException"><paramref name="entry"/>'s key cannot be cast to type <typeparamref name="TKey"/> or its value cannot be cast to type <typeparamref name="TValue"/>.</exception>
        [Pure] public static KeyValuePair<TKey, TValue> Cast<TKey, TValue>(this DictionaryEntry entry)
            => new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value!);

        /// <summary>
        ///   <para>Creates a <see cref="IDictionaryEnumerator"/> from the specified key-value pair <paramref name="enumerator"/>.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="enumerator">The <see cref="IEnumerator{T}"/> that enumerates <see cref="KeyValuePair{TKey,TValue}"/> items.</param>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates <see cref="DictionaryEntry"/> items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        [Pure, MustDisposeResource]
        public static IDictionaryEnumerator ToDictionaryEnumerator<TKey, TValue>(
            [HandlesResourceDisposal] this IEnumerator<KeyValuePair<TKey, TValue>> enumerator
        )
        {
            if (enumerator is null) throw new ArgumentNullException(nameof(enumerator));
            return new DictionaryEnumerator<TKey, TValue>(enumerator);
        }

        private readonly struct DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IEnumerator<DictionaryEntry>
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
                => _enumerator = enumerator;

            public DictionaryEntry Current => _enumerator.Current.AsEntry();
            public DictionaryEntry Entry => Current;
            object IEnumerator.Current => Current;
            public object Key => _enumerator.Current.Key!;
            public object? Value => _enumerator.Current.Value;

            public bool MoveNext()
                => _enumerator.MoveNext();
            public void Reset()
                => _enumerator.Reset();
            public void Dispose()
                => _enumerator.Dispose();

        }

    }
}
