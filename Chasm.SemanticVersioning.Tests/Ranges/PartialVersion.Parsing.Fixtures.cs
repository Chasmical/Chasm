using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialVersionTests
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



            // add semantic version tests (include only fixtures with all version components)
            Regex hasAllComponents = new Regex(@"^[=vV\s]*\d+\s*\.\s*\d+\s*\.\s*\d+\s*");
            foreach (SemanticVersionTests.ParsingFixture other in SemanticVersionTests.CreateParsingFixtures())
            {
                if (!hasAllComponents.IsMatch(other.Source)) continue;
                if (other.ExceptionMessage is Exceptions.MinorNotFound or Exceptions.PatchNotFound) continue;

                object[] identifiers = [.. other.BuildMetadata ?? []];
                if (identifiers.Length > 0) identifiers[0] = $"+{identifiers[0]}";
                identifiers = (other.PreReleases ?? []).Concat(identifiers).ToArray();

                ParsingFixture fixture = New(other.Source, other.Options);
                if (other.IsValid)
                    fixture.Returns(other.Major, other.Minor, other.Patch, identifiers);
                else
                    fixture.Throws(other.ExceptionType, other.ExceptionMessage);
            }



            // TODO: omitted components

            // TODO: identifiers after omitted components



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<PartialVersion>
        {
            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;

            public PartialComponent Major { get; private set; }
            public PartialComponent Minor { get; private set; }
            public PartialComponent Patch { get; private set; }
            public IEnumerable<object>? PreReleases { get; private set; }
            public IEnumerable<string>? BuildMetadata { get; private set; }

            public LooseParsingFixture Returns(PartialComponent major, PartialComponent minor, PartialComponent patch, params object[] identifiers)
            {
                MarkAsComplete();

                Major = major;
                Minor = minor;
                Patch = patch;

                (PreReleases, BuildMetadata) = TestUtil.Split(identifiers, "+");

                return Extend<LooseParsingFixture>();
            }
            public override void AssertResult(PartialVersion? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Major, result.Major);
                Assert.Equal(Minor, result.Minor);
                Assert.Equal(Patch, result.Patch);
                Assert.Equal(PreReleases!, result.PreReleases.Select(static p => p.IsNumeric ? (object)(int)p : (string)p));
                Assert.Equal(BuildMetadata!, result.BuildMetadata);
            }

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
