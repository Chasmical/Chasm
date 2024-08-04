using JetBrains.Annotations;
using System.Diagnostics;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] public static VersionRange operator |(Comparator left, Comparator right)
            => VersionRange.FromTuple(Union(left, right));

        [Pure] internal static (ComparatorSet?, ComparatorSet?) Union(Comparator left, Comparator right)
        {
            (Comparator?, Comparator?) tuple = Union(left, right, out bool isRange);
            return isRange ? tuple : (ComparatorSet.FromTuple(tuple), null);
        }
        [Pure] internal static (Comparator?, Comparator?) Union(Comparator left, Comparator right, out bool isRange)
        {
            if (left is PrimitiveComparator primitiveLeft && right is PrimitiveComparator primitiveRight)
            {
                isRange = true;
                Comparator? result = TryUnionPrimitivesSingle(primitiveLeft, primitiveRight);
                return result is not null ? (result, null) : (left, right);
            }

            (PrimitiveComparator? left1, PrimitiveComparator? right1) = left.AsPrimitives();
            (PrimitiveComparator? left2, PrimitiveComparator? right2) = right.AsPrimitives();

            return UnionAdvanced(left1, right1, left2, right2, left, right, out isRange);
        }

        [Pure] internal static (Comparator?, Comparator?) UnionAdvanced(PrimitiveComparator? left1, PrimitiveComparator? right1,
                                                                        PrimitiveComparator? left2, PrimitiveComparator? right2,
                                                                        Comparator sugared1, Comparator sugared2, out bool isRange)
        {
            isRange = true;
            // =1.2.3 | ^1.0.0 ⇒ ^1.0.0
            // =1.2.3 | ^2.0.0 ⇒ =1.2.3 || ^2.0.0
            if (left1?.Operator.IsEQ() == true)
                return sugared2.IsSatisfiedBy(left1.Operand) ? (sugared2, null) : (sugared1, sugared2);
            // ^1.0.0 | =1.2.3 ⇒ ^1.0.0
            // ^2.0.0 | =1.2.3 ⇒ ^2.0.0 || =1.2.3
            if (left2?.Operator.IsEQ() == true)
                return sugared1.IsSatisfiedBy(left2.Operand) ? (sugared1, null) : (sugared1, sugared2);

            // >=1.0.0 <2.0.0-0 | >=3.0.0 <4.0.0-0 ⇒ as is
            //         --------   -------
            if (right1 is not null && left2 is not null && !Utility.DoComparatorsComplement(right1, left2))
                return (sugared1, sugared2);

            // >=3.0.0 <4.0.0-0 | >=1.0.0 <2.0.0-0 ⇒ as is
            // -------                    --------
            if (right2 is not null && left1 is not null && !Utility.DoComparatorsComplement(right2, left1))
                return (sugared1, sugared2);

            isRange = false;

            // >1.2.3 | * ⇒ *
            // <1.2.3 | * ⇒ *
            // >1.2.3 | >2.3.4 ⇒ >1.2.3
            // <1.2.3 | <2.3.4 ⇒ <2.3.4

            // -1 - first, 1 - second, 0 - either
            int leftC = Utility.CompareComparators(left1, left2);
            int rightC = Utility.CompareComparators(right1, right2);

            // TODO: determine preference based on whether a comparator is advanced
            if (leftC == 0 && rightC == 0 && false)
            {
                if (sugared1 is AdvancedComparator) return (sugared1, null);
                if (sugared2 is AdvancedComparator) return (sugared2, null);
                return (sugared1, null);
            }
            if (leftC <= 0 && rightC <= 0) return (sugared1, null);
            if (leftC >= 0 && rightC >= 0) return (sugared2, null);

            PrimitiveComparator? leftResult = leftC >= 0 ? left1 : left2;
            PrimitiveComparator? rightResult = rightC >= 0 ? right1 : right2;

            if (sugared1 is AdvancedComparator advanced1)
            {
                AdvancedComparator? resugared = Resugar(advanced1, leftResult, rightResult);
                if (resugared is not null) return (resugared, null);
            }
            if (sugared2 is AdvancedComparator advanced2)
            {
                AdvancedComparator? resugared = Resugar(advanced2, leftResult, rightResult);
                if (resugared is not null) return (resugared, null);
            }

            // At this point, simple combining and resugaring failed, so we'll just AND the results

            // Example (kind of): ^0.0.3          |  ~0.0.4          ⇒ >=0.0.3 <0.1.0-0
            //                   >=0.0.3 <0.0.4-0 | >=0.0.4 <0.1.0-0
            // TODO: This would resugar as ~0.0.3 though. I'm not sure if there are any cases that would run this branch.

            return (leftResult, rightResult);
        }

        [Pure] internal static Comparator? TryUnionPrimitivesSingle(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Same direction primitive comparators (not equality)
            if (Utility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = Utility.CompareComparators(left, right);
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
                return XRangeComparator.All;

            //  >1.2.3 | <=1.2.3 ⇒ *
            // >=1.2.3 |  <1.2.3 ⇒ *
            if ((left.Operator.IsSthThanOrEqual() || right.Operator.IsSthThanOrEqual()) && left.Operand.Equals(right.Operand))
                return XRangeComparator.All;

            // <0.0.0-0 handling
            if (PrimitiveComparator.None.Equals(left))
                return right;
            if (PrimitiveComparator.None.Equals(right))
                return left;

            // The comparators can't be combined into a primitive, meaning that in order
            // to union these two, a memory allocation (VersionRange) is required.

            // <1.2.3 | >3.4.5
            // <1.2.3 | >1.2.3
            return null;
        }

        [Pure] internal static PrimitiveComparator UnionGreaterThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be GT/GTE
            Debug.Assert(left.Operator.IsGTOrGTE() && right.Operator.IsGTOrGTE());

            return Utility.CompareComparators(left, right) <= 0 ? left : right;
        }
        [Pure] internal static PrimitiveComparator UnionLessThan(PrimitiveComparator left, PrimitiveComparator right)
        {
            // Both arguments must be LT/LTE
            Debug.Assert(left.Operator.IsLTOrLTE() && right.Operator.IsLTOrLTE());

            return Utility.CompareComparators(left, right) >= 0 ? left : right;
        }

    }
}
