using System;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test constructors with negative numbers
            int[] negativeNumbers = [-1, -42, -1204, int.MinValue];
            foreach (int number in negativeNumbers)
            {
                ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemverPreRelease(number));
                Assert.StartsWith(Exceptions.PreReleaseNegative, ex.Message);
            }

            // test constructors with null and empty values
            Assert.Throws<ArgumentNullException>(static () => new SemverPreRelease(null!));

            ArgumentException ex2 = Assert.Throws<ArgumentException>(static () => new SemverPreRelease(""));
            Assert.StartsWith(Exceptions.PreReleaseEmpty, ex2.Message);

            ex2 = Assert.Throws<ArgumentException>(static () => new SemverPreRelease("".AsSpan()));
            Assert.StartsWith(Exceptions.PreReleaseEmpty, ex2.Message);

        }

        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Conversions(FormattingFixture fixture)
        {
            SemverPreRelease preRelease = SemverPreRelease.Parse(fixture.Source);
            string text = preRelease.ToString();

            // test conversions of a pre-release to a string
            Assert.Equal(text, (string)preRelease);
            Assert.Equal(text, ((ReadOnlySpan<char>)preRelease).ToString());

            // test conversions of a string to a pre-release
            Assert.Equal(preRelease, (SemverPreRelease)text);
            Assert.Equal(preRelease, text.AsSpan());
            Assert.Equal(preRelease, new SemverPreRelease(text));
            Assert.Equal(preRelease, new SemverPreRelease(text.AsSpan()));

            // test conversion of an integer to a pre-release and vice versa
            if (preRelease.IsNumeric)
            {
                int number = int.Parse(text);
                Assert.Equal(number, (int)preRelease);
                Assert.Equal(number, preRelease.AsNumber);
                Assert.Equal(preRelease, new SemverPreRelease(number));
            }
        }

        [Fact]
        public void StaticProperties()
        {
            // make sure the property returns the correct value
            Assert.Equal(new SemverPreRelease(0), SemverPreRelease.Zero);
        }

    }
}
