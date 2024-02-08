using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> tilde version comparator.</para>
    /// </summary>
    public sealed class TildeComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="TildeComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The tilde version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public TildeComparator(PartialVersion operand) : base(operand) { }

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            // ~, ~x.x.x ⇒ *
            if (!Operand.Major.IsNumeric) return (null, null);
            int major = Operand.Major.AsNumber;

            if (!Operand.Minor.IsNumeric)
            {
                // >=M.0.0 <M+1.0.0-0
                // ~1, ~1.x.x ⇒ >=1.0.0 <2.0.0-0
                if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(major, 0, 0, null, null, default)),
                    LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default))
                );
            }
            int minor = Operand.Minor.AsNumber;
            int patch = Operand.Patch.GetValueOrZero();

            // >=M.m.p[-rr] <M.m+1.0-0
            // ~1.2, ~1.2.x ⇒ >=1.2.0 <1.3.0-0
            // ~1.2.3-rc ⇒ >=1.2.3-rc <1.3.0-0
            if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
            return (
                GreaterThanOrEqual(new SemanticVersion(major, minor, patch, Operand._preReleases, null, default)),
                LessThan(new SemanticVersion(major, minor + 1, 0, SemverPreRelease.ZeroArray, null, default))
            );
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
