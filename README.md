# Chasm

A set of utility and helper libraries.

## [Chasm.SemanticVersioning](./Chasm.SemanticVersioning/)

A [semantic versioning](https://semver.org/spec/v2.0.0.html) library focused on functionality and performance.

See more info in the project's README.

## [Chasm.Collections](./Chasm.Collections/)

Provides various collection-related extension and utility methods:
- `ArrayExtensions` - extension versions of most static methods in the `Array` class, and some more.
- `CollectionExtensions` - some extensions for lists of tuples and key-value pairs.
- `EnumerableExtensions` - a couple of extensions for enumerables.
- Methods for returning empty read-only collections/enumerators/dictionaries (`ReadOnlyCollection.Empty<T>()`).

## [Chasm.Utilities](./Chasm.Utilities/)

Provides various utility types and methods:
- `Util.Fail`, `Util.Catch`, `Util.Is`, `Util.With` utility methods for shortening common branch code.
- `DelegateDisposable` - a class for quickly and easily creating custom disposable objects.

## [Chasm.Formatting](./Chasm.Formatting/)

Provides various formatting and parsing utility types and methods:
- `SpanBuilder` - for formatting complex deep objects without unnecessary memory allocations.
- `SpanParser` - for fluently and efficiently parsing text.
