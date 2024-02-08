using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> caret version comparator.</para>
    /// </summary>
    public sealed class CaretComparator : AdvancedComparator
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="CaretComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The caret version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        public CaretComparator(PartialVersion operand) : base(operand) { }

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            // ^, ^x.x.x ⇒ *
            if (!Operand.Major.IsNumeric) return (null, null);
            int major = Operand.Major.AsNumber; // M is numeric

            if (major != 0 || !Operand.Minor.IsNumeric)
            {
                // >=M.m.p[-rr] <M+1.0.0-0
                // ^1.2.3 ⇒ >=1.2.3 <2.0.0-0
                // ^1, ^1.x.x ⇒ >=1.0.0 <2.0.0-0
                // ^0, ^0.x.x ⇒ >=0.0.0 <1.0.0-0
                // ^0.x.3 ⇒ >=0.0.3 <1.0.0-0 TODO: not in node-semver
                if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(major, Operand.Minor.GetValueOrZero(), Operand.Patch.GetValueOrZero(), null, null, default)),
                    LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default))
                );
            }
            int minor = Operand.Minor.AsNumber; // M is 0, m is numeric

            if (minor != 0 || !Operand.Patch.IsNumeric)
            {
                // >=0.m.p[-rr] <0.m+1.0-0
                // ^0.1.2 ⇒ >=0.1.2 <0.2.0-0
                // ^0.1, ^0.1.x ⇒ >=0.1.0 <0.2.0-0
                // ^0.0, ^0.0.x ⇒ >=0.0.0 <0.1.0-0
                if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                return (
                    GreaterThanOrEqual(new SemanticVersion(0, minor, Operand.Patch.GetValueOrZero(), null, null, default)),
                    LessThan(new SemanticVersion(0, minor + 1, 0, SemverPreRelease.ZeroArray, null, default))
                );
            }
            int patch = Operand.Patch.AsNumber; // M is 0, m is 0, p is numeric

            // >=0.0.p[-rr] <0.0.p+1-0
            // ^0.0.1 ⇒ >=0.0.1 <0.0.2-0
            // ^0.0.0 ⇒ >=0.0.0 <0.0.1-0
            if (patch == int.MaxValue) throw new InvalidOperationException(Exceptions.PatchTooBig);
            return (
                GreaterThanOrEqual(new SemanticVersion(0, 0, patch, null, null, default)),
                LessThan(new SemanticVersion(0, 0, patch + 1, SemverPreRelease.ZeroArray, null, default))
            );
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
