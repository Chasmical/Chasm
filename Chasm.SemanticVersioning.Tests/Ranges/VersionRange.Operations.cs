using System;
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
            _ = VersionRange.TryParse(fixture.Right, out VersionRange? rightRange);

            // TODO: at the moment, only operations with one comparator in each operand are tested

            Comparator left = Assert.Single(Assert.Single(leftRange.ComparatorSets).Comparators);
            Comparator? right = rightRange is null ? null : Assert.Single(Assert.Single(rightRange.ComparatorSets).Comparators);

            switch (fixture.Operation)
            {
                case '~':
                    //fixture.Test(() => ~left);
                    break;
                case '&':
                    Assert.NotNull(right);
                    fixture.Test(() => left & right);
                    break;
                case '|':
                    Assert.NotNull(right);
                    fixture.Test(() => left | right);
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }
    }
}
