#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
#pragma warning disable CS9113 // Parameter is unread.
    internal class NotNullWhenAttribute(bool _) : Attribute;
}
#endif
