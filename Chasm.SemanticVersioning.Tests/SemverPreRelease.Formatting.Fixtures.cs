using System;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        [Pure] public static FixtureAdapter<FormattingFixture> CreateFormattingFixtures()
        {
            FixtureAdapter<FormattingFixture> adapter = [];

            foreach (ParsingFixture fixture in CreateParsingFixtures())
                if (fixture.IsValid && fixture.Options is SemverOptions.Strict)
                {
                    adapter.Add(new FormattingFixture(fixture.Source)).Returns(fixture.Source);
                }

            return adapter;
        }

        public class FormattingFixture(string source, string? format = null) : FuncFixture<string>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public FormattingFixture() : this(null!) { }

            public string Source { get; } = source;
            public string? Format { get; } = format;
            private string Expected = null!;

            public void Returns(string expected)
            {
                MarkAsComplete();
                Expected = expected;
            }

            public override void AssertResult(string? result)
                => Assert.Equal(Expected, result);

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Format})";
        }
    }
}
