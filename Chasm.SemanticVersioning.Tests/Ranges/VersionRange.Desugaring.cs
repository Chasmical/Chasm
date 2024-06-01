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
    }
}
