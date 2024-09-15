using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class VersionRange
    {
        /// <summary>
        ///   <para>Returns the complement of the specified version <paramref name="range"/>.</para>
        /// </summary>
        /// <param name="range">The version range to get the complement of.</param>
        /// <returns>The complement of the specified version <paramref name="range"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="range"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator ~(VersionRange range)
        {
            ANE.ThrowIfNull(range);

            ComparatorSet[] sets = range._comparatorSets;
            // TODO: improve performance and memory usage here?
            VersionRange result = ~sets[0];
            for (int i = 1; i < sets.Length; i++)
                result &= ~sets[i];
            return result;
        }

        /// <summary>
        ///   <para>Returns the intersection of the two specified version ranges.</para>
        /// </summary>
        /// <param name="left">The first version range to intersect.</param>
        /// <param name="right">The second version range to intersect.</param>
        /// <returns>The intersection of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator &(VersionRange left, VersionRange right)
        {
            ANE.ThrowIfNull(left);
            ANE.ThrowIfNull(right);

            ComparatorSet[] leftSets = left._comparatorSets;
            ComparatorSet[] rightSets = right._comparatorSets;
            List<ComparatorSet> results = [];

            for (int i = 0; i < leftSets.Length; i++)
                for (int j = 0; j < rightSets.Length; j++)
                    AddWithCombine(results, leftSets[i] & rightSets[j]);

            return FromList(results);
        }

        /// <summary>
        ///   <para>Returns the union of the two specified version ranges.</para>
        /// </summary>
        /// <param name="left">The first version range to union.</param>
        /// <param name="right">The second version range to union.</param>
        /// <returns>The union of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.</exception>
        [Pure] public static VersionRange operator |(VersionRange left, VersionRange right)
        {
            ANE.ThrowIfNull(left);
            ANE.ThrowIfNull(right);

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
                if (append.Touches(sets[i]))
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
                    if (sets[i].Touches(sets[j]))
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
            if (results.Count == 0) return None;
            if (results.Count == 1) return results[0];
            return new VersionRange(results.ToArray(), default);
        }

    }
}
