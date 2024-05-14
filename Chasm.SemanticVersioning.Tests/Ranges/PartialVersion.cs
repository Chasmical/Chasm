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

        // TODO: test constructors with CreateFormattingFixtures()

        [Fact]
        public void StaticProperties()
        {
            // make sure that properties return correct values
            Assert.Equal(new PartialVersion('x'), PartialVersion.OneStar);

            // make sure that properties aren't constantly creating new instances
            Assert.Same(PartialVersion.OneStar, PartialVersion.OneStar);
        }

        // TODO: test properties with CreateFormattingFixtures()

    }
}
