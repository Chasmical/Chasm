using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<MatchingFixture> CreateMatchingFixtures()
        {
            FixtureAdapter<MatchingFixture> adapter = [];

            MatchingFixture New(string range)
                => adapter.Add(new MatchingFixture(range));



            // Primitive comparators
            New("=1.2.3")
                .Matches(["1.2.3", "1.2.3+BUILD"])
                .DoesNotMatch(["1.2.2", "1.2.3-0", "1.2.3-alpha.3", "1.2.4-beta.2", "1.2.4"]);

            New(">1.2.3")
                .Matches(["1.2.4", "1.3.5", "9.9.9"])
                .DoesNotMatch(["0.0.0", "0.1.2", "1.2.2", "1.2.3-0", "1.2.3-rc.0", "1.2.3", "1.2.4-0", "1.2.4-alpha.8", "1.5.4-beta.2"])
                .Matches(["1.2.4-beta.5", "2.3.5-preview.7"], true);

            New("<1.2.3")
                .Matches(["0.0.0", "0.2.3", "1.2.0", "1.2.2"])
                .DoesNotMatch(["0.1.2-rc.2", "1.2.3-0", "1.2.3-gamma.5", "1.2.3", "1.2.4-0", "1.2.4-rc.2", "9.9.9"])
                .Matches(["0.8.9-alpha.4"], true);

            New(">=1.2.3")
                .Matches(["1.2.3", "1.2.3+BUILD", "1.2.4", "1.3.5", "9.9.9"])
                .DoesNotMatch(["0.0.0", "0.1.2", "1.2.2", "1.2.3-0", "1.2.3-rc.0", "1.2.4-0", "1.2.4-alpha.8", "1.5.4-beta.2"])
                .Matches(["1.2.4-beta.5", "2.3.5-preview.7"], true);

            New("<=1.2.3")
                .Matches(["0.0.0", "0.2.3", "1.2.0", "1.2.2", "1.2.3", "1.2.3+BUILD"])
                .DoesNotMatch(["0.1.2-rc.2", "1.2.3-0", "1.2.3-gamma.5", "1.2.4-0", "1.2.4-rc.2", "9.9.9"])
                .Matches(["0.8.9-alpha.4", "1.2.0-0"], true);

            // There's no need to test advanced comparators, since we have
            // desugaring tests to ensure they desugar into primitives correctly.



            // Range with a hole in [1.2.3, 2.0.0)
            New("<1.2.3 || >=2.0.0")
                .Matches(["1.0.4", "1.2.2", "2.0.0", "2.3.4", "9.9.9"])
                .DoesNotMatch(["1.2.3-alpha", "1.2.3", "1.5.4", "2.0.3-rc.6"])
                .Matches(["1.2.0-alpha.1", "1.2.3-0", "2.0.1-0", "2.7.9-beta.0"], true)
                .DoesNotMatch(["1.2.3", "1.5.6-0", "2.0.0-0"], true);

            // Range with a hole in 1.2.3
            New("<1.2.3 || >1.2.3")
                .Matches(["0.0.0", "0.3.4", "1.2.2", "1.2.4", "9.8.7"])
                .DoesNotMatch(["0.7.2-alpha.8", "1.2.3-beta.3", "1.2.3", "1.2.4-0", "5.6.1-pre.2"])
                .Matches(["1.2.0-0", "1.2.3-0", "1.2.4-rc.3"], true)
                .DoesNotMatch(["1.2.3", "1.2.3+BUILD"], true);

            // Full range with three intersecting sets
            New("<1.2.0 || >1.0.3 <1.5.0 || >=1.4.0")
                .Matches(["1.0.0", "1.1.0", "1.2.0", "1.3.0", "1.4.0", "1.5.0", "1.6.0"])
                .DoesNotMatch(["1.0.0-0", "1.4.0-0", "2.0.0-0"])
                .Matches(["1.0.0-0", "1.4.0-0", "2.0.0-0"], true);

            // Ranges with pre-releases
            New(">=1.2.0-alpha.4 <1.3.0-0")
                .Matches(["1.2.0-alpha.4", "1.2.0-alpha.12", "1.2.5", "1.2.99"])
                .Matches(["1.2.1-rc.4", "1.2.9-0"], true)
                .DoesNotMatch(["1.1.0", "1.2.0-alpha.3", "1.3.0-0", "1.3.0-alpha.3"])
                .DoesNotMatch(["1.1.0", "1.2.0-alpha.3", "1.3.0-0", "1.3.0-alpha.3"], true);

            New(">5.6.0-0 <5.6.0-alpha.1")
                .Matches(["5.6.0-0.1", "5.6.0-1", "5.6.0-ALPHA", "5.6.0-alpha", "5.6.0-alpha.0"])
                .DoesNotMatch(["1.2.3", "5.6.0-0", "5.6.0-alpha.1", "5.6.0-beta.4", "5.6.0"])
                .DoesNotMatch(["1.2.3", "5.6.0-0", "5.6.0-alpha.1", "5.6.0-beta.4", "5.6.0"], true);



            return adapter;
        }

        public class MatchingFixture(string range) : FuncFixture<bool>
        {
            public string Range { get; } = range;
            public string Version { get; private set; } = null!;
            public bool IncludePreReleases { get; private set; }

            public bool Result { get; private set; }

            public MatchingFixtureExtender Matches(string[] matchVersions, bool includePreReleases = false)
                => SetResults(matchVersions, includePreReleases, true);
            public MatchingFixtureExtender DoesNotMatch(string[] matchVersions, bool includePreReleases = false)
                => SetResults(matchVersions, includePreReleases, false);

            private MatchingFixtureExtender SetResults(string[] matchVersions, bool includePreReleases, bool result)
            {
                SetOwnResults(matchVersions[0], includePreReleases, result);
                for (int i = 1; i < matchVersions.Length; i++)
                {
                    MatchingFixture newFixture = new MatchingFixture(Range);
                    Adapter!.Add(newFixture);
                    newFixture.SetOwnResults(matchVersions[i], includePreReleases, result);
                }
                return Extend<MatchingFixtureExtender>();
            }
            private void SetOwnResults(string matchVersion, bool includePreReleases, bool result)
            {
                MarkAsComplete();
                Version = matchVersion;
                IncludePreReleases = includePreReleases;
                Result = result;
            }

            public override void AssertResult(bool result)
            {
                if (Result) Assert.True(result);
                else Assert.False(result);
            }

            public override string ToString() => $"{base.ToString()} {Range}{(IncludePreReleases ? " (inc.pr)" : "")} {(Result ? "~" : "!~")} {Version}";
        }
        public class MatchingFixtureExtender : FixtureExtender<MatchingFixture>
        {
            public MatchingFixtureExtender Matches(string[] matchVersions, bool includePreReleases = false)
            {
                MatchingFixture newFixture = new MatchingFixture(Prototype.Range);
                Adapter.Add(newFixture);
                return newFixture.Matches(matchVersions, includePreReleases);
            }
            public MatchingFixtureExtender DoesNotMatch(string[] matchVersions, bool includePreReleases = false)
            {
                MatchingFixture newFixture = new MatchingFixture(Prototype.Range);
                Adapter.Add(newFixture);
                return newFixture.DoesNotMatch(matchVersions, includePreReleases);
            }
        }
    }
}
