using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public static class RangeUtility
    {
        /// <summary>
        ///   <para>Handles the conversion of a partial version to a semantic one the same way as <c>node-semver</c>: ignores components and pre-releases after an unspecified component, and removes build metadata. Major version component must be numeric at this point.</para>
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        public static SemanticVersion NodeSemverTrim(PartialVersion partial)
        {
            int major = (int)partial.Major._value;

            // Major is guaranteed to be numeric, since non-numerics are handled in AdvancedComparators
            Debug.Assert(major >= 0);

            int minor = (int)partial.Minor._value;
            int patch = minor >= 0 ? (int)partial.Patch._value : -1;

            SemverPreRelease[]? preReleases = null;
            ReadOnlyCollection<SemverPreRelease>? preReleasesReadonly = null;
            if (patch >= 0)
            {
                preReleases = partial._preReleases;
                preReleasesReadonly = partial._preReleasesReadonly;
            }
            else
            {
                patch = 0;
                if (minor < 0)
                    minor = 0;
            }

            return new SemanticVersion(major, minor, patch, preReleases, null, preReleasesReadonly, null);
        }

        [Pure] public static PrimitiveOperator Normalize(this PrimitiveOperator op)
            => (PrimitiveOperator)Math.Max((byte)op, (byte)1);

        [Pure] public static bool SameDirection(PrimitiveOperator a, PrimitiveOperator b)
            => a > PrimitiveOperator.Equal && b > PrimitiveOperator.Equal && (((byte)a + (byte)b) & 1) == 0;
        [Pure] public static bool IsGTOrGTE(this PrimitiveOperator op)
            => op is PrimitiveOperator.GreaterThan or PrimitiveOperator.GreaterThanOrEqual;
        [Pure] public static bool IsLTOrLTE(this PrimitiveOperator op)
            => op is PrimitiveOperator.LessThan or PrimitiveOperator.LessThanOrEqual;
        [Pure] public static bool IsEQ(this PrimitiveOperator op)
            => op <= PrimitiveOperator.Equal;
        [Pure] public static bool IsSthThanOrEqual(this PrimitiveOperator op)
            => op is PrimitiveOperator.GreaterThanOrEqual or PrimitiveOperator.LessThanOrEqual;
        [Pure] public static PrimitiveOperator Invert(this PrimitiveOperator op)
        {
            // 7 - 2 (>) = 5 (<=)
            // 7 - 3 (<) = 4 (>=)
            // 7 - 4 (>=) = 3 (<)
            // 7 - 5 (<=) = 2 (>)
            return 7 - op;
        }

        [Pure] public static int CompareComparators(PrimitiveComparator? left, PrimitiveComparator? right, int sign = 1)
        {
            // 1 should be used when comparing > and >=, while -1 - when comparing < and <=

            if (left is null) return right is null ? 0 : -sign;
            if (right is null) return sign;
            return CompareComparators(left.Operator, left.Operand, right.Operator, right.Operand);
        }
        [Pure] public static bool DoComparatorsIntersect(PrimitiveComparator? right, PrimitiveComparator? left)
        {
            if (right is null || left is null) return true;
            return DoComparatorsIntersect(right.Operator, right.Operand, left.Operator, left.Operand);
        }
        [Pure] public static bool DoComparatorsTouch(PrimitiveComparator? right, PrimitiveComparator? left)
        {
            if (right is null || left is null) return true;
            Debug.Assert(right.Operator.IsLTOrLTE());
            Debug.Assert(left.Operator.IsGTOrGTE());

            int cmp = right.Operand.CompareTo(left.Operand);
            return cmp > 0 || cmp == 0 && (right.Operator.IsSthThanOrEqual() || left.Operator.IsSthThanOrEqual());
        }

        [Pure] public static int CompareComparators(
            PrimitiveOperator leftOperator, SemanticVersion? leftOperand,
            PrimitiveOperator rightOperator, SemanticVersion? rightOperand,
            int nullSign = 1
        )
        {
            if (leftOperand is null) return rightOperand is null ? 0 : -nullSign;
            if (rightOperand is null) return nullSign;

            Debug.Assert(!leftOperator.IsEQ() && !rightOperator.IsEQ());
            Debug.Assert(leftOperator.IsGTOrGTE() == rightOperator.IsGTOrGTE());
            Debug.Assert(leftOperator.IsLTOrLTE() == rightOperator.IsLTOrLTE());

            int cmp = leftOperand.CompareTo(rightOperand);
            // >1.2.3 is greater than >=1.2.3, since it doesn't include =1.2.3
            // <=1.2.3 is greater than <1.2.3, since it also includes =1.2.3
            if (cmp == 0 && leftOperator != rightOperator)
                cmp = leftOperator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThanOrEqual ? 1 : -1;
            return cmp;
        }

        [Pure] public static bool DoComparatorsIntersect(
            PrimitiveOperator rightOperator, SemanticVersion? rightOperand,
            PrimitiveOperator leftOperator, SemanticVersion? leftOperand
        )
        {
            if (rightOperand is null || leftOperand is null) return true;
            Debug.Assert(rightOperator.IsLTOrLTE());
            Debug.Assert(leftOperator.IsGTOrGTE());

            int cmp = rightOperand.CompareTo(leftOperand);
            return cmp > 0 || cmp == 0 && rightOperator.IsSthThanOrEqual() && leftOperator.IsSthThanOrEqual();
        }

        /// <summary>
        ///   <para>Determines whether the union of the specified comparators is equivalent to <c>*</c>.</para>
        /// </summary>
        /// <param name="lessThan"></param>
        /// <param name="greaterThan"></param>
        /// <returns></returns>
        [Pure] public static bool DoComparatorsComplement(PrimitiveComparator lessThan, PrimitiveComparator greaterThan)
        {
            Debug.Assert(lessThan.Operator.IsLTOrLTE());
            Debug.Assert(greaterThan.Operator.IsGTOrGTE());

            int cmp = lessThan.Operand.CompareTo(greaterThan.Operand);
            //  <1.3.0 | >=1.3.0 ⇒ *
            // <=1.3.0 |  >1.3.0 ⇒ *
            // <=1.3.0 | >=1.3.0 ⇒ *
            if (cmp == 0) return lessThan.Operator.IsSthThanOrEqual() || greaterThan.Operator.IsSthThanOrEqual();

            // <=1.3.0 | >=1.4.0 ⇒ <=1.3.0 || >=1.4.0
            // <=1.3.0 | >=1.2.0 ⇒ *
            return cmp > 0;
        }

    }
}
