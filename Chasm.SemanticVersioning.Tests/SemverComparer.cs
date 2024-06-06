using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverComparerTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void TestDefault()
        {
            // test the default comparer options with standard fixtures

            TestComparison(SemverComparer.Default, FixturesSemanticVersionDefault());

            TestComparison(SemverComparer.Default, FixturesPartialVersionDefault());

            TestComparison(SemverComparer.Default, FixturesPartialComponentDefault());
        }

        [Fact]
        public void TestIncludeBuild()
        {
            // test the comparer that includes build metadata in the comparison

            // IncludeBuild is the same as Exact for semantic versions
            TestComparison(SemverComparer.IncludeBuild, FixturesSemanticVersionExact());

            // use fixtures with build metadata
            TestComparison(SemverComparer.IncludeBuild, FixturesPartialVersionIncludeBuild());

            // build metadata comparison doesn't affect partial components
            TestComparison(SemverComparer.IncludeBuild, FixturesPartialComponentDefault());
        }

        [Fact]
        public void TestDiffWildcards()
        {
            // test the comparer that differentiates between wildcards

            // DiffWildcards doesn't affect semantic versions
            TestComparison(SemverComparer.DiffWildcards, FixturesSemanticVersionDefault());

            // use fixtures with different wildcards
            TestComparison(SemverComparer.DiffWildcards, FixturesPartialVersionDiffWildcards());

            TestComparison(SemverComparer.DiffWildcards, FixturesPartialComponentExact());
        }

        [Fact]
        public void TestExact()
        {
            // test the comparer that includes everything in the comparison

            TestComparison(SemverComparer.Exact, FixturesSemanticVersionExact());

            TestComparison(SemverComparer.Exact, FixturesPartialVersionExact());

            TestComparison(SemverComparer.Exact, FixturesPartialComponentExact());
        }

        internal void TestComparison<T>(IComparer<T> comparerT, T[][] items) where T : notnull
        {
            T a = default!, b = default!;
            IComparer comparer = (IComparer)comparerT;
            IEqualityComparer<T> equalityT = (IEqualityComparer<T>)comparerT;
            IEqualityComparer equality = (IEqualityComparer)equalityT;


            // test comparer with nulls only
            Assert.True(equality.Equals(null, null));
            Assert.Equal(0, equality.GetHashCode(null!));
            Assert.Equal(0, comparer.Compare(null, null));

            if (default(T) is null)
            {
                Assert.True(equalityT.Equals(default, default));
                Assert.Equal(0, equalityT.GetHashCode(default!));
                Assert.Equal(0, comparerT.Compare(default, default));
            }

            Assert.Throws<ArgumentException>(() => equality.GetHashCode("0"));
            Assert.Throws<ArgumentException>(() => equality.GetHashCode(0));

            try
            {
                for (int i = 0; i < items.Length; i++)
                {
                    T[] aRow = items[i];
                    for (int k = 0; k < aRow.Length; k++)
                    {
                        a = aRow[k];

                        // test generic comparer methods with one object
                        Assert.True(equalityT.Equals(a, a));
                        Assert.Equal(equalityT.GetHashCode(a), equalityT.GetHashCode(a));
                        Assert.Equal(0, comparerT.Compare(a, a));

                        // test non-generic comparer methods with one object
                        Assert.True(equality.Equals(a, a));
                        Assert.Equal(equality.GetHashCode(a), equality.GetHashCode(a));
                        Assert.Equal(0, comparer.Compare(a, a));

                        // if T is nullable, make sure nulls are handled correctly
                        if (default(T) is null)
                        {
                            // test generic methods
                            Assert.False(equalityT.Equals(a, default));
                            Assert.False(equalityT.Equals(default, a));
                            Assert.Equal(1, comparerT.Compare(a, default));
                            Assert.Equal(-1, comparerT.Compare(default, a));

                            // test non-generic methods
                            Assert.False(equality.Equals(a, default(T)));
                            Assert.False(equality.Equals(default(T), a));
                            Assert.Equal(1, comparer.Compare(a, default(T)));
                            Assert.Equal(-1, comparer.Compare(default(T), a));
                        }

                        // make sure it doesn't work with other types
                        Assert.Throws<ArgumentException>(() => equality.Equals(a, "0"));
                        Assert.Throws<ArgumentException>(() => equality.Equals(a, 0));
                        Assert.Throws<ArgumentException>(() => comparer.Compare(a, "0"));
                        Assert.Throws<ArgumentException>(() => comparer.Compare(a, 0));

                        for (int j = 0; j < items.Length; j++)
                        {
                            T[] bRow = items[j];
                            for (int l = 0; l < bRow.Length; l++)
                            {
                                b = bRow[l];

                                // test generic comparer methods with both objects
                                Assert.Equal(i == j, equalityT.Equals(a, b));
                                Assert.Equal(i == j, equalityT.GetHashCode(a) == equalityT.GetHashCode(b));
                                Assert.Equal(i.CompareTo(j), Math.Sign(comparerT.Compare(a, b)));

                                // test non-generic comparer methods with both objects
                                Assert.Equal(i == j, equality.Equals(a, b));
                                Assert.Equal(i == j, equality.GetHashCode(a) == equality.GetHashCode(b));
                                Assert.Equal(i.CompareTo(j), Math.Sign(comparer.Compare(a, b)));

                            }
                        }
                    }
                }
            }
            catch
            {
                Output.WriteLine($"Error comparing {a} with {b}");
                throw;
            }
        }

    }
}
