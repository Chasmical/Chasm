# Chasm.Collections

Provides various collection-related extension and utility methods.

## `ArrayExtensions`

Contains extension versions of most static methods in the `Array` class.

```cs
int[] numbers = new int[10] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };

int index = numbers.IndexOf(8);

string[] strings = numbers.ConvertAll(num => num.ToString());

string? found = strings.Find(s => s.Length > 1);
```

Includes overloads of `Find`, `Exists`, `ConvertAll` and other methods that use delegates, that provide one or two extra parameters - the element's index and the source array. Similar to how it works in JavaScript arrays.

```cs
string[] distinct = strings.FindAll((el, i, arr) => i == arr.IndexOf(el));
```

Also includes some common LINQ methods optimized for arrays: `Cast`, `OfType` and `Contains`. And `Copy` for simpler shallow copying.

```cs
object[] arr = new object[] { "Hello", "World", "!" };

string[] strings = arr.Cast<string>();

object[] shallowCopy = arr.Copy();
```

## `CollectionExtensions`

Contains `Add` overloads for adding tuples and `KeyValuePair`s to collections, without having to enclose the elements in double parenthesis.

```cs
List<(string, double)> units = new();

units.Add("meter", 1);
units.Add("centimeter", 0.01);
```

## `EnumerableExtensions`

Contains `NotNull`, `ForEach`, `EmptyIfNull` and `Join` extension methods for enumerables.

```cs
string?[] values = new string?[] { "Hello", null, "World", null, "!" };

Console.WriteLine(values.NotNull().Join(", "));
// Output: Hello, World, !
```

## `Enumerator`, `ReadOnlyCollection`, `ReadOnlyDictionary`

Contain static `Empty<T>()` methods that returns a global empty enumerator/read-only collection/read-only dictionary.

```cs
if (size == 0) return ReadOnlyCollection.Empty<string>();
/* ... */
```

`Enumerator` contains three versions of this method: `Empty()` (`IEnumerator`), `Empty<T>()` (`IEnumerator<T>`) and `EmptyAsync<T>()` (`IAsyncEnumerator<T>`).
