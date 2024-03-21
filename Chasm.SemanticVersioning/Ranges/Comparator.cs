using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    /// <summary>
    ///   <para>Represents a valid <c>node-semver</c> version comparator.</para>
    /// </summary>
    public abstract partial class Comparator : ISpanBuildable
    {
        /// <summary>
        ///   <para>Determines whether this version comparator is a <see cref="PrimitiveComparator"/>.</para>
        /// </summary>
        // Note: Apparently, type-checking is quicker than calling an overridden getter
        public bool IsPrimitive => this is PrimitiveComparator;
        /// <summary>
        ///   <para>Determines whether this version comparator is an <see cref="AdvancedComparator"/>.</para>
        /// </summary>
        public bool IsAdvanced => !IsPrimitive;

        /// <summary>
        ///   <para>Determines whether this version comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if this version comparator can match a pre-release version with the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool CanMatchPreRelease(int major, int minor, int patch);
        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> satisfies this version comparator.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> satisfies this version comparator; otherwise, <see langword="false"/>.</returns>
        [Pure] public abstract bool IsSatisfiedBy(SemanticVersion? version);

        /// <summary>
        ///   <para>Determines whether the specified semantic <paramref name="version"/> is a pre-release version, and has the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components.</para>
        /// </summary>
        /// <param name="version">The semantic version to match.</param>
        /// <param name="major">The major version component of a pre-release version.</param>
        /// <param name="minor">The minor version component of a pre-release version.</param>
        /// <param name="patch">The patch version component of a pre-release version.</param>
        /// <returns><see langword="true"/>, if the specified semantic <paramref name="version"/> is a pre-release version, and has the specified <paramref name="major"/>, <paramref name="minor"/> and <paramref name="patch"/> version components; otherwise, <see langword="false"/>.</returns>
        [Pure] protected static bool CanMatchPreRelease(SemanticVersion? version, int major, int minor, int patch)
            => version?.IsPreRelease == true && version.Major == major && version.Minor == minor && version.Patch == patch;

        /// <inheritdoc cref="ISpanBuildable.CalculateLength"/>
        [Pure] protected internal abstract int CalculateLength();
        /// <inheritdoc cref="ISpanBuildable.BuildString"/>
        protected internal abstract void BuildString(ref SpanBuilder sb);

        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this version comparator.</para>
        /// </summary>
        /// <returns>The string representation of this version comparator.</returns>
        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

        // TODO: Add ArgumentNullException handling

        public static VersionRange operator |(Comparator left, Comparator right)
            => throw new NotImplementedException();

        public static VersionRange operator ~(Comparator comparator)
        {
            if (comparator is PrimitiveComparator primitive)
            {
                if (primitive.Operator is PrimitiveOperator.ImplicitEqual or PrimitiveOperator.Equal)
                    return InvertEqualityPrimitive(primitive);

                // >0.0.0-0 ⇒ =0.0.0-0 (special case)
                if (primitive.Operator == PrimitiveOperator.GreaterThan && primitive.Operand.Equals(SemanticVersion.MinValue))
                    return PrimitiveComparator.Equal(primitive.Operand);

                return InvertComparisonPrimitive(primitive);
            }
            return InvertAdvanced((AdvancedComparator)comparator);

            static VersionRange InvertAdvanced(AdvancedComparator advanced)
            {
                (PrimitiveComparator? left, PrimitiveComparator? right) = advanced.ToPrimitives();

                // If there's only one primitive, invert and return it;
                // that's also the case, when the left comparator is an equality one.
                if (left is null)
                    return right is null ? VersionRange.None : ~right;
                if (right is null)
                    return ~left;

                // Note: no need to check for >0.0.0-0 here, as advanced comparators aren't supposed to desugar to that

                return new VersionRange([
                    InvertComparisonPrimitive(left),
                    InvertComparisonPrimitive(right),
                ], default);
            }
            static VersionRange InvertEqualityPrimitive(PrimitiveComparator primitive)
            {
                // =0.0.0-0 ⇒ >0.0.0-0 (special case)
                if (primitive.Operand == SemanticVersion.MinValue)
                    return new VersionRange([PrimitiveComparator.GreaterThan(primitive.Operand)], default);

                // =1.2.3 ⇒ <1.2.3 || >1.2.3
                return new VersionRange([
                    PrimitiveComparator.LessThan(primitive.Operand),
                    PrimitiveComparator.GreaterThan(primitive.Operand),
                ], default);
            }
            static PrimitiveComparator InvertComparisonPrimitive(PrimitiveComparator primitive)
            {
                // 7 - 2 (GreaterThan)        = 5 (LessThanOrEqual)
                // 7 - 3 (LessThan)           = 4 (GreaterThanOrEqual)
                // 7 - 4 (GreaterThanOrEqual) = 3 (LessThan)
                // 7 - 5 (LessThanOrEqual)    = 2 (GreaterThan)
                return new PrimitiveComparator(primitive.Operand, 7 - primitive.Operator);
            }
        }

    }
}
