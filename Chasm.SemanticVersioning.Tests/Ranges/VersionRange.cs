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

        [Fact]
        public void Constructors()
        {
            PrimitiveComparator a = PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(1, 2, 3));
            PrimitiveComparator b = PrimitiveComparator.LessThan(new SemanticVersion(2, 0, 0, [0]));

            // test single comparator constructor
            VersionRange range = new VersionRange(a);
            Assert.Single(range.ComparatorSets);
            Assert.Single(range.ComparatorSets[0].Comparators, c => ReferenceEquals(c, a));

            // test params comparators constructor (empty params)
            range = new VersionRange(a, []);
            Assert.Single(range.ComparatorSets);
            Assert.Single(range.ComparatorSets[0].Comparators, c => ReferenceEquals(c, a));

            // test params comparators constructor
            range = new VersionRange(a, b);
            Assert.Collection(
                range.ComparatorSets,
                cs => Assert.Single(cs.Comparators, c => ReferenceEquals(c, a)),
                cs => Assert.Single(cs.Comparators, c => ReferenceEquals(c, b))
            );

            // test IEnumerable comparators constructor
            range = new VersionRange([a, b]);
            Assert.Collection(
                range.ComparatorSets,
                cs => Assert.Single(cs.Comparators, c => ReferenceEquals(c, a)),
                cs => Assert.Single(cs.Comparators, c => ReferenceEquals(c, b))
            );

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

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal("<0.0.0-0", VersionRange.None.ToString());
            Assert.Equal("*", VersionRange.All.ToString());

            // make sure that properties aren't constantly creating new instances
            Assert.Same(VersionRange.None, VersionRange.None);
            Assert.Same(VersionRange.All, VersionRange.All);
        }

    }
}
