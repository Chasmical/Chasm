using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public readonly partial struct SemverPreRelease
#if NET6_0_OR_GREATER
        : ISpanFormattable
#else
        : IFormattable
#endif
    {
        [Pure] internal int CalculateLength()
            => text?.Length ?? SpanBuilder.CalculateLength(number);
        internal void BuildString(ref SpanBuilder sb)
        {
            if (text is not null) sb.Append(text.AsSpan());
            else sb.Append(number);
        }

        /// <summary>
        ///   <para>Returns the string representation of this pre-release identifier.</para>
        /// </summary>
        /// <returns>The string representation of this pre-release identifier.</returns>
        [Pure] public override string ToString()
            => text ?? number.ToString();

        /// <inheritdoc cref="ISpanFormattable.TryFormat"/>
        [Pure] public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            string? str = text;
            if (str is not null)
            {
#if NET6_0_OR_GREATER
                bool res = str.TryCopyTo(destination);
#elif NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                bool res = ((ReadOnlySpan<char>)str).TryCopyTo(destination);
#else
                bool res = str.AsSpan().TryCopyTo(destination);
#endif
                charsWritten = res ? str.Length : 0;
                return res;
            }
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return number.TryFormat(destination, out charsWritten);
#else
            return number.ToString().TryCopyTo(destination, out charsWritten);
#endif
        }

        [Pure] string IFormattable.ToString(string? _, IFormatProvider? __)
            => ToString();
#if NET6_0_OR_GREATER
        [Pure] bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> _, IFormatProvider? __)
            => TryFormat(destination, out charsWritten);
#endif

    }
}
