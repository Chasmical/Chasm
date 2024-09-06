# Chasm.SemanticVersioning Changelog

### v2.7.3 (next)
- ⚡️ Microoptimized IL code size of `AdvancedComparator`'s and `HyphenRangeComparator`'s constructors;

### v2.7.2
- 📝 Added missing XML docs;
- ♻️ Removed some unnecessary code in utility methods;
- ♻️ Refactored conversion operators to use static property instances when possible;

### v2.7.1
- ✨ Implemented `IEqualityComparer<VersionRange>` methods in `SemverComparer`;
- ✨ Implemented `IEqualityComparer<ComparatorSet>` methods in `SemverComparer`;
- ✨ Implemented `IEqualityComparer<Comparator>` methods in `SemverComparer`;
- ✨ Added `SemverComparer.Compare(VersionRange?, VersionRange?)`;
- ✨ Added `SemverComparer.Equals(VersionRange?, VersionRange?)`;
- ✨ Added `SemverComparer.GetHashCode(VersionRange?)`;
- ✨ Added `SemverComparer.Compare(ComparatorSet?, ComparatorSet?)`;
- ✨ Added `SemverComparer.Equals(ComparatorSet?, ComparatorSet?)`;
- ✨ Added `SemverComparer.GetHashCode(ComparatorSet?)`;
- ✨ Added `SemverComparer.Compare(Comparator?, Comparator?)`;
- ✨ Added `SemverComparer.Equals(Comparator?, Comparator?)`;
- ✨ Added `SemverComparer.GetHashCode(Comparator?)`;
- 🐛 Now `SemverComparer.FromComparison` doesn't throw on `SemverComparison` combinations that don't have an associated static property instance;
- ♻️ Slightly refactored version range operations, removed resugaring code for now;

### v2.7.0
- 💥 Invalid hyphen ranges (e.g. `3.0.0 - 1.0.0`) now desugar to `<0.0.0-0`;
- ✨ Added `static operator ~(Comparator)`;
- ✨ Added `static operator &(Comparator, Comparator)`;
- ✨ Added `static operator |(Comparator, Comparator)`;
- ✨ Added `static operator ~(ComparatorSet)`;
- ✨ Added `static operator &(ComparatorSet, ComparatorSet)`;
- ✨ Added `static operator |(ComparatorSet, ComparatorSet)`;
- ✨ Added `static operator ~(VersionRange)`;
- ✨ Added `static operator &(VersionRange, VersionRange)`;
- ✨ Added `static operator |(VersionRange, VersionRange)`;
- ✨ Added `ComparatorSet.Contains(ComparatorSet)`;
- ✨ Added `ComparatorSet.Intersects(ComparatorSet)`;
- ✨ Added `ComparatorSet.Touches(ComparatorSet)`;
- 🩹 Added `[Pure]` attribute to `protected Comparator.IsSatisfiedByCore(SemanticVersion)`;

### v2.6.1
- 🐛 Added missing `TypeConverter`s for .NET Framework targets;

### v2.6.0
- ✨ Implemented `IEquatable<T>` methods in types: `Comparator`, `PrimitiveComparator`, `CaretComparator`, `HyphenRangeComparator`, `TildeComparator`, `XRangeComparator`;
- ✨ Implemented `IEqualityOperators<T,T,bool>` methods in `Comparator`;
- ✨ Added `abstract Comparator.Equals(object? obj)`;
- ✨ Added `abstract Comparator.GetHashCode()`;
- ✨ Added `static operator ==(Comparator?, Comparator?)`;
- ✨ Added `static operator !=(Comparator?, Comparator?)`;
- ✨ Added `PrimitiveComparator.Equals(PrimitiveComparator?)`;
- ✨ Added `PrimitiveComparator.Equals(object?)`;
- ✨ Added `PrimitiveComparator.GetHashCode()`;
- ✨ Added `CaretComparator.Equals(CaretComparator?)`;
- ✨ Added `CaretComparator.Equals(object?)`;
- ✨ Added `CaretComparator.GetHashCode()`;
- ✨ Added `HyphenRangeComparator.Equals(HyphenRangeComparator?)`;
- ✨ Added `HyphenRangeComparator.Equals(object?)`;
- ✨ Added `HyphenRangeComparator.GetHashCode()`;
- ✨ Added `TildeComparator.Equals(TildeComparator?)`;
- ✨ Added `TildeComparator.Equals(object?)`;
- ✨ Added `TildeComparator.GetHashCode()`;
- ✨ Added `XRangeComparator.Equals(XRangeComparator?)`;
- ✨ Added `XRangeComparator.Equals(object?)`;
- ✨ Added `XRangeComparator.GetHashCode()`;
- ♻️ Refactored shimmed attributes;

