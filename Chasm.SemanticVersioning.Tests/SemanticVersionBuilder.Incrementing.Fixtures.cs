using System;
using Xunit;
// ReSharper disable LambdaExpressionCanBeMadeStatic

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        public static FixtureAdapter<IncrementingFixture> CreateIncrementingFixtures()
        {
            FixtureAdapter<IncrementingFixture> adapter = [];

            IncrementingFixture New(string source)
                => adapter.Add(new IncrementingFixture(source));

            const int max = int.MaxValue;



            // IncrementPatch simple tests
            New("0.0.0-0")
                .After(b => b.IncrementPatch())
                .Returns("0.0.0").Then("0.0.1").Then("0.0.2");
            New("0.0.3-0")
                .After(b => b.IncrementPatch())
                .Returns("0.0.3").Then("0.0.4").Then("0.0.5");
            New("0.2.0-0")
                .After(b => b.IncrementPatch())
                .Returns("0.2.0").Then("0.2.1").Then("0.2.2");
            New("1.2.0-0")
                .After(b => b.IncrementPatch())
                .Returns("1.2.0").Then("1.2.1").Then("1.2.2");
            New("1.2.3-0")
                .After(b => b.IncrementPatch())
                .Returns("1.2.3").Then("1.2.4").Then("1.2.5");

            // IncrementPatch overflow
            New($"1.2.{max}-0")
                .After(b => b.IncrementPatch())
                .Returns($"1.2.{max}")
                .ThenAgain().Throws(Exceptions.PatchTooBig);



            return adapter;
        }

        public sealed class IncrementingFixture(string source) : FuncFixture<SemanticVersionBuilder>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public IncrementingFixture() : this(null!) { }

            public string Source { get; } = source;
            public Action<SemanticVersionBuilder>? Action { get; private set; }
            public string? Expected { get; private set; }

            protected override Type DefaultExceptionType => typeof(InvalidOperationException);

            public override void AssertResult(SemanticVersionBuilder? result)
            {
                // We need an exact match, that is, including build metadata
                Assert.Equal(Expected, result?.ToString());
            }

            public Stage2 After(Action<SemanticVersionBuilder> action)
            {
                Action = action;
                return new Stage2(this);
            }
            private Extender Returns(string expected)
            {
                MarkAsComplete();
                Expected = expected;
                return Extend<Extender>();
            }
            public override string ToString()
                => $"{base.ToString()} {Source} → {Expected}";

            public sealed class Stage2(IncrementingFixture fixture)
            {
                public Extender Returns(string source) => fixture.Returns(source);
                public void Throws(string? exceptionMessage) => fixture.Throws(exceptionMessage);
            }
            public sealed class Extender : FixtureExtender<IncrementingFixture>
            {
                public Stage2 ThenAfter(Action<SemanticVersionBuilder> action)
                    => AddNew(new IncrementingFixture(Prototype.Expected!)).After(action);
                public Stage2 ThenAgain()
                    => ThenAfter(Prototype.Action!);
                public Extender Then(string expected)
                    => ThenAgain().Returns(expected);
            }
        }
    }
}
