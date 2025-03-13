using System;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> tilde version comparator.</para>
    /// </summary>
    public sealed class TildeComparator : AdvancedComparator, IEquatable<TildeComparator>
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="TildeComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The tilde comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public TildeComparator(PartialVersion operand) : base(operand) { }

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            // ~x.x.x    ⇒ *
            // ~x.x.x-rc ⇒ *
            // ~x.2.3    ⇒ *
            // ~x.2.3-rc ⇒ *
            // (Note: node-semver ignores minor, patch and pre-releases here)
            if (!Operand.Major.IsNumeric) return (null, null);
            int major = Operand.Major.AsNumber; // M is numeric

            if (!Operand.Minor.IsNumeric)
            {
                // ~M.x.x[-rr] ⇒ >=M.0.0 <M+1.0.0-0

                // (Note: node-semver ignores components and pre-releases after an unspecified component)
                // ~1.x.x    ⇒ >=1.0.0    <2.0.0-0
                // ~1.x.x-rc ⇒ >=1.0.0    <2.0.0-0
                // ~1.x.3    ⇒ >=1.0.0    <2.0.0-0
                // ~1.x.3-rc ⇒ >=1.0.0    <2.0.0-0

                if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(major, 0, 0, null, null, null, null)),
                    LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, null, null))
                );
            }
            int minor = Operand.Minor.AsNumber; // M is numeric, m is numeric

            if (!Operand.Patch.IsNumeric)
            {
                // ~M.m.x[-rr] ⇒ >=M.m.0 <M.m+1.0-0

                // (Note: node-semver ignores components and pre-releases after an unspecified component)
                // ~1.2.x    ⇒ >=1.2.0    <1.3.0-0
                // ~1.2.x-rc ⇒ >=1.2.0    <1.3.0-0

                if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(major, minor, 0, null, null, null, null)),
                    LessThan(new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null))
                );
            }
            int patch = Operand.Patch.AsNumber;

            // ~M.m.p[-rr] ⇒ >=M.m.p[-rr] <M.m+1.0-0

            // ~1.2.3    ⇒ >=1.2.3    <1.3.0-0
            // ~1.2.3-rc ⇒ >=1.2.3-rc <1.3.0-0

            if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
            return (
                GreaterThanOrEqual(new SemanticVersion(major, minor, patch, Operand._preReleases, null, Operand._preReleasesReadonly, null)),
                LessThan(new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null))
            );
        }

        /// <summary>
        ///   <para>Determines whether this tilde comparator is equal to another specified tilde comparator.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The tilde comparator to compare with this tilde comparator.</param>
        /// <returns><see langword="true"/>, if this tilde comparator is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals([NotNullWhen(true)] TildeComparator? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other is not null && Operand.Equals(other.Operand);
        }
        /// <summary>
        ///   <para>Determines whether this tilde comparator is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this tilde comparator.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="CaretComparator"/> instance equal to this tilde comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as TildeComparator);
        /// <summary>
        ///   <para>Returns a hash code for this tilde comparator.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <returns>A hash code for this tilde comparator.</returns>
        [Pure] public override int GetHashCode()
        {
            // Add the type hashcode as well to avoid collisions between different types of comparators
            return HashCode.Combine(GetType(), Operand);
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => Operand.CalculateLength() + 1; // '~'
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            sb.Append('~');
            Operand.BuildString(ref sb);
        }

    }
}
