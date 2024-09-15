using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a set of extension methods for <see cref="IEnumerator{T}"/>s.</para>
    /// </summary>
    public static class EnumeratorExtensions
    {
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
            ANE.ThrowIfNull(enumerator);
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

            public bool MoveNext() => _enumerator.MoveNext();
            public void Reset() => _enumerator.Reset();
            public void Dispose() => _enumerator.Dispose();
        }

    }
}
