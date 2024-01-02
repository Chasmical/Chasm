using System;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Fact]
        public void ParsingNull()
        {
            Assert.Throws<ArgumentNullException>(static () => SemanticVersion.Parse(null!));
            Assert.Throws<ArgumentNullException>(static () => SemanticVersion.Parse(null!, SemverOptions.Loose));
            Assert.Throws<ArgumentNullException>(static () => TestUtil.Parse<SemanticVersion>(null!));

            Assert.False(SemanticVersion.TryParse(null, out _));
            Assert.False(SemanticVersion.TryParse(null, SemverOptions.Loose, out _));
            Assert.False(TestUtil.TryParse<SemanticVersion>(null!, out _));
        }

        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Parsing(ParsingFixture fixture)
        {
            Output.WriteLine($"Parsing {fixture}");

            string source = fixture.Source;
            SemverOptions options = fixture.Options;

            // test overloads with options (+ strict overloads)
            fixture.Test(() => SemanticVersion.Parse(source, options));
            fixture.Test(SemanticVersion.TryParse(source, options, out SemanticVersion? version), version);

            if (options is SemverOptions.Strict)
            {
                // test explicit IParsable and ISpanParsable methods
                fixture.Test(() => TestUtil.Parse<SemanticVersion>(source));
                fixture.Test(TestUtil.TryParse(source, out version), version);
                fixture.Test(() => TestUtil.SpanParse<SemanticVersion>(source.AsSpan()));
                fixture.Test(TestUtil.TrySpanParse(source.AsSpan(), out version), version);

                // make sure strictly valid versions can also be parsed with a loose parsing algorithm
                options = TestUtil.PseudoStrict;
                fixture.Test(() => SemanticVersion.Parse(source, options));
                fixture.Test(SemanticVersion.TryParse(source, options, out version), version);
            }

            if (fixture.IsValid)
            {
                // make sure that valid versions can also be parsed with loose options
                options = SemverOptions.Loose;
                fixture.Test(() => SemanticVersion.Parse(source, options));
                fixture.Test(SemanticVersion.TryParse(source, options, out version), version);
            }

        }

    }
}
