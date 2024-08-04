namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class ComparatorSet
    {
        public static VersionRange operator ~(ComparatorSet comparatorSet)
            => VersionRange.FromTuple(Complement(comparatorSet));

        internal static (Comparator, Comparator?) Complement(ComparatorSet comparatorSet)
        {
            (PrimitiveComparator? left, PrimitiveComparator? right) = comparatorSet.GetBounds();

            if (left?.Operator.IsEQ() == true)
                return (PrimitiveComparator.LessThan(left.Operand), PrimitiveComparator.GreaterThan(left.Operand));

            if (left is null)
                return (right is null ? PrimitiveComparator.None : Comparator.ComplementPrimitive(right), null);

            return (Comparator.ComplementPrimitive(left), right is null ? null : Comparator.ComplementPrimitive(right));
        }

        public static ComparatorSet operator &(ComparatorSet left, ComparatorSet right)
        {
            // each set represents a contiguous range of versions
            (PrimitiveComparator? low1, PrimitiveComparator? high1) = left.GetBounds();
            (PrimitiveComparator? low2, PrimitiveComparator? high2) = right.GetBounds();

            // TODO: refactor and fix precedence
            // determine the intersection's limits
            PrimitiveComparator? lowR = low1 is null ? low2 : low2 is null ? low1 : Comparator.IntersectGreaterThan(low1, low2);
            bool lowRIs1 = ReferenceEquals(lowR, low1);
            PrimitiveComparator? highR = high1 is null ? high2 : high2 is null ? high1 : Comparator.IntersectLessThan(lowRIs1 ? high1 : high2, lowRIs1 ? high2 : high1);

            // if the intersection is invalid or empty, return <0.0.0-0
            if (lowR is not null && highR is not null)
            {
                if (!lowR.IsSatisfiedByCore(highR.Operand) || !highR.IsSatisfiedByCore(lowR.Operand))
                    return None;
            }

            // see if it's still one of the sets (one range entirely contained by the other)
            if (ReferenceEquals(lowR, low1) && ReferenceEquals(highR, high1))
                return left;
            if (ReferenceEquals(lowR, low2) && ReferenceEquals(highR, high2))
                return right;

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
            if (leftHigh is not null && rightLow is not null && Utility.CompareComparators(leftHigh, rightLow) < 0 ||
                leftLow is not null && rightHigh is not null && Utility.CompareComparators(leftLow, rightHigh) > 0)
            {
                return new VersionRange([left, right], default);
            }

            // TODO: refactor and fix precedence
            // the ranges intersect, determine the union's limits
            PrimitiveComparator? resultLow = leftLow is null || rightLow is null ? null : Comparator.UnionGreaterThan(leftLow, rightLow);
            PrimitiveComparator? resultHigh = leftHigh is null || rightHigh is null ? null : Comparator.UnionLessThan(leftHigh, rightHigh);

            // see if it's still one of the sets (one range entirely contains the other)
            if (ReferenceEquals(resultLow, leftLow) && ReferenceEquals(resultHigh, leftHigh))
                return left;
            if (ReferenceEquals(resultLow, rightLow) && ReferenceEquals(resultHigh, rightHigh))
                return right;

            // return the union
            if (resultLow is null)
                return resultHigh is null ? VersionRange.All : resultHigh;
            if (resultHigh is null)
                return resultLow;

            return new ComparatorSet([resultLow, resultHigh], default);
        }

    }
}
