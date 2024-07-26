#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MemberNotNullWhenAttribute(bool returnValue, params string[] members) : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member) : this(returnValue, [member]) { }

        public bool ReturnValue { get; } = returnValue;
        public string[] Members { get; } = members;
    }
}
#endif
