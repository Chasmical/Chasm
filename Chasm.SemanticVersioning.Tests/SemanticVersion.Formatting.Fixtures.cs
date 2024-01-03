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
