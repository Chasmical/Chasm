using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Chasm.Collections;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Fact]
        public void Equality()
        {
            static void AssertEq(string leftRange, string rightRange, bool result, SemverComparison comparison = SemverComparison.Default)
            {
                VersionRange a = VersionRange.Parse(leftRange);
                VersionRange b = VersionRange.Parse(rightRange);
                SemverComparer cmp = SemverComparer.FromComparison(comparison);

                Assert.Equal(result, cmp.Equals(a, b));
                Assert.Equal(result, cmp.GetHashCode(a) == cmp.GetHashCode(b));

                if (comparison == SemverComparison.Default)
                {
                    Assert.Equal(result, a == b);
                    Assert.Equal(result, a.GetHashCode() == b.GetHashCode());
                }
            }

            AssertEq("^1.2.x", "^1.2", true);
            AssertEq("^1.2.x", "^1.2.*", true);
            AssertEq("^1.*.x", "^1.x.*", true);
            AssertEq("^x.x", "^*", true);

            AssertEq("^1.2.x", "^1.2", false, SemverComparison.DiffWildcards);
            AssertEq("^1.2.x", "^1.2.*", false, SemverComparison.DiffWildcards);
            AssertEq("^1.*.x", "^1.x.*", false, SemverComparison.DiffWildcards);
            AssertEq("^x.x", "^*.*", false, SemverComparison.DiffWildcards);

            AssertEq("1.2 - 3.4", "1.2.x - 3.4.*", true);
            AssertEq("1.2 - 3.4", "1.2.x - 3.4.*", false, SemverComparison.DiffWildcards);

            AssertEq("=1.2.3", "1.2.3", true);
            AssertEq("=1.2.3", "1.2.3", false, SemverComparison.DiffEquality);
            AssertEq("=1.2.x", "1.2.x", true);
            AssertEq("=1.2.x", "1.2.x", false, SemverComparison.DiffEquality);

            SemverComparer comparer = SemverComparer.Exact;
            Assert.Equal(0, comparer.GetHashCode((Comparator?)null));
            Assert.Equal(0, comparer.GetHashCode((ComparatorSet?)null));
            Assert.Equal(0, comparer.GetHashCode((VersionRange?)null));

        }

        [Theory, MemberData(nameof(CreateOperationFixtures))]
        public void EqualityTheory(OperationFixture fixture)
        {
            if (!fixture.IsValid) return;

            VersionRange?[] ranges =
            [
                VersionRange.Parse(fixture.Left),
                fixture.Right is not null ? VersionRange.Parse(fixture.Right) : null,
                fixture.Expected is not null ? VersionRange.Parse(fixture.Expected) : null,
            ];

            foreach (VersionRange range in ranges.NotNull())
            {
                // make sure the range and its components are always equal to themselves
                TestEquality(range, range);
                foreach (ComparatorSet set in range)
                {
                    TestEquality(set, set);
                    foreach (Comparator comparator in set)
                        TestEquality(comparator, comparator);
                }

                // create a copy and make sure it's equal to the original
                VersionRange copy = VersionRange.Parse(range.ToString());

                TestEquality(range, copy);
                TestEquality(copy, range);

                for (int i = 0; i < range.ComparatorSets.Count; i++)
                {
                    TestEquality(range[i], copy[i]);
                    TestEquality(copy[i], range[i]);

                    for (int j = 0; j < range[i].Comparators.Count; j++)
                    {
                        TestEquality(range[i][j], copy[i][j]);
                        TestEquality(copy[i][j], range[i][j]);
                    }
                }
            }

            static void TestEquality<T>(T a, T b) where T : class, IEqualityOperators<T, T, bool>
            {
                IEqualityComparer cmp = SemverComparer.Default;
                IEqualityComparer exact = SemverComparer.Exact;
                IEqualityComparer<T> cmpT = (IEqualityComparer<T>)cmp;
                IEqualityComparer<T> exactT = (IEqualityComparer<T>)exact;
                T nil = null!;

                Assert.True(a == b);
                Assert.True(a.Equals(b));
                Assert.True(cmp.Equals(a, b));
                Assert.True(cmpT.Equals(a, b));
                Assert.True(exact.Equals(a, b));
                Assert.True(exactT.Equals(a, b));

                Assert.False(a == nil);
                Assert.False(nil == a);
                Assert.False(a.Equals(nil));
                Assert.False(cmp.Equals(a, nil));
                Assert.False(cmp.Equals(nil, a));
                Assert.False(cmpT.Equals(a, nil));
                Assert.False(cmpT.Equals(nil, a));
                Assert.False(exact.Equals(a, nil));
                Assert.False(exact.Equals(nil, a));
                Assert.False(exactT.Equals(a, nil));
                Assert.False(exactT.Equals(nil, a));

                Assert.Equal(a.GetHashCode(), b.GetHashCode());
                Assert.Equal(cmp.GetHashCode(a), cmp.GetHashCode(b));
                Assert.Equal(cmpT.GetHashCode(a), cmpT.GetHashCode(b));
                Assert.Equal(exact.GetHashCode(a), exact.GetHashCode(b));
                Assert.Equal(exactT.GetHashCode(a), exactT.GetHashCode(b));
            }
        }

        [Fact]
        public void EqualityNull()
        {
            TestEqualityNulls<VersionRange>();
            TestEqualityNulls<ComparatorSet>();
            TestEqualityNulls<Comparator>();

            static void TestEqualityNulls<T>() where T : class, IEqualityOperators<T, T, bool>
            {
                IEqualityComparer cmp = SemverComparer.Default;
                IEqualityComparer exact = SemverComparer.Default;
                IEqualityComparer<T> cmpT = (IEqualityComparer<T>)cmp;
                IEqualityComparer<T> exactT = (IEqualityComparer<T>)exact;
                T nil = null!;

                Assert.True(nil == nil);
                Assert.True(cmp.Equals(nil, nil));
                Assert.True(cmpT.Equals(nil, nil));
                Assert.True(exact.Equals(nil, nil));
                Assert.True(exactT.Equals(nil, nil));
            }
        }

    }
}
