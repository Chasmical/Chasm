using System;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test constructors with negative version components
            int[] negativeNumbers = [-1, -42, -1204, int.MinValue];
            foreach (int num in negativeNumbers)
            {
                ArgumentOutOfRangeException e = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(num, 10, 20));
                Assert.StartsWith(Exceptions.MajorNegative, e.Message);

                e = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, num, 3));
                Assert.StartsWith(Exceptions.MinorNegative, e.Message);

                e = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, 5, num));
                Assert.StartsWith(Exceptions.PatchNegative, e.Message);
            }

            // test constructors with invalid build metadata
            ArgumentException e2 = Assert.Throws<ArgumentException>(static () => new SemanticVersion(1, 2, 3, [], ["$$$"]));
            Assert.StartsWith(Exceptions.BuildMetadataInvalid, e2.Message);

            e2 = Assert.Throws<ArgumentException>(static () => new SemanticVersion(1, 2, 3, [], [""]));
            Assert.StartsWith(Exceptions.BuildMetadataEmpty, e2.Message);

            e2 = Assert.Throws<ArgumentException>(static () => new SemanticVersion(1, 2, 3, [], [null!]));
            Assert.StartsWith(Exceptions.BuildMetadataNull, e2.Message);
        }

        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Constructors(FormattingFixture fixture)
        {
            SemanticVersion v = SemanticVersion.Parse(fixture.Source);

            // make sure that constructors result in equal instances
            Assert.Equal(v, new SemanticVersion(v.Major, v.Minor, v.Patch, v.PreReleases, v.BuildMetadata));
            if (v.BuildMetadata.Count == 0)
                Assert.Equal(v, new SemanticVersion(v.Major, v.Minor, v.Patch, v.PreReleases));
            if (v.PreReleases.Count == 0 && v.BuildMetadata.Count == 0)
                Assert.Equal(v, new SemanticVersion(v.Major, v.Minor, v.Patch));
        }

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal(new SemanticVersion(0, 0, 0, [0]), SemanticVersion.MinValue);
            Assert.Equal(new SemanticVersion(int.MaxValue, int.MaxValue, int.MaxValue), SemanticVersion.MaxValue);

            // make sure that properties aren't constantly creating new instances
            Assert.Same(SemanticVersion.MinValue, SemanticVersion.MinValue);
            Assert.Same(SemanticVersion.MaxValue, SemanticVersion.MaxValue);
        }

        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Properties(FormattingFixture fixture)
        {
            SemanticVersion version = SemanticVersion.Parse(fixture.Source);

            // test identifier collections
            Assert.Equal(version._preReleases, version.PreReleases);
            Assert.Equal(version._buildMetadata, version.BuildMetadata);
            Assert.Equal(version._preReleases, version.GetPreReleases().ToArray());
            Assert.Equal(version._buildMetadata, version.GetBuildMetadata().ToArray());

            // make sure that properties memoize the read-only collections
            Assert.Same(version.PreReleases, version.PreReleases);
            Assert.Same(version.BuildMetadata, version.BuildMetadata);

            // test boolean properties
            Assert.Equal(version.Major > 0 && version._preReleases.Length == 0, version.IsStable);
            Assert.Equal(version._preReleases.Length > 0, version.IsPreRelease);
            Assert.Equal(version._buildMetadata.Length > 0, version.HasBuildMetadata);
        }
    }
}
