using System;
using System.ComponentModel;
using System.Globalization;

// Default behaviour of overridable methods:
// - CanConvertFrom() - returns true on typeof(InstanceDescriptor)
// - CanConvertTo() - returns true on typeof(string)
// - ConvertFrom() - depends on InstanceDescriptor.Invoke()
// - ConvertTo() - supports conversion to strings (null is converted into "")

// ConvertTo methods work as needed with strings, so we only need to override the ConvertFrom methods

#if NET7_0_OR_GREATER
namespace Chasm.SemanticVersioning
{
    // A generic type converter that uses the IParsable<T> interface
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
#else
namespace Chasm.SemanticVersioning
{
    // Separate implementations for each type

    [TypeConverter(typeof(Converter))]
    public sealed partial class SemanticVersion
    {
        private sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => value is string text ? text.Length == 0 ? null : Parse(text) : base.ConvertFrom(context, culture, value);
        }
    }
    [TypeConverter(typeof(Converter))]
    public readonly partial struct SemverPreRelease
    {
        private sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => value is string text ? Parse(text) : base.ConvertFrom(context, culture, value);
        }
    }
}
namespace Chasm.SemanticVersioning.Ranges
{
    [TypeConverter(typeof(Converter))]
    public sealed partial class PartialVersion
    {
        private sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => value is string text ? text.Length == 0 ? null : Parse(text) : base.ConvertFrom(context, culture, value);
        }
    }
    [TypeConverter(typeof(Converter))]
    public readonly partial struct PartialComponent
    {
        private sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => value is string text ? Parse(text) : base.ConvertFrom(context, culture, value);
        }
    }
    [TypeConverter(typeof(Converter))]
    public sealed partial class VersionRange
    {
        private sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => value is string text ? text.Length == 0 ? null : Parse(text) : base.ConvertFrom(context, culture, value);
        }
    }
}
#endif
