using System;

namespace Chasm.SemanticVersioning.Tests
{
    public static class TestUtil
    {
        public const SemverOptions PseudoStrict = (SemverOptions)int.MinValue; // only the sign bit is on

        public static T Parse<T>(string text) where T : IParsable<T>
            => T.Parse(text, null);
        public static bool TryParse<T>(string text, out T? result) where T : IParsable<T>
            => T.TryParse(text, null, out result);
        public static T SpanParse<T>(ReadOnlySpan<char> text) where T : ISpanParsable<T>
            => T.Parse(text, null);
        public static bool TrySpanParse<T>(ReadOnlySpan<char> text, out T? result) where T : ISpanParsable<T>
            => T.TryParse(text, null, out result);

    }
}
