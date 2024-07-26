#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET40_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        // Note: this property is not shimmed, because it was introduced only in .NET 5
        // public string? Justification { get; set; }
    }
}
#endif
