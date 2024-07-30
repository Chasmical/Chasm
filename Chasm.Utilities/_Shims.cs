#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
    {
        public bool ReturnValue { get; } = returnValue;
    }
}
#endif

#if !(NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER)
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    // This is a compilation-only attribute, that just translates to a bit in the ImplFlags column
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    internal sealed class MethodImplAttribute(MethodImplOptions methodImplOptions) : Attribute
    {
        public MethodImplOptions Value { get; } = methodImplOptions;

        public MethodImplAttribute(short value) : this((MethodImplOptions)value) { }
        public MethodImplAttribute() : this(default(MethodImplOptions)) { }
    }
    [Flags] internal enum MethodImplOptions { AggressiveInlining = 256 }
}
#endif
