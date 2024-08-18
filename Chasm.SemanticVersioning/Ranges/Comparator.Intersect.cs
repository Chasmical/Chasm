using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        /// <summary>
        ///   <para>Returns the intersection of the two specified comparators.</para>
        /// </summary>
        /// <param name="left">The first comparator to intersect.</param>
        /// <param name="right">The second comparator to intersect.</param>
        /// <returns>The intersection of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static ComparatorSet operator &(Comparator left, Comparator right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            return ComparatorSet.FromTuple(Intersect(left, right));
        }

        [Pure] private static (Comparator?, Comparator?) Intersect(Comparator left, Comparator right)
        {
            // if both are primitives, use the simple IntersectPrimitives method
            if (left is PrimitiveComparator primitiveLeft && right is PrimitiveComparator primitiveRight)
            {
                PrimitiveComparator? result = IntersectPrimitives(primitiveLeft, primitiveRight);
                return result is not null ? (result, null) : (left, right);
            }

            // otherwise (if one of them is advanced), use the IntersectAdvanced method
            (PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh) = left.AsPrimitives();
            (PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh) = right.AsPrimitives();

            return IntersectAdvanced(leftLow, leftHigh, rightLow, rightHigh, left, right);
        }
        [Pure] private static (Comparator?, Comparator?) IntersectAdvanced(
            PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh,
            PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh,
            Comparator original1, Comparator original2
        )
        {
            // if one of the operands is an equality primitive, return either that primitive, or <0.0.0-0
            if (leftLow?.Operator.IsEQ() == true)
                return (original2.IsSatisfiedByCore(leftLow.Operand) ? original1 : PrimitiveComparator.None, null);
            if (rightLow?.Operator.IsEQ() == true)
                return (original1.IsSatisfiedByCore(rightLow.Operand) ? original2 : PrimitiveComparator.None, null);

            // determine the resulting intersection's bounds
            int lowK = RangeUtility.CompareComparators(leftLow, rightLow);
            int highK = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            // both operands' bounds are the same, return whichever one's an advanced comparator, or the left one
            if (lowK == 0 && highK == 0)
                return (original1.IsAdvanced || !original2.IsAdvanced ? original1 : original2, null);

            // left is inside right (leftLow >= rightLow && leftHigh <= rightHigh), return original left comparator
            if (lowK >= 0 && highK <= 0) return (original1, null);
            // right is inside left (leftLow <= rightLow && leftHigh >= rightHigh), return original right comparator
            if (lowK <= 0 && highK >= 0) return (original2, null);

            // store the resulting intersection's bounds
            PrimitiveComparator? resultLow = lowK >= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highK <= 0 ? leftHigh : rightHigh;

            // see if two primitives can be turned into one (i.e. =1.2.3 or <0.0.0-0)
            if (resultLow is not null && resultHigh is not null)
            {
                PrimitiveComparator? singleResult = IntersectOpposite(resultLow, resultHigh);
                if (singleResult is not null) return (singleResult, null);
            }

            // at this point, we only have >/>=/</<= primitives, which definitely intersect
            Debug.Assert(RangeUtility.DoComparatorsIntersect(resultHigh, resultLow));

            // see if any of the advanced comparator can be resugared back
            if (original1 is AdvancedComparator advanced1 && Resugar(advanced1, resultLow, resultHigh) is { } resugared1)
                return (resugared1, null);
            if (original2 is AdvancedComparator advanced2 && Resugar(advanced2, resultLow, resultHigh) is { } resugared2)
                return (resugared2, null);

            // arrange the primitives to be used in FromTuple method
            if (resultLow is null)
                return (resultHigh is null ? XRangeComparator.All : resultHigh, null);
            if (resultHigh is null)
                return (resultLow, null);

            return (resultLow, resultHigh);
        }

        [Pure] private static PrimitiveComparator? IntersectPrimitives(PrimitiveComparator left, PrimitiveComparator right)
        {
            // if the primitives compare in the same direction (but not equality), pick whichever one's more restricting
            if (RangeUtility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = RangeUtility.CompareComparators(left, right);
                return (left.Operator.IsGTOrGTE() ? cmp >= 0 : cmp <= 0) ? left : right;
            }

            // if one of the operands is an equality primitive, return either that primitive, or <0.0.0-0
            if (left.Operator.IsEQ())
                return right.IsSatisfiedByCore(left.Operand) ? left : PrimitiveComparator.None;
            if (right.Operator.IsEQ())
                return left.IsSatisfiedByCore(right.Operand) ? right : PrimitiveComparator.None;

            // at this point, the comparison directions are different
            return IntersectOpposite(left, right);
        }
        [Pure] private static PrimitiveComparator? IntersectOpposite(PrimitiveComparator left, PrimitiveComparator right)
        {
            // the primitives must compare in opposite directions here
            Debug.Assert(!left.Operator.IsEQ() && !right.Operator.IsEQ());
            Debug.Assert(left.Operator.IsGTOrGTE() != right.Operator.IsGTOrGTE());

            // if one of their bounds doesn't satisfy the other comparator, then return <0.0.0-0
            if (!left.IsSatisfiedByCore(right.Operand) || !right.IsSatisfiedByCore(left.Operand))
                return PrimitiveComparator.None;

            // try to combine >= and <= primitives into an equality primitive
            if (left.Operator.IsSthThanOrEqual() && right.Operator.IsSthThanOrEqual() && left.Operand.Equals(right.Operand))
                return PrimitiveComparator.Equal(left.Operand);

            // the comparators can't be combined into a single primitive, meaning that a ComparatorSet is required
            return null;
        }

    }
}
