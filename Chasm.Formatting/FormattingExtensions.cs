using System;
using System.Text;
using JetBrains.Annotations;

namespace Chasm.Formatting
{
    /// <summary>
    ///   <para>Provides a set of extension methods for formatting.</para>
    /// </summary>
    public static class FormattingExtensions
    {
        /// <summary>
        ///   <para>Tries to copy the contents of this string into the provided span of characters.</para>
        /// </summary>
        /// <param name="text">The string to copy.</param>
        /// <param name="destination">The span in which to copy the specified <paramref name="text"/>.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in <paramref name="destination"/>.</param>
        /// <returns><see langword="true"/>, if the copying was successful; otherwise, <see langword="false"/>.</returns>
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
        /// <summary>
        ///   <para>Tries to copy the contents of this span into the provided span.</para>
        /// </summary>
        /// <param name="span">The span to copy.</param>
        /// <param name="destination">The span in which to copy the specified <paramref name="span"/>.</param>
        /// <param name="itemsWritten">When this method returns, contains the number of items that were written in <paramref name="destination"/>.</param>
        /// <returns><see langword="true"/>, if the copying was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryCopyTo<T>(this Span<T> span, Span<T> destination, out int itemsWritten)
            => TryCopyTo((ReadOnlySpan<T>)span, destination, out itemsWritten);
        /// <summary>
        ///   <para>Tries to copy the contents of this read-only span into the provided span.</para>
        /// </summary>
        /// <param name="span">The span to copy.</param>
        /// <param name="destination">The span in which to copy the specified <paramref name="span"/>.</param>
        /// <param name="itemsWritten">When this method returns, contains the number of items that were written in <paramref name="destination"/>.</param>
        /// <returns><see langword="true"/>, if the copying was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryCopyTo<T>(this ReadOnlySpan<T> span, Span<T> destination, out int itemsWritten)
        {
            bool res = span.TryCopyTo(destination);
            itemsWritten = res ? span.Length : 0;
            return res;
        }

        /// <summary>
        ///   <para>Gets the specified <paramref name="stringBuilder"/>'s built string and clears the builder.</para>
        /// </summary>
        /// <param name="stringBuilder"><paramref name="stringBuilder"/> to get the built string of, and to clear.</param>
        /// <returns>The specified <paramref name="stringBuilder"/>'s built string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stringBuilder"/> is <see langword="null"/>.</exception>
        [Pure] public static string ToStringAndClear(this StringBuilder stringBuilder)
        {
            ANE.ThrowIfNull(stringBuilder);
            string result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }

    }
}
