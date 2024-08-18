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
                return (high is null ? PrimitiveComparator.None : ComplementComparisonPrimitive(high.Operator, high.Operand), null);

            // if it's an equality primitive, return <a.b.c || >a.b.c
            if (low.Operator.IsEQ())
                return (PrimitiveComparator.LessThan(low.Operand), PrimitiveComparator.GreaterThan(low.Operand));

            // complement one or both bounds, return <a.b.c || >x.y.z
            return (
                ComplementComparisonPrimitive(low.Operator, low.Operand),
                high is null ? null : ComplementComparisonPrimitive(high.Operator, high.Operand)
            );
        }
        [Pure] internal static Comparator ComplementComparisonPrimitive(PrimitiveOperator @operator, SemanticVersion operand)
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

    }
}
