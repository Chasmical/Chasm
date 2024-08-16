using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract partial class Comparator
    {
        [Pure] private static AdvancedComparator? Resugar(AdvancedComparator advanced, PrimitiveComparator? left, PrimitiveComparator? right)
        {
            AdvancedComparator? resugared = ResugarCore(advanced, left, right);
            Debug.Assert(resugared is null || resugared.ToPrimitives() == (left, right));
            // TODO: set _primitives on AdvancedComparator
            return resugared;
        }
        [Pure] private static AdvancedComparator? ResugarCore(AdvancedComparator advanced, PrimitiveComparator? left, PrimitiveComparator? right)
        {
            (PrimitiveComparator? origLeft, PrimitiveComparator? origRight) = advanced.ToPrimitives();
            if (ReferenceEquals(left, origLeft) && ReferenceEquals(right, origRight)) return advanced;

            // TODO: resugar comparators

            return null;
        }

    }
}
