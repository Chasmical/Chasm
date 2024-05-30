using System;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialComponentTests
    {
        [Pure] public static FixtureAdapter<ParsingFixture> CreateParsingFixtures()
        {
            FixtureAdapter<ParsingFixture> adapter = [];

            ParsingFixture New(string source, SemverOptions parsingOptions = SemverOptions.Strict)
                => adapter.Add(new ParsingFixture(source, parsingOptions));

            // ReSharper disable once JoinDeclarationAndInitializer
            SemverOptions options;

            // | Fixture type                 | Description                                 |
            // |------------------------------|---------------------------------------------|
            // | New(…).Returns(…)            | A valid fixture                             |
            // | New(…).Throws(…)             | An invalid fixture                          |
            // | New(…, o).ButStrictThrows(…) | Valid with options, invalid in strict mode  |



            // Numeric components
            New("0").Returns(0);
            New("123").Returns(123);
            New("-4").Throws(Exceptions.ComponentInvalid);
            New("2147483647").Returns(2147483647);
            New("2147483648").Throws(Exceptions.ComponentTooBig);

            // Leading zeroes
            options = SemverOptions.AllowLeadingZeroes;
            New("0042", options).Returns(42).ButStrictThrows(Exceptions.ComponentLeadingZeroes);

            // Wildcard and omitted components
            New("x").Returns('x');
            New("X").Returns('X');
            New("*").Returns('*');
            New("").Returns(null);

            // Extra wildcards
            options = SemverOptions.AllowExtraWildcards;
            New("xxx", options).Returns('x').ButStrictThrows(Exceptions.ComponentInvalid);
            New("****", options).Returns('*').ButStrictThrows(Exceptions.ComponentInvalid);
            New("XX", options).Returns('X').ButStrictThrows(Exceptions.ComponentInvalid);
            New("x**", options).Throws(Exceptions.ComponentInvalid);
            New("*Xx", options).Throws(Exceptions.ComponentInvalid);

            // Invalid characters
            New("$").Throws(Exceptions.ComponentInvalid);



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<PartialComponent>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public ParsingFixture() : this(null!, default) { }

            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;
            public PartialComponent Expected { get; private set; }

            public LooseParsingFixture Returns(PartialComponent expected)
            {
                MarkAsComplete();
                Expected = expected;
                return Extend<LooseParsingFixture>();
            }
            public override void AssertResult(PartialComponent result)
                => Assert.Equal(Expected, result);

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Options})";
        }
        public class LooseParsingFixture : FixtureExtender<ParsingFixture>
        {
            public void ButStrictThrows(string exceptionMessage)
            {
                Assert.NotEqual(SemverOptions.Strict, Prototype.Options);
                ParsingFixture newFixture = new ParsingFixture(Prototype.Source, SemverOptions.Strict);
                newFixture.Throws(typeof(ArgumentException), exceptionMessage);
                Adapter.Add(newFixture);
            }
        }

    }
}
