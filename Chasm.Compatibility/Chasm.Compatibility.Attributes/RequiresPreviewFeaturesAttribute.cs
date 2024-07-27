#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Runtime.Versioning.RequiresPreviewFeaturesAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly
                  | AttributeTargets.Module
                  | AttributeTargets.Class
                  | AttributeTargets.Interface
                  | AttributeTargets.Delegate
                  | AttributeTargets.Struct
                  | AttributeTargets.Enum
                  | AttributeTargets.Constructor
                  | AttributeTargets.Method
                  | AttributeTargets.Property
                  | AttributeTargets.Field
                  | AttributeTargets.Event, Inherited = false)]
    public sealed class RequiresPreviewFeaturesAttribute(string? message) : Attribute
    {
        public RequiresPreviewFeaturesAttribute() : this(null) { }

        public string? Message { get; } = message;
        public string? Url { get; set; }
    }
}
#endif
