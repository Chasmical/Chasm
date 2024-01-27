using System;

namespace Chasm.Formatting
{
    public static class FormattingExtensions
    {
        public static bool TryCopyTo(this string text, Span<char> destination, out int charsWritten)
        {
#if NET6_0_OR_GREATER
            bool res = text.TryCopyTo(destination);
            charsWritten = res ? text.Length : 0;
            return res;
#else
            return TryCopyTo(text.AsSpan(), destination, out charsWritten);
#endif
        }
        public static bool TryCopyTo<T>(this Span<T> span, Span<T> destination, out int itemsWritten)
            => TryCopyTo((ReadOnlySpan<T>)span, destination, out itemsWritten);
        public static bool TryCopyTo<T>(this ReadOnlySpan<T> span, Span<T> destination, out int itemsWritten)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            bool res = span.TryCopyTo(destination);
            itemsWritten = res ? span.Length : 0;
            return res;
#else
            if (span.Length < destination.Length)
            {
                span.CopyTo(destination);
                itemsWritten = span.Length;
                return true;
            }
            itemsWritten = 0;
            return false;
#endif
        }

    }
}
