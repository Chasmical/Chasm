using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a semantic version comparison operation that uses specific comparison rules.</para>
    /// </summary>
    public sealed class SemverComparer : IComparer, IEqualityComparer
                                       , IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
                                       , IComparer<PartialVersion>, IEqualityComparer<PartialVersion>
                                       , IComparer<PartialComponent>, IEqualityComparer<PartialComponent>
                                       , IEqualityComparer<VersionRange>
                                       , IEqualityComparer<ComparatorSet>
                                       , IEqualityComparer<Comparator>
    {
        private const string supportedComparisonTypes = $"{nameof(SemanticVersion)}, {nameof(PartialVersion)}, {nameof(PartialComponent)}";
        private const string supportedEqualityTypes = $"{supportedComparisonTypes}, {nameof(VersionRange)}, {nameof(ComparatorSet)}," +
                                                      $"{nameof(Comparator)}";

        private readonly bool isDefault;
        private readonly bool includeBuild;
        private readonly bool diffWildcards;
        private readonly bool diffEquality;

        private SemverComparer(SemverComparison comparison)
        {
            if (comparison > SemverComparison.Exact)
                throw new InvalidEnumArgumentException(nameof(comparison), (int)comparison, typeof(SemverComparison));

            isDefault = comparison == SemverComparison.Default;
            includeBuild = (comparison & SemverComparison.IncludeBuild) != 0;
            diffWildcards = (comparison & SemverComparison.DiffWildcards) != 0;
            diffEquality = (comparison & SemverComparison.DiffEquality) != 0;
        }

        [Pure] int IComparer.Compare(object? a, object? b)
        {
            if (a is null) return b is null ? 0 : -1;
            if (b is null) return 1;

            if (a is SemanticVersion versionA && b is SemanticVersion versionB)
                return Compare(versionA, versionB);
            if (a is PartialVersion partialA && b is PartialVersion partialB)
                return Compare(partialA, partialB);
            if (a is PartialComponent componentA && b is PartialComponent componentB)
                return Compare(componentA, componentB);

            throw new ArgumentException($"The objects must be of one of the following types: {supportedComparisonTypes}.");
        }
        [Pure] bool IEqualityComparer.Equals(object? a, object? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            if (a is SemanticVersion versionA && b is SemanticVersion versionB)
                return Equals(versionA, versionB);
            if (a is PartialVersion partialA && b is PartialVersion partialB)
                return Equals(partialA, partialB);
            if (a is PartialComponent componentA && b is PartialComponent componentB)
                return Equals(componentA, componentB);
            if (a is VersionRange rangeA && b is VersionRange rangeB)
                return Equals(rangeA, rangeB);
            if (a is ComparatorSet setA && b is ComparatorSet setB)
                return Equals(setA, setB);
            if (a is Comparator comparatorA && b is Comparator comparatorB)
                return Equals(comparatorA, comparatorB);

            throw new ArgumentException($"The objects must be of one of the following types: {supportedEqualityTypes}.");
        }
        [Pure] int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is null) return 0;
            if (obj is SemanticVersion version) return GetHashCode(version);
            if (obj is PartialVersion partial) return GetHashCode(partial);
            if (obj is PartialComponent component) return GetHashCode(component);
            if (obj is VersionRange range) return GetHashCode(range);
            if (obj is ComparatorSet set) return GetHashCode(set);
            if (obj is Comparator comparator) return GetHashCode(comparator);
            throw new ArgumentException($"The object must be of one of the following types: {supportedEqualityTypes}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Compares two semantic versions and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public int Compare(SemanticVersion? a, SemanticVersion? b)
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a is null) return -1;
            if (b is null) return 1;

            int res = a.CompareTo(b);
            if (res == 0 && includeBuild)
                res = Utility.CompareIdentifiers(a._buildMetadata, b._buildMetadata);
            return res;
        }
        /// <summary>
        ///   <para>Determines whether one semantic version is equal to another semantic version.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(SemanticVersion? a, SemanticVersion? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            bool res = a.Equals(b);
            if (res && includeBuild)
                res = Utility.SequenceEqual(a._buildMetadata, b._buildMetadata);
            return res;
        }
        /// <summary>
        ///   <para>Returns a hash code for the specified semantic version.</para>
        /// </summary>
        /// <param name="version">The semantic version to get a hash code for.</param>
        /// <returns>The hash code for the specified semantic version.</returns>
        [Pure] public int GetHashCode(SemanticVersion? version)
        {
            if (version is null) return 0;
            int res = version.GetHashCode();

            if (includeBuild && version._buildMetadata.Length > 0)
            {
                HashCode hash = new();
                hash.Add(res);

                string[] buildMetadata = version._buildMetadata;
                for (int i = 0; i < buildMetadata.Length; i++)
                    hash.Add(buildMetadata[i]);

                res = hash.ToHashCode();
            }
            return res;
        }

        /// <summary>
        ///   <para>Compares two partial versions and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public int Compare(PartialVersion? a, PartialVersion? b)
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a is null) return -1;
            if (b is null) return 1;

            int res;
            if (!diffWildcards)
            {
                res = a.CompareTo(b);
            }
            else
            {
                res = Compare(a.Major, b.Major);
                if (res != 0) return res;
                res = Compare(a.Minor, b.Minor);
                if (res != 0) return res;
                res = Compare(a.Patch, b.Patch);
                if (res != 0) return res;
                res = Utility.CompareIdentifiers(a._preReleases, b._preReleases);
            }

            if (res == 0 && includeBuild)
                res = Utility.CompareIdentifiers(a._buildMetadata, b._buildMetadata);
            return res;
        }
        /// <summary>
        ///   <para>Determines whether one partial version is equal to another partial version.</para>
        /// </summary>
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(PartialVersion? a, PartialVersion? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            bool res;
            if (!diffWildcards)
            {
                res = a.Equals(b);
            }
            else
            {
                res = ReferenceEquals(a, b)
                   || Equals(a.Major, b.Major) && Equals(a.Minor, b.Minor) && Equals(a.Patch, b.Patch)
                   && Utility.SequenceEqual(a._preReleases, b._preReleases);
            }

            if (res && includeBuild)
                res = Utility.SequenceEqual(a._buildMetadata, b._buildMetadata);
            return res;
        }
        /// <summary>
        ///   <para>Returns a hash code for the specified partial version.</para>
        /// </summary>
        /// <param name="partial">The partial version to get a hash code for.</param>
        /// <returns>The hash code for the specified partial version.</returns>
        [Pure] public int GetHashCode(PartialVersion? partial)
        {
            if (partial is null) return 0;

            if ((!diffWildcards || !partial.IsPartial) && (!includeBuild || partial._buildMetadata.Length == 0))
                return partial.GetHashCode();

            HashCode hash = new();
            hash.Add(GetHashCode(partial.Major));
            hash.Add(GetHashCode(partial.Minor));
            hash.Add(GetHashCode(partial.Patch));

            SemverPreRelease[] preReleases = partial._preReleases;
            for (int i = 0; i < preReleases.Length; i++)
                hash.Add(preReleases[i]);

            if (includeBuild)
            {
                string[] buildMetadata = partial._buildMetadata;
                for (int i = 0; i < buildMetadata.Length; i++)
                    hash.Add(buildMetadata[i]);
            }
            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Compares two partial version components and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first partial version component to compare.</param>
        /// <param name="b">The second partial version component to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public int Compare(PartialComponent a, PartialComponent b)
            => diffWildcards ? ((int)a._value).CompareTo((int)b._value) : a.CompareTo(b);
        /// <summary>
        ///   <para>Determines whether one partial version component is equal to another partial version component.</para>
        /// </summary>
        /// <param name="a">The first partial version component to compare.</param>
        /// <param name="b">The second partial version component to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(PartialComponent a, PartialComponent b)
            => diffWildcards ? a._value == b._value : a.Equals(b);
        /// <summary>
        ///   <para>Returns a hash code for the specified partial version component.</para>
        /// </summary>
        /// <param name="component">The partial version component to get a hash code for.</param>
        /// <returns>The hash code for the specified partial version component.</returns>
        [Pure] public int GetHashCode(PartialComponent component)
            => diffWildcards ? (int)component._value : component.GetHashCode();

        [Pure] public bool Equals(VersionRange? a, VersionRange? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (isDefault) return a.Equals(b);
            return Utility.SequenceEqual(a._comparatorSets.AsSpan(), b._comparatorSets, this);
        }
        [Pure] public int GetHashCode(VersionRange? range)
        {
            if (range is null) return 0;
            if (isDefault) return range.GetHashCode();

            HashCode hash = new();
            ComparatorSet[] sets = range._comparatorSets;
            for (int i = 0; i < sets.Length; i++)
                hash.Add(GetHashCode(sets[i]));
            return hash.ToHashCode();
        }

        [Pure] public bool Equals(ComparatorSet? a, ComparatorSet? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (isDefault) return a.Equals(b);
            return Utility.SequenceEqual(a._comparators.AsSpan(), b._comparators, this);
        }
        [Pure] public int GetHashCode(ComparatorSet? set)
        {
            if (set is null) return 0;
            if (isDefault) return set.GetHashCode();

            HashCode hash = new();
            Comparator[] comparators = set._comparators;
            for (int i = 0; i < comparators.Length; i++)
                hash.Add(GetHashCode(comparators[i]));
            return hash.ToHashCode();
        }

        [Pure] public bool Equals(Comparator? a, Comparator? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (isDefault) return a.Equals(b);

            switch (a)
            {
                case PrimitiveComparator primitiveA:
                    return b is PrimitiveComparator primitiveB && Equals(primitiveA.Operator, primitiveB.Operator) &&
                           Equals(primitiveA.Operand, primitiveB.Operand);
                case CaretComparator caretA:
                    return b is CaretComparator caretB && Equals(caretA.Operand, caretB.Operand);
                case HyphenRangeComparator hyphenA:
                    return b is HyphenRangeComparator hyphenB && Equals(hyphenA.From, hyphenB.From) && Equals(hyphenA.To, hyphenB.To);
                case TildeComparator tildeA:
                    return b is TildeComparator tildeB && Equals(tildeA.Operand, tildeB.Operand);
                case XRangeComparator xRangeA:
                    return b is XRangeComparator xRangeB && Equals(xRangeA.Operator, xRangeB.Operator) &&
                           Equals(xRangeA.Operand, xRangeB.Operand);
                default:
                    // dotcover disable
                    Debug.Fail("Invalid comparator type");
                    return a.Equals(b);
                    // dotcover enable
            }
        }
        [Pure] public int GetHashCode(Comparator? comparator)
        {
            if (comparator is null) return 0;
            if (isDefault) return comparator.GetHashCode();

            switch (comparator)
            {
                case PrimitiveComparator primitive:
                    return HashCode.Combine(GetHashCode(primitive.Operand), GetHashCode(primitive.Operator));
                case CaretComparator caret:
                    return HashCode.Combine(caret.GetType(), GetHashCode(caret.Operand));
                case HyphenRangeComparator hyphen:
                    return HashCode.Combine(hyphen.GetType(), GetHashCode(hyphen.From), GetHashCode(hyphen.To));
                case TildeComparator tilde:
                    return HashCode.Combine(tilde.GetType(), GetHashCode(tilde.Operand));
                case XRangeComparator xRange:
                    return HashCode.Combine(xRange.GetType(), GetHashCode(xRange.Operand), GetHashCode(xRange.Operator));
                default:
                    // dotcover disable
                    Debug.Fail("Invalid comparator type");
                    return comparator.GetHashCode();
                // dotcover enable
            }
        }

        private bool Equals(PrimitiveOperator a, PrimitiveOperator b)
            => diffEquality ? a == b : a.Normalize() == b.Normalize();
        private int GetHashCode(PrimitiveOperator @operator)
            => (int)(diffEquality ? @operator : @operator.Normalize());

        /// <summary>
        ///   <para>Returns a <see cref="SemverComparer"/> that uses the specified semantic version <paramref name="comparison"/> rules.</para>
        /// </summary>
        /// <param name="comparison">The semantic version comparison rules to use.</param>
        /// <returns>A <see cref="SemverComparer"/> that uses the specified semantic version <paramref name="comparison"/> rules.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="comparison"/> is not a valid set of comparison rules.</exception>
        [Pure] public static SemverComparer FromComparison(SemverComparison comparison) => comparison switch
        {
            SemverComparison.Default => Default,
            SemverComparison.IncludeBuild => IncludeBuild,
            SemverComparison.DiffWildcards => DiffWildcards,
            SemverComparison.Exact => Exact,
            _ => new SemverComparer(comparison),
        };

        /// <summary>
        ///   <para>Gets the default <see cref="SemverComparer"/>, that ignores build metadata.</para>
        /// </summary>
        public static SemverComparer Default { get; }
            = new SemverComparer(SemverComparison.Default);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/>, that includes build metadata in the comparison.</para>
        /// </summary>
        public static SemverComparer IncludeBuild { get; }
            = new SemverComparer(SemverComparison.IncludeBuild);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/>, that differentiates between different wildcard characters and omitted components.</para>
        /// </summary>
        public static SemverComparer DiffWildcards { get; }
            = new SemverComparer(SemverComparison.DiffWildcards);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/> that compares objects exactly: includes build metadata in the comparison, differentiates between different wildcard characters and omitted components, and differentiates between implicit and explicit equality operators.</para>
        /// </summary>
        public static SemverComparer Exact { get; }
            = new SemverComparer(SemverComparison.Exact);

    }
}
