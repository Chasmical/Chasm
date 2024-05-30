using System;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<MatchingFixture> CreateMatchingFixtures()
        {
            FixtureAdapter<MatchingFixture> adapter = [];

            MatchingFixture New(string range)
                => adapter.Add(new MatchingFixture(range));

            // Since the set of versions matched with includePreReleases
            // is a superset of those matched without includePreReleases,
            // the meaning of the fixtures is the following:

            // Matches([])          - matches either way
            // MatchesWithIncPr([]) - matches only with includePreReleases
            // DoesNotMatch([])     - doesn't match either way



            // Primitive comparators
            New("=1.2.3")
                .Matches(["1.2.3", "1.2.3+BUILD"])
                .DoesNotMatch(["1.2.2", "1.2.3-0", "1.2.3-alpha.3", "1.2.4-beta.2", "1.2.4"]);

            New(">1.2.3")
                .Matches(["1.2.4", "1.3.5", "9.9.9"])
                .MatchesWithIncPr(["1.2.4-0", "1.2.4-beta.5", "1.5.4-beta.2", "2.3.5-preview.7"])
                .DoesNotMatch(["0.0.0", "0.1.2", "1.2.2", "1.2.3-0", "1.2.3-rc.0", "1.2.3"]);

            New("<1.2.3")
                .Matches(["0.0.0", "0.2.3", "1.2.0", "1.2.2"])
                .MatchesWithIncPr(["0.1.2-rc.2", "0.8.9-alpha.4", "1.2.3-0", "1.2.3-gamma.5"])
                .DoesNotMatch(["1.2.3", "1.2.4-0", "1.2.4-rc.2", "9.9.9"]);

            New(">=1.2.3")
                .Matches(["1.2.3", "1.2.3+BUILD", "1.2.4", "1.3.5", "9.9.9"])
                .MatchesWithIncPr(["1.2.4-0", "1.2.4-beta.5", "1.5.4-beta.2", "2.3.5-preview.7"])
                .DoesNotMatch(["0.0.0", "0.1.2", "1.2.2", "1.2.3-0", "1.2.3-rc.0"]);

            New("<=1.2.3")
                .Matches(["0.0.0", "0.2.3", "1.2.0", "1.2.2", "1.2.3", "1.2.3+BUILD"])
                .MatchesWithIncPr(["0.1.2-rc.2", "0.8.9-alpha.4", "1.2.0-0", "1.2.3-0", "1.2.3-gamma.5"])
                .DoesNotMatch(["1.2.4-0", "1.2.4-rc.2", "9.9.9"]);



            // There's no need to test advanced comparators, since we have
            // desugaring tests to ensure they desugar into primitives correctly.



            // Range with a hole in [1.2.3, 2.0.0)
            New("<1.2.3 || >=2.0.0")
                .Matches(["1.0.4", "1.2.2", "2.0.0", "2.3.4", "9.9.9"])
                .MatchesWithIncPr(["1.2.0-alpha.1", "1.2.3-0", "1.2.3-alpha", "2.0.1-0", "2.0.3-rc.6", "2.7.9-beta.0"])
                .DoesNotMatch(["1.2.3", "1.5.4", "1.5.6-0", "2.0.0-0"]);

            // Range with a hole in 1.2.3
            New("<1.2.3 || >1.2.3")
                .Matches(["0.0.0", "0.3.4", "1.2.2", "1.2.4", "9.8.7"])
                .MatchesWithIncPr(["0.7.2-alpha.8", "1.2.0-0", "1.2.3-0", "1.2.3-beta.3", "1.2.4-0", "1.2.4-rc.3", "5.6.1-pre.2"])
                .DoesNotMatch(["1.2.3", "1.2.3+BUILD"]);

            // Full range with three intersecting sets
            New("<1.2.0 || >1.0.3 <1.5.0 || >=1.4.0")
                .Matches(["1.0.0", "1.1.0", "1.2.0", "1.3.0", "1.4.0", "1.5.0", "1.6.0"])
                .MatchesWithIncPr(["1.0.0-0", "1.4.0-0", "2.0.0-0"]);

            // Ranges with pre-releases
            New(">=1.2.0-alpha.4 <1.3.0-0")
                .Matches(["1.2.0-alpha.4", "1.2.0-alpha.12", "1.2.5", "1.2.99"])
                .MatchesWithIncPr(["1.2.1-rc.4", "1.2.9-0"])
                .DoesNotMatch(["1.1.0", "1.2.0-alpha.3", "1.3.0-0", "1.3.0-alpha.3"]);

            New(">5.6.0-0 <5.6.0-alpha.1")
                .Matches(["5.6.0-0.1", "5.6.0-1", "5.6.0-ALPHA", "5.6.0-alpha", "5.6.0-alpha.0"])
                .DoesNotMatch(["1.2.3", "5.6.0-0", "5.6.0-alpha.1", "5.6.0-beta.4", "5.6.0"]);



            return adapter;
        }

        public class MatchingFixture(string range) : Fixture
        {
            [Obsolete(TestUtil.DeserCtor, true)] public MatchingFixture() : this(null!) { }

            public string Range { get; } = range;
            public string Version { get; private set; } = null!;

            public bool? Result { get; private set; }
            // true - matches either way; false - doesn't match either way; null - only with includePreReleases.

            public MatchingFixtureExtender Matches(params string[] matchVersions)
                => SetResults(matchVersions, true);
            public MatchingFixtureExtender DoesNotMatch(params string[] matchVersions)
                => SetResults(matchVersions, false);
            public MatchingFixtureExtender MatchesWithIncPr(params string[] matchVersions)
                => SetResults(matchVersions, null);

            private MatchingFixtureExtender SetResults(string[] matchVersions, bool? result)
            {
                SetOwnResult(matchVersions[0], result);
                for (int i = 1; i < matchVersions.Length; i++)
                {
                    MatchingFixture newFixture = new MatchingFixture(Range);
                    Adapter!.Add(newFixture);
                    newFixture.SetOwnResult(matchVersions[i], result);
                }
                return Extend<MatchingFixtureExtender>();
            }
            private void SetOwnResult(string matchVersion, bool? result)
            {
                MarkAsComplete();
                Version = matchVersion;
                Result = result;
            }

            public override string ToString()
                => $"{base.ToString()} {Range}{(Result is null ? " (inc.pr)" : "")} {(Result != false ? "~" : "!~")} {Version}";
        }
        public class MatchingFixtureExtender : FixtureExtender<MatchingFixture>
        {
            public MatchingFixtureExtender MatchesWithIncPr(params string[] matchVersions)
                => AddNew(new MatchingFixture(Prototype.Range)).MatchesWithIncPr(matchVersions);
            public MatchingFixtureExtender DoesNotMatch(params string[] matchVersions)
                => AddNew(new MatchingFixture(Prototype.Range)).DoesNotMatch(matchVersions);
        }
    }
}
