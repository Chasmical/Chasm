using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] public static VersionRange operator ~(Comparator comparator)
            => VersionRange.FromTuple(Complement(comparator));

        [Pure] public static (ComparatorSet, ComparatorSet?) Complement(Comparator comparator)
        {
            if (ReferenceEquals(comparator, PrimitiveComparator.None))
                return (XRangeComparator.All, null);
            if (ReferenceEquals(comparator, XRangeComparator.All))
                return (PrimitiveComparator.None, null);

            (PrimitiveComparator? left, PrimitiveComparator? right) = comparator.AsPrimitives();

            if (left?.Operator.IsEQ() == true)
                return (PrimitiveComparator.LessThan(left.Operand), PrimitiveComparator.GreaterThan(left.Operand));

            if (left is null)
                return right is null ? (PrimitiveComparator.None, null) : (ComplementPrimitive(right), null);

            return (ComplementPrimitive(left), right is null ? null : ComplementPrimitive(right));
        }

        [Pure] private static PrimitiveComparator ComplementPrimitive(PrimitiveComparator primitive)
        {
            Debug.Assert(!primitive.Operator.IsEQ());
            return new PrimitiveComparator(primitive.Operand, primitive.Operator.Invert());
        }

    }
}
