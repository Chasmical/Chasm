using JetBrains.Annotations;

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

        [Pure] internal static (Comparator?, Comparator?) UnionAdvanced(PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh,
                                                                        PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh,
                                                                        Comparator sugared1, Comparator sugared2, out bool isRange)
        {
            isRange = true;
            // =1.2.3 | ^1.0.0 ⇒ ^1.0.0
            // =1.2.3 | ^2.0.0 ⇒ =1.2.3 || ^2.0.0
            if (leftLow?.Operator.IsEQ() == true)
                return sugared2.IsSatisfiedBy(leftLow.Operand) ? (sugared2, null) : (sugared1, sugared2);
            // ^1.0.0 | =1.2.3 ⇒ ^1.0.0
            // ^2.0.0 | =1.2.3 ⇒ ^2.0.0 || =1.2.3
            if (rightLow?.Operator.IsEQ() == true)
                return sugared1.IsSatisfiedBy(rightLow.Operand) ? (sugared1, null) : (sugared1, sugared2);

            // >=1.0.0 <2.0.0-0 | >=3.0.0 <4.0.0-0 ⇒ as is
            //         --------   -------
            if (leftHigh is not null && rightLow is not null && !RangeUtility.DoComparatorsComplement(leftHigh, rightLow))
                return (sugared1, sugared2);

            // >=3.0.0 <4.0.0-0 | >=1.0.0 <2.0.0-0 ⇒ as is
            // -------                    --------
            if (rightHigh is not null && leftLow is not null && !RangeUtility.DoComparatorsComplement(rightHigh, leftLow))
                return (sugared1, sugared2);

            isRange = false;

            // >1.2.3 | * ⇒ *
            // <1.2.3 | * ⇒ *
            // >1.2.3 | >2.3.4 ⇒ >1.2.3
            // <1.2.3 | <2.3.4 ⇒ <2.3.4

            // -1 - first, 1 - second, 0 - either
            int leftC = RangeUtility.CompareComparators(leftLow, rightLow);
            int rightC = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            if (leftC == 0 && rightC == 0)
                return (sugared1.IsAdvanced || !sugared2.IsAdvanced ? sugared1 : sugared2, null);

            if (leftC <= 0 && rightC >= 0) return (sugared1, null);
            if (leftC >= 0 && rightC <= 0) return (sugared2, null);

            PrimitiveComparator? leftResult = leftC <= 0 ? leftLow : rightLow;
            PrimitiveComparator? rightResult = rightC >= 0 ? leftHigh : rightHigh;

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
            if (RangeUtility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = RangeUtility.CompareComparators(left, right);
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

    }
}
