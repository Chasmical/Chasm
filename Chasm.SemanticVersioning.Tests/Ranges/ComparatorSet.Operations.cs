using System;
using System.Linq;
using Chasm.Collections;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class ComparatorSetTests
    {
        private static TheoryData<string>? comparatorSetFixturesCache;
        public static TheoryData<string> GetComparatorSetFixtures()
        {
            if (comparatorSetFixturesCache is not null) return comparatorSetFixturesCache;
            TheoryData<string> data = comparatorSetFixturesCache = [];

            // add all unique comparator sets used in CreateOperationFixtures()
            data.AddRange(
                VersionRangeTests
                    .CreateOperationFixtures().Fixtures
                    .SelectMany(f => new[] { f.Left, f.Right, f.Expected })
                    .NotNull().SelectMany(r => r.Split("||", StringSplitOptions.TrimEntries))
                    .Distinct().ToArray()
            );

            return data;
        }

        [Theory, MemberData(nameof(GetComparatorSetFixtures))]
        public void Contains(string aStr)
        {
            ComparatorSet a = VersionRange.Parse(aStr)._comparatorSets[0];
            ComparatorSet aNormalized = a.Normalize().Desugar();

            foreach (string bStr in GetComparatorSetFixtures().GetFixtures())
            {
                ComparatorSet b = VersionRange.Parse(bStr)._comparatorSets[0];
                ComparatorSet bNormalized = b.Normalize().Desugar();

                try
                {
                    ComparatorSet intersection = (a & b).Normalize().Desugar();
                    // if A contains B, then (A & B) = B
                    Assert.Equal(intersection == bNormalized, a.Contains(b));
                    // if B contains A, then (A & B) = A
                    Assert.Equal(intersection == aNormalized, b.Contains(a));
                }
                catch
                {
                    Output.WriteLine($"A    : {aStr}");
                    Output.WriteLine($"B    : {bStr}");
                    Output.WriteLine($"A & B: {a & b}");
                    throw;
                }
            }
        }
        [Theory, MemberData(nameof(GetComparatorSetFixtures))]
        public void Intersects(string aStr)
        {
            ComparatorSet a = VersionRange.Parse(aStr)._comparatorSets[0];
            ComparatorSet aNormalized = a.Normalize().Desugar();

            foreach (string bStr in GetComparatorSetFixtures().GetFixtures())
            {
                ComparatorSet b = VersionRange.Parse(bStr)._comparatorSets[0];
                ComparatorSet bNormalized = b.Normalize().Desugar();

                try
                {
                    ComparatorSet intersection = (a & b).Normalize().Desugar();
                    if (aNormalized == ComparatorSet.None || bNormalized == ComparatorSet.None)
                    {
                        // only <0.0.0-0 can intersect <0.0.0-0
                        Assert.Equal(aNormalized == bNormalized, a.Intersects(b));
                        Assert.Equal(aNormalized == bNormalized, b.Intersects(a));
                    }
                    else
                    {
                        // A and B intersect only if their intersection (A & B) isn't empty
                        Assert.Equal(intersection != ComparatorSet.None, a.Intersects(b));
                        Assert.Equal(intersection != ComparatorSet.None, b.Intersects(a));
                    }
                }
                catch
                {
                    Output.WriteLine($"A    : {aStr}");
                    Output.WriteLine($"B    : {bStr}");
                    Output.WriteLine($"A & B: {(a & b).Normalize().Desugar()}");
                    throw;
                }
            }
        }
        [Theory, MemberData(nameof(GetComparatorSetFixtures))]
        public void Touches(string aStr)
        {
            ComparatorSet a = VersionRange.Parse(aStr)._comparatorSets[0];
            ComparatorSet aNormalized = a.Normalize().Desugar();

            foreach (string bStr in GetComparatorSetFixtures().GetFixtures())
            {
                ComparatorSet b = VersionRange.Parse(bStr)._comparatorSets[0];
                ComparatorSet bNormalized = b.Normalize().Desugar();

                try
                {
                    VersionRange union = (a | b).Desugar();
                    if (aNormalized == ComparatorSet.None || bNormalized == ComparatorSet.None)
                    {
                        // only <0.0.0-0 can touch <0.0.0-0
                        ComparatorSet other = aNormalized == ComparatorSet.None ? bNormalized : aNormalized;
                        Assert.Equal(other == ComparatorSet.None, a.Touches(b));
                        Assert.Equal(other == ComparatorSet.None, b.Touches(a));
                    }
                    else
                    {
                        // A and B touch only if (A | B) contains only one comparator set
                        Assert.Equal(union.ComparatorSets.Count == 1, a.Touches(b));
                        Assert.Equal(union.ComparatorSets.Count == 1, b.Touches(a));
                    }
                }
                catch
                {
                    Output.WriteLine($"A    : {aStr}");
                    Output.WriteLine($"B    : {bStr}");
                    Output.WriteLine($"A & B: {(a & b).Normalize().Desugar()}");
                    throw;
                }
            }
        }

    }
}
