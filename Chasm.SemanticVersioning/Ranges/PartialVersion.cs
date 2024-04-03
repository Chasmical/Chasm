using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chasm.Collections;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> partial version.</para>
    /// </summary>
    public sealed partial class PartialVersion : IEquatable<PartialVersion>, IComparable, IComparable<PartialVersion>
#if NET7_0_OR_GREATER
                                               , System.Numerics.IEqualityOperators<PartialVersion, PartialVersion, bool>
#endif
    {
        /// <summary>
        ///   <para>Gets the partial version's first, major version component.</para>
        /// </summary>
        public PartialComponent Major { get; }
        /// <summary>
        ///   <para>Gets the partial version's second, minor version component.</para>
        /// </summary>
        public PartialComponent Minor { get; }
        /// <summary>
        ///   <para>Gets the partial version's third, patch version component.</para>
        /// </summary>
        public PartialComponent Patch { get; }

        internal readonly SemverPreRelease[] _preReleases;
        internal readonly string[] _buildMetadata;
        internal ReadOnlyCollection<SemverPreRelease>? _preReleasesReadonly;
        internal ReadOnlyCollection<string>? _buildMetadataReadonly;

        /// <summary>
        ///   <para>Gets a read-only collection of the partial version's pre-release identifiers.</para>
        /// </summary>
        public ReadOnlyCollection<SemverPreRelease> PreReleases
            => _preReleasesReadonly ??= _preReleases.AsReadOnly();
        /// <summary>
        ///   <para>Gets a read-only collection of the partial version's build metadata identifiers.</para>
        /// </summary>
        public ReadOnlyCollection<string> BuildMetadata
            => _buildMetadataReadonly ??= _buildMetadata.AsReadOnly();

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="major"/> version component.</para>
        /// </summary>
        /// <param name="major">The partial version's major version component.</param>
        /// <exception cref="ArgumentException"><paramref name="major"/> is omitted.</exception>
        public PartialVersion(PartialComponent major)
            : this(major, PartialComponent.Omitted, PartialComponent.Omitted, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="major"/> and <paramref name="minor"/> version components.</para>
        /// </summary>
        /// <param name="major">The partial version's major version component.</param>
        /// <param name="minor">The partial version's minor version component.</param>
        /// <exception cref="ArgumentException"><paramref name="major"/> is omitted.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor)
            : this(major, minor, PartialComponent.Omitted, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The partial version's major version component.</param>
        /// <param name="minor">The partial version's minor version component.</param>
        /// <param name="patch">The partial version's patch version component.</param>
        /// <exception cref="ArgumentException"><paramref name="major"/> is omitted; or <paramref name="minor"/> is omitted, but <paramref name="patch"/> isn't.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch)
            : this(major, minor, patch, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The partial version's major version component.</param>
        /// <param name="minor">The partial version's minor version component.</param>
        /// <param name="patch">The partial version's patch version component.</param>
        /// <param name="preReleases">A collection of the partial version's pre-release identifiers.</param>
        /// <exception cref="ArgumentException"><paramref name="major"/> is omitted; or <paramref name="minor"/> is omitted, but <paramref name="patch"/> isn't.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<SemverPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The partial version's major version component.</param>
        /// <param name="minor">The partial version's minor version component.</param>
        /// <param name="patch">The partial version's patch version component.</param>
        /// <param name="preReleases">A collection of the partial version's pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of the partial version's build metadata identifiers.</param>
        /// <exception cref="ArgumentException"><paramref name="major"/> is omitted, but <paramref name="minor"/> or <paramref name="patch"/> isn't; or <paramref name="minor"/> is omitted, but <paramref name="patch"/> isn't; or <paramref name="buildMetadata"/> contains <see langword="null"/> or an invalid build metadata identifier.</exception>
        public PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                              [InstantHandle] IEnumerable<SemverPreRelease>? preReleases,
                              [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major.IsOmitted) throw new ArgumentException(Exceptions.MajorOmitted, nameof(major));
            if (minor.IsOmitted && !patch.IsOmitted) throw new ArgumentException(Exceptions.MinorOmitted, nameof(minor));
            Major = major;
            Minor = minor;
            Patch = patch;

            _preReleases = preReleases is null ? [] : preReleases.ToArray();
            if (buildMetadata is not null)
            {
                string[] array = buildMetadata.ToArray();
                for (int i = 0; i < array.Length; i++)
                    Utility.ValidateBuildMetadataItem(array[i], nameof(buildMetadata));
                _buildMetadata = array;
            }
            else _buildMetadata = [];
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class using the specified <paramref name="systemVersion"/>'s <see cref="Version.Major"/>, <see cref="Version.Minor"/> and <see cref="Version.Build"/> version components (build component may be omitted, revision component is ignored).</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> object to get the partial version components from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systemVersion"/> is <see langword="null"/>.</exception>
        public PartialVersion(Version systemVersion)
        {
            if (systemVersion is null) throw new ArgumentNullException(nameof(systemVersion));
            Major = new PartialComponent(systemVersion.Major, default);
            Minor = new PartialComponent(systemVersion.Minor, default);
            Patch = new PartialComponent(systemVersion.Build, default);
            _preReleases = [];
            _buildMetadata = [];
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class using the specified <paramref name="semanticVersion"/>'s major, minor and patch version components and pre-release and build metadata identifiers.</para>
        /// </summary>
        /// <param name="semanticVersion">The semantic version to get the partial version components and identifiers from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="semanticVersion"/> is <see langword="null"/>.</exception>
        public PartialVersion(SemanticVersion semanticVersion)
        {
            if (semanticVersion is null) throw new ArgumentNullException(nameof(semanticVersion));
            Major = new PartialComponent(semanticVersion.Major, default);
            Minor = new PartialComponent(semanticVersion.Minor, default);
            Patch = new PartialComponent(semanticVersion.Patch, default);
            _preReleases = semanticVersion._preReleases;
            _preReleasesReadonly = semanticVersion._preReleasesReadonly;
            _buildMetadata = semanticVersion._buildMetadata;
            _buildMetadataReadonly = semanticVersion._buildMetadataReadonly;
        }

        /// <summary>
        ///   <para>Defines an explicit conversion of a <see cref="Version"/> to a partial version using the specified <paramref name="systemVersion"/>'s <see cref="Version.Major"/>, <see cref="Version.Minor"/> and <see cref="Version.Build"/> version components (build component may be omitted, revision component is ignored).</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> object to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(systemVersion))]
        public static explicit operator PartialVersion?(Version? systemVersion)
            => systemVersion is null ? null : new PartialVersion(systemVersion);
        /// <summary>
        ///   <para>Defines an implicit conversion of a semantic version to a partial version.</para>
        /// </summary>
        /// <param name="semanticVersion">The semantic version to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(semanticVersion))]
        public static implicit operator PartialVersion?(SemanticVersion? semanticVersion)
            => semanticVersion is null ? null : new PartialVersion(semanticVersion);

        /// <summary>
        ///   <para>Defines an explicit conversion of a partial version to a <see cref="Version"/>, replacing wildcards in major and minor components with zeroes, and in patch component with -1 (undefined, or omitted).</para>
        /// </summary>
        /// <param name="partialVersion">The partial version to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(partialVersion))]
        public static explicit operator Version?(PartialVersion? partialVersion)
        {
            if (partialVersion is null) return null;
            return new Version(
                partialVersion.Major.GetValueOrZero(),
                partialVersion.Minor.GetValueOrZero(),
                partialVersion.Patch.GetValueOrMinusOne()
            );
        }

        /// <summary>
        ///   <para>Determines whether the partial version has any wildcard or omitted version components.</para>
        /// </summary>
        public bool IsPartial => !Major.IsNumeric || !Minor.IsNumeric || !Patch.IsNumeric;
        /// <summary>
        ///   <para>Determines whether the partial version has any pre-release identifiers.</para>
        /// </summary>
        public bool IsPreRelease => _preReleases.Length != 0;
        /// <summary>
        ///   <para>Determines whether the partial version has any build metadata identifiers.</para>
        /// </summary>
        public bool HasBuildMetadata => _buildMetadata.Length != 0;

        /// <summary>
        ///   <para>Gets a read-only span of the partial version's pre-release identifiers.</para>
        /// </summary>
        /// <returns>A read-only span of the partial version's pre-release identifiers.</returns>
        [Pure] public ReadOnlySpan<SemverPreRelease> GetPreReleases()
            => _preReleases;
        /// <summary>
        ///   <para>Gets a read-only span of the partial version's build metadata identifiers.</para>
        /// </summary>
        /// <returns>A read-only span of the partial version's build metadata identifiers.</returns>
        [Pure] public ReadOnlySpan<string> GetBuildMetadata()
            => _buildMetadata;

        /// <summary>
        ///   <para>Determines whether this partial version is equal to another specified partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The partial version to compare with this partial version.</param>
        /// <returns><see langword="true"/>, if this partial version is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals(PartialVersion? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || !Major.Equals(other.Major) || !Minor.Equals(other.Minor) || !Patch.Equals(other.Patch)) return false;
            return Utility.EqualsIdentifiers(_preReleases, other._preReleases);
        }
        /// <summary>
        ///   <para>Determines whether this partial version is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this partial version.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="PartialVersion"/> instance equal to this partial version; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals(object? obj)
            => Equals(obj as PartialVersion);
        /// <summary>
        ///   <para>Returns a hash code for this partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <returns>A hash code for this partial version.</returns>
        [Pure] public override int GetHashCode()
        {
            if (_preReleases.Length == 0) return HashCode.Combine(Major, Minor, Patch);

            HashCode hash = new();
            hash.Add(Major);
            hash.Add(Minor);
            hash.Add(Patch);

            SemverPreRelease[] preReleases = _preReleases;
            for (int i = 0; i < preReleases.Length; i++)
                hash.Add(preReleases[i]);

            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Compares this partial version with another specified partial version and returns an integer that indicates whether this partial version precedes, follows or occurs in the same position in the sort order as the <paramref name="other"/> partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The partial version to compare with this partial version.</param>
        /// <returns>&lt;0, if this partial version precedes <paramref name="other"/> in the sort order;<br/>=0, if this partial version occurs in the same position in the sort order as <paramref name="other"/>;<br/>&gt;0, if this partial version follows <paramref name="other"/> in the sort order.</returns>
        [Pure] public int CompareTo(PartialVersion? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            int res = Major.CompareTo(other.Major);
            if (res != 0) return res;
            res = Minor.CompareTo(other.Minor);
            if (res != 0) return res;
            res = Patch.CompareTo(other.Patch);
            if (res != 0) return res;

            return Utility.CompareIdentifiers(_preReleases, other._preReleases);
        }
        [Pure] int IComparable.CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is PartialVersion other) return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(PartialVersion)}", nameof(obj));
        }

        /// <summary>
        ///   <para>Determines whether two specified partial versions are equal.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version to compare.</param>
        /// <param name="right">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialVersion? left, PartialVersion? right)
            => left is null ? right is null : left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified partial versions are not equal.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuildMetadata"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DifferentiateWildcards"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version to compare.</param>
        /// <param name="right">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialVersion? left, PartialVersion? right)
            => !(left == right);

    }
}
