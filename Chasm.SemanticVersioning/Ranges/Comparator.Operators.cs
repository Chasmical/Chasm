using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        /// <summary>
        ///   <para>Returns the complement of the specified <paramref name="comparator"/>.</para>
        /// </summary>
        /// <param name="comparator">The comparator to get the complement of.</param>
        /// <returns>The complement of the specified <paramref name="comparator"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="comparator"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator ~(Comparator comparator)
        {
            if (comparator is null) throw new ArgumentNullException(nameof(comparator));
            return VersionRange.FromTuple(Complement(comparator));
        }

        [Pure] private static (Comparator, Comparator?) Complement(Comparator comparator)
        {
            (PrimitiveComparator? low, PrimitiveComparator? high) = comparator.AsPrimitives();

            // if both bounds are null, return <0.0.0-0, otherwise if the lower one is null, complement the upper bound
            if (low is null)
                return (high is null ? PrimitiveComparator.None : ComplementComparison(high.Operator, high.Operand), null);

            // if it's an equality primitive, return <a.b.c || >a.b.c
            if (low.Operator.IsEQ())
                return (PrimitiveComparator.LessThan(low.Operand), PrimitiveComparator.GreaterThan(low.Operand));

            // complement one or both bounds, return <a.b.c || >x.y.z
            return (
                ComplementComparison(low.Operator, low.Operand),
                high is null ? null : ComplementComparison(high.Operator, high.Operand)
            );
        }
        [Pure] internal static Comparator ComplementComparison(PrimitiveOperator @operator, SemanticVersion operand)
        {
            // accept only comparison-operator primitives
            Debug.Assert(!@operator.IsEQ());

            // special case for <0.0.0-0 - return *
            if (@operator == PrimitiveOperator.LessThan && operand.Equals(SemanticVersion.MinValue))
                return XRangeComparator.All;

            // assert that the values are as expected
            Debug.Assert(7 - PrimitiveOperator.GreaterThan == PrimitiveOperator.LessThanOrEqual);
            Debug.Assert(7 - PrimitiveOperator.LessThan == PrimitiveOperator.GreaterThanOrEqual);
            Debug.Assert(7 - PrimitiveOperator.GreaterThanOrEqual == PrimitiveOperator.LessThan);
            Debug.Assert(7 - PrimitiveOperator.LessThanOrEqual == PrimitiveOperator.GreaterThan);

            // return a primitive with the same operand and inverted operator
            return new PrimitiveComparator(operand, 7 - @operator);
        }

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

            // TODO: try to resugar advanced comparators

            // at this point, I think it's impossible for any of these to be null,
            // but I'll leave the null checks with returns just in case.
            Debug.Assert(resultLow is not null && resultHigh is not null);

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
        [Pure] internal static PrimitiveComparator? IntersectOpposite(PrimitiveComparator left, PrimitiveComparator right)
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

        /// <summary>
        ///   <para>Returns the union of the two specified comparators.</para>
        /// </summary>
        /// <param name="left">The first comparator to union.</param>
        /// <param name="right">The second comparator to union.</param>
        /// <returns>The union of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator |(Comparator left, Comparator right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));
            (Comparator?, Comparator?) tuple = Union(left, right, out bool isRange);
            return VersionRange.FromTuple(isRange ? tuple! : (ComparatorSet.FromTuple(tuple), null));
        }

        [Pure] private static (Comparator?, Comparator?) Union(Comparator left, Comparator right, out bool isRange)
        {
            // if both are primitives, use the simple UnionPrimitives method
            if (left is PrimitiveComparator primitiveLeft && right is PrimitiveComparator primitiveRight)
            {
                // it's either a single primitive or two non-intersecting primitives, like <a.b.c || >x.y.z
                isRange = true;
                Comparator? result = UnionPrimitives(primitiveLeft, primitiveRight);
                return result is not null ? (result, null) : (left, right);
            }

            // otherwise (if one of them is advanced), use the UnionAdvanced method
            (PrimitiveComparator? left1, PrimitiveComparator? right1) = left.AsPrimitives();
            (PrimitiveComparator? left2, PrimitiveComparator? right2) = right.AsPrimitives();

            return UnionAdvanced(left1, right1, left2, right2, left, right, out isRange);
        }
        [Pure] private static (Comparator?, Comparator?) UnionAdvanced(
            PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh,
            PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh,
            Comparator original1, Comparator original2, out bool isRange
        )
        {
            isRange = true; // resulting comparators are unioned as one/two sets (<a.b.c || >x.y.z)

            // if one of the operands is an equality primitive, return either the other comparator, or union both of them
            if (leftLow?.Operator.IsEQ() == true)
                return original2.IsSatisfiedByCore(leftLow.Operand) ? (original2, null) : (original1, original2);
            if (rightLow?.Operator.IsEQ() == true)
                return original1.IsSatisfiedByCore(rightLow.Operand) ? (original1, null) : (original1, original2);

            // if the comparators don't intersect, combine them in a version range
            if (!RangeUtility.DoComparatorsComplement(leftHigh, rightLow) || !RangeUtility.DoComparatorsComplement(rightHigh, leftLow))
                return (original1, original2);

            isRange = false; // resulting comparators are intersected as one set (>a.b.c <x.y.z)

            // determine the resulting union's bounds
            int lowK = RangeUtility.CompareComparators(leftLow, rightLow);
            int highK = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            // both operands' bounds are the same, return whichever one's an advanced comparator, or the left one
            if (lowK == 0 && highK == 0)
                return (original1.IsAdvanced || !original2.IsAdvanced ? original1 : original2, null);

            // left contains right (leftLow <= rightLow && leftHigh >= rightHigh), return original left comparator
            if (lowK <= 0 && highK >= 0) return (original1, null);
            // right contains left (leftLow >= rightLow && leftHigh <= rightHigh), return original right comparator
            if (lowK >= 0 && highK <= 0) return (original2, null);

            // store the resulting union's bounds
            PrimitiveComparator? resultLow = lowK <= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highK >= 0 ? leftHigh : rightHigh;

            if (resultLow is null && resultHigh is null)
                return (XRangeComparator.All, null);

            // TODO: try to resugar advanced comparators

            // at this point, simple combining and resugaring failed, so we'll just AND the results

            // arrange the primitives to be used in FromTuple method
            if (resultLow is null)
                return (resultHigh is null ? XRangeComparator.All : resultHigh, null);
            if (resultHigh is null)
                return (resultLow, null);

            return (resultLow, resultHigh);
        }

        [Pure] private static Comparator? UnionPrimitives(PrimitiveComparator left, PrimitiveComparator right)
        {
            // if primitives compare in the same direction (but not equality), pick whichever one's less restricting
            if (RangeUtility.SameDirection(left.Operator, right.Operator))
            {
                int cmp = RangeUtility.CompareComparators(left, right);
                return (left.Operator.IsGTOrGTE() ? cmp <= 0 : cmp >= 0) ? left : right;
            }

            // if one of the operands is an equality primitive that can be combined into the other, return the other one
            if (left.Operator.IsEQ())
                return right.IsSatisfiedByCore(left.Operand) ? right : null;
            if (right.Operator.IsEQ())
                return left.IsSatisfiedByCore(right.Operand) ? left : null;

            // at this point, the comparison directions are different
            bool leftIsLow = left.Operator.IsGTOrGTE();
            PrimitiveComparator low = leftIsLow ? left : right;
            PrimitiveComparator high = leftIsLow ? right : left;

            // if the primitives complement each other, return *
            if (RangeUtility.DoComparatorsComplement(high, low))
                return XRangeComparator.All;

            // if the upper bound is <0.0.0-0, return just the lower bound
            if (high.Equals(PrimitiveComparator.None)) return low;

            // the comparators can't be combined into a single primitive, meaning that a VersionRange is required
            return null;
        }

    }
}
