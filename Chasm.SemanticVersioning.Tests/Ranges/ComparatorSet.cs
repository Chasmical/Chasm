using System;
using System.Collections.Generic;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class ComparatorSetTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(static () => new ComparatorSet([null!]));
            Assert.StartsWith(Exceptions.ComparatorsNull, ex.Message);
            ex = Assert.Throws<ArgumentException>(static () => new ComparatorSet(new List<Comparator> { null! }));
            Assert.StartsWith(Exceptions.ComparatorsNull, ex.Message);
        }

        [Fact]
        public void Constructors()
        {
            PrimitiveComparator a = PrimitiveComparator.GreaterThanOrEqual(new SemanticVersion(1, 2, 3));
            PrimitiveComparator b = PrimitiveComparator.LessThan(new SemanticVersion(2, 0, 0, [0]));

            // test single comparator constructor
            ComparatorSet set = new ComparatorSet(a);
            Assert.Same(a, Assert.Single(set.Comparators));

            // test params comparators constructor (empty params)
            set = new ComparatorSet([]);
            Assert.Empty(set.Comparators);

            // test params comparators constructor
            set = new ComparatorSet(a, b);
            Assert.Collection(
                set.Comparators,
                cs => Assert.Same(a, cs),
                cs => Assert.Same(b, cs)
            );

            // test IEnumerable comparators constructor
            set = new ComparatorSet(new List<Comparator> { a, b });
            Assert.Collection(
                set.Comparators,
                cs => Assert.Same(a, cs),
                cs => Assert.Same(b, cs)
            );

        }

        [Fact]
        public void Properties()
        {
            ComparatorSet set = VersionRange.Parse(">3.5.x <2.0.0-0 >=1.3.0")[0];

            // test comparator collections
            Assert.Equal(set._comparators, set.Comparators);
            Assert.Equal(set._comparators, set.GetComparators().ToArray());

            // make sure the read-only collection is memoized
            Assert.Same(set.Comparators, set.Comparators);

            // test the GetEnumerator() method
            List<Comparator> comparators = [];
            foreach (Comparator comparator in set)
                comparators.Add(comparator);
            Assert.Equal(set._comparators, comparators);

            // test this[int index] property
            for (int i = 0; i < set._comparators.Length; i++)
                Assert.Equal(set._comparators[i], set[i]);

        }

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal("<0.0.0-0", ComparatorSet.None.ToString());
            Assert.Equal("*", ComparatorSet.All.ToString());

            // make sure that properties aren't constantly creating new instances
            Assert.Same(ComparatorSet.None, ComparatorSet.None);
            Assert.Same(ComparatorSet.All, ComparatorSet.All);
        }

    }
}
