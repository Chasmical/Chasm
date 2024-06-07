using System;
using Xunit;
// ReSharper disable LambdaExpressionCanBeMadeStatic

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests
    {
        public static FixtureAdapter<IncrementingFixture> CreateIncrementingFixtures()
        {
            FixtureAdapter<IncrementingFixture> adapter = [];

            IncrementingFixture New(string source)
                => adapter.Add(new IncrementingFixture(source));

            const int max = int.MaxValue;



            // IncrementPatch tests
            New("0.0.0-0").After(IncrementType.Patch).Returns("0.0.0");
            New("0.0.0").After(IncrementType.Patch).Returns("0.0.1");
            New("1.2.0-0").After(IncrementType.Patch).Returns("1.2.0");
            New("1.2.3-0").After(IncrementType.Patch).Returns("1.2.3");
            New("1.2.3").After(IncrementType.Patch).Returns("1.2.4");

            // IncrementPatch overflow
            New($"1.2.{max}-0").After(IncrementType.Patch).Returns($"1.2.{max}");
            New($"1.2.{max}").After(IncrementType.Patch).Throws(Exceptions.PatchTooBig);
            New($"{max}.{max}.{max}-0").After(IncrementType.Patch).Returns($"{max}.{max}.{max}");
            New($"{max}.{max}.{max}").After(IncrementType.Patch).Throws(Exceptions.PatchTooBig);

            // IncrementMinor tests
            New("0.0.0-0").After(IncrementType.Minor).Returns("0.0.0");
            New("0.0.0").After(IncrementType.Minor).Returns("0.1.0");
            New("1.2.0-0").After(IncrementType.Minor).Returns("1.2.0");
            New("1.2.3-0").After(IncrementType.Minor).Returns("1.3.0");
            New("1.2.3").After(IncrementType.Minor).Returns("1.3.0");

            // IncrementMinor overflow
            New($"{max}.{max}.0-0").After(IncrementType.Minor).Returns($"{max}.{max}.0");
            New($"{max}.{max}.0").After(IncrementType.Minor).Throws(Exceptions.MinorTooBig);
            New($"{max}.{max - 1}.{max}").After(IncrementType.Minor).Returns($"{max}.{max}.0");
            New($"{max}.{max}.{max}").After(IncrementType.Minor).Throws(Exceptions.MinorTooBig);

            // IncrementMajor tests
            New("0.0.0-0").After(IncrementType.Major).Returns("0.0.0");
            New("0.0.0").After(IncrementType.Major).Returns("1.0.0");
            New("1.0.0-0").After(IncrementType.Major).Returns("1.0.0");
            New("1.2.3-0").After(IncrementType.Major).Returns("2.0.0");
            New("1.2.3").After(IncrementType.Major).Returns("2.0.0");

            // IncrementMajor overflow
            New($"{max - 1}.0.0-0").After(IncrementType.Major).Returns($"{max - 1}.0.0");
            New($"{max}.0.0").After(IncrementType.Major).Throws(Exceptions.MajorTooBig);
            New($"{max - 1}.{max}.{max}").After(IncrementType.Major).Returns($"{max}.0.0");
            New($"{max}.{max}.{max}").After(IncrementType.Major).Throws(Exceptions.MajorTooBig);



            // IncrementPreRelease tests
            New("1.2.2").After(IncrementType.PreRelease).Returns("1.2.3-0");
            New("1.2.3-0").After(IncrementType.PreRelease).Returns("1.2.3-1");
            New("1.2.3-9").After(IncrementType.PreRelease).Returns("1.2.3-10");
            New("1.2.3-rc").After(IncrementType.PreRelease).Returns("1.2.3-rc.0");
            New("1.2.3-rc.0").After(IncrementType.PreRelease).Returns("1.2.3-rc.1");
            New("1.2.3-7.-8.9").After(IncrementType.PreRelease).Returns("1.2.3-7.-8.10");
            New("1.2.3-7.-8.9.beta").After(IncrementType.PreRelease).Returns("1.2.3-7.-8.10.beta");

            // IncrementPreRelease overflow
            New($"1.2.3-rc.{max - 1}").After(IncrementType.PreRelease).Returns($"1.2.3-rc.{max}");
            New($"1.2.3-rc.{max}").After(IncrementType.PreRelease).Throws(Exceptions.PreReleaseTooBig);
            New($"1.2.3-{max}.rc.{max - 1}").After(IncrementType.PreRelease).Returns($"1.2.3-{max}.rc.{max}");
            New($"1.2.3-{max}.rc.{max}").After(IncrementType.PreRelease).Throws(Exceptions.PreReleaseTooBig);
            New($"1.2.{max - 1}").After(IncrementType.PreRelease).Returns($"1.2.{max}-0");
            New($"1.2.{max}").After(IncrementType.PreRelease).Throws(Exceptions.PatchTooBig);



            // IncrementPreRelease with pre-release
            New("1.2.2").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-0").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-beta.gamma").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-beta.gamma.4").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-beta.0").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.1");
            New("1.2.3-beta.1.beta").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.2.beta");
            New("1.2.3-alpha.2").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-rc.3").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");

            // IncrementPreRelease with and without a pre-release
            New("1.2.3-beta.gamma.5").After(IncrementType.PreRelease).Returns("1.2.3-beta.gamma.6");
            New("1.2.3-beta.gamma.5").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-beta.5.gamma").After(IncrementType.PreRelease).Returns("1.2.3-beta.6.gamma");
            New("1.2.3-beta.5.gamma").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.6.gamma");
            New("1.2.3-beta.gamma").After(IncrementType.PreRelease).Returns("1.2.3-beta.gamma.0");
            New("1.2.3-beta.gamma").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New("1.2.3-beta").After(IncrementType.PreRelease).Returns("1.2.3-beta.0");
            New("1.2.3-beta").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");

            // IncrementPreRelease with pre-release overflow
            New($"1.2.3-alpha.{max}").After(IncrementType.PreRelease, "beta").Returns("1.2.3-beta.0");
            New($"1.2.3-alpha.{max}.0.beta").After(IncrementType.PreRelease, "alpha").Returns($"1.2.3-alpha.{max}.1.beta");
            New($"1.2.3-alpha.{max}.0.beta.{max - 1}").After(IncrementType.PreRelease, "alpha").Returns($"1.2.3-alpha.{max}.0.beta.{max}");
            New($"1.2.3-alpha.{max}.0.beta.{max}").After(IncrementType.PreRelease, "alpha").Throws(Exceptions.PreReleaseTooBig);



            // IncrementPrePatch tests
            New("1.2.2").After(IncrementType.PrePatch).Returns("1.2.3-0");
            New("1.2.2-0").After(IncrementType.PrePatch).Returns("1.2.3-0");
            New("1.2.2-dev.0").After(IncrementType.PrePatch).Returns("1.2.3-0");
            New("1.2.2").After(IncrementType.PrePatch, "dev").Returns("1.2.3-dev.0");
            New("1.2.2-0").After(IncrementType.PrePatch, "dev").Returns("1.2.3-dev.0");
            New("1.2.2-dev.0").After(IncrementType.PrePatch, "dev").Returns("1.2.3-dev.0");

            // IncrementPrePatch overflow
            New($"1.2.{max}-dev.{max}").After(IncrementType.PrePatch).Throws(Exceptions.PatchTooBig);
            New($"1.2.{max}-dev.{max}").After(IncrementType.PrePatch, "dev").Throws(Exceptions.PatchTooBig);

            // IncrementPreMinor tests
            New("1.2.0").After(IncrementType.PreMinor).Returns("1.3.0-0");
            New("1.2.0-0").After(IncrementType.PreMinor).Returns("1.3.0-0");
            New("1.2.0-dev.0").After(IncrementType.PreMinor).Returns("1.3.0-0");
            New("1.2.0").After(IncrementType.PreMinor, "dev").Returns("1.3.0-dev.0");
            New("1.2.0-0").After(IncrementType.PreMinor, "dev").Returns("1.3.0-dev.0");
            New("1.2.0-dev.0").After(IncrementType.PreMinor, "dev").Returns("1.3.0-dev.0");

            // IncrementPreMinor overflow
            New($"1.{max}.3-dev.{max}").After(IncrementType.PreMinor).Throws(Exceptions.MinorTooBig);
            New($"1.{max}.3-dev.{max}").After(IncrementType.PreMinor, "dev").Throws(Exceptions.MinorTooBig);

            // IncrementPreMajor tests
            New("1.0.0").After(IncrementType.PreMajor).Returns("2.0.0-0");
            New("1.0.0-0").After(IncrementType.PreMajor).Returns("2.0.0-0");
            New("1.0.0-dev.0").After(IncrementType.PreMajor).Returns("2.0.0-0");
            New("1.0.0").After(IncrementType.PreMajor, "dev").Returns("2.0.0-dev.0");
            New("1.0.0-0").After(IncrementType.PreMajor, "dev").Returns("2.0.0-dev.0");
            New("1.0.0-dev.0").After(IncrementType.PreMajor, "dev").Returns("2.0.0-dev.0");

            // IncrementPreMajor overflow
            New($"{max}.2.3-dev.{max}").After(IncrementType.PreMajor).Throws(Exceptions.MajorTooBig);
            New($"{max}.2.3-dev.{max}").After(IncrementType.PreMajor, "dev").Throws(Exceptions.MajorTooBig);



            return adapter;
        }

        public sealed class IncrementingFixture(string source) : FuncFixture<SemanticVersionBuilder>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public IncrementingFixture() : this(null!) { }

            public string Source { get; } = source;
            public IncrementType Type { get; private set; }
            private string _preRelease = null!;
            public SemverPreRelease PreRelease => _preRelease;
            public string? Expected { get; private set; }

            protected override Type DefaultExceptionType => typeof(InvalidOperationException);

            public override void AssertResult(SemanticVersionBuilder? result)
            {
                // We need an exact match, that is, including build metadata
                Assert.Equal(Expected, result?.ToString());
            }

            public Stage2 After(IncrementType type, SemverPreRelease preRelease = default)
            {
                Type = type;
                _preRelease = (string)preRelease;
                return new Stage2(this);
            }
            private void Returns(string expected)
            {
                MarkAsComplete();
                Expected = expected;
            }
            public override string ToString()
                => $"{base.ToString()} {Source} → ({Type}) → {Expected}";

            public sealed class Stage2(IncrementingFixture fixture)
            {
                public void Returns(string expected) => fixture.Returns(expected);
                public void Throws(string exceptionMessage) => fixture.Throws(exceptionMessage);
            }
        }
    }
}
