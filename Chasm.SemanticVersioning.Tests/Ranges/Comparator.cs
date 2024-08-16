using System;
using System.ComponentModel;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
using Xunit.Abstractions;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Chasm.SemanticVersioning.Tests
{
    public class ComparatorTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test null argument exceptions
            Assert.Throws<ArgumentNullException>(static () => new PrimitiveComparator(null!));
            Assert.Throws<ArgumentNullException>(static () => new XRangeComparator((PartialVersion)null!));
            Assert.Throws<ArgumentNullException>(static () => new XRangeComparator((PrimitiveComparator)null!));
            Assert.Throws<ArgumentNullException>(static () => new CaretComparator(null!));
            Assert.Throws<ArgumentNullException>(static () => new TildeComparator(null!));
            Assert.Throws<ArgumentNullException>(static () => new HyphenRangeComparator(null!, PartialVersion.OneStar));
            Assert.Throws<ArgumentNullException>(static () => new HyphenRangeComparator(PartialVersion.OneStar, null!));

            SemanticVersion version = new SemanticVersion(1, 2, 3);
            PartialVersion partial = new PartialVersion(4, 5, 'x');

            // test invalid PrimitiveOperator values
            for (int i = (int)PrimitiveOperator.LessThanOrEqual + 1; i < 255; i++)
            {
                PrimitiveOperator op = (PrimitiveOperator)i;
                InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => new PrimitiveComparator(version, op));
                Assert.Equal("operator", ex.ParamName);
                ex = Assert.Throws<InvalidEnumArgumentException>(() => new XRangeComparator(partial, op));
                Assert.Equal("operator", ex.ParamName);
            }
        }

        [Fact]
        public void ConstructorsPrimitive()
        {
            // test primitive comparator constructor
            SemanticVersion version = new SemanticVersion(4, 2, 0, ["beta", 7], ["DEV", "007--"]);

            foreach (PrimitiveOperator op in Enum.GetValues<PrimitiveOperator>())
            {
                PrimitiveComparator comparator = new PrimitiveComparator(version, op);
                Assert.Same(version, comparator.Operand);
                Assert.Equal(op, comparator.Operator);

                Assert.True(comparator.IsPrimitive);
                Assert.True(((Comparator)comparator).IsPrimitive);
                Assert.False(comparator.IsAdvanced);
                Assert.False(((Comparator)comparator).IsAdvanced);
            }

            // test X-Range comparator constructor
            PartialVersion partial = new PartialVersion('x', 5, 2, ["pre", 5], ["DEV", "04-"]);

            foreach (PrimitiveOperator op in Enum.GetValues<PrimitiveOperator>())
            {
                XRangeComparator comparator = new XRangeComparator(partial, op);
                Assert.Same(partial, comparator.Operand);
                Assert.Equal(op, comparator.Operator);

                Assert.False(comparator.IsPrimitive);
                Assert.False(((Comparator)comparator).IsPrimitive);
                Assert.True(comparator.IsAdvanced);
                Assert.True(((Comparator)comparator).IsAdvanced);
            }
        }

        [Fact]
        public void ConstructorsOther()
        {
            PartialVersion partial = new PartialVersion('x', 4, 0, ["beta", 27], ["BUILD", "96a--"]);

            // test Caret comparator constructor and formatting too I guess
            {
                CaretComparator comparator = new CaretComparator(partial);
                Assert.Same(partial, comparator.Operand);
                Assert.Equal("^x.4.0-beta.27+BUILD.96a--", comparator.ToString());

                Assert.False(comparator.IsPrimitive);
                Assert.False(((Comparator)comparator).IsPrimitive);
                Assert.True(comparator.IsAdvanced);
                Assert.True(((Comparator)comparator).IsAdvanced);
            }
            // test Tilde comparator constructor and formatting
            {
                TildeComparator comparator = new TildeComparator(partial);
                Assert.Same(partial, comparator.Operand);
                Assert.Equal("~x.4.0-beta.27+BUILD.96a--", comparator.ToString());

                Assert.False(comparator.IsPrimitive);
                Assert.False(((Comparator)comparator).IsPrimitive);
                Assert.True(comparator.IsAdvanced);
                Assert.True(((Comparator)comparator).IsAdvanced);
            }
            // test Hyphen Range comparator constructor and formatting
            {
                PartialVersion partial2 = new PartialVersion(4, 5, '*', ["alpha", 2], ["DEV"]);
                HyphenRangeComparator comparator = new HyphenRangeComparator(partial, partial2);

                Assert.Same(partial, comparator.Operand);
                Assert.Same(partial, ((AdvancedComparator)comparator).Operand);
                Assert.Same(partial, comparator.From);
                Assert.Same(partial2, comparator.To);
                Assert.Equal("x.4.0-beta.27+BUILD.96a-- - 4.5.*-alpha.2+DEV", comparator.ToString());

                Assert.False(comparator.IsPrimitive);
                Assert.False(((Comparator)comparator).IsPrimitive);
                Assert.True(comparator.IsAdvanced);
                Assert.True(((Comparator)comparator).IsAdvanced);
            }
        }

        [Fact]
        public void XRangeConversion()
        {
            SemanticVersion version = new SemanticVersion(1, 2, 3, ["alpha", 5], ["DEV", "07--"]);
            PartialVersion partial = new PartialVersion(1, 'x', '*', ["beta"], ["BUILD"]);

            // test PartialVersion to XRangeComparator conversion
            XRangeComparator xRange = partial;
            Assert.Equal(partial, xRange.Operand);
            Assert.Equal(PrimitiveOperator.ImplicitEqual, xRange.Operator);

            foreach (PrimitiveOperator op in Enum.GetValues<PrimitiveOperator>())
            {
                // test PrimitiveComparator to XRangeComparator conversion
                xRange = new PrimitiveComparator(version, op);
                Assert.Equal(version, xRange.Operand);
                Assert.Equal(op, xRange.Operator);

                // test XRangeComparator to PrimitiveComparator conversion
                PrimitiveComparator primitive = (PrimitiveComparator)new XRangeComparator(partial, op);
                Assert.Equal((SemanticVersion)partial, primitive.Operand);
                Assert.Equal(op, primitive.Operator);
            }

        }

    }
}
