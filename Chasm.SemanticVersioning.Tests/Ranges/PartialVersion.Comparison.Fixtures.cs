using Chasm.Collections;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialVersionTests
    {
        [Pure] public static PartialVersion[][] CreateComparisonFixtures()
        {
            string[][] sources =
            [
                ["x.x.x", "*", "X.*"],
                ["x.x.5", "*.X.5"],
                ["x.2.*", "*.2", "X.2.*"],
                ["0.0.x-0", "0.0.X-0", "0.0.*-0"],
                ["0.0.x", "0.0.X", "0.0.*", "0.0"],
                [SemanticVersion.MinValue.ToString()],
                ["0.0.0-0.0"],
                ["0.0.0--"],
                ["0.0.0-A"],
                ["0.0.0-Z"],
                ["0.0.0-a"],
                ["0.0.0-z"],
                ["0.0.1"],
                ["0.0.4"],
                ["0.1.x", "0.1.X", "0.1.*", "0.1"],
                ["0.1.0-pre"],
                ["0.1.0"],
                ["0.1.9"],
                ["0.5.0"],
                ["1.0.0"],
                ["1.2.3-a"],
                ["1.2.3-al"],
                ["1.2.3-al.beta"],
                ["1.2.3-al-beta"],
                ["1.2.3-alpha"],
                ["1.2.3-beta"],
                ["1.2.3-beta.0"],
                ["1.2.3-beta.91"],
                ["1.2.3-beta.456"],
                ["1.2.3-beta.gamma"],
                ["1.2.3"],
                ["4.5.6-0"],
                ["4.5.6-pre.0"],
                ["4.5.6"],
                ["99.x.*", "99.X.x", "99.*.X", "99"],
                ["99.x.5", "99.X.5", "99.*.5"],
                ["99.99.99-0"],
                ["99.99.99"],
                [SemanticVersion.MaxValue.ToString()],
            ];
            return sources.ConvertAll(static r => r.ConvertAll(PartialVersion.Parse));
        }
    }
}
