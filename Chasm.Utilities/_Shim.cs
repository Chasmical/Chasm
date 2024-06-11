#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis
{
    #pragma warning disable CS9113 // Parameter is unread.
    internal class NotNullWhenAttribute(bool _) : Attribute;
}
#endif
#if !(NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER)
// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices
{
    // This is a compilation-only attribute, that just translates to a bit in the ImplFlags column
    internal class MethodImplAttribute(MethodImplOptions options) : Attribute;
    internal enum MethodImplOptions { AggressiveInlining = 256 }
}
#endif
