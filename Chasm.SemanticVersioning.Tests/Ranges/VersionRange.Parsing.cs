using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Fact]
        public void ParsingNull()
        {
            Assert.Throws<ArgumentNullException>(static () => VersionRange.Parse(null!));
            Assert.Throws<ArgumentNullException>(static () => VersionRange.Parse(null!, SemverOptions.Loose));
            Assert.Throws<ArgumentNullException>(static () => TestUtil.Parse<VersionRange>(null!));

            Assert.False(VersionRange.TryParse(null, out _));
            Assert.False(VersionRange.TryParse(null, SemverOptions.Loose, out _));
            Assert.False(TestUtil.TryParse<VersionRange>(null!, out _));
        }

        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Parsing(ParsingFixture fixture)
        {
            Output.WriteLine($"Parsing {fixture}");

            string source = fixture.Source;
            SemverOptions options = fixture.Options;

            // test overloads with options (+ strict overloads)
            fixture.Test(() => VersionRange.Parse(source, options));
            fixture.Test(VersionRange.TryParse(source, options, out VersionRange? version), version);

            if (options is SemverOptions.Strict)
            {
                // test explicit IParsable and ISpanParsable methods
                fixture.Test(() => TestUtil.Parse<VersionRange>(source));
                fixture.Test(TestUtil.TryParse(source, out version), version);
                fixture.Test(() => TestUtil.SpanParse<VersionRange>(source.AsSpan()));
                fixture.Test(TestUtil.TrySpanParse(source.AsSpan(), out version), version);

                // make sure strictly valid ranges can also be parsed with a loose parsing algorithm
                options = TestUtil.PseudoStrict;
                fixture.Test(() => VersionRange.Parse(source, options));
                fixture.Test(VersionRange.TryParse(source, options, out version), version);
            }

            if (fixture.IsValid)
            {
                // make sure that valid ranges can also be parsed with loose options
                options = SemverOptions.Loose;
                fixture.Test(() => VersionRange.Parse(source, options));
                fixture.Test(VersionRange.TryParse(source, options, out version), version);
            }

        }

    }
}
