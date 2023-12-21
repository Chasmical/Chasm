using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.Formatting
{
    /// <summary>
    ///   <para>Represents an unsafe string builder, that allocates all of the necessary memory beforehand.</para>
    /// </summary>
#if NET5_0_OR_GREATER
    [SkipLocalsInit]
#endif
    public unsafe ref struct SpanBuilder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SpanBuilder(Span<char> buffer) => this.buffer = buffer;
        private readonly Span<char> buffer;
        private int pos;

        /// <summary>
        ///   <para>Appends the specified character to the builder.</para>
        /// </summary>
        /// <param name="c">The character to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
            => buffer[pos++] = c;
        /// <summary>
        ///   <para>Appends two specified characters to the builder.</para>
        /// </summary>
        /// <param name="a">The first character to append to the builder.</param>
        /// <param name="b">The second character to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char a, char b)
        {
            buffer[pos++] = a;
            buffer[pos++] = b;
        }
        /// <summary>
        ///   <para>Appends three specified characters to the builder.</para>
        /// </summary>
        /// <param name="a">The first character to append to the builder.</param>
        /// <param name="b">The second character to append to the builder.</param>
        /// <param name="c">The third character to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char a, char b, char c)
        {
            buffer[pos++] = a;
            buffer[pos++] = b;
            buffer[pos++] = c;
        }
        /// <summary>
        ///   <para>Appends four specified characters to the builder.</para>
        /// </summary>
        /// <param name="a">The first character to append to the builder.</param>
        /// <param name="b">The second character to append to the builder.</param>
        /// <param name="c">The third character to append to the builder.</param>
        /// <param name="d">The third character to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char a, char b, char c, char d)
        {
            buffer[pos++] = a;
            buffer[pos++] = b;
            buffer[pos++] = c;
            buffer[pos++] = d;
        }

        /// <summary>
        ///   <para>Appends the specified read-only span of characters to the builder.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> text)
        {
            text.CopyTo(buffer[pos..]);
            pos += text.Length;
        }

        /// <summary>
        ///   <para>Appends the specified <see cref="ISpanBuildable"/> instance to the builder.</para>
        /// </summary>
        /// <param name="buildable">The <see cref="ISpanBuildable"/> instance to append to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ISpanBuildable buildable)
            => buildable.BuildString(ref this);

        /// <summary>
        ///   <para>Appends the string representation of the specified 32-bit unsigned integer to the builder.</para>
        /// </summary>
        /// <param name="number">The 32-bit unsigned integer to append the string representation of to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(uint number)
        {
            pos += CalculateLength(number);
            fixed (char* ptr = &buffer[pos])
                FillDigits(ptr, number);
        }
        /// <summary>
        ///   <para>Appends the string representation of the specified 32-bit signed integer to the builder.</para>
        /// </summary>
        /// <param name="number">The 32-bit signed integer to append the string representation of to the builder.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(int number)
        {
            if (number >= 0) Append((uint)number);
            else AppendNegative((uint)-unchecked((uint)number));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendNegative(uint number)
        {
            buffer[pos] = '-';
            pos += CalculateLength(number) + 1;
            fixed (char* ptr = &buffer[pos])
                FillDigits(ptr, number);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillDigits(char* end, uint number)
        {
            do
            {
#if NET6_0_OR_GREATER
                (number, uint rem) = Math.DivRem(number, 10u);
#elif NETSTANDARD2_0_OR_GREATER || NET11_OR_GREATER || NETCOREAPP2_0_OR_GREATER
                number = (uint)Math.DivRem(number, 10, out long rem);
#else
                uint div = number / 10u;
                uint rem = number - div * 10u;
                number = div;
#endif
                *--end = (char)('0' + rem);
            }
            while (number != 0u);
        }

        /// <summary>
        ///   <para>Returns the length of the string representation of the specified 32-bit unsigned integer.</para>
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to calculate the length of.</param>
        /// <returns>The length of the string representation of the specified 32-bit unsigned integer.</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(uint value)
        {
            // Copied from: int System.Buffers.Text.FormattingHelpers.CalculateLength(uint)

            int digits = 1;
            if (value >= 100000u)
            {
                value /= 100000u;
                digits += 5;
            }
            if (value >= 10u)
            {
                if (value < 100u) digits++;
                else if (value < 1000u) digits += 2;
                else if (value < 10000u) digits += 3;
                else digits += 4;
            }
            return digits;
        }
        /// <summary>
        ///   <para>Returns the length of the string representation of the specified 32-bit signed integer.</para>
        /// </summary>
        /// <param name="value">The 32-bit signed integer to calculate the length of.</param>
        /// <returns>The length of the string representation of the specified 32-bit signed integer.</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateLength(int value)
            => value >= 0 ? CalculateLength((uint)value) : CalculateLength((uint)-unchecked((uint)value)) + 1;

        /// <summary>
        ///   <para>Returns the string representation of the specified <paramref name="buildable"/> instance.</para>
        /// </summary>
        /// <param name="buildable">The <see cref="ISpanBuildable"/> instance.</param>
        /// <returns>The string representation of the specified <paramref name="buildable"/> instance.</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(ISpanBuildable buildable)
            // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
            => string.Create(buildable.CalculateLength(), buildable, BuildSimpleDelegate);
        /// <summary>
        ///   <para>Returns the string representation of the specified <paramref name="buildable"/> instance, using the specified <paramref name="format"/>.</para>
        /// </summary>
        /// <param name="buildable">The <see cref="ISpanBuildableFormat"/> instance.</param>
        /// <param name="format">The format to use.</param>
        /// <returns>The string representation of the specified <paramref name="buildable"/> instance, as specified <paramref name="format"/>.</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(ISpanBuildableFormat buildable, ReadOnlySpan<char> format)
        {
            fixed (char* formatPointer = format)
            {
                FormatInfo info = new FormatInfo(buildable, formatPointer, format.Length);
                // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
                return string.Create(buildable.CalculateLength(format), info, BuildFormatDelegate);
            }
        }
        /// <summary>
        ///   <para>Creates a string of the specified <paramref name="length"/>, constructed by the specified <paramref name="action"/>.</para>
        /// </summary>
        /// <param name="length">The length of the string to create.</param>
        /// <param name="action">The action that fills the string with characters.</param>
        /// <returns>The created string of the specified <paramref name="length"/>, filled by the specified <paramref name="action"/>.</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(int length, SpanBuilderAction action)
            // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
            => string.Create(length, action, BuildActionDelegate);

        private static readonly SpanAction<char, ISpanBuildable> BuildSimpleDelegate = BuildSimple;
        private static readonly SpanAction<char, FormatInfo> BuildFormatDelegate = BuildFormat;
        private static readonly SpanAction<char, SpanBuilderAction> BuildActionDelegate = BuildAction;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildSimple(Span<char> span, ISpanBuildable buildable)
        {
            SpanBuilder sb = new SpanBuilder(span);
            buildable.BuildString(ref sb);
            if (sb.pos != span.Length) throw new InvalidOperationException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildFormat(Span<char> span, FormatInfo info)
        {
            SpanBuilder sb = new SpanBuilder(span);
            ReadOnlySpan<char> format = new ReadOnlySpan<char>(info.FormatStart, info.FormatLength);
            info.Buildable.BuildString(ref sb, format);
            if (sb.pos != span.Length) throw new InvalidOperationException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildAction(Span<char> span, SpanBuilderAction action)
        {
            SpanBuilder sb = new SpanBuilder(span);
            action(ref sb);
            if (sb.pos != span.Length) throw new InvalidOperationException();
        }

        /// <summary>
        ///   <para>Represents the method that uses a <see cref="SpanBuilder"/> instance to construct a string.</para>
        /// </summary>
        /// <param name="sb">The <see cref="SpanBuilder"/> instance to use.</param>
        public delegate void SpanBuilderAction(ref SpanBuilder sb);

        private readonly struct FormatInfo(ISpanBuildableFormat buildable, char* formatStart, int formatLength)
        {
            public readonly ISpanBuildableFormat Buildable = buildable;
            public readonly char* FormatStart = formatStart;
            public readonly int FormatLength = formatLength;
        }

    }
    /// <summary>
    ///   <para>Provides functionality to construct a string representation of an object without unnecessary memory allocations.</para>
    /// </summary>
    public interface ISpanBuildable
    {
        /// <summary>
        ///   <para>Returns the length of the string representation of the current instance.</para>
        /// </summary>
        /// <returns>The length of the string representation of the current instance.</returns>
        [Pure] int CalculateLength();
        /// <summary>
        ///   <para>Appends the string representation of the current instance to the specified <see cref="SpanBuilder"/> instance.</para>
        /// </summary>
        /// <param name="sb">The <see cref="SpanBuilder"/> instance to use.</param>
        void BuildString(ref SpanBuilder sb);
    }
    /// <summary>
    ///   <para>Provides functionality to construct a string representation of an object without unnecessary memory allocations.</para>
    /// </summary>
    public interface ISpanBuildableFormat
    {
        /// <summary>
        ///   <para>Returns the length of the string representation of the current instance, using the specified <paramref name="format"/>.</para>
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The length of the string representation of the current instance, using the specified <paramref name="format"/>.</returns>
        [Pure] int CalculateLength(ReadOnlySpan<char> format);
        /// <summary>
        ///   <para>Appends the string representation of the current instance to the specified <see cref="SpanBuilder"/> instance, using the specified <paramref name="format"/>.</para>
        /// </summary>
        /// <param name="sb">The <see cref="SpanBuilder"/> instance to use.</param>
        /// <param name="format">The format to use.</param>
        void BuildString(ref SpanBuilder sb, ReadOnlySpan<char> format);
    }
}
