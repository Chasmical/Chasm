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
        public void Constructors()
        {
            // test null argument exceptions
            Assert.Throws<ArgumentNullException>(static () => new VersionRange((ComparatorSet)null!));
            Assert.Throws<ArgumentNullException>(static () => new VersionRange((ComparatorSet[])null!));
            Assert.Throws<ArgumentException>(static () => new VersionRange([null!]));
            Assert.Throws<ArgumentNullException>(static () => new VersionRange(null!, [ComparatorSet.None]));
            Assert.Throws<ArgumentException>(static () => new VersionRange(ComparatorSet.None, [null!]));

            // try creating an empty version range
            ArgumentException e = Assert.Throws<ArgumentException>(static () => new VersionRange([]));
            Assert.StartsWith(Exceptions.VersionRangeEmpty, e.Message);

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
