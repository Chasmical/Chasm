using System.Runtime.CompilerServices;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        [Theory, MemberData(nameof(CreateIncrementingFixtures))]
        public void Incrementing(IncrementingFixture fixture)
        {
            SemanticVersion version = SemanticVersion.Parse(fixture.Source);

            // test None increment type
            Assert.Equal(version, new SemanticVersionBuilder(version).Increment(IncrementType.None).ToVersion());

            // test the overload with a pre-release
            fixture.Test(() =>
            {
                SemanticVersionBuilder builder = new(version);
                try
                {
                    return builder.Increment(fixture.Type, fixture.PreRelease);
                }
                catch
                {
                    // ensure that after a failed operation the builder hasn't changed state
                    Assert.Equal(version, builder.ToVersion());
                    throw;
                }
            });

            if (fixture.PreRelease == SemverPreRelease.Zero)
            {
                // test the overload without a pre-release
                fixture.Test(() =>
                {
                    SemanticVersionBuilder builder = new(version);
                    try
                    {
                        return builder.Increment(fixture.Type);
                    }
                    catch
                    {
                        // ensure that after a failed operation the builder hasn't changed state
                        Assert.Equal(version, builder.ToVersion());
                        throw;
                    }
                });

                // test the overloads without a pre-release, to cover IncrementPre[Component] methods
                fixture.Test(() =>
                {
                    SemanticVersionBuilder builder = new(version);
                    try
                    {
                        return fixture.Type switch
                        {
                            IncrementType.Major => builder.IncrementMajor(),
                            IncrementType.Minor => builder.IncrementMinor(),
                            IncrementType.Patch => builder.IncrementPatch(),
                            IncrementType.PrePatch => builder.IncrementPrePatch(),
                            IncrementType.PreMinor => builder.IncrementPreMinor(),
                            IncrementType.PreMajor => builder.IncrementPreMajor(),
                            IncrementType.PreRelease => builder.IncrementPreRelease(),
                            _ => throw new SwitchExpressionException(),
                        };
                    }
                    catch
                    {
                        // ensure that after a failed operation the builder hasn't changed state
                        Assert.Equal(version, builder.ToVersion());
                        throw;
                    }
                });
            }

        }

    }
}
