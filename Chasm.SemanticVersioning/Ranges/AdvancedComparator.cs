using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract class AdvancedComparator : Comparator
    {
        public sealed override bool IsPrimitive => false;

        public PartialVersion Operand { get; }

        private (PrimitiveComparator? left, PrimitiveComparator? right)? primitives;

        protected AdvancedComparator(PartialVersion operand)
            => Operand = operand ?? throw new ArgumentNullException(nameof(operand));

        /// <inheritdoc/>
        [Pure] public override bool CanMatchPreRelease(int major, int minor, int patch)
            => Operand.IsPreRelease
            && (!Operand.Major.IsNumeric || Operand.Major.GetValueOrZero() == major)
            && (!Operand.Minor.IsNumeric || Operand.Minor.GetValueOrZero() == minor)
            && (!Operand.Patch.IsNumeric || Operand.Patch.GetValueOrZero() == patch);
        /// <inheritdoc/>
        [Pure] public override bool IsSatisfiedBy(SemanticVersion? version)
        {
            if (version is null) return false;
            (PrimitiveComparator? left, PrimitiveComparator? right) = ToPrimitives();
            return left?.IsSatisfiedBy(version) is not false && right?.IsSatisfiedBy(version) is not false;
        }

        [Pure] public (PrimitiveComparator?, PrimitiveComparator?) ToPrimitives()
            => primitives ??= ConvertToPrimitives();

        [Pure] protected abstract (PrimitiveComparator?, PrimitiveComparator?) ConvertToPrimitives();

    }
}
