using System;
using System.Collections.Generic;
using System.Linq;
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



            // copy fixtures from semantic version tests
            foreach (SemanticVersionTests.ParsingFixture other in SemanticVersionTests.CreateParsingFixtures())
            {
                // exclude fixtures with optional components (PartialVersion handles them differently)
                const SemverOptions excludeOptional = ~(SemverOptions.OptionalMinor | SemverOptions.OptionalPatch);
                if (other.IsValid
                    ? !SemanticVersion.TryParse(other.Source, other.Options & excludeOptional, out _)
                    : other.ExceptionMessage is Exceptions.MinorNotFound or Exceptions.PatchNotFound)
                {
                    continue;
                }

                object[] identifiers = [.. other.BuildMetadata ?? []];
                if (identifiers.Length > 0) identifiers[0] = $"+{identifiers[0]}";
                identifiers = (other.PreReleases ?? []).Concat(identifiers).ToArray();

                ParsingFixture fixture = New(other.Source, other.Options);
                if (other.IsValid)
                    fixture.Returns(other.Major, other.Minor, other.Patch, identifiers);
                else
                    fixture.Throws(other.ExceptionType, other.ExceptionMessage);
            }



            // Wildcard components
            New("1.2.x").Returns(1, 2, 'x');
            // Identifiers and components after wildcards technically ARE allowed, they're just ignored when matching
            New("1.x.*-pre+build").Returns(1, 'x', '*', "pre", "+build");
            New("1.X.5-pre+build").Returns(1, 'x', 5, "pre", "+build");
            New("*.x.X-pre+build").Returns('*', 'x', 'X', "pre", "+build");

            // Omitted components
            New("1.2").Returns(1, 2, null);
            New("1").Returns(1, null, null);
            New("").Throws(Exceptions.MajorNotFound);
            // Omitted + wildcard components
            New("1.x").Returns(1, 'x', null);
            New("X.5").Returns('X', 5, null);
            New("*").Returns('*', null, null);

            // Identifiers are not allowed after an omitted component
            New("1.x-pre").Throws(Exceptions.PreReleaseAfterOmitted);
            New("*-pre").Throws(Exceptions.PreReleaseAfterOmitted);
            New("1.X+build").Throws(Exceptions.BuildMetadataAfterOmitted);
            New("*+build").Throws(Exceptions.BuildMetadataAfterOmitted);

            // Extra wildcard characters
            options = SemverOptions.AllowExtraWildcards;
            New("****.XXX.x-pre+build", options).Returns('*', 'X', 'x', "pre", "+build").ButStrictThrows(Exceptions.MajorInvalid);
            New("1.xx.3-pre+build", options).Returns(1, 'x', 3, "pre", "+build").ButStrictThrows(Exceptions.MinorInvalid);
            New("1.2.XXX-pre+build", options).Returns(1, 2, 'X', "pre", "+build").ButStrictThrows(Exceptions.PatchInvalid);



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<PartialVersion>
        {
            public ParsingFixture() : this(null!, default) { }

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
