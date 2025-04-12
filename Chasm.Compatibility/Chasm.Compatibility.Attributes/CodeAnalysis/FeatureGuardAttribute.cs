#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.FeatureGuardAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class FeatureGuardAttribute(Type featureType) : Attribute
    {
        public Type FeatureType { get; } = featureType;
    }
}
#endif
