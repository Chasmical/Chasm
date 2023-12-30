using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        public static FixtureAdapter<ParsingFixture> ParsingFixtures { get; } = CreateParsingFixtures();

        [Pure]
        private static FixtureAdapter<ParsingFixture> CreateParsingFixtures()
        {
            FixtureAdapter<ParsingFixture> adapter = [];

            ParsingFixture New(string source, SemverOptions parsingOptions = SemverOptions.Strict)
                => adapter.Add(new ParsingFixture(source, parsingOptions));



            // Empty pre-release
            New("").Throws(Exceptions.PreReleaseEmpty);

            // Numeric pre-releases
            New("0").Returns(0);
            New("3").Returns(3);

            // Int32 overflow
            New("2147483647").Returns(2147483647);
            New("2147483648").Throws(Exceptions.PreReleaseTooBig);
            New("2147483650").Throws(Exceptions.PreReleaseTooBig);
            New("2147483647012").Throws(Exceptions.PreReleaseTooBig);

            // Leading zeroes
            New("001").Throws(Exceptions.PreReleaseLeadingZeroes);
            New("001", SemverOptions.AllowLeadingZeroes).Returns(1);
            New("0002147483647").Throws(Exceptions.PreReleaseLeadingZeroes);
            New("0002147483647", SemverOptions.AllowLeadingZeroes).Returns(2147483647);

            // Alphanumeric pre-releases
            New("alpha").Returns("alpha");
            New("PRE-009").Returns("PRE-009");
            New("0beta-07---").Returns("0beta-07---");
            New("-7").Returns("-7");

            // Invalid characters
            New("alpha-beta$$$").Throws(Exceptions.PreReleaseInvalid);
            New("$beta---debug-").Throws(Exceptions.PreReleaseInvalid);

            // Leading and trailing whitespace
            New(" \n 34").Throws(Exceptions.PreReleaseInvalid);
            New("  \rDebug8-Build-").Throws(Exceptions.PreReleaseInvalid);
            New(" \n 34", SemverOptions.AllowLeadingWhite).Returns(34);
            New("  \rDebug8-Build-", SemverOptions.AllowLeadingWhite).Returns("Debug8-Build-");

            New("34 \n ").Throws(Exceptions.PreReleaseInvalid);
            New("Debug8-Build-  \r").Throws(Exceptions.PreReleaseInvalid);
            New("34 \n ", SemverOptions.AllowTrailingWhite).Returns(34);
            New("Debug8-Build-  \r", SemverOptions.AllowTrailingWhite).Returns("Debug8-Build-");



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<SemverPreRelease>
        {
            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;
            private object expected = null!;

            public void Returns(int identifier) => ReturnsComplete(expected = identifier);
            public void Returns(string identifier) => ReturnsComplete(expected = identifier);
            private void ReturnsComplete(object _) => MarkAsComplete();

            public override void AssertResult(SemverPreRelease result)
            {
                Assert.Equal(result.IsNumeric, expected is int);
                Assert.Equal(result.IsNumeric ? (int)result : (string)result, expected);
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Options})";
        }
    }
}
