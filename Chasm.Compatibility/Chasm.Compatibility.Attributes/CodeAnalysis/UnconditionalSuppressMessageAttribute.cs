#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessageAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class UnconditionalSuppressMessageAttribute(string category, string checkId) : Attribute
    {
        public string Category { get; } = category;
        public string CheckId { get; } = checkId;
        public string? Scope { get; set; }
        public string? Target { get; set; }
        public string? MessageId { get; set; }
        public string? Justification { get; set; }
    }
}
#endif
