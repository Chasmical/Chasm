using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialComponentTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test constructors with negative version components
            int[] negativeNumbers = [-1, -42, -1204, int.MinValue];
            foreach (int number in negativeNumbers)
            {
                ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => new PartialComponent(number));
                Assert.StartsWith(Exceptions.ComponentNegative, ex.Message);
            }

            // test constructors with valid and invalid ASCII characters
            for (char character = (char)0; character < (char)128; character++)
            {
                if (character is 'x' or 'X' or '*' or >= '0' and <= '9')
                {
                    Assert.Equal(character.ToString(), new PartialComponent(character).ToString());
                    continue;
                }
                ArgumentException ex = Assert.Throws<ArgumentException>(() => new PartialComponent(character));
                Assert.StartsWith(Exceptions.ComponentInvalid, ex.Message);
            }

        }

        [Fact]
        public void ConversionOperators()
        {
            // test PartialComponent to int conversion
            Assert.Equal(123, (int)(PartialComponent)123);
            Assert.Equal(0, (int)(PartialComponent)'x');
            Assert.Equal(0, (int)(PartialComponent)'X');
            Assert.Equal(0, (int)(PartialComponent)'*');
            Assert.Equal(0, (int)(PartialComponent)null);

            // test PartialComponent to int? conversion
            Assert.Equal(123, (int?)(PartialComponent)123);
            Assert.Null((int?)(PartialComponent)'x');
            Assert.Null((int?)(PartialComponent)'X');
            Assert.Null((int?)(PartialComponent)'*');
            Assert.Null((int?)(PartialComponent)null);

            // test PartialComponent to char conversion
            foreach (char character in "0123456789xX*")
                Assert.Equal(character, (char)(PartialComponent)character);

            // test components that can't be represented as single characters
            ArgumentException ex = Assert.Throws<ArgumentException>(static () => (char)(PartialComponent)123);
            Assert.StartsWith(Exceptions.ComponentNotSingleChar, ex.Message);
            ex = Assert.Throws<ArgumentException>(static () => (char)(PartialComponent)null);
            Assert.StartsWith(Exceptions.ComponentNotSingleChar, ex.Message);

        }

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal(0, PartialComponent.Zero);
            Assert.Equal('x', PartialComponent.LowerX);
            Assert.Equal('X', PartialComponent.UpperX);
            Assert.Equal('*', PartialComponent.Star);
            Assert.Equal((int?)null, PartialComponent.Omitted);
        }

        [Fact]
        public void Properties()
        {
            InvalidOperationException ex;

            // test numeric components' properties and formatting
            int[] numbers = [0, 1, 2, 3, 4, 8, 875, 5541, int.MaxValue];
            foreach (int number in numbers)
            {
                PartialComponent component = number;
                Assert.True(component.IsNumeric);
                Assert.False(component.IsWildcard);
                Assert.False(component.IsOmitted);

                Assert.Equal(number, component.AsNumber);
                Assert.Equal(number.ToString(), component.ToString());

                ex = Assert.Throws<InvalidOperationException>(() => component.AsWildcard);
                Assert.Equal(Exceptions.ComponentNotWildcard, ex.Message);
            }

            // test wildcard components' properties and formatting
            foreach (char wildcard in "xX*")
            {
                PartialComponent component = wildcard;
                Assert.False(component.IsNumeric);
                Assert.True(component.IsWildcard);
                Assert.False(component.IsOmitted);

                Assert.Equal(wildcard, component.AsWildcard);
                Assert.Equal(wildcard.ToString(), component.ToString());

                ex = Assert.Throws<InvalidOperationException>(() => component.AsNumber);
                Assert.Equal(Exceptions.ComponentNotNumeric, ex.Message);
            }

            // test omitted component's properties and formatting
            PartialComponent omitted = PartialComponent.Omitted;
            Assert.False(omitted.IsNumeric);
            Assert.False(omitted.IsWildcard);
            Assert.True(omitted.IsOmitted);

            Assert.Equal("", omitted.ToString());

            ex = Assert.Throws<InvalidOperationException>(() => omitted.AsNumber);
            Assert.Equal(Exceptions.ComponentNotNumeric, ex.Message);
            ex = Assert.Throws<InvalidOperationException>(() => omitted.AsWildcard);
            Assert.Equal(Exceptions.ComponentNotWildcard, ex.Message);
        }

    }
}
