using Chasm.Collections;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Fact]
        public void Equality()
        {
            static void AssertEq(string left, string right, bool result, SemverComparison comparison = SemverComparison.Default)
            {
                VersionRange leftRange = VersionRange.Parse(left);
                VersionRange rightRange = VersionRange.Parse(right);
                if (result)
                {
                    if (comparison == SemverComparison.Default) Assert.Equal(leftRange, rightRange);
                    Assert.Equal(leftRange, rightRange, SemverComparer.FromComparison(comparison));
                }
                else
                {
                    if (comparison == SemverComparison.Default) Assert.NotEqual(leftRange, rightRange);
                    Assert.NotEqual(leftRange, rightRange, SemverComparer.FromComparison(comparison));
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
                VersionRange copy = VersionRange.Parse(range.ToString());

                Assert.Equal(range, copy);
                Assert.Equal(range, copy, SemverComparer.Default);
                Assert.Equal(range, copy, SemverComparer.FromComparison(SemverComparison.Exact));
            }

        }

    }
}
