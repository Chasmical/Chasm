using System;
using System.ComponentModel;
using System.Globalization;

// Default behavior of overridable methods:
// - CanConvertFrom() - returns true on typeof(InstanceDescriptor)
// - CanConvertTo() - returns true on typeof(string)
// - ConvertFrom() - depends on InstanceDescriptor.Invoke()
// - ConvertTo() - supports conversion to strings (null is converted into "")

// ConvertTo methods work as needed with strings, so we only need to override the ConvertFrom methods

#if NET7_0_OR_GREATER
namespace Chasm.SemanticVersioning
{
    // Use a generic type converter, that uses the IParsable<T> interface, if possible
    internal sealed class ParsableTypeConverter<T> : TypeConverter where T : IParsable<T>
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text)
            {
                if (default(T) is null && text.Length == 0) return null;
                return T.Parse(text, culture);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(ParsableTypeConverter<SemanticVersion>))]
    public sealed partial class SemanticVersion;
    [TypeConverter(typeof(ParsableTypeConverter<SemverPreRelease>))]
    public readonly partial struct SemverPreRelease;
}
namespace Chasm.SemanticVersioning.Ranges
{
    [TypeConverter(typeof(ParsableTypeConverter<PartialVersion>))]
    public sealed partial class PartialVersion;
    [TypeConverter(typeof(ParsableTypeConverter<PartialComponent>))]
    public readonly partial struct PartialComponent;
    [TypeConverter(typeof(ParsableTypeConverter<VersionRange>))]
    public sealed partial class VersionRange;
}
#elif NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET11_OR_GREATER
namespace Chasm.SemanticVersioning
{
    // Otherwise, use a generic type converter that uses an abstract method
    internal abstract class ParsableTypeConverter<T> : TypeConverter where T : notnull
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text)
            {
                if (default(T) is null && text.Length == 0) return null;
                return ConvertString(text);
            }
            return base.ConvertFrom(context, culture, value);
        }

        protected abstract T ConvertString(string text);
    }

    [TypeConverter(typeof(Converter))]
    public sealed partial class SemanticVersion
    {
        private sealed class Converter : ParsableTypeConverter<SemanticVersion>
        {
            protected override SemanticVersion ConvertString(string text) => Parse(text);
        }
    }
    [TypeConverter(typeof(Converter))]
    public readonly partial struct SemverPreRelease
    {
        private sealed class Converter : ParsableTypeConverter<SemverPreRelease>
        {
            protected override SemverPreRelease ConvertString(string text) => Parse(text);
        }
    }
}
namespace Chasm.SemanticVersioning.Ranges
{
    [TypeConverter(typeof(Converter))]
    public sealed partial class PartialVersion
    {
        private sealed class Converter : ParsableTypeConverter<PartialVersion>
        {
            protected override PartialVersion ConvertString(string text) => Parse(text);
        }
    }
    [TypeConverter(typeof(Converter))]
    public readonly partial struct PartialComponent
    {
        private sealed class Converter : ParsableTypeConverter<PartialComponent>
        {
            protected override PartialComponent ConvertString(string text) => Parse(text);
        }
    }
    [TypeConverter(typeof(Converter))]
    public sealed partial class VersionRange
    {
        private sealed class Converter : ParsableTypeConverter<VersionRange>
        {
            protected override VersionRange ConvertString(string text) => Parse(text);
        }
    }
}
#endif
