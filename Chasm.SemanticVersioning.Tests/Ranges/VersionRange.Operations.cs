using System;
using System.Collections.Generic;
using Chasm.SemanticVersioning.Ranges;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Theory, MemberData(nameof(CreateOperationFixtures))]
        public void Operations(OperationFixture fixture)
        {
            AssertOperationFixture(fixture);

            if (fixture.IsCommutative && fixture.Right is not null)
            {
                Output.WriteLine("\n");
                OperationFixture altered = new OperationFixture(fixture.Operation, fixture.Right, fixture.Left);
                altered.Returns(fixture.Expected!, false);
                AssertOperationFixture(altered);
            }
        }

        private void AssertOperationFixture(OperationFixture fixture)
        {
            VersionRange leftRange = VersionRange.Parse(fixture.Left);
            VersionRange? rightRange = fixture.Right is null ? null : VersionRange.Parse(fixture.Right);

            Output.WriteLine($"{leftRange}\n{fixture.Operation}\n{rightRange}\n=\n{fixture.Expected}");

            static T? AsSingle<T>(IList<T>? list)
                => list?.Count == 1 ? list[0] : default;

            ComparatorSet? leftSet = AsSingle(leftRange.ComparatorSets);
            ComparatorSet? rightSet = AsSingle(rightRange?.ComparatorSets);

            Comparator? leftComparator = AsSingle(leftSet?.Comparators);
            Comparator? rightComparator = AsSingle(rightSet?.Comparators);

            // TODO: add tests to ensure that singleton All and None instances are used when possible

            switch (fixture.Operation)
            {
                case '~':
                    Assert.Null(rightRange);

                    // Test operations on comparator sets and comparators
                    if (leftComparator is not null)
                        fixture.Test(() => ~leftComparator);
                    if (leftSet is not null)
                        fixture.Test(() => ~leftSet);

                    // Test operations on ranges
                    fixture.Test(() => ~leftRange);
                    break;
                case '|':
                    Assert.NotNull(rightRange);

                    // Test operations on comparator sets and comparators
                    if (leftComparator is not null && rightComparator is not null)
                        fixture.Test(() => leftComparator | rightComparator);
                    if (leftSet is not null && rightSet is not null)
                        fixture.Test(() => leftSet | rightSet);

                    // Test operations on ranges
                    fixture.Test(() => leftRange | rightRange);
                    break;
                case '&':
                    Assert.NotNull(rightRange);

                    // Test operations on comparator sets and comparators
                    if (leftComparator is not null && rightComparator is not null)
                        fixture.Test(() => leftComparator & rightComparator);
                    if (leftSet is not null && rightSet is not null)
                        fixture.Test(() => leftSet & rightSet);

                    // Test operations on ranges
                    fixture.Test(() => leftRange & rightRange);
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }
    }
}
