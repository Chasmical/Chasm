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
            // ~x.x.x    ⇒ *
            // ~x.x.x-rc ⇒ *
            // ~x.2.3    ⇒ *
            // ~x.2.3-rc ⇒ *
            if (!Operand.Major.IsNumeric) return (null, null);
            int major = Operand.Major.AsNumber; // M is numeric

            if (!Operand.Minor.IsNumeric)
            {
                // ~M.x.x[-rr] ⇒ >=M.0.0[-rr] <M+1.0.0-0

                // ~1.x.x    ⇒ >=1.0.0    <2.0.0-0
                // ~1.x.x-rc ⇒ >=1.0.0-rc <2.0.0-0 (TODO: node-semver ignores pre-releases if there are unspecified components)
                // ~1.x.3    ⇒ >=1.0.3    <2.0.0-0 (TODO: node-semver ignores specified patch if minor is unspecified)
                // ~1.x.3-rc ⇒ >=1.0.3-rc <2.0.0-0 (TODO: node-semver ignores specified patch and pre-releases if minor is unspecified)
                if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(Operand)),
                    LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default))
                );
            }
            int minor = Operand.Minor.AsNumber; // M is numeric, m is numeric
            int patch = (int)Operand.Patch;

            // ~M.m.p[-rr] ⇒ >=M.m.p[-rr] <M.m+1.0-0

            // ~1.2.x    ⇒ >=1.2.0    <1.3.0-0
            // ~1.2.x-rc ⇒ >=1.2.0-rc <1.3.0-0 (TODO: node-semver ignores pre-releases if there are unspecified components)
            // ~1.2.3    ⇒ >=1.2.3    <1.3.0-0
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
