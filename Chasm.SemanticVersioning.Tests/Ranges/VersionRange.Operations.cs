using System;
using System.Linq;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Theory, MemberData(nameof(CreateOperationFixtures))]
        public void Operations(OperationFixture fixture)
        {
            VersionRange leftRange = VersionRange.Parse(fixture.Left);
            VersionRange? rightRange = fixture.Right is null ? null : VersionRange.Parse(fixture.Right);

            ComparatorSet? leftSet = leftRange.ComparatorSets.SingleOrDefault();
            ComparatorSet? rightSet = rightRange?.ComparatorSets.SingleOrDefault();

            Comparator? leftComparator = leftSet?.Comparators.SingleOrDefault();
            Comparator? rightComparator = rightSet?.Comparators.SingleOrDefault();

            switch (fixture.Operation)
            {
                case '~':
                    Assert.Null(rightRange);

                    //fixture.Test(() => ~leftRange);

                    // Test operations on comparator sets and comparators
                    //if (leftSet is not null)
                    //    fixture.Test(() => ~leftSet);
                    if (leftComparator is not null)
                        fixture.Test(() => ~leftComparator);
                    break;
                case '&':
                    Assert.NotNull(rightRange);
                    //fixture.Test(() => leftRange & rightRange);

                    // Test operations on comparator sets and comparators
                    //if (leftSet is not null && rightSet is not null)
                    //    fixture.Test(() => leftSet & rightSet);
                    if (leftComparator is not null && rightComparator is not null)
                        fixture.Test(() => leftComparator & rightComparator);

                    break;
                case '|':
                    Assert.NotNull(rightRange);
                    //fixture.Test(() => leftRange | rightRange);

                    // Test operations on comparator sets and comparators
                    //if (leftSet is not null && rightSet is not null)
                    //    fixture.Test(() => leftSet | rightSet);
                    if (leftComparator is not null && rightComparator is not null)
                        fixture.Test(() => leftComparator | rightComparator);

                    break;
                default:
                    throw new InvalidOperationException();
            }

        }
    }
}
