using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a semantic version comparison operation that uses specific comparison rules.</para>
    /// </summary>
    public abstract class SemverComparer : IComparer, IEqualityComparer
                                         , IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
                                         , IComparer<PartialComponent>, IEqualityComparer<PartialComponent>
                                         , IComparer<PartialVersion>, IEqualityComparer<PartialVersion>
    {
        private const string supportedTypes = $"{nameof(SemanticVersion)}, {nameof(PartialVersion)} or {nameof(PartialComponent)}";

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

            throw new ArgumentException($"The object must be of type {supportedTypes}.");
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

            throw new ArgumentException($"The objects must be of type {supportedTypes}.");
        }
        [Pure] int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is null) return 0;
            if (obj is SemanticVersion version) return GetHashCode(version);
            if (obj is PartialVersion partial) return GetHashCode(partial);
            if (obj is PartialComponent component) return GetHashCode(component);
            throw new ArgumentException($"The object must be of type {supportedTypes}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Compares two semantic versions and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public abstract int Compare(SemanticVersion? a, SemanticVersion? b);
        /// <summary>
        ///   <para>Determines whether one semantic version is equal to another semantic version.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool Equals(SemanticVersion? a, SemanticVersion? b);
        /// <summary>
        ///   <para>Returns a hash code for the specified semantic version.</para>
        /// </summary>
        /// <param name="version">The semantic version to get a hash code for.</param>
        /// <returns>The hash code for the specified semantic version.</returns>
        [Pure] public abstract int GetHashCode(SemanticVersion? version);

        /// <summary>
        ///   <para>Compares two partial version components and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first partial version component to compare.</param>
        /// <param name="b">The second partial version component to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public abstract int Compare(PartialComponent a, PartialComponent b);
        /// <summary>
        ///   <para>Determines whether one partial version component is equal to another partial version component.</para>
        /// </summary>
        /// <param name="a">The first partial version component to compare.</param>
        /// <param name="b">The second partial version component to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool Equals(PartialComponent a, PartialComponent b);
        /// <summary>
        ///   <para>Returns a hash code for the specified partial version component.</para>
        /// </summary>
        /// <param name="component">The partial version component to get a hash code for.</param>
        /// <returns>The hash code for the specified partial version component.</returns>
        [Pure] public abstract int GetHashCode(PartialComponent component);

        /// <summary>
        ///   <para>Compares two partial versions and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.</para>
        /// </summary>
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public abstract int Compare(PartialVersion? a, PartialVersion? b);
        /// <summary>
        ///   <para>Determines whether one partial version is equal to another partial version.</para>
        /// </summary>
        /// <param name="a">The first partial version to compare.</param>
        /// <param name="b">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool Equals(PartialVersion? a, PartialVersion? b);
        /// <summary>
        ///   <para>Returns a hash code for the specified partial version.</para>
        /// </summary>
        /// <param name="partial">The partial version to get a hash code for.</param>
        /// <returns>The hash code for the specified partial version.</returns>
        [Pure] public abstract int GetHashCode(PartialVersion? partial);

        /// <summary>
        ///   <para>Returns a <see cref="SemverComparer"/> that uses the specified semantic version <paramref name="comparison"/> rules.</para>
        /// </summary>
        /// <param name="comparison">The semantic version comparison rules to use.</param>
        /// <returns>A <see cref="SemverComparer"/> that uses the specified semantic version <paramref name="comparison"/> rules.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="comparison"/> is not a valid set of comparison rules.</exception>
        [Pure] public static SemverComparer FromComparison(SemverComparison comparison) => comparison switch
        {
            SemverComparison.Default => Default,
            SemverComparison.IncludeBuildMetadata => IncludeBuildMetadata,
            SemverComparison.DifferentiateWildcards => DifferentiateWildcards,
            SemverComparison.IncludeBuildMetadata | SemverComparison.DifferentiateWildcards => IncludeBuildDiffWildcards,
            _ => throw new InvalidEnumArgumentException(nameof(comparison), (int)comparison, typeof(SemverComparison)),
        };

        /// <summary>
        ///   <para>Gets the default <see cref="SemverComparer"/>, that ignores the build metadata, and, when comparing partial versions, considers wildcard characters and omitted components equal.</para>
        /// </summary>
        public static SemverComparer Default { get; }
            = new ConfigurableSemverComparer(SemverComparison.Default);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/>, that includes the build metadata in the comparison, and, when comparing partial versions, considers wildcard characters and omitted components equal.</para>
        /// </summary>
        public static SemverComparer IncludeBuildMetadata { get; }
            = new ConfigurableSemverComparer(SemverComparison.IncludeBuildMetadata);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/>, that ignores the build metadata, and, when comparing partial versions, differentiates between different wildcard characters and omitted components.</para>
        /// </summary>
        public static SemverComparer DifferentiateWildcards { get; }
            = new ConfigurableSemverComparer(SemverComparison.DifferentiateWildcards);
        /// <summary>
        ///   <para>Gets the <see cref="SemverComparer"/>, that includes the build metadata in the comparison, and, when comparing partial versions, differentiates between different wildcard characters and omitted components.</para>
        /// </summary>
        public static SemverComparer IncludeBuildDiffWildcards { get; }
            = new ConfigurableSemverComparer(SemverComparison.IncludeBuildMetadata | SemverComparison.DifferentiateWildcards);

        internal sealed class ConfigurableSemverComparer(SemverComparison comparison) : SemverComparer
        {
            private readonly bool includeBuildMetadata = (comparison & SemverComparison.IncludeBuildMetadata) != 0;
            private readonly bool differentiateWildcards = (comparison & SemverComparison.DifferentiateWildcards) != 0;

            [Pure] public override int Compare(SemanticVersion? a, SemanticVersion? b)
            {
                if (a is null) return b is null ? 0 : -1;
                int res = a.CompareTo(b);
                if (res == 0 && includeBuildMetadata)
                    res = Utility.CompareIdentifiers(a._buildMetadata, b!._buildMetadata);
                return res;
            }
            [Pure] public override bool Equals(SemanticVersion? a, SemanticVersion? b)
            {
                if (a is null) return b is null;
                bool res = a.Equals(b);
                if (res && includeBuildMetadata)
                    res = Utility.EqualsIdentifiers(a._buildMetadata, b!._buildMetadata);
                return res;
            }
            [Pure] public override int GetHashCode(SemanticVersion? version)
            {
                if (version is null) return 0;
                int res = version.GetHashCode();
                if (!includeBuildMetadata || version._buildMetadata.Length == 0)
                    return res;

                HashCode hash = new();
                hash.Add(res);

                string[] buildMetadata = version._buildMetadata;
                for (int i = 0; i < buildMetadata.Length; i++)
                    hash.Add(buildMetadata[i]);

                return hash.ToHashCode();
            }

            [Pure] public override int Compare(PartialComponent a, PartialComponent b)
                => differentiateWildcards ? a._value.CompareTo(b._value) : a.CompareTo(b);
            [Pure] public override bool Equals(PartialComponent a, PartialComponent b)
                => differentiateWildcards ? a._value == b._value : a.Equals(b);
            [Pure] public override int GetHashCode(PartialComponent component)
                => differentiateWildcards ? component._value : component.GetHashCode();

            [Pure] public override int Compare(PartialVersion? a, PartialVersion? b)
            {
                if (a is null) return b is null ? 0 : -1;
                if (!differentiateWildcards || b is null) return a.CompareTo(b);

                int res = Compare(a.Major, b.Major);
                if (res != 0) return res;
                res = Compare(a.Minor, b.Minor);
                if (res != 0) return res;
                res = Compare(a.Patch, b.Patch);
                if (res != 0) return res;

                res = Utility.CompareIdentifiers(a._preReleases, b._preReleases);
                if (res == 0 && includeBuildMetadata)
                    res = Utility.CompareIdentifiers(a._buildMetadata, b._buildMetadata);
                return res;
            }
            [Pure] public override bool Equals(PartialVersion? a, PartialVersion? b)
            {
                if (a is null) return b is null;
                if (b is null) return false;
                if (!includeBuildMetadata && !differentiateWildcards)
                    return a.Equals(b);

                return Equals(a.Major, b.Major) && Equals(a.Minor, b.Minor) && Equals(a.Patch, b.Patch) &&
                       Utility.EqualsIdentifiers(a._preReleases, b._preReleases) &&
                       (!includeBuildMetadata || Utility.EqualsIdentifiers(a._buildMetadata, b._buildMetadata));
            }
            [Pure] public override int GetHashCode(PartialVersion? partial)
            {
                if (partial is null) return 0;
                if (!includeBuildMetadata && !differentiateWildcards)
                    return partial.GetHashCode();

                HashCode hash = new();
                hash.Add(GetHashCode(partial.Major));
                hash.Add(GetHashCode(partial.Minor));
                hash.Add(GetHashCode(partial.Patch));

                SemverPreRelease[] preReleases = partial._preReleases;
                for (int i = 0; i < preReleases.Length; i++)
                    hash.Add(preReleases[i]);

                if (includeBuildMetadata)
                {
                    string[] buildMetadata = partial._buildMetadata;
                    for (int i = 0; i < buildMetadata.Length; i++)
                        hash.Add(buildMetadata[i]);
                }

                return hash.ToHashCode();

            }

        }

    }
}
