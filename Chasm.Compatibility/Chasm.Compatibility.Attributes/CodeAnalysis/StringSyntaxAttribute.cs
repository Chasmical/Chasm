#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
[assembly: TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute))]
#else
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class StringSyntaxAttribute(string syntax, params object?[] arguments) : Attribute
    {
        public StringSyntaxAttribute(string syntax) : this(syntax, []) { }

        public string Syntax { get; } = syntax;
        public object?[] Arguments { get; } = arguments;

        public const string CompositeFormat = nameof(CompositeFormat);
        public const string DateOnlyFormat = nameof(DateOnlyFormat);
        public const string DateTimeFormat = nameof(DateTimeFormat);
        public const string EnumFormat = nameof(EnumFormat);
        public const string GuidFormat = nameof(GuidFormat);
        public const string Json = nameof(Json);
        public const string NumericFormat = nameof(NumericFormat);
        public const string Regex = nameof(Regex);
        public const string TimeOnlyFormat = nameof(TimeOnlyFormat);
        public const string TimeSpanFormat = nameof(TimeSpanFormat);
        public const string Uri = nameof(Uri);
        public const string Xml = nameof(Xml);

    }
}
#endif
