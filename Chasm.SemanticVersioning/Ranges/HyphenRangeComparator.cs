using System;
using System.Diagnostics;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    using static PrimitiveComparator;

    public sealed class HyphenRangeComparator : AdvancedComparator
    {
        public PartialVersion From => base.Operand;
        public PartialVersion To { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete($"You should use the {nameof(From)} property instead, when you know it's an instance of the {nameof(HyphenRangeComparator)} class.")]
        public new PartialVersion Operand => base.Operand;

        public HyphenRangeComparator(PartialVersion from, PartialVersion to) : base(from) => To = to;

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
            => CanMatchPreRelease(From, major, minor, patch) || CanMatchPreRelease(To, major, minor, patch);

        /// <inheritdoc/>
        [Pure] protected override (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives()
        {
            return (ConvertFrom(From), ConvertTo(To));

            static PrimitiveComparator? ConvertFrom(PartialVersion from)
            {
                // x.x.x - ... ⇒ * ...
                if (!from.Major.IsNumeric) return null;

                // 1.x.x - ... ⇒ >=1.0.0 ...
                if (!from.Minor.IsNumeric)
                    return GreaterThanOrEqual(new SemanticVersion(from.Major.AsNumber, 0, 0, null, null, default));

                // 1.2.x - ... ⇒ >=1.2.0 ...
                if (!from.Patch.IsNumeric)
                    return GreaterThanOrEqual(new SemanticVersion(from.Major.AsNumber, from.Minor.AsNumber, 0, null, null, default));

                // 1.2.3 - ... ⇒ >=1.2.3 ...
                return GreaterThanOrEqual(new SemanticVersion(from));
            }
            static PrimitiveComparator? ConvertTo(PartialVersion to)
            {
                // ... - x.x.x ⇒ ... *
                if (!to.Major.IsNumeric) return null;

                // ... - 1.x.x ⇒ ... <2.0.0-0
                if (!to.Minor.IsNumeric)
                {
                    int major = to.Major.AsNumber;
                    if (major == int.MaxValue) throw new InvalidOperationException(Exceptions.MajorTooBig);
                    return LessThan(new SemanticVersion(major + 1, 0, 0, SemverPreRelease.ZeroArray, null, default));
                }

                // ... - 1.2.x ⇒ ... <1.3.0-0
                if (!to.Patch.IsNumeric)
                {
                    int minor = to.Minor.AsNumber;
                    if (minor == int.MaxValue) throw new InvalidOperationException(Exceptions.MinorTooBig);
                    return LessThan(new SemanticVersion(to.Major.AsNumber, minor + 1, 0, SemverPreRelease.ZeroArray, null, default));
                }

                // ... - 1.2.3 ⇒ ... <=1.2.3
                return LessThanOrEqual(new SemanticVersion(to));
            }
        }

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => From.CalculateLength() + To.CalculateLength() + 3; // ' - '
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            From.BuildString(ref sb);
            sb.Append(" - ");
            To.BuildString(ref sb);
        }

    }
}
