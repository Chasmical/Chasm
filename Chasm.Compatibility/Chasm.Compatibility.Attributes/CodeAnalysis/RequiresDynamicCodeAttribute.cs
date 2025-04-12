#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    public sealed class RequiresDynamicCodeAttribute(string message) : Attribute
    {
        public string Message { get; } = message;
        public string? Url { get; set; }
    }
}
#endif
