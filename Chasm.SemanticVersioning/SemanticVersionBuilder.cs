using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Provides a custom constructor for semantic versions and modifies version components and identifiers for the <see cref="SemanticVersion"/> class.</para>
    /// </summary>
    public sealed partial class SemanticVersionBuilder
    {
        private int _major;
        private int _minor;
        private int _patch;

        private readonly List<SemverPreRelease> _preReleases;
        private readonly List<string> _buildMetadata;
        private PreReleaseCollection? _preReleasesCollection;
        private BuildMetadataCollection? _buildMetadataCollection;

        /// <summary>
        ///   <para>Gets the semantic version's collection of pre-release identifiers.</para>
        /// </summary>
        public PreReleaseCollection PreReleases
            => _preReleasesCollection ??= new PreReleaseCollection(this);
        /// <summary>
        ///   <para>Gets the semantic version's collection of build metadata identifiers.</para>
        /// </summary>
        public BuildMetadataCollection BuildMetadata
            => _buildMetadataCollection ??= new BuildMetadataCollection(this);

        /// <summary>
        ///   <para>Gets or sets the semantic version's major version component.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int Major
        {
            get => _major;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MajorNegative);
                _major = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the semantic version's minor version component.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int Minor
        {
            get => _minor;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MinorNegative);
                _minor = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the semantic version's patch version component.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than 0.</exception>
        public int Patch
        {
            get => _patch;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.PatchNegative);
                _patch = value;
            }
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class.</para>
        /// </summary>
        public SemanticVersionBuilder()
            : this(0, 0, 0, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch)
            : this(major, minor, patch, null, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, and <paramref name="preReleases"/>.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <param name="preReleases">A collection of the semantic version's pre-release identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch,
                                      [InstantHandle] IEnumerable<SemverPreRelease>? preReleases)
            : this(major, minor, patch, preReleases, null) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components, <paramref name="preReleases"/> and <paramref name="buildMetadata"/>.</para>
        /// </summary>
        /// <param name="major">The semantic version's major version component.</param>
        /// <param name="minor">The semantic version's minor version component.</param>
        /// <param name="patch">The semantic version's patch version component.</param>
        /// <param name="preReleases">A collection of the semantic version's pre-release identifiers.</param>
        /// <param name="buildMetadata">A collection of the semantic version's build metadata identifiers.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/>, <paramref name="minor"/> or <paramref name="patch"/> is less than 0.</exception>
        /// <exception cref="ArgumentException"><paramref name="buildMetadata"/> contains <see langword="null"/> or an invalid build metadata identifier.</exception>
        public SemanticVersionBuilder(int major, int minor, int patch,
                                      [InstantHandle] IEnumerable<SemverPreRelease>? preReleases,
                                      [InstantHandle] IEnumerable<string>? buildMetadata)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            _major = major;
            _minor = minor;
            _patch = patch;

            _preReleases = preReleases is null ? [] : preReleases.ToList();

            List<string> list;
            if (buildMetadata is not null)
            {
                list = buildMetadata.ToList();
                for (int i = 0; i < list.Count; i++)
                    Utility.ValidateBuildMetadataItem(list[i], nameof(buildMetadata));
            }
            else list = [];

            _buildMetadata = list;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="SemanticVersionBuilder"/> class with the specified <paramref name="version"/>.</para>
        /// </summary>
        /// <param name="version">The <see cref="SemanticVersion"/> object to get the semantic version components and identifiers from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="version"/> is <see langword="null"/>.</exception>
        public SemanticVersionBuilder(SemanticVersion version)
        {
            if (version is null) throw new ArgumentNullException(nameof(version));
            _major = version.Major;
            _minor = version.Minor;
            _patch = version.Patch;
            _preReleases = version._preReleases.ToList();
            _buildMetadata = version._buildMetadata.ToList();
        }

        /// <summary>
        ///   <para>Sets the semantic version's major version component.</para>
        /// </summary>
        /// <param name="major">The major version component to set.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="major"/> is less than 0.</exception>
        public SemanticVersionBuilder WithMajor(int major)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), major, Exceptions.MajorNegative);
            _major = major;
            return this;
        }
        /// <summary>
        ///   <para>Sets the semantic version's minor version component.</para>
        /// </summary>
        /// <param name="minor">The minor version component to set.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minor"/> is less than 0.</exception>
        public SemanticVersionBuilder WithMinor(int minor)
        {
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), minor, Exceptions.MinorNegative);
            _minor = minor;
            return this;
        }
        /// <summary>
        ///   <para>Sets the semantic version's patch version component.</para>
        /// </summary>
        /// <param name="patch">The patch version component to set.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="patch"/> is less than 0.</exception>
        public SemanticVersionBuilder WithPatch(int patch)
        {
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), patch, Exceptions.PatchNegative);
            _patch = patch;
            return this;
        }

        /// <summary>
        ///   <para>Appends the specified <paramref name="preRelease"/> identifier to the semantic version.</para>
        /// </summary>
        /// <param name="preRelease">The pre-release identifier to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public SemanticVersionBuilder AppendPreRelease(SemverPreRelease preRelease)
        {
            _preReleases.Add(preRelease);
            return this;
        }
        /// <summary>
        ///   <para>Removes all pre-release identifiers from the semantic version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public SemanticVersionBuilder ClearPreReleases()
        {
            _preReleases.Clear();
            return this;
        }

        /// <summary>
        ///   <para>Appends the specified build metadata <paramref name="identifier"/> to the semantic version.</para>
        /// </summary>
        /// <param name="identifier">The build metadata identifier to append.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="identifier"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="identifier"/> is not a valid build metadata identifier.</exception>
        public SemanticVersionBuilder AppendBuildMetadata(string identifier)
        {
            if (identifier is null) throw new ArgumentNullException(nameof(identifier));
            Utility.ValidateBuildMetadataItem(identifier, nameof(identifier));
            _buildMetadata.Add(identifier);
            return this;
        }
        /// <summary>
        ///   <para>Removes all build metadata identifiers from the semantic version.</para>
        /// </summary>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public SemanticVersionBuilder ClearBuildMetadata()
        {
            _buildMetadata.Clear();
            return this;
        }

        /// <summary>
        ///   <para>Converts the value of this <see cref="SemanticVersionBuilder"/> to a <see cref="SemanticVersion"/>.</para>
        /// </summary>
        /// <returns>The <see cref="SemanticVersion"/> instance constructed by this <see cref="SemanticVersionBuilder"/>.</returns>
        [Pure] public SemanticVersion ToVersion()
            => new SemanticVersion(_major, _minor, _patch, _preReleases.ToArray(), _buildMetadata.ToArray(), null, null);
        /// <summary>
        ///   <para>Returns the SemVer 2.0.0 compliant string representation of the semantic version.</para>
        /// </summary>
        /// <returns>The SemVer 2.0.0 compliant string representation of the semantic version.</returns>
        [Pure] public override string ToString()
            => ToVersion().ToString();

        /// <summary>
        ///   <para>Represents the collection of pre-release identifiers in a <see cref="SemanticVersionBuilder"/>.</para>
        /// </summary>
        public sealed class PreReleaseCollection : Collection<SemverPreRelease>
        {
            /// <summary>
            ///   <para>Initializes a new instance of the <see cref="PreReleaseCollection"/> class as a wrapper for the specified <paramref name="builder"/>'s collection of pre-release identifiers.</para>
            /// </summary>
            /// <param name="builder">The semantic version builder to act as a wrapper for.</param>
            /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
            public PreReleaseCollection(SemanticVersionBuilder builder) : base(builder._preReleases) { }
        }
        /// <summary>
        ///   <para>Represents the collection of build metadata identifiers in a <see cref="SemanticVersionBuilder"/>.</para>
        /// </summary>
        public sealed class BuildMetadataCollection : Collection<string>
        {
            /// <summary>
            ///   <para>Initializes a new instance of the <see cref="PreReleaseCollection"/> class as a wrapper for the specified <paramref name="builder"/>'s collection of build metadata identifiers.</para>
            /// </summary>
            /// <param name="builder">The semantic version builder to act as a wrapper for.</param>
            /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <see langword="null"/>.</exception>
            public BuildMetadataCollection(SemanticVersionBuilder builder) : base(builder._buildMetadata) { }

            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="item"/> is not a valid build metadata identifier.</exception>
            protected override void InsertItem(int index, string item)
            {
                if (item is null) throw new ArgumentNullException(nameof(item));
                Utility.ValidateBuildMetadataItem(item, nameof(item));
                base.InsertItem(index, item);
            }
            /// <inheritdoc/>
            /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="item"/> is not a valid build metadata identifier.</exception>
            protected override void SetItem(int index, string item)
            {
                if (item is null) throw new ArgumentNullException(nameof(item));
                Utility.ValidateBuildMetadataItem(item, nameof(item));
                base.SetItem(index, item);
            }
        }

    }
}
