using Xunit;
using Xunit.Abstractions;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionBuilderTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

    }
}
