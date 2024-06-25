using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] public static ComparatorSet operator &(Comparator left, Comparator right)
            => ComparatorSet.FromTuple(Intersect(left, right));

        [Pure] internal static (Comparator?, Comparator?) Intersect(Comparator left, Comparator right)
        {
            if (left is PrimitiveComparator primitiveLeft && right is PrimitiveComparator primitiveRight)
            {
                PrimitiveComparator? result = TryIntersectPrimitivesSingle(primitiveLeft, primitiveRight);
                return result is not null ? (result, null) : (left, right);
            }

            (PrimitiveComparator? left1, PrimitiveComparator? right1) = left.AsPrimitives();
            (PrimitiveComparator? left2, PrimitiveComparator? right2) = right.AsPrimitives();

            return IntersectAdvanced(left1, right1, left2, right2, left, right);
        }

        [Pure] private static (Comparator?, Comparator?) IntersectAdvanced(PrimitiveComparator? left1, PrimitiveComparator? right1,
                                                                           PrimitiveComparator? left2, PrimitiveComparator? right2,
                                                                           Comparator sugared1, Comparator sugared2)
        {
            // =1.2.3 & ^1.0.0 ⇒ =1.2.3
            // =1.2.3 & ^2.0.0 ⇒ <0.0.0-0
            if (left1?.Operator.IsEQ() == true)
                return (sugared2.IsSatisfiedBy(left1.Operand) ? sugared1 : PrimitiveComparator.None, null);
            // ^1.0.0 & =1.2.3 ⇒ =1.2.3
            // ^2.0.0 & =1.2.3 ⇒ <0.0.0-0
            if (left2?.Operator.IsEQ() == true)
                return (sugared1.IsSatisfiedBy(left2.Operand) ? sugared2 : PrimitiveComparator.None, null);

            PrimitiveComparator? leftResult = left1 is null ? left2 : left2 is null ? left1 : IntersectGreaterThan(left1, left2);
            PrimitiveComparator? rightResult = right1 is null ? right2 : right2 is null ? right1 : IntersectLessThan(right1, right2);

            if (rightResult is not null && leftResult is not null)
            {
                // see if the two primitives can be turned into one (e.g. =1.2.3 or <0.0.0-0)
                PrimitiveComparator? singleResult = TryIntersectOppositeSingle(leftResult, rightResult);
                // if it's null, it's either an equality primitive or a <0.0.0-0
                if (singleResult is not null) return (singleResult, null);
            }

            // At this point, we only have >, >=, <, <= primitives

            if (sugared1 is AdvancedComparator advanced1)
            {
                if (ReferenceEquals(leftResult, left1) && ReferenceEquals(rightResult, right1)) return (sugared1, null);
                // TODO: attempt to resugar
            }
            if (sugared2 is AdvancedComparator advanced2)
            {
                if (ReferenceEquals(leftResult, left2) && ReferenceEquals(rightResult, right2)) return (sugared2, null);
                // TODO: attempt to resugar
            }

            if (leftResult is null)
                return (rightResult is null ? XRangeComparator.All : rightResult, null);
            if (rightResult is null)
                return (leftResult, null);

            return (leftResult, rightResult);
        }

        [Pure] internal static PrimitiveComparator? TryIntersectPrimitivesSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Same direction primitive comparators (not equality)
            if (Utility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = Utility.CompareSameDirection(left, right);
                return (left.Operator.IsGTOrGTE() ? cmp >= 0 : cmp <= 0) ? left : right;
            }

            // =1.2.3 & <3.4.5 ⇒ =1.2.3
            // =1.2.3 & >3.4.5 ⇒ <0.0.0-0
            if (left.Operator.IsEQ())
                return right.IsSatisfiedByCore(left.Operand) ? left : PrimitiveComparator.None;

            // >1.2.3 & =3.4.5 ⇒ =3.4.5
            // <1.2.3 & =3.4.5 ⇒ <0.0.0-0
            if (right.Operator.IsEQ())
                return left.IsSatisfiedByCore(right.Operand) ? right : PrimitiveComparator.None;

            // At this point, the comparison directions are different
            return TryIntersectOppositeSingle(left, right);
        }
        [Pure] private static PrimitiveComparator? TryIntersectOppositeSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // The arguments must compare in opposite directions
            Debug.Assert(!left.Operator.IsEQ() && !right.Operator.IsEQ());
            Debug.Assert(left.Operator.IsGTOrGTE() != right.Operator.IsGTOrGTE());

            //  <1.2.3 &  >3.4.5 ⇒ <0.0.0-0
            //  >3.4.5 &  <1.2.3 ⇒ <0.0.0-0
            // >=1.2.3 &  <1.2.3 ⇒ <0.0.0-0
            if (!left.IsSatisfiedByCore(right.Operand) || !right.IsSatisfiedByCore(left.Operand))
                return PrimitiveComparator.None;

            // >=1.2.3 & <=1.2.3 ⇒ =1.2.3
            if (left.Operator.IsSthThanOrEqual() && right.Operator.IsSthThanOrEqual() && left.Operand.Equals(right.Operand))
                return PrimitiveComparator.Equal(left.Operand);

            // The comparators can't be combined into a primitive, meaning that in order
            // to intersect these two, a memory allocation (ComparatorSet) is required.

            //  >1.2.3 &  <3.4.5 ⇒ >1.2.3 <3.4.5
            // >=1.2.3 & <=3.4.5 ⇒ >=1.2.3 <=3.4.5
            return null;
        }

        [Pure] private static PrimitiveComparator IntersectGreaterThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be GT/GTE
            Debug.Assert(left.Operator.IsGTOrGTE() && right.Operator.IsGTOrGTE());

            return Utility.CompareSameDirection(left, right) >= 0 ? left : right;
        }
        [Pure] private static PrimitiveComparator IntersectLessThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be LT/LTE
            Debug.Assert(left.Operator.IsLTOrLTE() && right.Operator.IsLTOrLTE());

            return Utility.CompareSameDirection(left, right) <= 0 ? left : right;
        }

    }
}
