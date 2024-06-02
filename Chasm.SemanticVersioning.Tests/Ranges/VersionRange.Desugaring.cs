using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Theory, MemberData(nameof(CreateDesugaringFixtures))]
        public void Desugaring(DesugaringFixture fixture)
        {
            Output.WriteLine($"Desugaring {fixture}");

            // test desugaring of a VersionRange
            VersionRange range = VersionRange.Parse(fixture.Source);
            Assert.True(range.IsSugared);

            fixture.Test(() =>
            {
                VersionRange desugared = range.Desugar();
                Assert.False(desugared.IsSugared);
                Assert.Same(desugared, desugared.Desugar());
                return desugared;
            });

            if (range.ComparatorSets.Count == 1)
            {
                // test desugaring of a ComparatorSet
                ComparatorSet set = range.ComparatorSets[0];
                Assert.True(set.IsSugared);

                fixture.Test(() =>
                {
                    ComparatorSet desugared = set.Desugar();
                    Assert.False(desugared.IsSugared);
                    Assert.Same(desugared, desugared.Desugar());
                    return desugared;
                });
            }
        }

        [Fact]
        public void DesugaringSimpleXRanges()
        {
            SemanticVersion version = new SemanticVersion(1, 2, 3, ["pre", 4], ["dev07-"]);
            PartialVersion partial = version;

            foreach (PrimitiveOperator op in Enum.GetValues<PrimitiveOperator>())
            {
                // make sure that a simple "X-Range" properly desugars to a primitive
                XRangeComparator xRange = new XRangeComparator(partial, op);
                PrimitiveComparator comparator = Assert.Single(xRange.ToPrimitivesArray());
                Assert.Equal(op, comparator.Operator);
                Assert.Equal(version, comparator.Operand);
            }
        }

        [Fact]
        public void DesugaringPrimitives()
        {
            // range with all primitive comparators
            VersionRange range = VersionRange.Parse(">=12.34.56 <20.0.0-0 || >3.5.0 <3.7.0 || >=1.2.0-alpha.4");

            // test primitive range desugaring
            Assert.False(range.IsSugared);
            Assert.Same(range, range.Desugar());

            foreach (ComparatorSet set in range.ComparatorSets)
            {
                // test primitive comparator set desugaring
                Assert.False(set.IsSugared);
                Assert.Same(set, set.Desugar());
            }

            // range with primitive and advanced comparators
            range = VersionRange.Parse("^0.5.x || >=0.3 <2.3.0 || >=2.3.1 <4.2.0");

            Assert.True(range.IsSugared);
            VersionRange desugared = range.Desugar();
            Assert.False(desugared.IsSugared);
            Assert.Equal(">=0.5.0 <0.6.0-0 || >=0.3.0 <2.3.0 || >=2.3.1 <4.2.0", desugared.ToString());

        }

    }
}
