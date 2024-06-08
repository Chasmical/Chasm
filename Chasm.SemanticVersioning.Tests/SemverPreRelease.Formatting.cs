using System;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests
    {
        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Formatting(FormattingFixture fixture)
        {
            Output.WriteLine($"Formatting {fixture}");
            SemverPreRelease preRelease = fixture.Source;

            // test ToString() methods
            fixture.Test(preRelease.ToString);
            fixture.Test(() => ((IFormattable)preRelease).ToString(null, null));

            // test TryFormat() methods
            fixture.Test(() => TestUtil.FormatWithTryFormat(preRelease.TryFormat));
            fixture.Test(() => TestUtil.FormatWithTryFormat(preRelease));
        }
    }
}
