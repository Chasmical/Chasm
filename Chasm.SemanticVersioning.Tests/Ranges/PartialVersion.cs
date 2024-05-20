using System;
using Chasm.SemanticVersioning.Ranges;
using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class PartialVersionTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

        [Fact]
        public void ConstructorsArguments()
        {
            // test constructors with invalid build metadata
            ArgumentException ex = Assert.Throws<ArgumentException>(static () => new PartialVersion(1, 2, 3, [], ["$$$"]));
            Assert.StartsWith(Exceptions.BuildMetadataInvalid, ex.Message);
            ex = Assert.Throws<ArgumentException>(static () => new PartialVersion(1, 2, 3, [], [""]));
            Assert.StartsWith(Exceptions.BuildMetadataEmpty, ex.Message);
            ex = Assert.Throws<ArgumentException>(static () => new PartialVersion(1, 2, 3, [], [null!]));
            Assert.StartsWith(Exceptions.BuildMetadataNull, ex.Message);

            // test constructors with incorrectly placed omitted components
            PartialComponent o = PartialComponent.Omitted;
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(1, o, 3));
            Assert.StartsWith(Exceptions.MinorOmitted, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(o, 2, 3));
            Assert.StartsWith(Exceptions.MajorOmitted, ex.Message);

            // test identifiers after omitted components
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(1, 2, o, [0]));
            Assert.StartsWith(Exceptions.PreReleaseAfterOmitted, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(1, 2, o, [], ["007"]));
            Assert.StartsWith(Exceptions.BuildMetadataAfterOmitted, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(1, o, o, [0]));
            Assert.StartsWith(Exceptions.PreReleaseAfterOmitted, ex.Message);
            ex = Assert.Throws<ArgumentException>(() => new PartialVersion(1, o, o, [], ["007"]));
            Assert.StartsWith(Exceptions.BuildMetadataAfterOmitted, ex.Message);

        }

        [Fact]
        public void ConversionOperators()
        {
            // make sure nulls are handled properly
            Assert.Null((Version)(PartialVersion)null!);
            Assert.Null((PartialVersion)(Version)null!);
            Assert.Throws<ArgumentNullException>(static () => new PartialVersion((Version)null!));
            Assert.Null((SemanticVersion)(PartialVersion)null!);
            Assert.Null((PartialVersion)(SemanticVersion)null!);
            Assert.Throws<ArgumentNullException>(static () => new PartialVersion((SemanticVersion)null!));

            // test PartialVersion to Version conversion
            Assert.Equal(
                new Version(1, 2, 3),
                (Version)new PartialVersion(1, 2, 3)
            );
            Assert.Equal(
                new Version(1, 2, 3),
                (Version)new PartialVersion(1, 2, 3, ["pre", 8], ["build"])
            );
            Assert.Equal(
                new Version(1, 2),
                (Version)new PartialVersion(1, 2)
            );

            // test PartialVersion to SemanticVersion conversion
            Assert.Equal(
                new SemanticVersion(1, 2, 3, ["pre", 5], ["build"]),
                (SemanticVersion)new PartialVersion(1, 2, 3, ["pre", 5], ["build"])
            );
            Assert.Equal(
                new SemanticVersion(0, 2, 0, ["pre", 5], ["build"]),
                (SemanticVersion)new PartialVersion('X', 2, '*', ["pre", 5], ["build"])
            );
            Assert.Equal(
                new SemanticVersion(3, 0, 0),
                (SemanticVersion)new PartialVersion(3, null, null)
            );
        }

        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Constructors(FormattingFixture fixture)
        {
            PartialVersion v = PartialVersion.Parse(fixture.Source);

            // test constructor with a SemanticVersion parameter
            if (!v.IsPartial)
            {
                SemanticVersion semver = new SemanticVersion((int)v.Major, (int)v.Minor, (int)v.Patch, v.PreReleases, v.BuildMetadata);
                Assert.Equal(v, new PartialVersion(semver));
                Assert.Equal(v, semver);
            }
            // test constructor with a Version parameter
            if (v.Major.IsNumeric && v.Minor.IsNumeric && !v.IsPreRelease && !v.HasBuildMetadata && !v.Patch.IsWildcard)
            {
                Version systemVersion = v.Patch.IsOmitted
                    ? new Version((int)v.Major, (int)v.Minor)
                    : new Version((int)v.Major, (int)v.Minor, (int)v.Patch);
                Assert.Equal(v, new PartialVersion(systemVersion));
                Assert.Equal(v, (PartialVersion)systemVersion);
            }

            // make sure that constructors result in equal instances
            Assert.Equal(v, new PartialVersion(v.Major, v.Minor, v.Patch, v.PreReleases, v.BuildMetadata));
            if (v.BuildMetadata.Count != 0) return;
            Assert.Equal(v, new PartialVersion(v.Major, v.Minor, v.Patch, v.PreReleases));
            if (v.PreReleases.Count != 0) return;
            Assert.Equal(v, new PartialVersion(v.Major, v.Minor, v.Patch));
            if (!v.Patch.IsOmitted) return;
            Assert.Equal(v, new PartialVersion(v.Major, v.Minor));
            if (!v.Minor.IsOmitted) return;
            Assert.Equal(v, new PartialVersion(v.Major));
        }

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal(new PartialVersion('x'), PartialVersion.OneStar);

            // make sure that properties aren't constantly creating new instances
            Assert.Same(PartialVersion.OneStar, PartialVersion.OneStar);
        }

        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Properties(FormattingFixture fixture)
        {
            PartialVersion version = PartialVersion.Parse(fixture.Source);

            // test identifier collections
            Assert.Equal(version._preReleases, version.PreReleases);
            Assert.Equal(version._buildMetadata, version.BuildMetadata);
            Assert.Equal(version._preReleases, version.GetPreReleases().ToArray());
            Assert.Equal(version._buildMetadata, version.GetBuildMetadata().ToArray());

            // make sure that properties memoize the read-only collections
            Assert.Same(version.PreReleases, version.PreReleases);
            Assert.Same(version.BuildMetadata, version.BuildMetadata);

            // test boolean properties
            Assert.Equal(!version.Major.IsNumeric || !version.Minor.IsNumeric || !version.Patch.IsNumeric, version.IsPartial);
            Assert.Equal(version._preReleases.Length > 0, version.IsPreRelease);
            Assert.Equal(version._buildMetadata.Length > 0, version.HasBuildMetadata);
        }

    }
}
