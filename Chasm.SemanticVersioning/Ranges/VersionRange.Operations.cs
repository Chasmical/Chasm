using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class VersionRange
    {
        [Pure] public static VersionRange operator ~(VersionRange range)
        {
            if (range is null) throw new ArgumentNullException(nameof(range));

            ComparatorSet[] sets = range._comparatorSets;
            // TODO: improve performance and memory usage here?
            VersionRange result = ~sets[0];
            for (int i = 1; i < sets.Length; i++)
                result &= ~sets[i];
            return result;
        }

        [Pure] public static VersionRange operator &(VersionRange left, VersionRange right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));

            ComparatorSet[] leftSets = left._comparatorSets;
            ComparatorSet[] rightSets = right._comparatorSets;
            List<ComparatorSet> results = [];

            for (int i = 0; i < leftSets.Length; i++)
                for (int j = 0; j < rightSets.Length; j++)
                    AddWithCombine(results, leftSets[i] & rightSets[j]);

            return FromList(results);
        }

        [Pure] public static VersionRange operator |(VersionRange left, VersionRange right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (right is null) throw new ArgumentNullException(nameof(right));

            ComparatorSet[] leftSets = left._comparatorSets;
            ComparatorSet[] rightSets = right._comparatorSets;
            List<ComparatorSet> results = [];

            for (int i = 0; i < leftSets.Length; i++)
                AddWithCombine(results, leftSets[i]);
            for (int i = 0; i < rightSets.Length; i++)
                AddWithCombine(results, rightSets[i]);

            return FromList(results);
        }

        private static void AddWithCombine(List<ComparatorSet> sets, ComparatorSet append)
        {
            if (append.Equals(ComparatorSet.None)) return;

            for (int i = 0; i < sets.Count; i++)
            {
                if (append.Intersects(sets[i]))
                {
                    // TODO: optimize this, combine without memory allocation and duplicate GetBounds() calls
                    VersionRange combined = append | sets[i];
                    Debug.Assert(combined._comparatorSets.Length == 1);
                    sets[i] = combined._comparatorSets[0];

                    // see if the resulting set intersects with any other sets,
                    // and combine the comparator sets until there are no changes
                    while (TryCombineOneIntersection(sets)) { }
                    return;
                }
            }

            sets.Add(append);
        }
        [Pure] private static bool TryCombineOneIntersection(List<ComparatorSet> sets)
        {
            int count = sets.Count;
            for (int i = 0; i < count; i++)
                for (int j = i + 1; j < count; j++)
                    if (sets[i].Intersects(sets[j]))
                    {
                        VersionRange combined = sets[i] | sets[j];
                        Debug.Assert(combined._comparatorSets.Length == 1);
                        sets[i] = combined._comparatorSets[0];
                        sets.RemoveAt(j);
                        return true;
                    }
            return false;
        }
        [Pure] private static VersionRange FromList(List<ComparatorSet> results)
        {
            if (results.Count == 1)
            {
                ComparatorSet only = results[0];
                if (only.Equals(ComparatorSet.None)) return None;
                if (only.Equals(ComparatorSet.All)) return All;
            }
            if (results.Count == 0) return None;
            return new VersionRange(results.ToArray(), default);
        }

    }
}
