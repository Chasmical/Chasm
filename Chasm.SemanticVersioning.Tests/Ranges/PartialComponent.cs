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
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal("0", PartialComponent.Zero.ToString());
            Assert.Equal("x", PartialComponent.LowerX.ToString());
            Assert.Equal("X", PartialComponent.UpperX.ToString());
            Assert.Equal("*", PartialComponent.Star.ToString());
            Assert.Equal("", PartialComponent.Omitted.ToString());
        }

        [Fact]
        public void Properties()
        {
            InvalidOperationException ex;

            // test numeric components' properties
            int[] numbers = [0, 1, 2, 3, 4, 8, 875, 5541, int.MaxValue];
            foreach (int number in numbers)
            {
                PartialComponent component = number;
                Assert.True(component.IsNumeric);
                Assert.False(component.IsWildcard);
                Assert.False(component.IsOmitted);

                Assert.Equal(number, component.AsNumber);
                ex = Assert.Throws<InvalidOperationException>(() => component.AsWildcard);
                Assert.Equal(Exceptions.ComponentNotWildcard, ex.Message);
            }

            // test wildcard components' properties
            foreach (char wildcard in "xX*")
            {
                PartialComponent component = wildcard;
                Assert.False(component.IsNumeric);
                Assert.True(component.IsWildcard);
                Assert.False(component.IsOmitted);

                Assert.Equal(wildcard, component.AsWildcard);
                ex = Assert.Throws<InvalidOperationException>(() => component.AsNumber);
                Assert.Equal(Exceptions.ComponentNotNumeric, ex.Message);
            }

            // test omitted component's properties
            PartialComponent omitted = PartialComponent.Omitted;
            Assert.False(omitted.IsNumeric);
            Assert.False(omitted.IsWildcard);
            Assert.True(omitted.IsOmitted);

            ex = Assert.Throws<InvalidOperationException>(() => omitted.AsNumber);
            Assert.Equal(Exceptions.ComponentNotNumeric, ex.Message);
            ex = Assert.Throws<InvalidOperationException>(() => omitted.AsWildcard);
            Assert.Equal(Exceptions.ComponentNotWildcard, ex.Message);
        }

    }
}
