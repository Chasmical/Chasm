using System;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<ParsingFixture> CreateParsingFixtures()
        {
            FixtureAdapter<ParsingFixture> adapter = [];

            ParsingFixture New(string source, SemverOptions parsingOptions = SemverOptions.Strict)
                => adapter.Add(new ParsingFixture(source, parsingOptions));

            // ReSharper disable once JoinDeclarationAndInitializer
            SemverOptions options;



            const SemverOptions removeOptions
                = SemverOptions.AllowEqualsPrefix // '=' is used by comparison comparators
                | SemverOptions.AllowInnerWhite // interferes with the hyphen range syntax
                | SemverOptions.OptionalMinor // interferes with partial versions
                | SemverOptions.OptionalPatch
                | SemverOptions.AllowLeftovers; // interferes with the rest of the version range

            // Make sure all comparators work individually
            foreach (PartialVersionTests.ParsingFixture fixture in PartialVersionTests.CreateParsingFixtures())
            {
                if (!fixture.IsValid) continue;
                SemverOptions opt = fixture.Options & ~removeOptions;

                // make sure the version is still parsable with new options
                if (!PartialVersion.TryParse(fixture.Source, opt, out PartialVersion? partial)) continue;

                // re-add the whitespace option between comparator parts
                if ((fixture.Options & (SemverOptions.AllowLeadingWhite | SemverOptions.AllowTrailingWhite)) != 0)
                    opt |= SemverOptions.AllowInnerWhite;

                // Single X-Range and primitive comparators
                if (partial.IsPartial)
                {
                    New($"{fixture.Source}", opt).Returns(XRangeComparator.ImplicitEqual(partial));
                    New($"={fixture.Source}", opt).Returns(XRangeComparator.Equal(partial));
                    New($">{fixture.Source}", opt).Returns(XRangeComparator.GreaterThan(partial));
                    New($"<{fixture.Source}", opt).Returns(XRangeComparator.LessThan(partial));
                    New($">={fixture.Source}", opt).Returns(XRangeComparator.GreaterThanOrEqual(partial));
                    New($"<={fixture.Source}", opt).Returns(XRangeComparator.LessThanOrEqual(partial));
                }
                else
                {
                    SemanticVersion version = (SemanticVersion)partial;
                    New($"{fixture.Source}", opt).Returns(PrimitiveComparator.ImplicitEqual(version));
                    New($"={fixture.Source}", opt).Returns(PrimitiveComparator.Equal(version));
                    New($">{fixture.Source}", opt).Returns(PrimitiveComparator.GreaterThan(version));
                    New($"<{fixture.Source}", opt).Returns(PrimitiveComparator.LessThan(version));
                    New($">={fixture.Source}", opt).Returns(PrimitiveComparator.GreaterThanOrEqual(version));
                    New($"<={fixture.Source}", opt).Returns(PrimitiveComparator.LessThanOrEqual(version));
                }

                // Single advanced comparators
                New($"^{fixture.Source}", opt).Returns(new CaretComparator(partial));
                New($"~{fixture.Source}", opt).Returns(new TildeComparator(partial));
                New($"{fixture.Source} - {fixture.Source}", opt).Returns(new HyphenRangeComparator(partial, partial));

                // Invalid characters
                New($"{fixture.Source.TrimEnd()}$$$", opt).Throws(Exceptions.Leftovers);
            }



            // "Empty" version range (single empty comparator set), equivalent to *
            New("").Returns(new ComparatorSet());

            // Version range with multiple comparators
            New(">=1.2.3 <5.0.0").Returns(new ComparatorSet(
                PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(1, 2, 3)),
                PrimitiveComparator.LessThan(new SemanticVersion(5, 0, 0))
            ));
            New(">5.0.0 <=12.0.0 <45.0.0").Returns(new ComparatorSet(
                PrimitiveComparator.GreaterThan(new SemanticVersion(5, 0, 0)),
                PrimitiveComparator.LessThanOrEqual(new SemanticVersion(12, 0, 0)),
                PrimitiveComparator.LessThan(new SemanticVersion(45, 0, 0))
            ));

            // Version range with multiple comparator sets
            New(">=1.2.0 <=1.5.0 || >1.7.0-alpha.5 <2.0.0-0 || >=3.0.0-beta.4").Returns([
                new ComparatorSet(
                    PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(1, 2, 0)),
                    PrimitiveComparator.LessThanOrEqual(new SemanticVersion(1, 5, 0))
                ),
                new ComparatorSet(
                    PrimitiveComparator.GreaterThan(new SemanticVersion(1, 7, 0, ["alpha", 5])),
                    PrimitiveComparator.LessThan(new SemanticVersion(2, 0, 0, [0]))
                ),
                PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(3, 0, 0, ["beta", 4])),
            ]);

            // X-Ranges and Hyphen ranges
            New("1.x 2.0 - 3.0.5 <=3.0.2 || 1.2 - 1.4.5-beta").Returns([
                new ComparatorSet(
                    XRangeComparator.ImplicitEqual(new PartialVersion(1, 'x')),
                    new HyphenRangeComparator(new PartialVersion(2, 0), new PartialVersion(3, 0, 5)),
                    PrimitiveComparator.LessThanOrEqual(new SemanticVersion(3, 0, 2))
                ),
                new HyphenRangeComparator(new PartialVersion(1, 2), new PartialVersion(1, 4, 5, ["beta"])),
            ]);

            // Caret and Tilde ranges
            New("^2.0.0-beta.5 <2.5.0 || ~1.2.* >1.2.4").Returns([
                new ComparatorSet(
                    new CaretComparator(new PartialVersion(2, 0, 0, ["beta", 5])),
                    PrimitiveComparator.LessThan(new SemanticVersion(2, 5, 0))
                ),
                new ComparatorSet(
                    new TildeComparator(new PartialVersion(1, 2, '*')),
                    PrimitiveComparator.GreaterThan(new SemanticVersion(1, 2, 4))
                ),
            ]);



            // Handling errors in partial versions
            New("^2147483648").Throws(Exceptions.MajorTooBig);
            New("~2147483648").Throws(Exceptions.MajorTooBig);
            New("=2147483648").Throws(Exceptions.MajorTooBig);
            New(">2147483648").Throws(Exceptions.MajorTooBig);
            New("<2147483648").Throws(Exceptions.MajorTooBig);
            New(">=2147483648").Throws(Exceptions.MajorTooBig);
            New("<=2147483648").Throws(Exceptions.MajorTooBig);
            New("2147483648 - 123").Throws(Exceptions.MajorTooBig);
            New("123 - 2147483648").Throws(Exceptions.MajorTooBig);



            // Any amount of spaces is valid between comparator sets
            New(">1.2.3    ||  <3.4.5").Returns([
                PrimitiveComparator.GreaterThan(new SemanticVersion(1, 2, 3)),
                PrimitiveComparator.LessThan(new SemanticVersion(3, 4, 5)),
            ]);

            // Only one space between individual comparators
            New(">1.2.3 <3.4.5 || ^5.6.0").Returns([
                new ComparatorSet(
                    PrimitiveComparator.GreaterThan(new SemanticVersion(1, 2, 3)),
                    PrimitiveComparator.LessThan(new SemanticVersion(3, 4, 5))
                ),
                new CaretComparator(new PartialVersion(5, 6, 0)),
            ]);
            New(">1.2.3   <3.4.5 || ^5.6.0").Throws(Exceptions.Leftovers);

            // Various amounts of whitespace in a hyphen range
            options = SemverOptions.AllowInnerWhite;
            New("1.2\n  \r- \t4.5.6", options).Returns([
                new HyphenRangeComparator(new PartialVersion(1, 2), new PartialVersion(4, 5, 6)),
            ]);

            // Invalid amounts of whitespace for hyphen ranges
            New("1 \t-4.5", options).Throws(Exceptions.MajorNotFound);
            New("1.2.3- 4.5", options).Throws(Exceptions.PreReleaseEmpty);

            New("1.2.3-4.5", options).Returns([
                PrimitiveComparator.ImplicitEqual(new SemanticVersion(1, 2, 3, [4, 5])),
            ]);



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<VersionRange>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public ParsingFixture() : this(null!, default) { }

            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;

            public VersionRange? Expected { get; private set; }

            private void MarkDiscard(object? _) => MarkAsComplete();

            public void Returns(Comparator comparator) => MarkDiscard(Expected = comparator);
            public void Returns(ComparatorSet comparatorSet) => MarkDiscard(Expected = comparatorSet);
            public void Returns(ComparatorSet[] comparatorSets) => MarkDiscard(Expected = new VersionRange(comparatorSets));

            public override void AssertResult(VersionRange? result)
            {
                Assert.Equal(Expected, result);
                Assert.Equal(Expected, result, SemverComparer.Exact!);
                Assert.Equal(Expected?.ToString(), result?.ToString());
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Options})";
        }
    }
}
