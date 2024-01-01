using System;
using JetBrains.Annotations;
using Xunit;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        [Fact]
        public void Comparison()
        {
            SemverPreRelease[] fixtures = CreateComparisonFixtures();
            SemverPreRelease a = default, b = default;

            try
            {
                for (int i = 0; i < fixtures.Length; i++)
                {
                    a = fixtures[i];

                    // Test Equals and CompareTo against null
                    Assert.False(((object)a).Equals(null));
                    Assert.Equal(1, ((IComparable)a).CompareTo(null));

                    // Make sure they don't work with objects of other types
                    Assert.False(((object)a).Equals("0"));
                    Assert.False(((object)a).Equals(0));
                    Assert.Throws<ArgumentException>(() => ((IComparable)a).CompareTo("0"));
                    Assert.Throws<ArgumentException>(() => ((IComparable)a).CompareTo(0));

                    // Test against other pre-release identifiers
                    for (int j = 0; j < fixtures.Length; j++)
                    {
                        b = fixtures[j];

                        // Test Equals and CompareTo implementations
                        Assert.Equal(i.Equals(j), a.Equals(b));
                        Assert.Equal(i.Equals(j), ((object)a).Equals(b));
                        // As specified by IComparable, CompareTo doesn't necessarily return -1 or 1 on inequality
                        Assert.Equal(i.CompareTo(j), Math.Sign(a.CompareTo(b)));
                        Assert.Equal(i.CompareTo(j), Math.Sign(((IComparable)a).CompareTo(b)));

                        // Test overloaded operators
                        Assert.Equal(i == j, a == b);
                        Assert.Equal(i != j, a != b);
                        Assert.Equal(i > j, a > b);
                        Assert.Equal(i < j, a < b);
                        Assert.Equal(i >= j, a >= b);
                        Assert.Equal(i <= j, a <= b);

                    }
                }
            }
            catch
            {
                Output.WriteLine($"Error comparing {a} with {b}");
                throw;
            }
        }

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
