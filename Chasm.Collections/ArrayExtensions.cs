using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindIndex<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i))
                    return i;
            return -1;
        }
        /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindIndex<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i, array))
                    return i;
            return -1;
        }

        /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindLastIndex<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindLastIndex(array, predicate);
        /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindLastIndex<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = array.Length - 1; i >= 0; i--)
                if (predicate(array[i], i))
                    return i;
            return -1;
        }
        /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
        [Pure] public static int FindLastIndex<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = array.Length - 1; i >= 0; i--)
                if (predicate(array[i], i, array))
                    return i;
            return -1;
        }

        /// <inheritdoc cref="Array.Exists{T}(T[], Predicate{T})"/>
        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.Exists(array, predicate);
        /// <inheritdoc cref="Array.Exists{T}(T[], Predicate{T})"/>
        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
            => FindIndex(array, predicate) != -1;
        /// <inheritdoc cref="Array.Exists{T}(T[], Predicate{T})"/>
        [Pure] public static bool Exists<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
            => FindIndex(array, predicate) != -1;

        /// <inheritdoc cref="Array.TrueForAll{T}(T[], Predicate{T})"/>
        [Pure] public static bool TrueForAll<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.TrueForAll(array, predicate);
        /// <inheritdoc cref="Array.TrueForAll{T}(T[], Predicate{T})"/>
        [Pure] public static bool TrueForAll<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (!predicate(array[i], i))
                    return false;
            return true;
        }
        /// <inheritdoc cref="Array.TrueForAll{T}(T[], Predicate{T})"/>
        [Pure] public static bool TrueForAll<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (!predicate(array[i], i, array))
                    return false;
            return true;
        }

        /// <inheritdoc cref="Array.Find{T}(T[], Predicate{T})"/>
        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.Find(array, predicate);
        /// <inheritdoc cref="Array.Find{T}(T[], Predicate{T})"/>
        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i))
                    return array[i];
            return default;
        }
        /// <inheritdoc cref="Array.Find{T}(T[], Predicate{T})"/>
        [Pure] public static T? Find<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i, array))
                    return array[i];
            return default;
        }

        /// <inheritdoc cref="Array.FindLast{T}(T[], Predicate{T})"/>
        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindLast(array, predicate);
        /// <inheritdoc cref="Array.FindLast{T}(T[], Predicate{T})"/>
        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = array.Length - 1; i >= 0; i--)
                if (predicate(array[i], i))
                    return array[i];
            return default;
        }
        /// <inheritdoc cref="Array.FindLast{T}(T[], Predicate{T})"/>
        [Pure] public static T? FindLast<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            for (int i = array.Length - 1; i >= 0; i--)
                if (predicate(array[i], i, array))
                    return array[i];
            return default;
        }

        /// <inheritdoc cref="Array.FindAll{T}(T[], Predicate{T})"/>
        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Predicate<T> predicate)
            => Array.FindAll(array, predicate);
        /// <inheritdoc cref="Array.FindAll{T}(T[], Predicate{T})"/>
        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Func<T, int, bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            List<T> list = new();
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i))
                    list.Add(array[i]);
            return list.ToArray();
        }
        /// <inheritdoc cref="Array.FindAll{T}(T[], Predicate{T})"/>
        [Pure] public static T[] FindAll<T>(this T[] array, [InstantHandle] Func<T, int, T[], bool> predicate)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            List<T> list = new();
            for (int i = 0; i < array.Length; i++)
                if (predicate(array[i], i, array))
                    list.Add(array[i]);
            return list.ToArray();
        }

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER
        /// <inheritdoc cref="Array.ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
        [Pure] public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, [InstantHandle] Converter<TInput, TOutput> converter)
            => Array.ConvertAll(array, converter);

        /// <inheritdoc cref="ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
        [Obsolete("Prefer the overload with Converter<TInput, TOutput> parameter.")]
#endif
        [Pure] public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, [InstantHandle] Func<TInput, TOutput> converter, bool _ = true)
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER
            return Array.ConvertAll(array, new Converter<TInput, TOutput>(converter));
#else
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (converter is null) throw new ArgumentNullException(nameof(converter));
            TOutput[] result = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = converter(array[i]);
            return result;
#endif
        }

        /// <inheritdoc cref="Array.ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
        [Pure] public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, [InstantHandle] Func<TInput, int, TOutput> converter)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (converter is null) throw new ArgumentNullException(nameof(converter));
            TOutput[] result = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = converter(array[i], i);
            return result;
        }
        /// <inheritdoc cref="Array.ConvertAll{TInput, TOutput}(TInput[], Converter{TInput, TOutput})"/>
        [Pure] public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, [InstantHandle] Func<TInput, int, TInput[], TOutput> converter)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (converter is null) throw new ArgumentNullException(nameof(converter));
            TOutput[] result = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = converter(array[i], i, array);
            return result;
        }

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
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Array.Fill(array, value);
#else
            if (array is null) throw new ArgumentNullException(nameof(array));
            Fill(array, value, 0, array.Length);
#endif
        }
        /// <inheritdoc cref="Array.Fill{T}(T[], T, int, int)"/>
        public static void Fill<T>(this T[] array, T value, int startIndex, int count)
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Array.Fill(array, value, startIndex, count);
#else
            if (array is null) throw new ArgumentNullException(nameof(array));
            const string outOfRangeMsg = "Index was out of range. Must be non-negative and less than or equal to the size of the collection.";
            if (startIndex < 0 || startIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(startIndex), outOfRangeMsg);
            const string outOfRangeMsg2 = "Count must be positive and count must refer to a location within the string/array/collection.";
            if (count < 0 || startIndex > array.Length - count) throw new ArgumentOutOfRangeException(nameof(count), outOfRangeMsg2);

            int end = startIndex + count;
            for (int i = startIndex; i < end; i++)
                array[i] = value;
