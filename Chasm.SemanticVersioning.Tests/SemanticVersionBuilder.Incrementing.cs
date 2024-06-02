using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        [Theory, MemberData(nameof(CreateIncrementingFixtures))]
        public void Incrementing(IncrementingFixture fixture)
        {
            SemanticVersion version = SemanticVersion.Parse(fixture.Source);
            SemanticVersionBuilder builder = new SemanticVersionBuilder(version);

            fixture.Test(() =>
            {
                fixture.Action!(builder);
                return builder;
            });
        }

    }
}
