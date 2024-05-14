using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialVersionTests
    {
        [Fact]
        public void ParsingNull()
        {
            Assert.Throws<ArgumentNullException>(static () => PartialVersion.Parse(null!));
            Assert.Throws<ArgumentNullException>(static () => PartialVersion.Parse(null!, SemverOptions.Loose));
            Assert.Throws<ArgumentNullException>(static () => TestUtil.Parse<PartialVersion>(null!));

            Assert.False(PartialVersion.TryParse(null, out _));
            Assert.False(PartialVersion.TryParse(null, SemverOptions.Loose, out _));
            Assert.False(TestUtil.TryParse<PartialVersion>(null!, out _));
        }

        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Parsing(ParsingFixture fixture)
        {
            Output.WriteLine($"Parsing {fixture}");

            string source = fixture.Source;
            SemverOptions options = fixture.Options;

            // test overloads with options (+ strict overloads)
            fixture.Test(() => PartialVersion.Parse(source, options));
            fixture.Test(PartialVersion.TryParse(source, options, out PartialVersion? version), version);

            if (options is SemverOptions.Strict)
            {
                // test explicit IParsable and ISpanParsable methods
                fixture.Test(() => TestUtil.Parse<PartialVersion>(source));
                fixture.Test(TestUtil.TryParse(source, out version), version);
                fixture.Test(() => TestUtil.SpanParse<PartialVersion>(source.AsSpan()));
                fixture.Test(TestUtil.TrySpanParse(source.AsSpan(), out version), version);

                // make sure strictly valid versions can also be parsed with a loose parsing algorithm
                options = TestUtil.PseudoStrict;
                fixture.Test(() => PartialVersion.Parse(source, options));
                fixture.Test(PartialVersion.TryParse(source, options, out version), version);
            }

            if (fixture.IsValid)
            {
                // make sure that valid versions can also be parsed with loose options
                options = SemverOptions.Loose;
                fixture.Test(() => PartialVersion.Parse(source, options));
                fixture.Test(PartialVersion.TryParse(source, options, out version), version);
            }

        }

    }
}
