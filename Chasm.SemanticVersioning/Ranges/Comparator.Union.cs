using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
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
            return VersionRange.FromTuple(Union(left, right));
        }

        [Pure] private static (ComparatorSet?, ComparatorSet?) Union(Comparator left, Comparator right)
        {
            (Comparator?, Comparator?) tuple = Union(left, right, out bool isRange);
            return isRange ? tuple : (ComparatorSet.FromTuple(tuple), null);
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
            Comparator sugared1, Comparator sugared2, out bool isRange
        )
        {
            isRange = true; // resulting comparators are unioned as one/two sets (<a.b.c || >x.y.z)

            // if one of the operands is an equality primitive, return either the other comparator, or union both of them
            if (leftLow?.Operator.IsEQ() == true)
                return sugared2.IsSatisfiedByCore(leftLow.Operand) ? (sugared2, null) : (sugared1, sugared2);
            if (rightLow?.Operator.IsEQ() == true)
                return sugared1.IsSatisfiedByCore(rightLow.Operand) ? (sugared1, null) : (sugared1, sugared2);

            // >=1.0.0 <2.0.0-0 | >=3.0.0 <4.0.0-0 ⇒ as is
            //         --------   -------
            if (leftHigh is not null && rightLow is not null && !RangeUtility.DoComparatorsComplement(leftHigh, rightLow))
                return (sugared1, sugared2);

            // >=3.0.0 <4.0.0-0 | >=1.0.0 <2.0.0-0 ⇒ as is
            // -------                    --------
            if (rightHigh is not null && leftLow is not null && !RangeUtility.DoComparatorsComplement(rightHigh, leftLow))
                return (sugared1, sugared2);

            isRange = false; // resulting comparators are intersected as one set (>a.b.c <x.y.z)

            // determine the resulting union's bounds
            int lowK = RangeUtility.CompareComparators(leftLow, rightLow);
            int highK = RangeUtility.CompareComparators(leftHigh, rightHigh, -1);

            // both operands' bounds are the same, return whichever one's an advanced comparator, or the left one
            if (lowK == 0 && highK == 0)
                return (sugared1.IsAdvanced || !sugared2.IsAdvanced ? sugared1 : sugared2, null);

            // left contains right (leftLow <= rightLow && leftHigh >= rightHigh), return original left comparator
            if (lowK <= 0 && highK >= 0) return (sugared1, null);
            // right contains left (leftLow >= rightLow && leftHigh <= rightHigh), return original right comparator
            if (lowK >= 0 && highK <= 0) return (sugared2, null);

            // store the resulting union's bounds
            PrimitiveComparator? resultLow = lowK <= 0 ? leftLow : rightLow;
            PrimitiveComparator? resultHigh = highK >= 0 ? leftHigh : rightHigh;

            // see if any of the advanced comparator can be resugared back
            if (sugared1 is AdvancedComparator advanced1 && Resugar(advanced1, resultLow, resultHigh) is { } resugared1)
                return (resugared1, null);
            if (sugared2 is AdvancedComparator advanced2 && Resugar(advanced2, resultLow, resultHigh) is { } resugared2)
                return (resugared2, null);

            // at this point, simple combining and resugaring failed, so we'll just AND the results
            // TODO: does anything even hit this case?
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
