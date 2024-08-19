
## [Chasm.SemanticVersioning](./Chasm.SemanticVersioning#readme)

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.SemanticVersioning)](https://www.nuget.org/packages/Chasm.SemanticVersioning/)
[![Open issues](https://img.shields.io/github/issues/Chasmical/Chasm)](https://github.com/Chasmical/Chasm/issues)
[![Open PRs](https://img.shields.io/github/issues-pr/Chasmical/Chasm)](https://github.com/Chasmical/Chasm/pulls)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](./LICENSE)

A library designed for efficiently working with [SemVer 2.0.0](https://semver.org/spec/v2.0.0.html) versions and [`node-semver`](https://github.com/npm/node-semver) version ranges.

- **Focus on functionality and performance.** I will make sure to implement any common manipulations with semantic versions, and I will microoptimize the hell out of everything! [See the benchmarks](./Chasm.SemanticVersioning.Benchmarks#readme). `SemanticVersion`, `SemverPreRelease`, `SemverOptions`, `SemanticVersionBuilder`, `SemverComparer`, `SemverComparison`.

- **Implements `node-semver`'s version ranges.** Notably, advanced comparators and wildcards (`^1.2.x`, `~5.3`) are preserved as is, instead of being desugared into primitives like in all other libraries. That allows to interpret and manipulate version ranges more precisely. `VersionRange`, `ComparatorSet`, `Comparator`, `PartialVersion`, `PartialComponent`, `PrimitiveComparator`, `AdvancedComparator`, `CaretComparator`, `HyphenRangeComparator`, `TildeComparator`, `XRangeComparator`.

- **Operations with version ranges.** Now this is definitely a unique feature - this library defines operations for `Comparator`, `ComparatorSet` and `VersionRange`. You can complement (`~`), union (`|`), intersect (`&`) and desugar ranges. Soon you'll also be able to normalize, transform and minimize ranges.

- **Default comparison ignores build metadata.** I think it's more correct to have the default comparison be compliant with SemVer's specification. You can still do metadata-sensitive comparison using a custom comparer, if you want - `SemverComparer.IncludeBuild`.

- **Out-of-the-box serialization support.** Supports serialization/deserialization with `Newtonsoft.Json`, `System.Text.Json` and `System.Xml` (and any libraries using `TypeConverter`s) with no extra configuration needed.

- **Thoroughly tested, 100% code coverage.** As of v2.5.1, all of the library's functionality has been covered by tests. Of course, like with all tests, there may be some really obscure edge cases that haven't been covered, but it's really unlikely you'll ever run into them. And if you do, [file an issue here](https://github.com/Chasmical/Chasm/issues)!

- **.NET-style documentation.** Written in the style of `System` namespace docs. I don't know if it's worth advertising, but I really like how descriptive and consistent it is, so I thought I should mention that.

See more information and examples in [the project's README](./Chasm.SemanticVersioning#readme).



## [Chasm.Collections](./Chasm.Collections#readme)

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Collections)](https://www.nuget.org/packages/Chasm.Collections/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](./LICENSE)

Provides various collection-related extension and utility methods:
- `ArrayExtensions` - extension versions of most static methods in the `Array` class, and some more.
- `CollectionExtensions` - some extensions for lists of tuples and key-value pairs.
- `EnumerableExtensions` - a couple of extensions for enumerables.
- Methods for returning empty read-only collections/enumerators/dictionaries (`ReadOnlyCollection.Empty<T>()`).



## [Chasm.Utilities](./Chasm.Utilities#readme)

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Utilities)](https://www.nuget.org/packages/Chasm.Utilities/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](./LICENSE)

Provides various utility types and methods:
- `Util.Fail`, `Util.Catch`, `Util.Is`, `Util.With` utility methods for shortening common branch code.
- `DelegateDisposable` - a class for quickly and easily creating custom disposable objects.



## [Chasm.Formatting](./Chasm.Formatting#readme)

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Formatting)](https://www.nuget.org/packages/Chasm.Formatting/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](./LICENSE)

Provides various formatting and parsing utility types and methods:
- `SpanBuilder` - for formatting complex deep objects without unnecessary memory allocations.
- `SpanParser` - for fluently and efficiently parsing text.
