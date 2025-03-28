﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> partial version.</para>
    /// </summary>
    public sealed partial class PartialVersion : IEquatable<PartialVersion>, IComparable, IComparable<PartialVersion>, IXmlSerializable
#if NET7_0_OR_GREATER
                                               , System.Numerics.IEqualityOperators<PartialVersion, PartialVersion, bool>
#endif
    {
        private readonly PartialComponent _major, _minor, _patch;

        /// <summary>
        ///   <para>Gets the partial version's first, major version component.</para>
        /// </summary>
        public PartialComponent Major => _major;
        /// <summary>
        ///   <para>Gets the partial version's second, minor version component.</para>
        /// </summary>
        public PartialComponent Minor => _minor;
        /// <summary>
        ///   <para>Gets the partial version's third, patch version component.</para>
        /// </summary>
        public PartialComponent Patch => _patch;

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
            _major = major;
            _minor = minor;
            _patch = patch;

            SemverPreRelease[] preReleasesArray = preReleases is null ? [] : preReleases.ToArray();
            string[] buildMetadataArray = buildMetadata is null ? [] : buildMetadata.ToArray();

            for (int i = 0; i < buildMetadataArray.Length; i++)
                Utility.ValidateBuildMetadataItem(buildMetadataArray[i], nameof(buildMetadata));

            if (patch.IsOmitted)
            {
                if (preReleasesArray.Length > 0) throw new ArgumentException(Exceptions.PreReleaseAfterOmitted, nameof(preReleases));
                if (buildMetadataArray.Length > 0) throw new ArgumentException(Exceptions.BuildMetadataAfterOmitted, nameof(buildMetadata));
            }

            _preReleases = preReleasesArray;
            _buildMetadata = buildMetadataArray;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="PartialVersion"/> class using the specified <paramref name="systemVersion"/>'s <see cref="Version.Major"/>, <see cref="Version.Minor"/> and <see cref="Version.Build"/> version components (build component may be omitted, revision component is ignored).</para>
        /// </summary>
        /// <param name="systemVersion">The <see cref="Version"/> object to get the partial version components from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="systemVersion"/> is <see langword="null"/>.</exception>
        public PartialVersion(Version systemVersion)
        {
            ANE.ThrowIfNull(systemVersion);
            _major = new PartialComponent(systemVersion.Major, default);
            _minor = new PartialComponent(systemVersion.Minor, default);
            _patch = new PartialComponent(systemVersion.Build, default);
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
            ANE.ThrowIfNull(semanticVersion);
            _major = new PartialComponent(semanticVersion.Major, default);
            _minor = new PartialComponent(semanticVersion.Minor, default);
            _patch = new PartialComponent(semanticVersion.Patch, default);
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
            int major = partialVersion.Major.GetValueOrZero();
            int minor = partialVersion.Minor.GetValueOrZero();
            int patch = (int)partialVersion.Patch._value;
            return patch >= 0 ? new Version(major, minor, patch) : new Version(major, minor);
        }
        /// <summary>
        ///   <para>Defines an explicit conversion of a partial version to a semantic version, replacing wildcards in version components with zeroes.</para>
        /// </summary>
        /// <param name="partialVersion">The partial version to convert.</param>
        [Pure] [return: NotNullIfNotNull(nameof(partialVersion))]
        public static explicit operator SemanticVersion?(PartialVersion? partialVersion)
        {
            if (partialVersion is null) return null;
            return new SemanticVersion(
                partialVersion.Major.GetValueOrZero(),
                partialVersion.Minor.GetValueOrZero(),
                partialVersion.Patch.GetValueOrZero(),
                partialVersion._preReleases,
                partialVersion._buildMetadata,
                partialVersion._preReleasesReadonly,
                partialVersion._buildMetadataReadonly
            );
        }

        /// <summary>
        ///   <para>Defines an explicit conversion of a version string to a partial version.</para>
        /// </summary>
        /// <param name="versionString">The string containing a partial version to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="versionString"/> is not a valid partial version.</exception>
        [Pure] [return: NotNullIfNotNull(nameof(versionString))]
        public static explicit operator PartialVersion?(string? versionString)
            => versionString is null ? null : Parse(versionString);

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
        ///   <para>Gets the one-star partial version with all other components omitted, <c>*</c>.</para>
        /// </summary>
        public static PartialVersion OneStar { get; } = new PartialVersion(PartialComponent.Star);

        /// <summary>
        ///   <para>Determines whether this partial version is equal to another specified partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The partial version to compare with this partial version.</param>
        /// <returns><see langword="true"/>, if this partial version is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals([NotNullWhen(true)] PartialVersion? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null || !Major.Equals(other.Major) || !Minor.Equals(other.Minor) || !Patch.Equals(other.Patch)) return false;
            return Utility.SequenceEqual(_preReleases, other._preReleases);
        }
        /// <summary>
        ///   <para>Determines whether this partial version is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this partial version.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="PartialVersion"/> instance equal to this partial version; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as PartialVersion);
        /// <summary>
        ///   <para>Returns a hash code for this partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <returns>A hash code for this partial version.</returns>
        [Pure] public override int GetHashCode()
        {
            SemverPreRelease[] preReleases = _preReleases;
            if (preReleases.Length == 0) return HashCode.Combine(Major, Minor, Patch);

            HashCode hash = new();
            hash.Add(Major);
            hash.Add(Minor);
            hash.Add(Patch);

            for (int i = 0; i < preReleases.Length; i++)
                hash.Add(preReleases[i]);

            return hash.ToHashCode();
        }

        /// <summary>
        ///   <para>Compares this partial version with another specified partial version and returns an integer that indicates whether this partial version precedes, follows or occurs in the same position in the sort order as the <paramref name="other"/> partial version.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
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
        ///   <para>Determines whether two specified partial versions are equal.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version to compare.</param>
        /// <param name="right">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(PartialVersion? left, PartialVersion? right)
            => left is null ? right is null : left.Equals(right);
        /// <summary>
        ///   <para>Determines whether two specified partial versions are not equal.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="left">The first partial version to compare.</param>
        /// <param name="right">The second partial version to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(PartialVersion? left, PartialVersion? right)
            => !(left == right);

        #region IXmlSerializable implementation
        System.Xml.Schema.XmlSchema? IXmlSerializable.GetSchema() => null;

#pragma warning disable CS8618
        [Obsolete] private PartialVersion() { }
#pragma warning restore CS8618

        void IXmlSerializable.WriteXml(XmlWriter xml)
            => xml.WriteString(ToString());
        void IXmlSerializable.ReadXml(XmlReader xml)
        {
            if (_preReleases is not null) throw new InvalidOperationException();
            PartialVersion version = Parse(xml.ReadElementContentAsString());
            Unsafe.AsRef(in _major) = version._major;
            Unsafe.AsRef(in _minor) = version._minor;
            Unsafe.AsRef(in _patch) = version._patch;
            Unsafe.AsRef(in _preReleases) = version._preReleases;
            Unsafe.AsRef(in _buildMetadata) = version._buildMetadata;
        }
        #endregion

    }
}
