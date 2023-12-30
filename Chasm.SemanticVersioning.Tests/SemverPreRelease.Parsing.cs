using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Parsing(ParsingFixture fixture)
        {
            Output.WriteLine($"Parsing {fixture}");

            string source = fixture.Source;
            SemverOptions options = fixture.Options;

            fixture.Test(() => SemverPreRelease.Parse(source, options));
            fixture.Test(SemverPreRelease.TryParse(source, options, out SemverPreRelease preRelease), preRelease);

            if (options is SemverOptions.Strict)
            {
                // test explicit IParsable and ISpanParsable interface methods
                fixture.Test(() => TestUtil.Parse<SemverPreRelease>(source));
                fixture.Test(TestUtil.TryParse(source, out preRelease), preRelease);
                fixture.Test(() => TestUtil.SpanParse<SemverPreRelease>(source));
                fixture.Test(TestUtil.TrySpanParse(source, out preRelease), preRelease);

                // make sure that strictly valid pre-releases can also be parsed with a loose parsing algorithm
                fixture.Test(() => SemverPreRelease.Parse(source, TestUtil.PseudoStrict));
                fixture.Test(SemverPreRelease.TryParse(source, TestUtil.PseudoStrict, out preRelease), preRelease);
            }

            if (fixture.IsValid)
            {
                // make sure that valid pre-releases can also be parsed with loose options
                fixture.Test(() => SemverPreRelease.Parse(source, SemverOptions.Loose));
                fixture.Test(SemverPreRelease.TryParse(source, SemverOptions.Loose, out preRelease), preRelease);
            }
        }
    }
}
