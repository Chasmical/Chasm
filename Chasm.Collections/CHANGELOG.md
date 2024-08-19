# Chasm.Collections Changelog

### v2.3.0
- ✨ Added `ArrayExtensions.NotNull<T>(this T[])`;
- ✨ Added `EnumerableExtensions.Only<T>(this IEnumerable<T>)`;
- ✨ Added `EnumerableExtensions.Only<T>(this IEnumerable<T>, Func<T, bool>)`;
- ✨ Added `EnumerableExtensions.OnlyOrDefault<T>(this IEnumerable<T>)`;
- ✨ Added `EnumerableExtensions.OnlyOrDefault<T>(this IEnumerable<T>, Func<T, bool>)`;

### v2.2.7
- 🧩 Added `net35` target. Now targets: `net8.0`, `net6.0`, `net5.0`, `netcoreapp3.0`,  `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.3`, `netstandard1.0`, `net47`, `net46`, `net45`, `net35`;

### v2.2.6
- 📄 Updated license information;

### v2.2.5
- 🩹 Added missing `[Pure]` and `[InstantHandle]` attributes;

### v2.2.4
- ✨ Added `EnumerableExtensions.Join<T>(this IEnumerable<T>, char)`;
- ✨ Added `EnumerableExtensions.Join<T>(this IEnumerable<T>, string)`;

### v2.2.3
- ♻️ Refactored read-only collections' `Empty()` methods to use .NET 8's new `Empty` properties;

### v2.2.2
- 🧩 Retargeted to: `net8.0`, `net6.0`, `net5.0`, `netcoreapp3.0`,  `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.3`, `netstandard1.0`, `net47`, `net46`, `net45`;
- ⬆️ Upgraded `JetBrains.Annotations` from 2023.2.0 to 2023.3.0;

