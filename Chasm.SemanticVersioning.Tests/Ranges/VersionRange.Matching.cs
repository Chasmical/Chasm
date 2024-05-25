using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Theory, MemberData(nameof(CreateMatchingFixtures))]
        public void Matching(MatchingFixture fixture)
        {
            Output.WriteLine($"Matching {fixture}");

            VersionRange range = VersionRange.Parse(fixture.Range);
            SemanticVersion version = SemanticVersion.Parse(fixture.Version);

            if (fixture.Result == true)
            {
                // true - matches either way
                Assert.True(range.IsSatisfiedBy(version));
                Assert.True(range.IsSatisfiedBy(version, true));
            }
            else if (fixture.Result == false)
            {
                // false - doesn't match either way
                Assert.False(range.IsSatisfiedBy(version));
                Assert.False(range.IsSatisfiedBy(version, true));
            }
            else
            {
                // null - matches only with includePreReleases
                Assert.False(range.IsSatisfiedBy(version));
                Assert.True(range.IsSatisfiedBy(version, true));
            }

        }
    }
}