### v2.5.2
- 🧩 Added `net5.0`, `netcoreapp2.0`, `netstandard2.0` and `net461` targets, and removed `netcoreapp2.1` target. Now targets: `net8.0`, `net7.0`, `net6.0`, `net5.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netstandard2.1`, `netstandard2.0`, `net461`;

### v2.5.1
- ✨ Added `VersionRange.GetEnumerator()`;
- ✨ Added `VersionRange.this[int]`;
- ✨ Added `ComparatorSet.GetEnumerator()`;
- ✨ Added `ComparatorSet.this[int]`;

### v2.5.0
- ✨ Added `TypeConverter` support for types: `SemanticVersion`, `SemverPreRelease`, `PartialVersion`, `PartialComponent`, `VersionRange`;
- ✨ Added `JsonConverter` support for types: `SemanticVersion`, `SemverPreRelease`, `PartialVersion`, `PartialComponent`, `VersionRange`;
- ✨ Implemented `IXmlSerializable` in types: `SemanticVersion`, `SemverPreRelease`, `PartialVersion`, `PartialComponent`, `VersionRange`;
- 🧩 Added `netcoreapp3.0` target. Now targets: `net8.0`, `net7.0`, `net6.0`, `netcoreapp3.0`, `netcoreapp2.1`, `netstandard2.1`;

### v2.4.1
- 🐛 Fixed trimming components and pre-releases on partial versions in comparators;
- 🔥 Removed unused `ISpanBuildable` implementations in `SemverPreRelease` and `PartialComponent`;
- ✏️ Fixed typo in `SemverComparer.Compare(object?, object?)` exception message;
- ⚡️ Reduced the amount of referenced external types and methods;
- ⚡️ Micro-optimized the size and performance of some methods;

