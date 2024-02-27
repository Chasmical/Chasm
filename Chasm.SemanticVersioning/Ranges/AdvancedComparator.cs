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
        /// <summary>
        ///   <para>Returns <see langword="false"/>, since this version comparator is not primitive.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it's not a primitive version comparator.")]
        public new bool IsPrimitive => false;
        /// <summary>
        ///   <para>Returns <see langword="true"/>, since this version comparator is advanced.</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Obsolete("You already know that it is an advanced version comparator.")]
        public new bool IsAdvanced => true;

        /// <summary>
        ///   <para>Gets the advanced version comparator's operand.</para>
        /// </summary>
        public PartialVersion Operand { get; }

        // For convenience, the left comparator when not null is always a '>', '>=' or '=',
        // while the right one when not null is always a '<' or '<=' comparator.
        // This may change in the future, so don't rely on it outside of this project.
        private (PrimitiveComparator?, PrimitiveComparator?)? primitives;

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
