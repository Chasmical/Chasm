using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public readonly partial struct SemverPreRelease
#if NET7_0_OR_GREATER
        : ISpanParsable<SemverPreRelease>
#endif
    {
        // ReSharper disable once UnusedParameter.Local
        private SemverPreRelease(string validIdentifier, bool _)
        {
            // Make sure the internal constructor isn't used with an invalid parameter
            Debug.Assert(Utility.AllValidCharacters(validIdentifier.AsSpan()));

            text = validIdentifier;
        }

        [Pure] private static SemverErrorCode ParseInitial(ReadOnlySpan<char> text, bool allowLeadingZeroes, out int result)
        {
            result = -1;
            if (text.IsEmpty) return SemverErrorCode.PreReleaseEmpty;
            if (Utility.IsNumeric(text))
            {
                if (!allowLeadingZeroes && text[0] == '0' && text.Length > 1)
                    return SemverErrorCode.PreReleaseLeadingZeroes;
                if (!Utility.TryParseNonNegativeInt32(text, out result))
                    return SemverErrorCode.PreReleaseTooBig;
            }
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseTrimmed(ReadOnlySpan<char> text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            preRelease = default;
            SemverErrorCode code = ParseInitial(text, allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return code;
            if (result == -1)
            {
                if (!Utility.AllValidCharacters(text)) return SemverErrorCode.PreReleaseInvalid;
                preRelease = new SemverPreRelease(text.ToString(), default);
            }
            else preRelease = new SemverPreRelease(result);
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseTrimmed(string text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            preRelease = default;
            SemverErrorCode code = ParseInitial(text.AsSpan(), allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return code;
            if (result == -1)
            {
                if (!Utility.AllValidCharacters(text.AsSpan())) return SemverErrorCode.PreReleaseInvalid;
                preRelease = new SemverPreRelease(text, default);
            }
            else preRelease = new SemverPreRelease(result);
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseValidated(ReadOnlySpan<char> text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            preRelease = default;
            SemverErrorCode code = ParseInitial(text, allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return code;
            preRelease = result == -1 ? new SemverPreRelease(text.ToString(), default) : new SemverPreRelease(result);
            return SemverErrorCode.Success;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing a pre-release identifier to convert.</param>
        /// <returns>The <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid pre-release identifier.</exception>
        [Pure] public static SemverPreRelease Parse(string text)
        {
            ANE.ThrowIfNull(text);
            return ParseTrimmed(text, false, out SemverPreRelease preRelease).ReturnOrThrow(preRelease, nameof(text));
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a pre-release identifier to convert.</param>
        /// <returns>The <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid pre-release identifier.</exception>
        [Pure] public static SemverPreRelease Parse(ReadOnlySpan<char> text)
            => ParseTrimmed(text, false, out SemverPreRelease preRelease).ReturnOrThrow(preRelease, nameof(text));
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a pre-release identifier to convert.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out SemverPreRelease preRelease)
            => ParseTrimmed(text.AsSpan(), false, out preRelease) is SemverErrorCode.Success;
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a pre-release identifier to convert.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out SemverPreRelease preRelease)
            => ParseTrimmed(text, false, out preRelease) is SemverErrorCode.Success;

        /// <summary>
        ///   <para>Converts the specified string representation of a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string containing a pre-release identifier to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid pre-release identifier.</exception>
        [Pure] public static SemverPreRelease Parse(string text, SemverOptions options)
        {
            ANE.ThrowIfNull(text);
            ReadOnlySpan<char> trimmed = Utility.Trim(text.AsSpan(), options);
            bool alz = (options & SemverOptions.AllowLeadingZeroes) != 0;
            SemverErrorCode code = trimmed.Length != text.Length
                ? ParseTrimmed(trimmed, alz, out SemverPreRelease preRelease)
                : ParseTrimmed(text, alz, out preRelease);
            return code.ReturnOrThrow(preRelease, nameof(text));
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a pre-release identifier to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid pre-release identifier.</exception>
        [Pure] public static SemverPreRelease Parse(ReadOnlySpan<char> text, SemverOptions options)
        {
            bool alz = (options & SemverOptions.AllowLeadingZeroes) != 0;
            SemverErrorCode code = ParseTrimmed(Utility.Trim(text, options), alz, out SemverPreRelease preRelease);
            return code.ReturnOrThrow(preRelease, nameof(text));
        }
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a pre-release identifier to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, SemverOptions options, out SemverPreRelease preRelease)
        {
            if (text is not null)
            {
                ReadOnlySpan<char> trimmed = Utility.Trim(text.AsSpan(), options);
                bool alz = (options & SemverOptions.AllowLeadingZeroes) != 0;
                SemverErrorCode code = trimmed.Length != text.Length
                    ? ParseTrimmed(trimmed, alz, out preRelease)
                    : ParseTrimmed(text, alz, out preRelease);
                return code is SemverErrorCode.Success;
            }
            preRelease = default;
            return false;
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a pre-release identifier to an equivalent <see cref="SemverPreRelease"/> structure using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a pre-release identifier to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="preRelease">When this method returns, contains the <see cref="SemverPreRelease"/> structure equivalent to the pre-release identifier specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemverOptions options, out SemverPreRelease preRelease)
        {
            bool alz = (options & SemverOptions.AllowLeadingZeroes) != 0;
            return ParseTrimmed(Utility.Trim(text, options), alz, out preRelease) is SemverErrorCode.Success;
        }

#if NET7_0_OR_GREATER
        [Pure] static SemverPreRelease IParsable<SemverPreRelease>.Parse(string s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool IParsable<SemverPreRelease>.TryParse(string? s, IFormatProvider? _, out SemverPreRelease preRelease)
            => TryParse(s, out preRelease);
        [Pure] static SemverPreRelease ISpanParsable<SemverPreRelease>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool ISpanParsable<SemverPreRelease>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, out SemverPreRelease preRelease)
            => TryParse(s, out preRelease);
#endif

    }
}
