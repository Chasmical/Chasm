using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        internal SemanticVersion(int major, int minor, int patch, SemverPreRelease[]? preReleases, string[]? buildMetadata, bool _)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            _preReleases = preReleases ?? [];
            _buildMetadata = buildMetadata ?? [];
        }

        [Pure] internal static SemverErrorCode ParseStrict(ReadOnlySpan<char> text, out SemanticVersion? version)
        {
            SpanParser parser = new SpanParser(text);
            return ParseStrict(ref parser, out version);
        }
        [Pure] internal static SemverErrorCode ParseStrict(ref SpanParser parser, out SemanticVersion? version)
        {
            version = null;

            ReadOnlySpan<char> read = parser.ReadAsciiDigits();
            if (read.IsEmpty) return SemverErrorCode.MajorNotFound;
            if (read[0] == '0' && read.Length > 1) return SemverErrorCode.MajorLeadingZeroes;
            if (!int.TryParse(read, NumberStyles.None, null, out int major))
                return SemverErrorCode.MajorTooBig;

            if (!parser.Skip('.')) return SemverErrorCode.MinorNotFound;

            read = parser.ReadAsciiDigits();
            if (read.IsEmpty) return SemverErrorCode.MinorNotFound;
            if (read[0] == '0' && read.Length > 1) return SemverErrorCode.MinorLeadingZeroes;
            if (!int.TryParse(read, NumberStyles.None, null, out int minor))
                return SemverErrorCode.MinorTooBig;

            if (!parser.Skip('.')) return SemverErrorCode.PatchNotFound;

            read = parser.ReadAsciiDigits();
            if (read.IsEmpty) return SemverErrorCode.PatchNotFound;
            if (read[0] == '0' && read.Length > 1) return SemverErrorCode.PatchLeadingZeroes;
            if (!int.TryParse(read, NumberStyles.None, null, out int patch))
                return SemverErrorCode.PatchTooBig;

            SemverPreRelease[]? preReleases = null;
            string[]? buildMetadata = null;

            if (parser.Skip('-'))
            {
                List<SemverPreRelease> list = [];
                do
                {
                    read = parser.ReadSemverIdentifier();
                    SemverErrorCode code = SemverPreRelease.ParseValidated(read, false, out SemverPreRelease preRelease);
                    if (code is not SemverErrorCode.Success) return code;
                    list.Add(preRelease);
                }
                while (parser.Skip('.'));
                preReleases = list.ToArray();
            }
            if (parser.Skip('+'))
            {
                List<string> list = [];
                do
                {
                    read = parser.ReadSemverIdentifier();
                    if (read.IsEmpty) return SemverErrorCode.BuildMetadataEmpty;
                    list.Add(new string(read));
                }
                while (parser.Skip('.'));
                buildMetadata = list.ToArray();
            }

            if (parser.CanRead()) return SemverErrorCode.Leftovers;

            version = new SemanticVersion(major, minor, patch, preReleases, buildMetadata, default);
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

            throw new NotImplementedException();
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
