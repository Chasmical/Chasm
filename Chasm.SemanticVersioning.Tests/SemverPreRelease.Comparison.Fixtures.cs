using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        [Pure] public static SemverPreRelease[] CreateComparisonFixtures() =>
        [
            "0",
            "1",
            "3",
            "10",
            "100",
            "293",
            "1000",
            "2147483647",
            "--0",
            "--alpha",
            "-0",
            "-1",
            "-1024",
            "-32",
            "GAMMA",
            "alpha",
            "alpha0",
            "alpha1",
            "alpha10",
            "alpha2",
            "gamma",
            "omega",
            "rc",
        ];
    }
}
