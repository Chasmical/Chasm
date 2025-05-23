﻿using System;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> caret version comparator.</para>
    /// </summary>
    public sealed class CaretComparator : AdvancedComparator, IEquatable<CaretComparator>
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="CaretComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The caret comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public CaretComparator(PartialVersion operand) : base(operand) { }

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            // ^x.x.x    ⇒ *
            // ^x.x.x-rc ⇒ *
            // ^x.2.3    ⇒ *
            // ^x.2.3-rc ⇒ *
            // (Note: node-semver ignores minor, patch and pre-releases here)
            if (!Operand.Major.IsNumeric) return (null, null);
            int major = Operand.Major.AsNumber; // M is numeric

            if (major != 0 || !Operand.Minor.IsNumeric)
            {
                // ^M.m.p[-rr] ⇒ >=M.m.p[-rr] <M+1.0.0-0
                // ^M.x.x[-rr] ⇒ >=M.0.0      <M+1.0.0-0

                // ^1.2.3    ⇒ >=1.2.3    <2.0.0-0
                // ^1.2.3-rc ⇒ >=1.2.3-rc <2.0.0-0

                // (Note: node-semver ignores components and pre-releases after an unspecified component)
                // ^1.2.x    ⇒ >=1.2.0    <2.0.0-0
                // ^1.2.x-rc ⇒ >=1.2.0    <2.0.0-0
                // ^1.x.x    ⇒ >=1.0.0    <2.0.0-0
                // ^1.x.x-rc ⇒ >=1.0.0    <2.0.0-0
                // ^1.x.3    ⇒ >=1.0.0    <2.0.0-0
                // ^1.x.3-rc ⇒ >=1.0.0    <2.0.0-0
                // ^0.x.x    ⇒ >=0.0.0    <1.0.0-0
                // ^0.x.x-rc ⇒ >=0.0.0    <1.0.0-0

                if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                return (
                    GreaterThanOrEqual(RangeUtility.NodeSemverTrim(Operand)),
                    LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, null, null))
                );
            }
            int minor = Operand.Minor.AsNumber; // M is 0, m is numeric

            if (minor != 0 || !Operand.Patch.IsNumeric)
            {
                // ^0.m.p[-rr] ⇒ >=0.m.p[-rr] <0.m+1.0-0
                // ^0.m.x[-rr] ⇒ >=0.m.0      <0.m+1.0-0

                // ^0.2.3    ⇒ >=0.2.3    <0.3.0-0
                // ^0.2.3-rc ⇒ >=0.2.3-rc <0.3.0-0

                // (Note: node-semver ignores components and pre-releases after an unspecified component)
                // ^0.2.x    ⇒ >=0.2.0    <0.3.0-0
                // ^0.2.x-rc ⇒ >=0.2.0    <0.3.0-0
                // ^0.0.x    ⇒ >=0.0.0    <0.1.0-0
                // ^0.0.x-rc ⇒ >=0.0.0    <0.1.0-0

                if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                return (
                    GreaterThanOrEqual(RangeUtility.NodeSemverTrim(Operand)),
                    LessThan(new SemanticVersion(0, minor + 1, 0, SemverPreRelease.ZeroArray, null, null, null))
                );
            }
            int patch = Operand.Patch.AsNumber; // M is 0, m is 0, p is numeric

            // ^0.0.p[-rr] ⇒ >=0.0.p[-rr] <0.0.p+1-0

            // ^0.0.3    ⇒ >=0.0.3    <0.0.4-0
            // ^0.0.3-rc ⇒ >=0.0.3-rc <0.0.4-0
            // ^0.0.0    ⇒ >=0.0.0    <0.0.1-0
            // ^0.0.0-rc ⇒ >=0.0.0-rc <0.0.1-0

            if (patch == int.MaxValue) throw new InvalidOperationException(Exceptions.PatchTooBig);
            return (
                GreaterThanOrEqual(new SemanticVersion(0, 0, patch, Operand._preReleases, null, Operand._preReleasesReadonly, null)),
                LessThan(new SemanticVersion(0, 0, patch + 1, SemverPreRelease.ZeroArray, null, null, null))
            );
        }

        /// <summary>
        ///   <para>Determines whether this caret comparator is equal to another specified caret comparator.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="other">The caret comparator to compare with this caret comparator.</param>
        /// <returns><see langword="true"/>, if this caret comparator is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public bool Equals([NotNullWhen(true)] CaretComparator? other)
        {
            if (ReferenceEquals(this, other)) return true;
            return other is not null && Operand.Equals(other.Operand);
        }
        /// <summary>
        ///   <para>Determines whether this caret comparator is equal to the specified <paramref name="obj"/>.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this caret comparator.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="CaretComparator"/> instance equal to this caret comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as CaretComparator);
        /// <summary>
        ///   <para>Returns a hash code for this caret comparator.<br/>Build metadata is ignored and non-numeric version components are considered equal in this comparison. For build metadata-sensitive comparison, use <see cref="SemverComparer.IncludeBuild"/>, and for version component character-sensitive comparison, use <see cref="SemverComparer.DiffWildcards"/>.</para>
        /// </summary>
        /// <returns>A hash code for this caret comparator.</returns>
        [Pure] public override int GetHashCode()
        {
            // Add the type hashcode as well to avoid collisions between different types of comparators
            return HashCode.Combine(GetType(), Operand);
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => Operand.CalculateLength() + 1; // '^'
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            sb.Append('^');
            Operand.BuildString(ref sb);
        }

    }
}
