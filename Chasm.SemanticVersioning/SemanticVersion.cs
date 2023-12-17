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
    public sealed class SemanticVersion
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
        public SemanticVersion(int major, int minor, int patch, params SemverPreRelease[] preReleases)
            : this(major, minor, patch, preReleases, null) { }
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

    }
}
