using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.Collections
{
    /// <summary>
    ///   <para>Provides a set of extension forms of the static methods in the <see cref="Array"/> class, and some more.</para>
    /// </summary>
    public static class ArrayExtensions
    {
        /// <inheritdoc cref="Array.IndexOf(Array, object?)"/>
        [Pure] public static int IndexOf(this Array array, object? value)
            => Array.IndexOf(array, value);
        /// <inheritdoc cref="Array.IndexOf(Array, object?, int)"/>
        [Pure] public static int IndexOf(this Array array, object? value, int startIndex)
            => Array.IndexOf(array, value, startIndex);
        /// <inheritdoc cref="Array.IndexOf(Array, object?, int, int)"/>
        [Pure] public static int IndexOf(this Array array, object? value, int startIndex, int count)
            => Array.IndexOf(array, value, startIndex, count);
        /// <inheritdoc cref="Array.IndexOf{T}(T[], T)"/>
        [Pure] public static int IndexOf<T>(this T[] array, T value)
            => Array.IndexOf(array, value);
        /// <inheritdoc cref="Array.IndexOf{T}(T[], T, int)"/>
        [Pure] public static int IndexOf<T>(this T[] array, T value, int startIndex)
            => Array.IndexOf(array, value, startIndex);
        /// <inheritdoc cref="Array.IndexOf{T}(T[], T, int, int)"/>
        [Pure] public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
            => Array.IndexOf(array, value, startIndex, count);

        /// <inheritdoc cref="Array.LastIndexOf(Array, object?)"/>
        [Pure] public static int LastIndexOf(this Array array, object? value)
            => Array.LastIndexOf(array, value);
        /// <inheritdoc cref="Array.LastIndexOf(Array, object?, int)"/>
        [Pure] public static int LastIndexOf(this Array array, object? value, int startIndex)
            => Array.LastIndexOf(array, value, startIndex);
        /// <inheritdoc cref="Array.LastIndexOf(Array, object?, int, int)"/>
        [Pure] public static int LastIndexOf(this Array array, object? value, int startIndex, int count)
            => Array.LastIndexOf(array, value, startIndex, count);
        /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T)"/>
        [Pure] public static int LastIndexOf<T>(this T[] array, T value)
            => Array.LastIndexOf(array, value);
        /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T, int)"/>
        [Pure] public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
            => Array.LastIndexOf(array, value, startIndex);
        /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T, int, int)"/>
        [Pure] public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
            => Array.LastIndexOf(array, value, startIndex, count);

        /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindIndex<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindIndex(array, predicate);

        /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindLastIndex<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindLastIndex(array, predicate);

        /// <inheritdoc cref="Array.Exists{T}(T[], Predicate{T})"/>
        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.Exists(array, predicate);

        /// <inheritdoc cref="Array.TrueForAll{T}(T[], Predicate{T})"/>
        [Pure] public static bool TrueForAll<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.TrueForAll(array, predicate);

        /// <inheritdoc cref="Array.Find{T}(T[], Predicate{T})"/>
        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.Find(array, predicate);

        /// <inheritdoc cref="Array.FindLast{T}(T[], Predicate{T})"/>
        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindLast(array, predicate);

        /// <inheritdoc cref="Array.FindAll{T}(T[], Predicate{T})"/>
        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindAll(array, predicate);

        /// <inheritdoc cref="Array.ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
        [Pure] public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, [InstantHandle] Converter<TInput, TOutput> converter)
            => Array.ConvertAll(array, converter);

        /// <inheritdoc cref="Array.Clear(Array)"/>
        public static void Clear(this Array array)
        {
#if NET6_0_OR_GREATER
            Array.Clear(array);
#else
            if (array is null) throw new ArgumentNullException(nameof(array));
            Array.Clear(array, 0, array.Length);
#endif
        }
        /// <inheritdoc cref="Array.Clear(Array, int, int)"/>
        public static void Clear(this Array array, int startIndex, int count)
            => Array.Clear(array, startIndex, count);

        /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
        public static void Fill<T>(this T[] array, T value)
            => Array.Fill(array, value);
        /// <inheritdoc cref="Array.Fill{T}(T[], T, int, int)"/>
        public static void Fill<T>(this T[] array, T value, int startIndex, int count)
            => Array.Fill(array, value, startIndex, count);

        /// <inheritdoc cref="Array.Reverse(Array)"/>
        public static void Reverse(this Array array)
            => Array.Reverse(array);
        /// <inheritdoc cref="Array.Reverse(Array, int, int)"/>
        public static void Reverse(this Array array, int startIndex, int count)
            => Array.Reverse(array, startIndex, count);
        /// <inheritdoc cref="Array.Reverse{T}(T[])"/>
        public static void Reverse<T>(this T[] array)
            => Array.Reverse(array);
        /// <inheritdoc cref="Array.Reverse{T}(T[], int, int)"/>
        public static void Reverse<T>(this T[] array, int startIndex, int count)
            => Array.Reverse(array, startIndex, count);

        /// <inheritdoc cref="Array.BinarySearch(Array, object?)"/>
        [Pure] public static int BinarySearch(this Array array, object? value)
            => Array.BinarySearch(array, value);
        /// <inheritdoc cref="Array.BinarySearch(Array, object?, IComparer?)"/>
        [Pure] public static int BinarySearch(this Array array, object? value, IComparer? comparer)
            => Array.BinarySearch(array, value, comparer);
        /// <inheritdoc cref="Array.BinarySearch(Array, int, int, object?)"/>
        [Pure] public static int BinarySearch(this Array array, int startIndex, int count, object? value)
            => Array.BinarySearch(array, startIndex, count, value);
        /// <inheritdoc cref="Array.BinarySearch(Array, int, int, object?, IComparer?)"/>
        [Pure] public static int BinarySearch(this Array array, int startIndex, int count, object? value, IComparer? comparer)
            => Array.BinarySearch(array, startIndex, count, value, comparer);
        /// <inheritdoc cref="Array.BinarySearch{T}(T[], T)"/>
        [Pure] public static int BinarySearch<T>(this T[] array, T value)
            => Array.BinarySearch(array, value);
        /// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T}?)"/>
        [Pure] public static int BinarySearch<T>(this T[] array, T value, IComparer<T>? comparer)
            => Array.BinarySearch(array, value, comparer);
        /// <inheritdoc cref="Array.BinarySearch{T}(T[], int, int, T)"/>
        [Pure] public static int BinarySearch<T>(this T[] array, int startIndex, int count, T value)
            => Array.BinarySearch(array, startIndex, count, value);
        /// <inheritdoc cref="Array.BinarySearch{T}(T[], int, int, T, IComparer{T}?)"/>
        [Pure] public static int BinarySearch<T>(this T[] array, int startIndex, int count, T value, IComparer<T>? comparer)
            => Array.BinarySearch(array, startIndex, count, value, comparer);

        /// <inheritdoc cref="Array.Sort(Array)"/>
        public static void Sort(this Array array)
            => Array.Sort(array);
        /// <inheritdoc cref="Array.Sort(Array, IComparer?)"/>
        public static void Sort(this Array array, IComparer? comparer)
            => Array.Sort(array, comparer);
        /// <inheritdoc cref="Array.Sort(Array, int, int)"/>
        public static void Sort(this Array array, int startIndex, int count)
            => Array.Sort(array, startIndex, count);
        /// <inheritdoc cref="Array.Sort(Array, int, int, IComparer?)"/>
        public static void Sort(this Array array, int startIndex, int count, IComparer? comparer)
            => Array.Sort(array, startIndex, count, comparer);
        /// <inheritdoc cref="Array.Sort{T}(T[])"/>
        public static void Sort<T>(this T[] array)
            => Array.Sort(array);
        /// <inheritdoc cref="Array.Sort{T}(T[], IComparer{T}?)"/>
        public static void Sort<T>(this T[] array, IComparer<T>? comparer)
            => Array.Sort(array, comparer);
        /// <inheritdoc cref="Array.Sort{T}(T[], int, int)"/>
        public static void Sort<T>(this T[] array, int startIndex, int count)
            => Array.Sort(array, startIndex, count);
        /// <inheritdoc cref="Array.Sort{T}(T[], int, int, IComparer{T}?)"/>
        public static void Sort<T>(this T[] array, int startIndex, int count, IComparer<T>? comparer)
            => Array.Sort(array, startIndex, count, comparer);

        /// <inheritdoc cref="Array.ForEach{T}(T[], Action{T})"/>
        public static void ForEach<T>(this T[] array, [InstantHandle] Action<T> action)
            => Array.ForEach(array, action);

    }
}
