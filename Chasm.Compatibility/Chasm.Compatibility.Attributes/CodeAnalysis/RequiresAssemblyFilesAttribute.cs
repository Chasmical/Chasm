#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.RequiresAssemblyFilesAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor |
                    AttributeTargets.Event |
                    AttributeTargets.Method |
                    AttributeTargets.Property,
                    Inherited = false,
                    AllowMultiple = false)]
    public sealed class RequiresAssemblyFilesAttribute(string message) : Attribute
    {
        public RequiresAssemblyFilesAttribute() : this(null!) { }
        public string? Message { get; } = message;
        public string? Url { get; set; }
    }
}
#endif
