#if NET7_0_OR_GREATER
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chasm.SemanticVersioning
{
    // Use a generic type converter, that uses the IParsable<T> interface, if possible
    internal sealed class ParsableJsonConverter<T> : JsonConverter<T> where T : IParsable<T>
    {
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            => writer.WriteStringValue(value?.ToString());

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? text = reader.GetString();
            if (default(T) is null && (text is null || text.Length == 0)) return default;
            return T.Parse(text!, null);
        }
    }

    [JsonConverter(typeof(ParsableJsonConverter<SemanticVersion>))]
    public sealed partial class SemanticVersion;
    [JsonConverter(typeof(ParsableJsonConverter<SemverPreRelease>))]
    public readonly partial struct SemverPreRelease;
}
namespace Chasm.SemanticVersioning.Ranges
{
    [JsonConverter(typeof(ParsableJsonConverter<PartialVersion>))]
    public sealed partial class PartialVersion;
    [JsonConverter(typeof(ParsableJsonConverter<PartialComponent>))]
    public readonly partial struct PartialComponent;
    [JsonConverter(typeof(ParsableJsonConverter<VersionRange>))]
    public sealed partial class VersionRange;
}
#elif NETCOREAPP3_0_OR_GREATER || NET4_6_2_OR_GREATER
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chasm.SemanticVersioning
{
    // Otherwise, use a generic type converter that uses an abstract method
    internal abstract class ParsableJsonConverter<T> : JsonConverter<T?> where T : notnull
    {
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            => writer.WriteStringValue(value?.ToString());

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? text = reader.GetString();
            if (default(T) is null && (text is null || text.Length == 0)) return default;
            return ConvertString(text!);
        }
        protected abstract T ConvertString(string text);
    }

    [JsonConverter(typeof(ConverterJson))]
    public sealed partial class SemanticVersion
    {
        private sealed class ConverterJson : ParsableJsonConverter<SemanticVersion>
        {
            protected override SemanticVersion ConvertString(string text) => Parse(text);
        }
    }
    [JsonConverter(typeof(ConverterJson))]
    public readonly partial struct SemverPreRelease
    {
        private sealed class ConverterJson : ParsableJsonConverter<SemverPreRelease>
        {
            protected override SemverPreRelease ConvertString(string text) => Parse(text);
        }
    }
}
namespace Chasm.SemanticVersioning.Ranges
{
    [JsonConverter(typeof(ConverterJson))]
    public sealed partial class PartialVersion
    {
        private sealed class ConverterJson : ParsableJsonConverter<PartialVersion>
        {
            protected override PartialVersion ConvertString(string text) => Parse(text);
        }
    }
    [JsonConverter(typeof(ConverterJson))]
    public readonly partial struct PartialComponent
    {
        private sealed class ConverterJson : ParsableJsonConverter<PartialComponent>
        {
            protected override PartialComponent ConvertString(string text) => Parse(text);
        }
    }
    [JsonConverter(typeof(ConverterJson))]
    public sealed partial class VersionRange
    {
        private sealed class ConverterJson : ParsableJsonConverter<VersionRange>
        {
            protected override VersionRange ConvertString(string text) => Parse(text);
        }
    }
}
#endif
