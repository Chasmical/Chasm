using System;
using System.Diagnostics;
using System.Globalization;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public readonly partial struct PartialComponent
#if NET7_0_OR_GREATER
        : ISpanParsable<PartialComponent>
#endif
    {
        // ReSharper disable once UnusedParameter.Local
        internal PartialComponent(int value, bool _)
        {
            // Make sure the internal constructor isn't used with an invalid parameter
            Debug.Assert(value is >= 0 or -1 or -'x' or -'X' or -'*');

            _value = (uint)value;
        }

        /// <summary>
        ///   <para>Converts the specified wildcard or numeric character representing a partial version component to an equivalent <see cref="PartialComponent"/> structure.</para>
        /// </summary>
        /// <param name="character">The wildcard or numeric character representing a partial version component to convert.</param>
        /// <returns>The <see cref="PartialComponent"/> structure equivalent to the partial version component represented by the <paramref name="character"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="character"/> does not represent a valid partial version component.</exception>
        [Pure] public static PartialComponent Parse(char character) => character switch
        {
            'x' or 'X' or '*' => new PartialComponent(-character, default),
            >= '0' and <= '9' => new PartialComponent(character - '0', default),
            _ => throw new ArgumentException(Exceptions.ComponentInvalid, nameof(character)),
        };
        /// <summary>
        ///   <para>Tries to convert the wildcard or numeric character representing a partial version component to an equivalent <see cref="PartialComponent"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="character">The wildcard or numeric character representing a partial version component to convert.</param>
        /// <param name="component">When this method returns, contains the <see cref="PartialComponent"/> structure equivalent to the partial version component represented by the <paramref name="character"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(char character, out PartialComponent component)
        {
            switch (character)
            {
                case 'x' or 'X' or '*':
                    component = new PartialComponent(-character, default);
                    break;
                case >= '0' and <= '9':
                    component = new PartialComponent(character - '0', default);
                    break;
                default:
                    component = default;
                    return false;
            }
            return true;
        }

        [Pure] internal static SemverErrorCode ParseTrimmed(ReadOnlySpan<char> text, SemverOptions options, out PartialComponent component)
        {
            if (text.IsEmpty)
            {
                component = new PartialComponent(-1, default);
                return SemverErrorCode.Success;
            }
            if (text.Length == 1) return TryParse(text[0], out component) ? SemverErrorCode.Success : SemverErrorCode.ComponentInvalid;
            component = default;

            if (Utility.IsNumeric(text))
            {
                if ((options & SemverOptions.AllowLeadingZeroes) == 0 && text[0] == '0' && text.Length > 1)
                    return SemverErrorCode.ComponentLeadingZeroes;
                if (!int.TryParse(text, NumberStyles.None, null, out int value))
                    return SemverErrorCode.ComponentTooBig;
                component = new PartialComponent(value, default);
                return SemverErrorCode.Success;
            }

            // at this point, it's not a numeric component, and text's length > 1
            if ((options & SemverOptions.AllowExtraWildcards) == 0)
                return SemverErrorCode.ComponentInvalid;

            // make sure all the wildcard characters are the same
            char wildcard = text[0];
            for (int i = 1; i < text.Length; i++)
                if (text[i] != wildcard)
                    return SemverErrorCode.ComponentInvalid;

            component = new PartialComponent(-wildcard, default);
            return SemverErrorCode.Success;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a partial version component to an equivalent <see cref="PartialComponent"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version component to convert.</param>
        /// <returns>The <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version component.</exception>
        [Pure] public static PartialComponent Parse(string text)
            => text is null ? throw new ArgumentNullException(nameof(text)) : Parse(text.AsSpan());
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version component to an equivalent <see cref="PartialComponent"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version component to convert.</param>
        /// <returns>The <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version component.</exception>
        [Pure] public static PartialComponent Parse(ReadOnlySpan<char> text)
        {
            SemverErrorCode code = ParseTrimmed(text, SemverOptions.Strict, out PartialComponent component);
            return code.ReturnOrThrow(component, nameof(text));
        }
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version component to an equivalent <see cref="PartialComponent"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version component to convert.</param>
        /// <param name="component">When this method returns, contains the <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out PartialComponent component)
        {
            if (text is not null) return TryParse(text.AsSpan(), out component);
            component = default;
            return false;
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version component to an equivalent <see cref="PartialComponent"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version component to convert.</param>
        /// <param name="component">When this method returns, contains the <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out PartialComponent component)
            => ParseTrimmed(text, SemverOptions.Strict, out component) is SemverErrorCode.Success;

        /// <summary>
        ///   <para>Converts the specified string representation of a partial version component to an equivalent <see cref="PartialComponent"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version component to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version component.</exception>
        [Pure] public static PartialComponent Parse(string text, SemverOptions options)
            => text is null ? throw new ArgumentNullException(nameof(text)) : Parse(text.AsSpan(), options);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version component to an equivalent <see cref="PartialComponent"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version component to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version component.</exception>
        [Pure] public static PartialComponent Parse(ReadOnlySpan<char> text, SemverOptions options)
        {
            SemverErrorCode code = ParseTrimmed(Utility.Trim(text, options), options, out PartialComponent component);
            return code.ReturnOrThrow(component, nameof(text));
        }
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version component to an equivalent <see cref="PartialComponent"/> structure using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version component to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="component">When this method returns, contains the <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, SemverOptions options, out PartialComponent component)
        {
            if (text is not null) return TryParse(text.AsSpan(), options, out component);
            component = default;
            return false;
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version component to an equivalent <see cref="PartialComponent"/> structure using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version component to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="component">When this method returns, contains the <see cref="PartialComponent"/> structure equivalent to the partial version component specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemverOptions options, out PartialComponent component)
            => ParseTrimmed(Utility.Trim(text, options), options, out component) is SemverErrorCode.Success;

#if NET7_0_OR_GREATER
        [Pure] static PartialComponent IParsable<PartialComponent>.Parse(string s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool IParsable<PartialComponent>.TryParse(string? s, IFormatProvider? _, out PartialComponent preRelease)
            => TryParse(s, out preRelease);
        [Pure] static PartialComponent ISpanParsable<PartialComponent>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool ISpanParsable<PartialComponent>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, out PartialComponent preRelease)
            => TryParse(s, out preRelease);
#endif

    }
}
