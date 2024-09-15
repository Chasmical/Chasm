#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS8763

namespace Chasm.SemanticVersioning
{
    // redirect the system's Debug to this one, for proper nullable analysis
    internal static class Debug
    {
        [Conditional("DEBUG")]
        public static void Assert([DoesNotReturnIf(false)] bool condition)
            => System.Diagnostics.Debug.Assert(condition);

        [Conditional("DEBUG"), DoesNotReturn]
        public static void Fail(string message)
            => System.Diagnostics.Debug.Fail(message);
    }
}
#endif
