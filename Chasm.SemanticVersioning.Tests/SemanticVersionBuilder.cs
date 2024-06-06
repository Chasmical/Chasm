using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Pure] public static TheoryData<string> CreateVersionFixtures()
            => SemanticVersionTests.CreateFormattingFixtures().Convert(static f => f.Source);

        [Fact]
        public void ConstructorsArguments()
        {
            // test constructors with negative version components
            int[] negativeNumbers = [-1, -42, -1204, int.MinValue];
            foreach (int num in negativeNumbers)
            {
                ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(num, 10, 20));
                Assert.StartsWith(Exceptions.MajorNegative, ex.Message);

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(0, num, 3));
                Assert.StartsWith(Exceptions.MinorNegative, ex.Message);

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersionBuilder(0, 5, num));
                Assert.StartsWith(Exceptions.PatchNegative, ex.Message);
            }

            // test constructors with invalid build metadata
            ArgumentException ex2 = Assert.Throws<ArgumentException>(static () => new SemanticVersionBuilder(1, 2, 3, [], ["$$$"]));
            Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex2.Message);

            ex2 = Assert.Throws<ArgumentException>(static () => new SemanticVersionBuilder(1, 2, 3, [], [""]));
            Assert.StartsWith(Exceptions.BuildMetadataEmpty, ex2.Message);

            ex2 = Assert.Throws<ArgumentException>(static () => new SemanticVersionBuilder(1, 2, 3, [], [null!]));
            Assert.StartsWith(Exceptions.BuildMetadataNull, ex2.Message);

            // test constructor with null
            Assert.Throws<ArgumentNullException>(static () => new SemanticVersionBuilder(null!));

        }

        [Theory, MemberData(nameof(CreateVersionFixtures))]
        public void Constructors(string source)
        {
            SemanticVersion v = SemanticVersion.Parse(source);

            // test properties
            SemanticVersionBuilder builder = new SemanticVersionBuilder(v);
            Assert.Equal(v.Major, builder.Major);
            Assert.Equal(v.Minor, builder.Minor);
            Assert.Equal(v.Patch, builder.Patch);
            Assert.Equal(v.PreReleases, builder.PreReleases);
            Assert.Equal(v.BuildMetadata, builder.BuildMetadata);

            // test other constructors and ToVersion
            Assert.Equal(v, new SemanticVersionBuilder(v).ToVersion());

            Assert.Equal(v, new SemanticVersionBuilder(v.Major, v.Minor, v.Patch, v.PreReleases, v.BuildMetadata).ToVersion());
            if (v.BuildMetadata.Count != 0) return;
            Assert.Equal(v, new SemanticVersionBuilder(v.Major, v.Minor, v.Patch, v.PreReleases).ToVersion());
            if (v.PreReleases.Count != 0) return;
            Assert.Equal(v, new SemanticVersionBuilder(v.Major, v.Minor, v.Patch).ToVersion());
        }

        [Theory, MemberData(nameof(CreateVersionFixtures))]
        public void Properties(string source)
        {
            SemanticVersion version = SemanticVersion.Parse(source);
            SemanticVersionBuilder builder = new SemanticVersionBuilder(version);

            // test properties
            Assert.Equal(version.Major, builder.Major);
            Assert.Equal(version.Minor, builder.Minor);
            Assert.Equal(version.Patch, builder.Patch);
            Assert.Equal(version.PreReleases, builder.PreReleases);
            Assert.Equal(version.BuildMetadata, builder.BuildMetadata);
        }

        [Fact]
        public void FluentBuilding()
        {
            SemanticVersionBuilder builder = new();

            // set components
            Assert.Same(builder, builder.WithMajor(123));
            Assert.Equal("123.0.0", builder.ToString());

            Assert.Same(builder, builder.WithMinor(456));
            Assert.Equal("123.456.0", builder.ToString());

            Assert.Same(builder, builder.WithPatch(789));
            Assert.Equal("123.456.789", builder.ToString());

            // append and remove pre-releases
            Assert.Same(builder, builder.AppendPreRelease("beta"));
            Assert.Equal("123.456.789-beta", builder.ToString());

            Assert.Same(builder, builder.AppendPreRelease(7));
            Assert.Equal("123.456.789-beta.7", builder.ToString());

            builder.PreReleases[1] = "dev";
            Assert.Equal("123.456.789-beta.dev", builder.ToString());

            builder.PreReleases.RemoveAt(1);
            Assert.Equal("123.456.789-beta", builder.ToString());

            builder.PreReleases.Add(12);
            Assert.Equal("123.456.789-beta.12", builder.ToString());

            // append and remove build metadata
            Assert.Same(builder, builder.AppendBuildMetadata("BUILD"));
            Assert.Equal("123.456.789-beta.12+BUILD", builder.ToString());

            Assert.Same(builder, builder.AppendBuildMetadata("07a"));
            Assert.Equal("123.456.789-beta.12+BUILD.07a", builder.ToString());

            builder.BuildMetadata[1] = "TEST-a";
            Assert.Equal("123.456.789-beta.12+BUILD.TEST-a", builder.ToString());

            builder.BuildMetadata.Remove("TEST-a");
            Assert.Equal("123.456.789-beta.12+BUILD", builder.ToString());

            builder.BuildMetadata.Add("-003-");
            Assert.Equal("123.456.789-beta.12+BUILD.-003-", builder.ToString());

            // set components through properties
            builder.Major = 56;
            Assert.Equal("56.456.789-beta.12+BUILD.-003-", builder.ToString());
            builder.Minor = 23;
            Assert.Equal("56.23.789-beta.12+BUILD.-003-", builder.ToString());
            builder.Patch = 791;
            Assert.Equal("56.23.791-beta.12+BUILD.-003-", builder.ToString());

            // clear pre-release and build metadata identifiers
            Assert.Same(builder, builder.ClearPreReleases());
            Assert.Equal("56.23.791+BUILD.-003-", builder.ToString());

            Assert.Same(builder, builder.ClearBuildMetadata());
            Assert.Equal("56.23.791", builder.ToString());

        }

        [Fact]
        public void FluentBuildingArguments()
        {
            SemanticVersion version = new SemanticVersion(1, 2, 3, ["dev", 7], ["BUILD", "-007-"]);
            SemanticVersionBuilder builder = new SemanticVersionBuilder(version);

            // test properties and methods with negative version components
            int[] negativeNumbers = [-1, -42, -1204, int.MinValue];
            foreach (int num in negativeNumbers)
            {
                ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.Major = num);
                Assert.StartsWith(Exceptions.MajorNegative, ex.Message);
                // ensure that after a failed operation the builder hasn't changed state
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.Minor = num);
                Assert.StartsWith(Exceptions.MinorNegative, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.Patch = num);
                Assert.StartsWith(Exceptions.PatchNegative, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithMajor(num));
                Assert.StartsWith(Exceptions.MajorNegative, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithMinor(num));
                Assert.StartsWith(Exceptions.MinorNegative, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithPatch(num));
                Assert.StartsWith(Exceptions.PatchNegative, ex.Message);
                Assert.Equal(version, builder.ToVersion());
            }

            // try adding null build metadata identifier
            Assert.Throws<ArgumentNullException>(() => builder.AppendBuildMetadata(null!));
            Assert.Throws<ArgumentNullException>(() => builder.BuildMetadata.Add(null!));
            Assert.Throws<ArgumentNullException>(() => builder.BuildMetadata[1] = null!);

            // try adding invalid build metadata identifier
            {
                ArgumentException ex = Assert.Throws<ArgumentException>(() => builder.AppendBuildMetadata("beta#2"));
                Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentException>(() => builder.BuildMetadata.Add("beta#2"));
                Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex.Message);
                Assert.Equal(version, builder.ToVersion());

                ex = Assert.Throws<ArgumentException>(() => builder.BuildMetadata[1] = "beta#2");
                Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex.Message);
                Assert.Equal(version, builder.ToVersion());
            }

            // test Increment with an invalid increment type
            Assert.Throws<InvalidEnumArgumentException>(() => builder.Increment((IncrementType)9));
            Assert.Equal(version, builder.ToVersion());

        }

    }
}
