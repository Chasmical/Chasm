using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test null argument exceptions
            Assert.Throws<ArgumentNullException>(static () => new VersionRange((ComparatorSet)null!));
            Assert.Throws<ArgumentNullException>(static () => new VersionRange((ComparatorSet[])null!));
            Assert.Throws<ArgumentNullException>(static () => new VersionRange(null!, [ComparatorSet.None]));

            // test constructors with null comparator sets
            ArgumentException ex = Assert.Throws<ArgumentException>(static () => new VersionRange([null!]));
            Assert.StartsWith(Exceptions.ComparatorSetsNull, ex.Message);
            ex = Assert.Throws<ArgumentException>(static () => new VersionRange(ComparatorSet.None, [null!]));
            Assert.StartsWith(Exceptions.ComparatorSetsNull, ex.Message);

            // try creating an empty version range
            ex = Assert.Throws<ArgumentException>(static () => new VersionRange([]));
            Assert.StartsWith(Exceptions.VersionRangeEmpty, ex.Message);

        }

        [Theory, MemberData(nameof(CreateParsingFixtures))]
        public void Properties(ParsingFixture fixture)
        {
            if (!fixture.IsValid) return;
            VersionRange range = VersionRange.Parse(fixture.Source, fixture.Options);

            // test comparator set collections
            Assert.Equal(range._comparatorSets, range.ComparatorSets);
            Assert.Equal(range._comparatorSets, range.GetComparatorSets().ToArray());

            // make sure the read-only collection is memoized
            Assert.Same(range.ComparatorSets, range.ComparatorSets);

        }


    }
}
