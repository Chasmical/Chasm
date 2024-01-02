using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Compares two semantic versions for equivalence, while also taking into account their build metadata identifiers.</para>
    /// </summary>
    public sealed class BuildMetadataComparer : IComparer, IEqualityComparer,
                                                IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        private BuildMetadataComparer() { }

        /// <summary>
        ///   <para>Gets an instance of the <see cref="BuildMetadataComparer"/> type.</para>
        /// </summary>
        public static BuildMetadataComparer Instance { get; } = new BuildMetadataComparer();

        [Pure] int IComparer.Compare(object? a, object? b)
        {
            if (a is null) return b is null ? 0 : -1;
            if (b is null) return 1;

            if (a is SemanticVersion versionA && b is SemanticVersion versionB)
                return Compare(versionA, versionB);

            throw new ArgumentException($"The object must be of type {nameof(SemanticVersion)}.");
        }
        [Pure] bool IEqualityComparer.Equals(object? a, object? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            if (a is SemanticVersion versionA && b is SemanticVersion versionB)
                return Equals(versionA, versionB);

            throw new ArgumentException($"The objects must be of type {nameof(SemanticVersion)}.");
        }
        [Pure] int IEqualityComparer.GetHashCode(object? obj)
        {
            if (obj is null) return 0;
            if (obj is SemanticVersion version) return GetHashCode(version);
            throw new ArgumentException($"The object must be of type {nameof(SemanticVersion)}.", nameof(obj));
        }

        /// <summary>
        ///   <para>Compares two semantic versions and returns an integer that indicates whether one precedes, follows or occurs in the same position in the sort order as another.<br/>Build metadata is included in this comparison. For build metadata-insensitive comparison, use <see cref="SemanticVersion.CompareTo"/>.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns>&lt;0, if <paramref name="a"/> precedes <paramref name="b"/> in the sort order;<br/>=0, if <paramref name="a"/> occurs in the same position in the sort order as <paramref name="b"/>;<br/>&gt;0, if <paramref name="a"/> follows <paramref name="b"/> in the sort order.</returns>
        [Pure] public int Compare(SemanticVersion? a, SemanticVersion? b)
        {
            if (a is null) return b is null ? 0 : -1;
            int res = a.CompareTo(b);
            return res != 0 ? res : Utility.CompareIdentifiers(a._buildMetadata, b!._buildMetadata);
        }
        /// <summary>
        ///   <para>Determines whether one semantic version is equal to another semantic version.<br/>Build metadata is included in this comparison. For build metadata-insensitive comparison, use <see cref="SemanticVersion.Equals(SemanticVersion?)"/>.</para>
        /// </summary>
        /// <param name="a">The first semantic version to compare.</param>
        /// <param name="b">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(SemanticVersion? a, SemanticVersion? b)
        {
            if (a is null) return b is null;
            return a.Equals(b) && Utility.EqualsIdentifiers(a._buildMetadata, b._buildMetadata);
        }
        /// <summary>
        ///   <para>Returns a hash code for the specified semantic version.<br/>Build metadata is included in this comparison. For build metadata-insensitive comparison, use <see cref="SemanticVersion.GetHashCode"/>.</para>
        /// </summary>
        /// <param name="version">The semantic version to get a hash code for.</param>
        /// <returns>The hash code for the specified semantic version.</returns>
        [Pure] public int GetHashCode(SemanticVersion? version)
        {
            if (version is null) return 0;
            if (version._buildMetadata.Length == 0) return version.GetHashCode();

            HashCode hash = new HashCode();
            hash.Add(version);

            string[] buildMetadata = version._buildMetadata;
            for (int i = 0; i < buildMetadata.Length; i++)
                hash.Add(buildMetadata[i]);

            return hash.ToHashCode();
        }

    }
}
