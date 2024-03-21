using System;
using System.Diagnostics;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        public static ComparatorSet operator &(Comparator left, Comparator right)
        {
            if (left is PrimitiveComparator primitiveLeft)
            {
                if (right is PrimitiveComparator primitiveRight)
                    return IntersectPrimitives(primitiveLeft, primitiveRight);
                return IntersectAdvanced(primitiveLeft, (AdvancedComparator)right);
            }
            else
            {
                if (right is PrimitiveComparator primitiveRight)
                    return IntersectAdvanced((AdvancedComparator)left, primitiveRight);
                return IntersectAdvanced((AdvancedComparator)left, (AdvancedComparator)right);
            }
        }

        private static ComparatorSet IntersectAdvanced(PrimitiveComparator primitive, AdvancedComparator advanced)
        {
            (PrimitiveComparator? desugared1, PrimitiveComparator? desugared2) = advanced.ToPrimitives();

            if (desugared1 is null)
                return desugared2 is null ? primitive : IntersectSimpleAdvanced(primitive, desugared2, advanced, desugared2);
            if (desugared2 is null)
                return IntersectSimpleAdvanced(primitive, desugared1, advanced, desugared1);

            bool isLess = primitive.Operator.IsLTOrLTE();
            return IntersectDoubleAdvanced(isLess ? null : primitive, isLess ? primitive : null, desugared1, desugared2, primitive, advanced);
        }
        private static ComparatorSet IntersectAdvanced(AdvancedComparator advanced, PrimitiveComparator primitive)
        {
            (PrimitiveComparator? desugared1, PrimitiveComparator? desugared2) = advanced.ToPrimitives();

            if (desugared1 is null)
                return desugared2 is null ? primitive : IntersectSimpleAdvanced(desugared2, primitive, advanced, desugared2);
            if (desugared2 is null)
                return IntersectSimpleAdvanced(desugared1, primitive, advanced, desugared1);

            bool isLess = primitive.Operator.IsLTOrLTE();
            return IntersectDoubleAdvanced(desugared1, desugared2, isLess ? null : primitive, isLess ? primitive : null, advanced, primitive);
        }
        private static ComparatorSet IntersectAdvanced(AdvancedComparator advancedLeft, AdvancedComparator advancedRight)
        {
            (PrimitiveComparator? left1, PrimitiveComparator? right1) = advancedLeft.ToPrimitives();
            (PrimitiveComparator? left2, PrimitiveComparator? right2) = advancedRight.ToPrimitives();

            return IntersectDoubleAdvanced(left1, right1, left2, right2, advancedLeft, advancedRight);
        }

        // TODO: is this simplified method really needed? IntersectDoubleAdvanced can be used instead
        private static ComparatorSet IntersectSimpleAdvanced(PrimitiveComparator left, PrimitiveComparator right,
                                                             AdvancedComparator advanced, PrimitiveComparator desugared)
        {
            PrimitiveComparator? result = TryIntersectPrimitivesSingle(left, right);

            // If the result is the desugared right operand, return its sugared version
            if (ReferenceEquals(result, desugared)) return advanced;

            // TODO: attempt to "resugar": >=1.2.3 & ^1.0.0 ⇒ ^1.2.3

            // Otherwise, return a comparator set with both primitives
            return result ?? new ComparatorSet([left, right], default);
        }
        private static ComparatorSet IntersectDoubleAdvanced(PrimitiveComparator? left1, PrimitiveComparator? right1,
                                                             PrimitiveComparator? left2, PrimitiveComparator? right2,
                                                             Comparator sugared1, Comparator sugared2)
        {
            // =1.2.3 & ^1.0.0 ⇒ =1.2.3
            // =1.2.3 & ^2.0.0 ⇒ <0.0.0-0
            if (left1?.Operator.IsEQ() == true)
                return sugared2.IsSatisfiedBy(left1.Operand) ? sugared1 : ComparatorSet.None;
            // ^1.0.0 & =1.2.3 ⇒ =1.2.3
            // ^2.0.0 & =1.2.3 ⇒ <0.0.0-0
            if (left2?.Operator.IsEQ() == true)
                return sugared1.IsSatisfiedBy(left2.Operand) ? sugared2 : ComparatorSet.None;

            PrimitiveComparator? leftResult = left1 is null ? left2 : left2 is null ? left1 : IntersectGreaterThan(left1, left2);
            PrimitiveComparator? rightResult = right1 is null ? right2 : right2 is null ? right1 : IntersectLessThan(right1, right2);

            // If the result is the desugared left operand, return its sugared version
            if (ReferenceEquals(leftResult, left1) && ReferenceEquals(rightResult, right1))
                return sugared1;
            // If the result is the desugared right operand, return its sugared version
            if (ReferenceEquals(leftResult, left2) && ReferenceEquals(rightResult, right2))
                return sugared2;

            // TODO: attempt to "resugar": ^1.0.0 & 1.2.3 - 2.0.0 ⇒ ^1.2.3

            if (leftResult is null)
                return rightResult ?? ComparatorSet.All;
            if (rightResult is null)
                return leftResult;

            return new ComparatorSet([leftResult, rightResult], default);
        }

        private static ComparatorSet IntersectPrimitives(PrimitiveComparator left, PrimitiveComparator right)
            => TryIntersectPrimitivesSingle(left, right) ?? new ComparatorSet([left, right], default);
        private static PrimitiveComparator? TryIntersectPrimitivesSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Same direction primitive comparators (not equality) - >1.2.3 & >3.4.5
            if (Utility.SameDirection(left.Operator, right.Operator))
                return left.Operator.IsGTOrGTE() ? IntersectGreaterThan(left, right) : IntersectLessThan(left, right);

            // =1.2.3 & <3.4.5 ⇒ =1.2.3
            // =1.2.3 & >3.4.5 ⇒ <0.0.0-0
            if (left.Operator.IsEQ())
                return right.IsSatisfiedBy(left.Operand) ? left : PrimitiveComparator.None;
            // >1.2.3 & =3.4.5 ⇒ =3.4.5
            // <1.2.3 & =3.4.5 ⇒ <0.0.0-0
            if (right.Operator.IsEQ())
                return left.IsSatisfiedBy(right.Operand) ? right : PrimitiveComparator.None;

            // At this point, we have different comparison directions
            return TryIntersectOppositeSingle(left, right);
        }
        private static PrimitiveComparator IntersectGreaterThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be GT/GTE
            Debug.Assert(left.Operator.IsGTOrGTE());
            Debug.Assert(right.Operator.IsGTOrGTE());

            int cmp = left.Operand.CompareTo(right.Operand);
            //  >1.2.3 &  >3.4.5 ⇒  >3.4.5
            // >=1.2.3 & >=3.4.5 ⇒ >=3.4.5
            if (cmp == 0 && left.Operator != right.Operator)
            {
                //  >1.2.3 & >=1.2.3 ⇒ >1.2.3
                // >=1.2.3 &  >1.2.3 ⇒ >1.2.3
                cmp = left.Operator == PrimitiveOperator.GreaterThan ? 1 : -1;
            }
            return cmp >= 0 ? left : right;
        }
        private static PrimitiveComparator IntersectLessThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be LT/LTE
            Debug.Assert(left.Operator.IsLTOrLTE());
            Debug.Assert(right.Operator.IsLTOrLTE());

            int cmp = left.Operand.CompareTo(right.Operand);
            //  <1.2.3 &  <3.4.5 ⇒  <1.2.3
            // <=1.2.3 & <=3.4.5 ⇒ <=1.2.3
            if (cmp == 0 && left.Operator != right.Operator)
            {
                //  <1.2.3 & <=1.2.3 ⇒ <1.2.3
                // <=1.2.3 &  <1.2.3 ⇒ <1.2.3
                cmp = left.Operator == PrimitiveOperator.LessThan ? -1 : 1;
            }
            return cmp <= 0 ? left : right;
        }
        private static PrimitiveComparator? TryIntersectOppositeSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // left and right have different comparison directions
            Debug.Assert(!left.Operator.IsEQ());
            Debug.Assert(!right.Operator.IsEQ());
            Debug.Assert(left.Operator.IsGTOrGTE() != right.Operator.IsGTOrGTE());

            //  <1.2.3 &  >3.4.5 ⇒ <0.0.0-0
            //  >3.4.5 &  <1.2.3 ⇒ <0.0.0-0
            // >=1.2.3 &  <1.2.3 ⇒ <0.0.0-0
            if (!left.IsSatisfiedBy(right.Operand) || !right.IsSatisfiedBy(left.Operand))
                return PrimitiveComparator.None;

            // >=1.2.3 & <=1.2.3 ⇒ =1.2.3
            if (left.Operand.Equals(right.Operand) && left.Operator.IsSthThanOrEqual() && right.Operator.IsSthThanOrEqual())
                return PrimitiveComparator.Equal(left.Operand);

            // The comparators can't be combined into a primitive, meaning that in order
            // to intersect these two, a memory allocation (ComparatorSet) is required.

            // >1.2.3 & <3.4.5 ⇒ >1.2.3 <3.4.5
            return null;
        }

    }
}
