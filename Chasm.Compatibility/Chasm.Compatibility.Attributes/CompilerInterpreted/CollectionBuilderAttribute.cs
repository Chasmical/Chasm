#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(CollectionBuilderAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
    public sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute
    {
        public Type BuilderType { get; } = builderType;
        public string MethodName { get; } = methodName;
    }
}
#endif
