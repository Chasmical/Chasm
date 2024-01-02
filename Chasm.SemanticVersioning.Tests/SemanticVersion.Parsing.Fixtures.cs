using System;
using System.Collections.Generic;
using System.Linq;
using Chasm.Collections;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Pure] public static FixtureAdapter<ParsingFixture> CreateParsingFixtures()
        {
            FixtureAdapter<ParsingFixture> adapter = [];

            ParsingFixture New(string source, SemverOptions parsingOptions = SemverOptions.Strict)
                => adapter.Add(new ParsingFixture(source, parsingOptions));

            // ReSharper disable once JoinDeclarationAndInitializer
            SemverOptions options;

            // | Fixture type                 | Description                                 |
            // |------------------------------|---------------------------------------------|
            // | New(…).Returns(…)            | A valid fixture                             |
            // | New(…).Throws(…)             | An invalid fixture                          |
            // | New(…, o).ButStrictThrows(…) | Valid with options, invalid in strict mode  |



            // Simple numeric versions
            New("1.2.3").Returns(1, 2, 3);
            New("0.5.0").Returns(0, 5, 0);
            New("1.234.56789").Returns(1, 234, 56789);
            New("1234.567.89").Returns(1234, 567, 89);

            // Max component values
            New("2147483647.2.3").Returns(2147483647, 2, 3);
            New("1.2147483647.3").Returns(1, 2147483647, 3);
            New("1.2.2147483647").Returns(1, 2, 2147483647);
            New("2147483648.2.3").Throws(Exceptions.MajorTooBig);
            New("1.2147483648.3").Throws(Exceptions.MinorTooBig);
            New("1.2.2147483648").Throws(Exceptions.PatchTooBig);
            New("2147483647012.2.3").Throws(Exceptions.MajorTooBig);
            New("1.2147483647012.3").Throws(Exceptions.MinorTooBig);
            New("1.2.2147483647012").Throws(Exceptions.PatchTooBig);

            // Pre-releases
            New("1.2.3-alpha").Returns(1, 2, 3, "alpha");
            New("1.2.3-45678").Returns(1, 2, 3, 45678);
            New("1.2.3-alpha27-beta").Returns(1, 2, 3, "alpha27-beta");
            New("1.2.3-beta.5.alpha").Returns(1, 2, 3, "beta", 5, "alpha");
            // Build metadata
            New("1.2.3+test-build").Returns(1, 2, 3, "+test-build");
            New("1.2.3+045.test22").Returns(1, 2, 3, "+045", "test22");
            // Pre-releases and build metadata
            New("1.2.3-alpha.7+build.008").Returns(1, 2, 3, "alpha", 7, "+build", "008");

            // Leading and trailing hyphens
            New("1.2.3--45.beta-").Returns(1, 2, 3, "-45", "beta-");
            New("1.2.3---beta---.45-").Returns(1, 2, 3, "--beta---", "45-");
            New("1.2.3+--test-build-").Returns(1, 2, 3, "+--test-build-");
            New("1.2.3+---045-.-test").Returns(1, 2, 3, "+---045-", "-test");

            // Leading zeroes
            options = SemverOptions.AllowLeadingZeroes;
            New("004.2.0", options).Returns(4, 2, 0).ButStrictThrows(Exceptions.MajorLeadingZeroes);
            New("4.002.0", options).Returns(4, 2, 0).ButStrictThrows(Exceptions.MinorLeadingZeroes);
            New("4.2.000", options).Returns(4, 2, 0).ButStrictThrows(Exceptions.PatchLeadingZeroes);
            New("1.2.3-004", options).Returns(1, 2, 3, 4).ButStrictThrows(Exceptions.PreReleaseLeadingZeroes);
            New("1.2.3-004a").Returns(1, 2, 3, "004a");
            New("1.2.3+0007").Returns(1, 2, 3, "+0007");
            New("0002147483647.00.00", options).Returns(2147483647, 0, 0);
            New("00.0002147483647.00", options).Returns(0, 2147483647, 0);
            New("00.00.0002147483647", options).Returns(0, 0, 2147483647);

            // Equals prefix
            options = SemverOptions.AllowEqualsPrefix;
            New("=1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            New("==1.2.3", options).Throws(Exceptions.MajorNotFound); // only one '=' is allowed
            // Version prefix
            options = SemverOptions.AllowVersionPrefix;
            New("v1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            New("V1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            New("vv1.2.3", options).Throws(Exceptions.MajorNotFound); // only one 'v'/'V' is allowed
            New("vV1.2.3", options).Throws(Exceptions.MajorNotFound);
            New("Vv1.2.3", options).Throws(Exceptions.MajorNotFound);
            New("VV1.2.3", options).Throws(Exceptions.MajorNotFound);
            // Equals and version prefix
            options = SemverOptions.AllowEqualsPrefix | SemverOptions.AllowVersionPrefix;
            New("=v1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            New("=V1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            New("v=1.2.3", options).Throws(Exceptions.MajorNotFound); // only in this order: '=' ('v' | 'V')
            New("V=1.2.3", options).Throws(Exceptions.MajorNotFound);

            // Leading whitespace
            options = SemverOptions.AllowLeadingWhite;
            New(" \r\t\n1.2.3", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.MajorNotFound);
            // Trailing whitespace
            options = SemverOptions.AllowTrailingWhite;
            New("1.2.3-pre+build\r\t\n ", options).Returns(1, 2, 3, "pre", "+build").ButStrictThrows(Exceptions.Leftovers);
            // Inner whitespace
            options = SemverOptions.AllowInnerWhite;
            New("1 . 2 . 3 - alpha . 0 + build", options).Returns(1, 2, 3, "alpha", 0, "+build").ButStrictThrows(Exceptions.MinorNotFound);

            // Inner whitespace + trailing whitespace
            options = SemverOptions.AllowInnerWhite;
            New("1 .2 .3 -beta +007    ", options).Throws(Exceptions.Leftovers);
            New("1 .2 .3 -beta +007 $$$", options).Throws(Exceptions.Leftovers);
            options = SemverOptions.AllowInnerWhite | SemverOptions.AllowTrailingWhite;
            New("1 .2 .3 -beta +007    ", options).Returns(1, 2, 3, "beta", "+007");
            New("1 .2 .3 -beta +007 $$$", options).Throws(Exceptions.Leftovers);

            // Optional patch component
            options = SemverOptions.OptionalPatch;
            New("1.2.", options).Returns(1, 2, 0).ButStrictThrows(Exceptions.PatchNotFound);
            New("1.2", options).Returns(1, 2, 0).ButStrictThrows(Exceptions.PatchNotFound);
            New("1.", options).Throws(Exceptions.MinorNotFound);
            New("1", options).Throws(Exceptions.MinorNotFound);
            // Optional minor and patch components
            options = SemverOptions.OptionalMinor;
            New("1.2.", options).Throws(Exceptions.PatchNotFound);
            New("1.2", options).Throws(Exceptions.PatchNotFound);
            New("1.", options).Returns(1, 0, 0).ButStrictThrows(Exceptions.MinorNotFound);
            New("1", options).Returns(1, 0, 0).ButStrictThrows(Exceptions.MinorNotFound);

            // Allow leftovers
            options = SemverOptions.AllowLeftovers;
            New("$$$", options).Throws(Exceptions.MajorNotFound);
            New("1$$$", options).Throws(Exceptions.MinorNotFound);
            New("1.2.$$$", options).Throws(Exceptions.PatchNotFound);
            New("1.2.3$$$", options).Returns(1, 2, 3).ButStrictThrows(Exceptions.Leftovers);
            New("1.2.3-$$$", options).Throws(Exceptions.PreReleaseEmpty);
            New("1.2.3-alpha+$$$", options).Throws(Exceptions.BuildMetadataEmpty);
            New("1.2.3-gamma+123$$$", options).Returns(1, 2, 3, "gamma", "+123").ButStrictThrows(Exceptions.Leftovers);

            // Optional pre-release separator
            options = SemverOptions.OptionalPreReleaseSeparator;
            New("1.2.3alpha", options).Returns(1, 2, 3, "alpha").ButStrictThrows(Exceptions.Leftovers);
            New("1.2.3alpha5b70", options).Returns(1, 2, 3, "alpha", 5, "b", 70).ButStrictThrows(Exceptions.Leftovers);



            // All options
            options = SemverOptions.Loose;
            New(" = v 1 . 002 . 3- alpha. 03 .rc + DEV . BUILD ", options).Returns(1, 2, 3, "alpha", 3, "rc", "+DEV", "BUILD");

            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<SemanticVersion>
        {
            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;

            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Patch { get; private set; }
            public IEnumerable<object>? PreReleases { get; private set; }
            public IEnumerable<string>? BuildMetadata { get; private set; }

            public LooseParsingFixture Returns(int major, int minor, int patch, params object[] identifiers)
            {
                MarkAsComplete();

                Major = major;
                Minor = minor;
                Patch = patch;

                int buildMetadataIndex = Array.FindIndex(identifiers, static obj => obj is string str && str[0] == '+');
                if (buildMetadataIndex < 0) buildMetadataIndex = identifiers.Length;

                PreReleases = identifiers[..buildMetadataIndex];
                if (buildMetadataIndex < identifiers.Length)
                {
                    string[] buildMetadata = identifiers[buildMetadataIndex..].Cast<string>();
                    buildMetadata[0] = buildMetadata[0][1..]; // remove leading '+'
                    BuildMetadata = buildMetadata;
                }
                else BuildMetadata = Enumerable.Empty<string>();

                return Extend<LooseParsingFixture>();
            }
            public override void AssertResult(SemanticVersion? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Major, result.Major);
                Assert.Equal(Minor, result.Minor);
                Assert.Equal(Patch, result.Patch);
                Assert.Equal(PreReleases!, result.PreReleases.Select(static p => p.IsNumeric ? (object)(int)p : (string)p));
                Assert.Equal(BuildMetadata!, result.BuildMetadata);
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Options})";
        }
        public class LooseParsingFixture : FixtureExtender<ParsingFixture>
        {
            public void ButStrictThrows(string exceptionMessage)
            {
                Assert.NotEqual(SemverOptions.Strict, Prototype.Options);
                ParsingFixture newFixture = new ParsingFixture(Prototype.Source, SemverOptions.Strict);
                newFixture.Throws(typeof(ArgumentException), exceptionMessage);
                Adapter.Add(newFixture);
            }
        }

    }
}
