using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Theory, MemberData(nameof(CreateMatchingFixtures))]
        public void Matching(MatchingFixture fixture)
        {
            VersionRange range = VersionRange.Parse(fixture.Range);
            SemanticVersion version = SemanticVersion.Parse(fixture.Version);

            fixture.Test(() => range.IsSatisfiedBy(version, fixture.IncludePreReleases));

            if (!fixture.IncludePreReleases)
            {
                fixture.Test(() => range.IsSatisfiedBy(version));

                // Setting includePreReleases to true only expands the set of matching versions,
                // so if a version matches with `false`, it should match with `true` as well.
                if (fixture.Result)
                    fixture.Test(() => range.IsSatisfiedBy(version, true));
            }
        }
    }
}
