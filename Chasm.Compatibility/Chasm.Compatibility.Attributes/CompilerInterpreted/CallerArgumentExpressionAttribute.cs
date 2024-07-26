#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(CallerArgumentExpressionAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
    {
        public string ParameterName { get; } = parameterName;
    }
}
#endif
