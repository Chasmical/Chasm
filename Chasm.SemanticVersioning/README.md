# Chasm.SemanticVersioning

You're probably wondering "Why should I use this library instead of any other more popular alternatives?". Well, here's a quick overview...

- **Focus on functionality and performance.** I will make sure to implement any common manipulations with semantic versions, and I will microoptimize the hell out of everything! *(not as much as diving into assembly code though)* I'll whip up some benchmarks later to show you the difference.

- **Nice concise naming.** `SemanticVersion`, `SemverPreRelease`, `SemverOptions`. *It's a bit inconsistent with `SemanticVersion` compared to other types, but calling it `SemverVersion` would sound redundant.*

- **.NET-style documentation.** Written in the style of `System` namespace docs. I don't know if it's worth advertising, but I really like how descriptive and consistent it is, so I thought I should mention that.

- **In active development, lots of plans.** See the to-do list below.

## To-do List

### Extra functionality

- [ ] Use more efficient formatting (Chasm.Formatting);
- [ ] Advanced `SemanticVersion` formatting (`M.m.p-rrr+ddd`);
- [ ] `SemanticVersionBuilder` class;
- [ ] `BuildMetadataComparer` class;
- [ ] Advanced `SemverPreRelease` formatting, maybe?;
- [ ] `SemverPreRelease.ParseMultiple/Many` method;
- [ ] Option to ignore empty pre-releases/build metadata during parsing;
- [ ] Option to allow an older version syntax, like `1.2.3beta5`;

### `node-semver` version ranges

- [ ] Classes `PartialVersion`, `VersionRange`, `ComparatorSet`, `Comparator`;
- [ ] Primitive version comparators;
- [ ] Advanced version comparators;
- [ ] `PartialVersion` parsing and formatting;
- [ ] Parsing of version ranges and its components;

## `SemanticVersion`

`SemanticVersion` represents a valid semantic version as per the SemVer 2.0.0 specification.

```cs
var a = SemanticVersion.Parse("1.0.0-alpha.8");
var b = SemanticVersion.Parse("=v 1.2-pre  ", SemverOptions.Loose);

Console.WriteLine($"{a} < {b} = {a < b}");
// 1.0.0-alpha.8 < 1.2-pre = true
```

**Note that the default comparison doesn't account for build metadata!**

## `SemverPreRelease`

`SemverPreRelease` is a pre-release identifier. Can be implicitly created from strings and ints.

```cs
var pre = new SemverPreRelease[] { "alpha", 0 };
var version = new SemanticVersion(1, 2, 3, pre);
```

## `SemverOptions`

`SemverOptions` specifies a bunch of different semantic version parsing options.

```cs
var options = SemverOptions.AllowVersionPrefix | SemverOptions.AllowInnerWhite | SemverOptions.OptionalMinor;
var version = SemanticVersion.Parse("v2 - alpha");
// Parsed as "2.0.0-alpha"
```
