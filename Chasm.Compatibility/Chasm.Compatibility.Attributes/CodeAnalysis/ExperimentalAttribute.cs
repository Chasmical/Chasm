#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.ExperimentalAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Assembly
                  | AttributeTargets.Module
                  | AttributeTargets.Class
                  | AttributeTargets.Struct
                  | AttributeTargets.Enum
                  | AttributeTargets.Constructor
                  | AttributeTargets.Method
                  | AttributeTargets.Property
                  | AttributeTargets.Field
                  | AttributeTargets.Event
                  | AttributeTargets.Interface
                  | AttributeTargets.Delegate, Inherited = false)]
    public sealed class ExperimentalAttribute(string diagnosticId) : Attribute
    {
        public string DiagnosticId { get; } = diagnosticId;
        public string? UrlFormat { get; set; }
    }
}
#endif
