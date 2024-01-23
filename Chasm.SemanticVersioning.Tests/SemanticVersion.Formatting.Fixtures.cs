using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Pure] public static FixtureAdapter<FormattingFixture> CreateFormattingFixtures()
        {
            FixtureAdapter<FormattingFixture> adapter = [];

            FormattingFixture New(string source, string? format = null)
                => adapter.Add(new FormattingFixture(source, format));



            // Add strictly valid parsing fixtures
            foreach (ParsingFixture fixture in CreateParsingFixtures())
                if (fixture.IsValid && fixture.Options is SemverOptions.Strict)
                    New(fixture.Source).Returns(fixture.Source);



            // Simple component-only formats
            New("1.2.3", "M.m.p").Returns("1.2.3");
            New("1.2.3-alpha+meta", "M.m.p").Returns("1.2.3");
            // Mixed-up component-only formats
            New("12.34.56", "p.m.M+M-p").Returns("56.34.12+12-56");

            // Optional components
            New("1.2.0-rc", "M.mm.pp-rr").Returns("1.2-rc");
            New("1.0.3-rc", "M.mm.pp-rr").Returns("1.0.3-rc");
            New("1.0.0-rc", "M.mm.pp-rr").Returns("1-rc");

            // Quoted escaped text
            New("1.2.3-rc.05b+meta", "'M.m.p-rr+dd:' M.m.p-rr+dd").Returns("M.m.p-rr+dd: 1.2.3-rc.05b+meta");
            New("1.2.3-rc.05b+meta", "\"M.m.p-rr+dd:\" M.m.p-rr+dd").Returns("M.m.p-rr+dd: 1.2.3-rc.05b+meta");

            // Indexed identifiers
            New("0.0.0-0.1.2.3.4.5", "r0 r4 r3 r r r r").Returns("0 4 3 4 5"); // last two 'r' are extra
            New("0.0.0---abc-.-52.0xF2AB", "r0.r1+r2+r3").Returns("--abc-.-52+0xF2AB"); // last 'r3' is extra

            return adapter;
        }

        public sealed class FormattingFixture(string source, string? format = null) : FuncFixture<string>
        {
            public string Source { get; } = source;
            public string? Format { get; } = format;
            private string? Expected;

            public void Returns(string expected)
            {
                MarkAsComplete();
                Expected = expected;
            }
            public override void AssertResult(string? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Expected, result);
            }
            public override string ToString()
                => $"{base.ToString()} \"{Source}\" ({Format})";
        }
    }
}
