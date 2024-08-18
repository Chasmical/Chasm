using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    internal static class RangeUtility
    {
        /// <summary>
        ///   <para>Handles the conversion of a partial version to a semantic one the same way as <c>node-semver</c>: ignores components and pre-releases after an unspecified component, and removes build metadata. Major version component must be numeric at this point.</para>
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        [Pure] public static SemanticVersion NodeSemverTrim(PartialVersion partial)
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

        [Pure] public static int CompareComparators(PrimitiveComparator? left, PrimitiveComparator? right, int sign = 1)
        {
            // 1 should be used when comparing > and >=, while -1 - when comparing < and <=

            if (left is null) return right is null ? 0 : -sign;
            if (right is null) return sign;
            return CompareComparators(left.Operator, left.Operand, right.Operator, right.Operand);
        }
        [Pure] public static bool DoComparatorsIntersect(PrimitiveComparator? high, PrimitiveComparator? low)
        {
            if (high is null || low is null) return true;
            return DoComparatorsIntersect(high.Operator, high.Operand, low.Operator, low.Operand);
        }
        [Pure] public static bool DoComparatorsComplement(PrimitiveComparator? high, PrimitiveComparator? low)
        {
            if (high is null || low is null) return true;
            Debug.Assert(high.Operator.IsLTOrLTE());
            Debug.Assert(low.Operator.IsGTOrGTE());

            int cmp = high.Operand.CompareTo(low.Operand);
            return cmp > 0 || cmp == 0 && (high.Operator.IsSthThanOrEqual() || low.Operator.IsSthThanOrEqual());
        }

        [Pure] public static int CompareComparators(
            PrimitiveOperator leftOperator, SemanticVersion? leftOperand,
            PrimitiveOperator rightOperator, SemanticVersion? rightOperand,
            int nullSign = 1
        )
        {
            // nullSign = 1  - comparing > and >=
            // nullSign = -1 - comparing < and <=
            // nullSign = 0  - suppress direction mismatch assertion

            if (leftOperand is null) return rightOperand is null ? 0 : -nullSign;
            if (rightOperand is null) return nullSign;

            Debug.Assert(!leftOperator.IsEQ() && !rightOperator.IsEQ());
            Debug.Assert(nullSign == 0 || leftOperator.IsGTOrGTE() == rightOperator.IsGTOrGTE());
            Debug.Assert(nullSign == 0 || leftOperator.IsLTOrLTE() == rightOperator.IsLTOrLTE());

            int cmp = leftOperand.CompareTo(rightOperand);
            // >1.2.3 is greater than >=1.2.3, since it doesn't include =1.2.3
            // <=1.2.3 is greater than <1.2.3, since it also includes =1.2.3
            if (cmp == 0 && leftOperator != rightOperator)
                cmp = leftOperator is PrimitiveOperator.GreaterThan or PrimitiveOperator.LessThanOrEqual ? 1 : -1;
            return cmp;
        }

        [Pure] public static bool DoComparatorsIntersect(
            PrimitiveOperator highOperator, SemanticVersion? highOperand,
            PrimitiveOperator lowOperator, SemanticVersion? lowOperand
        )
        {
            if (highOperand is null || lowOperand is null) return true;
            Debug.Assert(highOperator.IsLTOrLTE());
            Debug.Assert(lowOperator.IsGTOrGTE());

            int cmp = highOperand.CompareTo(lowOperand);
            return cmp > 0 || cmp == 0 && highOperator.IsSthThanOrEqual() && lowOperator.IsSthThanOrEqual();
        }

    }
}
