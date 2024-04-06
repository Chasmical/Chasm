using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a static method for getting instances of empty read-only dictionaries.</para>
    /// </summary>
    public static class ReadOnlyDictionary
    {
        /// <summary>
        ///   <para>Returns an empty read-only dictionary.</para>
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <returns>An empty read-only dictionary.</returns>
        [Pure] public static ReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : notnull
        {
#if NET8_0_OR_GREATER
            return ReadOnlyDictionary<TKey, TValue>.Empty;
#else
            return EmptyDictionary<TKey, TValue>.Instance;
#endif
        }

#if !NET8_0_OR_GREATER
        private static class EmptyDictionary<TKey, TValue> where TKey : notnull
        {
            internal static readonly ReadOnlyDictionary<TKey, TValue> Instance = new(new Dictionary<TKey, TValue>());
        }
#endif

    }
}
