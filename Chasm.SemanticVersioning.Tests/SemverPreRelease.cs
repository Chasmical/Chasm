using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemverPreReleaseTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;
    }
}
