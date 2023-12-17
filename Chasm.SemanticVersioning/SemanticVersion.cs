using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chasm.Collections;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Represents a valid semantic version, compliant to the SemVer 2.0.0 specification.</para>
    /// </summary>
    public sealed class SemanticVersion : IEquatable<SemanticVersion>, IComparable, IComparable<SemanticVersion>
#if NET7_0_OR_GREATER
                                        , System.Numerics.IComparisonOperators<SemanticVersion, SemanticVersion, bool>
                                        , System.Numerics.IMinMaxValue<SemanticVersion>
#endif
    {
        /// <summary>
        ///   <para>Gets the semantic version's first, major version component.</para>
        /// </summary>
        public int Major { get; }
        /// <summary>
        ///   <para>Gets the semantic version's second, minor version component.</para>
        /// </summary>
        public int Minor { get; }
        /// <summary>
        ///   <para>Gets the semantic version's third, patch version component.</para>
        /// </summary>
        public int Patch { get; }

        private readonly SemverPreRelease[] _preReleases;
        private readonly string[] _buildMetadata;
        private ReadOnlyCollection<SemverPreRelease>? _preReleasesReadonly;
        private ReadOnlyCollection<string>? _buildMetadataReadonly;

        /// <summary>
        ///   <para>Gets a read-only collection of the semantic version's pre-release identifiers.</para>
        /// </summary>
        public ReadOnlyCollection<SemverPreRelease> PreReleases
            => _preReleasesReadonly ??= _preReleases.AsReadOnly();
        /// <summary>
        ///   <para>Gets a read-only collection of the semantic version's build metadata identifiers.</para>
        /// </summary>
        public ReadOnlyCollection<string> BuildMetadata
            => _buildMetadataReadonly ??= _buildMetadata.AsReadOnly();

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <param name="preReleases">A collection of the semantic version's pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersion(int major, int minor, int patch, [InstantHandle] IEnumerable<SemverPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <param name="preReleases">A collection of the semantic version's pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of the semantic version's build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="buildMetadata"/> contains <see langword="null"/> or an invalid build metadata identifier.</exception>
        public SemanticVersion(int major, int minor, int patch,
                               [InstantHandle] IEnumerable<SemverPreRelease>? preReleases,
                               [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            Major = major;
            Minor = minor;
            Patch = patch;

            _preReleases = preReleases is null ? Array.Empty<SemverPreRelease>() : preReleases.ToArray();
            if (buildMetadata is not null)
            {
                string[] array = buildMetadata.ToArray();
                for (int i = 0; i < array.Length; i++)
                    Utility.ValidateBuildMetadataItem(array[i], nameof(buildMetadata));
                _buildMetadata = array;
            }
            else _buildMetadata = Array.Empty<string>();
        }

        /// <summary>
        ///   <para>Determines whether the semantic version is considered stable, that is, has a major version component greater than zero, and has no pre-release identifiers.</para>
        /// </summary>
        public bool IsStable => Major != 0 && _preReleases.Length == 0;
        /// <summary>
        ///   <para>Determines whether the semantic version has any pre-release identifiers.</para>
        /// </summary>
        public bool IsPreRelease => _preReleases.Length != 0;
        /// <summary>
        ///   <para>Determines whether the semantic version has any build metadata identifiers.</para>
        /// </summary>
        public bool HasBuildMetadata => _buildMetadata.Length != 0;

        /// <summary>
        ///   <para>Gets the minimum possible valid semantic version, <c>0.0.0-0</c>.</para>
        /// </summary>
        public static SemanticVersion MinValue { get; } = new SemanticVersion(0, 0, 0, SemverPreRelease.ZeroArray, null);
        /// <summary>
        ///   <para>Gets the maximum possible valid semantic version in this implementation of SemVer, <c>2147483647.2147483647.2147483647</c>.</para>
        /// </summary>
        public static SemanticVersion MaxValue { get; } = new SemanticVersion(int.MaxValue, int.MaxValue, int.MaxValue, null, null);

        /// <summary>
        ///   <para>Determines whether this semantic version is equal to another specified semantic version.</para>
        /// </summary>
        /// <param name="other">The semantic version to compare with this semantic version.</param>
        /// <returns><see langword="true"/>, if this semantic version is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(SemanticVersion? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || Major != other.Major || Minor != other.Minor || Patch != other.Patch) return false;
            return Utility.EqualsIdentifiers(_preReleases, other._preReleases);
        }
        /// <summary>
        ///   <para>Determines whether this semantic version is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this semantic version.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="SemanticVersion"/> instance equal to this semantic version; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as SemanticVersion);
        /// <summary>
        ///   <para>Returns a hash code for this semantic version.</para>
        /// </summary>
        /// <returns>A hash code for this semantic version.</returns>
        [Pure] public override int GetHashCode()
        {
            if (_preReleases.Length == 0) return HashCode.Combine(Major, Minor, Patch);

            HashCode hash = new();
            hash.Add(Major);
            hash.Add(Minor);
            hash.Add(Patch);

            for (int i = 0, length = _preReleases.Length; i < length; i++)
                hash.Add(_preReleases[i]);

            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Compares this semantic version with another specified semantic version and returns an integer that indicates whether this semantic version precedes, follows or occurs in the same position in the sort order as the <paramref name="other"/> semantic version.</para>
        /// </summary>
        /// <param name="other">The semantic version to compare with this semantic version.</param>
        /// <returns>&lt;0, if this semantic version precedes <paramref name="other"/> in the sort order;<br/>=0, if this semantic version occurs in the same position in the sort order as <paramref name="other"/>;<br/>&gt;0, if this semantic version follows <paramref name="other"/> in the sort order.</returns>
        [Pure] public int CompareTo(SemanticVersion? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            int res = Major - other.Major;
            if (res != 0) return res;
            res = Minor - other.Minor;
            if (res != 0) return res;
            res = Patch - other.Patch;
            if (res != 0) return res;

            return Utility.CompareIdentifiers(_preReleases, other._preReleases);
        }
        [Pure] int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is SemanticVersion other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(SemanticVersion)}", nameof(obj));
        }

        /// <summary>
        ///   <para>Determines whether two specified semantic versions are equal.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(SemanticVersion? left, SemanticVersion? right)
            => left is null ? right is null : left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified semantic versions are not equal.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(SemanticVersion? left, SemanticVersion? right)
            => left is null ? right is not null : !left.Equals(right);

        /// <summary>
        ///   <para>Determines whether a specified semantic version is greater than another specified semantic version.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >(SemanticVersion? left, SemanticVersion? right)
            => left is not null && left.CompareTo(right) > 0;
        /// <summary>
        ///   <para>Determines whether a specified semantic version is less than another specified semantic version.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <(SemanticVersion? left, SemanticVersion? right)
            => left is null ? right is not null : left.CompareTo(right) < 0;
        /// <summary>
        ///   <para>Determines whether a specified semantic version is greater than or equal to another specified semantic version.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator >=(SemanticVersion? left, SemanticVersion? right)
            => left is null ? right is null : left.CompareTo(right) >= 0;
        /// <summary>
        ///   <para>Determines whether a specified semantic version is less than or equal to another specified semantic version.</para>
        /// </summary>
        /// <param name="left">The first semantic version to compare.</param>
        /// <param name="right">The second semantic version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator <=(SemanticVersion? left, SemanticVersion? right)
            => left is null || left.CompareTo(right) <= 0;

    }
}
