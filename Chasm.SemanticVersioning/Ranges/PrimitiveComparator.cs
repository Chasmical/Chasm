using System;
using System.ComponentModel;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed class PrimitiveComparator : Comparator
    {
        public override bool IsPrimitive => true;

        public SemanticVersion Operand { get; }
        public PrimitiveOperator Operator { get; }

        public PrimitiveComparator(SemanticVersion operand, PrimitiveOperator @operator)
        {
            if (operand is null) throw new ArgumentNullException(nameof(operand));
            if (@operator > PrimitiveOperator.LessThanOrEqual)
                throw new InvalidEnumArgumentException(nameof(@operator), (int)@operator, typeof(PrimitiveOperator));
            Operand = operand;
            Operator = @operator;
        }

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
            => CanMatchPreRelease(Operand, major, minor, patch);
        /// <inheritdoc/>
        [Pure] public override bool IsSatisfiedBy(SemanticVersion? version)
        {
            if (version is null) return false;
            int res = version.CompareTo(Operand);
            return Operator switch
            {
                PrimitiveOperator.Equal => res == 0,
                PrimitiveOperator.GreaterThan => res > 0,
                PrimitiveOperator.GreaterThanOrEqual => res >= 0,
                PrimitiveOperator.LessThan => res < 0,
                PrimitiveOperator.LessThanOrEqual => res <= 0,
#if NET7_0_OR_GREATER
                _ => throw new System.Diagnostics.UnreachableException(),
#else
                _ => throw new InvalidOperationException(),
#endif
            };
        }

        [Pure] public static PrimitiveComparator Equal(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.Equal);
        [Pure] public static PrimitiveComparator GreaterThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThan);
        [Pure] public static PrimitiveComparator LessThan(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.LessThan);
        [Pure] public static PrimitiveComparator GreaterThanOrEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.GreaterThanOrEqual);
        [Pure] public static PrimitiveComparator LessThanOrEqual(SemanticVersion operand)
            => new PrimitiveComparator(operand, PrimitiveOperator.LessThanOrEqual);

        /// <inheritdoc/>
        [Pure] protected internal override int CalculateLength()
            => Operand.CalculateLength() + (Operator > PrimitiveOperator.LessThan ? 2 : 1);
        /// <inheritdoc/>
        protected internal override void BuildString(ref SpanBuilder sb)
        {
            switch (Operator)
            {
                case PrimitiveOperator.Equal:
                    sb.Append('=');
                    break;
                case PrimitiveOperator.GreaterThan:
                    sb.Append('>');
                    break;
                case PrimitiveOperator.LessThan:
                    sb.Append('<');
                    break;
                case PrimitiveOperator.GreaterThanOrEqual:
                    sb.Append('>');
                    goto case PrimitiveOperator.Equal;
                case PrimitiveOperator.LessThanOrEqual:
                    sb.Append('<');
                    goto case PrimitiveOperator.Equal;
            }
            Operand.BuildString(ref sb);
        }

    }
}
