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

            VersionRange range = VersionRange.Parse(fixture.Source);
            Assert.True(range.IsSugared);

            fixture.Test(() =>
            {
                VersionRange desugared = range.Desugar();
                Assert.False(desugared.IsSugared);
                Assert.Same(desugared, desugared.Desugar());
                return desugared;
            });
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

    }
}
