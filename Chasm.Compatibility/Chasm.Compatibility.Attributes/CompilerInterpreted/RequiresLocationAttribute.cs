#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(RequiresLocationAttribute))]
#else
using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class RequiresLocationAttribute : Attribute;
}
#endif
