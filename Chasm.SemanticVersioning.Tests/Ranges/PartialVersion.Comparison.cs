﻿using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialVersionTests
    {
        [Fact]
        public void Comparison()
        {
            PartialVersion[][] fixtures1 = CreateComparisonFixtures();
            PartialVersion[][] fixtures2 = CreateComparisonFixtures();
            PartialVersion? a = default, b = default;

            try
            {
                for (int i = 0; i < fixtures1.Length; i++)
                {
                    PartialVersion[] aRow = fixtures1[i];
                    for (int k = 0; k < aRow.Length; k++)
                    {
                        a = aRow[k];

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

                        for (int j = 0; j < fixtures2.Length; j++)
                        {
                            PartialVersion[] bRow = fixtures2[j];
                            for (int l = 0; l < bRow.Length; l++)
                            {
                                b = bRow[l];

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
                                // Assert.Equal(i > j, a > b);
                                // Assert.Equal(i < j, a < b);
                                // Assert.Equal(i >= j, a >= b);
                                // Assert.Equal(i <= j, a <= b);

                            }
                        }
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
