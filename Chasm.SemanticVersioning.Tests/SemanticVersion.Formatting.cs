using System;
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
            string? format = fixture.Format;

            // test ToString() methods
            if (format is null) fixture.Test(version.ToString);
            fixture.Test(() => version.ToString(format));
            fixture.Test(() => ((IFormattable)version).ToString(format, null));

            // test TryFormat() methods
            if (format is null) fixture.Test(() => TestUtil.FormatWithTryFormat(version.TryFormat));
            fixture.Test(() => TestUtil.FormatWithTryFormat(version, format));

            // make sure that "M.m.p-rr+dd" is the default format
            if (format is null)
            {
                FormattingFixture fixture2 = new(fixture.Source, "M.m.p-rr+dd");
                fixture2.Returns(fixture.Source);
                Formatting(fixture2);
            }
        }
    }
}
