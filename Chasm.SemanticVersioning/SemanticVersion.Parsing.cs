using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning
{
    public partial class SemanticVersion
#if NET7_0_OR_GREATER
        : ISpanParsable<SemanticVersion>
#endif
    {
        // ReSharper disable once UnusedParameter.Local
        internal SemanticVersion(int major, int minor, int patch,
                                 SemverPreRelease[]? preReleases, string[]? buildMetadata,
                                 ReadOnlyCollection<SemverPreRelease>? preReleasesReadonly,
                                 ReadOnlyCollection<string>? buildMetadataReadonly)
        {
            // Make sure the internal constructor isn't used with invalid parameters
            Debug.Assert(major >= 0);
            Debug.Assert(minor >= 0);
            Debug.Assert(patch >= 0);
            Debug.Assert(Array.TrueForAll(buildMetadata ?? [], static b => Utility.AllValidCharacters(b.AsSpan())));
            Debug.Assert(preReleasesReadonly is null || preReleasesReadonly.Count == (preReleases?.Length ?? 0));
            Debug.Assert(buildMetadataReadonly is null || buildMetadataReadonly.Count == (buildMetadata?.Length ?? 0));

            _major = major;
            _minor = minor;
            _patch = patch;
            _preReleases = preReleases ?? [];
            _buildMetadata = buildMetadata ?? [];
            _preReleasesReadonly = preReleasesReadonly;
            _buildMetadataReadonly = buildMetadataReadonly;
        }

        [Pure] internal static SemverErrorCode ParseStrict(ReadOnlySpan<char> text, out SemanticVersion? version)
        {
            version = null;

            int i = 0, length = text.Length;

            // read all ASCII digits at current position
            while (i < length && (uint)text[i] - '0' <= '9' - '0') i++;
            // if the position is still zero, return MajorNotFound
            if (i == 0) return SemverErrorCode.MajorNotFound;
            // if the first digit is zero, and the position is greater than 1 (position == component length)
            if (text[0] == '0' && i > 1) return SemverErrorCode.MajorLeadingZeroes;
            // try to parse the major component
            if (!Utility.TryParseNonNegativeInt32(text.Slice(0, i), out int major))
                return SemverErrorCode.MajorTooBig;

            // the next character must be a '.'
            if (i >= length || text[i] != '.') return SemverErrorCode.MinorNotFound;

            int start = ++i; // move past '.', and save current position as start
            while (i < length && (uint)text[i] - '0' <= '9' - '0') i++;
            if (i == start) return SemverErrorCode.MinorNotFound;
            if (text[start] == '0' && i - start > 1) return SemverErrorCode.MinorLeadingZeroes;
            // try to parse the minor component
            if (!Utility.TryParseNonNegativeInt32(text.Slice(start, i - start), out int minor))
                return SemverErrorCode.MinorTooBig;

            // the next character must be a '.'
            if (i >= length || text[i] != '.') return SemverErrorCode.PatchNotFound;

            start = ++i; // move past '.', and save current position as start
            while (i < length && (uint)text[i] - '0' <= '9' - '0') i++;
            if (i == start) return SemverErrorCode.PatchNotFound;
            if (text[start] == '0' && i - start > 1) return SemverErrorCode.PatchLeadingZeroes;
            // try to parse the patch component
            if (!Utility.TryParseNonNegativeInt32(text.Slice(start, i - start), out int patch))
                return SemverErrorCode.PatchTooBig;

            SemverPreRelease[]? preReleases = null;
            string[]? buildMetadata = null;

            if (i < length && text[i] == '-')
            {
                List<SemverPreRelease> list = [];
                do
                {
                    start = ++i; // move past '-' or '.', and save current position as start
                    while (i < length && Utility.IsValidCharacter(text[i])) i++;
                    // parse the pre-release identifier
                    SemverErrorCode code = SemverPreRelease.ParseValidated(
                        text.Slice(start, i - start), false, out SemverPreRelease preRelease
                    );
                    // add the identifier to the list, if successful
                    if (code is not SemverErrorCode.Success) return code;
                    list.Add(preRelease);
                }
                while (i < length && text[i] == '.');
                // store the array with the results
                preReleases = list.ToArray();
            }
            if (i < length && text[i] == '+')
            {
                List<string> list = [];
                do
                {
                    start = ++i; // move past '+' or '.', and save current position as start
                    while (i < length && Utility.IsValidCharacter(text[i])) i++;
                    // add the identifier to the list, if not empty
                    if (i == start) return SemverErrorCode.BuildMetadataEmpty;
                    list.Add(text.Slice(start, i - start).ToString());
                }
                while (i < length && text[i] == '.');
                // store the array with the results
                buildMetadata = list.ToArray();
            }

            // if there are still unprocessed characters left, return Leftovers code
            if (i < length) return SemverErrorCode.Leftovers;

            version = new SemanticVersion(major, minor, patch, preReleases, buildMetadata, null, null);
            return SemverErrorCode.Success;
        }

        [Pure] internal static SemverErrorCode ParseLoose(ReadOnlySpan<char> text, SemverOptions options, out SemanticVersion? version)
        {
            SpanParser parser = new SpanParser(text);
            return ParseLoose(ref parser, options, out version);
        }
        [Pure] internal static SemverErrorCode ParseLoose(ref SpanParser parser, SemverOptions options, out SemanticVersion? version)
        {
            version = null;

            if ((options & SemverOptions.AllowLeadingWhite) != 0)
                parser.SkipWhitespaces();

            bool innerWhite = (options & SemverOptions.AllowInnerWhite) != 0;

            if ((options & SemverOptions.AllowEqualsPrefix) != 0 && parser.Skip('='))
            {
                if (innerWhite) parser.SkipWhitespaces();
            }
            if ((options & SemverOptions.AllowVersionPrefix) != 0 && parser.SkipAny('v', 'V'))
            {
                if (innerWhite) parser.SkipWhitespaces();
            }

            bool allowLeadingZeroes = (options & SemverOptions.AllowLeadingZeroes) != 0;

            ReadOnlySpan<char> read = parser.ReadAsciiDigits();
            if (read.IsEmpty) return SemverErrorCode.MajorNotFound;
            if (!allowLeadingZeroes && read[0] == '0' && read.Length > 1) return SemverErrorCode.MajorLeadingZeroes;
            if (!Utility.TryParseNonNegativeInt32(read, out int major))
                return SemverErrorCode.MajorTooBig;
            if (innerWhite) parser.SkipWhitespaces();

            static bool SkipAndWhitespace(ref SpanParser parser, char c, bool skipWhitespace)
            {
                bool res = parser.Skip(c);
                if (res && skipWhitespace) parser.SkipWhitespaces();
                return res;
            }

            int minor = 0;
            int patch = 0;

            if (SkipAndWhitespace(ref parser, '.', innerWhite) && !(read = parser.ReadAsciiDigits()).IsEmpty)
            {
                if (!allowLeadingZeroes && read[0] == '0' && read.Length > 1) return SemverErrorCode.MinorLeadingZeroes;
                if (!Utility.TryParseNonNegativeInt32(read, out minor))
                    return SemverErrorCode.MinorTooBig;
                if (innerWhite) parser.SkipWhitespaces();

                if (SkipAndWhitespace(ref parser, '.', innerWhite) && !(read = parser.ReadAsciiDigits()).IsEmpty)
                {
                    if (!allowLeadingZeroes && read[0] == '0' && read.Length > 1) return SemverErrorCode.PatchLeadingZeroes;
                    if (!Utility.TryParseNonNegativeInt32(read, out patch))
                        return SemverErrorCode.PatchTooBig;
                    if (innerWhite) parser.SkipWhitespaces();

                }
                else if ((options & SemverOptions.OptionalPatch) == 0)
                    return SemverErrorCode.PatchNotFound;

            }
            else if ((options & SemverOptions.OptionalMinor) == 0)
                return SemverErrorCode.MinorNotFound;

            SemverPreRelease[]? preReleases = null;
            if (parser.Skip('-'))
            {
                bool removeEmpty = (options & SemverOptions.RemoveEmptyPreReleases) != 0;
                List<SemverPreRelease> list = [];
                do
                {
                    if (innerWhite) parser.SkipWhitespaces();
                    unsafe { read = parser.ReadWhile(&Utility.IsValidCharacter); }
                    if (read.IsEmpty)
                    {
                        if (removeEmpty) continue;
                        return SemverErrorCode.PreReleaseEmpty;
                    }
                    SemverErrorCode code = SemverPreRelease.ParseValidated(read, allowLeadingZeroes, out SemverPreRelease preRelease);
                    if (code is not SemverErrorCode.Success) return code;
                    list.Add(preRelease);
                    if (innerWhite) parser.SkipWhitespaces();
                }
                while (parser.Skip('.'));
                preReleases = list.ToArray();
            }
            else if ((options & SemverOptions.OptionalPreReleaseSeparator) != 0
                  && parser.TryPeek(out char next) && Utility.IsValidCharacter(next))
            {
                List<SemverPreRelease> list = [];
                do
                {
                    bool isDigit = (uint)next - '0' < 10u;
                    if (next == '-') return SemverErrorCode.PreReleaseInvalid;
                    read = isDigit ? parser.ReadAsciiDigits() : parser.ReadAsciiLetters();
                    SemverErrorCode code = SemverPreRelease.ParseValidated(read, allowLeadingZeroes, out SemverPreRelease preRelease);
                    if (code is not SemverErrorCode.Success) return code;
                    list.Add(preRelease);
                    if (innerWhite) parser.SkipWhitespaces();
                }
                while (parser.TryPeek(out next) && Utility.IsValidCharacter(next));
                preReleases = list.ToArray();
            }

            string[]? buildMetadata = null;
            if (parser.Skip('+'))
            {
                bool removeEmpty = (options & SemverOptions.RemoveEmptyBuildMetadata) != 0;
                List<string> list = [];
                do
                {
                    if (innerWhite) parser.SkipWhitespaces();
                    unsafe { read = parser.ReadWhile(&Utility.IsValidCharacter); }
                    if (read.IsEmpty)
                    {
                        if (removeEmpty) continue;
                        return SemverErrorCode.BuildMetadataEmpty;
                    }
                    list.Add(read.ToString());
                    if (innerWhite) parser.SkipWhitespaces();
                }
                while (parser.Skip('.'));
                buildMetadata = list.ToArray();
            }

            if ((options & SemverOptions.AllowTrailingWhite) != 0)
            {
                if (!innerWhite) parser.SkipWhitespaces();
            }
            else if (innerWhite)
                parser.UndoSkippingWhitespace();

            if ((options & SemverOptions.AllowLeftovers) == 0 && parser.CanRead())
                return SemverErrorCode.Leftovers;

            version = new SemanticVersion(major, minor, patch, preReleases, buildMetadata, null, null);
            return SemverErrorCode.Success;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> instance.</para>
        /// </summary>
        /// <param name="text">The string containing a semantic version to convert.</param>
        /// <returns>The <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid semantic version.</exception>
        [Pure] public static SemanticVersion Parse(string text)
            => text is null ? throw new ArgumentNullException(nameof(text)) : Parse(text.AsSpan());
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a semantic version to an equivalent <see cref="SemanticVersion"/> instance.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a semantic version to convert.</param>
        /// <returns>The <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid semantic version.</exception>
        [Pure] public static SemanticVersion Parse(ReadOnlySpan<char> text)
            => ParseStrict(text, out SemanticVersion? version).ReturnOrThrow(version, nameof(text));
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a semantic version to convert.</param>
        /// <param name="version">When this method returns, contains the <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(text.AsSpan(), out version);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a semantic version to an equivalent <see cref="SemanticVersion"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a semantic version to convert.</param>
        /// <param name="version">When this method returns, contains the <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out SemanticVersion? version)
            => ParseStrict(text, out version) is SemverErrorCode.Success;

        /// <summary>
        ///   <para>Converts the specified string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string containing a semantic version to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid semantic version.</exception>
        [Pure] public static SemanticVersion Parse(string text, SemverOptions options)
            => text is null ? throw new ArgumentNullException(nameof(text)) : Parse(text.AsSpan(), options);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a semantic version to an equivalent <see cref="SemanticVersion"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a semantic version to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <returns>The <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid semantic version.</exception>
        [Pure] public static SemanticVersion Parse(ReadOnlySpan<char> text, SemverOptions options)
        {
            if (options is SemverOptions.Strict) return Parse(text);
            return ParseLoose(text, options, out SemanticVersion? version).ReturnOrThrow(version, nameof(text));
        }
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a semantic version to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="version">When this method returns, contains the <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, SemverOptions options, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(text.AsSpan(), options, out version);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a semantic version to an equivalent <see cref="SemanticVersion"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a semantic version to convert.</param>
        /// <param name="options">The semantic version parsing options to use.</param>
        /// <param name="version">When this method returns, contains the <see cref="SemanticVersion"/> instance equivalent to the semantic version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemverOptions options, [NotNullWhen(true)] out SemanticVersion? version)
        {
            if (options is SemverOptions.Strict) return TryParse(text, out version);
            return ParseLoose(text, options, out version) is SemverErrorCode.Success;
        }

#if NET7_0_OR_GREATER
        [Pure] static SemanticVersion IParsable<SemanticVersion>.Parse(string s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool IParsable<SemanticVersion>.TryParse(string? s, IFormatProvider? _, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(s, out version);
        [Pure] static SemanticVersion ISpanParsable<SemanticVersion>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool ISpanParsable<SemanticVersion>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, [NotNullWhen(true)] out SemanticVersion? version)
            => TryParse(s, out version);
#endif

    }
}
