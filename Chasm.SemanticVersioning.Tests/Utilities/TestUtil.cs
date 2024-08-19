using System;
using System.Collections.Generic;
using System.Linq;
using Chasm.Collections;
using Xunit;

namespace Chasm.SemanticVersioning.Tests
{
    public static class TestUtil
    {
        public const SemverOptions PseudoStrict = (SemverOptions)int.MinValue; // only the sign bit is on

        // ReSharper disable once IdentifierTypo
        public const string DeserCtor = "This constructor is only used for deserialization.";

        public static T Parse<T>(string text) where T : IParsable<T>
            => T.Parse(text, null);
        public static bool TryParse<T>(string text, out T? result) where T : IParsable<T>
            => T.TryParse(text, null, out result);
        public static T SpanParse<T>(ReadOnlySpan<char> text) where T : ISpanParsable<T>
            => T.Parse(text, null);
        public static bool TrySpanParse<T>(ReadOnlySpan<char> text, out T? result) where T : ISpanParsable<T>
            => T.TryParse(text, null, out result);

        public static string FormatWithTryFormat(ISpanFormattable obj, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            Span<char> buffer = stackalloc char[128];
            int length;
            while (!obj.TryFormat(buffer, out length, format, provider))
            {
                if (buffer.Length >= 4096) throw new InvalidOperationException();
                buffer = new char[buffer.Length * 2];
            }
            // make sure that only the reported region was written to
            Assert.Equal(buffer[length..].ToString(), new string('\0', buffer.Length - length));

            return buffer[..length].ToString();
        }
        public static string FormatWithTryFormat(TryFormatDelegate func)
            => FormatWithTryFormat(new TryFormatStruct(func));

        public delegate bool TryFormatDelegate(Span<char> destination, out int charsWritten);

        private readonly struct TryFormatStruct(TryFormatDelegate func) : ISpanFormattable
        {
            string IFormattable.ToString(string? _, IFormatProvider? __)
                => throw new InvalidOperationException();
            bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> _, IFormatProvider? __)
                => func(destination, out charsWritten);
        }

        public static (IEnumerable<object>, IEnumerable<string>) Split(object[] identifiers, string prefix)
        {
            int buildMetadataIndex = Array.FindIndex(identifiers, obj => obj is string str && str.StartsWith(prefix, StringComparison.Ordinal));
            if (buildMetadataIndex < 0) buildMetadataIndex = identifiers.Length;

            object[] left = identifiers[..buildMetadataIndex];
            string[] right = identifiers[buildMetadataIndex..].Cast<string>();
            if (right.Length > 0) right[0] = right[0][prefix.Length..];
            return (left, right);
        }

        public static IEnumerable<T> GetFixtures<T>(this TheoryData<T> data)
            => data.Select(args => (T)args[0]!);

    }
}
