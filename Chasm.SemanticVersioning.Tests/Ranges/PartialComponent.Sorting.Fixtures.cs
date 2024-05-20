using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialComponentTests
    {
        [Pure] public static PartialComponent[][] CreateSortingFixtures()
        {
            return [
                ['x', 'X', '*', null],
                [0],
                [1],
                [2],
                [24],
                [42],
                [201],
                [19812322],
                [int.MaxValue],
            ];
        }
    }
}
