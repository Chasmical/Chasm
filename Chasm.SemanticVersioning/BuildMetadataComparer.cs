using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    /// <summary>
    ///   <para>Compares two semantic versions for equivalence, while also taking into account their build metadata identifiers.</para>
    /// </summary>
    [Obsolete("Use the SemverComparer.IncludeBuildMetadata property instead.")]
    public sealed class BuildMetadataComparer : IComparer, IEqualityComparer
                                              , IComparer<SemanticVersion>, IEqualityComparer<SemanticVersion>
    {
        private BuildMetadataComparer() { }

        private readonly SemverComparer Comparer = SemverComparer.IncludeBuildMetadata;

        /// <summary>
        ///   <para>Gets an instance of the <see cref="BuildMetadataComparer"/> type.</para>
        /// </summary>
        public static BuildMetadataComparer Instance { get; } = new BuildMetadataComparer();

        int IComparer.Compare(object? a, object? b) => ((IComparer)Comparer).Compare(a, b);
        bool IEqualityComparer.Equals(object? a, object? b) => ((IEqualityComparer)Comparer).Equals(a, b);
        int IEqualityComparer.GetHashCode(object? obj) => ((IEqualityComparer)Comparer).GetHashCode(obj!);

        /// <inheritdoc cref="SemverComparer.Compare(SemanticVersion?, SemanticVersion?)"/>
        [Pure] public int Compare(SemanticVersion? a, SemanticVersion? b) => Comparer.Compare(a, b);
        /// <inheritdoc cref="SemverComparer.Equals(SemanticVersion?, SemanticVersion?)"/>
        [Pure] public bool Equals(SemanticVersion? a, SemanticVersion? b) => Comparer.Equals(a, b);
        /// <inheritdoc cref="SemverComparer.GetHashCode(SemanticVersion?)"/>
        [Pure] public int GetHashCode(SemanticVersion? version) => Comparer.GetHashCode(version);

    }
}
