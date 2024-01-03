using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Theory, MemberData(nameof(CreateFormattingFixtures))]
        public void Formatting(FormattingFixture fixture)
        {
            Output.WriteLine($"Formatting {fixture}");
            SemanticVersion version = SemanticVersion.Parse(fixture.Source);

            // test ToString() methods
            fixture.Test(version.ToString);
            //fixture.Test(() => ((IFormattable)version).ToString(null, null));

            // test TryFormat() methods
            //fixture.Test(() => TestUtil.FormatWithTryFormat(version.TryFormat));
            //fixture.Test(() => TestUtil.FormatWithTryFormat(version));
        }
    }
}
