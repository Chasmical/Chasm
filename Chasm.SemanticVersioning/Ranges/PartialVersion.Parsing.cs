using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public sealed partial class PartialVersion
#if NET7_0_OR_GREATER
        : ISpanParsable<PartialVersion>
#endif
    {
        // ReSharper disable once UnusedParameter.Local
        internal PartialVersion(PartialComponent major, PartialComponent minor, PartialComponent patch,
                                SemverPreRelease[]? preReleases, string[]? buildMetadata,
                                ReadOnlyCollection<SemverPreRelease>? preReleasesReadonly,
                                ReadOnlyCollection<string>? buildMetadataReadonly)
        {
            // Make sure the internal constructor isn't used with invalid parameters
            Debug.Assert((int)major._value is >= 0 or -1 or -'x' or -'X' or -'*');
            Debug.Assert((int)minor._value is >= 0 or -1 or -'x' or -'X' or -'*');
            Debug.Assert((int)patch._value is >= 0 or -1 or -'x' or -'X' or -'*');
            Debug.Assert(Array.TrueForAll(buildMetadata ?? [], static b => Utility.AllValidCharacters(b)));
            Debug.Assert(preReleasesReadonly is null || preReleasesReadonly.Count == (preReleases?.Length ?? 0));
            Debug.Assert(buildMetadataReadonly is null || buildMetadataReadonly.Count == (buildMetadata?.Length ?? 0));

            Major = major;
            Minor = minor;
            Patch = patch;
            _preReleases = preReleases ?? [];
            _buildMetadata = buildMetadata ?? [];
            _preReleasesReadonly = preReleasesReadonly;
            _buildMetadataReadonly = buildMetadataReadonly;
        }

        [Pure] internal static SemverErrorCode ParseLoose(ReadOnlySpan<char> text, SemverOptions options, out PartialVersion? partial)
        {
            SpanParser parser = new SpanParser(text);
            return ParseLoose(ref parser, options, out partial);
        }
        [Pure] internal static SemverErrorCode ParseLoose(ref SpanParser parser, SemverOptions options, out PartialVersion? partial)
        {
            partial = null;

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

            ReadOnlySpan<char> read = Utility.ReadPartialComponent(ref parser);
            if (read.IsEmpty) return SemverErrorCode.MajorNotFound;

            SemverErrorCode code = PartialComponent.ParseTrimmed(read, options, out PartialComponent major);
            if (code is not SemverErrorCode.Success) return code | SemverErrorCode.MAJOR;
            if (innerWhite) parser.SkipWhitespaces();

            PartialComponent minor = PartialComponent.Omitted;
            PartialComponent patch = PartialComponent.Omitted;

            bool allowIdentifiers = false;

            if (parser.Skip('.'))
            {
                if (innerWhite) parser.SkipWhitespaces();
                read = Utility.ReadPartialComponent(ref parser);
                if (!read.IsEmpty)
                {
                    code = PartialComponent.ParseTrimmed(read, options, out minor);
                    if (code is not SemverErrorCode.Success) return code | SemverErrorCode.MINOR;
                    if (innerWhite) parser.SkipWhitespaces();

                    if (parser.Skip('.'))
                    {
                        if (innerWhite) parser.SkipWhitespaces();
                        read = Utility.ReadPartialComponent(ref parser);
                        if (!read.IsEmpty)
                        {
                            code = PartialComponent.ParseTrimmed(read, options, out patch);
                            if (code is not SemverErrorCode.Success) return code | SemverErrorCode.PATCH;
                            if (innerWhite) parser.SkipWhitespaces();
                            allowIdentifiers = true;
                        }
                        else if ((options & SemverOptions.OptionalPatch) == 0)
                            return SemverErrorCode.PatchNotFound;
                    }
                }
                else if ((options & SemverOptions.OptionalMinor) == 0)
                    return SemverErrorCode.MinorNotFound;
            }

            SemverPreRelease[]? preReleases = null;

            if (parser.Skip('-'))
            {
                if (!allowIdentifiers) return SemverErrorCode.PreReleaseAfterOmitted;

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
                    code = SemverPreRelease.ParseValidated(read, allowLeadingZeroes, out SemverPreRelease preRelease);
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
                if (!allowIdentifiers) return SemverErrorCode.PreReleaseAfterOmitted;

                List<SemverPreRelease> list = [];
                do
                {
                    bool isDigit = (uint)next - '0' < 10u;
                    if (next == '-') return SemverErrorCode.PreReleaseInvalid;
                    read = isDigit ? parser.ReadAsciiDigits() : parser.ReadAsciiLetters();
                    code = SemverPreRelease.ParseValidated(read, allowLeadingZeroes, out SemverPreRelease preRelease);
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
                if (!allowIdentifiers) return SemverErrorCode.BuildMetadataAfterOmitted;

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
                    list.Add(new string(read));
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

            partial = new PartialVersion(major, minor, patch, preReleases, buildMetadata, null, null);
            return SemverErrorCode.Success;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> instance.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version to convert.</param>
        /// <returns>The <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(string text)
            => Parse(text, SemverOptions.Strict);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> instance.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version to convert.</param>
        /// <returns>The <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(ReadOnlySpan<char> text)
            => Parse(text, SemverOptions.Strict);
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version to convert.</param>
        /// <param name="partial">When this method returns, contains the <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, [NotNullWhen(true)] out PartialVersion? partial)
            => TryParse(text, SemverOptions.Strict, out partial);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> instance, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version to convert.</param>
        /// <param name="partial">When this method returns, contains the <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out PartialVersion? partial)
            => TryParse(text, SemverOptions.Strict, out partial);

        /// <summary>
        ///   <para>Converts the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version to convert.</param>
        /// <param name="options">The partial version parsing options to use.</param>
        /// <returns>The <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(string text, SemverOptions options)
            => text is null ? throw new ArgumentNullException(nameof(text)) : Parse(text.AsSpan(), options);
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> instance, using the specified parsing <paramref name="options"/>.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version to convert.</param>
        /// <param name="options">The partial version parsing options to use.</param>
        /// <returns>The <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid partial version.</exception>
        [Pure] public static PartialVersion Parse(ReadOnlySpan<char> text, SemverOptions options)
            => ParseLoose(text, options, out PartialVersion? partial).ReturnOrThrow(partial, nameof(text));
        /// <summary>
        ///   <para>Tries to convert the specified string representation of a partial version to an equivalent <see cref="PartialVersion"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a partial version to convert.</param>
        /// <param name="options">The partial version parsing options to use.</param>
        /// <param name="partial">When this method returns, contains the <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, SemverOptions options, [NotNullWhen(true)] out PartialVersion? partial)
            => TryParse(text.AsSpan(), options, out partial);
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a partial version to an equivalent <see cref="PartialVersion"/> instance, using the specified parsing <paramref name="options"/>, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a partial version to convert.</param>
        /// <param name="options">The partial version parsing options to use.</param>
        /// <param name="partial">When this method returns, contains the <see cref="PartialVersion"/> instance equivalent to the partial version specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="null"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, SemverOptions options, [NotNullWhen(true)] out PartialVersion? partial)
            => ParseLoose(text, options, out partial) is SemverErrorCode.Success;

#if NET7_0_OR_GREATER
        [Pure] static PartialVersion IParsable<PartialVersion>.Parse(string s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool IParsable<PartialVersion>.TryParse(string? s, IFormatProvider? _, [NotNullWhen(true)] out PartialVersion? partial)
            => TryParse(s, out partial);
        [Pure] static PartialVersion ISpanParsable<PartialVersion>.Parse(ReadOnlySpan<char> s, IFormatProvider? _)
            => Parse(s);
        [Pure] static bool ISpanParsable<PartialVersion>.TryParse(ReadOnlySpan<char> s, IFormatProvider? _, [NotNullWhen(true)] out PartialVersion? partial)
            => TryParse(s, out partial);
#endif

    }
}
