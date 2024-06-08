# Chasm

A set of utility and helper libraries.

## [Chasm.SemanticVersioning](./Chasm.SemanticVersioning#readme)

A library designed for efficiently working with [SemVer 2.0.0](https://semver.org/spec/v2.0.0.html) versions and [`node-semver`](https://github.com/npm/node-semver) version ranges.

- **Focus on functionality and performance.** I will make sure to implement any common manipulations with semantic versions, and I will microoptimize the hell out of everything! [See the benchmarks](./Chasm.SemanticVersioning.Benchmarks#readme). `SemanticVersion`, `SemverPreRelease`, `SemverOptions`, `SemanticVersionBuilder`, `SemverComparer`, `SemverComparison`.

- **Implements `node-semver`'s version ranges.** Notably, advanced comparators and wildcards (`^1.2.x`, `~5.3`) are preserved as is, instead of being desugared into primitives like in all other libraries. That allows to interpret and manipulate version ranges more precisely. `VersionRange`, `ComparatorSet`, `Comparator`, `PartialVersion`, `PartialComponent`, `AdvancedComparator`, `PrimitiveComparator`, `CaretComparator`, `HyphenRangeComparator`, `TildeComparator`, `XRangeComparator`.

- **Default comparison ignores build metadata.** I think it's more correct to have the default comparison be compliant with SemVer's specification. You can still do metadata-sensitive comparison using a custom comparer, if you want - `SemverComparer.IncludeBuild`.

- **.NET-style documentation.** Written in the style of `System` namespace docs. I don't know if it's worth advertising, but I really like how descriptive and consistent it is, so I thought I should mention that.

See more information and examples in [the project's README](./Chasm.SemanticVersioning#readme).

## [Chasm.Collections](./Chasm.Collections#readme)

Provides various collection-related extension and utility methods:
- `ArrayExtensions` - extension versions of most static methods in the `Array` class, and some more.
- `CollectionExtensions` - some extensions for lists of tuples and key-value pairs.
- `EnumerableExtensions` - a couple of extensions for enumerables.
- Methods for returning empty read-only collections/enumerators/dictionaries (`ReadOnlyCollection.Empty<T>()`).

## [Chasm.Utilities](./Chasm.Utilities#readme)

Provides various utility types and methods:
- `Util.Fail`, `Util.Catch`, `Util.Is`, `Util.With` utility methods for shortening common branch code.
- `DelegateDisposable` - a class for quickly and easily creating custom disposable objects.

## [Chasm.Formatting](./Chasm.Formatting#readme)

Provides various formatting and parsing utility types and methods:
- `SpanBuilder` - for formatting complex deep objects without unnecessary memory allocations.
- `SpanParser` - for fluently and efficiently parsing text.
