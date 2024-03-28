# Chasm.SemanticVersioning

You're probably wondering "Why should I use this library instead of any other more popular alternatives?". Well, here's a quick overview...

- **Focus on functionality and performance.** I will make sure to implement any common manipulations with semantic versions, and I will microoptimize the hell out of everything! *(not as much as diving into assembly code though)* I'll whip up some benchmarks later to show you the difference.

- **Default comparison ignores build metadata.** Feel free to disagree, but that's just my decision. It feels more correct to have the default comparison be compliant with SemVer's specification. You can still do metadata-sensitive comparison using a custom comparer, if you want - `SemverComparer.IncludeBuildMetadata`.

- **Implements `node-semver`'s version ranges.** Notably, advanced comparators and wildcards (`^1.2.x`, `~5.3`) are preserved as is, and after being parsed, they will still output these exact strings. All other libraries desugar the advanced comparators into primitives, and I didn't really like that. I wanted to be able to manipulate version ranges in a more precise fashion.

- **.NET-style documentation.** Written in the style of `System` namespace docs. I don't know if it's worth advertising, but I really like how descriptive and consistent it is, so I thought I should mention that.

- **In active development, lots of plans.** See the to-do list below.



## Table of contents

1. [`SemanticVersion` class](#semanticversion-class)
2. [`SemverPreRelease` struct](#semverprerelease-struct)
3. [`SemverOptions` enum](#semveroptions-enum)
4. [`SemanticVersionBuilder` class](#semanticversionbuilder-class)
5. [`node-semver`'s version ranges](#node-semvers-version-ranges)
6. [Advanced `SemanticVersion` formatting](#advanced-semanticversion-formatting)



## To-do List

### Semantic versions

- [ ] `SemverPreRelease.ParseMultiple/Many` method;
- [ ] Advanced `SemverPreRelease` formatting, maybe?;

### Version ranges

- [x] Classes `PartialVersion`, `VersionRange`, `ComparatorSet`, `Comparator`;
- [x] Primitive version comparators;
- [x] Advanced version comparators;
- [x] `PartialVersion` parsing and formatting;
- [x] Parsing of version ranges and its components;
- [ ] Operators (union `|`, intersection `&`, absolute complement `~`);
- [x] Desugar methods;
- [ ] Simplify methods;
- [ ] IsSubset/Superset methods;
- [ ] Coercing versions?
- [ ] Diffing versions?
- [ ] Decrementing versions?



## `SemanticVersion` class

`SemanticVersion` represents a valid semantic version as per the SemVer 2.0.0 specification.

```cs
var a = SemanticVersion.Parse("1.0.0-alpha.8");
var b = SemanticVersion.Parse("=v 1.02-pre  ", SemverOptions.Loose);

Console.WriteLine($"{a} < {b} = {a < b}");
// 1.0.0-alpha.8 < 1.2-pre = True
```

**Note that the default comparison doesn't account for build metadata!** For build metadata-sensitive comparison, use `SemverComparer.IncludeBuildMetadata`.

```cs
var a = SemanticVersion.Parse("1.2.3-4");
var b = SemanticVersion.Parse("1.2.3-4+BUILD");

Console.WriteLine($"{a} == {b} = {a == b}");
// 1.2.3-4 == 1.2.3-4+BUILD = True

var cmp = SemverComparer.IncludeBuildMetadata;
Console.WriteLine($"{a} === {b} = {cmp.Equals(a, b)}");
// 1.2.3-4 === 1.2.3-4+BUILD = False
```



## `SemverPreRelease` struct

`SemverPreRelease` is a pre-release identifier. Can be implicitly created from strings and ints.

```cs
var pre = new SemverPreRelease[] { "alpha", 0 };
var version = new SemanticVersion(1, 2, 3, pre);

// Or with C# 12's collection expressions:
var version = new SemanticVersion(1, 2, 3, ["alpha", 0]);
```



## `SemverOptions` enum

`SemverOptions` specifies a bunch of different semantic version parsing options.

```cs
var options = SemverOptions.AllowVersionPrefix | SemverOptions.AllowInnerWhite
            | SemverOptions.OptionalMinor | SemverOptions.RemoveEmptyPreReleases;

var version = SemanticVersion.Parse("v2 -. .alpha.", options);
// Parsed as "2.0.0-alpha"
```



### `SemanticVersionBuilder` class

`SemanticVersionBuilder` can be used to manipulate semantic versions step by step.

```cs
var builder = new SemanticVersionBuilder(1, 2, 3);

builder
    .WithPatch(7)
    .AppendPreRelease("alpha")
    .AppendPreRelease(0);

var a = builder.ToVersion(); // 1.2.7-alpha.0

builder.IncrementPatch();

var b = builder.ToVersion(); // 1.2.8

builder.Increment(IncrementType.PreMinor, "beta");

var c = builder.ToVersion(); // 1.3.0-beta.0
```



### `node-semver`'s version ranges

`VersionRange`, `ComparatorSet`, `Comparator` classes provide methods for doing stuff with [`node-semver`](https://github.com/npm/node-semver)'s version ranges. Notably, advanced comparators and wildcards (`^1.2.x`, `~5.3`) are preserved as is, instead of being desugared into primitives like in all other libraries.

```cs
var range = VersionRange.Parse("~5.3");

var a = SemanticVersion.Parse("5.0.1");
var b = SemanticVersion.Parse("5.3.2");
var c = SemanticVersion.Parse("5.5.0");

Console.WriteLine($"{a} satisfies {range}: {range.IsSatisfiedBy(a)}");
Console.WriteLine($"{b} satisfies {range}: {range.IsSatisfiedBy(b)}");
Console.WriteLine($"{c} satisfies {range}: {range.IsSatisfiedBy(c)}");
// 5.0.1 satisfies ~5.3: False
// 5.3.2 satisfies ~5.3: True
// 5.5.0 satisfies ~5.3: False
```



### Advanced `SemanticVersion` formatting

You can change the order of the semantic version components, escape sequences of characters, specify pre-release and build metadata by indices, and etc. The formatting here is pretty powerful and well optimized, but, of course, not universal - there's only so much that you can ask of a simple formatting method...

```cs
var a = SemanticVersion.Parse("7.0.8-beta.5+DEV.09");
Console.WriteLine(a.ToString("'version:' m.p.M-r1.r0 (dd)"));
// version: 0.8.7-5.beta (DEV.09)
```

#### Basic format identifiers
- `M` - major version component.
- `m` - minor version component.
- `p` - patch version component.
- `rr` - all pre-release identifiers.
- `dd` - all build metadata identifiers.

The standard, SemVer 2.0.0, format is `M.m.p-rr+dd`.

#### Optional components
- `mm` - optional minor version component. Omitted, if **both minor and patch** components are zero.
- `pp` - optional patch version component. Omitted if it's zero.

When an optional component is omitted, a separator preceding it is omitted as well. **For example:** Formatting `1.2.0` using `M.mm.pp` yields `1.2.0`, and formatting `2.0.0` - yields `2`.

#### Pre-release identifiers
- `r0`, `r1`, `r2`, … - the first/second/third/… pre-release identifier.
- `r` - the pre-release identifier that comes after the last one specified. **For example:** `r0.r.r.r5.r` would be equivalent to `r0.r1.r2.r5.r6`.
- `rr` - all of the pre-release identifiers that come after the last specified pre-release identifier, separated by `.` dots. **For example:** `r2.rr` would output all pre-releases except for the first two (`r0`, `r1`).

When a specified pre-release identifier doesn't exist, it is omitted, along with a separator preceding it. **For example:** Formatting `1.0.0-alpha.5` using `M-r0.r1.rr` yields `1-alpha.5`, and formatting `1.0.0` - yields `1`.

#### Build metadata identifiers
- `d0`, `d1`, `d2`, … - the first/second/third/… build metadata identifier.
- `d` - the build metadata identifier that comes after the last one specified. **For example:** `d.d2.d.d5` would be equivalent to `d0.d2.d3.d5`.
- `dd` - all of the build metadata identifiers that come after the last specified build metadata identifier, separated by `.` dots. **For example:** `d1.dd` would output all build metadata except for the first one (`d0`).

When a specified build metadata identifier doesn't exist, it is omitted, along with a separator preceding it. Formatting `1.0.0+dev` using `M+d0.d1.dd` yields `1+dev`, and formatting `1.0.0` - yields `1`.

#### Escaping sequences
- `\Majo\r: M` - backslash-escaped characters, which are output as is (without the backslash, of course). The backslash itself can be escaped as well (`\\M`).
- `'map'`, `"Arr!"` - quote-escaped sequence of characters, the contents of which are output as is (`map`, `Arr!`). Note that inside quote-escaped sequences, `\` (backslash) doesn't escape and is instead output as is.

#### Separators
- `.`, `-`, `+`, `_`, ` ` - separator characters. When a specified format identifier is omitted, the separator character preceding it is omitted as well.


