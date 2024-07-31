using System.Collections.Generic;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class VersionRange
    {
        public static VersionRange operator ~(VersionRange range)
        {
            ComparatorSet[] sets = range._comparatorSets;

            // TODO: improve performance and memory usage here?

            VersionRange result = ~sets[0];

            for (int i = 1; i < sets.Length; i++)
                result &= ~sets[i];

            return result;
        }

        public static VersionRange operator &(VersionRange left, VersionRange right)
        {
            List<ComparatorSet> sets = [];

            ComparatorSet[] leftSets = left._comparatorSets;
            ComparatorSet[] rightSets = right._comparatorSets;

            for (int i = 0; i < leftSets.Length; i++)
                for (int j = 0; j < rightSets.Length; j++)
                {
                    ComparatorSet intersection = leftSets[i] & rightSets[j];
                    if (!ReferenceEquals(intersection, ComparatorSet.None))
                        sets.Add(intersection);
                }

            if (sets.Count == 0) return None;

            return new VersionRange(sets.ToArray(), default);
        }

        public static VersionRange operator |(VersionRange left, VersionRange right)
        {
            // TODO: put this in a cycle, combining the sets' ranges, until there are no changes
            List<ComparatorSet> leftResults = [];
            List<ComparatorSet> rightResults = [];

            ComparatorSet[] leftSets = left._comparatorSets;
            ComparatorSet[] rightSets = right._comparatorSets;

            for (int i = 0; i < leftSets.Length; i++)
            {
                ComparatorSet accumulator = leftSets[i];

                for (int j = 0; j < rightSets.Length; j++)
                {
                    // TODO: use a tuple-returning Union method instead
                    VersionRange range = accumulator | rightSets[j];
                    // if the sets were combined into one, use the new value
                    if (range.ComparatorSets.Count == 1)
                        accumulator = range[0];
                    else
                        rightResults.Add(rightSets[j]);
                }
                // TODO: check for more intersections?

                leftResults.Add(accumulator);
            }
            // TODO: check for more intersections?

            if (rightResults.Count == 0)
            {
                if (ReferenceEquals(leftResults[0], ComparatorSet.None)) return None;
                if (ReferenceEquals(leftResults[0], ComparatorSet.All)) return All;
            }

            return new VersionRange([..leftResults, ..rightResults], default);
        }

    }
}
