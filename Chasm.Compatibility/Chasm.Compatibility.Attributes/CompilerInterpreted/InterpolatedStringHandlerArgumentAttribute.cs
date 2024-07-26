#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(InterpolatedStringHandlerArgumentAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class InterpolatedStringHandlerArgumentAttribute(params string[] arguments) : Attribute
    {
        public InterpolatedStringHandlerArgumentAttribute(string argument) : this([argument]) { }

        public string[] Arguments { get; } = arguments;
    }
}
#endif
