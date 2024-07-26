#if NETCOREAPP1_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(AsyncMethodBuilderAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class
                  | AttributeTargets.Struct
                  | AttributeTargets.Interface
                  | AttributeTargets.Delegate
                  | AttributeTargets.Enum
                  | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AsyncMethodBuilderAttribute(Type builderType) : Attribute
    {
        public Type BuilderType { get; } = builderType;
    }
}
#endif