### v2.4.0
- ⚠️ Marked `BuildMetadataComparer` as obsolete, use the new `SemverComparer` instead;
<!-- -->
- ✨ Added `SemanticVersion(Version)`;
- ✨ Added `explicit SemanticVersion(Version)`;
- ✨ Added `explicit Version(SemanticVersion)`;
<!-- -->
- ✨ Added `SemverOptions.AllowExtraWildcards`;
<!-- -->
- ✨ Added `readonly struct PartialComponent : IEquatable<PartialComponent>, IComparable, IComparable<PartialComponent>, IComparisonOperator<PartialComponent, PartialComponent, bool>, ISpanParsable<PartialComponent>`;
- ✨ Added `PartialComponent(int)`;
- ✨ Added `PartialComponent(char)`;
- ✨ Added `implicit PartialComponent(int)`;
- ✨ Added `implicit PartialComponent(char)`;
- ✨ Added `implicit PartialComponent(int?)`;
- ✨ Added `explicit int(PartialComponent)`;
- ✨ Added `explicit char(PartialComponent)`;
- ✨ Added `explicit int?(PartialComponent)`;
- ✨ Added `PartialComponent.IsNumeric`;
- ✨ Added `PartialComponent.IsWildcard`;
- ✨ Added `PartialComponent.IsOmitted`;
- ✨ Added `PartialComponent.AsNumber`;
- ✨ Added `PartialComponent.AsWildcard`;
- ✨ Added `static PartialComponent.Zero`;
- ✨ Added `static PartialComponent.LowerX`;
- ✨ Added `static PartialComponent.UpperX`;
- ✨ Added `static PartialComponent.Star`;
- ✨ Added `static PartialComponent.Omitted`;
- ✨ Added `PartialComponent.Equals(PartialComponent)`;
- ✨ Added `PartialComponent.Equals(object?)`;
- ✨ Added `PartialComponent.GetHashCode()`;
- ✨ Added `PartialComponent.CompareTo(PartialComponent)`;
- ✨ Added `static operator ==(PartialComponent, PartialComponent)`;
- ✨ Added `static operator !=(PartialComponent, PartialComponent)`;
- ✨ Added `static operator >(PartialComponent, PartialComponent)`;
- ✨ Added `static operator <(PartialComponent, PartialComponent)`;
- ✨ Added `static operator >=(PartialComponent, PartialComponent)`;
- ✨ Added `static operator <=(PartialComponent, PartialComponent)`;
- ✨ Added `PartialComponent.ToString()`;
- ✨ Added `static PartialComponent.Parse(char)`;
- ✨ Added `static PartialComponent.TryParse(char, out PartialComponent)`;
- ✨ Added `static PartialComponent.Parse(string)`;
- ✨ Added `static PartialComponent.Parse(ReadOnlySpan<char>)`;
- ✨ Added `static PartialComponent.TryParse(string?, out PartialComponent)`;
- ✨ Added `static PartialComponent.TryParse(ReadOnlySpan<char>, out PartialComponent)`;
- ✨ Added `static PartialComponent.Parse(string, SemverOptions)`;
- ✨ Added `static PartialComponent.Parse(ReadOnlySpan<char>, SemverOptions)`;
- ✨ Added `static PartialComponent.TryParse(string?, SemverOptions, out PartialComponent)`;
- ✨ Added `static PartialComponent.TryParse(ReadOnlySpan<char>, SemverOptions, out PartialComponent)`;
<!-- -->
- ✨ Added `sealed class PartialVersion : IEquatable<PartialVersion>, IComparable, IComparable<PartialVersion>, IEqualityOperators<PartialVersion, PartialVersion, bool>, ISpanParsable<PartialVersion>`;
- ✨ Added `PartialVersion.Major`;
- ✨ Added `PartialVersion.Minor`;
- ✨ Added `PartialVersion.Patch`;
- ✨ Added `PartialVersion.PreReleases`;
- ✨ Added `PartialVersion.BuildMetadata`;
- ✨ Added `PartialVersion(PartialComponent)`;
- ✨ Added `PartialVersion(PartialComponent, PartialComponent)`;
- ✨ Added `PartialVersion(PartialComponent, PartialComponent, PartialComponent)`;
- ✨ Added `PartialVersion(PartialComponent, PartialComponent, PartialComponent, IEnumerable<SemverPreRelease>?)`;
- ✨ Added `PartialVersion(PartialComponent, PartialComponent, PartialComponent, IEnumerable<SemverPreRelease>?, IEnumerable<string>?)`;
- ✨ Added `PartialVersion(Version)`;
- ✨ Added `PartialVersion(SemanticVersion)`;
- ✨ Added `explicit PartialVersion(Version)`;
- ✨ Added `implicit PartialVersion(SemanticVersion)`;
- ✨ Added `explicit Version(PartialVersion)`;
- ✨ Added `explicit SemanticVersion(PartialVersion)`;
- ✨ Added `PartialVersion.IsPartial`;
- ✨ Added `PartialVersion.IsPreRelease`;
- ✨ Added `PartialVersion.HasBuildMetadata`;
- ✨ Added `PartialVersion.GetPreReleases()`;
- ✨ Added `PartialVersion.GetBuildMetadata()`;
- ✨ Added `static PartialVersion.OneStar`;
- ✨ Added `PartialVersion.Equals(PartialVersion?)`;
- ✨ Added `PartialVersion.Equals(object?)`;
- ✨ Added `PartialVersion.GetHashCode()`;
- ✨ Added `PartialVersion.CompareTo(PartialVersion?)`;
- ✨ Added `static operator ==(PartialVersion?, PartialVersion?)`;
- ✨ Added `static operator !=(PartialVersion?, PartialVersion?)`;
- ✨ Added `PartialVersion.ToString()`;
- ✨ Added `static PartialVersion.Parse(string)`;
- ✨ Added `static PartialVersion.Parse(ReadOnlySpan<char>)`;
- ✨ Added `static PartialVersion.TryParse(string?, out PartialVersion?)`;
- ✨ Added `static PartialVersion.TryParse(ReadOnlySpan<char>, out PartialVersion?)`;
- ✨ Added `static PartialVersion.Parse(string, SemverOptions)`;
- ✨ Added `static PartialVersion.Parse(ReadOnlySpan<char>, SemverOptions)`;
- ✨ Added `static PartialVersion.TryParse(string?, SemverOptions, out PartialVersion?)`;
- ✨ Added `static PartialVersion.TryParse(ReadOnlySpan<char>, SemverOptions, out PartialVersion?)`;
<!-- -->
- ✨ Added `sealed class VersionRange : ISpanParsable<VersionRange>`;
- ✨ Added `VersionRange.ComparatorSets`;
- ✨ Added `VersionRange(ComparatorSet)`;
- ✨ Added `VersionRange(ComparatorSet, params ComparatorSet[]?)`;
- ✨ Added `VersionRange(IEnumerable<ComparatorSet>)`;
- ✨ Added `implicit VersionRange?(Comparator?)`;
- ✨ Added `implicit VersionRange?(ComparatorSet?)`;
- ✨ Added `VersionRange.IsSugared`;
- ✨ Added `VersionRange.GetComparatorSets()`;
- ✨ Added `VersionRange.IsSatisfiedBy(SemanticVersion?)`;
- ✨ Added `VersionRange.IsSatisfiedBy(SemanticVersion?, bool)`;
- ✨ Added `VersionRange.Desugar()`;
- ✨ Added `static VersionRange.None`;
- ✨ Added `static VersionRange.All`;
- ✨ Added `VersionRange.ToString()`;
- ✨ Added `static VersionRange.Parse(string)`;
- ✨ Added `static VersionRange.Parse(ReadOnlySpan<char>)`;
- ✨ Added `static VersionRange.TryParse(string?, out VersionRange?)`;
- ✨ Added `static VersionRange.TryParse(ReadOnlySpan<char>, out VersionRange?)`;
- ✨ Added `static VersionRange.Parse(string, SemverOptions)`;
- ✨ Added `static VersionRange.Parse(ReadOnlySpan<char>, SemverOptions)`;
- ✨ Added `static VersionRange.TryParse(string?, SemverOptions, out VersionRange?)`;
- ✨ Added `static VersionRange.TryParse(ReadOnlySpan<char>, SemverOptions, out VersionRange?)`;
<!-- -->
- ✨ Added `sealed class ComparatorSet`;
- ✨ Added `ComparatorSet.Comparators`;
- ✨ Added `ComparatorSet(params Comparator[]?)`;
- ✨ Added `ComparatorSet(IEnumerable<Comparator>?)`;
- ✨ Added `implicit ComparatorSet?(Comparator?)`;
- ✨ Added `ComparatorSet.IsSugared`;
- ✨ Added `ComparatorSet.GetComparators()`;
- ✨ Added `ComparatorSet.IsSatisfiedBy(SemanticVersion?)`;
- ✨ Added `ComparatorSet.IsSatisfiedBy(SemanticVersion?, bool)`;
- ✨ Added `ComparatorSet.Desugar()`;
- ✨ Added `static ComparatorSet.None`;
- ✨ Added `static ComparatorSet.All`;
- ✨ Added `ComparatorSet.ToString()`;
<!-- -->
- ✨ Added `abstract class Comparator`;
- ✨ Added `Comparator.IsPrimitive`;
- ✨ Added `Comparator.IsAdvanced`;
- ✨ Added `abstract Comparator.CanMatchPreRelease(int, int, int)`;
- ✨ Added `static Comparator.CanMatchPreRelease(SemanticVersion?, int, int, int)`;
- ✨ Added `Comparator.IsSatisfiedBy(SemanticVersion?)`;
- ✨ Added `Comparator.IsSatisfiedBy(SemanticVersion?, bool)`;
- ✨ Added `abstract Comparator.IsSatisfiedByCore(SemanticVersion)`;
- ✨ Added `abstract Comparator.CalculateLength()`;
- ✨ Added `abstract Comparator.BuildString(ref SpanBuilder)`;
- ✨ Added `Comparator.ToString()`;
<!-- -->
- ✨ Added `sealed class PrimitiveComparator : Comparator`;
- ✨ Added `PrimitiveComparator.IsPrimitive` (obsolete, const value);
- ✨ Added `PrimitiveComparator.IsAdvanced` (obsolete, const value);
- ✨ Added `PrimitiveComparator.Operand`;
- ✨ Added `PrimitiveComparator.Operator`;
- ✨ Added `PrimitiveComparator(SemanticVersion)`;
- ✨ Added `PrimitiveComparator(SemanticVersion, PrimitiveOperator)`;
- ✨ Added `static PrimitiveComparator.ImplicitEqual(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.Equal(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.GreaterThan(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.LessThan(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.GreaterThanOrEqual(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.LessThanOrEqual(SemanticVersion)`;
- ✨ Added `static PrimitiveComparator.None`;
- ✨ Added `static PrimitiveComparator.All`;
<!-- -->
- ✨ Added `enum PrimitiveOperator : byte`;
- ✨ Added `PrimitiveOperator.ImplicitEqual`;
- ✨ Added `PrimitiveOperator.Equal`;
- ✨ Added `PrimitiveOperator.GreaterThan`;
- ✨ Added `PrimitiveOperator.LessThan`;
- ✨ Added `PrimitiveOperator.GreaterThanOrEqual`;
- ✨ Added `PrimitiveOperator.LessThanOrEqual`;
<!-- -->
- ✨ Added `abstract class AdvancedComparator : Comparator`;
- ✨ Added `AdvancedComparator.IsPrimitive` (obsolete, const value);
- ✨ Added `AdvancedComparator.IsAdvanced` (obsolete, const value);
- ✨ Added `AdvancedComparator.Operand`;
- ✨ Added `AdvancedComparator(PartialVersion)`;
- ✨ Added `AdvancedComparator.ToPrimitives()`;
- ✨ Added `abstract AdvancedComparator.ConvertToPrimitives()`;
<!-- -->
- ✨ Added `sealed class CaretComparator : AdvancedComparator`;
- ✨ Added `CaretComparator(PartialVersion)`;
<!-- -->
- ✨ Added `sealed class TildeComparator : AdvancedComparator`;
- ✨ Added `TildeComparator(PartialVersion)`;
<!-- -->
- ✨ Added `sealed class HyphenRangeComparator : AdvancedComparator`;
- ✨ Added `HyphenRangeComparator.From`;
- ✨ Added `HyphenRangeComparator.To`;
- ✨ Added `HyphenRangeComparator.Operand` (obsolete, use `From`);
- ✨ Added `HyphenRangeComparator(PartialVersion, PartialVersion)`;
<!-- -->
- ✨ Added `sealed class XRangeComparator : AdvancedComparator`;
- ✨ Added `XRangeComparator(PartialVersion)`;
- ✨ Added `XRangeComparator(PrimitiveComparator)`;
- ✨ Added `XRangeComparator(PartialVersion, PrimitiveOperator)`;
- ✨ Added `implicit XRangeComparator?(PartialVersion?)`;
- ✨ Added `implicit XRangeComparator?(PrimitiveComparator?)`;
- ✨ Added `explicit PrimitiveComparator?(XRangeComparator?)`;
- ✨ Added `static XRangeComparator.ImplicitEqual(PartialVersion)`;
- ✨ Added `static XRangeComparator.Equal(PartialVersion)`;
- ✨ Added `static XRangeComparator.GreaterThan(PartialVersion)`;
- ✨ Added `static XRangeComparator.LessThan(PartialVersion)`;
- ✨ Added `static XRangeComparator.GreaterThanOrEqual(PartialVersion)`;
- ✨ Added `static XRangeComparator.LessThanOrEqual(PartialVersion)`;
- ✨ Added `static XRangeComparator.All`;
<!-- -->
- ✨ Added `sealed class SemverComparer : IComparer, IEqualityComparer, IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>, IComparer<PartialVersion>, IEqualityComparer<PartialVersion>, IComparer<PartialComponent>, IEqualityComparer<PartialComponent>`;
- ✨ Added `SemverComparer.Compare(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `SemverComparer.Equals(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `SemverComparer.GetHashCode(SemanticVersion?)`;
- ✨ Added `SemverComparer.Compare(PartialVersion?, PartialVersion?)`;
- ✨ Added `SemverComparer.Equals(PartialVersion?, PartialVersion?)`;
- ✨ Added `SemverComparer.GetHashCode(PartialVersion?)`;
- ✨ Added `SemverComparer.Compare(PartialComponent, PartialComponent)`;
- ✨ Added `SemverComparer.Equals(PartialComponent, PartialComponent)`;
- ✨ Added `SemverComparer.GetHashCode(PartialComponent)`;
- ✨ Added `static SemverComparer.FromComparison(SemverComparison)`;
- ✨ Added `static SemverComparer.Default`;
- ✨ Added `static SemverComparer.IncludeBuild`;
- ✨ Added `static SemverComparer.DiffWildcards`;
- ✨ Added `static SemverComparer.Exact`;
<!-- -->
- ✨ Added `enum SemverComparison : byte`;
- ✨ Added `SemverComparison.Default`;
- ✨ Added `SemverComparison.IncludeBuild`;
- ✨ Added `SemverComparison.DiffWildcards`;
- ✨ Added `SemverComparison.DiffEquality`;
- ✨ Added `SemverComparison.Exact`;
<!-- -->
- 🐛 Fixed `SemanticVersionBuilder`'s incrementing with alphanumeric pre-releases;
- 🩹 Changed `SemanticVersionBuilder.Increment` methods to throw `InvalidEnumArgumentException`;
- 🩹 Added missing `[Pure]` attributes to `SemanticVersion` formatting methods;
- ♻️ Refactored `SemanticVersion` parsing a bit;
- ⚡️ Improved performance of `SemanticVersion.GetHashCode()`;
- ⚡️ Improved performance of `SemverPreRelease` in general;
- ⚡️ Improved performance of `SemverPreRelease` parsing methods;
- ⚡️ Improved performance of `SemanticVersionBuilder`'s pre-release incrementing;
- ⚡️ Micro-optimized the size of `SemanticVersion` and `SemverPreRelease` operators;
- 📄 Updated license information;

### v2.3.0
- ✨ Implemented advanced semantic version formatting;
- ✨ Implemented `ISpanFormattable` and `IFormattable` methods in `SemanticVersion`;
- ✨ Added `SemanticVersion.ToString(string?)`;
- ✨ Added `SemanticVersion.ToString(ReadOnlySpan<char>)`;
- ✨ Added `SemanticVersion.TryFormat(Span<char>, out int)`;
- ✨ Added `SemanticVersion.TryFormat(Span<char>, out int, ReadOnlySpan<char>)`;

### v2.2.0
- ✨ Added `sealed class SemanticVersionBuilder`;
- ✨ Added `SemanticVersionBuilder.Major`;
- ✨ Added `SemanticVersionBuilder.Minor`;
- ✨ Added `SemanticVersionBuilder.Patch`;
- ✨ Added `SemanticVersionBuilder.PreReleases`;
- ✨ Added `SemanticVersionBuilder.BuildMetadata`;
- ✨ Added `SemanticVersionBuilder()`;
- ✨ Added `SemanticVersionBuilder(int, int, int)`;
- ✨ Added `SemanticVersionBuilder(int, int, int, IEnumerable<SemverPreRelease>?)`;
- ✨ Added `SemanticVersionBuilder(int, int, int, IEnumerable<SemverPreRelease>?, IEnumerable<string>?)`;
- ✨ Added `SemanticVersionBuilder(SemanticVersion)`;
- ✨ Added `SemanticVersionBuilder.WithMajor(int)`;
- ✨ Added `SemanticVersionBuilder.WithMinor(int)`;
- ✨ Added `SemanticVersionBuilder.WithPatch(int)`;
- ✨ Added `SemanticVersionBuilder.AppendPreRelease(SemverPreRelease)`;
- ✨ Added `SemanticVersionBuilder.ClearPreReleases()`;
- ✨ Added `SemanticVersionBuilder.AppendBuildMetadata(string)`;
- ✨ Added `SemanticVersionBuilder.ClearBuildMetadata()`;
- ✨ Added `SemanticVersionBuilder.ToVersion()`;
- ✨ Added `SemanticVersionBuilder.ToString()`;
<!-- -->
- ✨ Added `sealed class SemanticVersionBuilder.PreReleaseCollection : Collection<SemverPreRelease>`;
- ✨ Added `SemanticVersionBuilder.PreReleaseCollection(SemanticVersionBuilder)`;
<!-- -->
- ✨ Added `sealed class SemanticVersionBuilder.BuildMetadataCollection : Collection<string>`;
- ✨ Added `SemanticVersionBuilder.BuildMetadataCollection(SemanticVersionBuilder)`;
<!-- -->
- ✨ Added `SemanticVersionBuilder.IncrementMajor()`;
- ✨ Added `SemanticVersionBuilder.IncrementMinor()`;
- ✨ Added `SemanticVersionBuilder.IncrementPatch()`;
- ✨ Added `SemanticVersionBuilder.IncrementPreMajor()`;
- ✨ Added `SemanticVersionBuilder.IncrementPreMajor(SemverPreRelease)`;
- ✨ Added `SemanticVersionBuilder.IncrementPreMinor()`;
- ✨ Added `SemanticVersionBuilder.IncrementPreMinor(SemverPreRelease)`;
- ✨ Added `SemanticVersionBuilder.IncrementPrePatch()`;
- ✨ Added `SemanticVersionBuilder.IncrementPrePatch(SemverPreRelease)`;
- ✨ Added `SemanticVersionBuilder.IncrementPreRelease()`;
- ✨ Added `SemanticVersionBuilder.IncrementPreRelease(SemverPreRelease)`;
- ✨ Added `SemanticVersionBuilder.Increment(IncrementType)`;
- ✨ Added `SemanticVersionBuilder.Increment(IncrementType, SemverPreRelease)`;
<!-- -->
- ✨ Added `enum IncrementType : byte`;
- ✨ Added `IncrementType.None`;
- ✨ Added `IncrementType.Major`;
- ✨ Added `IncrementType.Minor`;
- ✨ Added `IncrementType.Patch`;
- ✨ Added `IncrementType.PreMajor`;
- ✨ Added `IncrementType.PreMinor`;
- ✨ Added `IncrementType.PrePatch`;
- ✨ Added `IncrementType.PreRelease`;
<!-- -->
- ⚡️ Significantly improved `SemanticVersion` formatting performance;

### v2.1.0
- ✨ Added `sealed class BuildMetadataComparer : IComparer, IEqualityComparer, IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>`;
- ✨ Added `static BuildMetadataComparer.Instance`;
- ✨ Added `BuildMetadataComparer.Compare(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `BuildMetadataComparer.Equals(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `BuildMetadataComparer.GetHashCode(SemanticVersion?)`;
<!-- -->
- ✨ Added `SemverOptions.OptionalPreReleaseSeparator`;
- ✨ Added `SemverOptions.RemoveEmptyPreReleases`;
- ✨ Added `SemverOptions.RemoveEmptyBuildMetadata`;
<!-- -->
- ✨ Implemented the above options in `SemanticVersion` parsing methods;

### v2.0.0
- 🧩 Targets: `net8.0`, `net7.0`, `net6.0`, `netcoreapp2.1`, `netstandard2.1`;
<!-- -->
- ✨ Added `sealed class SemanticVersion : IEquatable<SemanticVersion>, IComparable, IComparable<SemanticVersion>, IComparisonOperators<SemanticVersion, SemanticVersion, bool>, IMinMaxValue<SemanticVersion>, ISpanParsable<SemanticVersion>`;
- ✨ Added `SemanticVersion(int, int, int)`;
- ✨ Added `SemanticVersion(int, int, int, IEnumerable<SemverPreRelease>?)`;
- ✨ Added `SemanticVersion(int, int, int, IEnumerable<SemverPreRelease>?, IEnumerable<string>?)`;
- ✨ Added `SemanticVersion.Major`;
- ✨ Added `SemanticVersion.Minor`;
- ✨ Added `SemanticVersion.Patch`;
- ✨ Added `SemanticVersion.PreReleases`;
- ✨ Added `SemanticVersion.BuildMetadata`;
- ✨ Added `SemanticVersion.IsStable`;
- ✨ Added `SemanticVersion.IsPreRelease`;
- ✨ Added `SemanticVersion.HasBuildMetadata`;
- ✨ Added `SemanticVersion.GetPreReleases()`;
- ✨ Added `SemanticVersion.GetBuildMetadata()`;
- ✨ Added `static SemanticVersion.MinValue`;
- ✨ Added `static SemanticVersion.MaxValue`;
- ✨ Added `SemanticVersion.Equals(SemanticVersion?)`;
- ✨ Added `SemanticVersion.Equals(object?)`;
- ✨ Added `SemanticVersion.GetHashCode()`;
- ✨ Added `SemanticVersion.CompareTo(SemanticVersion?)`;
- ✨ Added `static operator ==(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `static operator !=(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `static operator >(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `static operator <(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `static operator >=(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `static operator <=(SemanticVersion?, SemanticVersion?)`;
- ✨ Added `SemanticVersion.ToString()`;
- ✨ Added `static SemanticVersion.Parse(string)`;
- ✨ Added `static SemanticVersion.Parse(ReadOnlySpan<char>)`;
- ✨ Added `static SemanticVersion.TryParse(string, out SemanticVersion?)`;
- ✨ Added `static SemanticVersion.TryParse(ReadOnlySpan<char>, out SemanticVersion?)`;
- ✨ Added `static SemanticVersion.Parse(string, SemverOptions)`;
- ✨ Added `static SemanticVersion.Parse(ReadOnlySpan<char>, SemverOptions)`;
- ✨ Added `static SemanticVersion.TryParse(string, SemverOptions, out SemanticVersion?)`;
- ✨ Added `static SemanticVersion.TryParse(ReadOnlySpan<char>, SemverOptions, out SemanticVersion?)`;
<!-- -->
- ✨ Added `enum SemverOptions`;
- ✨ Added `SemverOptions.Strict`;
- ✨ Added `SemverOptions.AllowLeadingZeroes`;
- ✨ Added `SemverOptions.AllowEqualsPrefix`;
- ✨ Added `SemverOptions.AllowVersionPrefix`;
- ✨ Added `SemverOptions.AllowLeadingWhite`;
- ✨ Added `SemverOptions.AllowTrailingWhite`;
- ✨ Added `SemverOptions.AllowInnerWhite`;
- ✨ Added `SemverOptions.OptionalMinor`;
- ✨ Added `SemverOptions.OptionalPatch`;
- ✨ Added `SemverOptions.Loose`;
<!-- -->
- ✨ Added `readonly struct SemverPreRelease : IEquatable<SemverPreRelease>, IComparable, IComparable<SemverPreRelease>, IComparisonOperators<SemverPreRelease, SemverPreRelease, bool>, ISpanFormattable, ISpanParsable<SemverPreRelease>`;
- ✨ Added `SemverPreRelease(int)`;
- ✨ Added `SemverPreRelease(string)`;
- ✨ Added `SemverPreRelease(ReadOnlySpan<char>)`;
- ✨ Added `implicit SemverPreRelease(int)`;
- ✨ Added `implicit SemverPreRelease(string)`;
- ✨ Added `implicit SemverPreRelease(ReadOnlySpan<char>)`;
- ✨ Added `explicit int(SemverPreRelease)`;
- ✨ Added `explicit string(SemverPreRelease)`;
- ✨ Added `explicit ReadOnlySpan<char>(SemverPreRelease)`;
- ✨ Added `SemverPreRelease.IsNumeric`;
- ✨ Added `SemverPreRelease.AsNumber`;
- ✨ Added `static SemverPreRelease.Zero`;
- ✨ Added `SemverPreRelease.Equals(SemverPreRelease)`;
- ✨ Added `SemverPreRelease.Equals(object?)`;
- ✨ Added `SemverPreRelease.GetHashCode()`;
- ✨ Added `SemverPreRelease.CompareTo(SemverPreRelease)`;
- ✨ Added `static operator ==(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `static operator !=(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `static operator >(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `static operator <(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `static operator >=(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `static operator <=(SemverPreRelease?, SemverPreRelease?)`;
- ✨ Added `SemverPreRelease.ToString()`;
- ✨ Added `SemverPreRelease.TryFormat(Span<char>, out int)`;
- ✨ Added `static SemverPreRelease.Parse(string)`;
- ✨ Added `static SemverPreRelease.Parse(ReadOnlySpan<char>)`;
- ✨ Added `static SemverPreRelease.TryParse(string, out SemverPreRelease)`;
- ✨ Added `static SemverPreRelease.TryParse(ReadOnlySpan<char>, out SemverPreRelease)`;
- ✨ Added `static SemverPreRelease.Parse(string, SemverOptions)`;
- ✨ Added `static SemverPreRelease.Parse(ReadOnlySpan<char>, SemverOptions)`;
- ✨ Added `static SemverPreRelease.TryParse(string, SemverOptions, out SemverPreRelease)`;
- ✨ Added `static SemverPreRelease.TryParse(ReadOnlySpan<char>, SemverOptions, out SemverPreRelease)`;
