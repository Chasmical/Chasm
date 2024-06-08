using System;
using System.ComponentModel;
using System.Globalization;

#if NET7_0_OR_GREATER
namespace Chasm.SemanticVersioning
{
    internal sealed class ParsableTypeConverter<T> : TypeConverter where T : IParsable<T>
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text) return text.Length == 0 ? null : T.Parse(text, culture);
            // Defer to TypeConverter to support InstanceDescriptors
            return base.ConvertFrom(context, culture, value);
        }
    }
}
#else
namespace Chasm.SemanticVersioning
{
    // Default behaviour of overridable methods:
    // - CanConvertFrom() - returns true on typeof(InstanceDescriptor)
    // - CanConvertTo() - returns true on typeof(string)
    // - ConvertFrom() - defers to InstanceDescriptor.Invoke()
    // - ConvertTo() - supports conversion to string (null is turned into "")

    // Conversion to strings works as needed, so we only need to override the ConvertFrom methods

    public sealed partial class SemanticVersion
    {
        internal sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value is string text) return text.Length == 0 ? null : Parse(text);
                // Defer to TypeConverter to support InstanceDescriptors
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
    public readonly partial struct SemverPreRelease
    {
        internal sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value is string text) return Parse(text);
                // Defer to TypeConverter to support InstanceDescriptors
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class PartialVersion
    {
        internal sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value is string text) return text.Length == 0 ? null : Parse(text);
                // Defer to TypeConverter to support InstanceDescriptors
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
    public readonly partial struct PartialComponent
    {
        internal sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value is string text) return Parse(text);
                // Defer to TypeConverter to support InstanceDescriptors
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
    public sealed partial class VersionRange
    {
        internal sealed class Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string);
            public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                if (value is string text) return text.Length == 0 ? null : Parse(text);
                // Defer to TypeConverter to support InstanceDescriptors
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
#endif
