using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class ComparatorSet
    {
        /// <summary>
        ///   <para>Returns the complement of the specified <paramref name="comparatorSet"/>.</para>
        /// </summary>
        /// <param name="comparatorSet">The comparator set to get the complement of.</param>
        /// <returns>The complement of the specified <paramref name="comparatorSet"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="comparatorSet"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator ~(ComparatorSet comparatorSet)
        {
            if (comparatorSet is null) throw new ArgumentNullException(nameof(comparatorSet));
            return VersionRange.FromTuple(Complement(comparatorSet));
        }

        [Pure] private static (Comparator, Comparator?) Complement(ComparatorSet comparatorSet)
        {
            var (lowOp, low, highOp, high) = comparatorSet.GetBoundsCore();

            if (low is null)
                return (high is null ? PrimitiveComparator.None : Comparator.ComplementComparison(highOp, high), null);

            return (
                Comparator.ComplementComparison(lowOp, low),
                high is null ? null : Comparator.ComplementComparison(highOp, high)
            );
        }

        /// <summary>
        ///   <para>Returns the intersection of the two specified comparator sets.</para>
        /// </summary>
        /// <param name="left">The first comparator set to intersect.</param>
        /// <param name="right">The second comparator set to intersect.</param>
        /// <returns>The intersection of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static ComparatorSet operator &(ComparatorSet left, ComparatorSet right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));

            // TODO: rewrite the entire method, to account for resugaring of advanced comparators

            // each set represents a contiguous range of versions
            // TODO: refactor to use GetBoundsCore() to minimize allocations with = primitives?
            (PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh) = left.GetBounds();
            (PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh) = right.GetBounds();

            // determine the resulting intersection's bounds
            int lowK = RangeUtility.CompareComparators(leftLow, rightLow);
            int highK = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            // both operands' bounds are the same, return whichever one contains advanced comparators, or the left one
            if (lowK == 0 && highK == 0)
                return left.IsSugared || !right.IsSugared ? left : right;

            // left is inside right (leftLow >= rightLow && leftHigh <= rightHigh), return original left set
            if (lowK >= 0 && highK <= 0) return left;
            // right is inside left (leftLow <= rightLow && leftHigh >= rightHigh), return original right set
            if (lowK <= 0 && highK >= 0) return right;

            // store the resulting intersection's bounds
            PrimitiveComparator? resultLow = lowK >= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highK <= 0 ? leftHigh : rightHigh;

            // see if two primitives can be turned into one (i.e. =1.2.3 or <0.0.0-0)
            if (resultLow is not null && resultHigh is not null)
            {
                PrimitiveComparator? singleResult = Comparator.IntersectOpposite(resultLow, resultHigh);
                if (singleResult is not null) return singleResult;
            }

            // return the intersection as a comparator set
            if (resultLow is null)
                return resultHigh is null ? All : resultHigh;
            if (resultHigh is null)
                return resultLow;

            return new ComparatorSet([resultLow, resultHigh], default);
        }

        /// <summary>
        ///   <para>Returns the union of the two specified comparator sets.</para>
        /// </summary>
        /// <param name="left">The first comparator set to union.</param>
        /// <param name="right">The second comparator set to union.</param>
        /// <returns>The union of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator |(ComparatorSet left, ComparatorSet right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));

            // TODO: rewrite the entire method, to account for resugaring of advanced comparators

            // each set represents a contiguous range of versions
            // TODO: refactor to use GetBoundsCore() to minimize allocations with = primitives?
            (PrimitiveComparator? leftLow, PrimitiveComparator? leftHigh) = left.GetBounds();
            (PrimitiveComparator? rightLow, PrimitiveComparator? rightHigh) = right.GetBounds();

            if (PrimitiveComparator.None.Equals(leftHigh)) return right;
            if (PrimitiveComparator.None.Equals(rightHigh)) return left;

            // if the sets do not intersect, combine them in a version range
            if (!RangeUtility.DoComparatorsComplement(rightHigh, leftLow) || !RangeUtility.DoComparatorsComplement(leftHigh, rightLow))
                return new VersionRange([left, right], default);

            // determine the resulting union's bounds
            int lowK = RangeUtility.CompareComparators(leftLow, rightLow);
            int highK = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            // both operands' bounds are the same, return whichever one's an advanced comparator, or the left one
            if (lowK == 0 && highK == 0)
                return left.IsSugared || !right.IsSugared ? left : right;

            // left contains right (leftLow <= rightLow && leftHigh >= rightHigh), return original left comparator
            if (lowK <= 0 && highK >= 0) return left;
            // right contains left (leftLow >= rightLow && leftHigh <= rightHigh), return original right comparator
            if (lowK >= 0 && highK <= 0) return right;

            // store the resulting union's bounds
            PrimitiveComparator? resultLow = lowK <= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highK >= 0 ? leftHigh : rightHigh;

            if (resultLow is null && resultHigh is null) return All;

            // return the union as a version range
            if (resultLow is null)
                return resultHigh is null ? VersionRange.All : resultHigh;
            if (resultHigh is null)
                return resultLow;

            return new ComparatorSet([resultLow, resultHigh], default);
        }

    }
}
