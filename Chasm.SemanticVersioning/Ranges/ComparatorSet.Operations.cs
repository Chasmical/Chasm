namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class ComparatorSet
    {
        public static VersionRange operator ~(ComparatorSet comparatorSet)
            => VersionRange.FromTuple(Complement(comparatorSet));

        internal static (Comparator, Comparator?) Complement(ComparatorSet comparatorSet)
        {
            var (leftOp, left, rightOp, right) = comparatorSet.GetBoundsCore();

            if (left is not null && leftOp.IsEQ())
                return (PrimitiveComparator.LessThan(left), PrimitiveComparator.GreaterThan(left));

            if (!RangeUtility.DoComparatorsIntersect(rightOp, right, leftOp, left))
                return (XRangeComparator.All, null);

            if (left is null)
                return (right is null ? PrimitiveComparator.None : Comparator.ComplementPrimitive(rightOp, right), null);

            return (
                Comparator.ComplementPrimitive(leftOp, left),
                right is null ? null : Comparator.ComplementPrimitive(rightOp, right)
            );
        }

        public static ComparatorSet operator &(ComparatorSet left, ComparatorSet right)
        {
            // each set represents a contiguous range of versions
            (PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh) = left.GetBounds();
            (PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh) = right.GetBounds();

            // -1 - second, 1 - first, 0 - either
            int leftC = RangeUtility.CompareComparators(leftLow, rightLow);
            int rightC = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            if (leftC == 0 && rightC == 0)
                return left.IsSugared || !right.IsSugared ? left : right;

            if (leftC >= 0 && rightC <= 0) return left;
            if (leftC <= 0 && rightC >= 0) return right;

            PrimitiveComparator? lowR = leftC >= 0 ? leftLow : rightLow;
            PrimitiveComparator? highR = rightC <= 0 ? leftHigh : rightHigh;

            // if the intersection is invalid or empty, return <0.0.0-0
            if (lowR is not null && highR is not null)
            {
                if (!lowR.IsSatisfiedByCore(highR.Operand) || !highR.IsSatisfiedByCore(lowR.Operand))
                    return None;
            }

            // combine into an equality primitive
            if (lowR?.Operator is PrimitiveOperator.GreaterThanOrEqual
             && highR?.Operator is PrimitiveOperator.LessThanOrEqual
             && lowR.Operand.Equals(highR.Operand))
                return PrimitiveComparator.Equal(lowR.Operand);

            // return the intersection
            if (lowR is null)
                return highR is null ? All : highR;
            if (highR is null)
                return lowR;

            return new ComparatorSet([lowR, highR], default);
        }

        public static VersionRange operator |(ComparatorSet left, ComparatorSet right)
        {
            // each set represents a contiguous range of versions
            (PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh) = left.GetBounds();
            (PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh) = right.GetBounds();

            if (PrimitiveComparator.None.Equals(leftHigh))
                return right;
            if (PrimitiveComparator.None.Equals(rightHigh))
                return left;

            // if the ranges do not intersect, combine them in a version range
            if (!RangeUtility.DoComparatorsIntersect(rightHigh, leftLow) || !RangeUtility.DoComparatorsIntersect(leftHigh, rightLow))
            {
                return new VersionRange([left, right], default);
            }

            // -1 - first, 1 - second, 0 - either
            int lowC = RangeUtility.CompareComparators(leftLow, rightLow);
            int highC = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            if (lowC == 0 && highC == 0)
                return left.IsSugared || !right.IsSugared ? left : right;

            if (lowC <= 0 && highC >= 0) return left;
            if (lowC >= 0 && highC <= 0) return right;

            PrimitiveComparator? resultLow = lowC <= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highC >= 0 ? leftHigh : rightHigh;

            // return the union
            if (resultLow is null)
                return resultHigh is null ? VersionRange.All : resultHigh;
            if (resultHigh is null)
                return resultLow;

            return new ComparatorSet([resultLow, resultHigh], default);
        }

    }
}
