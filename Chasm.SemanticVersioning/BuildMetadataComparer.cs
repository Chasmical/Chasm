using System;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Compares two semantic versions for equivalence, while also taking into account their build metadata identifiers.</para>
    /// </summary>
    [Obsolete("Use the SemverComparer.IncludeBuildMetadata property instead.")]
    public sealed class BuildMetadataComparer : SemverComparer
    {
        private BuildMetadataComparer() { }

        /// <summary>
        ///   <para>Gets an instance of the <see cref="BuildMetadataComparer"/> type.</para>
        /// </summary>
        public static BuildMetadataComparer Instance { get; } = new BuildMetadataComparer();

        /// <inheritdoc/>
        [Pure] public override int Compare(SemanticVersion? a, SemanticVersion? b)
            => IncludeBuildMetadata.Compare(a, b);
        /// <inheritdoc/>
        [Pure] public override bool Equals(SemanticVersion? a, SemanticVersion? b)
            => IncludeBuildMetadata.Equals(a, b);
        /// <inheritdoc/>
        [Pure] public override int GetHashCode(SemanticVersion? version)
            => IncludeBuildMetadata.GetHashCode(version);

        /// <inheritdoc/>
        [Pure] public override int Compare(PartialComponent a, PartialComponent b)
            => IncludeBuildMetadata.Compare(a, b);
        /// <inheritdoc/>
        [Pure] public override bool Equals(PartialComponent a, PartialComponent b)
            => IncludeBuildMetadata.Equals(a, b);
        /// <inheritdoc/>
        [Pure] public override int GetHashCode(PartialComponent version)
            => IncludeBuildMetadata.GetHashCode(version);

        /// <inheritdoc/>
        [Pure] public override int Compare(PartialVersion? a, PartialVersion? b)
            => IncludeBuildMetadata.Compare(a, b);
        /// <inheritdoc/>
        [Pure] public override bool Equals(PartialVersion? a, PartialVersion? b)
            => IncludeBuildMetadata.Equals(a, b);
        /// <inheritdoc/>
        [Pure] public override int GetHashCode(PartialVersion? version)
            => IncludeBuildMetadata.GetHashCode(version);

    }
}
