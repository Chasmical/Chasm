using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> advanced version comparator.</para>
    /// </summary>
    public abstract class AdvancedComparator : Comparator
    {
        /// <summary>
        ///   <para>Returns <see langword="false"/>, since this version comparator is not primitive.</para>
        /// </summary>
        public sealed override bool IsPrimitive => false;

        /// <summary>
        ///   <para>Gets the advanced version comparator's operand.</para>
        /// </summary>
        public PartialVersion Operand { get; }

        private (PrimitiveComparator? left, PrimitiveComparator? right)? primitives;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="AdvancedComparator"/> class with the specified <paramref name="operand"/>.</para>
        /// </summary>
        /// <param name="operand">The advanced version comparator's operand.</param>
        /// <exception cref="ArgumentNullException"><paramref name="operand"/> is <see langword="null"/>.</exception>
        protected AdvancedComparator(PartialVersion operand)
            => Operand = operand ?? throw new ArgumentNullException(nameof(operand));

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
            => CanMatchPreRelease(Operand, major, minor, patch);
        /// <inheritdoc/>
        [Pure] public override bool IsSatisfiedBy(SemanticVersion? version)
        {
            if (version is null) return false;
            (PrimitiveComparator? left, PrimitiveComparator? right) = ToPrimitives();
            return left?.IsSatisfiedBy(version) != false && right?.IsSatisfiedBy(version) != false;
        }

        /// <summary>
        ///   <para>Converts this advanced version comparator into zero, one or two primitive version comparators, a set of which is equivalent to this advanced version comparator.</para>
        /// </summary>
        /// <returns>A tuple of zero, one or two primitive version comparators, a set of which is equivalent to this advanced version comparator.</returns>
        [Pure] public (PrimitiveComparator? Left, PrimitiveComparator? Right) ToPrimitives()
            => primitives ??= ConvertToPrimitives();
        /// <summary>
        ///   <para>Converts this advanced version comparator into zero, one or two primitive version comparators, a set of which is equivalent to this advanced version comparator.</para>
        /// </summary>
        /// <returns>A tuple of zero, one or two primitive version comparators, a set of which is equivalent to this advanced version comparator.</returns>
        [Pure] protected abstract (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives();

    }
}
