using System;
using Xunit;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Fact]
        public void Comparison()
        {
            SemanticVersion[] fixtures1 = CreateComparisonFixtures();
            SemanticVersion[] fixtures2 = CreateComparisonFixtures();
            SemanticVersion? a = default, b = default;

            try
            {
                for (int i = 0; i < fixtures1.Length; i++)
                {
                    a = fixtures1[i];

                    // Test Equals and CompareTo against null
                    Assert.False(a.Equals(null));
                    Assert.False(((object)a).Equals(null));
                    Assert.Equal(1, a.CompareTo(null));
                    Assert.Equal(1, ((IComparable)a).CompareTo(null));

                    // Make sure they don't work with objects of other types
                    Assert.False(a.Equals("0"));
                    Assert.False(a.Equals(0));
                    Assert.Throws<ArgumentException>(() => ((IComparable)a).CompareTo("0"));
                    Assert.Throws<ArgumentException>(() => ((IComparable)a).CompareTo(0));

                    // Test against itself
                    Assert.True(a.Equals(a));
                    Assert.True(((object)a).Equals(a));
                    Assert.Equal(0, a.CompareTo(a));
                    Assert.Equal(0, ((IComparable)a).CompareTo(a));

                    // Test against other versions
                    for (int j = 0; j < fixtures2.Length; j++)
                    {
                        b = fixtures2[j];

                        // Test Equals and CompareTo implementations
                        Assert.Equal(i.Equals(j), a.Equals(b));
                        Assert.Equal(i.Equals(j), ((object)a).Equals(b));
                        // As specified by IComparable, CompareTo doesn't necessarily return -1 or 1 on inequality
                        Assert.Equal(i.CompareTo(j), Math.Sign(a.CompareTo(b)));
                        Assert.Equal(i.CompareTo(j), Math.Sign(((IComparable)a).CompareTo(b)));
                        // Make sure the hash code is consistent
                        Assert.Equal(i == j, a.GetHashCode() == b.GetHashCode());

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
    }
}
