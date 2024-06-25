using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] public static VersionRange operator |(Comparator left, Comparator right)
            => VersionRange.FromTuple(Union(left, right));

        [Pure] public static (ComparatorSet, ComparatorSet?) Union(Comparator left, Comparator right)
        {
            if (left is PrimitiveComparator primitiveLeft && right is PrimitiveComparator primitiveRight)
            {
                PrimitiveComparator? result = TryUnionPrimitivesSingle(primitiveLeft, primitiveRight);
                return result is not null ? (result, null) : (left, right);
            }

            (PrimitiveComparator? left1, PrimitiveComparator? right1) = left.AsPrimitives();
            (PrimitiveComparator? left2, PrimitiveComparator? right2) = right.AsPrimitives();

            return UnionAdvanced(left1, right1, left2, right2, left, right);
        }

        [Pure] private static (ComparatorSet, ComparatorSet?) UnionAdvanced(PrimitiveComparator? left1, PrimitiveComparator? right1,
                                                                            PrimitiveComparator? left2, PrimitiveComparator? right2,
                                                                            Comparator sugared1, Comparator sugared2)
        {
            // =1.2.3 | ^1.0.0 ⇒ ^1.0.0
            // =1.2.3 | ^2.0.0 ⇒ =1.2.3 || ^2.0.0
            if (left1?.Operator.IsEQ() == true)
                return sugared2.IsSatisfiedBy(left1.Operand) ? (sugared2, null) : (sugared1, sugared2);
            // ^1.0.0 | =1.2.3 ⇒ ^1.0.0
            // ^2.0.0 | =1.2.3 ⇒ ^2.0.0 || =1.2.3
            if (left2?.Operator.IsEQ() == true)
                return sugared1.IsSatisfiedBy(left2.Operand) ? (sugared1, null) : (sugared1, sugared2);



        }

        [Pure] private static PrimitiveComparator? TryUnionPrimitivesSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Same direction primitive comparators (not equality)
            if (Utility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = Utility.CompareSameDirection(left, right);
                return (left.Operator.IsGTOrGTE() ? cmp <= 0 : cmp >= 0) ? left : right;
            }

            // =1.2.3 | <3.4.5 ⇒ <3.4.5
            // =1.2.3 | >3.4.5 ⇒ =1.2.3 || >3.4.5
            if (left.Operator.IsEQ())
                return right.IsSatisfiedByCore(left.Operand) ? right : null;

            // <3.4.5 | =1.2.3 ⇒ <3.4.5
            // >3.4.5 | =1.2.3 ⇒ >3.4.5 || =1.2.3
            if (right.Operator.IsEQ())
                return left.IsSatisfiedByCore(right.Operand) ? left : null;

            // At this point the comparison directions are different
            // >1.2.3 | <3.4.5 ⇒ *
            // <3.4.5 | >1.2.3 ⇒ *
            if (left.IsSatisfiedByCore(right.Operand))
                return PrimitiveComparator.All;

            //  >1.2.3 | <=1.2.3 ⇒ *
            // >=1.2.3 |  <1.2.3 ⇒ *
            if ((left.Operator.IsSthThanOrEqual() || right.Operator.IsSthThanOrEqual()) && left.Operand.Equals(right.Operand))
                return PrimitiveComparator.All;

            // The comparators can't be combined into a primitive, meaning that in order
            // to union these two, a memory allocation (VersionRange) is required.

            // <1.2.3 | >3.4.5
            // <1.2.3 | >1.2.3
            return null;
        }

    }
}