### v2.2.1
- 🧩 Retargeted to: `net7.0`, `netcoreapp3.0`,  `netcoreapp2.0`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.3`, `netstandard1.0`, `net47`, `net46`, `net45`;
- ✨ Added `ArrayExtensions.ConvertAll<TInput, TOutput>(this Input[], Func<TInput, TOutput, bool)` for compatibility with `netcoreapp1.0`/`netstandard1.3`/`netstandard1.0` (marked as `[Obsolete]` on newer targets);

### v2.2.0
- ✨ Added `static class EnumerableExtensions`;
- ✨ Added `EnumerableExtensions.NotNull<T>(this IEnumerable<T?>)`;
- ✨ Added `EnumerableExtensions.ForEach<T>(this IEnumerable<T>, Action<T>)`;
- ✨ Added `EnumerableExtensions.EmptyIfNull<T>(this IEnumerable<T>?)`;
- ✨ Added `ArrayExtensions.Cast<TOutput>(this Array)`;
- ✨ Added `ArrayExtensions.OfType<TOutput>(this Array)`;
- ✨ Added `ArrayExtensions.Copy<T>(this T[])`;
- 🐛 Fixed `ArrayExtensions.Contains(this Array, object?)` behavior on non-zero-based arrays;

### v2.1.0
- ✨ Added `static class CollectionExtensions`;
- ✨ Added `CollectionExtensions.Add(this ICollection<KeyValuePair<TKey, TValue>>, TKey, TValue)`;
- ✨ Added `CollectionExtensions.Add(this IDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>)`;
- ✨ Added `CollectionExtensions.Add<...>(this ICollection<(...)>, ...)`;
- ✨ Added `static class ArrayExtensions`;
- ✨ Added `ArrayExtensions.IndexOf(this Array, object?)`;
- ✨ Added `ArrayExtensions.IndexOf(this Array, object?, int)`;
- ✨ Added `ArrayExtensions.IndexOf(this Array, object?, int, int)`;
- ✨ Added `ArrayExtensions.IndexOf<T>(this T[], T)`;
- ✨ Added `ArrayExtensions.IndexOf<T>(this T[], T, int)`;
- ✨ Added `ArrayExtensions.IndexOf<T>(this T[], T, int, int)`;
- ✨ Added `ArrayExtensions.LastIndexOf(this Array, object?)`;
- ✨ Added `ArrayExtensions.LastIndexOf(this Array, object?, int)`;
- ✨ Added `ArrayExtensions.LastIndexOf(this Array, object?, int, int)`;
- ✨ Added `ArrayExtensions.LastIndexOf<T>(this T[], T)`;
- ✨ Added `ArrayExtensions.LastIndexOf<T>(this T[], T, int)`;
- ✨ Added `ArrayExtensions.LastIndexOf<T>(this T[], T, int, int)`;
- ✨ Added `ArrayExtensions.FindIndex<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.FindIndex<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.FindIndex<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.FindLastIndex<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.FindLastIndex<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.FindLastIndex<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.Exists<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.Exists<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.Exists<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.TrueForAll<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.TrueForAll<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.TrueForAll<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.Find<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.Find<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.Find<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.FindLast<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.FindLast<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.FindLast<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.FindAll<T>(this T[], Predicate<T>)`;
- ✨ Added `ArrayExtensions.FindAll<T>(this T[], Func<T, int, bool>)`;
- ✨ Added `ArrayExtensions.FindAll<T>(this T[], Func<T, int, T[], bool>)`;
- ✨ Added `ArrayExtensions.ConvertAll<TInput, TOutput>(this TInput[], Converter<TInput, TOutput>)`;
- ✨ Added `ArrayExtensions.ConvertAll<TInput, TOutput>(this TInput[], Func<TInput, int, TOutput>)`;
- ✨ Added `ArrayExtensions.ConvertAll<TInput, TOutput>(this TInput[], Func<TInput, int, TInput[], TOutput)`;
- ✨ Added `ArrayExtensions.Clear(this Array)`;
- ✨ Added `ArrayExtensions.Clear(this Array, int, int)`;
- ✨ Added `ArrayExtensions.Fill<T>(this T[], T)`;
- ✨ Added `ArrayExtensions.Fill<T>(this T[], T, int, int)`;
- ✨ Added `ArrayExtensions.Reverse(this Array)`;
- ✨ Added `ArrayExtensions.Reverse(this Array, int, int)`;
- ✨ Added `ArrayExtensions.Reverse<T>(this T[])`;
- ✨ Added `ArrayExtensions.Reverse<T>(this T[], int, int)`;
- ✨ Added `ArrayExtensions.BinarySearch(this Array, object?)`;
- ✨ Added `ArrayExtensions.BinarySearch(this Array, object?, IComparer?)`;
- ✨ Added `ArrayExtensions.BinarySearch(this Array, int, int, object?)`;
- ✨ Added `ArrayExtensions.BinarySearch(this Array, int, int, object?, IComparer?)`;
- ✨ Added `ArrayExtensions.BinarySearch<T>(this T[], T)`;
- ✨ Added `ArrayExtensions.BinarySearch<T>(this T[], T, IComparer<T>?)`;
- ✨ Added `ArrayExtensions.BinarySearch<T>(this T[], int, int, T)`;
- ✨ Added `ArrayExtensions.BinarySearch<T>(this T[], int, int, T, IComparer<T>?)`;
- ✨ Added `ArrayExtensions.Sort(this Array)`;
- ✨ Added `ArrayExtensions.Sort(this Array, IComparer?)`;
- ✨ Added `ArrayExtensions.Sort(this Array, int, int)`;
- ✨ Added `ArrayExtensions.Sort(this Array, int, int, IComparer?)`;
- ✨ Added `ArrayExtensions.Sort<T>(this T[])`;
- ✨ Added `ArrayExtensions.Sort<T>(this T[], IComparer<T>?)`;
- ✨ Added `ArrayExtensions.Sort<T>(this T[], int, int)`;
- ✨ Added `ArrayExtensions.Sort<T>(this T[], int, int, IComparer<T>?)`;
- ✨ Added `ArrayExtensions.ForEach<T>(this T[], Action<T>)`;
- ✨ Added `ArrayExtensions.ForEach<T>(this T[], Action<T, int>)`;
- ✨ Added `ArrayExtensions.ForEach<T>(this T[], Action<T, int, T[]>)`;
- ✨ Added `ArrayExtensions.Contains(this Array, object?)`;
- ✨ Added `ArrayExtensions.Contains<T>(this T[], T)`;
- ✨ Added `ArrayExtensions.AsReadOnly<T>(this T[])`;

### v2.0.0
- 🧩 Targets: `net7.0`, `netcoreapp3.1`, `netstandard2.1`;
- ✨ Added `static class Enumerator`;
- ✨ Added `Enumerator.Empty()`;
- ✨ Added `Enumerator.Empty<T>()`;
- ✨ Added `Enumerator.EmptyAsync<T>()`;
- ✨ Added `static class ReadOnlyCollection`;
- ✨ Added `ReadOnlyCollection.Empty<T>()`;
- ✨ Added `static class ReadOnlyDictionary`;
- ✨ Added `ReadOnlyDictionary.Empty<TKey, TValue>()`;
