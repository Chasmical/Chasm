using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public readonly partial struct SemverPreRelease
        : ISpanBuildable
#if NET6_0_OR_GREATER
        , ISpanFormattable
#else
        , IFormattable
#endif
    {
        [Pure] internal int CalculateLength()
            => text?.Length ?? SpanBuilder.CalculateLength(number);
        internal void BuildString(ref SpanBuilder sb)
        {
            if (text is not null) sb.Append(text);
            else sb.Append(number);
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this pre-release identifier.</para>
        /// </summary>
        /// <returns>The string representation of this pre-release identifier.</returns>
        [Pure] public override string ToString()
            => text ?? number.ToString();

        /// <inheritdoc cref="ISpanFormattable.TryFormat"/>
        [Pure] public bool TryFormat(Span<char> destination, out int charsWritten)
        {
            return text is null
                ? number.TryFormat(destination, out charsWritten)
                : text.TryCopyTo(destination, out charsWritten);
        }

        [Pure] string IFormattable.ToString(string? _, IFormatProvider? __)
            => ToString();
#if NET6_0_OR_GREATER
        [Pure] bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> _, IFormatProvider? __)
            => TryFormat(destination, out charsWritten);
#endif

    }
}
