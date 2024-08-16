using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] public static VersionRange operator ~(Comparator comparator)
        {
            if (comparator is null) throw new ArgumentNullException(nameof(comparator));
            return VersionRange.FromTuple(Complement(comparator));
        }

        [Pure] private static (Comparator, Comparator?) Complement(Comparator comparator)
        {
            (PrimitiveComparator? left, PrimitiveComparator? right) = comparator.AsPrimitives();

            if (left is null)
                return (right is null ? PrimitiveComparator.None : ComplementPrimitive(right.Operator, right.Operand), null);

            if (left.Operator.IsEQ())
                return (PrimitiveComparator.LessThan(left.Operand), PrimitiveComparator.GreaterThan(left.Operand));

            return (
                ComplementPrimitive(left.Operator, left.Operand),
                right is null ? null : ComplementPrimitive(right.Operator, right.Operand)
            );
        }

        [Pure] internal static Comparator ComplementPrimitive(PrimitiveOperator @operator, SemanticVersion operand)
        {
            Debug.Assert(!@operator.IsEQ());

            if (@operator == PrimitiveOperator.LessThan && operand.Equals(SemanticVersion.MinValue))
                return XRangeComparator.All;

            return new PrimitiveComparator(operand, @operator.Invert());
        }

    }
}
