#if NETCOREAPP1_0_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(TupleElementNamesAttribute))]
#else
// Adapted from .NET's source code
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Field
                  | AttributeTargets.Parameter
                  | AttributeTargets.Property
                  | AttributeTargets.ReturnValue
                  | AttributeTargets.Class
                  | AttributeTargets.Struct
                  | AttributeTargets.Event)]
    public sealed class TupleElementNamesAttribute(string?[] transformNames) : Attribute
    {
        private readonly string?[] _transformNames = transformNames ?? throw new ArgumentNullException(nameof(transformNames));
        public IList<string?> TransformNames => _transformNames;
    }
}
#endif