#endif
        }

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
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET20_OR_GREATER
            Array.ForEach(array, action);
#else
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (action is null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < array.Length; i++)
                action(array[i]);
#endif
        }
        /// <inheritdoc cref="Array.ForEach{T}(T[], Action{T})"/>
        public static void ForEach<T>(this T[] array, [InstantHandle] Action<T, int> action)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (action is null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < array.Length; i++)
                action(array[i], i);
        }
        /// <inheritdoc cref="Array.ForEach{T}(T[], Action{T})"/>
        public static void ForEach<T>(this T[] array, [InstantHandle] Action<T, int, T[]> action)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (action is null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < array.Length; i++)
                action(array[i], i, array);
        }

        // Note: Do not support multidimensional arrays in the Contains method.
        //       Both Array.IndexOf and IList.Contains don't handle that case.

        /// <summary>
        ///   <para>Determines whether the specified one-dimensional <paramref name="array"/> contains the specified <paramref name="value"/>.</para>
        /// </summary>
        /// <param name="array">The one-dimensional array to search.</param>
        /// <param name="value">The object to locate in <paramref name="array"/>.</param>
        /// <returns><see langword="true"/>, if <paramref name="value"/> was found in the <paramref name="array"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="RankException"><paramref name="array"/> is multidimensional.</exception>
        [Pure] public static bool Contains(this Array array, object? value)
            => Array.IndexOf(array, value) != array.GetLowerBound(0) - 1;
        /// <summary>
        ///   <para>Determines whether the specified one-dimensional <paramref name="array"/> contains the specified <paramref name="value"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array to search.</param>
        /// <param name="value">The object to locate in <paramref name="array"/>.</param>
        /// <returns><see langword="true"/>, if <paramref name="value"/> was found in the <paramref name="array"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="RankException"><paramref name="array"/> is multidimensional.</exception>
        [Pure] public static bool Contains<T>(this T[] array, T value)
            => Array.IndexOf(array, value) != -1;

        /// <summary>
        ///   <para>Casts the elements of the specified one-dimensional, zero-based <paramref name="array"/> to the specified type.</para>
        /// </summary>
        /// <typeparam name="TOutput">The type to cast the elements of the <paramref name="array"/> to.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array to cast the elements of.</param>
        /// <returns>An array that contains each element of the source <paramref name="array"/> cast to the specified type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="RankException"><paramref name="array"/> is multidimensional.</exception>
        /// <exception cref="InvalidCastException">An element in the <paramref name="array"/> cannot be cast to type <typeparamref name="TOutput"/>.</exception>
        [Pure] public static TOutput[] Cast<TOutput>(this Array array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new RankException($"{nameof(array)} is multidimensional.");
            if (array.GetLowerBound(0) != 0) throw new ArgumentException($"{nameof(array)} is not zero-based.");

            TOutput[] result = new TOutput[array.Length];
            if (array is object[] objectArray)
            {
                for (int i = 0; i < objectArray.Length; i++)
                    result[i] = (TOutput)objectArray[i];
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                    result[i] = (TOutput)array.GetValue(i)!;
            }
            return result;
        }
        /// <summary>
        ///   <para>Filters the elements of the specified one-dimensional, zero-based <paramref name="array"/> based on the specified type.</para>
        /// </summary>
        /// <typeparam name="TOutput">The type to filter the elements of the <paramref name="array"/> on.</typeparam>
        /// <param name="array">The one-dimensional, zero-based array whose elements to filter.</param>
        /// <returns>An array that contains elements from the source <paramref name="array"/> of type <typeparamref name="TOutput"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="RankException"><paramref name="array"/> is multidimensional.</exception>
        /// <exception cref="ArgumentException"><paramref name="array"/> is not zero-based.</exception>
        [Pure] public static TOutput[] OfType<TOutput>(this Array array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new RankException($"{nameof(array)} is multidimensional.");
            if (array.GetLowerBound(0) != 0) throw new ArgumentException($"{nameof(array)} is not zero-based.");

            List<TOutput> list = new();
            if (array is object[] objectArray)
            {
                for (int i = 0; i < objectArray.Length; i++)
                    if (objectArray[i] is TOutput output)
                        list.Add(output);
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                    if (array.GetValue(i) is TOutput output)
                        list.Add(output);
            }
            return list.ToArray();
        }

        /// <summary>
        ///   <para>Creates a shallow copy of the specified <paramref name="array"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The array to copy.</param>
        /// <returns>A shallow copy of the <paramref name="array"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Pure] public static T[] Copy<T>(this T[] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
#if NET5_0_OR_GREATER
            T[] copy = GC.AllocateUninitializedArray<T>(array.Length);
#else
            T[] copy = new T[array.Length];
#endif
            Array.Copy(array, 0, copy, 0, array.Length);
            return copy;
        }

        /// <summary>
        ///   <para>Returns a <see cref="ReadOnlyCollection{T}"/> wrapper for the specified <paramref name="array"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <param name="array">The array to wrap.</param>
        /// <returns>The <see cref="ReadOnlyCollection{T}"/> wrapper for the specified <paramref name="array"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        [Pure] public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            if (array is null) throw new ArgumentNullException(nameof(array));
            return array.Length != 0 ? new ReadOnlyCollection<T>(array) : ReadOnlyCollection.Empty<T>();
        }

    }
}
