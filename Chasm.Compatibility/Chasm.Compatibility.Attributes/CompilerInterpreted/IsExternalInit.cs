#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(IsExternalInit))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    public static class IsExternalInit;
}
#endif
