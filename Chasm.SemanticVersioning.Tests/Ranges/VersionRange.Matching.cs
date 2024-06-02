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

            // true  - matches either way
            // false - doesn't match either way
            // null  - matches only with includePreReleases
            bool satisfiesDefault = fixture.Result == true;
            bool satisfiesIncPr = fixture.Result != false;

            Assert.Equal(satisfiesDefault, range.IsSatisfiedBy(version));
            Assert.Equal(satisfiesIncPr, range.IsSatisfiedBy(version, true));

            // Test individual comparator set methods
            if (range.ComparatorSets.Count == 1)
            {
                ComparatorSet set = range.ComparatorSets[0];
                Assert.Equal(satisfiesDefault, set.IsSatisfiedBy(version));
                Assert.Equal(satisfiesIncPr, set.IsSatisfiedBy(version, true));

                // Test individual comparator methods
                if (set.Comparators.Count == 1)
                {
                    Comparator comp = set.Comparators[0];
                    Assert.Equal(satisfiesDefault, comp.IsSatisfiedBy(version));
                    Assert.Equal(satisfiesIncPr, comp.IsSatisfiedBy(version, true));
                }
            }

        }
    }
}
