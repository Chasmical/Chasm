using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialComponentTests
    {
        [Fact]
        public void ParsingNull()
        {
            Assert.Throws<ArgumentNullException>(static () => PartialComponent.Parse(null!));
            Assert.Throws<ArgumentNullException>(static () => PartialComponent.Parse(null!, SemverOptions.Loose));
            Assert.Throws<ArgumentNullException>(static () => TestUtil.Parse<PartialComponent>(null!));

            Assert.False(PartialComponent.TryParse(null, out _));
            Assert.False(PartialComponent.TryParse(null, SemverOptions.Loose, out _));
            Assert.False(TestUtil.TryParse<PartialComponent>(null!, out _));
        }

        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Parsing(ParsingFixture fixture)
        {
            Output.WriteLine($"Parsing {fixture}");

            string source = fixture.Source;
            SemverOptions options = fixture.Options;

            // test overloads with options (+ strict overloads)
            fixture.Test(() => PartialComponent.Parse(source, options));
            fixture.Test(PartialComponent.TryParse(source, options, out PartialComponent component), component);

            if (source.Length == 1)
            {
                fixture.Test(() => PartialComponent.Parse(source[0]));
                fixture.Test(PartialComponent.TryParse(source[0], out component), component);
            }

            if (options is SemverOptions.Strict)
            {
                // test explicit IParsable and ISpanParsable methods
                fixture.Test(() => TestUtil.Parse<PartialComponent>(source));
                fixture.Test(TestUtil.TryParse(source, out component), component);
                fixture.Test(() => TestUtil.SpanParse<PartialComponent>(source.AsSpan()));
                fixture.Test(TestUtil.TrySpanParse(source.AsSpan(), out component), component);

                // make sure strictly valid components can also be parsed with a loose parsing algorithm
                options = TestUtil.PseudoStrict;
                fixture.Test(() => PartialComponent.Parse(source, options));
                fixture.Test(PartialComponent.TryParse(source, options, out component), component);
            }

            if (fixture.IsValid)
            {
                // make sure that valid components can also be parsed with loose options
                options = SemverOptions.Loose;
                fixture.Test(() => PartialComponent.Parse(source, options));
                fixture.Test(PartialComponent.TryParse(source, options, out component), component);
            }

        }

    }
}
