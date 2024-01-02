using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;
    }
}
