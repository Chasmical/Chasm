﻿using System;

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
