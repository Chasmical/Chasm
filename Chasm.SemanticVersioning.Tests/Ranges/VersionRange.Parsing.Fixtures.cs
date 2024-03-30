using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure]
        public static FixtureAdapter<ParsingFixture> CreateParsingFixtures()
        {
            FixtureAdapter<ParsingFixture> adapter = [];

            ParsingFixture New(string source, SemverOptions parsingOptions = SemverOptions.Strict)
                => adapter.Add(new ParsingFixture(source, parsingOptions));

            // ReSharper disable once JoinDeclarationAndInitializer
            SemverOptions options;



            // Make sure all version comparators work individually
            foreach (SemanticVersionTests.ParsingFixture fixture in SemanticVersionTests.CreateParsingFixtures())
            {
                if (!fixture.IsValid) continue;
                const SemverOptions removeOptions = SemverOptions.AllowEqualsPrefix // '=' is used by comparison comparators
                                                  | SemverOptions.AllowInnerWhite // interferes with the hyphen range syntax
                                                  | SemverOptions.OptionalMinor // interferes with partial versions
                                                  | SemverOptions.OptionalPatch
                                                  | SemverOptions.AllowLeftovers; // interferes with the rest of the version range

                SemverOptions opt = fixture.Options & ~removeOptions;

                // make sure the version is still parsable with new options
                if (!SemanticVersion.TryParse(fixture.Source, opt, out SemanticVersion? version)) continue;

                // re-add the whitespace option between comparator parts
                if ((fixture.Options & (SemverOptions.AllowLeadingWhite | SemverOptions.AllowTrailingWhite)) != 0)
                    opt |= SemverOptions.AllowInnerWhite;

                // Single primitive comparators
                New($"{fixture.Source}", opt).Returns(PrimitiveComparator.ImplicitEqual(version));
                New($"={fixture.Source}", opt).Returns(PrimitiveComparator.Equal(version));
                New($">{fixture.Source}", opt).Returns(PrimitiveComparator.GreaterThan(version));
                New($"<{fixture.Source}", opt).Returns(PrimitiveComparator.LessThan(version));
                New($">={fixture.Source}", opt).Returns(PrimitiveComparator.GreaterThanOrEqual(version));
                New($"<={fixture.Source}", opt).Returns(PrimitiveComparator.LessThanOrEqual(version));

                // Single advanced comparators
                New($"^{fixture.Source}", opt).Returns(new CaretComparator(version));
                New($"~{fixture.Source}", opt).Returns(new TildeComparator(version));
                New($"{fixture.Source} - {fixture.Source}", opt).Returns(new HyphenRangeComparator(version, version));

                // Invalid characters
                New($"{fixture.Source.TrimEnd()}$$$", opt).Throws(Exceptions.Leftovers);
            }

            // TODO: do the same with partial versions later



            return adapter;
        }

        public class ParsingFixture(string source, SemverOptions options) : FuncFixture<VersionRange>
        {
            public string Source { get; } = source;
            public SemverOptions Options { get; } = options;

            public VersionRange? Expected { get; private set; }

            private void MarkDiscard(object? _) => MarkAsComplete();

            public void Returns(Comparator comparator) => MarkDiscard(Expected = comparator);
            public void Returns(Comparator[] comparators) => MarkDiscard(Expected = new VersionRange([..comparators]));
            public void Returns(ComparatorSet comparatorSet) => MarkDiscard(Expected = comparatorSet);
            public void Returns(ComparatorSet[] comparatorSets) => MarkDiscard(Expected = new VersionRange(comparatorSets));

            public override void AssertResult(VersionRange? result)
            {
                Assert.NotNull(result);
                // Note: It's hard to do a proper assertion with so many comparator types. This should be fine.
                Assert.Equal(Expected?.ToString(), result.ToString());
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\" ({Options})";
        }
    }
}
