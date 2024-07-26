#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(CallerMemberNameAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute;
}
#endif
