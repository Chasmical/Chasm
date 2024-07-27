#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_1_OR_GREATER || NET11_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Runtime.InteropServices.InAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class InAttribute : Attribute;
}
#endif
