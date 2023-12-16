# Chasm.Collections

Provides various collection-related extension and utility methods.

## `Enumerator`, `ReadOnlyCollection`, `ReadOnlyDictionary`

Contain static `Empty<T>()` methods that returns a global empty enumerator/read-only collection/read-only dictionary.

```cs
if (size == 0) return ReadOnlyCollection.Empty<string>();
/* ... */
```

`Enumerator` contains three versions of this method: `Empty()` (`IEnumerator`), `Empty<T>()` (`IEnumerator<T>`) and `EmptyAsync<T>()` (`IAsyncEnumerator<T>`).
