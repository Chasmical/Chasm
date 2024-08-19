using System;
using System.Collections.Generic;
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
            Assert.Throws<ArgumentNullException>(static () => new VersionRange(null!, ComparatorSet.None));

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
            ComparatorSet a = PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(1, 2, 3));
            ComparatorSet b = PrimitiveComparator.LessThan(new SemanticVersion(2, 0, 0, [0]));

            // test single comparator set constructor
            VersionRange range = new VersionRange(a);
            Assert.Same(a, Assert.Single(range.ComparatorSets));

            // test params comparator sets constructor (empty params)
            range = new VersionRange(a, []);
            Assert.Same(a, Assert.Single(range.ComparatorSets));

            // test params comparator sets constructor
            range = new VersionRange(a, b);
            Assert.Collection(
                range.ComparatorSets,
                cs => Assert.Same(a, cs),
                cs => Assert.Same(b, cs)
            );

            // test IEnumerable comparator sets constructor
            range = new VersionRange([a, b]);
            Assert.Collection(
                range.ComparatorSets,
                cs => Assert.Same(a, cs),
                cs => Assert.Same(b, cs)
            );

        }

        [Fact]
        public void Properties()
        {
            VersionRange range = VersionRange.Parse("^0.2 || >=0.3.2 <0.4.0 || ~1.2 || 2.3 - 4.4.x");

            // test comparator set collections
            Assert.Equal(range._comparatorSets, range.ComparatorSets);
            Assert.Equal(range._comparatorSets, range.GetComparatorSets().ToArray());

            // make sure the read-only collection is memoized
            Assert.Same(range.ComparatorSets, range.ComparatorSets);

            // test the GetEnumerator() method
            List<ComparatorSet> comparatorSets = [];
            foreach (ComparatorSet comparatorSet in range)
                comparatorSets.Add(comparatorSet);
            Assert.Equal(range._comparatorSets, comparatorSets);

            // test this[int index] property
            for (int i = 0; i < range._comparatorSets.Length; i++)
                Assert.Equal(range._comparatorSets[i], range[i]);

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
