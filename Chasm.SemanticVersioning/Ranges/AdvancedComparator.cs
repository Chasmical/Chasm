using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> advanced version comparator.</para>
    /// </summary>
    public abstract class AdvancedComparator : Comparator
    {
        // IEquatable<AdvancedComparator> isn't implemented here, since there's already
        // IEquatable<Comparator> implemented, so, that takes care of the generic constraints.

        /// <summary>
        ///   <para>Returns <see langword="false"/>, since this comparator is not primitive.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's not a primitive comparator.")]
        public new bool IsPrimitive => false;
        /// <summary>
        ///   <para>Returns <see langword="true"/>, since this comparator is advanced.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it is an advanced comparator.")]
        public new bool IsAdvanced => true;

        /// <summary>
        ///   <para>Gets the advanced comparator's operand.</para>
        /// </summary>
        public PartialVersion Operand { get; }

        // For convenience, the left comparator when not null is always a '>', '>=' or '=',
        // while the right one when not null is always a '<' or '<=' comparator.
        // Additionally, both comparators are guaranteed to not have any build metadata.
        // This may change in the future, so don't rely on it outside of this project.
        private (PrimitiveComparator?, PrimitiveComparator?)? primitives;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="AdvancedComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The advanced comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        protected AdvancedComparator(PartialVersion operand)
            => Operand = operand ?? throw new ArgumentNullException(nameof(operand));

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
        {
            (PrimitiveComparator? left, PrimitiveComparator? right) = ToPrimitives();
            return left is not null && CanMatchPreRelease(left.Operand, major, minor, patch)
                || right is not null && CanMatchPreRelease(right.Operand, major, minor, patch);
        }
        /// <inheritdoc/>
        [Pure] protected internal override bool IsSatisfiedByCore(SemanticVersion version)
        {
            (PrimitiveComparator? left, PrimitiveComparator? right) = ToPrimitives();
            return left?.IsSatisfiedByCore(version) != false && right?.IsSatisfiedByCore(version) != false;
        }

        /// <summary>
        ///   <para>Converts this advanced comparator into zero, one or two primitive comparators, a set of which is equivalent to this advanced comparator.</para>
        /// </summary>
        /// <returns>A tuple of zero, one or two primitive comparators, a set of which is equivalent to this advanced comparator.</returns>
        [Pure] public (PrimitiveComparator? Left, PrimitiveComparator? Right) ToPrimitives()
        {
            (PrimitiveComparator?, PrimitiveComparator?)? comparators = primitives;
            if (comparators is null) primitives = comparators = ConvertToPrimitives();

            // Make sure the advanced comparator is properly converted into primitives
            Debug.Assert(comparators.Value.Item1?.Operator is null
                or PrimitiveOperator.ImplicitEqual or PrimitiveOperator.Equal
                or PrimitiveOperator.GreaterThan or PrimitiveOperator.GreaterThanOrEqual);
            Debug.Assert(comparators.Value.Item2?.Operator is null
                or PrimitiveOperator.LessThan or PrimitiveOperator.LessThanOrEqual);

            return comparators.GetValueOrDefault();
        }
        /// <summary>
        ///   <para>Converts this advanced comparator into zero, one or two primitive comparators, a set of which is equivalent to this advanced comparator.</para>
        /// </summary>
        /// <returns>A tuple of zero, one or two primitive comparators, a set of which is equivalent to this advanced comparator.</returns>
        [Pure] protected abstract (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives();

        // TODO: ToPrimitivesArray() could be useful? Maybe think of a better name
        [Pure] internal PrimitiveComparator[] ToPrimitivesArray()
        {
            (PrimitiveComparator? left, PrimitiveComparator? right) = ToPrimitives();
            int count = (left is not null ? 1 : 0) + (right is not null ? 1 : 0);
            if (count == 0) return [];

            PrimitiveComparator[] comparators = new PrimitiveComparator[count];
            if (left is not null) comparators[0] = left;
            if (right is not null) comparators[left is not null ? 1 : 0] = right;
            return comparators;
        }

    }
}
