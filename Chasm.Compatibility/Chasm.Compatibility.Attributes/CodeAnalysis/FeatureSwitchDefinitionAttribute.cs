#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.FeatureSwitchDefinitionAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class FeatureSwitchDefinitionAttribute(string switchName) : Attribute
    {
        public string SwitchName { get; } = switchName;
    }
}
#endif
