using System;
using Chasm.SemanticVersioning.Ranges;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class VersionRangeTests
    {
        [Pure] public static FixtureAdapter<DesugaringFixture> CreateDesugaringFixtures()
        {
            FixtureAdapter<DesugaringFixture> adapter = [];

            DesugaringFixture New(string source)
                => adapter.Add(new DesugaringFixture(source));



            // Examples from node-semver README

            New("1.2.3 - 2.3.4").DesugarsTo(">=1.2.3 <=2.3.4");
            New("1.2 - 2.3.4").DesugarsTo(">=1.2.0 <=2.3.4");
            New("1.2.3 - 2.3").DesugarsTo(">=1.2.3 <2.4.0-0");
            New("1.2.3 - 2").DesugarsTo(">=1.2.3 <3.0.0-0");

            New("*").DesugarsTo(">=0.0.0");
            New("1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");

            New("1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.x.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("1.2.x").DesugarsTo(">=1.2.0 <1.3.0-0");

            New("~1.2.3").DesugarsTo(">=1.2.3 <1.3.0-0");
            New("~1.2").DesugarsTo(">=1.2.0 <1.3.0-0");
            New("~1").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("~0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("~0.2").DesugarsTo(">=0.2.0 <0.3.0-0");
            New("~0").DesugarsTo(">=0.0.0 <1.0.0-0");
            New("~1.2.3-beta.2").DesugarsTo(">=1.2.3-beta.2 <1.3.0-0");

            New("^1.2.3").DesugarsTo(">=1.2.3 <2.0.0-0");
            New("^0.2.3").DesugarsTo(">=0.2.3 <0.3.0-0");
            New("^0.0.3").DesugarsTo(">=0.0.3 <0.0.4-0");
            New("^1.2.3-beta.2").DesugarsTo(">=1.2.3-beta.2 <2.0.0-0");
            New("^0.0.3-beta").DesugarsTo(">=0.0.3-beta <0.0.4-0");

            New("^1.2.x").DesugarsTo(">=1.2.0 <2.0.0-0");
            New("^0.0.x").DesugarsTo(">=0.0.0 <0.1.0-0");
            New("^0.0").DesugarsTo(">=0.0.0 <0.1.0-0");
            New("^1.x").DesugarsTo(">=1.0.0 <2.0.0-0");
            New("^0.x").DesugarsTo(">=0.0.0 <1.0.0-0");



            return adapter;
        }

        public class DesugaringFixture(string source) : FuncFixture<VersionRange>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public DesugaringFixture() : this(null!) { }

            public string Source { get; } = source;
            public string? Expected { get; private set; }

            public void DesugarsTo(string expectedRange)
            {
                MarkAsComplete();
                Expected = expectedRange;
            }

            public override void AssertResult(VersionRange? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Expected, result.ToString());
            }

            public override string ToString() => $"{base.ToString()} \"{Source}\"";
        }
    }
}
