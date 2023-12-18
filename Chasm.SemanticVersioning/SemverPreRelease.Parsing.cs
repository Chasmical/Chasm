﻿using System;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public readonly partial struct SemverPreRelease
    {
        // ReSharper disable once UnusedParameter.Local
        private SemverPreRelease(string validIdentifier, bool _)
            => text = validIdentifier;

        [Pure] private static SemverErrorCode ParseInitial(ReadOnlySpan<char> text, bool allowLeadingZeroes, out int result)
        {
            if (text.IsEmpty) return Util.Fail(SemverErrorCode.PreReleaseEmpty, out result);
            if (Utility.IsNumeric(text))
            {
                if (!allowLeadingZeroes && text[0] == '0' && text.Length > 1)
                    return Util.Fail(SemverErrorCode.PreReleaseLeadingZeroes, out result);
                result = Utility.ParseNonNegativeInt32(text);
                return result == -1 ? SemverErrorCode.PreReleaseTooBig : SemverErrorCode.Success;
            }
            result = -1;
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseTrimmed(ReadOnlySpan<char> text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            SemverErrorCode code = ParseInitial(text, allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return Util.Fail(code, out preRelease);
            if (result == -1)
            {
                if (!Utility.AllValidCharacters(text)) return Util.Fail(SemverErrorCode.PreReleaseInvalid, out preRelease);
                preRelease = new SemverPreRelease(new string(text), default);
            }
            else preRelease = new SemverPreRelease(result);
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseTrimmed(string text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            SemverErrorCode code = ParseInitial(text, allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return Util.Fail(code, out preRelease);
            if (result == -1)
            {
                if (!Utility.AllValidCharacters(text)) return Util.Fail(SemverErrorCode.PreReleaseInvalid, out preRelease);
                preRelease = new SemverPreRelease(text, default);
            }
            else preRelease = new SemverPreRelease(result);
            return SemverErrorCode.Success;
        }
        [Pure] internal static SemverErrorCode ParseValidated(ReadOnlySpan<char> text, bool allowLeadingZeroes, out SemverPreRelease preRelease)
        {
            SemverErrorCode code = ParseInitial(text, allowLeadingZeroes, out int result);
            if (code is not SemverErrorCode.Success) return Util.Fail(code, out preRelease);
            preRelease = result == -1 ? new SemverPreRelease(new string(text), default) : new SemverPreRelease(result);
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
            if (text is null) throw new ArgumentNullException(nameof(text));
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
            => text is null ? Util.Fail(out preRelease) : ParseTrimmed(text, false, out preRelease) is SemverErrorCode.Success;
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
            if (text is null) throw new ArgumentNullException(nameof(text));
            ReadOnlySpan<char> trimmed = Utility.Trim(text, options);
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
            if (text is null) return Util.Fail(out preRelease);
            ReadOnlySpan<char> trimmed = Utility.Trim(text, options);
            bool alz = (options & SemverOptions.AllowLeadingZeroes) != 0;
            SemverErrorCode code = trimmed.Length != text.Length
                ? ParseTrimmed(trimmed, alz, out preRelease)
                : ParseTrimmed(text, alz, out preRelease);
            return code is SemverErrorCode.Success;
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

    }
}
