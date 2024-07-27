#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(CompilerFeatureRequiredAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
    {
        public string FeatureName { get; } = featureName;
        public bool IsOptional { get; set; }

        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);

    }
}
#endif
