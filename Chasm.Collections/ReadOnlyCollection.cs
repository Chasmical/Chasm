﻿using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a static method for getting instances of empty read-only collections.</para>
    /// </summary>
    public static class ReadOnlyCollection
    {
        /// <summary>
        ///   <para>Returns an empty read-only collection.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the read-only collection.</typeparam>
        /// <returns>An empty read-only collection.</returns>
        [Pure] public static ReadOnlyCollection<T> Empty<T>()
            => EmptyCollection<T>.Instance;

        private static class EmptyCollection<T>
        {
            internal static readonly ReadOnlyCollection<T> Instance = new(Array.Empty<T>());
        }
    }
}
